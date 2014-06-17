using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using ZeroMQ;
using EVEMarketWatch.Utils;

namespace EVEMarketWatch
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = ZmqContext.Create())
            {
                ConcurrentQueue<string> messages = new ConcurrentQueue<string>();

                Thread thread = new Thread(() =>
                {
                    using (var subscriber = context.CreateSocket(SocketType.SUB))
                    {
                        //Connect to the first publicly available relay.
                        subscriber.Connect("tcp://relay-us-central-1.eve-emdr.com:8050");

                        // Alternatively 'Subscribe' can be used
                        subscriber.Subscribe(new byte[0]);

                        while (true)
                        {
                            try
                            {
                                // Receive compressed raw market data.
                                int size = 0;
                                byte[] receivedData = new byte[65535];
                                subscriber.Receive(receivedData, out size);

                                // The following code lines remove the need of 'zlib' usage;
                                // 'zlib' actually uses the same algorith as 'DeflateStream'.
                                // To make the data compatible for 'DeflateStream', we only have to remove
                                // the four last bytes which are the adler32 checksum and
                                // the two first bytes which are the 'zlib' header.
                                byte[] decompressed;
                                byte[] choppedRawData = new byte[(size - 4)];
                                Array.Copy(receivedData, choppedRawData, choppedRawData.Length);
                                choppedRawData = choppedRawData.Skip(2).ToArray();

                                // Decompress the raw market data.
                                using (MemoryStream inStream = new MemoryStream(choppedRawData))
                                using (MemoryStream outStream = new MemoryStream())
                                {
                                    DeflateStream outZStream = new DeflateStream(inStream, CompressionMode.Decompress);
                                    outZStream.CopyTo(outStream);
                                    decompressed = outStream.ToArray();
                                }

                                // Transform data into JSON strings.
                                string marketJson = Encoding.UTF8.GetString(decompressed);

                                var obj = JsonConvert.DeserializeObject<DataInterchange>(marketJson);

                                var orders = obj.ConvertToOrders();

                                foreach (var order in orders)
                                {
                                    Console.WriteLine(order.price);
                                }
                            }
                            catch (ZmqException ex)
                            {
                                Console.WriteLine("ZMQ Exception occurred : {0}", ex.Message);
                            }
                        }
                    }
                });

                thread.Start();

                while (true)
                {
                    string value;
                    if (messages.TryDequeue(out value))
                    {
                        Console.WriteLine(value);
                    }
                }
            }

        }
    }
}

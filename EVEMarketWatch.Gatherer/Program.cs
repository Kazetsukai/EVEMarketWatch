using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;
using ZeroMQ;
using EVEMarketWatch.Utils;
using EVEMarketWatch.Core.Domain;
using EVEMarketWatch.Core;

namespace EVEMarketWatch
{
    class Program
    {
        private static readonly List<MapService> Services = new List<MapService>();

        static void Main(string[] args)
        {
            var incomingOrders = new ConcurrentQueue<Order>();

            var receiveThread = new Thread(() => ReceiveOrders(incomingOrders));
            receiveThread.Start();

            var db = new OrderStorage();

            var wssv = new WebSocketServer("ws://localhost:8088");

            wssv.AddWebSocketService<MapService>("/eve");
            wssv.Start();
            
            while (true)
            {
                SaveToDatabase(incomingOrders, db);
            }

            wssv.Stop();
        }

        private static void SaveToDatabase(ConcurrentQueue<Order> incomingOrders, OrderStorage db)
        {
            var orderList = new List<Order>();

            orderList.Clear();
            Order order;
            while (incomingOrders.TryDequeue(out order))
            {
                orderList.Add(order);

                foreach (var service in Services) //TODO: this is not thread safe
                    service.SendIt(order.solarSystemID.ToString());
            }

            db.AddOrders(orderList);
            //var orders = db.Orders;

            //Console.WriteLine(orders.Average(o => o.price) + "  - " + orders.Count());

            if (orderList.Count > 0) 
                Console.WriteLine(orderList.Count + " processed...");
        }

        private static void ReceiveOrders(ConcurrentQueue<Order> incomingOrders)
        {
            using (var context = ZmqContext.Create())
            {
                using (var subscriber = context.CreateSocket(ZeroMQ.SocketType.SUB))
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
                            var size = 0;
                            var receivedData = new byte[65535];
                            subscriber.Receive(receivedData, out size);

                            // The following code lines remove the need of 'zlib' usage;
                            // 'zlib' actually uses the same algorith as 'DeflateStream'.
                            // To make the data compatible for 'DeflateStream', we only have to remove
                            // the four last bytes which are the adler32 checksum and
                            // the two first bytes which are the 'zlib' header.
                            byte[] decompressed;
                            var choppedRawData = new byte[(size - 4)];
                            Array.Copy(receivedData, choppedRawData, choppedRawData.Length);
                            choppedRawData = choppedRawData.Skip(2).ToArray();

                            // Decompress the raw market data.
                            using (var inStream = new MemoryStream(choppedRawData))
                            using (var outStream = new MemoryStream())
                            {
                                var outZStream = new DeflateStream(inStream, CompressionMode.Decompress);
                                outZStream.CopyTo(outStream);
                                decompressed = outStream.ToArray();
                            }

                            // Transform data into JSON strings.
                            var marketJson = Encoding.UTF8.GetString(decompressed);

                            var obj = JsonConvert.DeserializeObject<DataInterchange>(marketJson);

                            var orders = obj.ConvertToOrders();

                            foreach (var order in orders)
                            {
                                incomingOrders.Enqueue(order);
                            }
                        }
                        catch (ZmqException ex)
                        {
                            Console.WriteLine("ZMQ Exception occurred : {0}", ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// When this class is automatically instantiated by the websocketserver
        /// it will add itself to a global list. Use 'SendIt' to send a string
        /// to connected clients
        /// </summary>
        public class MapService : WebSocketService
        {
            public bool Connected { get; set; }

            protected override void OnOpen()
            {
                Connected = true;
                Services.Add(this);

                base.OnOpen();
            }

            protected override void OnClose(CloseEventArgs e)
            {
                Connected = false;
                Services.Remove(this);

                base.OnClose(e);
            }

            public void SendIt(string message)
            {
                if (Connected)
                    Send(message);
            }
        }
    }
}

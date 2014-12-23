using EVEMarketWatch.Core.Database;
using EVEMarketWatch.Core.Database.Repository;
using EVEMarketWatch.Core.StaticData;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using Ninject;
using WebSocketSharp;
using WebSocketSharp.Server;
using ZeroMQ;
using EVEMarketWatch.Utils;
using EVEMarketWatch.Core.Domain;

namespace EVEMarketWatch
{
    class Program
    {
        private static readonly List<MapService> Services = new List<MapService>();
        private static IDictionary<int, InventoryType> _invTypes;

        static void Main(string[] args)
        {
            //start ninject
            var kernel = new StandardKernel();
            //kernel.Load(Assembly.GetExecutingAssembly());

            //start the db
            var db = new ConfigureDatabase();
            var sessionFactory = db.Create(kernel);

            _invTypes = InventoryType.GetAll();

            var incomingOrders = new ConcurrentQueue<DataInterchange>();

            var receiveThread = new Thread(() => ReceiveOrders(incomingOrders));
            receiveThread.Start();

            //var expired = new OrderQuery(sessionFactory).GetExpiredOrders();
            //var plex = new OrderQuery(sessionFactory).GetOrdersOfTypeId(29668).Where(x => x.IsSell()).OrderByDescending(x => x.price);

            using (var session = sessionFactory.OpenSession())
            {
                var orderRepo = new OrderRepository(session);

                while (true)
                {
                    SaveToDatabase(incomingOrders, orderRepo);
                }
            }
        }

        private class ClientTransmission
        {
            public ClientTransmission(Order order)
            {
                System = order.solarSystemID.ToString("N0").Replace(",", "");
                Type = _invTypes[order.typeID].TypeName;
            }

            public string System { get; set; }
            public string Type { get; set; }
        }

        private static void SaveToDatabase(ConcurrentQueue<DataInterchange> incomingData, OrderRepository db)
        {
            var orderList = new List<Order>();

            orderList.Clear();
            DataInterchange data;
            while (incomingData.TryDequeue(out data))
            {
                orderList = data.ConvertToOrders();

                /*                var container = new ClientTransmission(order);

                                var jsonString = JsonConvert.SerializeObject(container);

                                foreach (var service in Services) //TODO: this is not thread safe
                                    service.SendIt(jsonString);
                                    //service.SendIt(order.solarSystemID.ToString());*/
            }

            db.AddOrders(orderList);
            //var orders = db.Orders;

            //Console.WriteLine(orders.Average(o => o.price) + "  - " + orders.Count());

            if (!orderList.Any())
                return;

            Console.WriteLine(_invTypes.ContainsKey(orderList.First().typeID) ? _invTypes[orderList.First().typeID].TypeName : "***UNKNOWN***");
        }

        private static void ReceiveOrders(ConcurrentQueue<DataInterchange> incomingData)
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

                            incomingData.Enqueue(obj);

                            /*                            var orders = obj.ConvertToOrders();

                                                        foreach (var order in orders)
                                                        {
                                                            incomingData.Enqueue(order);
                                                        }*/
                        }
                        catch (Exception ex)
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

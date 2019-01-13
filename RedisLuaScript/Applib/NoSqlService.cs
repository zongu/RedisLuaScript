
namespace RedisLuaScript.Applib
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using RedLock;
    using StackExchange.Redis;

    internal static class NoSqlService
    {
        private static Lazy<ConnectionMultiplexer> lazyRedisConnections;

        public static ConnectionMultiplexer RedisConnections
        {
            get
            {
                if(lazyRedisConnections == null)
                {
                    NoSqlInit();
                }

                return lazyRedisConnections.Value;
            }
        }

        private static Lazy<RedisLockFactory> lazyDistributedLockService;

        public static RedisLockFactory DistributedLockService
        {
            get
            {
                if (lazyDistributedLockService == null)
                {
                    NoSqlInit();
                }

                return lazyDistributedLockService.Value;
            }
        }

        public static string RedisAffixKey
        {
            get
            {
                return "RedisLuaScript";
            }
        }

        private static void NoSqlInit()
        {
            lazyRedisConnections = new Lazy<ConnectionMultiplexer>(() =>
            {
                var options = ConfigurationOptions.Parse(ConfigHelper.RedisConn);
                options.AbortOnConnectFail = false;

                var muxer = ConnectionMultiplexer.Connect(options);
                muxer.ConnectionFailed += (sender, e) =>
                {
                    Console.WriteLine("redis failed: " + EndPointCollection.ToString(e.EndPoint) + "/" + e.ConnectionType);
                };
                muxer.ConnectionRestored += (sender, e) =>
                {
                    Console.WriteLine("redis restored: " + EndPointCollection.ToString(e.EndPoint) + "/" + e.ConnectionType);
                };

                return muxer;
            });

            lazyDistributedLockService = new Lazy<RedisLockFactory>(() =>
            {
                string[] endPointStrings = ConfigHelper.RedisDistributedLockConn.Split(',');
                var endPoints = new List<RedisLockEndPoint>();
                foreach (var endPointString in endPointStrings)
                {
                    int splitIndex = endPointString.IndexOf(':');
                    string hostName = endPointString.Substring(0, splitIndex);
                    int port = int.Parse(endPointString.Substring(splitIndex + 1));
                    endPoints.Add(new RedisLockEndPoint
                    {
                        ConnectionTimeout = 5000,
                        EndPoint = new DnsEndPoint(hostName, port),
                    });
                }
                return new RedisLockFactory(endPoints);
            });
        }
    }
}

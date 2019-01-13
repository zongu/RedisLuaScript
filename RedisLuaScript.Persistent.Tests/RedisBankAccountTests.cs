
namespace RedisLuaScript.Persistent.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RedisLuaScript.Domain.Model;
    using RedisLuaScript.Domain.Repository;
    using StackExchange.Redis;

    [TestClass]
    public class RedisBankAccountTests
    {
        private IBankAccountRespoitory repo;

        [TestInitialize]
        public void Init()
        {
            var lazyRedisConnections = new Lazy<ConnectionMultiplexer>(() =>
            {
                var options = ConfigurationOptions.Parse("localhost:6379");
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

            this.repo = new RedisBankAccountRespoitory(lazyRedisConnections.Value, "RedisLuaScriptTest");
        }

        [TestMethod]
        public void AddTest()
        {
            var result = this.repo.AddIfNotExist(new BankAccount()
            {
                BankAccountId = new Random(Guid.NewGuid().GetHashCode()).Next(100000000),
                Amount = 0,
                TransactionCount = 0
            });

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FindTest()
        {
            var bankAccountId = new Random(Guid.NewGuid().GetHashCode()).Next(100000000);

            var addResult = this.repo.AddIfNotExist(new BankAccount()
            {
                BankAccountId = bankAccountId,
                Amount = 0,
                TransactionCount = 0
            });

            Assert.IsTrue(addResult);

            var findResult = this.repo.Find(bankAccountId);
            Assert.IsNotNull(findResult);
            Assert.AreEqual(findResult.BankAccountId, bankAccountId);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var bankAccountId = new Random(Guid.NewGuid().GetHashCode()).Next(100000000);

            var addResult = this.repo.AddIfNotExist(new BankAccount()
            {
                BankAccountId = bankAccountId,
                Amount = 0,
                TransactionCount = 0
            });

            Assert.IsTrue(addResult);

            var updateResult = this.repo.UpdateIfExist(bankAccountId, 10);
            Assert.IsTrue(updateResult);

            var findResult = this.repo.Find(bankAccountId);
            Assert.IsNotNull(findResult);
            Assert.AreEqual(findResult.BankAccountId, bankAccountId);
            Assert.AreEqual(10, findResult.Amount);
            Assert.AreEqual(1, findResult.TransactionCount);

            var updateResult2 = this.repo.UpdateIfExist(bankAccountId, 10);
            Assert.IsTrue(updateResult2);

            findResult = this.repo.Find(bankAccountId);
            Assert.IsNotNull(findResult);
            Assert.AreEqual(findResult.BankAccountId, bankAccountId);
            Assert.AreEqual(20, findResult.Amount);
            Assert.AreEqual(2, findResult.TransactionCount);
        }
    }
}

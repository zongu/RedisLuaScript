
namespace RedisLuaScript.Persistent
{
    using System;
    using System.Collections.Generic;
    using RedisLuaScript.Domain.Model;
    using RedisLuaScript.Domain.Repository;
    using StackExchange.Redis;

    public class RedisBankAccountRespoitory : IBankAccountRespoitory
    {
        private string affixKey;

        private ConnectionMultiplexer conn;

        public RedisBankAccountRespoitory(ConnectionMultiplexer conn, string affixKey)
        {
            this.affixKey = affixKey;
            this.conn = conn;
        }

        public bool AddIfNotExist(BankAccount bankAccount)
        {
            return UseConnection(redis =>
            {
                var keys = new RedisKey[] { $"{this.affixKey}:{bankAccount.BankAccountId}" };
                var values = new RedisValue[]
                {
                    bankAccount.BankAccountId,
                    bankAccount.Amount,
                    bankAccount.TransactionCount
                };

                string script =
                @"
                    if tonumber(redis.call('EXISTS', KEYS[1])) == 1  then return 0 end
                    redis.call('HMSET', KEYS[1]
                    , 'BankAccountId', ARGV[1]
                    , 'Amount', ARGV[2]
                    , 'TransactionCount', ARGV[3])
                    redis.call('EXPIRE', KEYS[1], 3600)
                    return 1";

                //// redis.call('EXPIRE', KEYS[1], 3600) 3600 is TTL
                
                return (bool)redis.ScriptEvaluate(script, keys, values);
            });
        }

        public BankAccount Find(int bankAccountId)
        {
            return UseConnection(redis =>
            {
                var key = $"{this.affixKey}:{bankAccountId}";
                var entrys = redis.HashGetAll(key);

                if(entrys.Length > 0)
                {
                    var dic = new Dictionary<string, RedisValue>();
                    foreach(var entry in entrys)
                    {
                        dic.Add(entry.Name, entry.Value);
                    }

                    return new BankAccount()
                    {
                        BankAccountId = Convert.ToInt32(dic["BankAccountId"]),
                        Amount = Convert.ToInt64(dic["Amount"]),
                        TransactionCount = Convert.ToInt32(dic["TransactionCount"])
                    };
                }

                return null;
            });
        }

        public bool UpdateIfExist(int bankAccountId, long amount)
        {
            return UseConnection(redis =>
            {
                var keys = new RedisKey[] { $"{this.affixKey}:{bankAccountId}" };
                var values = new RedisValue[]
                {
                    amount
                };

                string script =
                @"
                    if tonumber(redis.call('EXISTS', KEYS[1])) == 0  then return 0 end
                    redis.call('HINCRBY', KEYS[1]
                    , 'Amount', ARGV[1])
                    redis.call('HINCRBY', KEYS[1]
                    , 'TransactionCount', 1)
                    return 1";

                return (bool)redis.ScriptEvaluate(script, keys, values);
            });
        }

        private T UseConnection<T>(Func<IDatabase, T> func)
        {
            var redis = conn.GetDatabase(15);
            return func(redis);
        }
    }
}


namespace RedisLuaScript.Applib
{
    using System;

    internal class RedisLockHelper
    {
        public static RedLock.IRedisLock GrabLock(int lockIndex, TimeSpan ttl, TimeSpan waitTime, TimeSpan retryTime)
        {
            string key = $"{NoSqlService.RedisAffixKey}RedisLockHelper:{lockIndex}";
            return NoSqlService.DistributedLockService.Create(key, ttl, waitTime, retryTime);
        }

        public static RedLock.IRedisLock GrabLock(int lockIndex)
        {
            return GrabLock(lockIndex, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(300));
        }
    }
}

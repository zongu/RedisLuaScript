
namespace RedisLuaScript.Domain.Model
{
    using Newtonsoft.Json;

    public class BankAccount
    {
        public int BankAccountId { get; set; }

        public long Amount { get; set; }

        public int TransactionCount { get; set; }

        public override string ToString()
            => JsonConvert.SerializeObject(this);
    }
}

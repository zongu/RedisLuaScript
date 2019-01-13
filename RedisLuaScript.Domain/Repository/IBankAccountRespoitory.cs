
namespace RedisLuaScript.Domain.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using RedisLuaScript.Domain.Model;

    public interface IBankAccountRespoitory
    {
        bool AddIfNotExist(BankAccount bankAccount);

        bool UpdateIfExist(int bankAccountId, long amount);

        BankAccount Find(int bankAccountId);
    }
}

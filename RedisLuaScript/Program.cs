
namespace RedisLuaScript
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Autofac;
    using RedisLuaScript.Domain.Model;
    using RedisLuaScript.Domain.Repository;

    class Program
    {
        const int people = 10;

        const int transationCount = 100;

        const int RandomIncAmountMaxmum = 100000000;

        static void Main(string[] args)
        {
            Console.WriteLine($"Process Start People:{people}, TransationCount:{transationCount}");
            using (var scope = Applib.AutofacConfig.Container.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IBankAccountRespoitory>();
                var bankAccountIds = Generate(repo);
                Transation(repo, bankAccountIds.ToArray());
                Console.WriteLine("Process End");
                Console.Read();
            }

        }

        private static IEnumerable<int> Generate (IBankAccountRespoitory repo)
        {
            List<int> bankAccountIds = new List<int>();
            foreach(var index in Enumerable.Range(0, people))
            {
                var bankAccountId = new Random(Guid.NewGuid().GetHashCode()).Next(100000000);
                var account = new BankAccount()
                {
                    BankAccountId = bankAccountId,
                    Amount = 0,
                    TransactionCount = 0
                };

                repo.AddIfNotExist(account);
                Console.WriteLine($"Add Account: {account.ToString()}");
                bankAccountIds.Add(bankAccountId);
            }

            return bankAccountIds;
        }

        private static void Transation(IBankAccountRespoitory repo, int[] bankAccountIds)
        {
            foreach(var index in Enumerable.Range(0, transationCount))
            {
                var ranNo = new Random(Guid.NewGuid().GetHashCode()).Next(bankAccountIds.Length);
                var ranAmount = new Random(Guid.NewGuid().GetHashCode()).Next(RandomIncAmountMaxmum);
                var bankAccountId = bankAccountIds[ranNo];

                repo.UpdateIfExist(bankAccountId, ranAmount);
                var data = repo.Find(bankAccountId);
                Console.WriteLine($"Transation Data:{data.ToString()}");
            }
        }
    }
}


namespace RedisLuaScript.Applib
{
    using Autofac;
    using RedisLuaScript.Domain.Repository;
    using RedisLuaScript.Persistent;

    internal static class AutofacConfig
    {
        private static IContainer container;

        public static IContainer Container
        {
            get
            {
                if (container == null)
                {
                    var builder = new ContainerBuilder();

                    {
                        builder.RegisterType<RedisBankAccountRespoitory>()
                            .WithParameter("conn", NoSqlService.RedisConnections)
                            .WithParameter("affixKey", NoSqlService.RedisAffixKey)
                            .As<IBankAccountRespoitory>()
                            .SingleInstance();
                    }

                    container = builder.Build();
                }

                return container;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EVEMarketWatch.Core.Domain;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Ninject;

namespace EVEMarketWatch.Core.Database
{
    public class ConfigureDatabase
    {
        private const string Sql2012ConnectionString = @"Server=.\SQLExpress; Database=evemarket; Integrated Security=SSPI;";

        public ConfigureDatabase()
        {

        }

        public ISessionFactory Create(IKernel kernel)
        {
            // Initialize NHibernate

            var sessionFactory = Fluently.Configure().Database(
                MsSqlConfiguration.MsSql2012.ConnectionString(
                    c => c.Is(Sql2012ConnectionString)))
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();

            kernel.Bind<ISessionFactory>().ToConstant(sessionFactory);

            return sessionFactory;
        }

        private static void BuildSchema(Configuration config)
        {
            new SchemaUpdate(config).Execute(true, true);
        }
    }
}

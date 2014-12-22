using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVEMarketWatch.Core.Domain;
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
        public ConfigureDatabase()
        {
            
        }

        public ISessionFactory Create(IKernel kernel)
        {
            // Initialize NHibernate
            var cfg = new Configuration();
            //cfg.Configure();
            cfg.DataBaseIntegration(x =>
            {
                x.ConnectionString = @"Server=.\SQLExpress; Database=evemarket; Integrated Security=SSPI;";
                x.Driver<SqlClientDriver>();
                x.Dialect<MsSql2012Dialect>();
            });

            cfg.AddAssembly(typeof(Order).Assembly);

            new SchemaUpdate(cfg).Execute(true, true);

            // Get ourselves an NHibernate Session
            var sessionFactory = cfg.BuildSessionFactory();

            kernel.Bind<ISessionFactory>().ToConstant(sessionFactory);

            return sessionFactory;
        }
    }
}

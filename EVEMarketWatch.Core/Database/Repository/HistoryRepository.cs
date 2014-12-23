using EVEMarketWatch.Core.Domain;
using NHibernate;

namespace EVEMarketWatch.Core.Database.Repository
{
    public class HistoryRepository : RepositoryBase<History>
    {
        private readonly ISession _session;

        public HistoryRepository(ISession session)
            : base(session)
        {
            _session = session;
        }
    }
}

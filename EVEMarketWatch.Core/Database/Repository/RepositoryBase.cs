using System.Collections.Generic;
using System.Linq;
using NHibernate;

namespace EVEMarketWatch.Core.Database.Repository
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
		protected ISession Session;

        protected RepositoryBase(ISession session)
		{
			Session = session;
		}

		public virtual void Add(T newItem)
		{
			Session.Save(newItem);
		}

		public virtual void Update(T updatedItem)
		{
			Session.Update(updatedItem);
		}

		public virtual void Delete(T deletedItem)
		{
			Session.Delete(deletedItem);
		}

		public T GetById(object id)
		{
			return Session.Get<T>(id);
		}

		public virtual List<T> GetAll()
		{
			return Session.QueryOver<T>().List().ToList();
		}
    }
}

using System.Collections.Generic;

namespace EVEMarketWatch.Core.Database.Repository
{
    public interface IRepository<T>
    {
        void Add(T newItem);

        void Update(T updatedItem);

        void Delete(T deletedItem);

        T GetById(object id);

        List<T> GetAll();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UserStorrage6.Model.Context;

namespace TestUserStorrage.Repo
{
    internal class PostgreRepositoryFake<T> : IRepository<T> where T : class
    {
        internal List<T> Entity { get; set; }

        internal PostgreRepositoryFake(List<T> entity)
        {
            Entity = entity;
        }

        public Task Add(T entity)
        {
            Entity.Add(entity);

            return Task.CompletedTask;
        }

        public void Attach(object obj)
        {
            throw new NotImplementedException();
        }

        public Task<T> Create(T entity)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<T> Entry(T entryObj)
        {
            throw new NotImplementedException();
        }

        public T GetById(object id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetQuery()
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Include(string navigationPropertyPath)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}

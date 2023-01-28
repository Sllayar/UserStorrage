using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using ApplicationDbContext = UsersStorrage.Models.Context.ApplicationDbContext;

namespace UserStorrage6.Model.Context
{
    public interface IRepository<T> : IDisposable where T : class
    {
        Task Add(T entity);
        Task<T> Create(T entity);
        Task<int> SaveChangesAsync();
        T GetById(object id);
        IQueryable<T> GetQuery();
        IQueryable<T> Include(string navigationPropertyPath);
        void Attach(object obj);
        Task<T> Entry(T entryObj);
    }
    public class PostgreRepository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class
    {
        private ApplicationDbContext _appContext;

        public PostgreRepository(
            IConfiguration config,
            ILoggerFactory loggerFactory,
            DbContextOptions<ApplicationDbContext> options) =>
                _appContext = new ApplicationDbContext(config, loggerFactory, options);

        public void Dispose()
        {
            if (_appContext != null)
            {
                _appContext.Dispose();;
            }
        }
        public IQueryable<TEntity> Include(string navigationPropertyPath)
        {
            return _appContext.Set<TEntity>().Include(navigationPropertyPath);
        }
        public async Task Add(TEntity entity)
        {
            await _appContext.AddAsync(entity);
        }
        public async Task<TEntity> Create(TEntity entity)
        {
            await _appContext.AddAsync(entity);
            await _appContext.SaveChangesAsync();
            return entity;
        }
        public Task<int> SaveChangesAsync()
        {
            return _appContext.SaveChangesAsync();
        }
        public TEntity? GetById(object id)
        {
            return _appContext.Set<TEntity>().Find(id);
        }
        public virtual IQueryable<TEntity> GetQuery()
        {
            return _appContext.Set<TEntity>().AsQueryable();
        }
        public void Attach(object obj)
        {
            _appContext.Attach(obj);
        }
        public async Task<TEntity> Entry(TEntity entryObj)
        {
            await _appContext.Entry<TEntity>(entryObj).GetDatabaseValuesAsync();
            return entryObj;
        }
    }
}

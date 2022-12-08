using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UserStorrage6.Model.Context;
using UserStorrage6.Model.DB;

namespace TestUserStorrage.Repo
{
    internal class PostgreRepositoryUserFake<T> : IRepository<T> where T : User
    {
        internal List<User> users = new List<User>();

        internal PostgreRepositoryUserFake() 
        {
            Service service1 = new Service()
            {
                Id = 1,
                Key = "key1",
                Author = "Sokol",
                Contacts = "89296526177",
                Name = "service1",
                Status = Status.Enable
            };

            Service service2 = new Service()
            {
                Id = 2,
                Key = "key2",
                Author = "Sokol",
                Contacts = "89296526188",
                Name = "service2",
                Status = Status.Enable
            };

            users = new List<User>()
            {
                new User()
                {
                    Id= 1,
                    SysLogin = "SokolES1",
                    OwnerLogin = "SokolES1",
                    Service = service1,
                    Type = UserStorrage6.Model.DB.Type.System,
                    Status = Status.Enable,
                    UpdateAt = DateTime.UtcNow,
                    CreateAT = DateTime.UtcNow
                },
                new User()
                {
                    Id= 2,
                    SysLogin = "SokolES2",
                    OwnerLogin = "SokolES2",
                    Service = service1,
                    Type = UserStorrage6.Model.DB.Type.System,
                    Status = Status.Enable,
                    UpdateAt = DateTime.UtcNow,
                    CreateAT = DateTime.UtcNow
                },
                new User()
                {
                    Id= 3,
                    SysLogin = "SokolES3",
                    OwnerLogin = "SokolES3",
                    Service = service1,
                    Type = UserStorrage6.Model.DB.Type.System,
                    Status = Status.Enable,
                    UpdateAt = DateTime.UtcNow,
                    CreateAT = DateTime.UtcNow
                },
                new User()
                {
                    Id= 4,
                    SysLogin = "SokolES4",
                    OwnerLogin = "SokolES4",
                    Service = service2,
                    Type = UserStorrage6.Model.DB.Type.System,
                    Status = Status.Enable,
                    UpdateAt = DateTime.UtcNow,
                    CreateAT = DateTime.UtcNow
                }
            };
        }

        public async Task Add(T entity)
        {
            users.Add(entity);
        }

        public void Attach(object obj)
        {
            users.Add((T)obj);
        }

        public async Task<T> Create(T entity)
        {
            users.Add(entity);

            return entity; 
        }

        public void Dispose()
        {
            users.Clear();
        }

        public async Task<T> Entry(T entryObj)
        {
            throw new NotImplementedException();
        }

        public T GetById(object id)
        {
            return (T)users.FirstOrDefault(u => u.Id == (int)id);
        }

        public IQueryable<T> GetQuery()
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Include(string navigationPropertyPath)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveChangesAsync()
        {
            return users.Last().Id;
        }
    }
}

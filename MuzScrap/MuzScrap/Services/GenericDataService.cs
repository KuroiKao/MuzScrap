using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MuzScrap.BaseContext;
using MuzScrap.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuzScrap.Services
{
    public class GenericDataService<T> : IDataService<T> where T : DomainObject
    {
        private readonly MuzScrapDbContextFactory _contextFactroy;

        public GenericDataService(MuzScrapDbContextFactory contextFactroy)
        {
            _contextFactroy = contextFactroy;
        }

        public async Task<T> Create(T entity)
        {
            using (MuzScrapDbContext context = _contextFactroy.CreateDbContext())
            {
                EntityEntry<T> createdResult = await context.Set<T>().AddAsync(entity);
                await context.SaveChangesAsync();

                return createdResult.Entity;
            }
        }

        public async Task<bool> Delete(int id)
        {
            using (MuzScrapDbContext context = _contextFactroy.CreateDbContext())
            {
                T entity = await context.Set<T>().FirstOrDefaultAsync((e) => e.Id == id);
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();

                return true;
            }
        }

        public async Task<T> Get(int id)
        {
            using (MuzScrapDbContext context = _contextFactroy.CreateDbContext())
            {
                T entity = await context.Set<T>().FirstOrDefaultAsync((e) => e.Id == id);

                return entity;
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using (MuzScrapDbContext context = _contextFactroy.CreateDbContext())
            {
                IEnumerable<T> entities = await context.Set<T>().ToListAsync();
                return entities;
            }
        }

        public async Task<T> Update(int id, T entity)
        {
            using (MuzScrapDbContext context = _contextFactroy.CreateDbContext())
            {
                entity.Id = id;

                context.Set<T>().Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }
    }
}

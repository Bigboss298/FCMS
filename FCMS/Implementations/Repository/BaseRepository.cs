using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
using FCMS.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FCMS.Implementations.Repository
{
    public class BaseRepository : IBaseRepository
    {
        protected readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Delete<T>(T entity) where T : BaseEntity => _context.Set<T>().Remove(entity);


        public async Task<T> Get<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            return await _context.Set<T>().FirstOrDefaultAsync(expression);
        }

        public async Task<IReadOnlyList<T>> GetAll<T>() where T : BaseEntity
        {
            return await _context.Set<T>().ToListAsync();
        }

        public Task<IReadOnlyList<T>> GetAll<T>(string param) where T : BaseEntity
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(T entity) where T : BaseEntity => _context.Set<T>().Add(entity);
       

        public IQueryable<T> QueryWhere<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            return _context.Set<T>().Where(expression);
        }

        public void Update<T>(T entity) where T : BaseEntity => _context.Set<T>().Update(entity);
      
    }
}

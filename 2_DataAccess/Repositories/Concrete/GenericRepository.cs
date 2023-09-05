using AppCore.Records.Bases;
using DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Concrete
{
    public class GenericRepository<T> : IGenericRepository<T> where T : EntityBase, new()
    {
        protected ETradeContext _context;

        protected IQueryable<T> _query;
        public GenericRepository(ETradeContext context)
        {
            _context = context;

            _query = _context.Set<T>();
        }

        public T? Get(Expression<Func<T, bool>> expression)
        {
            return _query.Where(expression).FirstOrDefault();
        }

        public List<T> GetList(Expression<Func<T, bool>> expression)
        {
            return _query.Where(expression).ToList();
        }
        public List<T> GetList()
        {
            return _query.ToList();
        }

        public GenericRepository<T> Include(List<string> includes)
        {
            foreach (string include in includes)
            {
                _query = _query.Include(include);
            }

            return this;
        }

        public GenericRepository<T> OrderBy<TKey>(Expression<Func<T, TKey>> expression)
        {
            _query = _query.OrderBy(expression);

            return this;
        }

        public void Add(T entity)
        {
            entity.Guid = Guid.NewGuid().ToString();

            _context.Add(entity);

            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _context.Update(entity);
            _context.SaveChanges();

        }

        public void Delete(Expression<Func<T, bool>> predicate)
        {
            var entities = _query.Where(predicate).ToList();

            _context.RemoveRange(entities);

            _context.SaveChanges();
        }

        public GenericRepository<T> ThenBy<TKey>(Expression<Func<T, TKey>> expression)
        {
            if (_query is IOrderedQueryable<T> q)
            {
                _query = q.ThenBy(expression);
            }
            return this;
        }

        public GenericRepository<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> expression)
        {
            _query = _query.OrderByDescending(expression);

            return this;
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> expression)
        {
            return await _query.Where(expression).ToListAsync();
        }

        public async Task<List<T>> GetListAsync()
        {
            return await _query.ToListAsync();
        }
    }
}

﻿using Business.Records.Bases;
using Repositories.Concrete;
using System.Linq.Expressions;

namespace Repositories.Abstract
{
    public interface IGenericRepository<T> where T : EntityBase, new()
    {
        T? Get(Expression<Func<T, bool>> expression);
        List<T> GetList(Expression<Func<T, bool>> expression);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> expression);
        List<T> GetList();
        Task<List<T>> GetListAsync();
        void Add(T entity);
        void Update(T entity);
        void Delete(Expression<Func<T, bool>> predicate);
        GenericRepository<T> Include(List<string> includes);
        GenericRepository<T> OrderBy<TKey>(Expression<Func<T, TKey>> expression);
        GenericRepository<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> expression);
        GenericRepository<T> ThenBy<TKey>(Expression<Func<T, TKey>> expression);
    }
}

using crud.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TestProject.WebAPI.Data;

namespace crud.Repository.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        TestProjectContext _ApplicationDBContext;
        public Repository(TestProjectContext applicationDBContext)
        {
            this._ApplicationDBContext = applicationDBContext;
        }
        public void Create(T entity)
        {
            this._ApplicationDBContext.Set<T>().Add(entity);
        }
        public void CreateRange(IEnumerable<T> entity)
        {
            this._ApplicationDBContext.Set<T>().AddRange(entity);
        }
        public void Delete(T entity)
        {
            this._ApplicationDBContext.Set<T>().Remove(entity);
        }

        public IQueryable<T> FindAll()
        {
            return this._ApplicationDBContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this._ApplicationDBContext.Set<T>().Where(expression).AsNoTracking();
        }

        public void Update(T entity)
        {

            //this._ApplicationDBContext.Set<T>().Update(entity).Property<int>("UserId").Metadata.ValueGenerated.ForUpdate() = false;
            this._ApplicationDBContext.Set<T>().Update(entity);
        }
    }
}

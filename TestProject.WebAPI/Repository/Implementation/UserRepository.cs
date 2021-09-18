using crud.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TestProject.WebAPI.Data;

namespace crud.Repository.Implementation
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(TestProjectContext repositoryContext)
             : base(repositoryContext)
        {
        }
    }
}

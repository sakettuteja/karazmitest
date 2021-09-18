
using crud.Repository.Abstraction;
using crud.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestProject.WebAPI.Data;

namespace crud.Repository
{
    public class RepositoryWrapper: IRepositoryWrapper
    {
        private TestProjectContext _repoContext;
        private IUserRepository _user;

        public RepositoryWrapper(TestProjectContext applicationDBContext)
        {
            this._repoContext = applicationDBContext;
        }

        public IUserRepository Users
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_repoContext);
                }
                return _user;
            }
        }

        public void Save()
        {
            _repoContext.SaveChanges();
        }
    }
}

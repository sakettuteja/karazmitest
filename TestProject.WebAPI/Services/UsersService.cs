using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using crud.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using TestProject.WebAPI.Data;

namespace TestProject.WebAPI.Services
{
    public class UsersService : IUsersService
    {
        //private readonly TestProjectContext _testProjectContext;
        private readonly IRepositoryWrapper _repoWrapper;

        public UsersService(IRepositoryWrapper repositoryWrapper)
        {
            _repoWrapper = repositoryWrapper;
        }


        public IEnumerable<User> Get(int[] ids, Filters filters)
        {

            if (ids.Length == 0 &&  filters.Ages.Length == 0 && filters.FirstNames.Length == 0 && filters.LastNames.Length == 0)
                return this._repoWrapper.Users.FindAll();

            List<User> userList = new List<User>();

            if (ids.Length > 0 ) {
                foreach (var id in ids)
                {
                    userList.Add(this._repoWrapper.Users.FindByCondition(x => x.UserId == id).FirstOrDefault());
                }
            }
            if (filters.Ages.Length > 0)
            {
                foreach (var id in filters.Ages)
                {
                    userList.Add(this._repoWrapper.Users.FindByCondition(x => x.Age == id).FirstOrDefault());
                }
            }
            if (filters.FirstNames.Length > 0)
            {
                foreach (var id in filters.FirstNames)
                {
                    userList.Add(this._repoWrapper.Users.FindByCondition(x => x.FirstName == id).FirstOrDefault());
                }
            }
            if (filters.LastNames.Length > 0)
            {
                foreach (var id in filters.LastNames)
                {
                    userList.Add(this._repoWrapper.Users.FindByCondition(x => x.LastName == id).FirstOrDefault());
                }
            }

            return userList;     
        }

        public IEnumerable<User> Get(int id)
        {
            return this._repoWrapper.Users.FindByCondition(x=> x.UserId == id);
            
        }
        public User GetByEmail(string Email)
        {
            return this._repoWrapper.Users.FindByCondition(x => x.Email == Email).FirstOrDefault();
        }

        public User Add(User user)
        {
          
                User user1 = new User()
                {
                    Age = user.Age,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    //Id = employee.Id,
                    LastName = user.LastName,
                    Password = user.Password
                };

                this._repoWrapper.Users.Create(user1);
                this._repoWrapper.Save();
            return user1;
        }

        public IEnumerable<User> AddRange(IEnumerable<User> users)
        {
             this._repoWrapper.Users.CreateRange(users);
            this._repoWrapper.Save();
            return users;
        }

        public User Update(User user)
        {
            this._repoWrapper.Users.Update(user);
            this._repoWrapper.Save();
            return user;
        }

        public bool Delete(User user)
        {
            this._repoWrapper.Users.Delete(user);
            this._repoWrapper.Save();
            return true;
        }
    }

    public interface IUsersService
    {
        IEnumerable<User> Get(int[] ids, Filters filters);
        IEnumerable<User> Get(int id);
        User GetByEmail (string Email);
        User Add(User user);

        IEnumerable<User> AddRange(IEnumerable<User> users);

        User Update(User user);

        bool Delete(User user);
    }

    public class Filters
    {
        public uint[] Ages { get; set; }
        public string[] FirstNames { get; set; }
        public string[] LastNames { get; set; }
    }

    public class FilterWrapper
    {
        public Filters filters { get; set; }
        public int[] ids { get; set; }
    }
}

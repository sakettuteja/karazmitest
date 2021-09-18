using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestProject.WebAPI.Data;
using TestProject.WebAPI.SeedData;
using TestProject.WebAPI.Services;

namespace TestProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _userservice;
        public UserController(IUsersService userservice)
        {
            this._userservice = userservice;
        }

        //[Route("/users")]
        //[HttpGet]//, Authorize(Roles = "Administrator")]
        //public IActionResult GetEmployeeDetails([FromBody] FilterWrapper filters)
        //{
        //    try
        //    {
        //        var Employees = this._userservice.Get(filters.ids, filters.filters);

        //        if (!Employees.Any())
        //            return NotFound();
        //        else
        //            return Ok(Employees);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex);
        //    }
        //}

        [Route("/users")]
        [HttpGet]//, Authorize(Roles = "Administrator")]
        public IActionResult GetAllEmployeeDetails()
        {
            try
            {
                int[] ids = new int[0];
                Filters filters = new Filters()
                {
                    Ages = new uint[0],
                    FirstNames = new string[0],
                    LastNames = new string[0]
                };

                var Employees = this._userservice.Get(ids, filters);

                if (!Employees.Any())
                    return NotFound();
                else
                    return Ok(Employees);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("/users")]
        public IActionResult AddEmployee([FromBody] CreateUserForm createUserForm)
        {
            try
            {
                if (createUserForm == null)
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "400", ErrorDescription = "Employee Details required" };
                    return BadRequest(er);
                }

                if (!ModelState.IsValid)
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "400", ErrorDescription = "Employee Details required" };
                    return BadRequest(er);
                }

                User CreateUserForm1 = new User()
                {
                    Age = createUserForm.Age,
                    Email = createUserForm.Email,
                    FirstName = createUserForm.FirstName,
                    LastName = createUserForm.LastName,
                    Password = createUserForm.Password
                };

                this._userservice.Add(CreateUserForm1);
                return Ok(User);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

        }





        [HttpGet]
        [Route("/user/{id:int?}")]
        public IActionResult GetEmployeeWithCondition([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "404", ErrorDescription = "Employee with id not found" };
                    return NotFound(er);
                }

                var emp = this._userservice.Get(id);

                if (!emp.Any())
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "404", ErrorDescription = "Employee with id not found" };
                    return NotFound(er);
                }

                return Ok(emp.FirstOrDefault());
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        //[HttpGet]
        //[Route("/restaurant/query/{city?}")]
        //public IActionResult GetEmployeeWithcityCondition([FromQuery] string city)
        //{
        //    try
        //    {
        //        if (city == null)
        //        {
        //            ErrorResponse er = new ErrorResponse() { ErrorCode = "404", ErrorDescription = "Employee with city not found, city name required" };
        //            return NotFound(er);
        //        }

        //        var emp = this._repoWrapper.Employees.FindByCondition(x => x.FirstName.Equals(city));

        //        if (!emp.Any())
        //        {
        //            ErrorResponse er = new ErrorResponse() { ErrorCode = "404", ErrorDescription = "Employee with city not found" };
        //            return NotFound(er);
        //        }

        //        return Ok(emp);
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(ex);
        //    }
        //}
        [HttpDelete]
        [Route("/users/{id:int?}")]
        public IActionResult deleteemployee([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "404", ErrorDescription = "Employee with id not found" };
                    return NotFound(er);
                }

                var emp = this._userservice.Get(id);

                if (!emp.Any())
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "404", ErrorDescription = "Employee with id not found" };
                    return NotFound(er);
                }

                this._userservice.Delete(emp.FirstOrDefault());
                object obj = new { statuscode = "200", description = "Employee Deleted Successfully" };

                return NoContent();
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("/user/{id:int?}")]
        public IActionResult updateemployee([FromQuery] int id, [FromBody] UpdateUserForm updateEmployeeDTO)
        {
            try
            {

                if (id == 0)
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "404", ErrorDescription = "Employee with id not found" };
                    return NotFound(er);
                }

                if (updateEmployeeDTO == null)
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "400", ErrorDescription = "Employee Details required" };
                    return BadRequest(er);
                }

                if (!ModelState.IsValid)
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "400", ErrorDescription = "Employee Details required" };
                    return BadRequest(er);
                }

                var emp = this._userservice.Get(id).FirstOrDefault();

                if (emp == null)
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "404", ErrorDescription = "Employee with id not found to update" };
                    return NotFound(er);
                }



                emp.Age = updateEmployeeDTO.Age;
                emp.Email = updateEmployeeDTO.Email;
                emp.FirstName = updateEmployeeDTO.FirstName;
                //UserId = updateEmployeeDTO.Id,
                emp.LastName = updateEmployeeDTO.LastName;
                emp.Password = updateEmployeeDTO.Password;


                this._userservice.Update(emp);
                return NoContent();
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("/import")]
        public IActionResult AddEmployee(IFormFile formFile)
        {
            try
            {
                byte[] buffer = new byte[16 * 1024];
                string jsonStr = "";
                using (Stream stream = formFile.OpenReadStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int read;
                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                        jsonStr = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                
                CreateUserForm employee = null;

                

                var listcreateUserForms = JsonConvert.DeserializeObject<IEnumerable<CreateUserForm>>(jsonStr);

                if (listcreateUserForms == null)
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "400", ErrorDescription = "Employee Details required" };
                    return BadRequest(er);
                }

                if (!ModelState.IsValid)
                {
                    ErrorResponse er = new ErrorResponse() { ErrorCode = "400", ErrorDescription = "Employee Details required" };
                    return BadRequest(er);
                }
                List<User> listuser = new List<User>();
                foreach (var item in listcreateUserForms)
                {
                    User CreateUserForm1 = new User()
                    {
                        Age = item.Age,
                        Email = item.Email,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Password = item.Password
                    };
                    listuser.Add(CreateUserForm1);
                    
                }
                this._userservice.AddRange(listuser);
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

        }

        [HttpPost]
        [Route("/currentuser/{emailid?}")]
        [Authorize]
        public IActionResult GetUserByEmail ([FromQuery] string emailid)
        {
            try
            {
                
                var Employees = this._userservice.GetByEmail(emailid);

                if (Employees == null)
                    return NotFound();
                else
                    return Ok(Employees);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}

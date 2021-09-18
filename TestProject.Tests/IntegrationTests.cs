using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using TestProject.WebAPI;
using TestProject.WebAPI.Data;
using TestProject.WebAPI.Models;
using TestProject.WebAPI.SeedData;
using Xunit;

namespace TestProject.Tests
{
    public class IntegrationTests
    {
        private TestServer _server;

        public HttpClient Client { get; private set; }

        public IntegrationTests()
        {
            SetUpClient();
        }

        private async Task SeedData()
        {
            var createForm0 = GenerateCreateForm("Mike", "Emil", 24, "testemail1@mail.com", "12345678");
            var response0 = await Client.PostAsync("https://localhost:44350/users", new StringContent(JsonConvert.SerializeObject(createForm0), Encoding.UTF8, "application/json"));

            var createForm1 = GenerateCreateForm("Daniel", "Johnson", 19, "testemail2@mail.com", "12345678");
            var response1 = await Client.PostAsync("https://localhost:44350/users", new StringContent(JsonConvert.SerializeObject(createForm1), Encoding.UTF8, "application/json"));

            var createForm2 = GenerateCreateForm("Daniel", "Olson", 19, "testemail3@mail.com", "12345678");
            var response2 = await Client.PostAsync("https://localhost:44350/users", new StringContent(JsonConvert.SerializeObject(createForm2), Encoding.UTF8, "application/json"));

            var createForm3 = GenerateCreateForm("Olga", "Verso", 19, "testemail21s2@mail.com", "12345678");
            var response3 = await Client.PostAsync("https://localhost:44350/users", new StringContent(JsonConvert.SerializeObject(createForm3), Encoding.UTF8, "application/json"));
        }

        private CreateUserForm GenerateCreateForm(string firstName, string lastName, uint age, string email, string password)
        {
            return new CreateUserForm()
            {
                FirstName = firstName,
                Age = age,
                Email = email,
                LastName = lastName,
                Password = password
            };
        }

        // TEST NAME - getAllEntriesById
        // TEST DESCRIPTION - It finds all users in Database
        [Fact]
        public async Task Test1()
        {
            await SeedData();

            var response0 = await Client.GetAsync("https://localhost:44350/users");
            response0.StatusCode.Should().BeEquivalentTo(200);

            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(response0.Content.ReadAsStringAsync().Result);
            users.Count().Should().Be(4);
        }

        // TEST NAME - getSingleEntryById
        // TEST DESCRIPTION - It finds single user by ID
        [Fact]
        public async Task Test2()
        {
            //await SeedData();

            var response0 = await Client.GetAsync("https://localhost:44350/user?id=1");
            response0.StatusCode.Should().BeEquivalentTo(200);

            var user = JsonConvert.DeserializeObject<User>(response0.Content.ReadAsStringAsync().Result);
            user.Age.Should().Be(24);

            var response1 = await Client.GetAsync("https://localhost:44350/user?id=101");
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status404NotFound);
        }

        // TEST NAME - getSingleEntryById
        // TEST DESCRIPTION - It finds single user by ID
        [Fact]
        public async Task Test3()
        {
            //await SeedData();

            var response1 = await Client.GetAsync("/api/users");
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(response1.Content.ReadAsStringAsync().Result);
            users.Count().Should().Be(8);

            var response2 = await Client.GetAsync("/api/users?firstNames=Mike&firstNames=Daniel");
            response2.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            var filteredUsers = JsonConvert.DeserializeObject<IEnumerable<User>>(response2.Content.ReadAsStringAsync().Result).ToArray();
            filteredUsers.Length.Should().Be(3);
            filteredUsers.Where(x => x.FirstName == "Mike").ToArray().Length.Should().Be(1);
            filteredUsers.Where(x => x.FirstName == "Daniel").ToArray().Length.Should().Be(2);
        }

        // TEST NAME - deleteUserById
        // TEST DESCRIPTION - Check delete user web api end point
        [Fact]
        public async Task Test4()
        {
            //await SeedData();

            var response0 = await Client.DeleteAsync("https://localhost:44350/users?id=2");
            response0.StatusCode.Should().BeEquivalentTo(StatusCodes.Status204NoContent);

            var response1 = await Client.GetAsync("https://localhost:44350/user?id=2");
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status404NotFound);
        }

        // TEST NAME - updateUserById
        // TEST DESCRIPTION - Check update user web api end point
        [Fact]
        public async Task Test5()
        {
            //await SeedData();

            var updateForm = new UpdateUserForm()
            {
                Id = 1,
                Age = 40,
                Email = "testemail1@mail.com",
                FirstName = "Mike",
                LastName = "Emil",
                Password = "0000000"
            };

            var response0 = await Client.PutAsync("https://localhost:44350/user?id=3", new StringContent(JsonConvert.SerializeObject(updateForm), Encoding.UTF8, "application/json"));
            response0.StatusCode.Should().BeEquivalentTo(StatusCodes.Status204NoContent);

            var response1 = await Client.GetAsync("https://localhost:44350/user?id=3");
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);

            var user = JsonConvert.DeserializeObject<User>(response1.Content.ReadAsStringAsync().Result);
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            user.Age.Should().Be(40);
            user.Password.Should().Be("0000000");
        }

        // TEST NAME - exportUsers
        // TEST DESCRIPTION - In this test user should send byte array to the web api and put all users(count is 1000) into the database
        [Fact]
        public async Task Test6()
        {
            //Here data is exporting to the end point
            var myJsonString = File.ReadAllBytes("MOCK_DATA.json");
            var content = new ByteArrayContent(myJsonString);
            content.Headers.Add("name", "formFile");
            var response0 = await Client.PostAsync("https://localhost:44350/import", content);
            response0.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);

            //Here expect to see all users from web api end point (1000).
            var response1 = await Client.GetAsync("https://localhost:44350/users");
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(response1.Content.ReadAsStringAsync().Result);
            users.Count().Should().Be(1000);

            //Here check that the data is exported in the correct way
            var response2 = await Client.GetAsync("/api/users?firstNames=Veronika&firstNames=Frances");
            response2.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            var filteredUsers = JsonConvert.DeserializeObject<IEnumerable<User>>(response2.Content.ReadAsStringAsync().Result).ToArray();
            filteredUsers.Length.Should().Be(3);
            filteredUsers.Where(x => x.FirstName == "Frances").ToArray().Length.Should().Be(1);
            filteredUsers.Where(x => x.FirstName == "Veronika").ToArray().Length.Should().Be(2);
        }

        // TEST NAME - checkAuthorization
        // TEST DESCRIPTION - Here need to implement authorization by JWT tokens
        [Fact]
        public async Task Test7()
        {
            //await SeedData();
            var userLoginForm = new LoginUserForm { Email = "testemail2@mail.com", Password = "Password@123" };

            //Getting token by email and password
            var response0 = await Client.PostAsync("https://localhost:44350/token",
                new StringContent(JsonConvert.SerializeObject(userLoginForm), Encoding.UTF8, "application/json"));
            var jwtData = JsonConvert.DeserializeObject<LoginResponseModel>(response0.Content.ReadAsStringAsync().Result);

            string  url = "https://localhost:44350/currentuser?emailid='"+ jwtData.Username+"'";
            //Check that user Unauthorized
            var response1 = await Client.GetAsync(url);
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status401Unauthorized);

            //adding token to request and check this end-point again
            Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtData.AccessToken);
            var response2 = await Client.GetAsync(url);
            var user = JsonConvert.DeserializeObject<User>(response2.Content.ReadAsStringAsync().Result);
            user.Email.Should().BeEquivalentTo("testemail2@mail.com");
        }

        private void SetUpClient()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    var context = new TestProjectContext(new DbContextOptionsBuilder<TestProjectContext>()
                        .UseSqlServer("Server=DESKTOP-DJUIHTS\\SHIVETDB;Database=orders;User Id= sa;Password= ;")
                        //.UseSqlite("DataSource=:memory:")
                        .EnableSensitiveDataLogging()
                        .Options);

                    services.RemoveAll(typeof(TestProjectContext));
                    services.AddSingleton(context);

                    context.Database.OpenConnection();
                    context.Database.EnsureCreated();

                    context.SaveChanges();

                    // Clear local context cache
                    foreach (var entity in context.ChangeTracker.Entries().ToList())
                    {
                        entity.State = EntityState.Detached;
                    }
                });

            _server = new TestServer(builder);

            Client = _server.CreateClient();
        }
    }
}

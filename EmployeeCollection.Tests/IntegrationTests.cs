using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using EmployeeCollection.WebAPI;
using EmployeeCollection.WebAPI.Data;
using EmployeeCollection.WebAPI.SeedData;
using Xunit;

namespace EmployeeCollection.Tests
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
            var createForm0 = GenerateCreateForm("Charles", "California", "IT", 40, 4);
            var response0 = await Client.PostAsync("/api/employee", new StringContent(JsonConvert.SerializeObject(createForm0), Encoding.UTF8, "application/json"));

            var createForm1 = GenerateCreateForm("John", "New York", "Accounting", 30, 3);
            var response1 = await Client.PostAsync("/api/employee", new StringContent(JsonConvert.SerializeObject(createForm1), Encoding.UTF8, "application/json"));

            var createForm2 = GenerateCreateForm("Eric", "Seattle", "Law", 25, 2);
            var response2 = await Client.PostAsync("/api/employee", new StringContent(JsonConvert.SerializeObject(createForm2), Encoding.UTF8, "application/json"));

            var createForm3 = GenerateCreateForm("Jessica", "Florida", "Admin", 35, 1);
            var response3 = await Client.PostAsync("/api/employee", new StringContent(JsonConvert.SerializeObject(createForm3), Encoding.UTF8, "application/json"));
        }

        private CreateEmployeeForm GenerateCreateForm(string name, string location, string department, int age, float rating)
        {
            return new CreateEmployeeForm()
            {
                Name = name,
                Location = location,
                Department = department,
                Age = age,
                Rating = rating
            };
        }

        // TEST NAME - GetAllEmployees
        // TEST DESCRIPTION - Get the list of all employees
        [Fact]
        public async Task TestCase0()
        {
            await SeedData();

            var response0 = await Client.GetAsync("/api/employee");
            response0.StatusCode.Should().BeEquivalentTo(200);

            var realData0 = JsonConvert.DeserializeObject(response0.Content.ReadAsStringAsync().Result);
            var expectedData0 = JsonConvert.DeserializeObject("[{\"id\":1,\"name\":\"Charles\",\"location\":\"California\",\"rating\":4,\"department\":\"IT\",\"age\":40},{\"id\":2,\"name\":\"John\",\"location\":\"New York\",\"rating\":3,\"department\":\"Accounting\",\"age\":30},{\"id\":3,\"name\":\"Eric\",\"location\":\"Seattle\",\"rating\":2,\"department\":\"Law\",\"age\":25},{\"id\":4,\"name\":\"Jessica\",\"location\":\"Florida\",\"rating\":1,\"department\":\"Admin\",\"age\":35}]");
            realData0.Should().BeEquivalentTo(expectedData0);
        }

        // TEST NAME - CreateEmployee
        // TEST DESCRIPTION - A new employee should be created
        [Fact]
        public async Task TestCase1()
        {
            await SeedData();

            // Create entry with id 5
            var createForm0 = GenerateCreateForm("Bob", "Miami", "Utility", 40, 5);
            var response0 = await Client.PostAsync("/api/employee", new StringContent(JsonConvert.SerializeObject(createForm0), Encoding.UTF8, "application/json"));
            response0.StatusCode.Should().BeEquivalentTo(201);

            var response1 = await Client.GetAsync("/api/employee");
            response1.StatusCode.Should().BeEquivalentTo(200);

            var realData1 = JsonConvert.DeserializeObject(response1.Content.ReadAsStringAsync().Result);
            var expectedData1 = JsonConvert.DeserializeObject("[{\"id\":1,\"name\":\"Charles\",\"location\":\"California\",\"rating\":4,\"department\":\"IT\",\"age\":40},{\"id\":2,\"name\":\"John\",\"location\":\"New York\",\"rating\":3,\"department\":\"Accounting\",\"age\":30},{\"id\":3,\"name\":\"Eric\",\"location\":\"Seattle\",\"rating\":2,\"department\":\"Law\",\"age\":25},{\"id\":4,\"name\":\"Jessica\",\"location\":\"Florida\",\"rating\":1,\"department\":\"Admin\",\"age\":35},{\"id\":5,\"name\":\"Bob\",\"location\":\"Miami\",\"rating\":5,\"department\":\"Utility\",\"age\":40}]");
            realData1.Should().BeEquivalentTo(expectedData1);
        }

        // TEST NAME - GetEmployeeById
        // TEST DESCRIPTION - Get employee by id
        [Fact]
        public async Task TestCase2()
        {
            await SeedData();

            // Get entry with id 3
            var response0 = await Client.GetAsync("/api/employee/3");
            response0.StatusCode.Should().BeEquivalentTo(200);

            var realData0 = JsonConvert.DeserializeObject(response0.Content.ReadAsStringAsync().Result);
            var expectedData0 = JsonConvert.DeserializeObject("{\"id\":3,\"name\":\"Eric\",\"location\":\"Seattle\",\"rating\":2,\"department\":\"Law\",\"age\":25}");
            realData0.Should().BeEquivalentTo(expectedData0);

            var response1 = await Client.GetAsync("/api/employee/6");
            response1.StatusCode.Should().BeEquivalentTo(404);
        }

        // TEST NAME - GetEmployeeByDepartment
        // TEST DESCRIPTION - Get employee by Department
        [Fact]
        public async Task TestCase3()
        {
            await SeedData();

            // Get employee with Department Law and Admin
            var response0 = await Client.GetAsync("api/employee?department=Law&department=Admin");
            response0.StatusCode.Should().BeEquivalentTo(200);

            var realData0 = JsonConvert.DeserializeObject(response0.Content.ReadAsStringAsync().Result);
            var expectedData0 = JsonConvert.DeserializeObject("[{\"id\":3,\"name\":\"Eric\",\"location\":\"Seattle\",\"rating\":2,\"department\":\"Law\",\"age\":25},{\"id\":4,\"name\":\"Jessica\",\"location\":\"Florida\",\"rating\":1,\"department\":\"Admin\",\"age\":35}]");
            realData0.Should().BeEquivalentTo(expectedData0);

            var response1 = await Client.GetAsync("api/employee?department=Operations");
            response1.StatusCode.Should().BeEquivalentTo(200);

            var realData1 = JsonConvert.DeserializeObject(response1.Content.ReadAsStringAsync().Result);
            realData1.Should().Equals("[]");
        }

        // TEST NAME - checkNonExistentApi
        // TEST DESCRIPTION - It should check if an API exists
        [Fact]
        public async Task TestCase4()
        {
            await SeedData();

            // Return with 404 if no API path exists 
            var response0 = await Client.GetAsync("/api/employee/id/123");
            response0.StatusCode.Should().BeEquivalentTo(404);

            // Return with 405 if API path exists but called with different method
            var response1 = await Client.PostAsync("/api/employee/123", null);
            response1.StatusCode.Should().BeEquivalentTo(405);
        }

        // TEST NAME - deleteEmployee
        // TEST DESCRIPTION - Delete an employee
        [Fact]
        public async Task TestCase5()
        {
            await SeedData();

            // Return with 204 if employee is deleted
            var response0 = await Client.DeleteAsync("/api/employee/2");
            response0.StatusCode.Should().Be(204);

            // Check if the employee does not exist
            var response1 = await Client.GetAsync("/api/employee/2");
            response1.StatusCode.Should().Be(404);

            // Return with 404 if employee is not found
            var response2 = await Client.DeleteAsync("/api/employee/10");
            response2.StatusCode.Should().Be(404);
        }

        private void SetUpClient()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    var context = new EmployeeCollectionContext(new DbContextOptionsBuilder<EmployeeCollectionContext>()
                        .UseSqlite("DataSource=:memory:")
                        .EnableSensitiveDataLogging()
                        .Options);

                    services.RemoveAll(typeof(EmployeeCollectionContext));
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

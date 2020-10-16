using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EmployeeCollection.WebAPI.SeedData
{
    public class CreateEmployeeForm
    {
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("rating")]
        public float Rating { get; set; }
    }
}

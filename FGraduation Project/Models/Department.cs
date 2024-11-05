using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace FGraduation_Project.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        [JsonIgnore]
        [ValidateNever]
        public  List<Employee> Employees { get; set; }= new List<Employee>();
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FGraduation_Project.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public int? NationalId { get; set; }
        public int? Age { get; set; }
        [ValidateNever]
        public double Salary { get; set; }
        [ValidateNever]
        public double? MonthlySalary { get; set; }
        [ValidateNever]
        public string? Date { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        [ValidateNever]
        public string? ImagePath { get; set; }
        [NotMapped]
        [ValidateNever]
        public IFormFile Image { get; set; }
        public string? AccountId { get; set; } 
        public string? UserName { get; set; }
        
        public int DepartmentId { get; set; }
        [ValidateNever]
        public  Department Department { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public virtual List<History> Histories { get; set; }=new List<History>();
       

    }
}

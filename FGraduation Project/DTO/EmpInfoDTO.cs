﻿namespace FGraduation_Project.DTO
{
    public class EmpInfoDTO
    { 
      //  public string? FirstName { get; set; }
       // public string? LastName { get; set; }
        public int? NationalId { get; set; }
        public int? Age { get; set; } 
        public string? Gender { get; set; } 
        public string? Address { get; set; }
        public IFormFile? Image { get; set; }
        public int DepartmentId { get; set; }
    }
}

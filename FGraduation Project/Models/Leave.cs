using System.ComponentModel.DataAnnotations;

namespace FGraduation_Project.Models
{
    public class Leave
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        public string From { get; set; }
        [Required]
        public string To { get; set; }
       
    }
}

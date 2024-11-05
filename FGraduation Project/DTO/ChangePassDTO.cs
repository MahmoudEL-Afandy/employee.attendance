using System.ComponentModel.DataAnnotations;

namespace FGraduation_Project.DTO
{
    public class ChangePassDTO
    {
        
        //public string UserName { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string Password { get; set; }
        [Compare("Password")]
        [Required]
        public string ConfirmPassword { get; set; }
    }
}

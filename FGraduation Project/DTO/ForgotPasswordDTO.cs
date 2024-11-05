using System.ComponentModel.DataAnnotations;

namespace FGraduation_Project.DTO
{
    public class ForgotPasswordDTO
    {
        // [Required]
        // [EmailAddress]
        // public string Email { get; set; }
      
        [Required]
        public string UserName { get; set; }
        public string NaviOFResetScreen { get; set; }
    }
}

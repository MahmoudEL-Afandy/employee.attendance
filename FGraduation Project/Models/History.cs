using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGraduation_Project.Models
{
    public class History
    {
        public int Id { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public bool IsAttended { get; set; }
        public bool NoAttended { get; set; }
        public bool IsLated { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        [ValidateNever]
        public virtual Employee Employee { get; set; }
        [ValidateNever]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public History()
        {

            DateTime dateTime = DateTime.Now;   
            Day=dateTime.ToString("dddd");
            Month = dateTime.ToString("MM");
            Date = dateTime.ToString("MM:dd:yyyy");
            Time = dateTime.ToString("hh:mm:ss:tt");
            IsAttended = false;
            IsLated=false;
            NoAttended = true;
        }
    }
}

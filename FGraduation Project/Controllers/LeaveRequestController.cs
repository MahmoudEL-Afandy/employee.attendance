using FGraduation_Project.Contexts;
using FGraduation_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FGraduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("LeavedEmps")]
        [Authorize(Roles = "Admin")]
        public IActionResult LeavedEmployees()
        {
            DateTime dateTime = DateTime.Now;

            var leaves = _context.Leaves.Select(x => new { x.EmployeeId, x.FirstName, x.Date, x.From, x.To }).
                Where(x => x.Date == dateTime.ToString("MM:dd:yyyy")).AsQueryable();

            if (!leaves.IsNullOrEmpty())
            {
                return Ok(leaves);
            }
            return BadRequest(new { message = "NoEmployeesLeaved" });

        }
        [HttpPost("LeaveReq")]
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult LeaveReq(string to)
        {
            if (to != null)
            {
                DateTime dateTime = DateTime.Now;
                Claim claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                string userId = claimId.Value;
                var emp = _context.Employees.FirstOrDefault(e => e.AccountId == userId);
                Leave leave = new Leave();
                leave.UserId = userId;
                leave.EmployeeId = emp.Id;
                leave.FirstName = emp.FirstName;
                leave.Date = dateTime.ToString("MM:dd:yyyy");
                leave.From = dateTime.ToString("hh:mm:tt");
                leave.To = to;
                _context.Leaves.Add(leave);
                _context.SaveChanges();
               // bool state = true;
                return Ok(emp); //state was here

            }
            return BadRequest();
        }


        [HttpPut("LeaveReqEdit")]
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult LeaveReqEdit(string to)
        {
            if (to != null)
            {
                Claim claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                string userId = claimId.Value;
                Leave leave = _context.Leaves.FirstOrDefault(x=>x.UserId== userId);
                leave.To = to;
                _context.Leaves.Update(leave);
                _context.SaveChanges();
                //bool state = true;
                return Ok(leave);  //state was here
            }
            return BadRequest();
        }
    }
}

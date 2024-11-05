using FGraduation_Project.Contexts;
using FGraduation_Project.DTO;
using FGraduation_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections;

namespace FGraduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("empsattend")]
        public IActionResult EmpAttend() 
        { 
            DateTime date = DateTime.Now;
             
            var Emps =_context.Historys.Select(x=>new {x.EmployeeId,x.Employee.NationalId,x.Employee.FirstName,x.IsAttended,x.Date})
                .Where(x=>x.IsAttended==true && x.Date==date.ToString("MM:dd:yyyy")).AsQueryable();
               if (!Emps.IsNullOrEmpty())
                return Ok(Emps);
            return NotFound(new { message = "NoEmpAttend" });


        }
        [HttpGet("empslated")]
        public IActionResult EmpLated ()
        {
            DateTime date = DateTime.Now;

            //IQueryable
            var Emps = _context.Historys.Select(x => new { x.EmployeeId, x.Employee.NationalId, x.Employee.FirstName, x.IsLated, x.Date })
              .Where(x => x.IsLated == true && x.Date == date.ToString("MM:dd:yyyy")).AsEnumerable();
            if (!Emps.IsNullOrEmpty()) return Ok(Emps);
            

            return NotFound(new { message = "NoEmpLated" });

        }
        [HttpGet("empsabsent")]
        public IActionResult EmpAbsent () 
        {
            DateTime date = DateTime.Now;
           
              var EmpsHistory = _context.Historys.Include(x => x.Employee).Select(x => new { x.EmployeeId, x.Date }).Where(x => x.Date == date.ToString("MM:dd:yyyy"))
                .AsEnumerable();
             var employees = _context.Employees.ToList();    
            List<AbsentReport> absentReports = new List<AbsentReport>();

            foreach(var employee in employees)
            {
                bool empFind = false;
                foreach(var Emphistory in EmpsHistory)
                {
                   
                    if(employee.Id ==Emphistory.EmployeeId)
                    {
                        empFind = true;
                    }

                }
                if(empFind==false) 
                {
                    AbsentReport absent = new AbsentReport();
                    absent.IsAbsent = true;
                    absent.EmployeeId=employee.Id;
                    absent.NationalId = employee.NationalId;
                    absent.FirstName = employee.FirstName;
                    absent.Date = date.ToString("MM:dd:yyyy");
                    absentReports.Add(absent);
                }
                
            }
            
            

         
            if (!absentReports.IsNullOrEmpty())
                return Ok(absentReports);

            return NotFound(new { message = "NoEmpAbsent" });
        }

        [HttpGet("monthlyattend")]
        public IActionResult MonthlyReportAttend ()
        {
            DateTime monthDate = DateTime.Now;
            
            var monthReportAtt = _context.Historys.Select(x => new {  x.Employee.NationalId, x.Employee.FirstName, x.Day, x.Month ,x.Date, x.IsAttended , x.IsLated }).
             Where(x =>( x.IsAttended == true || x.IsLated==true) && x.Month==monthDate.ToString("MM"));   
            
            if (!monthReportAtt.IsNullOrEmpty()) return Ok(monthReportAtt);
            return NotFound(new { message = "NoEmpAttend" });

        }
      /*
        [HttpGet("monthlylated")]
        public IActionResult MonthlyReportLated()
        {
            DateTime monthDate = DateTime.Now;

            IQueryable monthReportLated = _context.Historys.Select(x => new { x.EmployeeId, x.Employee.NationalId, x.Employee.FirstName, x.Day, x.Month, x.Date, x.IsLated }).
             Where(x => x.IsLated == true && x.Month == monthDate.ToString("MM"));
            return Ok(monthReportLated);

        }
      */
        [HttpGet("salaryreport")]
        public IActionResult SalaryReport ()
        {
            var EmpsSalary = _context.Employees.Select(x => new { x.Id, x.NationalId, x.FirstName, x.MonthlySalary, x.Date }).AsEnumerable();

           if (!EmpsSalary.IsNullOrEmpty()) return Ok(EmpsSalary);
            return NotFound(new { message = "NoMonSalary" });
        }
    }
}

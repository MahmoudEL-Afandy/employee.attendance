using FGraduation_Project.Contexts;
using FGraduation_Project.DTO;
using FGraduation_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FGraduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("addempinfo")]
        [Authorize(Roles = "Employee,Admin")]
        public IActionResult AddEmpInfo([FromForm] EmpInfoDTO empInfoDTO) 
        {
            if (ModelState.IsValid)
            {
               string UserName = User.Identity.Name;
              Claim ClaimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
               string UserId = ClaimId.Value;
                Employee employee = _context.Employees.FirstOrDefault(c => c.UserName == UserName);
                

                 //   employee.FirstName = empInfoDTO.FirstName;
                  //  employee.LastName = empInfoDTO.LastName;
                    employee.NationalId = empInfoDTO.NationalId;
                    employee.Age = empInfoDTO.Age;
                    employee.Gender = empInfoDTO.Gender;
                    employee.Address = empInfoDTO.Address;
                    employee.DepartmentId = empInfoDTO.DepartmentId;
                  //  employee.UserName = User.Identity.Name;
                    employee.AccountId = UserId;
                if (empInfoDTO.Image == null)
                {
                    employee.ImagePath = "\\Images2\\NIC.jpg";
                }
                else
                {
                    string imgExtension = Path.GetExtension(empInfoDTO.Image.FileName);
                    Guid imgGuid = Guid.NewGuid();
                    string imgName = imgGuid + imgExtension;
                    employee.ImagePath = "\\Images2\\employee\\" + imgName;
                    string imFullPath = _webHostEnvironment.WebRootPath + employee.ImagePath;
                    FileStream imgStream = new FileStream(imFullPath, FileMode.Create);
           /* */    empInfoDTO.Image.CopyTo(imgStream);

                    imgStream.Dispose();
                }


                _context.Employees.Update(employee);
                _context.SaveChanges();
                //bool state = true;
                return Ok(employee); //state was here

            }
            return BadRequest(ModelState);
        }
        [HttpGet("getallemployee")]
        [Authorize(Roles ="Admin")]
        public IActionResult GetAllEmployee ()
        {
            if (ModelState.IsValid) 
            {
                // List<Employee> employees = _context.Employees.Include(d=>d.Department).ToList();
                var employees = _context.Employees.Include(d => d.Department).Select(x => new { x.Id,x.NationalId ,x.FirstName,x.LastName,x.Gender,
                x.Age,x.Address,Department=x.Department.Name}).AsQueryable();
                if (!employees.IsNullOrEmpty())
                {
                    return Ok(employees);
                }
                return NotFound(new { message = "NotFoundEmps" });
            
            
            }
            return BadRequest(ModelState);
        }
        [HttpGet("getoneemployee")]
        [Authorize(Roles ="Employee,Admin")]
        public IActionResult GetOneEmployee () 
        {
            if (ModelState.IsValid)
            {

                Claim IdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                string userId = IdClaim.Value;

                Employee employee = _context.Employees.Include(d => d.Department).FirstOrDefault(x => x.AccountId == userId);
               
                 return Ok(employee);
                
                
                
            }
            return BadRequest(ModelState);
        }

        [HttpPut("updateempinfo")]
        [Authorize(Roles ="Employee,Admin")]
        public IActionResult UpdateEmpInfo ([FromForm]updateEmpInfoDTO empInfoDTO)
        { 
            if (ModelState.IsValid)
            { 
                Claim claimId =User.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.NameIdentifier);
                string userId = claimId.Value;
                Employee employee = _context.Employees.FirstOrDefault(x => x.AccountId == userId);
                if (employee != null) 
                { 
                    employee.FirstName = empInfoDTO.FirstName;
                    employee.LastName = empInfoDTO.LastName;
                    employee.NationalId = empInfoDTO.NationalId;
                    employee.Age = empInfoDTO.Age;
                    employee.Gender = empInfoDTO.Gender;
                    employee.Address = empInfoDTO.Address;
                    employee.DepartmentId = empInfoDTO.DepratmentId;
                    if (empInfoDTO.Image != null)
                    {
                        if (employee.ImagePath != "\\images\\NIC.jpg")
                        {
                            string oldImageFullPath = _webHostEnvironment.WebRootPath + employee.ImagePath;
                            if (System.IO.File.Exists(oldImageFullPath))
                            {
                                System.IO.File.Delete(oldImageFullPath);
                            }

                        }
                        string imgExtension = Path.GetExtension(empInfoDTO.Image.FileName);
                        Guid imgGuid = Guid.NewGuid();
                        string imgName = imgGuid + imgExtension;
                        employee.ImagePath = "\\Images2\\employee\\" + imgName;
                        string imgFullPath = _webHostEnvironment.WebRootPath + employee.ImagePath;
                        FileStream imgStream = new FileStream(imgFullPath, FileMode.Create);
                        empInfoDTO.Image.CopyTo(imgStream);
                        imgStream.Dispose();

                    }
                    _context.Employees.Update(employee);
                    _context.SaveChanges();
                    //bool state = true;
                    return Ok(employee); //state was here

                }
               
            }
            return BadRequest(ModelState);

        }

        [HttpDelete("deleteempinfo")]
        [Authorize(Roles ="Admin")]
        public IActionResult DeleteEmp (int id )
        {
            if (ModelState.IsValid)
            { 
                Employee Emp = _context.Employees.FirstOrDefault(x=>x.Id==id);
                if (Emp!=null)
                {
                    if (Emp.ImagePath != "\\images\\NIC.jpg")
                    {
                        string imgfullpath = _webHostEnvironment.WebRootPath + Emp.ImagePath;
                        if (System.IO.File.Exists(imgfullpath))
                        {
                            System.IO.File.Delete(imgfullpath);

                        }

                    }
                    _context.Employees.Remove(Emp);
                    _context.SaveChanges();
                    //bool state = true;
                    return Ok(Emp); //state was here
                }
                return BadRequest(new { message = "InvalidId" });

            }
            return BadRequest(ModelState);
        }
        [HttpGet("empattend")]
        [Authorize(Roles ="Admin,Employee")]
        public IActionResult EmpAttendList ()
        {
            Claim IdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            DateTime dateTime = DateTime.Now;
            string userId = IdClaim.Value;
            var history = _context.Historys.Select(x => new { x.UserId, x.Time, x.Day, x.Month, x.IsAttended, x.IsLated })
                .Where(x => x.UserId == userId && x.Month == dateTime.ToString("MM")).AsQueryable();
            return Ok(history);

        }


    }
}

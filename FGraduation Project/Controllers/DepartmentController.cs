using FGraduation_Project.Contexts;
using FGraduation_Project.DTO;
using FGraduation_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FGraduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class DepartmentController : ControllerBase
    {
         
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("adddept")]
        public IActionResult AddDepartment (DeptInfoDTO deptInfoDTO)
        {
            if(ModelState.IsValid) 
            {
                 Department department = new Department();
                 department.Name = deptInfoDTO.Name;
                 department.Manager=deptInfoDTO.Manager;
                 _context.Departments.Add(department);
                 _context.SaveChanges();
                //bool state = true;
                return Ok(department); //state was here


            }
            return BadRequest(ModelState);
        }
       
        [HttpGet("getalldept")]
        public IActionResult GetAllDepartment()
        {
            if (ModelState.IsValid)
            {
                List<Department> departments = _context.Departments.ToList();
                if (departments.Count > 0) return Ok(departments);
                return NotFound(new {message = "NotFoundDept" });
            }
            return BadRequest(ModelState);
            
        }


        [HttpGet("getdeptwithempname")]
        public IActionResult GetDeptWithEmpName(string name)
        {
            if (ModelState.IsValid)
            {
                Department department = _context.Departments.Include(x=>x.Employees).FirstOrDefault(x=>x.Name==name);
                if (department == null) return NotFound(new { message="NotFoundDept" });

               
                DeptDetailsWithEmpNameDTO deptwithemp = new DeptDetailsWithEmpNameDTO();
                deptwithemp.Name = department.Name;
                deptwithemp.Id = department.Id;
                deptwithemp.Manager=department.Manager;

                
                foreach (var item in department.Employees)
                {
                    deptwithemp.EmpNames.Add(item.FirstName);
                }
                

                return Ok(deptwithemp);
            }
            return BadRequest(ModelState);
           
        }

        [HttpGet("getdeptbyid")]
        public IActionResult GetDeptById (int id) 
        {
            if (ModelState.IsValid)
            {
                if (id == 0) BadRequest();
                Department department = _context.Departments.FirstOrDefault(x=>x.Id==id);
                if (department == null) return NotFound(new { message = "NotFoundDept" });
                return Ok(department);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("editdept")]
        public IActionResult EditDept (int id , DeptInfoDTO deptInfoDTO) 
        { 
            if (ModelState.IsValid)
            {
                Department department =_context.Departments.FirstOrDefault(x=>x.Id==id);
                if (department == null) return NotFound(new { message = "NotFoundDept" });
                department.Name = deptInfoDTO.Name;
                department.Manager=deptInfoDTO.Manager;
                _context.Departments.Update(department);
                _context.SaveChanges();
                //bool state = true;
                return Ok(department); //state was here
            }
          return BadRequest();
        }
        [HttpDelete("deletedepartment")]
        public IActionResult DeleteDepartment(int id) 
        {
            Department department= _context.Departments.FirstOrDefault(x=>x.Id==id);
            if (department == null) return NotFound( new { message = "NotFoundDept"});
            _context.Departments.Remove(department);
            _context.SaveChanges();
            //bool state = true;
            return Ok(department); //state was here
        
        
        }
    }
}

using FGraduation_Project.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FGraduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  //  [Authorize(Roles="Admin")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole (RoleDTO roleDTO)
        {
            if (ModelState.IsValid) 
            { 
                IdentityRole identityRole = new IdentityRole();
                identityRole.Name = roleDTO.RoleName;
                IdentityResult identityResult = await roleManager.CreateAsync(identityRole);
                
                if (identityResult.Succeeded)
                {
                    return Ok(new { message = "Role Created" });
                }
                else
                {
                    foreach (var item in identityResult.Errors) 
                    {
                        ModelState.AddModelError("", item.Description);
                    
                    }
                }
                        
            
            }
            return BadRequest(ModelState);

        }
        
    }
}

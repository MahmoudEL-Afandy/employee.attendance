using FGraduation_Project.Contexts;
using FGraduation_Project.DTO;
using FGraduation_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using ZXing;

namespace FGraduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender _emailSender;
         
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext _context;
       
      //  private readonly IEmailServices _emailServices;
        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration , ApplicationDbContext context , IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this._context = context;
            this._emailSender = emailSender;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterAccountDTO UserDto)
        {
            if (ModelState.IsValid) 
            {
                ApplicationUser user = new ApplicationUser();
                
                user.UserName = UserDto.UserName;
                user.PhoneNumber = UserDto.Phone;
                user.Email = UserDto.Email;
               
                  IdentityResult result = await userManager.CreateAsync(user,UserDto.Password);
                if (result.Succeeded) 
                {
                    await userManager.AddToRoleAsync(user, "Employee");
                    Employee employee = new Employee();
                    var dept = _context.Departments.FirstOrDefault();
                    //bool state = true;
                    employee.FirstName = UserDto.FirstName;
                    employee.LastName = UserDto.LastName;
                    employee.UserName = UserDto.UserName;
                    employee.DepartmentId = dept.Id;
                    employee.AccountId = user.Id;
                    _context.Employees.Add(employee);
                    _context.SaveChanges();

                    return Ok(result);
                }
                foreach (var item in result.Errors) 
                {
                    return BadRequest(item);
                }
            }
            return BadRequest(ModelState);

        }
        [HttpPost("AddAdmin")]
       // [Authorize(Roles ="Admin")] 
        public async Task<IActionResult> AddNewAdmin (RegisterAccountDTO UserDto)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = UserDto.UserName;
                user.PhoneNumber = UserDto.Phone;
                user.Email = UserDto.Email;
                IdentityResult result = await userManager.CreateAsync(user, UserDto.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    Employee employee = new Employee();
                    var dept = _context.Departments.FirstOrDefault();
                    //bool state = true;
                    employee.FirstName = UserDto.FirstName;
                    employee.LastName = UserDto.LastName;
                    employee.UserName = UserDto.UserName;
                    employee.DepartmentId = dept.Id;
                    employee.AccountId = user.Id;
                    _context.Employees.Add(employee);
                    _context.SaveChanges();
                    return Ok(result);
                }
                foreach (var item in result.Errors)
                {
                    return BadRequest(item);
                }
            }
            return BadRequest(ModelState);

        }

        [HttpPost("login")]
        public async Task<IActionResult> login(LoginUserDTO userDTO)
        {
            if(ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByNameAsync(userDTO.UserName);
                if (user != null) 
                { 
                   
                    bool found = await userManager.CheckPasswordAsync(user,userDTO.Password);
                    if (found==true )
                    {
                        // After the user Login we Must create the token(issuer, Audiance, claim, signingCredentials


                        // 1- Claim
                        var myclaim = new List<Claim>();
                        myclaim.Add(new Claim(ClaimTypes.Name, user.UserName));
                        myclaim.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        myclaim.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        var role = await userManager.GetRolesAsync(user);
                        foreach (var itemRole in role)
                        {
                            myclaim.Add(new Claim(ClaimTypes.Role, itemRole));

                        }
                        // 2- SigningCredentials 
                        // we should pass the secret KEY(string) and the key must be in byte so we will convert it
                        // and this key will use to Generate the signature  
                        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
                        SigningCredentials signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                        //
                        // then all this Component we should make it in this JWTSecurityToken then Create token 
                        JwtSecurityToken myToken = new JwtSecurityToken(
                           issuer: configuration["JWT:ValidIssuer"], //url Web api provider
                           audience: configuration["JWT:ValidAudience"], //url Consumer (usually published in this url)
                           claims: myclaim,
                           expires: DateTime.Now.AddHours(12),
                           signingCredentials: signingCred
                           );
                        /*  return Ok(new
                          {
                              token = new JwtSecurityTokenHandler().WriteToken(myToken)// here the Token Will Generate 
                              //expiration = myToken.ValidTo
                          });
                        */
                        Employee employee = _context.Employees.FirstOrDefault(x => x.AccountId == user.Id);
                        if (employee == null)
                        {
                            employee.AccountId = user.Id;
                        }
                        var token = new JwtSecurityTokenHandler().WriteToken(myToken);
                        
                      return Ok(new { token , role });
                    }
                    return BadRequest(new { message = "Incorrect Passward" });

                }
                return BadRequest(new { message = "Check Username" });
            }
            return BadRequest(ModelState);

        }



        [HttpPut("changepass")]
        [Authorize(Roles ="Employee,Admin")]
        public async Task<IActionResult> ChangePass (ChangePassDTO changePass)
        {
            if (ModelState.IsValid) 
            {
               // bool state = true;
                string UserName = User.Identity.Name;
                var user = await userManager.FindByNameAsync(UserName);
                var newPass = await userManager.ChangePasswordAsync(user, changePass.CurrentPassword, changePass.Password);
                if (!newPass.Succeeded)
                {
                    return Unauthorized(newPass.Errors);
                }
                return Ok(newPass); //state was here
            
            }
            return BadRequest(ModelState);
        }
     /*
        [HttpGet]
        public async Task<IActionResult> EmailTest ()
        {
            string email = "";
            string subject = "Test";
            string htmlMessage = "<h1>Welcome</h1>";
            await _emailSender.SendEmailAsync(email, subject, htmlMessage);

            return Ok("Email Sent");

        }
     */
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword (ForgotPasswordDTO forgotPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(forgotPassword.UserName);
                if (user != null)
                {
                    string email = user.Email;
                   // Claim calimId = User.Identity.
                    var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                    // var resetUrl = Url.Action("ResetPassword", "Account",new {token=resetToken, userId = user.Id },Request.Scheme);
                    var passResetUrl = $"{forgotPassword.NaviOFResetScreen}?username={forgotPassword.UserName}&token={resetToken}";
                    await _emailSender.SendEmailAsync(user.Email, "Reset your password",$"<p>Hi {user.Email},</p><p>To Reset Your Password Please <a href={passResetUrl}>Click Here </a></p>");
                    return Ok(resetToken);
                }
                return BadRequest(new { message = "UserNotFound" });

            }
            return BadRequest(ModelState);

        }
    
    
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword (ResetPassDTO resetPass)
        {
            if (ModelState.IsValid) 
            {
                var user = await userManager.FindByNameAsync (resetPass.UserName);
                if (user != null) 
                {
                   // var newToken = resetPass.Token.Replace(" ", "+");
                    var resetResult = await userManager.ResetPasswordAsync(user, resetPass.Token, resetPass.Password);
                
                    if (resetResult.Succeeded)
                    {
                        //bool state = true;
                        return Ok(resetResult);
                    }
                    return BadRequest(resetResult.Errors);

                }
                return NotFound(new { message = "UserNotFound" });
            }
            return BadRequest(ModelState);
        }
    

    }
}

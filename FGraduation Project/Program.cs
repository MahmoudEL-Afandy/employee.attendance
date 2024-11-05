using FGraduation_Project.Contexts;
using FGraduation_Project.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.UI.Services;

using Microsoft.OpenApi.Models;
using System.Text;
//using SendEmailManagement.Models;
//using NETCore.MailKit.Core;

var builder = WebApplication.CreateBuilder(args);

// Connection string 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CSFGP"));
});
builder.Services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders(); // this to active registration
builder.Services.Configure<DataProtectionTokenProviderOptions>(op => op.TokenLifespan = TimeSpan.FromHours(10));
builder.Services.AddScoped<IEmailSender, EmailSender>();
// we must add the following statement to use the Func that is Work under The Authontication (register Authontication)
builder.Services.AddAuthentication(options =>
{
// the following statement to make the authontication use the jwt not the Cookies and confirm the user login and have token 
options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Check if it has token or not 
options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // if not has token will redirect the user to login
options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // to make the Defaultscheme be jwt
}).AddJwtBearer(options =>
{
// the following statement to check the the token is true or not 
options.SaveToken = true; // not expired
options.RequireHttpsMetadata = false; // the type of Protocol http or https 
options.TokenValidationParameters = new TokenValidationParameters() //ValidIssuer": "http://localhost:5025/",
    //"ValidAudience
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
}
);
// add email configs
//var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguartion>();
//builder.Services.AddSingleton(emailConfig);
//builder.Services.AddScoped<IEmailService,EmailService>();
// Add services to the container.

builder.Services.AddControllers();
// policy 
builder.Services.AddCors(corsOptions => {
    corsOptions.AddPolicy("MyPolicy", corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
// this statement for setting the swagger to make it work with authontication 
builder.Services.AddSwaggerGen(c =>
{
c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo", Version = "v1" });
});
builder.Services.AddSwaggerGen(swagger =>
{
//This is to generate the Default UI of Swagger Documentation    
swagger.SwaggerDoc("v2", new OpenApiInfo
{
    Version = "v1",
    Title = "ASP.NET 7 Web API",
    Description = " ITI Projrcy"
});
// To Enable authorization using Swagger (JWT)    
swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
});
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseHttpsRedirection();
app.UseStaticFiles(); // this for images and html pages 
app.UseAuthentication();
app.UseCors("MyPolicy");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

//app.MapControllers();


app.Run();

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using ZXing.QrCode;
using ZXing;
using FGraduation_Project.Contexts;
using FGraduation_Project.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FGraduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin,Employee")]
    public class QRCodeController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        public QRCodeController(IWebHostEnvironment webHostEnvironment,ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        [HttpGet("Generate")]
        public IActionResult GenerateQRCode() //IFormCollection formCollection ,, formCollection["QRCodeString"]//string stringDate
        {

            DateTime dateTime = DateTime.Now;
            
            string dayName = dateTime.ToString("dddd");
           if (!(dayName == "Friday" || dayName == "Saturday"))
            {
                Claim claimId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                string userId = claimId.Value;
                History history = _context.Historys.FirstOrDefault(x=>x.UserId == userId && x.Date== dateTime.ToString("MM:dd:yyyy"));
                Employee employee = _context.Employees.FirstOrDefault(x=>x.AccountId== userId);
            

                if (history==null)
                {

                    string timeNow = dateTime.ToString("hh:mm:ss:tt");
                    string stringTime = timeNow;

                    var writer = new QRCodeWriter();
                    var resultBit = writer.encode(stringTime, BarcodeFormat.QR_CODE, 200, 200);
                    var matrix = resultBit;
                    var scale = 2;
                    Bitmap bitmapResult = new Bitmap(matrix.Width * scale, matrix.Height * scale);
                    for (int x = 0; x < matrix.Height; x++)
                    {
                        for (int y = 0; y < matrix.Width; y++)
                        {
                            Color pixel = matrix[x, y] ? Color.Black : Color.White;
                            for (int i = 0; i < scale; i++)
                            {
                                for (int j = 0; j < scale; j++)
                                    bitmapResult.SetPixel(x * scale + i, y * scale + j, pixel);

                            }
                        }
                    }
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    bitmapResult.Save(webRootPath + "\\Images\\QrcodeNew.png");
                    string qrImage = "\\Images\\QrcodeNew.png";
                    return Ok(new {message= qrImage });

                }
                return BadRequest(new { message = "AttendRecorded" });
            }

           var yasterday = dateTime.AddDays(-1).ToString("MM");
            if (yasterday != dateTime.ToString("MM"))
            {
                Claim claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                string userId = claimId.Value;
                Employee employee = _context.Employees.FirstOrDefault(x => x.AccountId == userId);
                employee.MonthlySalary = employee.Salary;
                employee.Date = dateTime.ToString("MM:yyyy");
                employee.Salary = 0;
                _context.Employees.Update(employee);
                _context.SaveChanges();

            }

          return BadRequest(new { message = "DayisWeekend" });

        }

        [HttpPost("Read")]
        public IActionResult ReadQRCode()
        {
           
            var webRootpath = _webHostEnvironment.WebRootPath;
            var path = webRootpath + "\\Images\\QrcodeNew.png";
            var reader = new BarcodeReaderGeneric();
            Bitmap bitmapImage = (Bitmap)Image.FromFile(path);
            string imageText;
            using  (bitmapImage)
            {
                LuminanceSource source;
                source = new ZXing.Windows.Compatibility.BitmapLuminanceSource(bitmapImage);

                Result result = reader.Decode(source);
                 imageText = result.Text;
             }
            DateTime dateTimeNow = DateTime.Now;
            string timeCompare = "10:00:00";
            string morningOrNight = dateTimeNow.ToString("tt");
            string time = dateTimeNow.ToString("hh:mm:ss");
            TimeOnly timeOnlyNow = TimeOnly.Parse(time);
            TimeOnly dateTime1 = TimeOnly.Parse(timeCompare);
            DateTime today = DateTime.Today;
            DateTime yasterday = today.AddDays(-1);



            Claim claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            string userId = claimId.Value;
            Employee employee = _context.Employees.FirstOrDefault(x => x.AccountId == userId);
            
            History history = new History();

            if (!(dateTime1 <= timeOnlyNow && morningOrNight=="PM"))
            {

                // History history1 = new History();
                history.IsAttended = true;
                history.NoAttended = false;
                history.EmployeeId = employee.Id;
                history.UserId = userId;
                // history1.Day = dateTime2.ToString("dddd");
                // history1.Time = dateTime2.ToString("hh:mm:ss:tt");
                //history1.Date = dateTime2.ToString("MM:dd:yyyy");
                if (today.ToString("MM") == yasterday.ToString("MM"))
                {
                    
                    employee.Salary += 300;
                }
                else
                {
                    employee.MonthlySalary = employee.Salary;
                    employee.Date = dateTimeNow.ToString("MM:yyyy");
                    employee.Salary = 0;
                    employee.Salary += 300;
                }
                _context.Employees.Update(employee);
                _context.Historys.Add(history);
                _context.SaveChanges();
                bool state = true;
                return Ok(employee); //check here (state was here)
            }
            history.IsLated = true;
            history.NoAttended = false;
            history.EmployeeId = employee.Id;
            history.UserId = userId;
            // employee.Salary += 200;
            if (today.ToString("MM") == yasterday.ToString("MM"))
            {
                employee.Salary += 200;
            }
            else
            {
                employee.MonthlySalary = employee.Salary;
                employee.Date = dateTimeNow.ToString("MM:yyyy");
                employee.Salary = 0;
                employee.Salary += 200;
            }
            _context.Employees.Update(employee);
            _context.Historys.Add(history);
            _context.SaveChanges();
            bool state1 = true;
            return Ok(employee); //chech here (state was here)
            }


        

    }
}

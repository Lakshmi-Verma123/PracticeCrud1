using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PracticeCrud1.Common.Repository;
using PracticeCrud1.Models;
using PracticeCrud1.Common; 
using Microsoft.AspNetCore.Http;

namespace PracticeCrud1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContactService _contactService;
        private readonly EmailService _emailService;

        public HomeController(IContactService contactService, EmailService emailService)
        {
            _contactService = contactService;
            _emailService = emailService;
        }

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public IActionResult ContactUs() => View();

       
        [HttpPost]
        public async Task<IActionResult> SendOtp(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { status = 400, message = "Email required" });

            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("EmailOtp", otp);
            HttpContext.Session.SetString("OtpEmail", email);
            HttpContext.Session.SetString("OtpExpiry", DateTime.UtcNow.AddMinutes(10).ToString("O"));

            await _emailService.SendOtpAsync(email, otp);
            return Ok(new { status = 200, message = "OTP sent to your email" });
        }

        [HttpPost]
        public IActionResult VerifyOtp(string email, string otp)
        {
            var sessionOtp = HttpContext.Session.GetString("EmailOtp") ?? "";
            var sessionEmail = HttpContext.Session.GetString("OtpEmail") ?? "";
            var expiryString = HttpContext.Session.GetString("OtpExpiry") ?? "";

            sessionEmail = sessionEmail.Trim().ToLower();
            email = email.Trim().ToLower();
            otp = otp.Trim();

            if (DateTime.TryParse(expiryString, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime expiryTime))
            {
                if (DateTime.UtcNow > expiryTime)
                    return Ok(new { status = 400, message = "OTP expired. Please request a new one." });
            }
            else return Ok(new { status = 400, message = "OTP invalid. Please request again." });

            if (sessionOtp == otp && sessionEmail == email)
                return Ok(new { status = 200, message = "Email verified successfully" });
            else
                return Ok(new { status = 400, message = "Invalid OTP" });
        }

        [HttpPost]
        public async Task<IActionResult> SaveStudentData(SaveStudentData request, IFormFile ImageFile)
        {
           
            if (request.Id != 0)
            {
                var oldData = await _contactService.GetStudentById(request.Id);
                if (oldData == null)
                    return Ok(new { statusCode = 404, message = "Record not found" });

                
                if (ImageFile == null)
                {
                    request.ImagePath = oldData.ImagePath;
                    request.ImageOriginalName = oldData.ImageOriginalName;
                }
            }

         
            if (ImageFile != null)
            {
                var existingRows = await _contactService.GetAllInquiryDetails();
                bool duplicate = existingRows.Any(x =>
                    x.ImageOriginalName?.ToLower() == ImageFile.FileName.ToLower() &&
                    x.Id != request.Id);

                if (duplicate)
                    return Ok(new { statusCode = 409, message = "This image is already added in another row" });

           
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                request.ImagePath = fileName;
                request.ImageOriginalName = ImageFile.FileName;
            }


            var res = await _contactService.SaveStudentData(request);
            return Ok(res);
        }

   
        [HttpGet]
        public async Task<IActionResult> GetInquiryDetails(int Id)
        {
            var data = await _contactService.GetInquiryDetails(Id);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudent(int Id)
        {
            var res = await _contactService.DeleteInquiryDetails(Id);
            return Json(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentById(int Id)
        {
            var data = await _contactService.GetStudentById(Id);
            return Ok(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
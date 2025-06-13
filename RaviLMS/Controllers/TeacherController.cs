using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using RaviLMS.Models;
using System.Data;
using System.Collections.Generic;
using RaviLMS.Repositories;


namespace RaviLMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        //public TeacherController(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        public TeacherController(IConfiguration configuration, EmailService emailService, ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Teacher teacher)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");
            try
            {
                await _teacherRepository.RegisterTeacherAsync(teacher);

               
                string subject = "🎉 Registration Received – Awaiting Approval";

                string body = $@"
                    <p style='font-family:Segoe UI, sans-serif; font-size:14px;'>
                        Dear <strong>{teacher.FullName} </strong>, 👋
                    </p>
                    <p style='font-family:Segoe UI, sans-serif; font-size:14px;'>
                        We're excited to welcome you to our <strong>Learning Management System (LMS)</strong> community. 📚<br/>
                        Thank you for registering as a teacher – we truly value your expertise and commitment to education. 🙌
                    </p>
                    <p style='font-family:Segoe UI, sans-serif; font-size:14px;'>
                        Your account has been successfully created and is currently <strong>awaiting admin approval</strong>. 🔐<br/>
                        You’ll be able to access your dashboard in approximately 2 minutes.
                    </p>
                    <p style='font-family:Segoe UI, sans-serif; font-size:14px;'>
                        If you have any questions, feel free to reach out anytime.
                    </p>
                    <br/>
                    <p style='font-family:Segoe UI, sans-serif; font-size:14px;'>
                        Best regards,<br/> 
                        <strong>RHS Team</strong> 🎓
                    </p>";


               
                await _emailService.SendEmailAsync(teacher.Email, subject, body);

                return Ok(new { message = "Teacher registered successfully. Awaiting admin approval." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = " Error occurred",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("approved")]
        public IActionResult GetApprovedTeachers()
        {
            var teachers = _teacherRepository.GetApprovedTeachers();
            return Ok(teachers);

        }
    }
}

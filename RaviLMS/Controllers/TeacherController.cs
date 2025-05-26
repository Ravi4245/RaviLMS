using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using RaviLMS.Models;
using System.Data;
using System.Collections.Generic;
using RaviLMS.Repositories;
//using RaviLMS.Services;

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

                //using (SqlConnection con = new SqlConnection(connectionString))
                //{
                //    string query = "INSERT INTO Teacher (FullName, Email, Password, Status) VALUES (@FullName, @Email, @Password, 'Pending')";

                //    using (SqlCommand cmd = new SqlCommand(query, con))
                //    {
                //        cmd.Parameters.AddWithValue("@FullName", teacher.FullName);
                //        cmd.Parameters.AddWithValue("@Email", teacher.Email);
                //        cmd.Parameters.AddWithValue("@Password", teacher.Password);

                //        con.Open();
                //        cmd.ExecuteNonQuery();
                //    }
                //}
                // Compose email
                string subject = "Registration Successful - Awaiting Approval";
                string body = $"<p>Dear {teacher.FullName},</p>" +
                              "<p>Thank you for registering in our Learning managment system. Your account is currently awaiting admin approval.</p>" +
                              "<p>You can access your account after 2 min.</p>" +
                              "<p> </p>" +
                              "<br/><p>Regards,<br/>LMS Team</p>";

                // Send email (async)
                await _emailService.SendEmailAsync(teacher.Email, subject, body);

                return Ok(new { message = "Teacher registered successfully. Awaiting admin approval." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred",
                    error = ex.Message,
                    stackTrace = ex.StackTrace // ➕ helps you identify the error line
                });
            }
        }

        [HttpGet("approved")]
        public IActionResult GetApprovedTeachers()
        {
            var teachers = _teacherRepository.GetApprovedTeachers();
            return Ok(teachers);


            //string connectionString = _configuration.GetConnectionString("LMSDB");
            //List<Teacher> teachers = new List<Teacher>();

            //using (SqlConnection con = new SqlConnection(connectionString))
            //{
            //    string query = "SELECT * FROM Teacher WHERE Status = 'Approved'";

            //    using (SqlCommand cmd = new SqlCommand(query, con))
            //    {
            //        con.Open();
            //        SqlDataReader reader = cmd.ExecuteReader();

            //        while (reader.Read())
            //        {
            //            teachers.Add(new Teacher
            //            {
            //                TeacherId = Convert.ToInt32(reader["TeacherId"]),
            //                FullName = reader["FullName"].ToString(),
            //                Email = reader["Email"].ToString()
            //            });
            //        }
            //    }
            //}

            //return Ok(teachers);
        }
    }
}

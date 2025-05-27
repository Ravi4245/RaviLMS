using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using RaviLMS.Models; // <-- Your model namespace
using System.Data;
using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
//using RaviLMS.Services;

namespace RaviLMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly EmailService _emailService;

        //public StudentController(IConfiguration configuration)
        //{
        //    _configuration = configuration;

        //}

        public StudentController(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Student student)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string queryy = "INSERT INTO Student (FullName, Email, Password, Status) VALUES (@FullName, @Email, @Password, 'Pending')";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FullName", student.FullName);
                        cmd.Parameters.AddWithValue("@Email", student.Email);
                        cmd.Parameters.AddWithValue("@Password", student.Password);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                }
                // Compose email
               string subject = "✅ Registration Successful – Awaiting Approval";

               string body = $@"
               <p style='font-family:Segoe UI, sans-serif; font-size:14px;'>
                        Dear <strong>{student.FullName}</strong>, 👋
                    </p>
                    <p style='font-family:Segoe UI, sans-serif; font-size:14px;'>
                        Thank you for registering with our <strong>Learning Management System (LMS)</strong>. 🎓<br/>
                        Your account has been successfully created and is currently <strong>awaiting admin approval</strong>. 🔐
                    </p>
                    <p style='font-family:Segoe UI, sans-serif; font-size:14px;'>
                        ⏳ Please allow a few minutes (approx. 2 minutes) for the system to process your access.<br/>
                        You’ll be able to log in once your registration is approved.
                    </p>
                    <p style='font-family:Segoe UI, sans-serif; font-size:14px;'>
                        If you have any questions, feel free to reply to this email.
                    </p>
                    <br/>
                    <p style='font-family:Segoe UI, sans-serif; font-size:14px;'>
                        Best regards,<br/>
                        <strong>RHS Team</strong> 💼
               </p>";


                // Send email (async)
                await _emailService.SendEmailAsync(student.Email, subject, body);

                return Ok(new { message = "Student registered successfully. Awaiting admin approval." });
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
        public IActionResult GetApprovedStudents()
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");
            List<Student> students = new List<Student>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Student WHERE Status = 'Approved'";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            StudentId = Convert.ToInt32(reader["StudentId"]),
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString()
                        });
                    }
                }
            }

            return Ok(students);
        }

        [HttpGet("all")]
        public IActionResult GetAllStudents()
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");
            List<Student> students = new List<Student>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Student";  // No WHERE clause, get all students

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            StudentId = Convert.ToInt32(reader["StudentId"]),
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString()
                            // Add other fields if needed
                        });
                    }
                }
            }

            return Ok(students);
        }


    }
}

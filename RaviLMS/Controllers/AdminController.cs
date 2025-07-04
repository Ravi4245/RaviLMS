﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using RaviLMS.Models;

namespace RaviLMS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;


        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] Admin admin)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Admin WHERE Email = @Email AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", admin.Email);
                    cmd.Parameters.AddWithValue("@Password", admin.Password);

                    con.Open();
                    int count = (int)cmd.ExecuteScalar();

                    if (count == 1)
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, admin.Email),
                            new Claim(ClaimTypes.Role, "Admin")
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: _configuration["Jwt:Issuer"],
                            audience: _configuration["Jwt:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials: creds
                        );

                        return Ok(new
                        {
                            success = true,
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            role = "Admin",
                            id = 0
                        });
                    }
                    else
                    {
                        return Unauthorized(new { success = false, message = "Invalid user credentials" });
                    }
                }
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("approve/student/{studentId}")]
        public IActionResult ApproveStudent(int studentId)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Student SET Status = 'Approved' WHERE StudentId = @StudentId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 1)
                    {
                        return Ok(new { message = "Student approved successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Student not found" });
                    }
                }
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("approve/teacher/{teacherId}")]
        public IActionResult ApproveTeacher(int teacherId)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Teacher SET Status = 'Approved' WHERE TeacherId = @TeacherId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 1)
                    {
                        return Ok(new { message = "Teacher approved successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Teacher does not found " });
                    }
                }
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("students/pending")]
        public IActionResult GetPendingStudents()
        {
            var pendingStudents = new List<Student>();
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT StudentId, FullName, Email FROM Student WHERE Status = 'Pending'";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        pendingStudents.Add(new Student
                        {
                            StudentId = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Email = reader.GetString(2)
                        });
                    }
                }
            }

            return Ok(pendingStudents);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("teachers/pending")]
        public IActionResult GetPendingTeachers()
        {
            var pendingTeachers = new List<Teacher>();
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT TeacherId, FullName, Email FROM Teacher WHERE Status = 'Pending'";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        pendingTeachers.Add(new Teacher
                        {
                            TeacherId = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Email = reader.GetString(2)
                        });
                    }
                }
            }

            return Ok(pendingTeachers);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("student/{studentId}")]
        public IActionResult DeleteStudent(int studentId)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Student WHERE StudentId = @StudentId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 1)
                    {
                        return Ok(new { message = "Student deleted successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Student not found" });
                    }
                }
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("teacher/{teacherId}")]
        public IActionResult DeleteTeacher(int teacherId)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Teacher WHERE TeacherId = @TeacherId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 1)
                    {
                        return Ok(new { message = "Teacher deleted successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Teacher not found" });
                    }
                }
            }
        }


        // Get all courses (or pending courses if you want filtering)
        [Authorize(Roles = "Admin")]
        [HttpGet("courses")]
        public IActionResult GetCourses()
        {
            var courses = new List<Course>();
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT CourseId, CourseName, Description,TeacherId  FROM Course";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            CourseId = reader.GetInt32(0),
                            CourseName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            TeacherId = reader.GetInt32(3)
                        });
                    }
                }
            }

            return Ok(courses);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpDelete("course/{courseId}")]
        public IActionResult DeleteCourse(int courseId)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Course WHERE CourseId = @CourseId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CourseId", courseId);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 1)
                    {
                        return Ok(new { message = "Course deleted successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Course not found" });
                    }
                }
            }
        }

       
        [Authorize(Roles = "Admin")]
        [HttpPut("course/{courseId}")]
        public IActionResult UpdateCourse(int courseId, [FromBody] Course updatedCourse)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Course SET CourseName = @CourseName, Description = @Description WHERE CourseId = @CourseId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CourseName", updatedCourse.CourseName);
                    cmd.Parameters.AddWithValue("@Description", updatedCourse.Description);
                    cmd.Parameters.AddWithValue("@CourseId", courseId);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 1)
                    {
                        return Ok(new { message = "Course updated successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Course not found" });
                    }
                }
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("teachers/approved")]
        public IActionResult GetApprovedTeachers()
        {
            var approvedTeachers = new List<Teacher>();
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT TeacherId, FullName, Email FROM Teacher WHERE Status = 'Approved'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        approvedTeachers.Add(new Teacher
                        {
                            TeacherId = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Email = reader.GetString(2)
                        });
                    }
                }
            }

            return Ok(approvedTeachers);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("students/approved")]
        public IActionResult GetApprovedStudents()
        {
            var approvedStudents = new List<Student>();
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT StudentId, FullName, Email FROM Student WHERE Status = 'Approved'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        approvedStudents.Add(new Student
                        {
                            StudentId = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Email = reader.GetString(2)
                        });
                    }
                }
            }

            return Ok(approvedStudents);
        }



        [Authorize(Roles = "Admin")]
        [HttpGet("counts")]
        public IActionResult GetCounts()
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");
            int pendingStudents = 0, approvedStudents = 0, pendingTeachers = 0, approvedTeachers = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Student WHERE Status = 'Pending'", con))
                {
                    pendingStudents = (int)cmd.ExecuteScalar();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Student WHERE Status = 'Approved'", con))
                {
                    approvedStudents = (int)cmd.ExecuteScalar();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Teacher WHERE Status = 'Pending'", con))
                {
                    pendingTeachers = (int)cmd.ExecuteScalar();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Teacher WHERE Status = 'Approved'", con))
                {
                    approvedTeachers = (int)cmd.ExecuteScalar();
                }
            }



            return Ok(new
            {
                pendingStudents,
                approvedStudents,
                pendingTeachers,
                approvedTeachers
            });
        }
    }
}

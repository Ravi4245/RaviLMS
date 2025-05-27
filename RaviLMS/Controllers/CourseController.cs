using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using RaviLMS.Models;
using System.Data;
using System.Collections.Generic;

namespace RaviLMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CourseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Add new course (by teacher)
        [HttpPost("add")]
        public IActionResult AddCourse([FromBody] Course course)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Course (CourseName,Description, TeacherId) VALUES (@CourseName,@Description, @TeacherId)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CourseName", course.CourseName);
                    cmd.Parameters.AddWithValue("@Description", course.Description);
                  cmd.Parameters.AddWithValue("@TeacherId", course.TeacherId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return Ok(new { message = "Course Successfully Created" });
        }

        // Get courses by teacher id
        [HttpGet("byTeacher/{teacherId}")]
        public IActionResult GetCoursesByTeacher(int teacherId)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");
            List<Course> courses = new List<Course>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Course WHERE TeacherId = @TeacherId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                   cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            CourseId = Convert.ToInt32(reader["CourseId"]),
                           CourseName = reader["CourseName"].ToString(),
                            TeacherId = Convert.ToInt32(reader["TeacherId"])
                        });
                    }
                }
            }

            return Ok(courses);
        }
    }
}

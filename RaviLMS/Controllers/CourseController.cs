using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using RaviLMS.Models;
using System.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

        

      [HttpGet("student/{studentId}")]
       public async Task<ActionResult<IEnumerable<Course>>> GetCoursesByStudent(int studentId)
       {
                var courses = new List<Course>();

     
                string connectionString = "Data Source=DESKTOP-5S4O1AS;Initial Catalog=RaviLms;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

                string query = @"
                SELECT c.CourseId, c.CourseName, c.Description, c.TeacherId
                FROM StudentCourse sc
                INNER JOIN Course c ON sc.CourseId = c.CourseId
                WHERE sc.StudentId = @StudentId";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@StudentId", studentId);

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var course = new Course
                        {
                            CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                            CourseName = reader.GetString(reader.GetOrdinal("CourseName")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            TeacherId = reader.GetInt32(reader.GetOrdinal("TeacherId"))
                        };

                        courses.Add(course);
                    }
                }
            }
        }

        return Ok(courses);
    }

}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using RaviLMS.Models;
using System.Collections.Generic;
using System.Data;

namespace RaviLMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AssignmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ✅ Add a new assignment
        [HttpPost("add")]
        public IActionResult AddAssignment([FromBody] Assignment assignment)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("LMSDB");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Assignment (Title, Description,CourseId, StudentId)
                                 VALUES (@Title,@Description, @CourseId, @StudentId )";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Title", assignment.Title);
                        cmd.Parameters.AddWithValue("@Description", assignment.Description);
                        cmd.Parameters.AddWithValue("@CourseId", assignment.CourseId);
                        cmd.Parameters.AddWithValue("@StudentId", assignment.StudentId);
                        //cmd.Parameters.AddWithValue("@Grade", string.IsNullOrEmpty(assignment.Grade) ? DBNull.Value : assignment.Grade);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok(new { message = "Assignment created successfully" });
            }
            catch (Exception ex)
            {
                // Log exception message here or return it
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ✅ Get all assignments submitted by a student
        [HttpGet("byStudent/{studentId}")]
        public IActionResult GetAssignmentsByStudent(int studentId)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");
            List<Assignment> assignments = new List<Assignment>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Assignment WHERE StudentId = @StudentId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        assignments.Add(new Assignment
                        {
                            AssignmentId = Convert.ToInt32(reader["AssignmentId"]),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            CourseId = Convert.ToInt32(reader["CourseId"]),
                            StudentId = Convert.ToInt32(reader["StudentId"]),
                            //Grade = reader["Grade"] == DBNull.Value ? null : reader["Grade"].ToString()
                        });
                    }
                }
            }

           
        }



    }
}

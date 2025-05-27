using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RaviLMS.Models;
using System.Data.SqlClient;

namespace RaviLMS.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly IConfiguration _configuration;

        public TeacherRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> RegisterTeacherAsync(Teacher teacher)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Teacher (FullName, Email, Password, Status) VALUES (@FullName, @Email, @Password, 'Pending')";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FullName", teacher.FullName);
                    cmd.Parameters.AddWithValue("@Email", teacher.Email);
                    cmd.Parameters.AddWithValue("@Password", teacher.Password);

                    con.Open();
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
            }
        }

        public List<Teacher> GetApprovedTeachers()
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");
            List<Teacher> teachers = new List<Teacher>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Teacher WHERE Status = 'Approved'";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        teachers.Add(new Teacher
                        {
                            TeacherId = Convert.ToInt32(reader["TeacherId"]),
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString()
                        });
                    }
                }
            }

        }
    }
}


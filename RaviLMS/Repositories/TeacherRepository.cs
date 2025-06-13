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
                string query = @"
            INSERT INTO Teacher 
                (FullName, Email, Password, Status, DateOfBirth, PhoneNumber, City) 
            VALUES 
                (@FullName, @Email, @Password, 'Pending', @DateOfBirth, @PhoneNumber, @City)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FullName", teacher.FullName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", teacher.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", teacher.Password ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfBirth", teacher.DateOfBirth ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", teacher.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", teacher.City ?? (object)DBNull.Value);

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

            return teachers;
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RaviLMS.Models;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RaviLMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string connectionString = _configuration.GetConnectionString("LMSDB");

            string tableName = request.Role switch
            {
                "Student" => "Student",
                "Teacher" => "Teacher",
                "Admin" => "Admin",
                _ => null
            };

            if (tableName == null)
                return BadRequest("Invalid role provided.");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = request.Role == "Admin"
                    ? $"SELECT * FROM {tableName} WHERE Email = @Email AND Password = @Password"
                    : $"SELECT * FROM {tableName} WHERE Email = @Email AND Password = @Password AND Status = 'Approved'";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", request.Email);
                    cmd.Parameters.AddWithValue("@Password", request.Password);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int id = request.Role switch
                        {
                            "Student" => (int)reader["StudentId"],
                            "Teacher" => (int)reader["TeacherId"],
                            "Admin" => (int)reader["AdminId"],
                            _ => 0
                        };

                        string token = GenerateJwtToken(request.Email, request.Role);

                        return Ok(new
                        {
                            success = true,
                            role = request.Role,
                            id = id,
                            token = token,
                            message = $"{request.Role} login successful."
                        });
                    }
                    else
                    {
                        return Unauthorized(new
                        {
                            success = false,
                            message = "Invalid credentials or not approved yet."
                        });
                    }
                }
            }
        }

        private string GenerateJwtToken(string email, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

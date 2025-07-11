
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;


namespace SafeVault
{
    public class RegisterUser : PageModel
    {
        private readonly string _connectionString;

        

        public RegisterUser(string connectionString)
        {
            _connectionString = connectionString;
            
        }

        public async Task<bool> RegisterUserAsync(string username, string password)
        {
            // Hash the password using SHA256
            using var sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);
            string passwordHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            const string query = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

            await connection.OpenAsync();
            try
            {
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected == 1;
            }
            catch (SqlException)
            {
                // Handle duplicate username or other DB errors as needed
                return false;
            }

        }
    }
}

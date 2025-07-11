using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace SafeVault
{
    public class LoginUser: PageModel
    {
        private readonly string _connectionString;
        
        public LoginUser(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            string allowedSpecialCharacters = "!@#$%^&*?";
            if (!ValidationHelpers.IsValidInput(username) || !ValidationHelpers.IsValidInput(password, allowedSpecialCharacters))

                return false;

            const string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND PasswordHash = HASHBYTES('SHA2_256', @Password)";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                await connection.OpenAsync();
                var result = (int)await command.ExecuteScalarAsync();
                return result == 1;
            }
        }
    }
}

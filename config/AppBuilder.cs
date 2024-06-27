
using Microsoft.EntityFrameworkCore;
using firstORM.data;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace firstORM
{
    public class AppBuilder
    {
        public static WebApplicationBuilder GenerateBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var mysqlVersion = new MySqlServerVersion(new Version(8, 0, 26));

            void AddMySql<TContext>(IServiceCollection services, MySqlServerVersion mysqlVersion) where TContext : DbContext
            {
                services.AddDbContext<TContext>(options =>
                    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), mysqlVersion));
            }
            AddMySql<LevoratechDbContext>(builder.Services, mysqlVersion);

            return builder;
        }
        string GenerateToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.ASCII.GetBytes("abcabcabcabcabcabcabcabcabcabcab");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public void TokenAuth(WebApplication? app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost("/login", async (HttpContext contex) =>
        {
            using var reader = new StreamReader(contex.Request.Body);
            var body = await reader.ReadToEndAsync();

            var json = JsonDocument.Parse(body);
            var username = json.RootElement.GetProperty("username").GetString();
            var email = json.RootElement.GetProperty("email").GetString();
            var senha = json.RootElement.GetProperty("senha").GetString();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<firstORM.data.LevoratechDbContext>();
                var user = dbContext.Users.SingleOrDefault(u => u.nome == username);
                
                

            if (user != null && user.senha == senha && user.email == email )
            {
                var token = GenerateToken();
                return token;
            }
            }
            return "senha invalida";
        });
        }

    }
}

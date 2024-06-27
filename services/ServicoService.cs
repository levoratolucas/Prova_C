namespace firstORM.rota
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;
    using firstORM.models;
    using System.Text.Json;

    public class ServicoService
    {

        LevoratechDbContext _dbContext;
        public ServicoService(LevoratechDbContext db)
        {
            _dbContext = db;
        }
        // Método para consultar todos os Servicos
        public async Task<List<ServicoModel>> GetAllServicosAsync()
        {
            return await _dbContext.Servico.ToListAsync();
        }

        // Método para consultar um Servico a partir do seu Id
        public async Task<ServicoModel> GetServicoByIdAsync(int id)
        {
            return await _dbContext.Servico.FindAsync(id);
        }

        // Método para  gravar um novo Servico
        public async Task AddServicoAsync(ServicoModel Servico)
        {
            _dbContext.Servico.Add(Servico);
            await _dbContext.SaveChangesAsync();
        }
        public async Task update(HttpContext context, WebApplication? app, string nome, decimal preco, bool status, int id)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LevoratechDbContext>();
                var Servico = await dbContext.Servico.FindAsync(id);

                if (Servico != null)
                {
                    Servico.nome = !string.IsNullOrEmpty(nome) ? nome : Servico.nome;
                    Servico.preco = preco != default ? preco : Servico.preco;
                    Servico.status = status;

                    await dbContext.SaveChangesAsync();

                    var ServicoJson = JsonSerializer.Serialize(Servico);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(ServicoJson);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Servico não encontrado");
                }
            }
        }
        // Método para atualizar os dados de um Servico
        public async Task UpdateServicoAsync(int id, ServicoModel Servico)
        {
            _dbContext.Entry(Servico).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        // Método para excluir um Servico
        public async Task DeleteServicoAsync(int id)
        {
            var Servico = await _dbContext.Servico.FindAsync(id);
            if (Servico != null)
            {
                _dbContext.Servico.Remove(Servico);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}

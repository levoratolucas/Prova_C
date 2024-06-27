using firstORM.models;
using Microsoft.EntityFrameworkCore;

namespace firstORM.data
{
    public class LevoratechDbContext : DbContext
    {
        public LevoratechDbContext(DbContextOptions<LevoratechDbContext> options) : base(options) { }

        public DbSet<ServicoModel> Servico { get; set; }
        public DbSet<ClienteModel> Cliente { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ContratoModel> Contrato { get; set; }

        public void AddUserModel(string nome, string email, string senha)
        {
            var newUser = new UserModel
            {
                nome = nome,
                email = email,
                senha = senha
            };
            Users.Add(newUser);
            SaveChanges();
        }
    }
}

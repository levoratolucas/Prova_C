
using firstORM;

public class Program
{
    public static void Main(string[] args)
    {
        AppBuilder auth = new AppBuilder();
        var builder = firstORM.AppBuilder.GenerateBuilder(args);
        var app = builder.Build();
        var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<firstORM.data.LevoratechDbContext>();


        var user = new firstORM.rota.UserRota();
        var clienteRota = new firstORM.rota.ClienteRota(dbContext);
        var ServicoRota = new firstORM.rota.ServicoRota(dbContext);
        var ContratoRota = new firstORM.rota.ContratoRota(dbContext);



        auth.TokenAuth(app);
        user.Rotas(app);
        clienteRota.Rotas(app);
        ServicoRota.Rotas(app);
        ContratoRota.Rotas(app);


        app.Run();
    }
}


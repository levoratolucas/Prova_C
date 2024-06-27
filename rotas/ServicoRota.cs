namespace firstORM.rota
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using firstORM.data;
    using firstORM.config;
    using firstORM.models;
    using System.Text.Json;

    public class ServicoRota
    {



        private ServicoService ServicoService;
        public ServicoRota(LevoratechDbContext db)
        {
            ServicoService = new ServicoService(db);
        }

        public void Rotas(WebApplication? app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost("/servico/adicionar", async (HttpContext context) =>
                {
                    if (!ValToken.ValidateToken(context, out _)) return;

                    using var reader = new System.IO.StreamReader(context.Request.Body);
                    var body = await reader.ReadToEndAsync();
                    var json = JsonDocument.Parse(body);
                    var nome = json.RootElement.GetProperty("nome").GetString();
                    var preco = json.RootElement.GetProperty("preco").GetDecimal();
                    var status = json.RootElement.GetProperty("status").GetBoolean();

                    if (!string.IsNullOrEmpty(nome) && preco > 0)
                    {
                        var Servico = new ServicoModel { nome = nome, preco = preco, status = status };
                        await ServicoService.AddServicoAsync(Servico);

                        var ServicoJson = JsonSerializer.Serialize(Servico);
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(ServicoJson);
                    }
                    else
                    {
                        var dadosJson = json.RootElement.ToString();
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(dadosJson);
                    }
                });


            app.MapGet("/servico/listar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;


                var Servicos = await ServicoService.GetAllServicosAsync();
                await context.Response.WriteAsJsonAsync(Servicos);


            });


            app.MapPost("/servico/procurar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt16();


                var Servicos = await ServicoService.GetServicoByIdAsync(id);
                await context.Response.WriteAsJsonAsync(Servicos);

            });


           app.MapPost("/servico/atualizar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt32();

                var nome = json.RootElement.TryGetProperty("nome", out var nomeProperty) ? nomeProperty.GetString() : string.Empty;
                var preco = json.RootElement.TryGetProperty("preco", out var precoProperty) ? precoProperty.GetDecimal() : default;
                var status = json.RootElement.TryGetProperty("status", out var statusProperty) ? statusProperty.GetBoolean() : false;

                await ServicoService.update(context, app, nome, preco, status, id);
            });


            app.MapPost("/servico/deletar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return;
                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var id = json.RootElement.GetProperty("id").GetInt32();


                var Servicos = await ServicoService.GetServicoByIdAsync(id);
                await ServicoService.DeleteServicoAsync(id);

                await context.Response.WriteAsync("executado");


            });
        }
    }
}

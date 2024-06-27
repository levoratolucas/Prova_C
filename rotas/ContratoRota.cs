using firstORM.services;
using firstORM.data;
using firstORM.models;
using firstORM.config;
using System.Text.Json;

namespace firstORM.rota
{
    public class ContratoRota
    {
        private readonly ContratoService _contratoService;

        public ContratoRota(LevoratechDbContext dbContext)
        {
            _contratoService = new ContratoService(dbContext);
        }

        public void Rotas(WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapPost("/contratos", async (HttpContext context, ContratoModel contrato) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return Results.Unauthorized();
                try
                {
                    var novoContrato = await _contratoService.AddContratoAsync(contrato);
                    return Results.Created($"/contratos/{novoContrato.Id}", novoContrato);
                }
                catch (ArgumentException e)
                {
                    return Results.BadRequest(e.Message);
                }
            });

            app.MapGet("/contratos/servico/sumarizada/{servicoId}", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return Results.Unauthorized();
                var servicoId = int.Parse(context.Request.RouteValues["servicoId"].ToString());
                var contratos = await _contratoService.GetContratosByServicoAgregadaAsync(servicoId);
                return Results.Ok(contratos);
            });

            app.MapGet("/contratos/cliente/detalhada/{clienteId}", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return Results.Unauthorized();
                var clienteId = int.Parse(context.Request.RouteValues["clienteId"].ToString());
                var contratos = await _contratoService.GetContratosByClienteDetalhadaAsync(clienteId);
                return Results.Ok(contratos);
            });

            app.MapGet("/contratos/cliente/sumarizada/{clienteId}", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return Results.Unauthorized();
                var clienteId = int.Parse(context.Request.RouteValues["clienteId"].ToString());
                var contratos = await _contratoService.GetContratosByClienteAgregadaAsync(clienteId);
                return Results.Ok(contratos);
            });

            app.MapPost("/contratar", async (HttpContext context) =>
            {
                if (!ValToken.ValidateToken(context, out _)) return Results.Unauthorized();

                using var reader = new System.IO.StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);
                var clienteId = json.RootElement.GetProperty("clienteId").GetInt32();
                var servicoId = json.RootElement.GetProperty("servicoId").GetInt32();
                var quantidade = json.RootElement.GetProperty("quantidade").GetInt32();

                try
                {
                    var contrato = await _contratoService.ComprarServicoAsync(clienteId, servicoId, quantidade);
                    return Results.Created($"/contratos/{contrato.Id}", contrato);
                }
                catch (ArgumentException e)
                {
                    return Results.BadRequest(e.Message);
                }
            });
        }
    }
}

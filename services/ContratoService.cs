using firstORM.data;
using firstORM.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace firstORM.services
{
    public class ContratoService
    {
        private readonly LevoratechDbContext _dbContext;

        public ContratoService(LevoratechDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ContratoModel> AddContratoAsync(ContratoModel contrato)
        {
            // Validação se o cliente e o serviço existem
            var cliente = await _dbContext.Cliente.FindAsync(contrato.ClienteId);
            var servico = await _dbContext.Servico.FindAsync(contrato.ServicoId);

            if (cliente == null || servico == null)
            {
                throw new ArgumentException("Cliente ou Serviço não encontrado");
            }

            contrato.PrecoUnitario = servico.preco;

            if (contrato.DataVenda == default(DateTime))
            {
                contrato.DataVenda = DateTime.Now;
            }

            if (string.IsNullOrEmpty(contrato.NumeroNotaFiscal))
            {
                contrato.NumeroNotaFiscal = Guid.NewGuid().ToString();
            }

            _dbContext.Contrato.Add(contrato);
            await _dbContext.SaveChangesAsync();
            return contrato;
        }

        public async Task<IEnumerable<object>> GetContratosByServicoDetalhadaAsync(int servicoId)
        {
            return await _dbContext.Contrato
                .Include(c => c.Cliente)
                .Include(c => c.Servico)
                .Where(c => c.ServicoId == servicoId)
                .Select(c => new 
                {
                    c.Id,
                    c.DataVenda,
                    ServicoNome = c.Servico.nome,
                    ClienteNome = c.Cliente.nome,
                    c.QuantidadeVendida,
                    c.PrecoUnitario
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetContratosByServicoAgregadaAsync(int servicoId)
        {
            return await _dbContext.Contrato
                .Include(c => c.Servico)
                .Where(c => c.ServicoId == servicoId)
                .GroupBy(c => new { c.ServicoId, c.Servico.nome })
                .Select(g => new 
                {
                    ServicoNome = g.Key.nome,
                    QuantidadeTotalVendida = g.Sum(c => c.QuantidadeVendida),
                    PrecoTotal = g.Sum(c => c.PrecoUnitario * c.QuantidadeVendida)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetContratosByClienteDetalhadaAsync(int clienteId)
        {
            return await _dbContext.Contrato
                .Include(c => c.Cliente)
                .Include(c => c.Servico)
                .Where(c => c.ClienteId == clienteId)
                .Select(c => new 
                {
                    c.Id,
                    c.DataVenda,
                    ServicoNome = c.Servico.nome,
                    c.QuantidadeVendida,
                    c.PrecoUnitario
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetContratosByClienteAgregadaAsync(int clienteId)
        {
            return await _dbContext.Contrato
                .Include(c => c.Cliente)
                .Where(c => c.ClienteId == clienteId)
                .GroupBy(c => c.ClienteId)
                .Select(g => new 
                {
                    ClienteNome = g.FirstOrDefault().Cliente.nome,
                    QuantidadeTotalVendida = g.Sum(c => c.QuantidadeVendida),
                    PrecoTotal = g.Sum(c => c.PrecoUnitario * c.QuantidadeVendida)
                })
                .ToListAsync();
        }
         public async Task<ContratoModel> ComprarServicoAsync(int clienteId, int servicoId, int quantidade)
        {
            var cliente = await _dbContext.Cliente.FindAsync(clienteId);
            var servico = await _dbContext.Servico.FindAsync(servicoId);

            if (cliente == null || servico == null)
            {
                throw new ArgumentException("Cliente ou Serviço não encontrado");
            }

            var contrato = new ContratoModel
            {
                DataVenda = DateTime.Now,
                NumeroNotaFiscal = Guid.NewGuid().ToString(),
                ClienteId = clienteId,
                ServicoId = servicoId,
                QuantidadeVendida = quantidade,
                PrecoUnitario = servico.preco
            };

            _dbContext.Contrato.Add(contrato);
            await _dbContext.SaveChangesAsync();
            return contrato;
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace firstORM.models
{
    public class ContratoModel
    {
        public int Id { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DataVenda { get; set; }

        public string NumeroNotaFiscal { get; set; }
        public int ClienteId { get; set; }
        public ClienteModel Cliente { get; set; }
        public int ServicoId { get; set; }
        public ServicoModel Servico { get; set; }
        public int QuantidadeHoras { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }
    }
}

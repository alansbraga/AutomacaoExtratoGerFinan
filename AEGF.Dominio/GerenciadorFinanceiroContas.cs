using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Dominio
{
    public class GerenciadorFinanceiroContas
    {
        public int Id { get; set; }
        public string ContaOrigem { get; set; }
        public string ContaDestino { get; set; }
        public bool MudaDataParaMesReferencia { get; set; }
        public bool MultiplicarMenosUm { get; set; }
        public bool IgnorarPositivos { get; set; }

    }
}

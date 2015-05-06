using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;

namespace AEGF.RepositorioJson
{
    public class GerenciadorLocal: GerenciadorFinanceiro
    {
        public new ICollection<GerenciadorFinanceiroConfiguracao> Configuracoes { get; set; }
        public new ICollection<GerenciadorFinanceiroContas> Contas { get; set; }

    }
}

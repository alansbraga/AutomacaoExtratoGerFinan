using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;

namespace AEGF.RepositorioJson
{
    class BancoLocal: Banco
    {
        public new ICollection<BancoConfiguracao> Configuracoes { get; set; }
    }
}

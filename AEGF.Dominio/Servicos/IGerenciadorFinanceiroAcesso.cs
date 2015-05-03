using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Dominio.Servicos
{
    public interface IGerenciadorFinanceiroAcesso
    {
        void ProcessarContas(IEnumerable<Extrato> extratos);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;

namespace AEGF.ServicoAplicacao
{
    public interface IIntegrarServicoAplicacao
    {
        IEnumerable<Extrato> IntegrarContas();
    }
}

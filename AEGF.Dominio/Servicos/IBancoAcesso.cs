using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Dominio.Servicos
{
    public interface IBancoAcesso
    {
        IEnumerable<Extrato> LerExtratos();
        string NomeUnico();
        void Iniciar(Banco banco);
    }
}

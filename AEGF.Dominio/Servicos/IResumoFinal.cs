using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Dominio.Servicos
{
    interface IResumoFinal
    {
        void CriarResumo(string arquivoSaida, IEnumerable<Extrato> extratos, IEnumerable<Exception> erros);
    }
}

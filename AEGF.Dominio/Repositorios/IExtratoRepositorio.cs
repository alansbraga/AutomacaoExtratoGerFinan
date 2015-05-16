using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Dominio.Repositorios
{
    public interface IExtratoRepositorio: IRepositorioBase<Extrato>
    {
        IEnumerable<Extrato> ObterPorDescricaoReferencia(string descricao, DateTime referencia);
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;

namespace AEGF.ServicoAplicacao
{
    public interface IGerenciadorBancoAcesso
    {
        void AdicionaBancoAcesso(IBancoAcesso bancoAcesso);
        IEnumerable<IBancoAcesso> CriaBancos();
    }
}

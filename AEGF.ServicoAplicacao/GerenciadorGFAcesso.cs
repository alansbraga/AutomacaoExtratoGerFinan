using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Repositorios;
using AEGF.Dominio.Servicos;

namespace AEGF.ServicoAplicacao
{
    public interface IGerenciadorGFAcesso
    {
        void AdicionaGFAcesso(IGerenciadorFinanceiroAcesso gfAcesso);
        IEnumerable<IGerenciadorFinanceiroAcesso> CriaGFs();
    }

    public class GerenciadorGFAcesso : IGerenciadorGFAcesso
    {
        private readonly IGerenciadorFinanceiroRepositorio _repositorio;
        private readonly Dictionary<string, IGerenciadorFinanceiroAcesso> _gerenciadoresFinanceiros;

        public GerenciadorGFAcesso(IGerenciadorFinanceiroRepositorio repositorio)
        {
            _repositorio = repositorio;
            _gerenciadoresFinanceiros = new Dictionary<string, IGerenciadorFinanceiroAcesso>();
        }

        private IGerenciadorFinanceiroAcesso CriaGFAcesso(GerenciadorFinanceiro gf)
        {
            var gfAcesso = _gerenciadoresFinanceiros[gf.Nome];
            gfAcesso.Iniciar(gf);
            return gfAcesso;
        }

        public void AdicionaGFAcesso(IGerenciadorFinanceiroAcesso gfAcesso)
        {
            _gerenciadoresFinanceiros.Add(gfAcesso.NomeUnico(), gfAcesso);
        }

        public IEnumerable<IGerenciadorFinanceiroAcesso> CriaGFs()
        {
            var gerenciadoresFinanceiros = _repositorio.ObterTodos();
            var retorno = new List<IGerenciadorFinanceiroAcesso>();
            foreach (var gf in gerenciadoresFinanceiros)
            {
                var gerenciadorBanco = CriaGFAcesso(gf);                
                retorno.Add(gerenciadorBanco);
            }
            return retorno;

        }

    }

}

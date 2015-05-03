using System.Collections.Generic;
using System.Linq;
using AEGF.Dominio;
using AEGF.Dominio.Repositorios;
using AEGF.Dominio.Servicos;

namespace AEGF.ServicoAplicacao
{
    public class IntegrarServicoAplicacao : IIntegrarServicoAplicacao
    {
        private readonly IBancoRepositorio _bancoRepositorio;
        private readonly IGerenciadorFinanceiroAcesso _gerenciadorFinanceiroAcesso;
        private readonly IGerenciadorBancoAcesso _gerenciadorBancoAcesso;

        public IntegrarServicoAplicacao(IBancoRepositorio bancoRepositorio, IGerenciadorFinanceiroAcesso gerenciadorFinanceiroAcesso, IGerenciadorBancoAcesso gerenciadorBancoAcesso)
        {
            _bancoRepositorio = bancoRepositorio;
            _gerenciadorFinanceiroAcesso = gerenciadorFinanceiroAcesso;
            _gerenciadorBancoAcesso = gerenciadorBancoAcesso;
        }

        public void IntegrarContas()
        {
            var bancos = _bancoRepositorio.ObterBancos();
            var extratos = new List<Extrato>();
            foreach (var banco in bancos)
            {
                var gerenciadorBanco = _gerenciadorBancoAcesso.CriaBancoAcesso(banco);
                var lidos = gerenciadorBanco.LerExtratos();
                if (lidos.Any())
                    extratos.AddRange(lidos);
            }
            _gerenciadorFinanceiroAcesso.ProcessarContas(extratos);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using AEGF.Dominio;
using AEGF.Dominio.Repositorios;
using AEGF.Dominio.Servicos;

namespace AEGF.ServicoAplicacao
{
    public class IntegrarServicoAplicacao : IIntegrarServicoAplicacao
    {
        private readonly IGerenciadorGFAcesso _gerenciadorFinanceiroAcesso;
        private readonly IGerenciadorBancoAcesso _gerenciadorBancoAcesso;

        public IntegrarServicoAplicacao(IGerenciadorGFAcesso gerenciadorFinanceiroAcesso, IGerenciadorBancoAcesso gerenciadorBancoAcesso)
        {
            _gerenciadorFinanceiroAcesso = gerenciadorFinanceiroAcesso;
            _gerenciadorBancoAcesso = gerenciadorBancoAcesso;
        }

        public IEnumerable<Extrato> IntegrarContas()
        {
            var bancos = _gerenciadorBancoAcesso.CriaBancos();
            var extratos = new List<Extrato>();
            foreach (var banco in bancos)
            {
                var lidos = banco.LerExtratos();
                if (lidos.Any())
                    extratos.AddRange(lidos);
            }

            foreach (var gf in _gerenciadorFinanceiroAcesso.CriaGFs())
            {
                gf.ProcessarContas(extratos);
            }
            return extratos;
        }
    }
}
using System;
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
        private readonly List<Exception> _erros;

        public IntegrarServicoAplicacao(IGerenciadorGFAcesso gerenciadorFinanceiroAcesso, IGerenciadorBancoAcesso gerenciadorBancoAcesso)
        {
            _gerenciadorFinanceiroAcesso = gerenciadorFinanceiroAcesso;
            _gerenciadorBancoAcesso = gerenciadorBancoAcesso;
            _erros = new List<Exception>();
        }

        public IEnumerable<Exception> Erros
        {
            get { return _erros; }
        }

        public IEnumerable<Extrato> IntegrarContas()
        {
            _erros.Clear();
            var bancos = _gerenciadorBancoAcesso.CriaBancos();
            var extratos = new List<Extrato>();
            foreach (var banco in bancos)
            {
                try
                {
                    var lidos = banco.LerExtratos();
                    if (lidos.Any())
                        extratos.AddRange(lidos);
                }
                catch (Exception e)
                {
                    _erros.Add(e);
                }
            }

            foreach (var gf in _gerenciadorFinanceiroAcesso.CriaGFs())
            {
                try
                {
                    gf.ProcessarContas(extratos);
                }
                catch (Exception e)
                {
                    _erros.Add(e);
                }
            }
            return extratos;
        }
    }
}
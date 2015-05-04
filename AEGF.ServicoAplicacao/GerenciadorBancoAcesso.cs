using System;
using System.Collections.Generic;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;

namespace AEGF.ServicoAplicacao
{
    public class GerenciadorBancoAcesso : IGerenciadorBancoAcesso
    {
        private readonly Dictionary<string, IBancoAcesso> _bancos;

        public GerenciadorBancoAcesso()
        {
            _bancos = new Dictionary<string, IBancoAcesso>();
        }

        public IBancoAcesso CriaBancoAcesso(Banco banco)
        {
            var bancoAcesso = _bancos[banco.NomeAcesso];
            bancoAcesso.Iniciar(banco);
            return bancoAcesso;
        }

        public void AdicionaBancoAcesso(IBancoAcesso bancoAcesso)
        {
            _bancos.Add(bancoAcesso.NomeUnico(), bancoAcesso);
        }
    }
}
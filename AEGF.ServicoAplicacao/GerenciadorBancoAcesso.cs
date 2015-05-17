using System;
using System.Collections.Generic;
using AEGF.Dominio;
using AEGF.Dominio.Repositorios;
using AEGF.Dominio.Servicos;

namespace AEGF.ServicoAplicacao
{
    public class GerenciadorBancoAcesso : IGerenciadorBancoAcesso
    {
        private readonly IBancoRepositorio _repositorio;
        private readonly Dictionary<string, IBancoAcesso> _bancos;

        public GerenciadorBancoAcesso(IBancoRepositorio repositorio)
        {
            _repositorio = repositorio;
            _bancos = new Dictionary<string, IBancoAcesso>();
        }

        private IBancoAcesso CriaBancoAcesso(Banco banco)
        {
            var bancoAcesso = _bancos[banco.NomeAcesso];
            bancoAcesso.Iniciar(banco);
            return bancoAcesso;
        }

        public void AdicionaBancoAcesso(IBancoAcesso bancoAcesso)
        {
            _bancos.Add(bancoAcesso.NomeUnico(), bancoAcesso);
        }

        public IEnumerable<IBancoAcesso> CriaBancos()
        {
            var bancos = _repositorio.ObterTodos();
            var retorno = new List<IBancoAcesso>();
            foreach (var banco in bancos)
            {
                var gerenciadorBanco = CriaBancoAcesso(banco);
                retorno.Add(gerenciadorBanco);
            }
            return retorno;
            
        }
    }
}
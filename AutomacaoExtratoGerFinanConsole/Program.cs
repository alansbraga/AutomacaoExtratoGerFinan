using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.BancosViaSite;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using AEGF.GFViaSite;
using AEGF.RepositorioJson;
using AEGF.ServicoAplicacao;
using AEGF.Dominio.Repositorios;

namespace AutomacaoExtratoGerFinanConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var unidadeTrabalho = new UnidadeTrabalhoJson();
            var repositorio = new BancoRepositorio(unidadeTrabalho);
            var repositorioGF = new GerenciadorFinanceiroRepositorio(unidadeTrabalho);
            var repositorioExtrato = new ExtratoRepositorio(unidadeTrabalho);

            var formatador = new FormatadorHtml();

            var resumoFinal = new ResumoFinal(repositorioExtrato, formatador);

            var gerenciador = repositorioGF.ObterTodos().First();
            var gerenciadorFinanceiro = new MinhasEconomiasViaSite();
            gerenciadorFinanceiro.Iniciar(gerenciador);
            //var gerenciadorFinanceiro = new GerenciadorFinanceiroAcessoConsole();


            var gerenciadorBanco = new GerenciadorBancoAcesso();
            gerenciadorBanco.AdicionaBancoAcesso(new SantanderSite());
            gerenciadorBanco.AdicionaBancoAcesso(new CEFSite());

            var integrador = new IntegrarServicoAplicacao(repositorio, gerenciadorFinanceiro, gerenciadorBanco);
            var extratos = integrador.IntegrarContas();

            var saida = "relatorioresumo.html";
            resumoFinal.CriarResumo(saida, extratos);            
            unidadeTrabalho.Gravar();

            Process.Start(saida);
        }
    }
}

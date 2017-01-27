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

            var repositorioExtrato = new ExtratoRepositorio(unidadeTrabalho);
            var formatador = new FormatadorHtml();
            var resumoFinal = new ResumoFinal(repositorioExtrato, formatador);

            var repositorioGF = new GerenciadorFinanceiroRepositorio(unidadeTrabalho);
            var gerenciadorGF = new GerenciadorGFAcesso(repositorioGF);
            gerenciadorGF.AdicionaGFAcesso(new MinhasEconomiasViaSite());
            gerenciadorGF.AdicionaGFAcesso(new GFGeradorOFX());

            var repositorio = new BancoRepositorio(unidadeTrabalho);
            var gerenciadorBanco = new GerenciadorBancoAcesso(repositorio);

            /*descomentar abaixo os bancos que você vai usar....*/
            //gerenciadorBanco.AdicionaBancoAcesso(new SantanderSite());
            //gerenciadorBanco.AdicionaBancoAcesso(new CetelemSite());

            gerenciadorBanco.AdicionaBancoAcesso(new CEFSiteFisico());
            gerenciadorBanco.AdicionaBancoAcesso(new CEFSiteJuridico());
            gerenciadorBanco.AdicionaBancoAcesso(new BanestesSite());
            gerenciadorBanco.AdicionaBancoAcesso(new BBSiteJuridico());

            var integrador = new IntegrarServicoAplicacao(gerenciadorGF, gerenciadorBanco);
            var extratos = integrador.IntegrarContas();

            var saida = "relatorioresumo.html";
            resumoFinal.CriarResumo(saida, extratos, integrador.Erros);            
            unidadeTrabalho.Gravar();

            Process.Start(saida);
        }
    }
}

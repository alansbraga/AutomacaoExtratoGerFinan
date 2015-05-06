using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using AEGF.Infra;

namespace AEGF.GFViaSite
{
    public class MinhasEconomiasViaSite: AcessoSelenium, IGerenciadorFinanceiroAcesso
    {
        private GerenciadorFinanceiro _gerenciador;

        public void ProcessarContas(IEnumerable<Extrato> extratos)
        {
            FazerLogin();
            PosicionarNasTransacoes();

            foreach (var extrato in extratos)
            {
                var gerador = new GeradorOFX(extrato, new OpcoesOFX()
                {
                    IgnorarPositivos = extrato.CartaoCredito,
                    MudaDataParaMesReferencia = extrato.CartaoCredito,
                    MultiplicarMenosUm = extrato.CartaoCredito
                });
                var arquivo = gerador.GravarTemporario();
                if (!File.Exists(arquivo)) 
                    continue;

                ClicarNoImportar();
                ClicarNoAvancar();
                SelecionarArquivo(arquivo);
                SelecionarConta(_gerenciador.LerConta(extrato.Descricao).ContaDestino);
                ConfirmarArquivo();
            }


        }

        public string NomeUnico()
        {
            return "MinhasEconomiasSite";
        }

        public void Iniciar(GerenciadorFinanceiro gerenciador)
        {
            _gerenciador = gerenciador;
        }

        private void ConfirmarArquivo()
        {
            ClicaXPath("//*[@id=\"ext-gen7\"]/div[17]/div[2]/div[1]/div/div/div[2]/div/table/tbody/tr/td[2]/table/tbody/tr/td[1]/table/tbody/tr/td/table/tbody/tr[2]/td[2]/em/button");
        }

        private void SelecionarConta(string contaDestino)
        {
            
            DigitaTextoXPath("//*[@id=\"ext-gen7\"]/div[17]/div[2]/div[1]/div/div/div[1]/div/div/form/div[2]/div[4]/div[1]/div/input[2]", contaDestino);
        }

        private void SelecionarArquivo(string arquivo)
        {
            DigitaTextoXPath("//*[@id=\"ext-gen7\"]/div[17]/div[2]/div[1]/div/div/div[1]/div/div/form/div[2]/div[2]/div[1]/div/input[1]", arquivo);
        }

        private void ClicarNoAvancar()
        {
            ClicaXPath("//*[@id=\"ext-gen7\"]/div[17]/div[2]/div[1]/div/div/div[2]/div/table/tbody/tr/td[2]/table/tbody/tr/td[1]/table/tbody/tr/td/table/tbody/tr[2]/td[2]/em/button");
        }

        private void ClicarNoImportar()
        {
            ClicaId("ext-gen703");
        }

        private void PosicionarNasTransacoes()
        {
            ClicaId("ext-gen418");
        }

        private void FazerLogin()
        {
            DigitaTextoId("email", _gerenciador.LerConfiguracao("email"));
            DigitaTextoId("senha", _gerenciador.LerConfiguracao("senha"));
            ClicaXPath("//*[@id=\"login\"]/button");
        }

        protected override string URLSite()
        {
            return "https://wwws.minhaseconomias.com.br/";
        }
    }
}

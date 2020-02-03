using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using AEGF.Infra;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AEGF.BancosViaSite
{
    public class CEFSiteFisico : AcessoSelenium, IBancoAcesso
    {
        private Banco _banco;
        private ICollection<Extrato> _extratos;

        public IEnumerable<Extrato> LerExtratos()
        {
            IniciarBrowser();
            Inicio();
            FazerLogin();
            LerExtrato();
            Sair();
            return _extratos;
        }


        private void LerExtrato()
        {
            Tempo();

            // pega o extrato mes atual
            VaiParaSelecaoExtrato();
            SelecionaMesAtual();
            var numConta = LerNumeroConta();
            LerTabelaExtrato(numConta, DateTime.Today.PrimeiroDia());
            Tempo();

            // pega o extrato mes anterior
            VaiParaSelecaoExtrato();
            SelecionaMesAnterior();
            LerTabelaExtrato(numConta, DateTime.Today.AddMonths(-1).PrimeiroDia());
        }

        private void VaiParaSelecaoExtrato()
        {
            driver.Navigate().GoToUrl("https://internetbanking.caixa.gov.br/sinbc/nb/home#!/sinbc/nb/carrossel");
            Tempo();
            driver.Navigate().GoToUrl("https://internetbanking.caixa.gov.br/SIIBC/interna#!/extrato_periodo.processa");
            Tempo();
        }

        private string LerNumeroConta()
        {
            return LerTextoCSS("#contaSelecionada li.conta strong");
        }

        private void LerTabelaExtrato(string numConta, DateTime referencia)
        {
            AguardarCSS("table.movimentacao");
            var extrato = CriaRetorno("table.movimentacao tr", false, 0, 2, 3);
            extrato.Referencia = referencia;
            extrato.Descricao = String.Format("CEF Conta Corrente - {0}", numConta);
            _extratos.Add(extrato);

        }

        private Extrato CriaRetorno(string cssPath, bool tipoCartao, int colData, int colDescricao, int colValor)
        {
            var trs = driver.FindElements(By.CssSelector(cssPath));
            var retorno = new Extrato { CartaoCredito = tipoCartao };

            AdicionaItens(retorno, trs, colData, colDescricao, colValor);

            return retorno;
        }

        private static void AdicionaItens(Extrato extrato, ReadOnlyCollection<IWebElement> linhas, int colData, int colDescricao, int colValor)
        {
            var linhaAtual = 0;

            foreach (var linha in linhas)
            {
                linhaAtual++;

                var colunas = linha.FindElements(By.TagName("td"));

                if (linhaAtual == 3)
                    extrato.SaldoAnterior = BuscaValor(colunas, colValor + 1);

                if (linhaAtual <= 3)
                    continue;

                if (colunas.Count < colValor)
                    continue;

                if (colunas[0].Text.ToLower().Contains("data"))
                    continue;

                var valor = BuscaValor(colunas, colValor);

                if (valor != 0)
                {
                    var item = new Transacao()
                    {
                        Valor = valor,
                        Descricao = colunas[colDescricao].Text,
                        Data = DateTime.Parse(colunas[colData].Text)
                    };
                    extrato.AdicionaTransacao(item);

                }
            }
        }

        private static double BuscaValor(ReadOnlyCollection<IWebElement> colunas, int colValor)
        {
            var colunaValor = colunas[colValor].Text.Split(' ');
            var valorStr = colunaValor[0];
            double valor;

            if (Double.TryParse(valorStr, out valor))
            {
                if ((valor != 0) && (colunaValor[1] == "D"))
                    valor *= -1;
            }
            return valor;
        }


        private void SelecionaMesAtual()
        {
            ClicaXPath("//label[@for='rdoTipoExtratoAtual']", true);
            AguardarLoadingSumir();
            ConfirmaMesExtrato();
        }

        private void ConfirmaMesExtrato()
        {
            const string id = "confirma";
            AguardarId(id);
            ClicaId(id);
            AguardarLoadingSumir();

        }

        private void SelecionaMesAnterior()
        {
            const string id = "rdoTipoExtratoOutro";
            AguardarId(id);
            ClicaId(id);
            AguardarLoadingSumir();
            ClicaXPath("//*[@id=\"dk_container_sltOutroMes\"]/a");
            AguardarLoadingSumir();
            ClicaXPath("//*[@id=\"dk_container_sltOutroMes\"]/div/ul/li[2]/a");
            AguardarLoadingSumir();
            ConfirmaMesExtrato();
        }

        private void FazerLogin()
        {
            AguardarLoadingSumir();
            var idEsperado = "nomeUsuario";
            AguardarId(idEsperado);
            DigitaTextoId(idEsperado, _banco.LerConfiguracao("usuario"));

            ClicaId("tpPessoaFisica");

            idEsperado = "btnLogin";
            Aguardar(By.Id(idEsperado), true);
            ClicaId(idEsperado);
            AguardarLoadingSumir();

            idEsperado = "lnkInitials";
            Aguardar(By.Id(idEsperado), true);
            ClicaId(idEsperado, true);
            AguardarLoadingSumir();

            Aguardar(By.XPath("//*[@id=\"teclado\"]/ul/li[contains(@class, 'key') and text()='a']"), true);

            var senha = _banco.LerConfiguracao("senha");

            foreach (var letra in senha)
            {
                var elemento =
                    driver.FindElement(
                        By.XPath(String.Format("//*[@id=\"teclado\"]/ul/li[contains(@class, 'key') and text()='{0}']", letra)));
                Actions builder = new Actions(driver);
                builder.MoveToElement(elemento).Click().Perform();
            }
            ClicaId("btnConfirmar");
            AguardarLoadingSumir();

        }

        private void Sair()
        {
            try
            {
                // realiza logoff da sessão!!!
                driver.FindElements(By.ClassName("logoff"))[0].Click();
                driver.FindElements(By.Id("btnSim"))[0].Click();
            }
            catch
            {
            }

            // fecha o navegador.
            FecharBrowser();
        }

        private void Inicio()
        {
            _extratos = new List<Extrato>();
        }

        public string NomeUnico()
        {
            return "CEFSiteFisico";
        }

        public void Iniciar(Banco banco)
        {
            _banco = banco;
        }

        protected override string URLSite()
        {
            return "https://internetbanking.caixa.gov.br/SIIBC/index.processa";
        }

        protected void AguardarLoadingSumir()
        {
            Tempo(5);
            while (HabilitadoXPath("//div[contains(@class, 'modalBgLoading')]"))
            {
                Tempo(10);
            }
        }
    }
}

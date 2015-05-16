using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using AEGF.Infra;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AEGF.BancosViaSite
{
    public class SantanderSite: AcessoSelenium, IBancoAcesso
    {
        private Banco _banco;
        private ICollection<Extrato> _extratos;
        private int _cartaoAtual;

        public IEnumerable<Extrato> LerExtratos()
        {
            IniciarBrowser();
            Inicio();
            FazerLogin();
            LerExtrato();
            LerCartoes();
            FecharBrowser();
            return _extratos;
        }

        private void LerCartoes()
        {
            do
            {
                _cartaoAtual++;
                ClicaFatura();
            } while (LerCartaoAtual());
        }

        private bool LerCartaoAtual()
        {
            VaiParaIFramePrinc();
            var trs = driver.FindElements(By.CssSelector("table.lista tr.trClaro"));
            if (_cartaoAtual > (trs.Count - 1))
                return false;

            var linha = trs[_cartaoAtual];
            var colunas = linha.FindElements(By.TagName("td"));

            var link = colunas[1].FindElement(By.TagName("a"));
            link.Click();

            var segunda = false;
            do
            {
                VaiParaIFramePrinc();
                FechaPostIt();
                

                var tds = driver.FindElements(By.CssSelector("div.caixa td.bold"));
                var conta = tds[2].Text;
                conta = conta.Replace("XXXX XXXX XXXX", "Final");
                var descricao = String.Format("{0} - {1}", tds[0].Text, conta);

                TrocaFrameId("iDetalhes");


                var extrato = CriaRetorno("#detfatura tr.trClaro", true, 0, 1, 2);
                extrato.Descricao = descricao;
                _extratos.Add(extrato);

                if (!segunda)
                {
                    VaiParaIFramePrinc();
                    SelecionaIndexXPath("//*[@id=\"cboFatura\"]", 2);
                    ClicaXPath("//*[@id=\"frmFatura\"]/div[7]/fieldset/a");
                    
                    
                    segunda = true;
                }
                else
                {
                    segunda = false;
                }
            } while (segunda);
            return true;
        }

        private void FechaPostIt()
        {
            ClicaXPath("//*[@id=\"divPostIt\"]/table/tbody/tr[1]/td/a/img");
        }

        private void ClicaFatura()
        {
            VaiParaMenu();
            ClicaXPath("//*[@id=\"3975Link\"]");
            driver.SwitchTo().DefaultContent();
            VaiParaCorpo();
            ClicaXPath("//*[@id=\"montaMenu\"]/ul/li[1]/ul/li[2]/a");
        }


        private void VaiParaMenu()
        {
            driver.SwitchTo().DefaultContent();
            VaiParaFramePrincipal();
            TrocaFrameXPath("//*[@id=\"frameMain\"]/frame[1]");
        }

        private void LerExtrato()
        {
            SelecionaPeriodo();
            LerTabelaExtrato();
        }

        private void LerTabelaExtrato()
        {
            var numeroConta = BuscaNumeroConta();

            VaiParaIFramePrinc();
            TrocaFrameXPath("//*[@id=\"extrato\"]");
            var extrato = CriaRetorno("table.lista tr.trClaro", false, 0, 2, 5);
            BuscaSaldo(extrato);
            extrato.Referencia = DateTime.Today;
            extrato.Descricao = "Conta Corrente " + numeroConta;
            _extratos.Add(extrato);

        }

        private void BuscaSaldo(Extrato extrato)
        {
            var trs = driver.FindElements(By.CssSelector("table.lista tr.trClaro"));
            var colunas = trs[0].FindElements(By.TagName("td"));

            var valorStr = colunas[6].Text;
            double valor;

            if (Double.TryParse(valorStr, out valor))
            {
                extrato.SaldoAnterior = valor;
            }
        }

        private string BuscaNumeroConta()
        {
            VaiParaCorpo();
            var numeroConta = driver.FindElement(By.XPath("//*[@id=\"ola\"]/table/tbody/tr/td[2]")).Text;
            numeroConta = new Regex("Conta: +([0-9.]+)").Match(numeroConta).Groups[1].Value;
            return numeroConta;
        }

        private Extrato CriaRetorno(string cssPath, bool tipoCartao, int colData, int colDescricao, int colValor)
        {
            var trs = driver.FindElements(By.CssSelector(cssPath));
            var retorno = new Extrato { CartaoCredito = tipoCartao };

            AdicionaItens(retorno, trs, colData, colDescricao, colValor);

            if (retorno.CartaoCredito)
            {
                try
                {
                    retorno.Referencia = DateTime.Parse(driver.FindElement(By.CssSelector("table.transacao strong")).Text);
                }
                catch(Exception)
                {
                    // quando não há movimentos
                }
                
            }

            return retorno;
        }

        private static void AdicionaItens(Extrato extrato, ReadOnlyCollection<IWebElement> linhas, int colData, int colDescricao, int colValor)
        {
            foreach (var linha in linhas)
            {
                var colunas = linha.FindElements(By.TagName("td"));

                var valorStr = colunas[colValor].Text;
                double valor;

                if (Double.TryParse(valorStr, out valor))
                {
                    var item = new Transacao
                    {
                        Valor = valor,
                        Descricao = colunas[colDescricao].Text,
                        Data = DateTime.Parse(colunas[colData].Text)
                    };
                    extrato.AdicionaTransacao(item);

                }
            }
        }

        private void VaiParaIFramePrinc()
        {
            VaiParaCorpo();
            TrocaFrameXPath("//*[@id=\"iframePrinc\"]");
        }

        private void SelecionaPeriodo()
        {
            VaiParaCorpo();
            TrocaFrameXPath("//*[@id=\"iframePainel\"]");
            SelecionaValorXPath("//*[@id=\"select\"]/select", "30");
            ClicaXPath("//*[@id=\"extrato\"]/tbody/tr/td[3]/a");
        }

        private void FazerLogin()
        {
            TrocaFrameId("iframetopo");

            DigitaTextoId("txtCPF", _banco.LerConfiguracao("CPF") /*_banco.Configuracoes.Single(configuracao => configuracao.Nome == "CPF").Valor*/);
            ClicaId("hrefOk");
            FechaMensagemPlugin();

            DigitaTextoId("txtSenha", _banco.LerConfiguracao("Senha")/*_banco.Configuracoes.Single(configuracao => configuracao.Nome == "Senha").Valor*/);
            ClicaXPath("//*[@id=\"divBotoes\"]/a[1]");
        }

        private void FechaMensagemPlugin()
        {
            VaiParaFramePrincipal();

            TrocaFrameNome("MainFrame");            
            if (ExisteId("txtSenha"))
                return;

            ClicaXPath("//*[@id=\"divFloaterStormFish\"]/div/map/area[1]");
        }


        private void VaiParaFramePrincipal()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(webDriver =>
            {
                try
                {
                    webDriver.SwitchTo().DefaultContent();
                    return webDriver.FindElement(By.Id("frmSet")) != null;
                }
                catch (Exception)
                {
                    return false;
                }
            });
            TrocaFrameXPath("//*[@id=\"frmSet\"]/frame[2]");
        }

        private void VaiParaCorpo()
        {
            VaiParaFramePrincipal();
            TrocaFrameXPath("//*[@id=\"frameMain\"]/frame[2]");
            
        }

        private void Inicio()
        {
            _extratos = new List<Extrato>();
            _cartaoAtual = -1;
        }

        public string NomeUnico()
        {
            return "SantanderSite";
        }

        public void Iniciar(Banco banco)
        {
            _banco = banco;
        }

        protected override string URLSite()
        {
            return "http://www.santander.com.br";
        }
    }
}

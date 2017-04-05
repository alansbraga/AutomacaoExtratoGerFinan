using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using AEGF.Infra;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
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
            FecharAvisos();
            LerExtrato();
            LerCartoes();
            FecharBrowser();
            return _extratos;
        }

        private void FecharAvisos()
        {
            FecharAvisoFimDeAno();
        }

        private void FecharAvisoFimDeAno()
        {
            VaiParaCorpo();
            if (ExisteId("FloaterAvisoCartao"))
            {
                TrocaFrameId("FloaterAvisoCartao");
                ClicaXPath("//*[@id=\"fimdeano\"]/area");
            }
            VaiParaFramePrincipal();
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


                if (!FaturaAcabouDeFechar())
                {
                    var extrato = CriaRetorno("#detfatura tr.trClaro", true, 0, 1, 2);
                    extrato.Descricao = descricao;
                    _extratos.Add(extrato);
                }

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

        private bool FaturaAcabouDeFechar()
        {
            return ExisteXPath("/html/body/div[1]/p");
        }

        private void FechaPostIt()
        {
            ClicaXPath("//*[@id=\"divPostIt\"]/table/tbody/tr[1]/td/a/img");
        }

        private void ClicaFatura()
        {
            VaiParaMenu();
            ClicaXPath("//*[@id=\"3975Link\"]", true);
            driver.SwitchTo().DefaultContent();
            VaiParaCorpo();
            ClicaXPath("//*[@id=\"montaMenu\"]/ul/li[1]/ul/li[2]/a", true);
        }


        private void VaiParaMenu()
        {
            driver.SwitchTo().DefaultContent();
            VaiParaFramePrincipal();
            TrocaFrameXPath("//*[@id=\"frameMain\"]/frame[1]", true);
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
            extrato.Referencia = DateTime.MinValue; // Sem referência para que o resumo fique sempre mostrando os novos registros
            extrato.Descricao = "Santander Conta Corrente " + numeroConta;
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
            AguardarCSS(cssPath);
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

        private void AdicionaItens(Extrato extrato, ReadOnlyCollection<IWebElement> linhas, int colData, int colDescricao, int colValor)
        {
            double valorDolar = CriaValorDolar(extrato);

            foreach (var linha in linhas)
            {
                var dolar = false;
                var colunas = linha.FindElements(By.TagName("td"));

                var valorStr = colunas[colValor].Text;
                double valor;

                if (!Double.TryParse(valorStr, out valor)) 
                    continue;
                if (valor == 0 && extrato.CartaoCredito)
                {
                    valorStr = colunas[colValor + 1].Text;
                    if (Double.TryParse(valorStr, out valor))
                    {
                        valor = valor*valorDolar;
                        valor = Math.Round(valor, 2);
                        dolar = true;
                    }
                        
                }
                var descricao = colunas[colDescricao].Text;
                if (dolar)
                    descricao += String.Format(" [Dolar: {0}]", valorDolar);
                var item = new Transacao
                {
                    Valor = valor,
                    Descricao = descricao,
                    Data = DateTime.Parse(colunas[colData].Text)
                };
                extrato.AdicionaTransacao(item);
            }
        }

        private double CriaValorDolar(Extrato extrato)
        {
            if (!extrato.CartaoCredito)
                return 0;
            try
            {
                var valorStr = driver.FindElement(By.XPath("//*[@id=\"resdespbra\"]/table/tbody/tr[1]/td[3]/table/tbody/tr[4]/td[2]/strong")).Text;
                double valor;
                if (Double.TryParse(valorStr, out valor))
                {
                    return valor;
                }
            }
            catch (Exception e)
            {
            }
            return 0;
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
            ClicaXPath("//*[@id=\"extrato\"]/tbody/tr/td[3]/a", true);
        }

        private void FazerLogin()
        {
            //TrocaFrameId("iframetopo");

            DigitaTextoId("cfp", _banco.LerConfiguracao("CPF") /*_banco.Configuracoes.Single(configuracao => configuracao.Nome == "CPF").Valor*/);
            ClicaXPath("//*[@id=\"formularioFisica\"]/fieldset/ul/li[2]/input");
            FechaMensagemPlugin();

            DigitaTextoId("txtSenha", _banco.LerConfiguracao("Senha")/*_banco.Configuracoes.Single(configuracao => configuracao.Nome == "Senha").Valor*/);
            ClicaXPath("//*[@id=\"divBotoes\"]/a[1]");
            FechaMensagemSenhaTelefone();
        }

        private void FechaMensagemSenhaTelefone()
        {
            VaiParaCorpo();
            if (ExisteId("FloaterAlertaAviso"))
            {
                TrocaFrameId("FloaterAlertaAviso");
                ClicaXPath("//*[@id=\"alertaAviso\"]/area");
            }
            VaiParaFramePrincipal();
        }

        private void FechaMensagemPlugin()
        {
            VaiParaFramePrincipal();
            TrocaFrameNome("MainFrame");

            var idPopup = "splash-10000-remind-me-later";
            // Próxima vez alterar o id para texto "Lembrar mais tarde" acho q isso não muda
            Tempo();
            if (ExisteId(idPopup))
                ClicaId(idPopup);
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
            TrocaFrameXPath("//*[@id=\"frameMain\"]/frame[2]", true);
            
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

//        protected override IWebDriver CriarBrowser()
//        {
////            var f = new FirefoxProfileManager();
////            var p = f.GetProfile("default");

////            return new FirefoxDriver(p);
//            return base.CriarBrowser();
//        }
    }
}

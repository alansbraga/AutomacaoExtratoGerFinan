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
            FecharSejaBemVindo();
            FecharTutorial();
        }

        private void FecharSejaBemVindo()
        {
            const string xpath = "//*[@id=\"navbar\"]/div[2]/a";
            Tempo();
            if (ExisteXPath(xpath))
                ClicaXPath(xpath);
        }

        private void FecharTutorial()
        {
            const string xpath = "/html/body/div[11]/div/div[2]/a";
            Tempo();
            if (ExisteXPath(xpath))
                ClicaXPath(xpath);
        }

        private void LerCartoes()
        {
            ClicaId("cartoes");
            ClicaXPath("//*[@id=\"subMenu-cartoes\"]//a[text() = 'Consultar faturas']");
            var primeiro = LerTextoXPath("//div[@class='contract-account']//span[2]");
            var cartoesDisponiveis = driver.FindElements(By.XPath("//*[@id=\"accountIndexCartoes\"]/ul/li"))
                .Select(i => new {
                    Nome = i.FindElement(By.TagName("div")).GetAttribute("data-creditcard-number"),
                    Item = i
                }).ToList();
            var primeiroCartao = cartoesDisponiveis
                .First(a => a.Nome == primeiro);
            cartoesDisponiveis.Remove(primeiroCartao);

            LerCartao(primeiroCartao.Nome, primeiroCartao.Item);
            foreach (var cartao in cartoesDisponiveis)
            {
                ClicaCartao(cartao.Item);
                LerCartao(cartao.Nome, cartao.Item);
            }
        }

        private void LerCartao(string nome, IWebElement cartao)
        {
            Tempo(3);
            ClicaXPath("//div[@class = 'pestanas']//a[text() = 'Fatura em aberto']");
            LerTabelaCartao(nome);
            ClicaXPath("//div[@class = 'pestanas']//a[contains(@class,'fechada_max')]");
            LerTabelaCartao(nome);
        }

        private void LerTabelaCartao(string nome)
        {
            var cotacaoDolar = BuscaCotacaoDolar();
            var trs = driver.FindElements(By.XPath("//div[@class = 'tabla_datos']/table/tbody/tr[not(contains(@class, 'cabecera2'))]"));
            var labelVencimento = driver.FindElement(By.XPath("//span[contains(@class, 'left') and text() = 'Data de vencimento:']"));
            var vencimento = labelVencimento.FindElement(By.XPath("../span[contains(@class, 'right')]"));
            if (!DateTime.TryParse(vencimento.Text.Trim(), out var dataVencimento))
                dataVencimento = DateTime.MinValue;
            var extrato = new Extrato
            {
                CartaoCredito = true,
                Descricao = $"Final {nome}",
                Referencia = dataVencimento
            };
            foreach (var tr in trs)
            {
                var colunas = tr.FindElements(By.TagName("td"));

                if (colunas.Count < 4)
                    continue;

                var transacao = new Transacao
                {
                    Descricao = colunas[1].Text.Trim()
                };

                if (!DateTime.TryParse(colunas[0].Text.Trim(), out var data))
                    continue;
                transacao.Data = data;

                var strValor = colunas[3].Text.Trim();
                var multiplicador = 1.0;
                if (string.IsNullOrWhiteSpace(strValor))
                {
                    multiplicador = cotacaoDolar;
                    strValor = colunas[2].Text.Trim();
                }

                if (!Double.TryParse(strValor, out var valor))
                    continue;
                transacao.Valor = valor * multiplicador;


                extrato.AdicionaTransacao(transacao);
            }
            if (extrato.Transacoes.Any())
                _extratos.Add(extrato);
        }

        private double BuscaCotacaoDolar()
        {
            Tempo(4);
            var trs = driver.FindElements(By.XPath("//table[contains(@class, 'doble') and contains(@class, 'derecha')]//tr"));
            var tds = trs[2].FindElements(By.TagName("td"));
            if (!Double.TryParse(tds[1].GetAttribute("textContent").Trim(), out var valor))
                valor = 1;
            return valor;

        }

        private void ClicaCartao(IWebElement cartao)
        {
            ClicaXPath("//i[contains(@class, 'selector-choose-item') and contains(@class, 'credit-card-choose-icon')]");
            cartao.Click();
        }

        private void LerExtrato()
        {
            ClicaId("ctacorrente");
            ClicaXPath("//*[@id=\"subMenu-ctacorrente\"]//a[text() = 'Extrato Conta Corrente']", true);
            ClicaXPath("//a[@id=\"daterange7\" and text() = '30 dias' ]", true);
            Tempo();

            LerTabelaExtrato();
        }

        private void LerTabelaExtrato()
        {
            var numeroConta = BuscaNumeroConta();
            var extrato = CriaExtratoCC();
            BuscaSaldo(extrato);
            extrato.Referencia = DateTime.MinValue; // Sem referência para que o resumo fique sempre mostrando os novos registros
            extrato.Descricao = "Santander Conta Corrente " + numeroConta;
            _extratos.Add(extrato);

        }

        private Extrato CriaExtratoCC()
        {
            const string cssPath = "div.extrato-table.tabla_datos.margin_bottom_standard.extrato-table-fix > table > tbody > tr";
            AguardarCSS(cssPath);
            var trs = driver.FindElements(By.CssSelector(cssPath));
            var retorno = new Extrato { CartaoCredito = false };

            AdicionaItensCC(retorno, trs);

            return retorno;
        }

        private static void AdicionaItensCC(Extrato extrato, ReadOnlyCollection<IWebElement> linhas)
        {
            DateTime ultimaData = DateTime.Now;
            foreach (var linha in linhas)
            {
                var colunas = linha.FindElements(By.TagName("td"));
                var valorStr = colunas[3].Text;
                valorStr = valorStr.Trim();
                if (!Double.TryParse(valorStr, out double valor))
                    continue;
                var descricao = colunas[1].Text;

                valorStr = colunas[4].Text;
                Double.TryParse(valorStr, out double saldo);

                DateTime data;
                var diaMes = colunas[0].Text;
                if (string.IsNullOrWhiteSpace(diaMes))
                {
                    data = ultimaData;
                }
                else
                {
                    data = DateTime.Parse($"{diaMes}/{DateTime.Now.Year}");
                    if (data > DateTime.Now)
                        data = data.AddYears(-1);
                    ultimaData = data;
                }

                var item = new Transacao
                {
                    Valor = valor,
                    Descricao = descricao,
                    Data = data,
                    Saldo = saldo
                };
                extrato.AdicionaTransacao(item);
            }
        }

        private static void BuscaSaldo(Extrato extrato)
        {
            var tr = extrato.Transacoes.FirstOrDefault();
            if (tr == null)
                return;

            extrato.SaldoAnterior = tr.Saldo - tr.Valor;
        }

        private string BuscaNumeroConta()
        {
            var numeroConta = LerTextoCSS("#\\30 > div.contract-info > div > span:nth-child(4)");
            return numeroConta;
        }

        private void FazerLogin()
        {
            var xpath = "//input[@type=\"tel\"]";
            AguardarXPath(xpath);
            DigitaTextoXPath(xpath, _banco.LerConfiguracao("CPF"));
            driver.FindElement(By.XPath(xpath)).SendKeys(Keys.Return);
            FechaPlugin();
            DigitaTextoId("senha", _banco.LerConfiguracao("Senha"));
            ClicaXPath("//div[contains(@class, 'form-group')]//input[@id='Entrar']");
        }

        private void FechaPlugin()
        {
            var id = "splash-10000-remind-me-later";
            AguardarId(id);
            if (ExisteId(id))
                ClicaId(id);
            
        }

        private void Inicio()
        {
            _extratos = new List<Extrato>();
            _cartaoAtual = -1;
        }

        public string NomeUnico()
        {
            return nameof(SantanderSite);
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

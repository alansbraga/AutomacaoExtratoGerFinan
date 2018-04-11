using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using AEGF.Infra;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.BancosViaSite
{
    public class CitibankSite : AcessoSelenium, IBancoAcesso
    {
        private Banco _banco;
        private ICollection<Extrato> _extratos;
        private DateTime ultimaData;

        public void Iniciar(Banco banco)
        {
            _banco = banco;
        }

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
            ClicaId("link_liCartoesCreditoCBOL", true);
            TrocaFrameId("cc_iframe", true);
            var xpath = "//*[@id=\"contentMenu\"]//a[text() = 'Faturas']";
            AguardarXPath(xpath);
            ClicaXPath(xpath, true);
            xpath = "//*[@id=\"/ICARD/icardTransactionHistoryContext\"]//a[text()='Fatura Fechada']";
            AguardarXPath(xpath);
            ClicaXPath(xpath, true);
            Tempo(5);
            xpath = "//*[@id=\"appRcpTbl\"]//span[@class='applabelFalt']";
            AguardarXPath(xpath);
            var ultimaDataStr = LerTexto(By.XPath(xpath));
            ultimaData = DateTime.Parse(ultimaDataStr);
            LerCartao();
            ultimaData = ultimaData.AddMonths(1);
            Tempo(5);
            xpath = "//*[@id=\"/ICARD/icardTransactionHistoryContext\"]//a[text()='Próxima Fatura']";
            AguardarXPath(xpath);
            ClicaXPath(xpath, true);
            Tempo(5);
            LerCartao();
        }

        private void LerCartao()
        {
            var xpath = "//*[@id=\"/ICARD/icardTransactionHistoryContext\"]/table[@class='appCellBorder1']/tbody/tr";
            AguardarXPath(xpath);
            var trs = driver.FindElements(By.XPath(xpath));
            xpath = "//*[@id=\"appRcpTbl\"]/table/tbody//div[contains(@class, 'plnTxt')]";
            var nome = LerTextoXPath(xpath);

            var extrato = new Extrato
            {
                CartaoCredito = true,
                Descricao = $"{nome}",
                Referencia = ultimaData
            };
            foreach (var tr in trs)
            {
                var colunas = tr.FindElements(By.TagName("td"));

                if (colunas.Count != 5)
                    continue;
                var texto = colunas[1].Text.Trim();
                if (texto == "Descritivo")
                    continue;
                if (texto == "Total da Fatura Anterior")
                    continue;

                var transacao = new Transacao
                {
                    Descricao = texto
                };

                if (!DateTime.TryParse(colunas[0].Text.Trim(), out var data))
                    continue;
                transacao.Data = data;

                var multiplicador = 1;

                if (colunas[4].Text.Trim() == "+")
                    multiplicador = -1;
                var strValor = colunas[3].Text.Trim();

                if (!Double.TryParse(strValor, out var valor))
                    continue;
                transacao.Valor = valor * multiplicador;

                extrato.AdicionaTransacao(transacao);
            }
            if (extrato.Transacoes.Any())
                _extratos.Add(extrato);

        }

        private void LerExtrato()
        {
            //
        }

        private void FazerLogin()
        {
            ClicaId("NH001_CL", true);
            DigitaTextoName("username", _banco.LerConfiguracao("usuario"));
            Clica(By.Name("password"));

            var senha = _banco.LerConfiguracao("senha");
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            foreach (var letra in senha)
            {
                js.ExecuteScript($"add('{letra}');");
            }
            js.ExecuteScript("hideVkb(null);");
            ClicaId("link_avtEnterSite");
        }

        private void Inicio()
        {
            _extratos = new List<Extrato>();
        }

        public string NomeUnico()
        {
            return nameof(CitibankSite);
        }

        protected override string URLSite()
        {
            return "https://www.citibank.com.br";
        }
    }
}

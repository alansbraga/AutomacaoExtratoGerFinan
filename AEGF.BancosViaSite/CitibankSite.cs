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
            //TrocaFrameId("cc_iframe", true);
            ClicaXPath("//*[@id=\"/ICARD/icardTransactionHistoryContext\"]//a[text()='Fatura Fechada']", true);
            //TrocaFrameId("cc_iframe", true);
            var ultimaDataStr = LerTexto(By.XPath("//*[@id=\"appRcpTbl\"]//span[@class='applabelFalt']"));
            ultimaData = DateTime.Parse(ultimaDataStr);
            LerCartao();
            ultimaData = ultimaData.AddMonths(1);
            ClicaXPath("//*[@id=\"/ICARD/icardTransactionHistoryContext\"]//a[text()='Fatura Aberta']", true);
            TrocaFrameId("cc_iframe", true);
            LerCartao();
        }

        private void LerCartao()
        {
            var trs = driver.FindElements(By.XPath("//*[@id=\"/ICARD/icardTransactionHistoryContext\"]/table[@class='appCellBorder1']/tbody/tr"));
            var nome = LerTextoXPath("//*[@id=\"appRcpTbl\"]/table/tbody//div[contains(@class, 'plnTxt')]");

            var extrato = new Extrato
            {
                CartaoCredito = true,
                Descricao = $"{nome}",
                Referencia = ultimaData
            };
            foreach (var tr in trs)
            {
                var colunas = tr.FindElements(By.TagName("td"));

                if (colunas.Count != 3)
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

                var trValor = colunas[2].FindElements(By.XPath("//td"));
                if (trValor.Count != 2)
                    continue;
                var multiplicador = -1;

                if (trValor[1].Text.Trim() == "+")
                    multiplicador = 1;
                var strValor = trValor[0].Text.Trim();

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

            foreach (var letra in senha)
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript($"add('{letra}');");
            }

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

using System;
using System.Collections.Generic;
using System.Linq;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using AEGF.Infra;
using OpenQA.Selenium;

namespace AEGF.BancosViaSite
{
    public class BanestesSite : AcessoSelenium, IBancoAcesso
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
            Sair();
            return _extratos;
        }

        private void Inicio()
        {
            _extratos = new List<Extrato>();
            _cartaoAtual = -2;
        }

        private void FazerLogin()
        {

            DigitaTextoId("ct", _banco.LerConfiguracao("usuario"));
            ClicaId("IBOK");
            TrocaPopUp(By.Id("tc1"), true);
            #region TELA DE LOGIN - BUSCA TECLAS DAS SENHAS E FORÇA LOGIN
            var senha = _banco.LerConfiguracao("senha");

            List<IWebElement> listaTeclas = new List<IWebElement>();
            listaTeclas.Add(driver.FindElement(By.Id("tc1")));
            listaTeclas.Add(driver.FindElement(By.Id("tc2")));
            listaTeclas.Add(driver.FindElement(By.Id("tc3")));
            listaTeclas.Add(driver.FindElement(By.Id("tc4")));
            listaTeclas.Add(driver.FindElement(By.Id("tc5")));
            listaTeclas.Add(driver.FindElement(By.Id("tc6")));
            listaTeclas.Add(driver.FindElement(By.Id("tc7")));
            listaTeclas.Add(driver.FindElement(By.Id("tc8")));
            listaTeclas.Add(driver.FindElement(By.Id("tc9")));
            listaTeclas.Add(driver.FindElement(By.Id("tc10")));

            var teclas = new System.Collections.ObjectModel.ReadOnlyCollection<IWebElement>(listaTeclas);

            foreach (var itemSENHA in senha.ToArray())
            {
                if (teclas.Count == 0)
                    teclas = driver.FindElements(By.ClassName("esconde"));

                foreach (var itemTECLA in teclas)
                {
                    if (itemTECLA.Text == itemSENHA.ToString())
                    {
                        itemTECLA.Click();
                        break;
                    }
                }
            }

            //clica no botão para entrar no banco

            driver.FindElements(By.ClassName("Coluna200"))[1].FindElement(By.CssSelector("a[onclick*='document.form1.submit()']")).Click();
            #endregion
        }

        private void Sair()
        {
            try
            {
                #region TRABALHO TERMINADO, SAI DO SITE!!!
                //FORÇA UM FOCO NO FRAME!!!
                driver.SwitchTo().DefaultContent();
                driver.SwitchTo().Frame(0);


                IList<IWebElement> linkMenu = driver.FindElements(By.CssSelector("*"));
                foreach (var item in linkMenu)
                {
                    try
                    {
                        if ((item.Text.ToUpper().Contains("SAIR")) && (item.TagName == "a"))
                        {
                            item.Click();
                            break;

                        }
                    }
                    catch { }
                }

                //FORÇA UM FOCO NO FRAME!!!
                driver.SwitchTo().DefaultContent();
                driver.SwitchTo().Frame(1);
                ClicaId("bt_enviar");
                #endregion
            }
            catch
            {
            }

            // fecha o navegador.
            FecharBrowser();
        }

        private void LerExtrato()
        {
            #region PROCURA MENU INICIAL EXTRATOS
            System.Threading.Thread.Sleep(new TimeSpan(0, 0, 5));
            //FORÇA UM FOCO NO FRAME!!!
            driver.SwitchTo().Frame(0);

            //clica no menu extrato
            IList<IWebElement> linkMenu = driver.FindElements(By.CssSelector("*"));
            foreach (var item in linkMenu)
            {
                try
                {
                    if ((item.Text.ToUpper().Contains("EXTRATOS E CONSULTAS")) && (item.TagName == "a"))
                    {
                        item.Click();
                        break;
                    }

                }
                catch { }
            }
            #endregion

            #region PROCURA BOTÃO EXTRATO MENSAL E FAZ DOWNLOAD DO EXCEL !!!
            //FORÇA UM FOCO NO FRAME!!!
            driver.SwitchTo().DefaultContent();
            driver.SwitchTo().Frame(1);


            IList<IWebElement> linkExtratoMensal = driver.FindElements(By.CssSelector("*"));
            foreach (var item in linkExtratoMensal)
            {
                try
                {
                    if ((item.Text.ToUpper().Contains("MENSAL")) && (item.TagName == "a"))
                    {
                        item.Click();
                        break;

                    }
                }
                catch { }
            }

            System.Threading.Thread.Sleep(new TimeSpan(0, 0, 5));

            //FAZ O DOWNLOAD DA PLANILHA EXCEL DO EXTRATO MENSAL!
            driver.FindElement(By.CssSelector("input[onclick*='document.form1.submit()']")).Click();
            #endregion

        }

        public string NomeUnico()
        {
            return "BanestesSite";
        }

        public void Iniciar(Banco banco)
        {
            _banco = banco;
        }

        protected override string URLSite()
        {
            return "http://www.banestes.com.br";
        }

    }
}

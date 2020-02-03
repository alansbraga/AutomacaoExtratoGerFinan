using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace AEGF.Infra
{
    public abstract class AcessoSelenium
    {
        public enum Browser
        {
            ChromeDriver,
            FirefoxDriver,
            InternetExplorerDriver
        }
        protected void IniciarBrowser(Browser browser = Browser.ChromeDriver)
        {
            driver = CriarBrowser(browser);
            driver.Navigate().GoToUrl(URLSite());
            driver.Manage().Window.Maximize();
        }

        protected virtual IWebDriver CriarBrowser(Browser browser = Browser.ChromeDriver)
        {
            return browser switch
            {
                Browser.FirefoxDriver => new FirefoxDriver(),
                Browser.InternetExplorerDriver => new InternetExplorerDriver(),
                _ => new ChromeDriver(),
            };
        }

        protected void FecharBrowser()
        {
            driver.Quit();
        }

        protected void TrocaPopUp(By seletor, bool aguardar = false)
        {
            if (aguardar)
                Aguardar(seletor, true, 60, true);
            //var frame = driver.FindElement(seletor);
            //driver.SwitchTo().Frame(frame);
        }

        protected void TrocaFrame(By seletor, bool aguardar = false)
        {
            if (aguardar)
                Aguardar(seletor);
            var frame = driver.FindElement(seletor);
            driver.SwitchTo().Frame(frame);
        }

        protected void TrocaFrameXPath(string xPath, bool aguardar = false)
        {
            TrocaFrame(By.XPath(xPath), aguardar);
        }

        protected void TrocaFrameNome(string nome, bool aguardar = false)
        {
            TrocaFrame(By.Name(nome), aguardar);
        }

        protected void TrocaFrameId(string id, bool aguardar = false)
        {
            TrocaFrame(By.Id(id), aguardar);
        }

        private void DigitaTexto(By seletor, string valor)
        {
            var query = driver.FindElement(seletor);
            query.SendKeys(valor);
        }

        protected void DigitaTextoId(string id, string valor)
        {
            DigitaTexto(By.Id(id), valor);
        }

        protected void DigitaTextoName(string id, string valor)
        {
            DigitaTexto(By.Name(id), valor);
        }

        protected void DigitaTextoXPath(string xPath, string valor)
        {
            DigitaTexto(By.XPath(xPath), valor);
        }

        protected void Clica(By seletor, bool aguardar = false)
        {
            if (aguardar)
                Aguardar(seletor);
            var query = driver.FindElement(seletor);
            query.Click();

        }
        protected void ClicaId(string id, bool aguardar = false)
        {
            Clica(By.Id(id), aguardar);
        }

        protected void ClicaCSS(string css)
        {
            Clica(By.CssSelector(css));
        }


        protected void ClicaXPath(string xPath, bool aguardar = false)
        {
            Clica(By.XPath(xPath), aguardar);
        }


        protected void SelecionaValorXPath(string xPath, string valor)
        {
            var dropdown = new SelectElement(driver.FindElement(By.XPath(xPath)));
            dropdown.SelectByValue(valor);
        }

        protected void SelecionaIndexXPath(string xPath, int valor)
        {
            var dropdown = new SelectElement(driver.FindElement(By.XPath(xPath)));
            dropdown.SelectByIndex(valor);
        }

        protected void AguardarId(string id, bool garantirHabilitado = true)
        {
            Aguardar(By.Id(id), garantirHabilitado);
        }

        protected void AguardarName(string id)
        {
            Aguardar(By.Name(id));
        }
        
        protected void AguardarXPath(string xPath)
        {
            Aguardar(By.XPath(xPath));
        }

        protected void AguardarCSS(string css)
        {
            Aguardar(By.CssSelector(css));
        }


        protected void Aguardar(By seletor)
        {
            Aguardar(seletor, true, 6000, false);
        }

        //protected void Aguardar(By seletor, bool garantirHabilitado = true, int segundos = 60)
        //{
        //    Aguardar(seletor, garantirHabilitado, segundos, false);
        //}

        protected void Aguardar(By seletor, bool garantirHabilitado = true, int segundos = 30, bool trocaPopUp = false)
        {
            var tempoEspera = TimeSpan.FromSeconds(segundos);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(segundos))
            {
                PollingInterval = TimeSpan.FromSeconds(5)
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

            var sw = new Stopwatch();
            sw.Start();
            wait.Until(webDriver =>
            {
                try
                {                    
                    if (trocaPopUp)
                        //FORÇA UM FOCO NO POPUP QUE ABRIU !!!!
                        webDriver.SwitchTo().Window(webDriver.WindowHandles.ToList().Last());

                    var element = webDriver.FindElement(seletor);
                    var retorno = element != null;
                    if (garantirHabilitado)
                        retorno = retorno && element.Displayed && element.Enabled;
                    return retorno;

                }
                catch (Exception ex)
                {
                    if (sw.Elapsed > tempoEspera)
                        return true;

                    return false;
                }
            });

        }

        public bool ExisteId(string id)
        {
            return Existe(By.Id(id));
        }

        public bool ExisteXPath(string xPath)
        {
            return Existe(By.XPath(xPath));
        }

        private bool Existe(By seletor)
        {
            try
            {
                var element = driver.FindElement(seletor);
                return element != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool VisivelId(string id)
        {
            return Visivel(By.Id(id));
        }

        public bool VisivelName(string id)
        {
            return Visivel(By.Name(id));
        }

        public bool VisivelXPath(string xPath)
        {
            return Visivel(By.XPath(xPath));
        }

        private bool Visivel(By seletor)
        {
            try
            {
                var element = driver.FindElement(seletor);
                return element.Displayed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HabilitadoId(string id)
        {
            return Habilitado(By.Id(id));
        }

        public bool HabilitadoName(string id)
        {
            return Habilitado(By.Name(id));
        }

        public bool HabilitadoXPath(string xPath)
        {
            return Habilitado(By.XPath(xPath));
        }

        private bool Habilitado(By seletor)
        {
            try
            {
                var element = driver.FindElement(seletor);
                return element.Displayed && element.Enabled;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string LerTextoCSS(string css)
        {
            return LerTexto(By.CssSelector(css));
        }

        public string LerTextoId(string id)
        {
            return LerTexto(By.Id(id));
        }

        public string LerTextoXPath(string xpath)
        {
            return LerTexto(By.XPath(xpath));
        }

        public string LerTexto(By seletor)
        {
            var element = driver.FindElement(seletor);
            return element.Text;

        }

        protected abstract string URLSite();

        public void Tempo(int segundos = 5)
        {
            // Site da CEF é temperamental, se for muito rápido ele dá erro
            // dá uns 5 segundos de espera....
            System.Threading.Thread.Sleep(new TimeSpan(0, 0, segundos));
        }


        public IWebDriver driver { get; set; }
    }
}

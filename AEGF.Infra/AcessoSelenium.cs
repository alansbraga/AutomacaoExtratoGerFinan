using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace AEGF.Infra
{
    public abstract class AcessoSelenium
    {
        protected void IniciarBrowser()
        {
            driver = CriarBrowser();
            driver.Navigate().GoToUrl(URLSite());
            driver.Manage().Window.Maximize();
        }

        protected virtual IWebDriver CriarBrowser()
        {
            // todo configurar o browser
            //IWebDriver driver = new FirefoxDriver();
            return new ChromeDriver();
            //return new FirefoxDriver();
        }

        protected void FecharBrowser()
        {
            driver.Quit();
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

        protected void DigitaTextoXPath(string xPath, string valor)
        {
            DigitaTexto(By.Id(xPath), valor);
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

        protected void AguardarId(string id)
        {
            Aguardar(By.Id(id));
        }

        protected void AguardarXPath(string xPath)
        {
            Aguardar(By.XPath(xPath));
        }

        protected void AguardarCSS(string css)
        {
            Aguardar(By.CssSelector(css));
        }


        protected void Aguardar(By seletor, bool garantirHabilitado = true, int segundos = 60)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(segundos));
            
            wait.Until(webDriver =>
            {
                try
                {
                    var element = webDriver.FindElement(seletor);
                    var retorno = element != null;
                    if (garantirHabilitado)
                        retorno = retorno && element.Displayed && element.Enabled;
                    return retorno;

                }
                catch (Exception)
                {
                    return false;
                }
            });

        }

        public bool ExisteId(string id)
        {
            return Existe(By.Id(id));
        }

        private bool Existe(By seletor)
        {
            try
            {
                var elemento = driver.FindElement(seletor);
                return elemento != null;
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

        public string LerTexto(By seletor)
        {
            var elemento = driver.FindElement(seletor);
            return elemento.Text;
            
        }

        protected abstract string URLSite();


        public IWebDriver driver { get; set; }
    }
}

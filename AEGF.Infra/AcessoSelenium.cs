using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
        }

        protected void FecharBrowser()
        {
            driver.Quit();
        }

        protected void TrocaFrame(By seletor)
        {
            var frame = driver.FindElement(seletor);
            driver.SwitchTo().Frame(frame);
        }

        protected void TrocaFrameXPath(string xPath)
        {
            TrocaFrame(By.XPath(xPath));
        }

        protected void TrocaFrameNome(string nome)
        {
            TrocaFrame(By.Name(nome));
        }

        protected void TrocaFrameId(string id)
        {
            TrocaFrame(By.Id(id));
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

        protected void Clica(By seletor)
        {
            var query = driver.FindElement(seletor);            
            query.Click();

        }
        protected void ClicaId(string id)
        {
            Clica(By.Id(id));
        }

        protected void ClicaCSS(string css)
        {
            Clica(By.CssSelector(css));
        }


        protected void ClicaXPath(string xPath)
        {
            Clica(By.XPath(xPath));
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


        protected void Aguardar(By seletor, bool garantirHabilitado = true, int segundos = 10)
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

        protected abstract string URLSite();


        public IWebDriver driver { get; set; }
    }
}

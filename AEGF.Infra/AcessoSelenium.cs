﻿using System;
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
            // todo configurar o browser
            //IWebDriver driver = new FirefoxDriver();
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl(URLSite());
            
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

        protected void Clica(By seletor)
        {
            var query = driver.FindElement(seletor);
            query.Click();

        }
        protected void ClicaId(string id)
        {
            Clica(By.Id(id));
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

        protected abstract string URLSite();


        public IWebDriver driver { get; set; }
    }
}
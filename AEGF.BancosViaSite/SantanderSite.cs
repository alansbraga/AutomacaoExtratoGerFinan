using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;

namespace AEGF.BancosViaSite
{
    public class SantanderSite: AcessoSelenium, IBancoAcesso
    {
        private Banco _banco;

        public IEnumerable<Extrato> LerExtratos()
        {
            throw new NotImplementedException();
        }

        public string NomeUnico()
        {
            return "SantanderSite";
        }

        public void Iniciar(Banco banco)
        {
            _banco = banco;
        }
    }
}

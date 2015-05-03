using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;

namespace AutomacaoExtratoGerFinanConsole
{
    public class GerenciadorFinanceiroAcessoConsole: IGerenciadorFinanceiroAcesso
    {
        public void ProcessarContas(IEnumerable<Extrato> extratos)
        {
            foreach (var extrato in extratos)
            {
                Console.WriteLine(extrato);
            }

            Console.WriteLine("Fim do Extrato. Aperte enter");
            Console.ReadLine();
        }
    }
}

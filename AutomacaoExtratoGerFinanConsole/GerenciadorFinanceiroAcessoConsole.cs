using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using Newtonsoft.Json;

namespace AutomacaoExtratoGerFinanConsole
{
    public class GerenciadorFinanceiroAcessoConsole: IGerenciadorFinanceiroAcesso
    {
        public void ProcessarContas(IEnumerable<Extrato> extratos)
        {
            foreach (var extrato in extratos)
            {
                foreach (var transacao in extrato.Transacoes)
                {
                    Console.WriteLine(transacao);
                }
                
            }

            File.WriteAllText(@"c:\tmp\extratos.json", JsonConvert.SerializeObject(extratos));
            

            Console.WriteLine("Fim do Extrato. Aperte enter");
            Console.ReadLine();
        }
    }
}

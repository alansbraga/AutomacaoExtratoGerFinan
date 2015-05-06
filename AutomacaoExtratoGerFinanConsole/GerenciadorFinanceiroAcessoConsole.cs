using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using AEGF.Infra;
using Newtonsoft.Json;

namespace AutomacaoExtratoGerFinanConsole
{
    public class GerenciadorFinanceiroAcessoConsole: IGerenciadorFinanceiroAcesso
    {
        private GerenciadorFinanceiro _gerenciador;

        public void ProcessarContas(IEnumerable<Extrato> extratos)
        {
            foreach (var extrato in extratos)
            {
                var gerador = new GeradorOFX(extrato, new OpcoesOFX
                                                      {
                                                          IgnorarPositivos = extrato.CartaoCredito,
                                                          MudaDataParaMesReferencia = extrato.CartaoCredito,
                                                          MultiplicarMenosUm = extrato.CartaoCredito
                                                      });
                Console.WriteLine("{0} {1}: {2}", extrato.Descricao, extrato.Referencia, gerador.GravarTemporario());

            }

            

            File.WriteAllText(@"c:\tmp\extratos.json", JsonConvert.SerializeObject(extratos));
            

            Console.WriteLine("Fim do Extrato. Aperte enter");
            Console.ReadLine();
        }

        public string NomeUnico()
        {
            return "";
        }

        public void Iniciar(GerenciadorFinanceiro gerenciador)
        {
            _gerenciador = gerenciador;
        }
    }
}

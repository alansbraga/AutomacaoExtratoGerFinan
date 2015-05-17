using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using AEGF.Infra;

namespace AEGF.ServicoAplicacao
{
    public class GFGeradorOFX : IGerenciadorFinanceiroAcesso
    {
        private GerenciadorFinanceiro _gerenciador;

        public void ProcessarContas(IEnumerable<Extrato> extratos)
        {
            foreach (var extrato in extratos)
            {
                var gerador = new GeradorOFX(extrato, new OpcoesOFX()
                {
                    IgnorarPositivos = extrato.CartaoCredito,
                    MudaDataParaMesReferencia = extrato.CartaoCredito,
                    MultiplicarMenosUm = extrato.CartaoCredito
                });
                var arquivo = Path.Combine(_gerenciador.LerConfiguracao("caminho"), gerador.NomeArquivoExtrato());
                gerador.GravarOFX(arquivo);
            }                       
        }

        public string NomeUnico()
        {
            return "GeradorOFX";
        }

        public void Iniciar(GerenciadorFinanceiro gerenciador)
        {
            _gerenciador = gerenciador;
        }
    }
}

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
                var conta = _gerenciador.LerConta(extrato.Descricao);

                if (conta == null)
                    continue;

                var contaDestino = conta.ContaDestino;

                if (String.IsNullOrEmpty(contaDestino))
                    continue;

                var gerador = new GeradorOFX(extrato, new OpcoesOFX(conta));
                var nomeArquivo = GeradorOFX.MakeValidFileName(String.Format("{0} - {1}.ofx", contaDestino, extrato.Referencia.ToString("yyyyMMdd")));
                var arquivo = Path.Combine(_gerenciador.LerConfiguracao("caminho"), nomeArquivo);
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

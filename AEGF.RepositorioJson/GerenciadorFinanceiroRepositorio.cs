using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Repositorios;
using Newtonsoft.Json;

namespace AEGF.RepositorioJson
{
    public class GerenciadorFinanceiroRepositorio: IGerenciadorFinanceiroRepositorio
    {
        public IEnumerable<GerenciadorFinanceiro> ObterTodos()
        {
            var nomeArquivo = "gerenciador.json";
            var doArquivo = JsonConvert.DeserializeObject<IEnumerable<GerenciadorLocal>>(File.ReadAllText(nomeArquivo));
            foreach (var gerenciadorLocal in doArquivo)
            {
                foreach (var configuracao in gerenciadorLocal.Configuracoes)
                {
                    gerenciadorLocal.AdicionaConfiguracao(configuracao.Nome, configuracao.Valor);
                }
                foreach (var conta in gerenciadorLocal.Contas)
                {
                    gerenciadorLocal.AdicionaConta(conta);
                }
            }
            return doArquivo;
        }
    }
}

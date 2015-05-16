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
    public class GerenciadorFinanceiroRepositorio : RepositorioBase<GerenciadorFinanceiro>, IGerenciadorFinanceiroRepositorio
    {
        public GerenciadorFinanceiroRepositorio(IUnidadeTrabalhoJson unidadeTrabalho): base(unidadeTrabalho)
        {

        }

       
        protected override void AntesAdicionar(GerenciadorFinanceiro entidade)
        {
            
        }

        protected override string DefineNomeArquivo()
        {
            return "gerenciador.json";
        }

        protected override void ProcessarPosCarregamento(IEnumerable<GerenciadorFinanceiro> doArquivo)
        {
        }

    }
}

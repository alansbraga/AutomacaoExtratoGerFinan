using AEGF.Dominio.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;

namespace AEGF.RepositorioJson
{
    public class ExtratoRepositorio : RepositorioBase<Extrato>, IExtratoRepositorio
    {
        public ExtratoRepositorio(IUnidadeTrabalhoJson unidadeTrabalho): base(unidadeTrabalho)
        {

        }

        protected override void AntesAdicionar(Extrato entidade)
        {
            
        }

        protected override string DefineNomeArquivo()
        {
            return "extrato.json";
        }

        protected override void ProcessarPosCarregamento(IEnumerable<Extrato> doArquivo)
        {
        }
        
    }
}

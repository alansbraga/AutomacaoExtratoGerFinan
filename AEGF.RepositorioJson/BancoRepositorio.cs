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
    public class BancoRepositorio : RepositorioBase<Banco>, IBancoRepositorio
    {
        public BancoRepositorio(IUnidadeTrabalhoJson unidadeTrabalho) : base(unidadeTrabalho)
        {

        }

        protected override string DefineNomeArquivo()
        {
            return "banco.json";
        }

        protected override void AntesAdicionar(Banco entidade)
        {
            
        }

        protected override void ProcessarPosCarregamento(IEnumerable<Banco> doArquivo)
        {
        }

    }
}

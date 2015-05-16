using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;

namespace AEGF.RepositorioJson
{
    public class UnidadeTrabalhoJson : IUnidadeTrabalhoJson
    {
        private readonly IList<IRepositorioJson> _repositorios;

        public UnidadeTrabalhoJson()
        {
            _repositorios = new List<IRepositorioJson>();
        }

        public void AdicionarRepositorio(IRepositorioJson repositorio)
        {
            _repositorios.Add(repositorio);
        }

        public void Cancelar()
        {
            foreach (var rep in _repositorios)
            {                
                rep.CarregarJson();
            }
        }

        public void Gravar()
        {
            foreach (var rep in _repositorios)
            {
                rep.SalvarJson();
            }
        }
    }
}

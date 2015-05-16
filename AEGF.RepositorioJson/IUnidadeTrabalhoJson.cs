using AEGF.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.RepositorioJson
{
    public interface IUnidadeTrabalhoJson: IUnidadeTrabalho
    {
        void AdicionarRepositorio(IRepositorioJson repositorio);
    }
}

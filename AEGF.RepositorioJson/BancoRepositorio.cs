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
    public class BancoRepositorio: IBancoRepositorio
    {
        public IEnumerable<Banco> ObterBancos()
        {
            var nomeArquivo = "banco.json";
            var doArquivo = JsonConvert.DeserializeObject<IEnumerable<BancoLocal>>(File.ReadAllText(nomeArquivo));
            return doArquivo;
        }
    }
}

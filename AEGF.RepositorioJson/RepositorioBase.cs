using AEGF.Dominio;
using AEGF.Dominio.Repositorios;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.RepositorioJson
{
    public abstract class RepositorioBase<T> : IRepositorioJson, IRepositorioBase<T> where T : Entidade
    {
        protected readonly List<T> _dados;
        protected RepositorioBase(IUnidadeTrabalhoJson unidadeTrabalho)
        {
            _dados = new List<T>();
            unidadeTrabalho.AdicionarRepositorio(this);
            CarregarJson();
        }

        public void CarregarJson()
        {
            var nomeArquivo = DefineNomeArquivo();
            _dados.Clear();
            if (!File.Exists(nomeArquivo))
                return;

            var doArquivo = JsonConvert.DeserializeObject<IEnumerable<T>>(File.ReadAllText(nomeArquivo));
            ProcessarPosCarregamento(doArquivo);
            _dados.AddRange(doArquivo);
        }

        public void SalvarJson()
        {
            var nomeArquivo = DefineNomeArquivo();
            var json = JsonConvert.SerializeObject(_dados);
            File.WriteAllText(nomeArquivo, json);
        }

        public void Adicionar(T entidade)
        {
            AntesAdicionar(entidade);
            if (entidade.Id != 0) 
                return;
            entidade.Id = BuscaUltimoId();
            _dados.Add(entidade);
        }


        protected virtual int BuscaUltimoId()
        {
            if (_dados.Any())
                return _dados.Max(e => e.Id) + 1;
            return 1;
        }

        public IEnumerable<T> ObterTodos()
        {            
            return _dados;
        }

        protected abstract void ProcessarPosCarregamento(IEnumerable<T> doArquivo);
        protected abstract string DefineNomeArquivo();
        protected abstract void AntesAdicionar(T entidade);
    }
}

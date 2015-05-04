using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Dominio
{
    public class Banco
    {
        public Banco()
        {
            _configuracoes = new List<BancoConfiguracao>();
        }

        private List<BancoConfiguracao> _configuracoes;
        public virtual int Id { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string NomeAcesso { get; set; }

        public virtual IEnumerable<BancoConfiguracao> Configuracoes
        {
            get { return _configuracoes; }
            set
            {
                _configuracoes.Clear();
                _configuracoes.AddRange(value);
            }
        }

        public void AdicionaConfiguracao(string nome, string valor)
        {
            // todo verifica nome
            // todo verifica valor
            var cfg = new BancoConfiguracao()
            {
                Nome = nome,
                Valor = valor
            };
            _configuracoes.Add(cfg);
        }

        public string LerConfiguracao(string nome)
        {
            return Configuracoes.Single(configuracao => configuracao.Nome == nome).Valor;
        }
    }
}

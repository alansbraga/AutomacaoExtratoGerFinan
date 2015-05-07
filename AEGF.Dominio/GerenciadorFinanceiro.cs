using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Dominio
{
    public class GerenciadorFinanceiro
    {
        private List<GerenciadorFinanceiroConfiguracao> _configuracoes;
        private List<GerenciadorFinanceiroContas> _contas;
        public virtual int Id { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Nome { get; set; }

        public GerenciadorFinanceiro()
        {
            _configuracoes = new List<GerenciadorFinanceiroConfiguracao>();
            _contas = new List<GerenciadorFinanceiroContas>();
        }

        public virtual IEnumerable<GerenciadorFinanceiroConfiguracao> Configuracoes
        {
            get { return _configuracoes; }
            set
            {
                _configuracoes.Clear();
                _configuracoes.AddRange(value);
            }
        }

        public virtual IEnumerable<GerenciadorFinanceiroContas> Contas
        {
            get { return _contas; }
            set
            {
                _contas.Clear();
                _contas.AddRange(value);
            }
        }


        public void AdicionaConfiguracao(string nome, string valor)
        {
            // todo verifica nome
            // todo verifica valor
            var cfg = new GerenciadorFinanceiroConfiguracao()
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

        public void AdicionaConta(GerenciadorFinanceiroContas conta)
        {
            // todo verificações
            _contas.Add(conta);
        }

        public GerenciadorFinanceiroContas LerConta(string contaOrigem)
        {
            return Contas.SingleOrDefault(conta => conta.ContaOrigem == contaOrigem);
        }


    }
}

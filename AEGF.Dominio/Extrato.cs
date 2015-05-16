using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Dominio
{
    public class Extrato: Entidade
    {
        private readonly List<Transacao> _transacoes;

        public Extrato()
        {
            _transacoes = new List<Transacao>();
        }
        public virtual bool CartaoCredito { get; set; }
        public virtual DateTime Referencia { get; set; }
        public virtual string Descricao { get; set; }
        public virtual double SaldoAnterior { get; set; }

        public virtual IEnumerable<Transacao> Transacoes
        {
            get { return _transacoes; }
            set
            {
                _transacoes.Clear();
                _transacoes.AddRange(value);
            }
        }

        public void AdicionaTransacao(Transacao novaTransacao)
        {
            _transacoes.Add(novaTransacao);
        }

        public double TotalMovimentacao { get; private set; }

        public void CalcularTotais()
        {
            TotalMovimentacao = 0;
            var atual = SaldoAnterior;
            foreach (var transacao in Transacoes)
            {
                if ((CartaoCredito && (transacao.Valor < 0)))
                    continue;
                atual += transacao.Valor;
                transacao.Saldo = atual;
                    TotalMovimentacao += transacao.Valor;
            }

        }
    }
}

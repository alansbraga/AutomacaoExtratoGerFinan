using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Dominio
{
    public class Transacao: Entidade, IEquatable<Transacao>
    {
        public Transacao()
        {
            Nova = true;
        }

        public virtual double Valor { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual string Descricao { get; set; }
        public virtual bool Nova { get; set; }
        public virtual double Saldo { get; set; }

        public bool Equals(Transacao other)
        {
            if (other == null)
                return false;
            return (Valor.Equals(other.Valor)) && (Data.Equals(other.Data)) && (Descricao.Equals(other.Descricao));
        }

        public override string ToString()
        {
            return String.Format("{0} - {1} - {2}", Data, Descricao, Valor);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Dominio
{
    public class Transacao
    {
        public virtual Double Valor { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual string Descricao { get; set; }

        public override string ToString()
        {
            return String.Format("{0} - {1} - {2}", Data, Descricao, Valor);
        }
    }
}

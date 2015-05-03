using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Dominio
{
    public class BancoConfiguracao
    {
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Valor { get; set; }
    }
}

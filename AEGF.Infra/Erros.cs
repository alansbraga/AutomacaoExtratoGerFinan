using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Infra
{
    public class Erros
    {
        public void InteragirErros(Exception e, Action<Exception> interacao)
        {
            var erro = e;
            while (erro != null)
            {
                interacao(erro);
                erro = erro.InnerException;
            }
        }

        public string ExceptionToString(Exception e)
        {
            StringBuilder stack = new StringBuilder();
            InteragirErros(e, exception =>
            {
                stack.AppendLine("------------------------------------------------");
                stack.Append("Mensagem: ");
                stack.AppendLine(exception.Message);
                stack.AppendFormat("Classe: {0}", e.GetType().Name);
                stack.AppendLine("");
                stack.Append("Stack: ");
                stack.AppendLine(exception.StackTrace);
            });
            return stack.ToString();
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using DotLiquid;

namespace AEGF.ServicoAplicacao
{
    public class FormatadorHtml: IFormatadorResumo
    {
        public void Formatar(string arquivoSaida, IEnumerable<Extrato> extratos)
        {
            var template = Template.Parse(File.ReadAllText("Resumo.template.html"));
            Template.RegisterFilter(typeof(MoneyFilter));
            RegistraTipo(typeof(Extrato));
            RegistraTipo(typeof(Transacao));
            var html = template.Render(Hash.FromDictionary(new Hash
            {
                {"extratos", extratos}
            }));

            File.WriteAllText(arquivoSaida, html);
        }

        private void RegistraTipo(Type type)
        {
            var a = type.GetProperties().Select(info => info.Name).ToArray();
            Template.RegisterSafeType(type, a);
        }
    }

    static class MoneyFilter
    {
        public static string Money(object input)
        {
            return string.Format(" {0:F2} ", input);
        }

    }
}

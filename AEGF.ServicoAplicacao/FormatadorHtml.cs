using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using AEGF.Infra;
using DotLiquid;

namespace AEGF.ServicoAplicacao
{
    public class FormatadorHtml: IFormatadorResumo
    {
        public void Formatar(string arquivoSaida, IEnumerable<Extrato> extratos, IEnumerable<Exception> erros)
        {
            var template = Template.Parse(File.ReadAllText("Resumo.template.html"));
            Template.RegisterFilter(typeof(MoneyFilter));
            RegistraTipo(typeof(Extrato));
            RegistraTipo(typeof(Transacao));
            var errosStr = CriaErrosString(erros);
            var html = template.Render(Hash.FromDictionary(new Hash
            {
                {"extratos", extratos},
                {"erros", errosStr}
            }));

            File.WriteAllText(arquivoSaida, html);
        }

        private IEnumerable<string> CriaErrosString(IEnumerable<Exception> erros)
        {
            var retorno = new List<string>();
            var erroTratamento = new Erros();
            foreach (var erro in erros)
            {
                retorno.Add(erroTratamento.ExceptionToString(erro));
            }            
            return retorno;
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;
using AEGF.Dominio.Servicos;
using Newtonsoft.Json;

namespace AutomacaoExtratoGerFinanConsole
{
    public class GerenciadorFinanceiroAcessoConsole: IGerenciadorFinanceiroAcesso
    {
        public void ProcessarContas(IEnumerable<Extrato> extratos)
        {
            foreach (var extrato in extratos)
            {
                CriaOfx(extrato);
                foreach (var transacao in extrato.Transacoes)
                {
                    Console.WriteLine(transacao);
                }
                
            }

            

            File.WriteAllText(@"c:\tmp\extratos.json", JsonConvert.SerializeObject(extratos));
            

            Console.WriteLine("Fim do Extrato. Aperte enter");
            Console.ReadLine();
        }

        private string formatDate(DateTime data)
        {
            return data.ToString("yyyyMMddhhmmss") + "[-3:GMT]";
        }

        private string formataValor(double valor)
        {
            var s = valor.ToString("####.00");
            s = s.Replace(',', '.');
            return s;
        }

        private void CriaOfx(Extrato extrato)
        {
            if (!extrato.Transacoes.Any())
                return;

            var mudaData = extrato.CartaoCredito;

            var menosUm = extrato.CartaoCredito;
            var ignoraPositivos = extrato.CartaoCredito;
            var novo = new StringBuilder(cabecalhoOFX(extrato));

            foreach (var item in extrato.Transacoes)
            {
                var valor = item.Valor;
                if (menosUm)
                    valor = valor*-1;

                if ((ignoraPositivos) && (!(valor < 0))) 
                    continue;

                var sDescricao = item.Descricao;
                var data = item.Data;
                if (mudaData)
                {
                    data = DateTime.Parse(extrato.Referencia);
                    data = new DateTime(data.Year, data.Month, 1);
                    sDescricao = sDescricao + " -- Dt.Mov: " + item.Data;
                }

                sDescricao = sDescricao.Replace('&', 'e');

                novo.AppendLine("\t\t\t\t\t<STMTTRN>");
                novo.AppendLine("\t\t\t\t\t\t<TRNTYPE>OTHER");
                novo.AppendLine("\t\t\t\t\t\t<DTPOSTED>" + formatDate(data));
                novo.AppendLine("\t\t\t\t\t\t<TRNAMT>" + formataValor(valor));
                novo.AppendLine("\t\t\t\t\t\t<FITID>00000000");
                novo.AppendLine("\t\t\t\t\t\t<CHECKNUM>00000000");
                novo.AppendLine("\t\t\t\t\t\t<PAYEEID>0");
                novo.AppendLine("\t\t\t\t\t\t<MEMO>" + sDescricao);
                novo.AppendLine("\t\t\t\t\t</STMTTRN>");
            }

            novo.AppendLine(rodapeOFX(extrato));

            GravarArquivo(extrato, novo.ToString());

        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        private void GravarArquivo(Extrato extrato, string conteudo)
        {
            var nomeArq = MakeValidFileName(extrato.Descricao + " " + extrato.Referencia + ".ofx");
            var arquivo = Path.Combine(@"c:\tmp\", nomeArq);
            File.WriteAllText(arquivo, conteudo);
        }

        private string rodapeOFX(Extrato extrato)
        {
            var retorno =
                "\t\t\t\t</BANKTRANLIST>\r\n" +
                "\t\t\t\t<LEDGERBAL>\r\n" +
                "\t\t\t\t\t<BALAMT>              0.00\r\n" +
                "\t\t\t\t\t<DTASOF>" + formatDate(new DateTime()) + "\r\n" +
                "\t\t\t\t</LEDGERBAL>\r\n" +
                "\t\t\t</STMTRS>\r\n" +
                "\t\t</STMTTRNRS>\r\n" +
                "\t</BANKMSGSRSV1>\r\n" +
                "</OFX>\r\n";
            return retorno;
        }

        private string cabecalhoOFX(Extrato extrato)
        {

            var retorno =
                "OFXHEADER:100\r\n" +
                "DATA:OFXSGML\r\n" +
                "VERSION:102\r\n" +
                "SECURITY:NONE\r\n" +
                "ENCODING:USASCII\r\n" +
                "CHARSET:1252\r\n" +
                "COMPRESSION:NONE\r\n" +
                "OLDFILEUID:NONE\r\n" +
                "NEWFILEUID:NONE\r\n" +
                "\r\n" +
                "<OFX>\r\n" +
                "\t<SIGNONMSGSRSV1>\r\n" +
                "\t\t<SONRS>\r\n" +
                "\t\t\t<STATUS>\r\n" +
                "\t\t\t\t<CODE>0\r\n" +
                "\t\t\t\t<SEVERITY>INFO\r\n" +
                "\t\t\t</STATUS>\r\n" +
                "\t\t\t<DTSERVER>" + formatDate(new DateTime()) + "\r\n" +
                "\t\t\t<LANGUAGE>ENG\r\n" +
                "\t\t\t<FI>\r\n" +
                "\t\t\t\t<ORG>SANTANDER\r\n" +
                "\t\t\t\t<FID>SANTANDER\r\n" +
                "\t\t\t</FI>\r\n" +
                "\t\t</SONRS>\r\n" +
                "\t</SIGNONMSGSRSV1>\r\n" +
                "\t<BANKMSGSRSV1>\r\n" +
                "\t\t<STMTTRNRS>\r\n" +
                "\t\t\t<TRNUID>1\r\n" +
                "\t\t\t<STATUS>\r\n" +
                "\t\t\t\t<CODE>0\r\n" +
                "\t\t\t\t<SEVERITY>INFO\r\n" +
                "\t\t\t</STATUS>\r\n" +
                "\t\t\t<STMTRS>\r\n" +
                "\t\t\t\t<CURDEF>BRL\r\n" +
                "\t\t\t\t<BANKACCTFROM>\r\n" +
                "\t\t\t\t\t<BANKID>033\r\n" +
                "\t\t\t\t\t<ACCTID>1234567890123\r\n" +
                "\t\t\t\t\t<ACCTTYPE>CHECKING\r\n" +
                "\t\t\t\t</BANKACCTFROM>\r\n" +
                "\t\t\t\t<BANKTRANLIST>\r\n" +
                "\t\t\t\t\t<DTSTART>" + formatDate(new DateTime()) + "\r\n" +
                "\t\t\t\t\t<DTEND>" + formatDate(new DateTime()) + "\r\n";

            return retorno;

        }
    }
}

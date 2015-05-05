using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEGF.Dominio;

namespace AEGF.Infra
{
    public class GeradorOFX
    {
        private readonly Extrato _extrato;
        private readonly OpcoesOFX _opcoes;

        public GeradorOFX(Extrato extrato, OpcoesOFX opcoes)
        {
            _extrato = extrato;
            _opcoes = opcoes;
        }

        private string FormataData(DateTime data)
        {
            return data.ToString("yyyyMMddhhmmss") + "[-3:GMT]";
        }

        private static string FormataValor(double valor)
        {
            var s = valor.ToString("####.00");
            s = s.Replace(',', '.');
            return s;
        }

        public string GravarTemporario()
        {
            var nomeArq = MakeValidFileName(_extrato.Descricao + " " + _extrato.Referencia + ".ofx");
            var arquivo = Path.Combine(Path.GetTempPath(), nomeArq);
            return GravarOFX(arquivo) ? arquivo : "";
        }

        public bool GravarOFX(string nomeArquivo)
        {
            var conteudo = CriaOfx();
            if (String.IsNullOrEmpty(conteudo))
                return false;
            File.WriteAllText(nomeArquivo, conteudo);
            return true;
        }

        private string CriaOfx()
        {
            if (!_extrato.Transacoes.Any())
                return "";

            var novo = new StringBuilder(CabecalhoOFX());

            foreach (var item in _extrato.Transacoes)
            {
                var valor = item.Valor;
                if (_opcoes.MultiplicarMenosUm)
                    valor = valor*-1;

                if ((_opcoes.IgnorarPositivos) && (!(valor < 0))) 
                    continue;

                var sDescricao = item.Descricao;
                var data = item.Data;
                if (_opcoes.MudaDataParaMesReferencia)
                {
                    data = _extrato.Referencia;
                    data = new DateTime(data.Year, data.Month, 1);
                    sDescricao = sDescricao + " -- Dt.Mov: " + item.Data;
                }

                sDescricao = sDescricao.Replace('&', 'e');

                novo.AppendLine("\t\t\t\t\t<STMTTRN>");
                novo.AppendLine("\t\t\t\t\t\t<TRNTYPE>OTHER");
                novo.AppendLine("\t\t\t\t\t\t<DTPOSTED>" + FormataData(data));
                novo.AppendLine("\t\t\t\t\t\t<TRNAMT>" + FormataValor(valor));
                novo.AppendLine("\t\t\t\t\t\t<FITID>00000000");
                novo.AppendLine("\t\t\t\t\t\t<CHECKNUM>00000000");
                novo.AppendLine("\t\t\t\t\t\t<PAYEEID>0");
                novo.AppendLine("\t\t\t\t\t\t<MEMO>" + sDescricao);
                novo.AppendLine("\t\t\t\t\t</STMTTRN>");
            }

            novo.AppendLine(RodapeOFX());

            return novo.ToString();

        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        private string RodapeOFX()
        {
            var retorno =
                "\t\t\t\t</BANKTRANLIST>\r\n" +
                "\t\t\t\t<LEDGERBAL>\r\n" +
                "\t\t\t\t\t<BALAMT>              0.00\r\n" +
                "\t\t\t\t\t<DTASOF>" + FormataData(DateTime.Now) + "\r\n" +
                "\t\t\t\t</LEDGERBAL>\r\n" +
                "\t\t\t</STMTRS>\r\n" +
                "\t\t</STMTTRNRS>\r\n" +
                "\t</BANKMSGSRSV1>\r\n" +
                "</OFX>\r\n";
            return retorno;
        }

        private string CabecalhoOFX()
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
                "\t\t\t<DTSERVER>" + FormataData(DateTime.Now) + "\r\n" +
                "\t\t\t<LANGUAGE>ENG\r\n" +
                "\t\t\t<FI>\r\n" +
                "\t\t\t\t<ORG>" + _extrato.Descricao + "\r\n" +
                "\t\t\t\t<FID>" + _extrato.Descricao + "\r\n" +
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
                "\t\t\t\t\t<DTSTART>" + FormataData(DateTime.Now) + "\r\n" +
                "\t\t\t\t\t<DTEND>" + FormataData(DateTime.Now) + "\r\n";

            return retorno;

        }
    }

    public class OpcoesOFX
    {
        public bool MudaDataParaMesReferencia { get; set; }
        public bool MultiplicarMenosUm { get; set; }
        public bool IgnorarPositivos { get; set; }
            
    }
}


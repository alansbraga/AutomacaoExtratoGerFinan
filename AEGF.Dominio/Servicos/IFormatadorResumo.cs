using System;
using System.Collections.Generic;

namespace AEGF.Dominio.Servicos
{
    public interface IFormatadorResumo
    {
        void Formatar(string arquivoSaida, IEnumerable<Extrato> extratos, IEnumerable<Exception> erros);
    }
}
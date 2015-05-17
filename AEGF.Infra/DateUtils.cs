using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGF.Infra
{
    public static class DateUtils
    {
        public static DateTime PrimeiroDia(this DateTime data)
        {
            return new DateTime(data.Year, data.Month, 1);
        }
    }
}

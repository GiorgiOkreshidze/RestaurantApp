using Function.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Formatters
{
    public interface IReportFormatter
    {
        string Format(List<SummaryEntry> summary);
    }
}

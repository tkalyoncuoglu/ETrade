using Business.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Abstract
{
    public interface IReportService
    {
        List<ReportItemModel> GetList(ReportFilterModel filter, bool useInnerJoin = false);
    }
}

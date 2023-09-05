using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;

namespace Repositories.Abstract
{
    public interface IReportRepository
    {
        public List<ReportItem> GetList(ReportFilter filter, bool useInnerJoin = false);
    }
}

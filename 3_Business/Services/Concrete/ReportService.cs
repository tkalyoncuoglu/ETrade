using Business.Models.Report;
using Business.Services.Abstract;
using DataAccess;
using Repositories.Abstract;

namespace Services.Concrete
{
    

    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;

        public ReportService(IReportRepository repo)
        {
            _repo = repo;
        }


        public List<ReportItemModel> GetList(ReportFilterModel filter, bool useInnerJoin = false)
        {
            var reportFilter = filter is not null ? new ReportFilter()
            {
                CategoryId = filter.CategoryId,
                ProductName = filter.ProductName,
                UnitPriceBegin = filter.UnitPriceBegin,
                UnitPriceEnd = filter.UnitPriceEnd,
                StockAmountBegin = filter.StockAmountBegin,
                StockAmountEnd = filter.StockAmountEnd,
                ExpirationDateBegin = filter.ExpirationDateBegin,
                ExpirationDateEnd = filter.ExpirationDateEnd,
                StoreIds = filter.StoreIds
            } : null;

            var r = _repo.GetList(reportFilter);

            var result = r.Select(x => new ReportItemModel
            {
                ProductName = x.ProductName,
                ProductDescription = x.ProductDescription,
                UnitPrice = x.UnitPrice,
                StockAmount = x.StockAmount,
                ExpirationDate = x.ExpirationDate,
                CategoryName = x.CategoryName,
                CategoryDescription = x.CategoryDescription,
                StoreName = x.StoreName,
                CategoryId = x.CategoryId,
                StoreId = x.StoreId,
                UnitPriceValue = x.UnitPriceValue,
                StockAmountValue = x.StockAmountValue,
                ExpirationDateValue = x.ExpirationDateValue
            }).ToList();
            return result;

        }
    }
}

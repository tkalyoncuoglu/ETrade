using AppCore.DataAccess.EntityFramework.Bases;
using AppCore.Records.Bases;
using DataAccess.Contexts;

namespace DataAccess.Repositories
{
    // new'lenebilen ve RecordBase'den miras alan tip olarak TEntity üzerinden entity tipini kullanacak,
    // RepoBase abstract class'ından miras alan ve veritabanı işlemlerini gerçekleştirecek somut class.
    public class Repo<TEntity> : RepoBase<TEntity> where TEntity : RecordBase, new()
    {
        public Repo(ETradeContext dbContext) : base(dbContext) // projemizin ETradeContext tipindeki dbContext'i Dependency Injection (Constructor Injection) ile
                                                               // Repo'ya dolayısıyla da RepoBase'e dışarıdan new'lenerek enjekte edilecek.
        {
        }
    }
}

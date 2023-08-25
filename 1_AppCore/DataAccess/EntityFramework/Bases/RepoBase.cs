#nullable disable

using AppCore.DataAccess.Bases;
using AppCore.Records.Bases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCore.DataAccess.EntityFramework.Bases
{
    // Repository Pattern: Veritabanındaki tablolarda (entity) kolay ve merkezi olarak CRUD (create, read, update, delete)
    // işlemlerinin yapılmasını sağlayan tasarım desenidir (design pattern). Önce DbSet'ler üzerinde istenilen değişiklikler yapılır
    // daha sonra tek bir iş olarak veritabanına yapılan değişiklikler SQL sorguları çalıştırılarak yansıtılır (Unit of Work).



    //public abstract class RepoBase<TEntity> 
    // Tip olarak TEntity üzerinden herhangi bir tip kullanacak abstract class.

    //public abstract class RepoBase<TEntity> : where TEntity : class 
    // Referans tip olarak TEntity üzerinden herhangi bir tip kullanacak abstract class.

    //public abstract class RepoBase<TEntity> : where TEntity : class, new() 
    // new'lenebilen referans tip olarak TEntity üzerinden herhangi bir tip kullanacak abstract class.

    //public abstract class RepoBase<TEntity> : where TEntity : RecordBase, new() 
    // new'lenebilen ve RecordBase'den miras alan tip olarak TEntity üzerinden entity tipini kullanacak abstract class.



    // new'lenebilen ve RecordBase'den miras alan tip olarak TEntity üzerinden entity tipini kullanacak,
    // aynı zamanda IDisposable interface'ini IRepoBase interface'i ile birlikte implemente edecek abstract class.
    public abstract class RepoBase<TEntity> : IRepoBase<TEntity> where TEntity : RecordBase, new()
    {
        protected readonly DbContext _dbContext; // DbContext EntityFramework'ün CRUD işlemleri yapmamızı sağlayan temel class'ı,
                                                 // readonly olarak sadece constructor üzerinden veya bu satırda set edilebilir.
                                                 // protected erişim bildirgeci ile _dbContext'in ihtiyaç halinde sadece bu class'tan miras alan repository'lerde kullanılması sağlanır.

        protected RepoBase(DbContext dbContext) // dbContext Dependency Injection (Constructor Injection) ile RepoBase'e dışarıdan new'lenerek enjekte edilecek.
        {
            _dbContext = dbContext;
        }

        // Read işlemi: ilgili entity için sorguyu oluşturur ancak çalıştırmaz.
        // Sorguyu çalıştırmak için ToList, SingleOrDefault, vb. methodları çağrılmalıdır.
        // isNoTracking parametresi false gönderildiğinde sorgu üzerinden dönülen DbSet'te değişikliklerin
        // takip edilmesi sağlanır, değikiliklerin takip edilmemesi için isNoTracking parametresi true gönderilmelidir.
        // virtual tanımladık ki bu class'dan miras alan class'larda ihtiyaca göre bu method ezilebilsin ve implementasyonu özelleştirilebilsin.
        public virtual IQueryable<TEntity> Query(bool isNoTracking = false)
        {
            if (isNoTracking)
                return _dbContext.Set<TEntity>().AsNoTracking();
            return _dbContext.Set<TEntity>();
        }

        // Read işlemi: yukarıdaki Query methodunun predicate (koşul veya koşullar) ile bool sonuç dönen,
        // bir veya isteğe göre daha fazla koşulun and ya da or ile birleştirilerek sorguyu where ile
        // filtreleyen ve dönen overload methodu.
        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool isNoTracking = false)
        {
            return Query(isNoTracking).Where(predicate);
        }

        // Read işlemi: bu class'ta belirtilen tip dışında belirtilen başka bir entity tipi üzerinden sorgu oluşturmamızı sağlar.
        // where tip tanımlanan methodlarda da class'larda kullanıldığı şekilde kullanılabilir.
        public virtual IQueryable<TRelationalEntity> Query<TRelationalEntity>() where TRelationalEntity : class, new()
        {
            return _dbContext.Set<TRelationalEntity>();
        }

        // Create işlemi: gönderilen entity'yi DbSet'e ekler ve eğer save parametresi true ise değişikliği Save methodu üzerinden veritabanına yansıtır.
        public virtual void Add(TEntity entity, bool save = true)
        {
            entity.Guid = Guid.NewGuid().ToString(); // her eklenecek kayıt için tekil bir Guid oluşturup entity'e atıyoruz ki istenirse Id yerine Guid üzerinden de işlemler yapılabilsin

            //DbContext.Set<TEntity>().Add(entity); // aşağıdaki satır ile de ekleme işlemi yapılabilir.
            _dbContext.Add(entity);

            if (save)
                Save();
        }

        // Update işlemi: gönderilen entity'yi DbSet'te günceller ve eğer save parametresi true ise değişikliği Save methodu üzerinden veritabanına yansıtır.
        public virtual void Update(TEntity entity, bool save = true)
        {
            //DbContext.Set<TEntity>().Update(entity); // aşağıdaki satır ile de güncelleme işlemi yapılabilir.
            _dbContext.Update(entity);

            if (save)
                Save();
        }

        // Delete işlemi: gönderilen koşul veya koşullar üzerinden entity'lere ulaşır, DbSet'ten entity'leri çıkarır,
        // son olarak Save methodu ile save parametresi true gönderildiyse tüm değişiklikleri tek seferde veritabanına yansıtır.
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate, bool save = true)
        {
            var entities = Query(predicate).ToList(); // ToList methodu sorgu oluşturulduktan sonra çağrılır ve geriye sorgu sonucundan
                                                      // dönen kayıtları bir obje listesi olarak döner.
            _dbContext.Set<TEntity>().RemoveRange(entities);
            
            if (save)
                Save();
        }

        // Delete işlemi: TEntity tipindeki entity'nin başka entity tipindeki bir referansı üzerinden ilişkilerini tutan TRelationalEntity tipi ile
        // bir veya daha fazla koşul için TEntity tipindeki entity'nin ilişkili kayıtlarının silinmesini sağlar, bu method çağrıldıktan sonra genelde
        // TEntity tipindeki entity güncellendiği veya silindiği için save parametresi default false olarak atanmıştır.
        // Dolayısıyla TEntity tipindeki entity de güncellendikten veya silindikten sonra değişiklikler Save methodu çağrılarak tek seferde veritabanına yansıtılabilir.
        // TEntity tipi dışındaki generic tipler bu methodda olduğu gibi kullanılabilir.
        public virtual void Delete<TRelationalEntity>(Expression<Func<TRelationalEntity, bool>> predicate, bool save = false) where TRelationalEntity : class, new()
        {
            var relationalEntities = Query<TRelationalEntity>().Where(predicate).ToList(); // yukarıdaki ilişkili entity'ler için oluşturduğumuz Query methodundan listeyi çekiyoruz.
            _dbContext.Set<TRelationalEntity>().RemoveRange(relationalEntities); // daha sonra ilişkili entity DbSet'inden çektiğimiz ilişkili entity listesini siliyoruz.
            
            if (save)
                Save();
		}

        // DbSet'lerdeki tüm değişikliklerden sonra oluşturulacak sorguların (insert, update ve delete) tek seferde veritabanında çalıştırılması: Unit of Work
        // SaveChanges methodu ile sorgunun çalıştırılması sonucunda etkilenen kayıt sayısı dönülebilir.
        public virtual int Save()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                // eğer istenirse buraya loglama kodları yazılarak hata alındığında örneğin exc.Message üzerinden logların
                // veritabanında, dosyada veya Windows Event Log'da tutulması sağlanabilir.

                throw exc; // hatayı SaveChanges methodunu çağırdığımız methoda fırlatıyoruz.
            }
        }

        public void Dispose()
        {
            _dbContext?.Dispose(); // ?: DbContext null ise bu satırı atla, değilse Dispose et.
            GC.SuppressFinalize(this); // Garbage Collector'a işimizin bittiğini söylüyoruz ki objeyi en kısa sürede hafızadan temizlesin.
        }
    }
}
using AppCore.Results;
using AppCore.Results.Bases;
using Business.Models;
using DataAccess.Entities;
using DataAccess.Repositories;

namespace Business.Services
{
    public interface IStoreService 
    {
        Result Add(StoreModel model); // Create işlemleri
        Result Update(StoreModel model); // Update işlemleri
        Result Delete(int id); // Delete işlemleri
        StoreModel? Edit(int id);
        List<StoreModel> GetAll();
        StoreModel? Details(int id);

    }

    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepo;

        public StoreService(IStoreRepository storeRepo)
        {
            _storeRepo = storeRepo;
        }

        public Result Add(StoreModel model)
        {
            // bir mağaza adı üzerinden hem sanal hem de sanal olmayan mağaza oluşturulabilmesini sağlayabilmek
            // ve bir mağazanın hem adı hem de sanal mı verisi üzerinden tek bir kaydının olabilmesi için
            // aşağıda Store Name ve IsVirtual üzerinden validasyon yapıyoruz

            var store = _storeRepo.Get(s => s.Name.ToLower() == model.Name.ToLower().Trim());

            if(store is not null && store.IsVirtual == model.IsVirtual) 
            {
                return new ErrorResult("Store can't be added because store with the same name exists!");
            }

            var entity = new Store()
            {
                IsVirtual = model.IsVirtual,
                Name = model.Name.Trim()
            };
            _storeRepo.Add(entity);
            return new SuccessResult();
        }

        public Result Delete(int id)
        {
            var store = _storeRepo.Include(new List<string> { "ProductStores" }).Get(s => s.Id == id); 
            if (store.ProductStores.Count > 0)
                return new ErrorResult("Store can't be deleted because store has products!");
            _storeRepo.Delete(s => s.Id == id);
            return new SuccessResult();
        }
       
        private StoreModel ToStoreModel(Store s)
        {
            return new StoreModel()
            {
                Guid = s.Guid,
                Id = s.Id,
                Name = s.Name,
                IsVirtual = s.IsVirtual,
                VirtualDisplay = s.IsVirtual ? "Yes" : "No"
            };
        }

        public Result Update(StoreModel model)
        {
            var store = _storeRepo.Get(s => s.Name.ToLower() == model.Name.ToLower().Trim());

            if (store is not null && store.IsVirtual == model.IsVirtual && store.Id != model.Id)
            {
                return new ErrorResult("Store can't be updated because store with the same name exists!");

            }

            var entity = new Store()
            {
                Id = model.Id,
                IsVirtual = model.IsVirtual,
                Name = model.Name.Trim()
            };
            _storeRepo.Update(entity);
            return new SuccessResult();
        }

        public List<StoreModel> GetAll()
        {
            return _storeRepo.OrderBy(s => s.IsVirtual).ThenBy(s => s.Name).GetList().Select(ToStoreModel).ToList();
        }

        public StoreModel? Edit(int id)
        {
            var store = _storeRepo.Get(s => s.Id == id); 
            if (store == null)
            {
                return null;
            }
            return ToStoreModel(store);
        }

        public StoreModel? Details(int id)
        {
            var store = _storeRepo.Get(s => s.Id == id);
            if (store == null)
            {
                return null;
            }
            return ToStoreModel(store);
        }
    }
}

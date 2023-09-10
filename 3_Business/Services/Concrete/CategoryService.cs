using Business.Results;
using Business.Models;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstract;
using Business.Results;
using Services.Abstract;

namespace Services.Concrete
{

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        /*
        public IQueryable<CategoryModel> Query()
        {
            // Query methodu ile sorguyu oluşturup OrderBy LINQ methodu ile kategori adlarına göre artan sıralıyoruz (azalan sıra için OrderByDescending kullanılır).
            return _categoryRepo.Query().OrderBy(c => c.Name).Select(c => new CategoryModel()
            {
                Description = c.Description,
                Guid = c.Guid,
                Id = c.Id,
                Name = c.Name,
                ProductsCount = c.Products.Count
            });
        }
        */
        private CategoryModel ToCategoryModel(Category c)
        {
            return new CategoryModel()
            {
                Description = c.Description,
                Id = c.Id,
                Name = c.Name,
                ProductsCount = c.Products.Count
            };
        }

        public Result Add(CategoryModel model)
        {
            var category = _categoryRepo.Get(c => c.Name.ToLower() == model.Name.ToLower().Trim());
            if (category is not null)
                return new ErrorResult("Category can't be added because category with the same name exists!");
            var entity = new Category()
            {
                Description = model.Description?.Trim(), // Description'ın null gelebilme ihtimali için sonunda ? kullanıyoruz
                Name = model.Name.Trim() // Name modelde zorunlu olduğundan null gelebilme ihtimali yok
            };
            _categoryRepo.Add(entity);
            return new SuccessResult(); // mesaj kullanmadan bir SuccessResult objesi oluşturduk ve döndük
        }

        public Result Update(CategoryModel model)
        {
            var category = _categoryRepo.Get(c => c.Name.ToLower() == model.Name.ToLower().Trim() && c.Id != model.Id);

            if (category is not null)
                return new ErrorResult("Category can't be updated because category with the same name exists!");
            var entity = new Category()
            {
                Id = model.Id, // güncelleme işlemi için mutlaka Id set edilmeli
                Description = model.Description?.Trim(), // Description'ın null gelebilme ihtimali için sonunda ? kullanıyoruz
                Name = model.Name.Trim() // Name modelde zorunlu olduğundan null gelebilme ihtimali yok
            };
            _categoryRepo.Update(entity);
            return new SuccessResult(); // mesaj kullanmadan bir SuccessResult objesi oluşturduk ve döndük
        }

        public Result Delete(int id)
        {
            var category = _categoryRepo.Include(new List<string> { "Products" }).Get(c => c.Id == id); // yukarıda daha önce oluşturduğumuz Query methodu üzerinden id ile kategoriyi çekiyoruz
            if (category.Products.Count > 0) // eğer çektiğimiz kategorinin ürünleri varsa veritabanındaki ürün ve kategori tabloları arasındaki ilişki Entity Framework
                                             // code first yaklaşımında otomatik cascade olarak oluşturulduğundan silinen kategorinin tüm ürünleri silinecektir,
                                             // bunu engellemek için yukarıdaki Query methodunda Products'ı da sorguya dahil etmiş ve bunun sonucunda
                                             // her bir kategori için bir ProductsCount ataması yapmıştık, dolayısıyla bu özellik üzerinden kontrol ederek
                                             // ilişkili ürün kayıtları varsa silme işlemine izin vermiyoruz.
                return new ErrorResult("Category can't be deleted because category has products!");
            _categoryRepo.Delete(c => c.Id == id);
            return new SuccessResult("Category deleted successfully.");
        }


        public async Task<List<CategoryModel>> GetListAsync() // bu method tanımı mutlaka ICategoryService içerisinde de yapılmalıdır ki
                                                              // controller'da çağrılabilsin.
        {
            var categories = await _categoryRepo.OrderBy(x => x.Name).GetListAsync(); //await: asynchronous wait yani asenkron bekleme anlamına gelir.
                                                                                      // await ile Async method sonucunda ulaşılan task tamamlanıp
                                                                                      // içerisindeki veriye (List<CategoryModel> tipinde) ulaşılır.
                                                                                      // eğer bir methodda Async bir method await ile kullanılıyorsa methodun
                                                                                      // dönüş tipinin başına async yazılmalı ve dönüş tipi de bir Task<> içerisine alınmalıdır.

            return categories.Select(ToCategoryModel).ToList();
        }

        public List<CategoryModel> GetList()
        {
            var categories = _categoryRepo.OrderBy(x => x.Name).
                Include(new List<string> { "Products" }).
                GetList().
                Select(ToCategoryModel).ToList();
            return categories;
        }

        public CategoryModel? Get(int id)
        {
            var category = _categoryRepo.Include(new List<string> { "Products" }).Get(x => x.Id == id);

            if (category is null)
            {
                return null;
            }

            return ToCategoryModel(category);
        }




    }
}

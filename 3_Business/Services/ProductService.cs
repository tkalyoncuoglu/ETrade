using AppCore.Results;
using AppCore.Results.Bases;
using Business.Models;
using DataAccess.Entities;
using DataAccess.Repositories;

namespace Business.Services
{
    public interface IProductService   // IProductService ProductModel tipi üzerinden IService'i implemente eden ve methodlarında
                                                              // ProductModel <-> Product dönüşümlerini yaparak Product tipindeki Repo üzerinden
                                                              // CRUD işlemleri için oluşturulan bir interface'tir.
    {
        Result Add(ProductModel model); // Create işlemleri
        Result Update(ProductModel model); // Update işlemleri
        Result Delete(int id); // Delete işlemleri
        Result DeleteImage(int id); // entity Image ve ImageExtension özelliklerini null olarak güncelleyerek ürün imajını siler
        public List<ProductModel> Get();
        public ProductModel? Edit(int id);
        public ProductModel? Details(int id);
    }

    public class ProductService : IProductService // ProductService IProductService'i implemente eden ve MVC projesindeki Program.cs IoC Container'ında
                                                  // bağımlılığı IProductService ile yönetilecek ve bu sayede ilgili controller'lara constructor üzerinden
                                                  // new'lenerek enjekte edilerek kullanılacak concrete (somut) bir class'tır.
    {

        private readonly IProductRepository _productRepository;

        private readonly IProductStoreRepository _productStoreRepository;

        public ProductService(IProductRepository productRepository, IProductStoreRepository productStoreRepository)
        {
            _productRepository = productRepository;
            _productStoreRepository = productStoreRepository; 
        }


        public List<ProductModel> Get() 
        {
			var products = _productRepository.
                OrderBy(product => product.Name).
                Include(new List<string> { "Category", "ProductStores.Store" }).
                GetList().
                Select(ToProductModel).ToList(); 
        
            return products;
        }

        public Result Add(ProductModel model)
        {
            Product? product = _productRepository.Get(p => p.Name.ToLower() == model.Name.ToLower().Trim()); 

            if (product != null) 
                return new ErrorResult("Product with the same name exists!");

            if (model.ExpirationDate.HasValue && model.ExpirationDate.Value < DateTime.Today)
                return new ErrorResult("Product can't be added because expiration date is earlier than today (" + DateTime.Today.ToString("MM/dd/yyyy"));

            Product entity = ToProduct(model);

            _productRepository.Add(entity);

            return new SuccessResult("Product added successfully."); 
        }

        public Result Update(ProductModel model) // Update işlemi: model kullanıcının view üzerinden doldurup gönderdiği objedir 
		{
            var product = _productRepository.Get(p => p.Name.ToLower() == model.Name.ToLower().Trim());
			if (product != null && product.Id != model.Id)
				return new ErrorResult("Product can't be updated because product with the same name exists!");
                
            _productStoreRepository.Delete(ps => ps.ProductId == model.Id); 

            Product? entity = _productRepository.Include(new List<string> { "ProductStores" }).Get(p => p.Id == model.Id);
			
            entity.CategoryId = model.CategoryId;

            entity.Description = model.Description?.Trim(); // Description verisinin null gelme ihtimali olduğundan sonuna ? ekliyoruz ki null geldiğinde Trim methodunu çalıştırmasın
                                                            // ve entity Description özelliğine null atasın, null gelmediğinde de gelen değeri entity Description özelliğine
                                                            // trim'leyerek atasın.

            entity.ExpirationDate = model.ExpirationDate;

            entity.Name = model.Name.Trim(); // Name ProductModel'de zorunlu olarak tanımlandığından direkt olarak değerini entity Name özelliğine atayabiliriz.

            entity.StockAmount = model.StockAmount.Value; // StockAmount ProductModel'de zorunlu olarak tanımlandığından direkt olarak Value ile değerine ulaşıp entity StockAmount
                                                          // özelliğine atayabiliriz.

            entity.UnitPrice = model.UnitPrice.Value; // UnitPrice ProductModel'de zorunlu olarak tanımlandığından direkt olarak Value ile değerine ulaşıp entity UnitPrice
                                                      // özelliğine atayabiliriz.


            entity.ProductStores = model.StoreIds.Select(sId => new ProductStore()
            {
                StoreId = sId
            }).ToList();


            if (model.Image != null && model.Image.Length > 0) // eğer modeldeki imaj binary verisi doluysa
            { 
                entity.Image = model.Image; // modeldeki Image binary verisini entity'deki Image özelliğine atıyoruz

				entity.ImageExtension = model.ImageExtension.ToLower(); // modeldeki ImageExtension verisini entity'deki ImageExtension özelliğine küçük harf yaparak atıyoruz ki
                                                                        // küçük harfler üzerinden dosya uzantısını saklayacağız,
                                                                        // Image null olmadığı kontrolünü yaptığımızdan ImageExtension null gelmeyecek, ImageExtension'ın sağında ? kullanmadık
			}
            // bu satırda eğer modeldeki imaj binary verisi boşsa ürünün mevcut imajını imajla ilgili özellikleri güncellemeden koruyoruz.

			_productRepository.Update(entity); 

			return new SuccessResult("Product updated successfully.");
		}

		public Result Delete(int id) // Delete işlemi: Genelde id üzerinden yapılır
        {
           
            _productStoreRepository.Delete(ps => ps.ProductId == id); // önce ürünün ilişkili ürün mağaza kayıtlarını repository üzerinden siliyoruz

            _productRepository.Delete(p => p.Id == id); // daha sonra ürün repository'sinde koşul (predicate) parametresi kullanan Delete methodunu
                                                  // Lambda Expression parametresi ile çağırıp ürünü siliyoruz ve hem ürün mağaza hem de ürün
                                                  // silmelerini save parametresi üzerinden tek seferde veritabanına yansıtıyoruz
            
            return new SuccessResult("Product deleted successfully."); // bu satırda silme işlemi başarıyla bittiğinden SuccessResult objesini mesajıyla beraber dönüyoruz ki
                                                                       // ilgili controller action'ında kullanabilelim.
        }

       

		public Result DeleteImage(int id) // entity Image ve ImageExtension özelliklerini null olarak güncelleyerek ürün imajını siler
		{
			Product? entity = _productRepository.Get(p => p.Id == id); // parametre olarak gelen id'ye göre tek ürünü çektik

            if (entity?.Image is null) // eğer ürünün imajı null ise
                return new ErrorResult("Product has no image to delete!"); // silinecek ürün imajı olmadığı mesajını ErrorResult objesi üzerinden dönüyoruz

            entity.Image = null; // Image özelliğine null atadık
            entity.ImageExtension = null; // ImageExtension özelliğine null atadık

            _productRepository.Update(entity); // veritabanında ürün tabolsunda entity'i güncelledik

            return new SuccessResult("Product image deleted successfully."); // başarılı işlem sonucunu döndük
		}

        public ProductModel? Edit(int id)
        {
            var product = _productRepository.Include(new List<string> { "Category", "ProductStores.Store" }).Get(x => x.Id == id);

            if(product is null)
            {
                return null;
            }

            return ToProductModel(product);
        }

        public ProductModel? Details(int id)
        {
            Product? product = _productRepository.Include(new List<string> { "Category", "ProductStores.Store" }).Get(p => p.Id == id);

            if (product == null) 
            { 
                return null;
            }

            return ToProductModel(product);
            
        }

        private ProductModel ToProductModel(Product product)
        {
            return new ProductModel()
            {
                // Entity özelliklerinin modeldeki karşılıklarının atanması (mapping işlemi), istenirse mapping işlemleri için AutoMapper kütüphanesi kullanılabilir.
                CategoryId = product.CategoryId,
                Description = product.Description,
                ExpirationDate = product.ExpirationDate,
                Guid = product.Guid,
                Id = product.Id,
                Name = product.Name,
                StockAmount = product.StockAmount,
                UnitPrice = product.UnitPrice,

                // View'da kullanıcıya gösterilecek özelliklerin (Display ile biten) atanması (mapping işlemi).
                UnitPriceDisplay = product.UnitPrice.ToString("C2"), // C: Currency (para birimi), N: Number (sayı) formatlama için kullanılır.
                                                                     // 2: ondalıktan sonra kaç hane olacağını belirtir.
                                                                     // Bölgesel ayarı MVC katmanında Program.cs'de yönettiğimiz için burada CultureInfo objesini kullanmaya gerek yoktur.

                //ExpirationDateDisplay = product.ExpirationDate != null ? product.ExpirationDate.Value.ToString("yyyy/MM/dd") : "", // nullable özellikler için 1. yöntem 
                ExpirationDateDisplay = product.ExpirationDate.HasValue ? product.ExpirationDate.Value.ToString("yyyy/MM/dd") : "", // nullable özellikler için 2. yöntem
                // Sıralama view'da kullanacağımız Javascript - CSS kütüphanesinde düzgün çalışsın diye yıl/ay/gün formatını kullandık.

                CategoryNameDisplay = product.Category.Name, // Ürünün ilişkili kategori referansı üzerinden adına ulaşıp özelliğe atadık.

                StoreNamesDisplay = string.Join("<br />", product.ProductStores.Select(productStore => productStore.Store.Name)),
                // ürünün mağaza adlarını string Join methodu ile <br /> (alt satır HTML tag'i) ayracı üzerinden tek bir string olarak birleştirir,
                // Select methodundaki Lambda Expression ile ürünün her bir ilişkili ürün mağazası üzerinden ilişkili mağazasına ulaşıp adlarını
                // string tipinde bir kolleksiyon olarak çektik

                StoreIds = product.ProductStores.Select(productStore => productStore.StoreId).ToList(),
                // ürün ekleme ve güncelleme işlemlerinde doldurulan tüm mağaza listesinde kullanıcının daha önce kaydetmiş olduğu mağazaları id'leri üzerinden çekiyoruz,
                // Select methodundaki Lambda Expression ile ürünün her bir ilişkili ürün mağazası üzerinden mağaza id'lerine ulaşıp int listesi tipinde
                // bir kolleksiyon olarak çektik

                Image = product.Image, // product entity'sindeki binary (byte[]) imaj verisini model'deki Image'a atıyoruz 

                ImageExtension = product.ImageExtension, // product entity'sindeki imaj uzantısını model'deki ImageExtension'a atıyoruz

                ImgSrcDisplay = product.Image != null ? // product entity'sinde Image null değilse
                   (
                       product.ImageExtension == ".jpg" || product.ImageExtension == ".jpeg" ? // product entity'sinde imajın dosya uzantısı (ImageExtension) .jpg veya .jpeg ise
                       "data:image/jpeg;base64," // içerik tipi (content type) olarak image/jpeg dönüyoruz
                       : "data:image/png;base64" // ImageExtension .png ise içerik tipi olarak image/png dönüyoruz
                   ) + Convert.ToBase64String(product.Image) // Image binary verisini base 64 üzerinden string'e dönüştürüp içerik tipinin sonuna ekliyoruz
                   : null // Image null ise null dönüyoruz
                // eğer başka dosya uzantıları da kullanılmak istenirse (örneğin .bmp) yukarıdaki ternary operator'a koşulu eklenmelidir
            };
        }

        private Product ToProduct(ProductModel model)
        {
            return new Product()
            {
                // 1. yöntem:
                //CategoryId = model.CategoryId ?? 0, // entity CategoryId özelliğine eğer modelin CategoryId'si null gelirse 0, dolu gelirse gelen değeri ata,
                // bu yöntemde dikkat edilmesi gereken veritabanındaki tabloya insert işlemi yapıldığında CategoryId'nin 0 atanması durumunda
                // kategori tablosunda 0 Id'li bir kategori bulunmadığından exception alınacağıdır.
                // 2. yöntem:
                CategoryId = model.CategoryId,

                // 1. yöntem: 
                //Description = model.Description == null ? null : model.Description.Trim(), // Description verisinin null gelme ihtimali olduğundan ternary operator kullanarak
                // null gelirse entity Description özelliğine null atanmasını, null gelmezse ise entity Description özelliğine değerinin trim'lenerek atanmasını sağlıyoruz.
                // 2. yöntem:
                Description = model.Description?.Trim(), // Description verisinin null gelme ihtimali olduğundan sonuna ? ekliyoruz ki null geldiğinde Trim methodunu çalıştırmasın
                                                         // ve entity Description özelliğine null atasın, null gelmediğinde de gelen değeri entity Description özelliğine
                                                         // trim'leyerek atasın.

                ExpirationDate = model.ExpirationDate,

                Name = model.Name.Trim(), // Name ProductModel'de zorunlu olarak tanımlandığından direkt olarak değerini entity Name özelliğine atayabiliriz.

                // 1. yöntem:
                //StockAmount = model.StockAmount ?? 0, // entity StockAmount özelliğine eğer modelin StockAmount'u null gelirse 0, dolu gelirse gelen değeri ata
                // 2. yöntem:
                StockAmount = model.StockAmount.Value, // StockAmount ProductModel'de zorunlu olarak tanımlandığından direkt olarak Value ile değerine ulaşıp entity StockAmount
                                                       // özelliğine atayabiliriz.

                // 1. yöntem:
                //UnitPrice = model.UnitPrice ?? 0, // entity UnitPrice özelliğine eğer modelin UnitPrice'ı null gelirse 0, dolu gelirse gelen değeri ata
                // 2. yöntem:
                UnitPrice = model.UnitPrice.Value, // UnitPrice ProductModel'de zorunlu olarak tanımlandığından direkt olarak Value ile değerine ulaşıp entity UnitPrice
                                                   // özelliğine atayabiliriz.

                ProductStores = model.StoreIds?.Select(sId => new ProductStore()
                {
                    StoreId = sId
                }).ToList(), // ürün mağaza ilişkisi için kullanıcı tarafından model üzerinden liste olarak gönderilen her bir sId (store id) delegesi için
                             // ürünle ilişkili bir ProductStore oluşturup StoreId özelliğini delege üzerinden set ediyoruz,
                             // kullanıcının mağaza seçmemesi durumunda StoreIds null geleceği için sonuna ? ekledik

                Image = model.Image, // modeldeki Image binary verisini entity'deki Image özelliğine atıyoruz

                ImageExtension = model.ImageExtension?.ToLower() // modeldeki null olabilecek (bu yüzden ? kullandık) ImageExtension verisini entity'deki ImageExtension özelliğine
                                                                 // atıyoruz ve veriyi null değilse küçük harf yapıyoruz ki küçük harfler üzerinden dosya uzantısını saklayacağız
            };


        }
    }
}

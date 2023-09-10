﻿using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MvcWebUI.Areas.Database.Controllers
{
    // Area'lar bir MVC projesindeki küçük MVC modülleri olarak düşünülebilir. Mutlaka oluşturulmaları şart değildir. Oluşturulduğunda projenin daha kolay yönetilmesini sağlar.
    // Herhangi bir area'nın controller'ında mutlaka aşağıdaki Area attribute'u tanımlanmış olmalıdır ve Program.cs içerisinde default MVC route tanımı (controller/action/id?)
    // yapılan yerin üzerine area scaffolding (otomatik yapıyı kodlar veya klasörler üzerinden oluşturma) tarafından projenin içerisine eklenen ScaffoldingReadMe.txt
    // dosyasındaki area'lar için route tanım kod bloğu eklenmelidir.

    [Area("Database")]
    public class HomeController : Controller
    {
        private readonly ETradeContext _db; // bu controller'ı kendimize development ortamında ilk verileri oluşturması bakımından kolaylık sağlaması
                                            // için oluşturduğumuzdan direkt DbContext objesini constructor üzerinden enjekte edip Index action'ında kullanıyoruz.

        public HomeController(ETradeContext db)
        {
            _db = db;
        }

        public IActionResult Index() // tarayıcıda ~/Database/Home/Index, ~/Database/Home veya ~/Database adresi girilerek veya herhangi bir view'da
                                     // (örneğin Views -> Shared klasörü altındaki _Layout.cshtml) bu adres için area da dikkate alınarak link oluşturularak çağrılabilir.
        {
            #region Mevcut verilerin silinmesi

            var productStores = _db.ProductStores.ToList(); // önce ürün mağaza listesini çekip daha sonra ürün mağaza DbSet'i üzerinden RemoveRange methodu ile silinmesini sağlıyoruz
            _db.ProductStores.RemoveRange(productStores);

            var stores = _db.Stores.ToList();
            _db.Stores.RemoveRange(stores);

            var products = _db.Products.ToList(); 
            _db.Products.RemoveRange(products);

            var categories = _db.Categories.ToList();
            _db.Categories.RemoveRange(categories);

			var userDetials = _db.UserDetails.ToList();
			_db.UserDetails.RemoveRange(userDetials);

			var users = _db.Users.ToList();
            _db.Users.RemoveRange(users);

			
			#endregion



			#region İlk verilerin oluşturulması
			_db.Stores.Add(new Store()
            {
                Name = "Hepsiburada",
                IsVirtual = true
            });
            _db.Stores.Add(new Store()
            {
                Name = "Vatan",
                IsVirtual = false
            });
            _db.SaveChanges(); // mağazaları veritabanına kaydediyoruz ki aşağıda oluşturacağımız ürünler için mağaza adı üzerinden istediğimiz mağaza kayıtlarına ulaşıp
                               // mağaza id'lerini ürün mağaza ilişki entity'si üzerinden tablosunda doldurabilelim

            _db.Categories.Add(new Category()
            {
                Name = "Computer",
                Description = "Laptops, desktops and computer peripherals",
                Products = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Laptop",
                        UnitPrice = 3000.5,
                        ExpirationDate = new DateTime(2032, 1, 27),
                        StockAmount = 10,
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Mouse",
                        UnitPrice = 20.5,
                        StockAmount = 50,
                        Description = "Computer peripheral",
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Keyboard",
                        UnitPrice = 40,
                        StockAmount = 45,
                        Description = "Computer peripheral",
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Monitor",
                        UnitPrice = 2500,
                        ExpirationDate = DateTime.Parse("05/19/2027"),
                        StockAmount = 20,
                        Description = "Computer peripheral",
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    }
                }
            });

            _db.Categories.Add(new Category()
            {
                Name = "Home Theater System",
                Products = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Speaker",
                        UnitPrice = 2500,
                        StockAmount = 70
                    },
                    new Product()
                    {
                        Name = "Receiver",
                        UnitPrice = 5000,
                        StockAmount = 30,
                        Description = "Home theater system component",
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Equalizer",
                        UnitPrice = 1000,
                        StockAmount = 40,
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    }
                }
            });

			_db.SaveChanges();

			
            
            #endregion



            #region DbSet'ler üzerinden yapılan değişikliklerin tek seferde veritabanına yansıtılması (Unit of Work)
            _db.SaveChanges();
            #endregion



            //return Content("Database seed successful."); // *1
            //return Content("<label style=\"color:red;\"><b>Database seed successful.</b></label>", "text/html"); // *2
            return Content("<label style=\"color:red;\"><b>Database seed successful.</b></label>", "text/html", Encoding.UTF8); // *3
            // Yeni bir view oluşturup işlem sonucunu göstermek yerine Controller base class'ının Content methodunu kullanıp işlem sonucunun boş bir sayfada gösterilmesini sağladık.
            // *1: Metinsel verinin düz yazı (plain text) olarak sayfada gösterilmesini sağlar.
            // *2: HTML verisinin sayfada HTML içerik olarak gösterilmesini sağlar.
            // *3: HTML verisinin sayfada Türkçe karakterleri destekleyen UTF8 karakter kümesi üzerinden HTML içerik olarak gösterilmesini sağlar.
        }
    }
}

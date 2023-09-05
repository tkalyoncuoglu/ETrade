#nullable disable
using Business.Services.Abstract;
using DataAccess.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MvcWebUI.Settings;
using Repositories.Abstract;
using Repositories.Concrete;
using Services.Abstract;
using Services.Concrete;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

#region Localization
// Web uygulamas�n�n b�lgesel ayar� a�a��daki �ekilde tek seferde konfig�re edilerek t�m projenin bu ayar� kullanmas� sa�lanabilir,
// dolay�s�yla veri formatlama veya d�n��t�rme gibi i�lemlerde her seferinde CultureInfo objesinin kullan�m gereksinimi ortadan kalkar.
// Bu �ekilde sadece tek bir b�lgesel ayar projede kullan�labilir.
List<CultureInfo> cultures = new List<CultureInfo>()
{
    new CultureInfo("en-US") // e�er uygulama T�rk�e olacaksa CultureInfo constructor'�n�n parametresini ("tr-TR") yapmak yeterlidir.
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault().Name);
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});
#endregion

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) 
    // projeye Cookie authentication default'lar�n� kullanarak kimlik do�rulama ekliyoruz
	
    .AddCookie(config => 
    // olu�turulacak cookie'yi config action delegesi �zerinden konfig�re ediyoruz, action delegeleri func delegeleri gibi bir sonu� d�nmez,
    // �zerlerinden burada oldu�u gibi genelde konfig�rasyon i�lemleri yap�l�r
	
    {
		config.LoginPath = "/Account/Users/Login"; 
        // e�er sisteme giri� yap�lmadan bir i�lem yap�lmaya �al���l�rsa kullan�c�y� Account area -> Users controller -> Login action'�na y�nlendir
		
        config.AccessDeniedPath = "/Account/Users/AccessDenied"; 
        // e�er sisteme giri� yap�ld�ktan sonra yetki d��� bir i�lem yap�lmaya �al���l�rsa kullan�c�y� Account area -> Users controller -> AccessDenied
        // action'�na y�nlendir
		
        config.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
        // sisteme giri� yap�ld�ktan sonra olu�an cookie 30 dakika boyunca kullan�labilsin
		
        config.SlidingExpiration = true; 
        // SlidingExpiration true yap�larak kullan�c� sistemde her i�lem yapt���nda cookie kullan�m s�resi yukar�da belirtilen 30 dakika uzat�l�r,
        // e�er false atan�rsa kullan�c�n�n cookie �mr� yukar�da belirtilen 30 dakika sonra sona erer ve yeniden giri� yapmak zorunda kal�r
	});
#endregion

#region Session
builder.Services.AddSession(config => // projeye config action delegesi konfig�rasyonlar� �zerinden session ekliyoruz
{
    config.IdleTimeout = TimeSpan.FromMinutes(30); // kullan�c�n�n web uygulamas�n� kullanmad���nda session'�n temizlenece�i s�re (30 dakika)
                                                   // default'u 20 dakika
    config.IOTimeout = Timeout.InfiniteTimeSpan; // sadece IdleTimeout'un kullan�lmas� i�in IOTimeout de�erini sonsuz yap�yoruz
});
#endregion

#region IoC Container : Inversion of Control Container (Ba��ml�l�klar�n Y�netimi) 
// Alternatif olarak Business katman�nda Autofac ve Ninject gibi k�t�phaneler de kullan�labilir.

// Unable to resolve service hatalar� burada ��z�mlenmelidir.

// AddScoped: istek (request) boyunca objenin referans�n� (genelde interface veya abstract class) kulland���m�z yerde obje (somut class'tan olu�turulacak)
// bir kere olu�turulur ve yan�t (response) d�nene kadar bu obje hayatta kal�r.
// AddSingleton: web uygulamas� ba�lad���nda objenin referansn� (genelde interface veya abstract class) kulland���m�z yerde obje (somut class'tan olu�turulacak)
// bir kere olu�turulur ve uygulama �al��t��� (IIS �zerinden uygulama durdurulmad��� veya yeniden ba�lat�lmad���) s�rece bu obje hayatta kal�r.
// AddTransient: istek (request) ba��ms�z ihtiya� olan objenin referans�n� (genelde interface veya abstract class) kulland���m�z her yerde bu objeyi new'ler.
// Genelde AddScoped methodu kullan�l�r.

string connectionString = builder.Configuration.GetConnectionString("ETradeDb"); // appsettings.json veya appsettings.Development.json dosyalar�ndaki isim �zerinden atanan
                                                                                 // veritaban� ba�lant� string'ini d�ner.

builder.Services.AddDbContext<ETradeContext>(); // projede herhangi bir class'ta ETradeContext tipinde 
                                                                                                 // constructor injection yap�ld���nda ETradeContext objesini new'leyerek
                                                                                                 // o class'a enjekte eder.       



builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IProductStoreRepository, ProductStoreRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductService, ProductService>(); // projede herhangi bir class'ta IProductService tipinde constructor injection yap�ld���nda
                                                               // ProductService objesini new'leyerek o class'a enjekte eder.

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IReportService, ReportService>();
#endregion

#region AppSettings
// �nce appsettings.json veya iste�e g�re appsettings.Development.json dosyalar�ndaki AppSettings b�l�m� (section) al�n�r,
// elle string olarak AppSettings yazmak yerine AppSettings class'�n�n nameof ile ad�n� kullanmak hata riskini ortadan kald�r�r.
//var section = builder.Configuration.GetSection("AppSettings");
var section = builder.Configuration.GetSection(nameof(AppSettings)); 

// Sonra bu section'daki �zellikler ile new'lenen AppSettings tipindeki obje �zellikleri ba�lan�r yani obje �zellikleri doldurulur.
section.Bind(new AppSettings());
#endregion

var app = builder.Build();

#region Localization
app.UseRequestLocalization(new RequestLocalizationOptions()
{
    DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault().Name),
    SupportedCultures = cultures,
    SupportedUICultures = cultures,
});
#endregion

// E�er istenirse AppCore -> App -> Environment class'� alt�ndaki IsDevelopment �zelli�i burada atanarak �rne�in Views -> Shared -> _Layout.cshtml
// view'�nda sadece development ortam� i�in Seed Database link'inin gelmesi sa�lanabilir.
AppCore.App.Environment.IsDevelopment = app.Environment.IsDevelopment();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

#region Authentication
app.UseAuthentication(); // sen kimsin?
#endregion

app.UseAuthorization(); // sen i�lem i�in yetkili misin?

#region Session
app.UseSession();
#endregion

#region Area
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});
#endregion

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // MVC default route tan�m�

app.Run();

// İstenirse direkt Program.cs altına kodlar yazılabilir.
// Bu kodlar aslında Program class'ının Main methodu içerisinde çalışır,
// ancak bu şekilde Program class'ını ve Main methodunu oluşturmaya gerek yoktur.
#region Bağımlılıkların Yönetimi (Inversion of Control - IoC)
MyRepoBase myRepo = new MyRepo();
IMyService myService = new MyService(myRepo);
MyController myController = new MyController(myService);
myController.Index();
#endregion



#region DataAccess Katmanı
public abstract class MyRepoBase
{ 
    public void Query()
    {
        Console.WriteLine("Entity sorgusu DbSet üzerinden oluşturuldu.");
    }
}

public class MyRepo : MyRepoBase
{
}
#endregion



#region Business Katmanı
public interface IMyService
{
    void Query();
}

public class MyService : IMyService
{
    #region Constructor Dependency Injection
    private readonly MyRepoBase _myRepo;

    public MyService(MyRepoBase myRepo)
    {
        _myRepo = myRepo;
    }
    #endregion

    public void Query()
    {
        _myRepo.Query();
        Console.WriteLine("Model sorgusu repository üzerinden dönen entity sorgusu üzerinden oluşturuldu.");
    }
}
#endregion



#region Application (MvcWebUI) veya Restful Servis (WebApi) Katmanları
public class MyController
{
    #region Constructor Dependency Injection
    private readonly IMyService _myService;

    public MyController(IMyService myService)
    {
        _myService = myService;
    }
    #endregion

    public void Index()
    {
        _myService.Query();
        Console.WriteLine("Service üzerinden dönen model sorgusu listeye dönüştürülüp kullanıcıya sunuldu.");
    }
}
#endregion
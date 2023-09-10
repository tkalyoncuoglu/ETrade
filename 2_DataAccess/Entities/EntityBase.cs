namespace Business.Records.Bases
{
    public abstract class EntityBase // ilişki entity'leri dışında tüm entity'lerin ve model'lerin miras alacağı ve veritabanındaki entity'lerin karşılığı olan tablolarda sütunları oluşacak özellikler
    {
        public int Id { get; set; }
    }
}

namespace EWS.Domain.Base;

public class DbStoreSchemaBase<T>
    where T : IDbStoreBase
{
    //auto increment
    //public int Id { get; set; }

    public string Name { get; set; }

    public T Data { get; set; }
        
    public DateTime Expired { get; set; }
}
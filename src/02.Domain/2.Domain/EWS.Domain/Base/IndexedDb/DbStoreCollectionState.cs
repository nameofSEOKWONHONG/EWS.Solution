namespace EWS.Domain.Base;

public class DbStoreState<T> : IDbStoreBase
{
    public T Data { get; set; }
}

public class DbStoreCollectionState<T> : IDbStoreBase
{
    public IEnumerable<T> Datum { get; set; }
}
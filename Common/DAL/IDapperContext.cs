namespace PracticeCrud1.Common.DAL
{
    public interface IDapperContext
    {
        Task<T> QueryFirst<T>(string procedureName, object parameters);
        Task<IEnumerable<T>> QueryAll<T>(string procedureName, object parameters);
        Task<T> QueryFirstOrDefault<T>(string v, object param);
    }
}

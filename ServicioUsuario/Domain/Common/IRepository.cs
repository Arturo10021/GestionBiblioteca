namespace ServicioUsuario.Domain.Common;

public interface IRepository<T, TId> where T : class
{
    T? GetById(TId id);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void SaveChanges();
}

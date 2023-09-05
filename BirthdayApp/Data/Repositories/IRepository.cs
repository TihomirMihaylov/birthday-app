namespace BirthdayApp.Data.Repositories
{
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        Task<List<TEntity>> AllAsync(CancellationToken cancellationToken);

        Task<List<TEntity>> AllAsNoTrackingAsync(CancellationToken cancellationToken);

        ValueTask<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] id);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;

namespace BirthdayApp.Data.Repositories
{
    public class EfRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        public EfRepository(ApplicationDbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            DbSet = Context.Set<TEntity>();
        }

        protected DbSet<TEntity> DbSet { get; set; }

        protected ApplicationDbContext Context { get; set; }

        public virtual Task<List<TEntity>> AllAsync(CancellationToken cancellationToken)
            => DbSet.ToListAsync(cancellationToken);


        public virtual Task<List<TEntity>> AllAsNoTrackingAsync(CancellationToken cancellationToken)
            => DbSet.AsNoTracking().ToListAsync(cancellationToken);

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
            => await DbSet.AddAsync(entity, cancellationToken);

        public virtual ValueTask<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] id) => DbSet.FindAsync(id, cancellationToken);

        public virtual void Update(TEntity entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => Context.SaveChangesAsync(cancellationToken);

        public void Dispose()
        {
            Context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

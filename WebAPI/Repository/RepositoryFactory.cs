using WebAPI.Models;

namespace WebAPI.Repository
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public RepositoryFactory(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            if (!_repositories.ContainsKey(typeof(T)))
            {
                var repository = new Repository<T>(_context);
                _repositories.Add(typeof(T), repository);
            }
            return (IRepository<T>)_repositories[typeof(T)];
        }
    }
}

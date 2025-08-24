
namespace InterViewProject.Serverice
{
    public class ProductListRepositoryService : IProductListRepository
    {
        private readonly CoffeeContext _dbContext;
        public ProductListRepositoryService(CoffeeContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ProductListView> GetAllProducts()
        {
            return _dbContext.ProductListViews.AsNoTracking().Where(product => product.TakeDown != true);
        }
    }
}


using InterViewProject.Controllers;
using InterViewProject.Models.ViewModel;

namespace InterViewProject.Serverice
{
    public class ProductRepositoryService : IProductRepository
    {
        private readonly CoffeeContext _dbContext;
        private readonly ILogger<ProductRepositoryService> _logger;
        public ProductRepositoryService(CoffeeContext dbContext, ILogger<ProductRepositoryService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ProductViewModel> GetProduct(int productid)
        {
            var target = await _dbContext.ProductListViews.FirstOrDefaultAsync(product=> product.ProductId == productid);
            if (target == null || target.TakeDown == true)
            {
                _logger.LogError("Product with ID {ProductId} not found or has been taken down.", productid);
                throw new KeyNotFoundException($"Product with ID {productid} not found or has been taken down.");
            }

            return new ProductViewModel
            {
                ProductName = target.ProductName,
                CountryName = target.CountryName,
                ContinentName = target.ContinentName,
                ProcessName = target.ProcessName,
                RoastingName = target.RoastingName,
                CategoriesName = target.CategoriesName,
                Price = target.Price,
                MainPhotoPath = target.MainPhotoPath,
                Description = target.Description
            };
        }
    }
}

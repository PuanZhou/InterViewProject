using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InterViewProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductListRepository _productListRepository;
        public HomeController(ILogger<HomeController> logger, IProductListRepository productListRepository)
        {
            _logger = logger;
            _productListRepository = productListRepository;
        }

        public IActionResult Index()
        {
            IQueryable<ProductListView> products = _productListRepository.GetAllProducts();
            
            if(products is null || !products.Any())
            {
                _logger.LogWarning("No products found in the database.");
            }

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using InterViewProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace InterViewProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepository _productRepository;
        public ProductController(ILogger<ProductController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;

        }

        public async Task<IActionResult> Index()
        {
            ProductUpSertModel UpsertModel = await _productRepository.GetUpsertModel();
            return View(UpsertModel);
        }

        public async Task<IActionResult> EditList()
        {
            List<EditProductListViewModel> editList = await _productRepository.GetEditProducts();
            return View(editList);
        }

        public async Task<IActionResult> Edit(int ProductId)
        {
            var product = await _productRepository.GetUpsertModel(ProductId);
            return View(product);
        }

        public async Task<IActionResult> EditPost(ProductUpSertModel product, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                bool result = await _productRepository.EditProduct(product.productInsertModel, file);
                if (result)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogError("Failed to insert product into the database.");
                    return RedirectToAction("Index", "Home");
                }
            }

            ProductUpSertModel refreshedModel = await _productRepository.GetUpsertModel();
            refreshedModel.productInsertModel = product.productInsertModel;

            return View("Index", refreshedModel);
        }
        public async Task<IActionResult> Insert(ProductUpSertModel product, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                bool result = await _productRepository.InsertProduct(product.productInsertModel, file);
                if (result)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogError("Failed to insert product into the database.");
                    return RedirectToAction("Index", "Home");
                }
            }

            ProductUpSertModel refreshedModel = await _productRepository.GetUpsertModel();
            refreshedModel.productInsertModel = product.productInsertModel;

            return View("Index", refreshedModel);
        }
    }
}

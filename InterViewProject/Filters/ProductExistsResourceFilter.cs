using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ProductExistsResourceFilter : IAsyncResourceFilter
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductExistsResourceFilter> _logger;

    public ProductExistsResourceFilter(IProductRepository productRepository, ILogger<ProductExistsResourceFilter> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        string? idStr = null;
        idStr ??= context.HttpContext.Request.Query["productid"].FirstOrDefault();
        if (idStr != null && int.TryParse(idStr.ToString(), out int productId))
        {
            try
            {
                var product = await _productRepository.GetProduct(productId);
                await next();
            }
            catch
            {
                // 商品不存在，回傳 404
                _logger.LogWarning("Product with ID {ProductId} not found.", productId);
                context.Result = new NotFoundResult();
            }
        }
        else
        {
            // 沒有productid，直接回傳 400
            _logger.LogError("Product ID is missing or invalid in the route data.");
            context.Result = new BadRequestResult();
        }
    }
}
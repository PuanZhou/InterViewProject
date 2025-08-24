namespace InterViewProject.Serverice
{
    public class ProductRepositoryService : IProductRepository
    {
        private readonly CoffeeContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;//取得wwwroot的路徑服務 (依賴注入DI)
        private readonly ILogger<ProductRepositoryService> _logger;
        public ProductRepositoryService(CoffeeContext dbContext, ILogger<ProductRepositoryService> logger, IWebHostEnvironment hostEnvironment)
        {
            _dbContext = dbContext;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ProductViewModel> GetProduct(int productid)
        {
            var target = await _dbContext.ProductListViews.FirstOrDefaultAsync(product => product.ProductId == productid);
            if (target == null || target.TakeDown == true)
            {
                _logger.LogError("Product with ID {ProductId} not found or has been taken down.", productid);
                throw new InvalidOperationException($"Product with ID {productid} not found or has been taken down.");
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

        public async Task<ProductUpSertModel> GetUpsertModel()
        {
            ProductUpSertModel ProductUpSertModel = new ProductUpSertModel()
            {
                productInsertModel = new ProductInsertModel(),
                CountryList = _dbContext.Countries.Select(c => new SelectListItem
                {
                    Text = c.CountryName,
                    Value = c.CountryId.ToString()
                }),
                RoastingList = _dbContext.Roastings.Select(r => new SelectListItem
                {
                    Text = r.RoastingName,
                    Value = r.RoastingId.ToString()
                }),
                ProcessNameList = _dbContext.Processes.Select(p => new SelectListItem
                {
                    Text = p.ProcessName,
                    Value = p.ProcessId.ToString()
                }),
                CategoriesList = _dbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.CategoriesName,
                    Value = c.CategoryId.ToString()
                })
            };

            return await Task.FromResult(ProductUpSertModel);
        }

        public async Task<bool> InsertProduct(ProductInsertModel product, IFormFile? file)
        {
            try
            {
                if (file != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    // 確保目錄存在
                    Directory.CreateDirectory(uploads);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStreams);
                    }
                    product.MainPhotoPath = @"\images\products\" + fileName + extension;
                }

               
                Product newProduct = new Product()
                {
                    ProductName = product.ProductName,
                    CountryId = product.CountryID,
                    Stock = product.Stock,
                    CategoryId = product.CategoriesID,
                    Price = product.Price,
                    MainPhotoPath = product.MainPhotoPath,
                    Description = product.Description,
                    TakeDown = false
                };

                if ((CategoryName)product.CategoriesID == CategoryName.咖啡)
                {
                    newProduct.Coffee = new Coffee()
                    {
                        CoffeeName = product.ProductName,
                        CountryId = product.CountryID,
                        ProcessId = product.ProcessID,
                        RoastingId = product.RoastingID
                    };
                }

                await _dbContext.Products.AddAsync(newProduct);

                var result = await _dbContext.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "插入產品 '{ProductName}' 時發生錯誤", product.ProductName);
                return false; 
            }
        }
    }
}

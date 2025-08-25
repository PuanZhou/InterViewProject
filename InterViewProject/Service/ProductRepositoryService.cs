

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

        public async Task<ProductUpSertModel> GetUpsertModel(int? productId)
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

            if (productId.HasValue)
            {
                var productid = productId.Value;
                var target = await _dbContext.Products.FirstOrDefaultAsync(x=>x.ProductId == productid);
                if (target is null)
                {
                    _logger.LogError("找不到產品 ID 為 '{ProductID}' 的產品", productId);
                    throw new IndexOutOfRangeException();
                }
                ProductUpSertModel.productInsertModel.ProductID = target.ProductId;
                ProductUpSertModel.productInsertModel.ProductName = target.ProductName;
                ProductUpSertModel.productInsertModel.CountryID = target.CountryId.Value;
                ProductUpSertModel.productInsertModel.Stock = target.Stock.Value;
                ProductUpSertModel.productInsertModel.CategoriesID = target.CategoryId;
                ProductUpSertModel.productInsertModel.Price = target.Price.Value;
                ProductUpSertModel.productInsertModel.MainPhotoPath = target.MainPhotoPath;
                ProductUpSertModel.productInsertModel.Description = target.Description;
                ProductUpSertModel.productInsertModel.TakeDown = target.TakeDown;
            }
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

        public async Task<List<EditProductListViewModel>> GetEditProducts()
        {
            return await _dbContext.ProductListViews.AsNoTracking().Select(product => new EditProductListViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
            }).ToListAsync();
        }

        public async Task<bool> EditProduct(ProductInsertModel product, IFormFile? file)
        {
            try
            {
                var targetproduct = await _dbContext.Products.FindAsync(product.ProductID);

                if (targetproduct is null)
                {
                    _logger.LogError("找不到產品 ID 為 '{ProductID}' 的產品", product.ProductID);
                    throw new IndexOutOfRangeException();
                }

                if (file != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    // 確保目錄存在
                    Directory.CreateDirectory(uploads);
                    if (product.MainPhotoPath != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, targetproduct.MainPhotoPath.TrimStart('\\'));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStreams);
                    }
                    product.MainPhotoPath = @"\images\products\" + fileName + extension;
                }


                targetproduct.ProductName = product.ProductName;
                targetproduct.CountryId = product.CountryID;
                targetproduct.Stock = product.Stock;
                targetproduct.CategoryId = product.CategoriesID;
                targetproduct.Price = product.Price;
                targetproduct.MainPhotoPath = product.MainPhotoPath;
                targetproduct.Description = product.Description;
                targetproduct.TakeDown = product.TakeDown;

                if ((CategoryName)product.CategoriesID == CategoryName.咖啡)
                {
                    targetproduct.Coffee.CoffeeName = product.ProductName;
                    targetproduct.Coffee.CountryId = product.CountryID;
                    targetproduct.Coffee.ProcessId = product.ProcessID;
                    targetproduct.Coffee.RoastingId = product.RoastingID;
                }
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

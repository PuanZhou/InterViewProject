using InterViewProject.Models.ViewModel;

namespace InterViewProject.Serverice.IRepository
{
    public interface IProductRepository
    {
        Task<ProductViewModel> GetProduct(int productid);
        Task<ProductUpSertModel> GetUpsertModel(int? productId = null);
        Task<bool> InsertProduct(ProductInsertModel product, IFormFile? file);
        Task<bool> EditProduct(ProductInsertModel product, IFormFile? file);
        Task<List<EditProductListViewModel>> GetEditProducts();
    }
}

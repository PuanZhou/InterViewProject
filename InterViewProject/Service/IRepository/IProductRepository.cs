using InterViewProject.Models.ViewModel;

namespace InterViewProject.Serverice.IRepository
{
    public interface IProductRepository
    {
        Task<ProductViewModel> GetProduct(int productid);
    }
}

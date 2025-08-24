namespace InterViewProject.Serverice.IRepository
{
    public interface IProductListRepository
    {
        IQueryable<ProductListView> GetAllProducts();
    }
}

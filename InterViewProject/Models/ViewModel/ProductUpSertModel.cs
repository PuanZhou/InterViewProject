#nullable disable
namespace InterViewProject.Models.ViewModel
{
    public class ProductUpSertModel
    {
        public ProductInsertModel productInsertModel { get; set; }
        public IEnumerable<SelectListItem> CountryList { get; set; } //取得所有國家填入下拉選單
        public IEnumerable<SelectListItem> RoastingList { get; set; }//取得所有烘培度填入下拉選單
        public IEnumerable<SelectListItem> ProcessNameList { get; set; }//取得所有處理法填入下拉選單
        public IEnumerable<SelectListItem> CategoriesList { get; set; }//取得所有種類填入下拉選單
    }
}

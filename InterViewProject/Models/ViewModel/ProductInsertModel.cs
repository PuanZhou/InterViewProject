namespace InterViewProject.Models.ViewModel
{
    public class ProductInsertModel
    {
        [Required]
        public string? ProductName { get; set; }
        
        [Required]
        public int CountryID { get; set; }
        public int ProcessID { get; set; }

        public int RoastingID { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public int CategoriesID { get; set; }
       
        [Required]
        public decimal? Price { get; set; }

        public string? MainPhotoPath { get; set; }

        public string? Description { get; set; }
    }
}

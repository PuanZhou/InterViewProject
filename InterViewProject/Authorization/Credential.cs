namespace InterViewProject.Authorization
{
    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}

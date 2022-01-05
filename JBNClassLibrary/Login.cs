using System.ComponentModel.DataAnnotations;

namespace JBNClassLibrary
{
    public class Login
    {
        public int? UserID { get; set; }

        [Required]
        [Display(Name = "UserName")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string FullName { get; set; }
        public int? RoleID { get; set; }
    }
}
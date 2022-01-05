using System.ComponentModel.DataAnnotations;

namespace JBNClassLibrary
{
    public class UserStatus
    {
        public int CustID { get; set; }
        public int StatusType { get; set; }
        [Required(ErrorMessage = "Please enter comments")]
        public string Comments { get; set; }
    }
}
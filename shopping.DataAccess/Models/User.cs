using System.ComponentModel.DataAnnotations;

namespace Shopping.DataAccess.Models
{
    public class User
    {
        [Key]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "FirstName is Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Enter Characters only")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone is Required")]
        [RegularExpression(@"^(?:(?:\+|0{0,2})91(\s*[\-]\s*)?|[0]?)?[6789]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{6,}$", ErrorMessage = "Password must contain: Minimum 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase Alphabet, 1 Number and 1 Special Character")]
        public string Password { get; set; }

        public string[] Roles { get; set; }
    }
}

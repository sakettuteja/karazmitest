using System.ComponentModel.DataAnnotations;

namespace TestProject.WebAPI.Models
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Required integer value"), MaxLength(100, ErrorMessage = "User Name cannot be more that 100")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Text, ErrorMessage = "Required integer value"), MaxLength(15, ErrorMessage = "Password cannot be more that 15")]
        public string Password { get; set; }
    }
}

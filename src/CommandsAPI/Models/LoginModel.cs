using System.ComponentModel.DataAnnotations;
namespace CommandAPI.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string email { get; set; }
    }
}
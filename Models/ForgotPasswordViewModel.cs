using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FlowerSite.Models
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress, Display(Name = "Registered email address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
    }
}

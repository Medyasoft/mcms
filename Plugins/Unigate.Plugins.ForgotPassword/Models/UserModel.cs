using System;

namespace Mcms.Plugins.ForgotPassword.Models
{
    public class UserModel
    {
        public string Title { get; set; }
        public Guid ContentId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
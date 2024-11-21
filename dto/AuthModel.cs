using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend_devops_rejsekort_v2.dto
{
    public class LoginModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class GoogleLoginModel
    {
        public required string IdToken { get; set; }
    }

    public class DeleteAccountModel
    {
        public string Password { get; set; }
    }

}

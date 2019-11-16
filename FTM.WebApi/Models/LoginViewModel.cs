using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.WebApi.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        public bool IsLoginFail { get; set; }
    }
}

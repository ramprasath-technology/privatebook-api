using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateBookAPI.Data.DTO
{
    public class PasswordReset
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
        public string SecurityAnswer { get; set; }
    }
}

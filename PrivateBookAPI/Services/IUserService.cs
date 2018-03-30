using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateBookAPI.Services
{
    public interface IUserService
    {
        string HashPassword(string password);
        void VerifyUser(string password);
    }
}

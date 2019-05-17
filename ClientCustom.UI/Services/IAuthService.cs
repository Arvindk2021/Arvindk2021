using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace ClientCustom.UI.Services
{
    public interface IAuthService
    {
        string GenerateToken(IAuthModel model);
    }
}
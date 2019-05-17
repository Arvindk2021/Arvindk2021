using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace ClientCustom.UI.Services
{
    public interface IAuthModel
    {
        string SecretKey { get;}

        string SecurityAlgoritm { get;}

        int ExpireMinutes { get; }

        Claim[] Claims { get; set; }
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Users.Controllers
{
    public class ClaimsController : Controller
    {
        public ViewResult Index()
        {
            return View(this.User?.Claims);
        }
    }
}
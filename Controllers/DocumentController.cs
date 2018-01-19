using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Models;

namespace Users.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private ProtectedDocument[] _docs = new ProtectedDocument[] {
            new ProtectedDocument { Title = "Q3 Budget", Author = "yj", Editor = "yj2"},
            new ProtectedDocument { Title = "Project Plan", Author = "admin",
            Editor = "yj2"}
        };

        private IAuthorizationService _authService;

        public DocumentController(IAuthorizationService authService)
        {
            _authService = authService;
        }

        public ViewResult Index() => View(_docs);

        public async Task<IActionResult> Edit(string title) {
            var doc = _docs.FirstOrDefault(p => p.Title == title);
            var authResult = await _authService.AuthorizeAsync(this.User, doc, "AuthorsAndEditors");
            if (authResult.Succeeded)
            {
                return View("Index", doc);
            }
            else
            {
                return new ChallengeResult();
            }
        }
    }
}
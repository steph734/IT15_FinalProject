using Microsoft.AspNetCore.Mvc;
using RealEstate.Models;
using RealEstate.Services;
using System.Diagnostics;

namespace RealEstate.Controllers
{
    public class HomeController : Controller
    {
        private readonly PropertyCatalog _catalog;

        public HomeController(PropertyCatalog catalog)
        {
            _catalog = catalog;
        }

        public IActionResult Index()
        {
            var all = _catalog.GetProperties();
            var vm = new HomeIndexViewModel
            {
                FeaturedProperties = all.Take(4).ToList(),
                SearchResults = all.Take(8).ToList(),
                Promotions =
                [
                    new PromotionItem
                    {
                        Title = "Zero buyer fees this month",
                        Description = "List with EstateFlow and we waive the buyer advisory fee on select BGC and Makati listings.",
                        Badge = "Limited time",
                        ImageUrl =
                            "https://images.unsplash.com/photo-1600585154084-4e5fe7c39198?auto=format&fit=crop&w=900&q=80"
                    },
                    new PromotionItem
                    {
                        Title = "Pre-approved renters move faster",
                        Description = "Complete a rental application in-app and get priority showings in Quezon City and Ortigas.",
                        Badge = "Rentals",
                        ImageUrl =
                            "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?auto=format&fit=crop&w=900&q=80"
                    }
                ],
                TrendingAreas = new[] { "BGC, Taguig", "Makati", "Cebu IT Park" }
            };
            return View(vm);
        }

        public IActionResult Contact(int? agentId, int? propertyId)
        {
            var model = new ContactViewModel();
            if (agentId is { } a)
            {
                model.AgentId = a;
                var agent = _catalog.GetAgent(a);
                if (agent is not null)
                    model.Subject = $"Message for {agent.Name}";
            }

            if (propertyId is { } p)
            {
                model.PropertyId = p;
                var prop = _catalog.GetProperty(p);
                if (prop is not null)
                    model.Subject ??= $"Inquiry: {prop.Title}";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            TempData["ContactSuccess"] = "Thanks for reaching out. A member of our team will respond shortly.";
            return RedirectToAction(nameof(Contact));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

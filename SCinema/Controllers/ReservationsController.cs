using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SCinema.Controllers
{
    [Authorize] // on exige une connexion
    public class ReservationsController : Controller
    {
        public IActionResult Index()
        {
            // TODO: lister les réservations du user connecté
            return View();
        }
    }
}

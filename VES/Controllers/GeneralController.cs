using Microsoft.AspNetCore.Mvc;

namespace VES.Controllers
{
    public class GeneralController:Controller
    {
        public ActionResult GetStarted()
        {
            return View();
        }
    }
}

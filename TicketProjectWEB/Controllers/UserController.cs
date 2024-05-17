using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketProjectWEB.Models;

namespace TicketProjectWEB.Controllers
{
    [SessionAuthorize]
    public class UserController : Controller
    {
        private readonly CoreProject5DbContext _context;
        public UserController(CoreProject5DbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult EditUser()
        {
            return View();
        }

        public IActionResult ShowsPendingData()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetPendingData()
        {
            if (User.Identity.IsAuthenticated)
            {
                var loggedInUsername = User.Identity.Name;

                Console.WriteLine("Logged-in User: " + loggedInUsername);

                if (loggedInUsername != null)
                {
                    var result = _context.Ticketts
                        .Where(m => m.Status == "Pending" )
                        .GroupBy(m => m.CreatedBy)
                        .Select(g => new
                        {
                            CreatedBy = g.Key,
                            TotalTicketsPending = g.Count()
                        })
                        .ToList();

                    return Json(result);
                }
                else
                {
                    Console.WriteLine("Invalid user ID format: " + loggedInUsername);
                    return Json(new { error = "Invalid user ID format." });
                }
            }
            else
            {
                Console.WriteLine("User not authenticated.");
                return Json(new { error = "User not authenticated." });
            }
        }





        //from this, I'm getting for All the Users...
        //[HttpPost]
        //public JsonResult GetPendingData()
        //{
        //    var result = _context.Ticketts
        //        .Where(m => m.Status == "Pending")
        //        .GroupBy(m => m.CreatedBy)
        //        .Select(g => new
        //        {
        //            CreatedBy = g.Key,
        //            TotalTicketsPending = g.Count()
        //        })
        //        .ToList();

        //    return Json(result);
        //}



    }
}

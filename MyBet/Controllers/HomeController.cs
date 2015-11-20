using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialBetting.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            var tmp = DateTime.Now.AddYears(18) < DateTime.Now.AddYears(19);
            

            ViewBag.Message = "Page containin all bets";

            BetsDBDataContext context = new BetsDBDataContext();
            ViewBag.ListOfBetLinks = context.Bets.Select(f => f.name+"|"+f.id);
            return View();
        }

        public ActionResult TruncateAllTables()
        {
            BetsDBDataContext context = new BetsDBDataContext();
            context.ExecuteCommand("TRUNCATE TABLE  [MyBets].[dbo].[OutcomeFiles]" +
                                   "TRUNCATE TABLE  [MyBets].[dbo].[BetFiles] " + 
                                   "TRUNCATE TABLE  [MyBets].[dbo].[Outcomes]" +
                                   "TRUNCATE TABLE  [MyBets].[dbo].[Bets]" +
                                   "TRUNCATE TABLE  [MyBets].[dbo].[BindBetFile]" +
                                   "TRUNCATE TABLE  [MyBets].[dbo].[BindBetOutcome]" +
                                   "TRUNCATE TABLE  [MyBets].[dbo].[BindOutcomeFile]");

            return RedirectToAction("Index");
        }
    }
}

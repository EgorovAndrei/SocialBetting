using SocialBetting.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SocialBetting.Controllers
{
    public class BetController : Controller
    {

        BetsDBDataContext context = new BetsDBDataContext();

        //
        // GET: /Bet/

        public ActionResult Index(int id)
        {
            Bet bet = context.Bets.Where(b => b.id == id).First();
            
            if(bet==null)
            {
                return HttpNotFound();
            }

            MyBetModel betM = new MyBetModel();
            betM.id = bet.id;
            betM.Name = bet.name;
            betM.Description = bet.description;

            var lstOutcomes = context.BindBetOutcomes.Where(b => b.idBet == bet.id).Select(b=>b.idOutcome).ToList();

            var lstInfOutcomes = context.Outcomes.Where(o => lstOutcomes.Contains(o.id)).Select(o => o.id + "|" + o.name).ToList();
            ViewBag.lstInfOutcomes = lstInfOutcomes;

            return View(betM);
        }

        //
        // GET: /Bet/Details/5

        public List<HttpPostedFileBase> lstFilesForOneBet;

        public ActionResult Details(int id)
        {
            return View();
        }

        //page for creating new bet
        public ActionResult CreateBet()
        {
            //list of already uploaded files for this bet
            
            ViewBag.ListOfFileNames = context.BetFiles.Select(f => f.name);

            return View();
        }

        //
        // POST: /Bet/Create
        [HttpPost]
        public ActionResult CreateBet(MyBetModel newBet)
        {
            try
            {
                // TODO: Add insert logic here

                BetsDBDataContext context = new BetsDBDataContext();

                Bet bet = new Bet();
                bet.name = newBet.Name;
                bet.description = newBet.Description;
                context.Bets.InsertOnSubmit(bet);
                context.SubmitChanges();

                if (Request.Files.Count > 0)
                {
                    List<HttpPostedFileBase> files = TempData["appl"] as List<HttpPostedFileBase>;

                    if (files != null && files.Count != 0)
                    {
                        MyBetModel.WriteFileInDB(files, bet.id);
                        TempData.Clear();
                    }
                }

                return RedirectToAction("CreateOutcome", "Outcome", new { idBet = bet.id });
            }
            catch
            {
                return View();
            }
        }


        [HttpPost]
        public ActionResult  UploadFileForBet(int id)
        {
            try
            {
                var file = Request.Files[0];
                List<HttpPostedFileBase> tmp = TempData["appl"] as List<HttpPostedFileBase>;
                if (tmp == null) { tmp = new List<HttpPostedFileBase>(); }
                tmp.Add(file);
                TempData["appl"] = tmp;
            }
            catch { return View(); }
            return View();
        }

        //
        // GET: /Bet/Edit/5

        public ActionResult Edit(int id)
        {
            Bet bet = context.Bets.Where(b => b.id == id).First();

            if (bet == null)
            {
                return HttpNotFound();
            }

            MyBetModel betM = new MyBetModel();
            betM.id = bet.id;
            betM.Name = bet.name;
            betM.Description = bet.description;

            return View(betM);
        }

        //
        // POST: /Bet/Edit/5

        [HttpPost]
        public ActionResult Edit(MyBetModel betM)
        {
            try
            {
                BetsDBDataContext context = new BetsDBDataContext();

                Bet bet = context.Bets.Where(b => b.id == betM.id).First();
                if(bet!=null)
                {
                    bet.name = betM.Name;
                    bet.description = betM.Description;
                    context.SubmitChanges();

                    return RedirectToAction("Index", new { id = bet.id });
                }

                return RedirectToAction("Index","Home");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Bet/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Bet/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public void DeleteFile(string id)
        {
            List<HttpPostedFileBase> tempD = TempData["appl"] as List<HttpPostedFileBase>;
            if (tempD == null) { tempD = new List<HttpPostedFileBase>(); }
            tempD.RemoveAll(f => f.FileName == id);
            TempData["appl"] = tempD;
            //some code for removing file
        }
    }
}

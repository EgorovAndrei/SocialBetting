using SocialBetting.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SocialBetting.Controllers
{
    public class OutcomeController : Controller
    {

        //
        // GET: /Outcome/

        BetsDBDataContext context = new BetsDBDataContext();

        public ActionResult Index(int id)
        {
            Outcome bet = context.Outcomes.Where(o => o.id == id).First();

            if (bet == null)
            {
                return HttpNotFound();
            }

            MyOutcomeModel outcomeM = new MyOutcomeModel();
            outcomeM.Id = bet.id;
            outcomeM.Name = bet.name;
            outcomeM.Description = bet.description;

            return View(outcomeM);
        }

        //
        // GET: /Outcome/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // Page for creating new outcome
        public ActionResult CreateOutcome(string idBet)
        {
            //list of uploaded files for this outcome
            BetsDBDataContext context = new BetsDBDataContext();
            ViewBag.ListOfFileNames = context.OutcomeFiles.Select(f => f.name);

            //list of outcomes for same bet
            var curBet = Convert.ToInt32(idBet);
            var listOutcomeId = context.BindBetOutcomes.Where(b => b.idBet == curBet).Select(b => b.idOutcome); 

            ViewBag.ListOutcomes = context.Outcomes.Where(o =>listOutcomeId.Contains(o.id)).Select(o=>o.name);
            return View();
        }

        // write outcome in DB
        [HttpPost]
        public ActionResult CreateOutcome(MyOutcomeModel newOutcome)
        {
            try
            {
                  
                BetsDBDataContext context = new BetsDBDataContext();

                Outcome outcome = new Outcome();
                //outcome.bet = Convert.ToInt32(newOutcome.Id);
                outcome.name = newOutcome.Name;
                outcome.description = newOutcome.Description;
                context.Outcomes.InsertOnSubmit(outcome);
                context.SubmitChanges();

                if (Request.Files.Count > 0)
                {
                    List<HttpPostedFileBase> files = TempData["appl"] as List<HttpPostedFileBase>;

                    if (files != null && files.Count != 0)
                    { 
                        MyOutcomeModel.WriteFileInDB(files, outcome.id);
                        TempData.Clear();
                    }
                }

                BindBetOutcome bindBO = new BindBetOutcome();
                bindBO.idBet = Convert.ToInt32(newOutcome.Id);
                bindBO.idOutcome = outcome.id;
                context.BindBetOutcomes.InsertOnSubmit(bindBO);
                context.SubmitChanges();

                return RedirectToAction("CreateOutcome", "Outcome", new { idBet = newOutcome.Id });
            }
            catch(Exception exc)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult UploadFileForOutcome(int id)
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
        // GET: /Outcome/Edit/5

        public ActionResult Edit(int id)
        {

            Outcome outcome = context.Outcomes.Where(b => b.id == id).First();

            if (outcome == null)
            {
                return HttpNotFound();
            }

            MyOutcomeModel outcomeM = new MyOutcomeModel();
            outcomeM.Id = outcome.id;
            outcomeM.Name = outcome.name;
            outcomeM.Description = outcome.description;

            return View(outcomeM);
        }

        //
        // POST: /Outcome/Edit/5

        [HttpPost]
        public ActionResult Edit(MyOutcomeModel outcomeM)
        {
            try
            {
                // TODO: Add update logic here

                Outcome outcome = context.Outcomes.Where(b => b.id == outcomeM.Id).First();
                if (outcome != null)
                {
                    outcome.name = outcomeM.Name;
                    outcome.description = outcomeM.Description;
                    context.SubmitChanges();

                    return RedirectToAction("Index", new { id = outcome.id });
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Outcome/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Outcome/Delete/5

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
    }
}

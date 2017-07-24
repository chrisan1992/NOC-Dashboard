using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NOC_Macro.Models;
using System.Net.Mail;

namespace NOC_Macro.Controllers
{
    public class MajorIncidentsController : Controller
    {
        private NOCEntities db = new NOCEntities();

        // GET: MajorIncidents
        public ActionResult Index()
        {
            return View(db.MajorIncidents.ToList());
        }

        // GET: MajorIncidents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MajorIncidents majorIncidents = db.MajorIncidents.Find(id);
            if (majorIncidents == null)
            {
                return HttpNotFound();
            }
            return View(majorIncidents);
        }

        // GET: MajorIncidents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MajorIncidents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "iD,incidentNumber,descr,product,dataCenter,categorization,customerType")] MajorIncidents majorIncidents, string [] product, string [] datacenter, string lyncCall)
        {
            majorIncidents.dataCenter = "";
            majorIncidents.product = "";
            foreach (string d in datacenter)
            {
                majorIncidents.dataCenter += d + "-";
            }
            foreach (string p in product)
            {
                majorIncidents.product += p + "-";
            }
            if (ModelState.IsValid)
            {
                db.MajorIncidents.Add(majorIncidents);
                db.SaveChanges();
                String emailResult = new EmailSender().SendMacroEmail(majorIncidents,lyncCall);
                return RedirectToAction("Index");
            }

            return View(majorIncidents);
        }

        // GET: MajorIncidents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MajorIncidents majorIncidents = db.MajorIncidents.Find(id);
            if (majorIncidents == null)
            {
                return HttpNotFound();
            }
            return View(majorIncidents);
        }

        // POST: MajorIncidents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "iD,incidentNumber,descr,product,dataCenter,categorization,customerType")] MajorIncidents majorIncidents)
        {
            if (ModelState.IsValid)
            {
                db.Entry(majorIncidents).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(majorIncidents);
        }

        // GET: MajorIncidents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MajorIncidents majorIncidents = db.MajorIncidents.Find(id);
            if (majorIncidents == null)
            {
                return HttpNotFound();
            }
            return View(majorIncidents);
        }

        // POST: MajorIncidents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MajorIncidents majorIncidents = db.MajorIncidents.Find(id);
            db.MajorIncidents.Remove(majorIncidents);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
    }
}

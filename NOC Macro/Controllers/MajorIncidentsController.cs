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
using Oracle.ManagedDataAccess.Client;
using System.Threading;

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
        public ActionResult Create([Bind(Include = "incidentNumber,descr,product,dataCenter,categorization,customerType,topCustomers")] MajorIncidents majorIncidents, string[] product, string[] datacenter, string lyncCall)
        {
            majorIncidents.dataCenter = string.Join(",",datacenter);
            majorIncidents.product = string.Join(",", product);
            if (ModelState.IsValid)
            {
                db.MajorIncidents.Add(majorIncidents);
                //now I have to add the first timeline to the incident
                Timeline t = new Timeline();
                t.incidentNumber = majorIncidents.incidentNumber;
                t.description = "Incident #"+majorIncidents.incidentNumber+" - "+majorIncidents.descr+" is opened.";
                t.time = DateTime.Now;
                t.username = Session["username"].ToString();
                db.Timeline.Add(t);
                //save changes to DB
                db.SaveChanges();
                String emailResult = new EmailSender().SendMacroEmail(majorIncidents, lyncCall);
                Session["message"] = "Major Incident created, email is being sent in the background.";
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
        public ActionResult Edit([Bind(Include = "incidentNumber,descr,product,dataCenter,categorization,customerType,topCustomers")] MajorIncidents majorIncidents)
        {
            if (ModelState.IsValid)
            {
                db.Entry(majorIncidents).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(majorIncidents);
        }
        
        public JsonResult DeleteConfirmed(int delete_id)
        {
            MajorIncidents majorIncidents = db.MajorIncidents.Find(delete_id);
            db.MajorIncidents.Remove(majorIncidents);
            db.SaveChanges();
            Session["message"] = "Incident has been deleted.";
            var results = new List<int>()
                {
                    1
                }.ToList();
            //1 is good
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// GET Method for Major Incident Dashboard
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Dashboard(int? id)
        {
            if (id >= 0)
            {//valid ID to query
                String query = "select reqs.description incidentTitle,";
                query += "dets.visible_parameter1 startDate, ";
                query += "dets.visible_parameter2 endDate, ";
                query += "dets.visible_parameter40 CustomerType, ";
                query += "hdets.visible_parameter17 GOwnership,";
                query += "hdets.visible_parameter18 environment, ";
                query += "hdets.visible_parameter8 ImpactType, ";
                query += "hdets.visible_parameter20 IncidentManager,";
                query += "hdets.visible_parameter19 DataCenter,";
                query += "hdets.visible_parameter13 ProductLine ";
                //From
                query += "from kcrt_request_details dets, kcrt_requests reqs, kcrt_req_header_details hdets ";
                //Where
                query += "where dets.request_id = " + id + " and reqs.request_id = " + id + " and hdets.request_id = " + id + " and dets.batch_number = 1";

                //executes the query and return the results
                DataTable dt = Utilities.ExecuteOracleQuery(query);
                if (dt != null)
                {
                    //group, datacenter, impact, environment, product, customers impacted
                    ViewBag.IncidentNumber = id;
                    ViewBag.IncidentName = dt.Rows[0][0].ToString();
                    ViewBag.StartDate = dt.Rows[0][1].ToString();
                    ViewBag.EndDate = dt.Rows[0][2].ToString();
                    ViewBag.CustomerType = dt.Rows[0][3].ToString();
                    ViewBag.GroupOwnership = dt.Rows[0][4].ToString();
                    ViewBag.Datacenter = dt.Rows[0][8].ToString();
                    ViewBag.ImpactType = dt.Rows[0][6].ToString();
                    ViewBag.Environment = dt.Rows[0][5].ToString();
                    ViewBag.ProductLine = dt.Rows[0][9].ToString();
                }
            }
            return View();
        }

        /// <summary>
        /// partial view to render the timeline items for a major incident
        /// </summary>
        /// <param name="incidentNumber"></param>
        /// <returns></returns>
        public PartialViewResult Timeline(int incidentNumber)
        {
            List<Timeline> timeline = db.Timeline.Where(row => row.incidentNumber == incidentNumber).ToList();
            return PartialView("Timeline", timeline);
        }

        /// <summary>
        /// Adds an update to the timeline and returns the result
        /// </summary>
        /// <param name="timeline">update to add</param>
        /// <returns></returns>
        public JsonResult AddTimeline(string timeline)
        {
            //every line of the json reads every number
            try
            {
                Timeline t = new Timeline();
                t.description = timeline;
                t.incidentNumber = 135102;//CHANGE THIS!!!
                t.time = DateTime.Now;
                t.username = Session["username"].ToString();
                db.Timeline.Add(t);
                db.SaveChanges();
                var results = new List<int>()
                {
                    1
                }.ToList();
                //1 is good
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var results = new List<int>()
                {
                    0
                }.ToList();
                //0 is bad
                return Json(results, JsonRequestBehavior.AllowGet);
            }            
        }

        public JsonResult IsIncidentExists(int incidentNumber)
        {
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
            return Json(!db.MajorIncidents.Any(x => x.incidentNumber == incidentNumber), JsonRequestBehavior.AllowGet);
        }
    }
}
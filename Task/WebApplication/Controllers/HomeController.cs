using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using WebApplication.Models;
using WebApplication.Repositories;

namespace WebApplication.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class HomeController : Controller
    {
        private IEmployeeService employeeService;

        public HomeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubscribeAsync(SubscriptionViewModel subscriptionViewModel)
        {
            if (ModelState.IsValid)
            {
                string url = Util.GetServiceUri("clients/subscribe");
                string callback = Request.Url.GetLeftPart(UriPartial.Authority) + "/home/receive";

                FlurlClient client = url.SetQueryParams(new
                {
                    date = subscriptionViewModel.Date.ToString("yyyy-MM-dd"),
                    callback = callback

                }).WithHeader("Accept-Client", "Fourth-Monitor");

                SubscriptionResponse response = await client.GetJsonAsync<SubscriptionResponse>();
            }

            return  RedirectToAction("Index");
        }

        [HttpPost]
        public EmptyResult Receive(List<SimulationData> data)
        {
            string token = HttpContext.Request.Headers["X-Fourth-Token"];
            if (data != null)
            {
                employeeService.SaveEmployees(data);
            }

            return new EmptyResult();
        }

        [HttpGet]
        public ViewResult Employees(int? searchID, DateTime? subscriptionDate, string sortOrder, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDSortParm = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.DateSortParm = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            if (subscriptionDate.HasValue)
            {
                ViewBag.SubscriptionDate = subscriptionDate.Value.Date;
            }

            var employees = employeeService.GetEmployees(searchID, sortOrder, subscriptionDate, page);
            
            return View(employees);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                employeeService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
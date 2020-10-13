using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Kaziya.CRM.ConnectionPooling.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOrganizationService _service;

        public HomeController(IOrganizationService service)
        {
            _service = service;
        }
        
        public ActionResult Index()
        {
            var query = new QueryExpression("account")
            {
                NoLock = true,
                TopCount = 2
            };

            var accounts = _service.RetrieveMultiple(query);
            
            return Json(new { accountCount = accounts.Entities.Count } , JsonRequestBehavior.AllowGet);
        }
    }
}
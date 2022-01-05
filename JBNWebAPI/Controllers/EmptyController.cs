using JBNClassLibrary;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNWebAPI.Controllers
{
    public class EmptyController : Controller
    {
        // GET: Empty
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SavePDF(ProformaInvoice proformaInvoice)
        {
            return new ViewAsPdf("ProformaInvoice", proformaInvoice)
            {
                PageSize = Size.A4,
                FileName = "ProformaInvoice_" + proformaInvoice.main.AdvertisementMainID + ".pdf"
            };
        }
    }
}
using JBNClassLibrary;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class apiController : Controller
    {
        // GET: api
        public ActionResult Index(int id)
        {
            DLAdvertisements dLAdvertisements = new DLAdvertisements();
            Advertisement Result = new Advertisement();
            Result.AdvertisementMainID = id;
            ProformaInvoice proformaInvoice = dLAdvertisements.GenerateProformaInvoice(Result);

            return new ViewAsPdf("ProformaInvoice", proformaInvoice)
            {
                PageSize = Size.A4,
                FileName = "ProformaInvoice.pdf"
            };
        }
        //public ActionResult SavePDF(int? AdvertisementMainID)
        //{
        //    DLAdvertisements dLAdvertisements = new DLAdvertisements();
        //    Advertisement Result = new Advertisement();
        //    Result.AdvertisementMainID = AdvertisementMainID.Value;
        //    ProformaInvoice proformaInvoice = dLAdvertisements.GenerateProformaInvoice(Result);

        //    return new ViewAsPdf("ProformaInvoice", proformaInvoice)
        //    {
        //        PageSize = Size.A4,
        //        FileName = "ProformaInvoice.pdf"
        //    };
        //}
    }
}
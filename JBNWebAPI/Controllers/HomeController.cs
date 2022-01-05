using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using JBNClassLibrary;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using iTextSharp;
using Syncfusion.HtmlConverter;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.html;
using System.Configuration;
using Rotativa.Options;

namespace JBNWebAPI.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        protected ControllerContext Context { get; set; }
        //[Authorize]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }

        public ActionResult SavePDF(int AdvertisementMainID)
        {
            DLAdvertisements dLAdvertisements = new DLAdvertisements();
            Advertisement Result = new Advertisement();
            Result.AdvertisementMainID = AdvertisementMainID;
            ProformaInvoice proformaInvoice = dLAdvertisements.GenerateProformaInvoice(Result);

            return new ViewAsPdf("ProformaInvoice", proformaInvoice)
            {
                PageSize = Size.A4,
                FileName = "ProformaInvoice.pdf"
            };
        }
        /// <summary>
        /// Creates an instance of an MVC controller from scratch 
        /// when no existing ControllerContext is present       
        /// </summary>
        /// <typeparam name="T">Type of the controller to create</typeparam>
        /// <returns>Controller Context for T</returns>
        /// <exception cref="InvalidOperationException">thrown if HttpContext not available</exception>
        public static T CreateController<T>(RouteData routeData = null)
                    where T : Controller, new()
        {
            // create a disconnected controller instance
            T controller = new T();

            // get context wrapper from HttpContext if available
            HttpContextBase wrapper = null;
            if (System.Web.HttpContext.Current != null)
                wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            else
                throw new InvalidOperationException(
                    "Can't create Controller Context if no active HttpContext instance is available.");

            if (routeData == null)
                routeData = new RouteData();

            // add the controller routing if not existing
            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add("controller", controller.GetType().Name
                                                            .ToLower()
                                                            .Replace("controller", ""));

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
            return controller;
        }

        public static class StringUtilities
        {
            public static string RenderViewToString(System.Web.Mvc.ControllerContext context, string viewPath, object model = null, bool partial = false)
            {
                // first find the ViewEngine for this view
                ViewEngineResult viewEngineResult = null;
                if (partial)
                {
                    viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
                }
                else
                {
                    viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);
                }

                if (viewEngineResult == null)
                {
                    throw new FileNotFoundException("View cannot be found.");
                }

                // get the view and attach the model to view data
                var view = viewEngineResult.View;
                context.Controller.ViewData.Model = model;

                string result = null;

                using (var sw = new StringWriter())
                {
                    var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
                    view.Render(ctx, sw);
                    result = sw.ToString();
                }

                return result.Trim();
            }
        }

        #region Old Code used to convert PDF
        //[Obsolete]
        //public void converttopdf(string HTMLString, string fileLocation)
        //{
        //    Document document = new Document();

        //    PdfWriter.GetInstance(document, new FileStream(fileLocation, FileMode.Create));
        //    document.Open();

        //    List<IElement> htmlarraylist = HTMLWorker.ParseToList(new StringReader(HTMLString), null);
        //    for (int k = 0; k < htmlarraylist.Count; k++)
        //    {
        //        document.Add((IElement)htmlarraylist[k]);
        //    }

        //    document.Close();
        //}


        //[Obsolete]
        //public static Byte[] PdfSharpConvert(String html)
        //{
        //    StringReader sr = new StringReader(html.ToString());
        //    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
        //    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //    byte[] bytes;
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
        //        pdfDoc.Open();

        //        htmlparser.Parse(sr);
        //        pdfDoc.Close();

        //        bytes = memoryStream.ToArray();
        //        memoryStream.Close();
        //    }
        //    return bytes;
        //}

        #endregion
        public byte[] ConverttoPDF(string HTMLStr)
        {
            byte[] bytesArray = null;
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var document = new Document(PageSize.A4, 20f, 20f, 20f, 20f))
                    {
                        using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
                        {
                            document.Open();
                            writer.CloseStream = false;
                            using (var strReader = new StringReader(HTMLStr))
                            {
                                //Set factories
                                HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
                                htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
                                //Set css
                                ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
                                //cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath("~/Content/Invoice.css"), true);
                                cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath("~/Content/min.css"), true);
                                //cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/jqmin.js"), true);
                                //cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/min.js"), true);
                                //Export
                                IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));
                                var worker = new XMLWorker(pipeline, true);
                                var xmlParse = new XMLParser(true, worker);
                                xmlParse.Parse(strReader);
                                xmlParse.Flush();
                            }
                            document.Close();
                        }
                    }
                    bytesArray = ms.ToArray();
                }
                return bytesArray;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [Obsolete]
        public Byte[] getPDF(int AdvertisementMainID)
        {
            DLAdvertisements dLAdvertisements = new DLAdvertisements();
            Advertisement Result = new Advertisement();
            Result.AdvertisementMainID = AdvertisementMainID;
            ProformaInvoice proformaInvoice = dLAdvertisements.GenerateProformaInvoice(Result);

            var actionPDF = new Rotativa.ViewAsPdf("ProformaInvoice", proformaInvoice)
            {
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
               PageMargins = { Left = 1, Right = 1 }
            };
            byte[] applicationPDFData = actionPDF.BuildPdf(this.ControllerContext);
            return applicationPDFData;
        }
    }
}

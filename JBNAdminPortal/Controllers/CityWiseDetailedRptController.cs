using JBNAdminPortal.Models;
using JBNClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using static JBNClassLibrary.CityWiseDetails;

namespace JBNAdminPortal.Controllers
{
    public class CityWiseDetailedRptController : Controller
    {
        JBNClassLibrary.JBNDBClass DAL = new JBNClassLibrary.JBNDBClass();
        CityWiseDetails CityWiseDAL = new CityWiseDetails();
        // GET: CityWiseDetailedRpt

        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                CustomerDetails context = new CustomerDetails();
                State state = new State();
                City city = new City();
                state.StateID = 0;
                city.StateWithCityID = 0;
                context.state = state;
                context.city = city;
                ViewBag.StateList = new SelectList(DAL.GetStateList(), "ID", "StateName");
                ViewBag.BusinessType = new SelectList(DAL.GetBusinessTypes(), "ID", "BusinessTypeName");
                ViewBag.SubCategory = new SelectList(DAL.GetSubCatagoryList(), "ID", "SubCategoryName");
                ViewBag.CustomerList = new SelectList(DAL.GetCustomerList(context), "CustID", "FirmName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="cityWiseDetail"></param>
        /// <returns></returns>
        public ActionResult CustomerList(CityWiseDetailedRpt cityWiseDetail)
        {
            PromoWithList promoWithList = new PromoWithList();
            promoWithList.detailedList = CityWiseDAL.CityDetailedList(cityWiseDetail).ToList();
            return PartialView(promoWithList);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchByDate(Search search)
        {
            PromoWithList promoWithList = new PromoWithList();

            CityWiseDetailedRpt cityWiseDetail = new CityWiseDetailedRpt();
            cityWiseDetail.FromDate = search.FromDate;
            cityWiseDetail.ToDate = search.ToDate;
            cityWiseDetail.StateID = search.StateID != null ? search.StateID.Value : 0;
            cityWiseDetail.CityID = search.StatewithCityID != null ? search.StatewithCityID.Value : 0;
            cityWiseDetail.SubCategoryID = search.SubCategoryID != null ? search.SubCategoryID.Value : 0;
            cityWiseDetail.BusinessTypeID = search.BusinessTypeID != null ? search.BusinessTypeID.Value : 0;
            cityWiseDetail.CustID = search.CustID != null ? search.CustID.Value : 0;
            promoWithList.detailedList = CityWiseDAL.CityDetailedList(cityWiseDetail).ToList();
            return PartialView("CustomerList", promoWithList);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="Prefix"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Autocomplete(string Prefix)
        {
            List<tblStateWithCity> tblStateWithCity = new List<tblStateWithCity>();
            tblStateWithCity = CityWiseDAL.GetCities(Prefix);
            return Json(tblStateWithCity, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="promotion"></param>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Promotion(PromoWithList promotion, HttpPostedFileBase postedFile)
        {
            if (Session["UserID"] != null)
            {
                List<Attachment> MailAttachments = new List<Attachment>();

                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    try
                    {
                        string fileName = Path.GetFileName(postedFile.FileName);
                        var attachment = new Attachment(postedFile.InputStream, fileName);
                        MailAttachments.Add(attachment);
                    }
                    catch (Exception) { }
                }
                string Result = CityWiseDAL.Promotion(promotion, MailAttachments);
                //RootData myDeserializedClass = JsonConvert.DeserializeObject<RootData>(Result);

                // Generate a new unique identifier against which the file can be stored
                string handle = Guid.NewGuid().ToString();
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(Result);
                writer.Flush();
                stream.Position = 0;
                TempData[handle] = stream.ToArray();
                //using (MemoryStream memoryStream = new MemoryStream())
                //{
                //    workbook.SaveAs(memoryStream);
                //    memoryStream.Position = 0;
                //    TempData[handle] = memoryStream.ToArray();
                //}
                // Note we are returning a filename as well as the handle
                return new JsonResult()
                {
                    Data = new { FileGuid = handle, FileName = "PromoResult.txt" }
                };
            }
            else
            {
                return Json("sessionexpired");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="fileGuid"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult DownloadResponseData(string fileGuid, string fileName)
        {
            byte[] data = TempData[fileGuid] as byte[];
            return File(data, "application/csv", fileName);
        }
    }
}
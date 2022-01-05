using JBNClassLibrary;
using JBNWebAPI.Logger;
using NUnit.Framework;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using RotativaHQ.MVC5;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace JBNWebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Advertisements")]
    public class AdvertisementsController : ApiController
    {
        DLAdvertisements dLAdvertisements = new DLAdvertisements();
        JBNDBClass jbndbclass = new JBNDBClass();
        private mwbtDealerEntities db = new mwbtDealerEntities();
        readonly Logger.LoggerBlock loggerBlock = new Logger.LoggerBlock();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        int IsLogWrite = Convert.ToInt32(ConfigurationManager.AppSettings["IsLogWrite"].ToString());

        //Item List advertisement creation
        [ResponseType(typeof(ItemCategory))]
        [Route("GetAllItems/{SearchText}")]
        public IHttpActionResult GetAllItems(string SearchText)
        {
            try
            {
                List<ItemCategory> itemCategories = new List<ItemCategory>();
                itemCategories = dLAdvertisements.GetAllItems(SearchText);

                if (itemCategories == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, itemCategories));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get advertisement areas
        [HttpGet]
        [ResponseType(typeof(tblAdvertisementArea))]
        [Route("GetAdvertisementAreas")]
        public IHttpActionResult GetAdvertisementAreas()
        {
            try
            {
                List<tblAdvertisementArea> tblAdvertisementAreas = dLAdvertisements.GetAdvertisementAreas();
                if (tblAdvertisementAreas == null || tblAdvertisementAreas.Count <= 0)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, tblAdvertisementAreas));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get advertisement types
        [HttpGet]
        [ResponseType(typeof(tblAdvertisementType))]
        [Route("GetAdvertisementTypes")]
        public IHttpActionResult GetAdvertisementTypes()
        {
            try
            {
                List<tblAdvertisementType> tblAdvertisementTypes = dLAdvertisements.GetAdvertisementTypes();
                if (tblAdvertisementTypes == null || tblAdvertisementTypes.Count <= 0)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, tblAdvertisementTypes));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get advertisement time slots
        [HttpGet]
        [ResponseType(typeof(tblAdvertisementTimeSlot))]
        [Route("GetAdTimeSlots")]
        public IHttpActionResult GetAdTimeSlots()
        {
            try
            {
                List<tblAdvertisementTimeSlot> tblAdvertisementTimeSlots = dLAdvertisements.GetAdTimeSlots();
                if (tblAdvertisementTimeSlots == null || tblAdvertisementTimeSlots.Count <= 0)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, tblAdvertisementTimeSlots));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(District))]
        [Route("GetDistricts/{DistrictName}")]
        public IHttpActionResult GetDistricts([FromBody] List<tblState> StateList, string DistrictName)
        {
            try
            {
                List<District> Districts = new List<District>();
                Districts = jbndbclass.GetDistricts(StateList, DistrictName);
                if (Districts == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Districts));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(City))]
        [Route("GetCitiesOfDistrict/{CityName}")]
        public IHttpActionResult GetCitiesOfDistrict([FromBody] List<tblDistrict> Districts, string CityName)
        {
            try
            {
                List<City> Cities = new List<City>();
                Cities = jbndbclass.GetCitiesOfDistrict(Districts, CityName);
                if (Cities == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
                }
                //return Ok(sendMessageParameters);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Cities));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(City))]
        [Route("GetStateWiseCities/{CityName}")]
        public IHttpActionResult GetStateWiseCities([FromBody] List<tblState> StateList, string CityName)
        {
            try
            {
                List<City> Cities = new List<City>();
                Cities = jbndbclass.GetStateWiseCities(StateList, CityName);
                if (Cities == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Cities));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(Advertisement))]
        [Route("CheckSlotAvailability")]
        public IHttpActionResult CheckSlotAvailability([FromBody] Advertisement advertisement)
        {
            try
            {
                AdvertisementMain result = new AdvertisementMain();
                result = dLAdvertisements.CheckSlotAvailability(advertisement);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [Obsolete]
        [HttpGet]
        [ResponseType(typeof(string))]
        [Route("CreateInvoice/{AdvertisementMainID}")]
        public IHttpActionResult CreateInvoice(int AdvertisementMainID)
        {
            try
            {
                HomeController controller = new HomeController();
                System.Web.Routing.RouteData route = new System.Web.Routing.RouteData();
                route.Values.Add("action", "getPdf"); // ActionName
                route.Values.Add("controller", "PDF"); // Controller Name
                System.Web.Mvc.ControllerContext newContext = new
                System.Web.Mvc.ControllerContext(new HttpContextWrapper(System.Web.HttpContext.Current), route, controller);
                controller.ControllerContext = newContext;
                var actionPDF = controller.getPDF(AdvertisementMainID);
                string filePath = HostingEnvironment.MapPath("~/Invoices/ProformaInvoice_" + AdvertisementMainID + ".pdf");
                System.IO.File.WriteAllBytes(filePath, actionPDF);
                string InvoiceURL = ConfigurationManager.AppSettings["InvoicePath"] + string.Format("ProformaInvoice_{0}.pdf", AdvertisementMainID);
                string URL = GetBaseUrl();
                Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(InvoiceURL));
                InvoiceURL = URL + InvoiceURL;
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, InvoiceURL));

                //HomeController homeController = RunControllerAsCurrentUser(new HomeController());
                //string InvoiceURL = homeController.SavePDF(AdvertisementMainID).ToString();
                //string URL = GetBaseUrl();
                //Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                //Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(InvoiceURL));
                //InvoiceURL = URL + InvoiceURL;
                //return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, actionPDF));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(Advertisement))]
        [Route("SaveAdvertisement")]
        public IHttpActionResult SaveAdvertisement([FromBody] AdvertisementMain advertisementMain)
        {
            try
            {
                AdvertisementMain result = new AdvertisementMain();
                result = dLAdvertisements.SaveAdvertisement(advertisementMain);

                if (result.DispayMessage.ToLower().Contains("success"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, result));
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get brands
        [HttpGet]
        [ResponseType(typeof(Brands))]
        [Route("GetBrands")]
        public IHttpActionResult GetBrands()
        {
            try
            {
                List<Brands> tblbrands = dLAdvertisements.GetBrands();
                if (tblbrands == null || tblbrands.Count <= 0)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, tblbrands));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [Route("UploadAdImage")]
        public async Task<IHttpActionResult> UploadAdImage()
        {
            try
            {
                Helper.TransactionLog("Hi", 0);
                
                byte[] imagesBytes = null;
                var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
                var json = await filesReadToProvider.Contents[0].ReadAsStringAsync();
                Helper.TransactionLog(filesReadToProvider.Contents.Count.ToString(), 0);
                if (filesReadToProvider.Contents.Count > 1)
                {
                    imagesBytes = await filesReadToProvider.Contents[1].ReadAsByteArrayAsync();
                }
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json, 0);

                AdvertisementMain advertisementMain = JsonConvert.DeserializeObject<AdvertisementMain>(json);
                AdvertisementMain Main = dLAdvertisements.GetAdvertisementMain(advertisementMain);
                Helper.CreateFolder(HttpContext.Current.Server.MapPath("~/AdImages/"));
                if (imagesBytes != null && imagesBytes.Length > 0)
                {
                    string filePath = HttpContext.Current.Server.MapPath("~/AdImages/" + string.Format("img_{0}.jpg", Main.AdvertisementName));
                    MemoryStream ms = new MemoryStream(imagesBytes, 0, imagesBytes.Length);
                    ms.Write(imagesBytes, 0, imagesBytes.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    image.Save(filePath);
                    string serverPath = ConfigurationManager.AppSettings["AdImagePath"] + string.Format("img_{0}.jpg", Main.AdvertisementName);
                    Main.AdImageURL = serverPath;
                }
                AdvertisementMain Result = dLAdvertisements.SaveAdvertisementImage(Main);
                string URL = GetBaseUrl();
                Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                if (!string.IsNullOrEmpty(Result.AdImageURL))
                {
                    Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(Result.AdImageURL));
                    Result.AdImageURL = URL + Result.AdImageURL;
                }
                
                return ResponseMessage(Request.CreateResponse(Result.StatusCode, Result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [Route("PostAdText")]
        public IHttpActionResult PostAdText([FromBody] AdvertisementMain advertisementMain)
        {
            try
            {
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(advertisementMain);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 0);

                AdvertisementMain Result = dLAdvertisements.PostAdText(advertisementMain);

                if(Result.StatusCode == HttpStatusCode.OK)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Result));
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, Result));
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get advertisements of an user
        [HttpGet]
        [ResponseType(typeof(AdvertisementMain_S))]
        [Route("GetAdvertisementsOfAnUser/{CustID}")]
        public IHttpActionResult GetAdvertisementsOfAnUser(int CustID)
        {
            try
            {
                List<AdvertisementMain_S> advertisementMains = dLAdvertisements.GetAdvertisementsOfAnUser(CustID);
                if (advertisementMains == null || advertisementMains.Count <= 0)
                {
                    return NotFound();
                }
                string URL = GetBaseUrl();
                Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                foreach (var item in advertisementMains)
                {
                    if (!string.IsNullOrEmpty(item.AdImageURL))
                    {
                        Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.AdImageURL));
                        item.AdImageURL = URL + item.AdImageURL;
                    }
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, advertisementMains));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get sent (approved) advertisements of an user
        [HttpGet]
        [ResponseType(typeof(AdvertisementMain_S))]
        [Route("GetSentAdvertisementsOfAnUser/{CustID}")]
        public IHttpActionResult GetSentAdvertisementsOfAnUser(int CustID)
        {
            try
            {
                List<AdvertisementMain_S> advertisementMains = dLAdvertisements.GetSentAdvertisementsOfAnUser(CustID);
                if (advertisementMains == null || advertisementMains.Count <= 0)
                {
                    return NotFound();
                }
                string URL = GetBaseUrl();
                Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                foreach (var item in advertisementMains)
                {
                    if (!string.IsNullOrEmpty(item.AdImageURL))
                    {
                        Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.AdImageURL));
                        item.AdImageURL = URL + item.AdImageURL;
                    }
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, advertisementMains));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get advertisement details
        [HttpGet]
        [ResponseType(typeof(AdvertisementMain))]
        [Route("GetAdvertisementDetails/{AdvertisementMainID}")]
        public IHttpActionResult GetAdvertisementDetails(int AdvertisementMainID)
        {
            try
            {
                AdvertisementMain advertisementMain = dLAdvertisements.GetAdvertisementDetails(AdvertisementMainID);
                if (advertisementMain == null)
                {
                    return NotFound();
                }
                string URL = GetBaseUrl();
                Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                if (!string.IsNullOrEmpty(advertisementMain.AdImageURL))
                {
                    Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(advertisementMain.AdImageURL));
                    advertisementMain.AdImageURL = URL + advertisementMain.AdImageURL;
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, advertisementMain));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get advertisement details
        [HttpGet]
        [ResponseType(typeof(AdvertisementMain))]
        [Route("GetBannerImages/{CustID}")]
        public IHttpActionResult GetBannerImages(int CustID)
        {
            try
            {
                List<Advertisement> advertisementList = dLAdvertisements.GetBannerImages(CustID, 2);

                if (advertisementList == null || advertisementList.Count == 0)
                {
                    advertisementList = jbndbclass.GetPromotions(1);
                }
                string URL = GetBaseUrl();
                Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);

                foreach (var item in advertisementList)
                {
                    if (!string.IsNullOrEmpty(item.AdImageURL))
                    {
                        Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.AdImageURL));
                        item.AdImageURL = URL + item.AdImageURL;
                    }
                    if(item.ProductID != null)
                    {
                        item.businessTypes = jbndbclass.GetBusinessTypesForItem(item.ProductID.Value);
                    }
                }

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, advertisementList));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get text details
        [HttpGet]
        [ResponseType(typeof(AdvertisementMain))]
        [Route("GetTextAdvertisements/{CustID}")]
        public IHttpActionResult GetTextAdvertisements(int CustID)
        {
            try
            {
                List<Advertisement> advertisementList = dLAdvertisements.GetBannerImages(CustID, 3);
                if (advertisementList == null || advertisementList.Count <= 0)
                {
                    advertisementList = jbndbclass.GetPromotions(2);
                }
                else
                {
                    foreach (var item in advertisementList)
                    {
                        //if (!string.IsNullOrEmpty(item.AdImageURL))
                        //{
                        //    Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.AdImageURL));
                        //    item.AdImageURL = URL + item.AdImageURL;
                        //}
                        if(item.ProductID != null)
                        {
                            item.businessTypes = jbndbclass.GetBusinessTypesForItem(item.ProductID.Value);
                        }
                    }
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, advertisementList));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get received advertisements
        [HttpGet]
        [ResponseType(typeof(Advertisement))]
        [Route("GetReceivedAdvertisements/{CustID}")]
        public IHttpActionResult GetReceivedAdvertisements(int CustID)
        {
            try
            {
                List<Advertisement> advertisementList = dLAdvertisements.GetAllAdvertisements(CustID);
                if (advertisementList == null)
                {
                    return NotFound();
                }
                string URL = GetBaseUrl();
                Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);

                foreach (var item in advertisementList)
                {
                    if (!string.IsNullOrEmpty(item.AdImageURL))
                    {
                        Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.AdImageURL));
                        item.AdImageURL = URL + item.AdImageURL;
                    }
                    item.businessTypes = jbndbclass.GetBusinessTypesForItem(item.ProductID.Value);
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, advertisementList));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //calcen advertisement details
        [HttpGet]
        [ResponseType(typeof(AdvertisementMain))]
        [Route("CancelAdvertisement/{AdvertisementMainID}")]
        public IHttpActionResult CancelAdvertisement(int AdvertisementMainID)
        {
            try
            {
                string Result = dLAdvertisements.CancelAdvertisement(AdvertisementMainID);                
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //Payment
        [HttpPost]
        [Route("PostAdPayments")]
        public IHttpActionResult PostAdPayments([FromBody] PaymentDetails paymentDetails)
        {
            try
            {
                PaymentDetails Result = dLAdvertisements.PostAdPayments(paymentDetails);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //Internal Methods
        public T RunControllerAsCurrentUser<T>(T controller, System.Web.Routing.RouteData routeData = null) where T : System.Web.Mvc.ControllerBase
        {
            routeData = new System.Web.Routing.RouteData();
            routeData.Values.Add("Controller", "Home");
            routeData.Values.Add("Action", "Index");
            var newContext = new System.Web.Mvc.ControllerContext(new System.Web.HttpContextWrapper(System.Web.HttpContext.Current), routeData ?? new System.Web.Routing.RouteData(), controller);
            controller.ControllerContext = newContext;
            return controller;
        }
        public string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        //[Obsolete]
        //[Route("Pdf/{AdvertisementMainID}")]
        //public Byte[] Pdf(int AdvertisementMainID)
        //{
        //    HomeController controller = new HomeController();
        //    System.Web.Routing.RouteData route = new System.Web.Routing.RouteData();
        //    route.Values.Add("action", "getPdf"); // ActionName
        //    route.Values.Add("controller", "PDF"); // Controller Name
        //    System.Web.Mvc.ControllerContext newContext = new
        //    System.Web.Mvc.ControllerContext(new HttpContextWrapper(System.Web.HttpContext.Current), route, controller);
        //    controller.ControllerContext = newContext;
        //    var actionPDF = controller.getPDF(AdvertisementMainID);
        //    return actionPDF;
        //    //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //    //response.Content = new ByteArrayContent(actionPDF);// new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
        //    //response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        //    //response.Content.Headers.ContentDisposition.FileName = "SamplePDF.PDF";
        //    //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        //    //return response;
        //}
    }
}

using JBNClassLibrary;
using JBNWebAPI.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;

namespace JBNWebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Product")]

    public class ProductController : ApiController
    {
        JBNDBClass jbndbclass = new JBNDBClass();
        private mwbtDealerEntities db = new mwbtDealerEntities();
        readonly Logger.LoggerBlock loggerBlock = new Logger.LoggerBlock();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        int IsLogWrite = Convert.ToInt32(ConfigurationManager.AppSettings["IsLogWrite"].ToString());

        //Search product in update profile page
        //added isProfessional on 05-10-2020
        [HttpPost]
        [ResponseType(typeof(tblSubCategory))]
        [Route("ChildCategoryList/{SearchText}/{CustID}/{isProfessional}")]
        public IHttpActionResult ChildCategoryList(string SearchText, int CustID, bool isProfessional)
        {
            try
            {
                if (IsLogWrite == 1)
                    Helper.TransactionLog(isProfessional.ToString() + "**" + SearchText, 1);
                //List<tblChildCategory> tblChildCategories = new List<tblChildCategory>();
                List<SubCategoryProducts> tblSubCategories = new List<SubCategoryProducts>();
                tblSubCategories = jbndbclass.GetSubCatagoryList(SearchText, CustID, isProfessional);
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(tblSubCategories);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 0);
                if (tblSubCategories == null)
                {
                    return NotFound();
                }
                return Ok(tblSubCategories);
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //Enquiry Page
        [HttpPost]
        [ResponseType(typeof(tblChildCategory))]
        [Route("ChildCategoryListForSearch/{SearchText}/{isProfessional}/{IsAdvertisement}")]
        public IHttpActionResult ChildCategoryListForSearch(string SearchText, bool isProfessional, bool IsAdvertisement = false)
        {
            try
            {
                if (IsLogWrite == 1)
                    Helper.TransactionLog(SearchText, 1);
                List<ChildCategories> tblChildCategories = new List<ChildCategories>();
                tblChildCategories = jbndbclass.GetChildCatagories(SearchText, isProfessional, IsAdvertisement);
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(tblChildCategories);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 0);
                if (tblChildCategories == null)
                {
                    return NotFound();
                }
                return Ok(tblChildCategories);
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }


        [HttpPost]
        [ResponseType(typeof(tblChildCategory))]
        [Route("SubCategoryList")]
        public IHttpActionResult SubCategoryList([FromBody] List<ChildCategoryList> ChildCategoryList)
        {
            try
            {
                List<tblSubCategory> CategoryList = new List<tblSubCategory>();
                CategoryList = jbndbclass.GetSubCatagories(ChildCategoryList);

                if (CategoryList == null)
                {
                    return NotFound();
                }
                return Ok(CategoryList);
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(tblCategoryProduct))]
        [Route("CategoryList")]
        public IHttpActionResult CategoryList([FromBody] List<SubCategoryList> SubCategoryList)
        {
            try
            {
                List<tblCategoryProduct> categoryList = new List<tblCategoryProduct>();
                categoryList = jbndbclass.GetCategory(SubCategoryList);
                if (categoryList == null)
                {
                    return NotFound();
                }
                return Ok(categoryList);
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(BusinessTypes))]
        [Route("BusinessTypeList")]
        public IHttpActionResult BusinessTypeList()
        {
            try
            {
                List<BusinessTypes> tblBusinessTypeList = new List<BusinessTypes>();
                tblBusinessTypeList = jbndbclass.GetBusinessTypes();
                if (tblBusinessTypeList == null)
                {
                    return NotFound();
                }
                return Ok(tblBusinessTypeList);
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(tblState))]
        [Route("StateList")]
        public IHttpActionResult StateList()
        {
            try
            {
                List<tblState> tblstateList = new List<tblState>();
                tblstateList = jbndbclass.GetStateList();
                if (tblstateList == null)
                {
                    return NotFound();
                }
                return Ok(tblstateList);
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(tblStateWithCity))]
        [Route("CityList/{StateId}/{CityName}")]
        public IHttpActionResult CityList(int StateId, string CityName)
        {
            try
            {
                //string FilePath = HttpContext.Current.Server.MapPath("~/States/" + StateId + ".txt");
                List<CityView> tblStateWithCitiesList = new List<CityView>();
                tblStateWithCitiesList = jbndbclass.GetCities(StateId, CityName);
                //tblStateWithCitiesList = jbndbclass.GetCitiesFromFile(FilePath, StateId);
                if (tblStateWithCitiesList == null)
                {
                    return NotFound();
                }
                return Ok(tblStateWithCitiesList);
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpGet]
        [ResponseType(typeof(tblStateWithCity))]
        [Route("GetTierOneCities")]
        public IHttpActionResult GetTierOneCities()
        {
            try
            {
                List<City> tblStateWithCitiesList = new List<City>();
                tblStateWithCitiesList = jbndbclass.GetTierOneCities();
                if (tblStateWithCitiesList == null)
                {
                    return NotFound();
                }
                return Ok(tblStateWithCitiesList);
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(SearchDistinctList))]
        [Route("SearchProductDealer")]
        public IHttpActionResult SearchProductDealer([FromBody] SearchParameters searchParameters)
        {
            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(searchParameters);
            if (IsLogWrite == 1)
                Helper.TransactionLog(json1, 1);
            loggerBlock.LogWriter.Write(json1, "");
            try
            {
                var searchDistinctList = jbndbclass.SearchProductDealer(searchParameters);
                string jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(searchDistinctList);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(jsonResult, 1);
                if (searchDistinctList != null)
                {
                    return Ok(searchDistinctList);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [ResponseType(typeof(tblBusinessType))]
        [Route("GetDealerBusinessTypes/{CustID}")]
        public IHttpActionResult GetDealerBusinessTypes(int CustID)
        {
            try
            {
                List<tblBusinessType> tblBusinessType = jbndbclass.GetDealerBusinessTypes(CustID);
                if (tblBusinessType != null)
                {
                    return Ok(tblBusinessType);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpGet]
        [ResponseType(typeof(tblStateWithCity))]
        [Route("GetAllCities")]
        public IHttpActionResult GetAllCities()
        {
            try
            {
                List<tblStateWithCity> tblStateWithCitiesList = new List<tblStateWithCity>();
                tblStateWithCitiesList = jbndbclass.GetAllCities();
                if (tblStateWithCitiesList == null)
                {
                    return NotFound();
                }
                return Ok(tblStateWithCitiesList);
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(tblCustomerDetail))]
        [Route("SubmitCustomerQuery")]
        public IHttpActionResult SubmitCustomerQuery([FromBody] SubmitQuery submitQuery)
        {
            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(submitQuery);
            if (IsLogWrite == 1)
                Helper.TransactionLog(json1, 1);
            try
            {
                #region
                //int randomNumber = 0;
                //Random r = new Random();
                //if (submitQuery.ProductPhoto != null)
                //{
                //    randomNumber = r.Next();
                //    string filePath = HttpContext.Current.Server.MapPath("~/Images/" + string.Format("img_{0}.jpg", randomNumber));
                //    byte[] imageBytes = Convert.FromBase64String(submitQuery.ProductPhoto);
                //    MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                //    ms.Write(imageBytes, 0, imageBytes.Length);
                //    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                //    image.Save(filePath);
                //    string serverPath = "http://" + Request.RequestUri.Authority + ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}.jpg", randomNumber);
                //    submitQuery.ProductPhoto = serverPath;
                //}

                //if (submitQuery.ProductPhoto2 != null)
                //{
                //    randomNumber = r.Next();
                //    string filePath = HttpContext.Current.Server.MapPath("~/Images/" + string.Format("img_{0}.jpg", randomNumber));
                //    byte[] imageBytes = Convert.FromBase64String(submitQuery.ProductPhoto2);
                //    MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                //    ms.Write(imageBytes, 0, imageBytes.Length);
                //    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                //    image.Save(filePath);
                //    string serverPath = "http://" + Request.RequestUri.Authority + ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}.jpg", randomNumber);
                //    submitQuery.ProductPhoto2 = serverPath;
                //}

                #endregion

                SubmitQuery Result = jbndbclass.SubmitCustomerQuery(submitQuery);
                return ResponseMessage(Request.CreateResponse(Result.StatusCode, Result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }


        [ResponseType(typeof(City))]
        [Route("GetCityOfDealer/{UserId}")]
        public IHttpActionResult GetCityOfDealer(int UserId)
        {
            //string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(submitQuery);
            loggerBlock.LogWriter.Write("user id is:", UserId.ToString());
            try
            {
                List<City> cities = jbndbclass.GetCityOfDealer(UserId);
                if (cities != null)
                    return Ok(cities);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(tblCustomerDetail))]
        [Route("SubmitCustomerQueryWithFiles")]
        public async Task<IHttpActionResult> SubmitCustomerQueryWithFiles()
        {
            try
            {
                byte[] imagesBytes1 = null;
                byte[] imageBytes2 = null;
                var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
                var json = await filesReadToProvider.Contents[0].ReadAsStringAsync();
                if (IsLogWrite == 1)
                {
                    Helper.TransactionLog(json, 1);
                    Helper.TransactionLog(filesReadToProvider.Contents.Count.ToString() + "**" + "Total Count", 1);
                }

                if (filesReadToProvider.Contents.Count > 1)
                {
                    if (filesReadToProvider.Contents.Count == 3)
                    {
                        imagesBytes1 = await filesReadToProvider.Contents[1].ReadAsByteArrayAsync();
                        imageBytes2 = await filesReadToProvider.Contents[2].ReadAsByteArrayAsync();
                    }
                    else
                    {
                        imagesBytes1 = await filesReadToProvider.Contents[1].ReadAsByteArrayAsync();
                    }
                }
                SubmitQuery submitQuery = JsonConvert.DeserializeObject<SubmitQuery>(json);

                int randomNumber = 0;
                Random r = new Random();
                Helper.CreateFolder(HttpContext.Current.Server.MapPath("~/Images/"));
                if (imagesBytes1 != null && imagesBytes1.Length > 0)
                {
                    randomNumber = r.Next();
                    string filePath = HttpContext.Current.Server.MapPath("~/Images/" + string.Format("img_{0}.jpg", randomNumber));
                    MemoryStream ms = new MemoryStream(imagesBytes1, 0, imagesBytes1.Length);
                    ms.Write(imagesBytes1, 0, imagesBytes1.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    image.Save(filePath);
                    string serverPath = ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}.jpg", randomNumber);
                    submitQuery.ProductPhoto = serverPath;
                }
                else
                {
                    if (!string.IsNullOrEmpty(submitQuery.ProductPhoto))
                    {
                        string serverPath = ConfigurationManager.AppSettings["AdImagePath"] + submitQuery.ProductPhoto;
                        submitQuery.ProductPhoto = serverPath;
                    }
                }

                if (imageBytes2 != null && imageBytes2.Length > 0)
                {
                    randomNumber = r.Next();
                    string filePath = HttpContext.Current.Server.MapPath("~/Images/" + string.Format("img_{0}.jpg", randomNumber));
                    MemoryStream ms = new MemoryStream(imageBytes2, 0, imageBytes2.Length);
                    ms.Write(imageBytes2, 0, imageBytes2.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    image.Save(filePath);
                    string serverPath = ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}.jpg", randomNumber);
                    submitQuery.ProductPhoto2 = serverPath;
                }

                SubmitQuery Result = jbndbclass.SubmitCustomerQuery(submitQuery);
                return ResponseMessage(Request.CreateResponse(Result.StatusCode, Result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(tblUserConversation))]
        [Route("SendMessage")]
        public async Task<IHttpActionResult> SendMessage()
        {
            try
            {
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                byte[] fileBytes = null;
                var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
                var json = await filesReadToProvider.Contents[0].ReadAsStringAsync();
                string filetype = string.Empty;

                tblUserConversation tblUserConversation = JsonConvert.DeserializeObject<tblUserConversation>(json);
                Helper.TransactionLog(filesReadToProvider.Contents.Count().ToString() + "Count", 1);
                //if (filesReadToProvider.Contents.Count > 2)
                //{
                //    Helper.TransactionLog("pdf yes", 1);
                //    fileBytes = await filesReadToProvider.Contents[2].ReadAsByteArrayAsync();
                //    filetype = filesReadToProvider.Contents[2].Headers.ContentType.MediaType;
                //    Helper.TransactionLog(filetype, 1);
                //    if (!string.IsNullOrEmpty(filetype))
                //    {
                //        if (filetype.Contains("pdf"))
                //        {
                //            if (fileBytes != null && fileBytes.Length > 0)
                //            {
                //                string dateNow = DateTimeNow.ToString();
                //                dateNow = dateNow.Replace(" ", string.Empty);
                //                dateNow = dateNow.Replace(":", "_");
                //                dateNow = dateNow.Replace("-", "_");
                //                if (dateNow.Contains('/'))
                //                    dateNow = dateNow.Replace("/", "_");

                //                Helper.CreateFolder(HttpContext.Current.Server.MapPath("~/Documents/"));

                //                string filePath = HttpContext.Current.Server.MapPath("~/Documents/" + string.Format("doc_{0}_{1}_{2}_{3}.pdf", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow));
                //                //byte[] imageBytes = Convert.FromBase64String(tblUserConversation.Image);
                //                MemoryStream ms = new MemoryStream(fileBytes, 0, fileBytes.Length);
                //                File.WriteAllBytes(filePath, fileBytes);
                //                //ms.Write(fileBytes, 0, fileBytes.Length);
                //                //System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                //                //image.Save(filePath);
                //                //string serverPath = "http://" + Request.RequestUri.Authority + ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}_{1}_{2}_{3}.jpg", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow);
                //                string serverPath = ConfigurationManager.AppSettings["DocumentPath"] + string.Format("doc_{0}_{1}_{2}_{3}.pdf", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow);
                //                tblUserConversation.Image = serverPath;
                //            }
                //        }
                //    }
                //}
                if(filesReadToProvider.Contents.Count > 1)
                {
                    fileBytes = await filesReadToProvider.Contents[1].ReadAsByteArrayAsync();
                    filetype = filesReadToProvider.Contents[1].Headers.ContentType.MediaType;
                    Helper.TransactionLog(filetype, 1);
                    if (filetype.Contains("jpeg") || filetype.Contains("jpg"))
                    {
                        Helper.TransactionLog("image yes", 1);
                        if (fileBytes != null && fileBytes.Length > 0)
                        {
                            string dateNow = DateTimeNow.ToString();
                            dateNow = dateNow.Replace(" ", string.Empty);
                            dateNow = dateNow.Replace(":", "_");
                            dateNow = dateNow.Replace("-", "_");
                            if (dateNow.Contains('/'))
                                dateNow = dateNow.Replace("/", "_");

                            Helper.CreateFolder(HttpContext.Current.Server.MapPath("~/Images/"));
                            string filePath = HttpContext.Current.Server.MapPath("~/Images/" + string.Format("img_{0}_{1}_{2}_{3}.jpg", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow));
                            //byte[] imageBytes = Convert.FromBase64String(tblUserConversation.Image);
                            MemoryStream ms = new MemoryStream(fileBytes, 0, fileBytes.Length);
                            ms.Write(fileBytes, 0, fileBytes.Length);
                            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                            image.Save(filePath);
                            //string serverPath = "http://" + Request.RequestUri.Authority + ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}_{1}_{2}_{3}.jpg", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow);
                            string serverPath = ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}_{1}_{2}_{3}.jpg", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow);
                            tblUserConversation.Image = serverPath;
                        }
                    }
                    else if (filetype.Contains("pdf"))
                    {
                        Helper.TransactionLog("pdf yes", 1);
                        string DocName = @filesReadToProvider.Contents[1].Headers.ContentDisposition.FileName;
                        DocName = GetSafeFilename(DocName);
                        DocName = DocName.Replace(".pdf", "");

                        string illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
                        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

                        foreach (char c in invalid)
                        {
                            illegal = illegal.Replace(c.ToString(), "");
                        }

                        //if (DocName.Contains("\\"))
                        //{

                        //}
                        if (fileBytes != null && fileBytes.Length > 0)
                        {
                            string dateNow = DateTimeNow.ToString();
                            dateNow = dateNow.Replace(" ", string.Empty);
                            dateNow = dateNow.Replace(":", "_");
                            dateNow = dateNow.Replace("-", "_");
                            if (dateNow.Contains('/'))
                                dateNow = dateNow.Replace("/", "_");

                            Helper.CreateFolder(HttpContext.Current.Server.MapPath("~/Documents/"));

                            string filePath = HttpContext.Current.Server.MapPath("~/Documents/" + string.Format(DocName + "_{0}_{1}_{2}_{3}.pdf", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow));
                            //byte[] imageBytes = Convert.FromBase64String(tblUserConversation.Image);
                            MemoryStream ms = new MemoryStream(fileBytes, 0, fileBytes.Length);
                            File.WriteAllBytes(filePath, fileBytes);
                            //ms.Write(fileBytes, 0, fileBytes.Length);
                            //System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                            //image.Save(filePath);
                            //string serverPath = "http://" + Request.RequestUri.Authority + ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}_{1}_{2}_{3}.jpg", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow);
                            string serverPath = ConfigurationManager.AppSettings["DocumentPath"] + string.Format(DocName + "_{0}_{1}_{2}_{3}.pdf", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow);
                            tblUserConversation.Image = serverPath;
                        }
                    }
                }
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json, 1);

                #region Old
                //if(fileBytes != null && fileBytes.Length > 0)
                //{
                //    filetype = filesReadToProvider.Contents[1].Headers.ContentType.MediaType;
                //    if (!string.IsNullOrEmpty(filetype))
                //    {
                //        if (filetype.Contains("jpeg") || filetype.Contains("jpg"))
                //        {
                //            if (fileBytes != null && fileBytes.Length > 0)
                //            {
                //                string dateNow = DateTimeNow.ToString();
                //                dateNow = dateNow.Replace(" ", string.Empty);
                //                dateNow = dateNow.Replace(":", "_");
                //                dateNow = dateNow.Replace("-", "_");
                //                if (dateNow.Contains('/'))
                //                    dateNow = dateNow.Replace("/", "_");

                //                Helper.CreateFolder(HttpContext.Current.Server.MapPath("~/Images/"));

                //                string filePath = HttpContext.Current.Server.MapPath("~/Images/" + string.Format("img_{0}_{1}_{2}_{3}.jpg", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow));
                //                //byte[] imageBytes = Convert.FromBase64String(tblUserConversation.Image);
                //                MemoryStream ms = new MemoryStream(fileBytes, 0, fileBytes.Length);
                //                ms.Write(fileBytes, 0, fileBytes.Length);
                //                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                //                image.Save(filePath);
                //                //string serverPath = "http://" + Request.RequestUri.Authority + ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}_{1}_{2}_{3}.jpg", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow);
                //                string serverPath = ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}_{1}_{2}_{3}.jpg", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow);
                //                tblUserConversation.Image = serverPath;
                //            }
                //        }
                //        else if (filetype.Contains("pdf"))
                //        {
                //            if (fileBytes != null && fileBytes.Length > 0)
                //            {
                //                string dateNow = DateTimeNow.ToString();
                //                dateNow = dateNow.Replace(" ", string.Empty);
                //                dateNow = dateNow.Replace(":", "_");
                //                dateNow = dateNow.Replace("-", "_");
                //                if (dateNow.Contains('/'))
                //                    dateNow = dateNow.Replace("/", "_");

                //                Helper.CreateFolder(HttpContext.Current.Server.MapPath("~/Documents/"));

                //                string filePath = HttpContext.Current.Server.MapPath("~/Documents/" + string.Format("doc_{0}_{1}_{2}_{3}.pdf", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow));
                //                //byte[] imageBytes = Convert.FromBase64String(tblUserConversation.Image);
                //                MemoryStream ms = new MemoryStream(fileBytes, 0, fileBytes.Length);
                //                File.WriteAllBytes(filePath, fileBytes);
                //                //ms.Write(fileBytes, 0, fileBytes.Length);
                //                //System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                //                //image.Save(filePath);
                //                //string serverPath = "http://" + Request.RequestUri.Authority + ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}_{1}_{2}_{3}.jpg", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow);
                //                string serverPath = ConfigurationManager.AppSettings["DocumentPath"] + string.Format("doc_{0}_{1}_{2}_{3}.pdf", tblUserConversation.QueryId, tblUserConversation.CustID, tblUserConversation.IsDealer, dateNow);
                //                tblUserConversation.Image = serverPath;
                //            }
                //        }
                //        else
                //        {
                //            Helper.LogError("Invalid File Type", null, null, null);
                //        }
                //    }
                //}



                //var ctx = HttpContext.Current;
                //var root = ctx.Server.MapPath("~/Images");
                //var provider =
                //    new MultipartFormDataStreamProvider(root);
                //await Request.Content.ReadAsMultipartAsync(provider);

                //foreach(var file in provider.FileData)
                //{
                //    var name = file.Headers.ContentDisposition.FileName;
                //    name = name.Trim('"');
                //    name = name + DateTime.Now;
                //    var localFileName = file.LocalFileName;
                //    var filePath = Path.Combine(root, name);
                //    File.Move(localFileName, filePath);
                //    string serverPath = "http://" + Request.RequestUri.Authority + ConfigurationManager.AppSettings["ImagePath"] + name;
                //    tblUserConversation.Image = serverPath;
                //}
                #endregion

                tblUserConversation result = jbndbclass.SendMessage(tblUserConversation);
                if (result == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        public string GetSafeFilename(string filename)
        {
            return string.Join("", filename.Split(Path.GetInvalidFileNameChars()));
        }

        [HttpGet]
        [ResponseType(typeof(SendMessageParameters))]
        [Route("GetMessages/{CustID}/{QueryId}/{ReceiverID}")]
        public IHttpActionResult GetMessages(int CustID, int QueryId, int ReceiverID)
        {
            try
            {
                if (IsLogWrite == 1)
                    Helper.TransactionLog(CustID + "**" + QueryId + "**" + ReceiverID, 0);

                SendMessageParameters sendMessageParameters = jbndbclass.GetMessages(CustID, QueryId, ReceiverID);
                if (sendMessageParameters == null)
                {
                    return NotFound();
                }
                else
                {
                    string URL = GetBaseUrl();
                    Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);


                    if (!string.IsNullOrEmpty(sendMessageParameters.Image))
                    {
                        Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(sendMessageParameters.Image));
                        sendMessageParameters.Image = URL + sendMessageParameters.Image;
                    }
                    if (!string.IsNullOrEmpty(sendMessageParameters.Image2))
                    {
                        Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(sendMessageParameters.Image2));
                        sendMessageParameters.Image2 = URL + sendMessageParameters.Image2;
                    }
                    //for(int i=0; i< sendMessageParameters.MessageList.Count; i++)
                    //{
                    //    if(sendMessageParameters.mess)
                    //}

                    foreach (var item in sendMessageParameters.MessageList)
                    {
                        if (!string.IsNullOrEmpty(item.Image))
                        {
                            string[] splitImage = item.Image.Split('/');
                            item.FileName = splitImage[3];
                            Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.Image));
                            item.Image = URL + item.Image;
                            if (item.Image.Contains(".pdf"))
                            {
                                item.FileType = "pdf";
                            }
                            else
                            {
                                item.FileType = "image";
                            }
                        }
                    }
                }
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(sendMessageParameters);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 1);

                //return Ok(sendMessageParameters);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, sendMessageParameters));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpGet]
        [ResponseType(typeof(CustomerQueries))]
        [Route("GetCustomerQueries/{CustID}/{IsFavorite}")]
        public IHttpActionResult GetCustomerQueries(int CustID, int IsFavorite)
        {
            try
            {
                if (IsLogWrite == 1)
                    Helper.TransactionLog(CustID + "**" + IsFavorite, 0);
                List<CustomerQueries> QueryList = jbndbclass.GetCustomerQueries(CustID, IsFavorite);
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(QueryList);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 1);
                if (QueryList == null)
                {
                    return NotFound();
                }
                foreach (var item in QueryList)
                {
                    if (!string.IsNullOrEmpty(item.SenderImage))
                    {
                        string URL = GetBaseUrl();
                        Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                        Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.SenderImage));
                        item.SenderImage = URL + item.SenderImage;
                    }
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, QueryList));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(CustomerQueries))]
        [Route("FilterCustomerQueries")]
        public IHttpActionResult FilterCustomerQueries(SearchParameters searchParameters)
        {
            try
            {
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(searchParameters);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 1);
                List<CustomerQueries> QueryList = jbndbclass.FilterCustomerQueries(searchParameters);
                string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(QueryList);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json2, 0);
                if (QueryList == null)
                {
                    return NotFound();
                }
                foreach (var item in QueryList)
                {
                    if (!string.IsNullOrEmpty(item.SenderImage))
                    {
                        string URL = GetBaseUrl();
                        Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                        Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.SenderImage));
                        item.SenderImage = URL + item.SenderImage;
                    }
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, QueryList));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(CustomerQueries))]
        [Route("DeleteConversation/{CustID}")]
        public IHttpActionResult DeleteConversation(int CustID, [FromBody] List<DeleteConversations> ConversationList)
        {
            List<CustomerQueries> obj = jbndbclass.DeleteConversation(CustID, ConversationList);

            //if(obj == null || obj.Count <= 0)
            //{
            //    return NotFound();
            //}
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, obj));
        }

        //Delete single chat 22-10-2020
        [HttpPost]
        [ResponseType(typeof(SendMessageParameters))]
        [Route("DeleteChat/{CustID}")]
        public IHttpActionResult DeleteChat(int CustID, [FromBody] List<DeleteChat> ConversationList)
        {
            string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(ConversationList);
            if (IsLogWrite == 1)
                Helper.TransactionLog(json2, 0);

            SendMessageParameters obj = jbndbclass.DeleteChat(CustID, ConversationList);

            //if(obj == null || obj.Count <= 0)
            //{
            //    return NotFound();
            //}
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, obj));
        }

        [HttpPost]
        [ResponseType(typeof(CustomerQueries))]
        [Route("AddOrDeleteFavorite/{CustID}/{QueryId}/{ReceiverID}/{IsFavorite}")]
        public IHttpActionResult AddOrDeleteFavorite(int CustID, int QueryId, int ReceiverID, int IsFavorite)
        {
            List<CustomerQueries> obj = jbndbclass.AddOrDeleteFavorite(CustID, QueryId, ReceiverID, IsFavorite);

            //if(obj == null || obj.Count <= 0)
            //{
            //    return NotFound();
            //}
            //return Ok(obj);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, obj));
        }

        //old one
        //public IHttpActionResult SubmitCustomerQuery([FromBody] SubmitQuery submitQuery)
        //{
        //    string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(submitQuery);
        //    loggerBlock.LogWriter.Write(json1, "");
        //    try
        //    {
        //        int randomNumber = 0;
        //        Random r = new Random();
        //        if (submitQuery.ProductPhoto != null)
        //        {
        //            randomNumber = r.Next();
        //            string filePath = HttpContext.Current.Server.MapPath("~/Images/" + string.Format("img_{0}.jpg", randomNumber));
        //            byte[] imageBytes = Convert.FromBase64String(submitQuery.ProductPhoto);
        //            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
        //            ms.Write(imageBytes, 0, imageBytes.Length);
        //            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
        //            image.Save(filePath);
        //            string serverPath = "http://" + Request.RequestUri.Authority + ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}.jpg", randomNumber);
        //            submitQuery.ProductPhoto = serverPath;
        //        }

        //        if (submitQuery.ProductPhoto2 != null)
        //        {
        //            randomNumber = r.Next();
        //            string filePath = HttpContext.Current.Server.MapPath("~/Images/" + string.Format("img_{0}.jpg", randomNumber));
        //            byte[] imageBytes = Convert.FromBase64String(submitQuery.ProductPhoto2);
        //            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
        //            ms.Write(imageBytes, 0, imageBytes.Length);
        //            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
        //            image.Save(filePath);
        //            string serverPath = "http://" + Request.RequestUri.Authority + ConfigurationManager.AppSettings["ImagePath"] + string.Format("img_{0}.jpg", randomNumber);
        //            submitQuery.ProductPhoto2 = serverPath;
        //        }

        //        jbndbclass.SubmitCustomerQuery(submitQuery);
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, submitQuery));
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, true);
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
        //    }
        //}

        [HttpPost]
        [Route("SendImage")]
        public bool SendImage(ImageContents ImageContents)
        {
            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(ImageContents);
            if (IsLogWrite == 1)
                Helper.LogError(json1, null, null, null, false);
            bool result = jbndbclass.SendImage(ImageContents.Image);
            return result;
        }

        [HttpPost]
        [Route("SendMailWithFile")]
        public async Task<IHttpActionResult> SendMailWithFile()
        {
            try
            {
                byte[] imagesBytes1 = null;
                var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
                var json = await filesReadToProvider.Contents[0].ReadAsStringAsync();
                if (filesReadToProvider.Contents.Count > 1)
                {
                    imagesBytes1 = await filesReadToProvider.Contents[1].ReadAsByteArrayAsync();
                }

                Attachment MailAttachment = new Attachment(new MemoryStream(imagesBytes1), "MailAttachment1.jpg");
                List<Attachment> MailAttachments = new List<Attachment>();
                MailAttachments.Add(MailAttachment);



                loggerBlock.LogWriter.Write(json, "");

                SendMailParameters sendMailParameters = JsonConvert.DeserializeObject<SendMailParameters>(json);

                CustomerDetails customerDetails = jbndbclass.GetCustomerDetails(sendMailParameters.CustID);
                if (customerDetails == null)
                {
                    return NotFound();
                }
                sendMailParameters.FirmName = customerDetails.FirmName;
                sendMailParameters.EmailID = customerDetails.EmailID;
                sendMailParameters.MobileNumber = customerDetails.MobileNumber;
                sendMailParameters.CityName = customerDetails.city.VillageLocalityName;
                string Result = jbndbclass.SendMails(sendMailParameters, MailAttachments);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [Route("SendMail")]
        public IHttpActionResult SendMail([FromBody] SendMailParameters sendMailParameters)
        {
            try
            {
                CustomerDetails GetData = jbndbclass.GetCustomerDetails(sendMailParameters.CustID);

                if (GetData == null)
                {
                    return NotFound();
                }
                sendMailParameters.FirmName = GetData.FirmName;
                sendMailParameters.EmailID = GetData.EmailID;
                sendMailParameters.MobileNumber = GetData.MobileNumber;
                sendMailParameters.CityName = GetData.city.VillageLocalityName;
                string Result = jbndbclass.SendMails(sendMailParameters);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpGet]
        [ResponseType(typeof(tblPromo))]
        [Route("GetPromoImages")]
        public IHttpActionResult GetPromoImages()
        {
            try
            {
                List<tblPromo> PromoImages = new List<tblPromo>();
                PromoImages = jbndbclass.GetPromoImages();
                if (PromoImages == null)
                {
                    return NotFound();
                }
                if (PromoImages != null && PromoImages.Count > 0)
                {
                    foreach (var item in PromoImages)
                    {
                        if (!string.IsNullOrEmpty(item.ImageURL))
                        {
                            string URL = GetBaseUrl();
                            Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                            Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.ImageURL));
                            item.ImageURL = URL + item.ImageURL;
                        }
                    }
                }
                //return Ok(sendMessageParameters);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, PromoImages));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
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

        //Feature added 24th Aug 2020
        //Get the list of Sent Enquiries
        [HttpPost]
        [ResponseType(typeof(Enquiries))]
        [Route("GetEnquiries")]
        public IHttpActionResult GetEnquiries(SentEnquirySearchParameters sentEnquirySearchParameters)
        {
            try
            {
                List<Enquiries> QueryList = jbndbclass.GetEnquiries(sentEnquirySearchParameters);
                if (QueryList == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, QueryList));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //Get the list of conversations of an enquiry
        [HttpGet]
        [ResponseType(typeof(CustomerQueries))]
        [Route("GetConversationsOfEnquiry/{QueryID}/{CustID}/{ReceiverID}/{IsFavorite}")]
        public IHttpActionResult GetConversationsOfEnquiry(int QueryID, int CustID, int ReceiverID, int IsFavorite)
        {
            try
            {
                List<CustomerQueries> QueryList = jbndbclass.GetConversationsOfEnquiry(QueryID, CustID, ReceiverID, IsFavorite);
                if (QueryList == null)
                {
                    return NotFound();
                }
                foreach (var item in QueryList)
                {
                    if (!string.IsNullOrEmpty(item.SenderImage))
                    {
                        string URL = GetBaseUrl();
                        Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                        Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.SenderImage));
                        item.SenderImage = URL + item.SenderImage;
                    }
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, QueryList));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get the list of customers for filter options
        [HttpGet]
        [ResponseType(typeof(Enquiries))]
        [Route("GetCustomerNamesForFilter/{CustID}")]
        public IHttpActionResult GetCustomerNamesForFilter(int CustID)
        {
            try
            {
                List<CustomerListForSearch> CustomerList = jbndbclass.GetCustomerNamesForFilter(CustID);
                if (CustomerList == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, CustomerList));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //Filter Sent Enquiries
        [HttpPost]
        [ResponseType(typeof(CustomerQueries))]
        [Route("FilterCustomerSentEnquiries")]
        public IHttpActionResult FilterCustomerSentEnquiries(SentEnquirySearchParameters sentEnquirySearchParameters)
        {
            try
            {
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(sentEnquirySearchParameters);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 1);
                List<CustomerQueries> QueryList = jbndbclass.FilterCustomerSentEnquiries(sentEnquirySearchParameters);
                if (QueryList == null)
                {
                    return NotFound();
                }
                foreach (var item in QueryList)
                {
                    if (!string.IsNullOrEmpty(item.SenderImage))
                    {
                        string URL = GetBaseUrl();
                        Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                        Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.SenderImage));
                        item.SenderImage = URL + item.SenderImage;
                    }
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, QueryList));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(CustomerQueries))]
        [Route("DeleteEnquiry/{CustID}")]
        public IHttpActionResult DeleteEnquiry(int CustID, [FromBody] List<DeleteEnquiry> ConversationList)
        {
            List<Enquiries> obj = jbndbclass.DeleteEnquiry(CustID, ConversationList);

            //if(obj == null || obj.Count <= 0)
            //{
            //    return NotFound();
            //}
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, obj));
        }

        [HttpPost]
        [ResponseType(typeof(CustomerQueries))]
        [Route("GetFavoriteConversations")]
        public IHttpActionResult GetFavoriteConversations(SearchParameters searchParameters)
        {
            try
            {
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(searchParameters);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 1);
                List<FavoriteConversations> QueryList = jbndbclass.GetFavoriteConversations(searchParameters);
                string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(QueryList);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json2, 0);
                if (QueryList == null)
                {
                    return NotFound();
                }
                foreach(var item in QueryList)
                {
                    if (!string.IsNullOrEmpty(item.SenderImage))
                    {
                        string URL = GetBaseUrl();
                        Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri);
                        Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(item.SenderImage));
                        item.SenderImage = URL + item.SenderImage;
                    }
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, QueryList));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [ResponseType(typeof(City))]
        [Route("GetCityOfSentEnquiries/{CustID}")]
        public IHttpActionResult GetCityOfSentEnquiries(int CustID)
        {
            //string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(submitQuery);
            loggerBlock.LogWriter.Write("user id is:", CustID.ToString());
            try
            {
                List<City> cities = jbndbclass.GetCityOfSentEnquiries(CustID);
                if (cities != null)
                    return Ok(cities);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [ResponseType(typeof(City))]
        [Route("GetFavoriteCities/{CustID}")]
        public IHttpActionResult GetFavoriteCities(int CustID)
        {
            //string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(submitQuery);
            loggerBlock.LogWriter.Write("user id is:", CustID.ToString());
            try
            {
                List<City> cities = jbndbclass.GetFavoriteCities(CustID);
                if (cities != null)
                    return Ok(cities);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [ResponseType(typeof(SubCategoryTypes))]
        [Route("GetAllSubCategoryList")]
        public IHttpActionResult GetAllSubCategoryList()
        {
            try
            {
                List<SubCategoryTypes> GetAllSubCategories = jbndbclass.GetAllSubCategoryList();
                if (GetAllSubCategories != null)
                    return Ok(GetAllSubCategories);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [ResponseType(typeof(ChildCategories))]
        [Route("GetChildCatagories/{SubCategoryID}")]
        public IHttpActionResult GetChildCatagories(int SubCategoryID)
        {
            try
            {
                List<ChildCategories> GetAllChildCategories = jbndbclass.GetChildCatagories(SubCategoryID);
                if (GetAllChildCategories != null)
                    return Ok(GetAllChildCategories);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [Route("SendItemRequestWithFile")]
        public async Task<IHttpActionResult> SendItemRequestWithFile()
        {
            try
            {
                byte[] imagesBytes1 = null, imagesBytes2 = null;
                var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
                var json = await filesReadToProvider.Contents[0].ReadAsStringAsync();
                if (filesReadToProvider.Contents.Count > 1)
                {
                    if (filesReadToProvider.Contents.Count == 3)
                    {
                        imagesBytes1 = await filesReadToProvider.Contents[1].ReadAsByteArrayAsync();
                        imagesBytes2 = await filesReadToProvider.Contents[2].ReadAsByteArrayAsync();
                    }
                    else
                    {
                        imagesBytes1 = await filesReadToProvider.Contents[1].ReadAsByteArrayAsync();
                    }
                }

                Attachment attachment1 = new Attachment(new MemoryStream(imagesBytes1), "MailAttachment1.jpg");
                Attachment attachment2 = new Attachment(new MemoryStream(imagesBytes2), "MailAttachment2.jpg");

                List<Attachment> MailAttachments = new List<Attachment>();
                MailAttachments.Add(attachment1);
                MailAttachments.Add(attachment2);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json, 0);

                SendMailParameters sendMailParameters = JsonConvert.DeserializeObject<SendMailParameters>(json);

                CustomerDetails customerDetails = jbndbclass.GetCustomerDetails(sendMailParameters.CustID);
                if (customerDetails == null)
                {
                    return NotFound();
                }
                sendMailParameters.FirmName = customerDetails.FirmName;
                sendMailParameters.EmailID = customerDetails.EmailID;
                sendMailParameters.MobileNumber = customerDetails.MobileNumber;
                sendMailParameters.CityName = customerDetails.city.VillageLocalityName;
                string Result = jbndbclass.RequestForItemCategory(sendMailParameters, MailAttachments);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, Result));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpGet]
        [ResponseType(typeof(tblBusinessDemand))]
        [Route("GetBusinessDemands")]
        public IHttpActionResult GetBusinessDemands()
        {
            try
            {
                List<tblBusinessDemand> BusinessDemands = new List<tblBusinessDemand>();
                BusinessDemands = jbndbclass.GetBusinessDemands();
                if (BusinessDemands == null)
                {
                    return NotFound();
                }

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, BusinessDemands));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //Implemented on 5th October 2020
        [HttpGet]
        [ResponseType(typeof(tblProfessionalRequirement))]
        [Route("GetProfessionalRequirements")]
        public IHttpActionResult GetProfessionalRequirements()
        {
            try
            {
                List<tblProfessionalRequirement> professionalRequirements = new List<tblProfessionalRequirement>();
                professionalRequirements = jbndbclass.GetProfessionalRequirements();
                if (professionalRequirements == null)
                {
                    return NotFound();
                }

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, professionalRequirements));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //get dashboard data (count)
        [HttpGet]
        [ResponseType(typeof(Dashboard))]
        [Route("GetDashboardData/{CustID}")]
        public IHttpActionResult GetDashboardData(int CustID)
        {
            try
            {
                Dashboard DashboardData = jbndbclass.GetDashboardData(CustID);
                if (DashboardData == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, DashboardData));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //Mark as Read / Unread Conversations

        [ResponseType(typeof(DealersDetails))]
        [Route("PostMessageReadUnread")]
        public IHttpActionResult PostMessageReadUnread([FromBody] List<DealersDetails> dealersDetails)
        {
            try
            {
                string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(dealersDetails);
                if (IsLogWrite == 1)
                    Helper.TransactionLog(json1, 1);

                List<DealersDetails> DealersDetailslist = jbndbclass.MarkAsReadUnread(dealersDetails);
                if (DealersDetailslist == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, DealersDetailslist));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //01-02-2021
        //Item List in one to one enquiry

        //Get State List for a customer
        [ResponseType(typeof(State))]
        [Route("GetStatesListForUser/{CustID}")]
        public IHttpActionResult GetStatesListForUser(int CustID)
        {
            try
            {
                List<State> states = new List<State>();
                states = jbndbclass.GetStatesListForUser(CustID);

                if (states == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, states));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        //Get CityList For an user
        [ResponseType(typeof(City))]
        [Route("GetCitiesListForUser/{CustID}/{StateID}")]
        public IHttpActionResult GetCitiesListForUser(int CustID, int StateID)
        {
            try
            {
                List<City> cities = new List<City>();
                cities = jbndbclass.GetCitiesListForUser(CustID, StateID);

                if (cities == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, cities));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [ResponseType(typeof(ProductsView))]
        [Route("GetItemNamesForUser/{CustID}")]
        public IHttpActionResult GetItemNamesForUser(int CustID)
        {
            try
            {
                List<ChildCategories> childcategories = new List<ChildCategories>();
                childcategories = jbndbclass.GetItemNamesForUser(CustID);

                if (childcategories == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, childcategories));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        //Get Business Types for an item selected
        [ResponseType(typeof(BusinessTypes))]
        [Route("GetBusinessTypesForItem/{ItemID}")]
        public IHttpActionResult GetBusinessTypesForItem(int ItemID)
        {
            try
            {
                List<BusinessTypes> businessTypes = new List<BusinessTypes>();
                businessTypes = jbndbclass.GetBusinessTypesForItem(ItemID);

                if (businessTypes == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, businessTypes));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        //Get Replier List for an user based on business type
        [HttpPost]
        [ResponseType(typeof(CustIDS))]
        [Route("GetReplierList")]
        public IHttpActionResult GetReplierList([FromBody] CustDetails custDetails)
        {
            try
            {
                List<CustIDS> custIDs = new List<CustIDS>();
                custIDs = jbndbclass.GetReplierList(custDetails);

                if (custIDs == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, custIDs));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(BusinessTypes))]
        [Route("GetBusinessTypelistFor_OnePlus")]
        public IHttpActionResult GetBusinessTypelistFor_OnePlus([FromBody] CustDetails custDetails)
        {
            try
            {
                List<BusinessTypes> BusinessTypelist = new List<BusinessTypes>();
                BusinessTypelist = jbndbclass.GetBusinessTypelistFor_OnePlus(custDetails);

                if (BusinessTypelist == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, BusinessTypelist));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(CustIDS))]
        [Route("GetLoadEnquiryStatelistOnePlus")]
        public IHttpActionResult GetLoadEnquiryStatelistOnePlus([FromBody] CustDetails custDetails)
        {
            try
            {
                List<State> StateEnqlist = new List<State>();
                StateEnqlist = jbndbclass.GetLoadEnquiry_StatelistOnePlus(custDetails);

                if (StateEnqlist == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, StateEnqlist));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(CustIDS))]
        [Route("GetLoadEnquiry_CitylistOnePlus")]
        public IHttpActionResult GetLoadEnquiry_CitylistOnePlus([FromBody] CustDetails custDetails)
        {
            try
            {
                List<City> CityEnqlist = new List<City>();
                CityEnqlist = jbndbclass.GetLoadEnquiry_CitylistOnePlus(custDetails);

                if (CityEnqlist == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, CityEnqlist));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [ResponseType(typeof(CustIDS))]
        [Route("FilterEnquiryList")]
        public IHttpActionResult FilterEnquiryList([FromBody] CustDetails custDetails)
        {
            try
            {
                List<CustIDS> custIDs = new List<CustIDS>();
                custIDs = jbndbclass.FilterEnquiryList(custDetails);

                if (custIDs == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, custIDs));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [ResponseType(typeof(EnquiryTypes))]
        [Route("GetEnquiryTypes")]
        public IHttpActionResult GetEnquiryTypes()
        {
            try
            {
                List<EnquiryTypes> enquiryTypes = new List<EnquiryTypes>();
                enquiryTypes = jbndbclass.GetEnquiryTypes();

                if (enquiryTypes == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, enquiryTypes));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        private void SendNotificationForDN(Notification Datacontext)
        {
            try
            {
                var applicationID = ConfigurationManager.AppSettings["APIKey"];
                var senderId = ConfigurationManager.AppSettings["SenderId"]; //"424344187672";
                //var senderId = ConfigurationManager.AppSettings["424344187672"]; //"424344187672";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                string DeviceId = string.Empty;
                string respsone = string.Empty;

                #region notification 
                
                var msgdata = new
                {
                    to = DeviceId, // Uncoment this if you want to test for single device
                    priority = "high",
                    Text_available = true,
                    //var xyz=(from tblGatePass)
                    notification = new
                    {
                        //Status:GP | Name  : dggh | OrderNumber : GP12345 | TransNumber : DN123 | TotalItemCount : 0
                        title = Datacontext.Title,
                        body = Datacontext.Body,
                        badge = 1,
                        sound = "default"
                    }
                };
                Serialization(applicationID, senderId, msgdata);
                #endregion
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
            }
        }
        private void Serialization(string applicationID, string senderId, object msgdata)
        {
            try
            {
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(msgdata);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                                Helper.TransactionLog(str, 1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
            }
        }

        [HttpGet]
        [ResponseType(typeof(PushNotifications))]
        [Route("GetPushNotifications/{CustID}")]
        public IHttpActionResult GetPushNotifications(int CustID)
        {
            try
            {
                List<PushNotifications> pushNotifications = new List<PushNotifications>();
                pushNotifications = jbndbclass.GetPushNotifications(CustID);

                if (pushNotifications == null)
                {
                    return NotFound();
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, pushNotifications));
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
    }
}

using JBNAdminPortal.Models;
using JBNClassLibrary;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace JBNAdminPortal.Controllers
{
    public class ItemCategoryController : Controller
    {
        // GET: ItemCategory
        DLItemCategory DAL = new DLItemCategory();


        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                List<ItemCategory> itemCategories = new List<ItemCategory>();
                return View(itemCategories.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            DLChildCategory dLChildCategory = new DLChildCategory();
            ViewBag.ChildCategory = new SelectList(dLChildCategory.GetChildCatList(), "ID", "ChildCategoryName");
            return PartialView();
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ProductImages"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemCategory item, HttpPostedFileBase[] ProductImages)
        {
            if(Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    List<string> strList = new List<string>();
                    if(ProductImages != null && ProductImages.Length > 0)
                    {
                        for (int i = 0; i < ProductImages.Length; i++)
                        {
                            if (ProductImages[i] != null)
                            {
                                var InputFileName = Path.GetFileName(ProductImages[i].FileName);
                                System.IO.Stream fs = ProductImages[i].InputStream;
                                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                                Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                                string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                                base64String = "data:image/png;base64," + base64String;
                                strList.Add(base64String);
                            }
                        }
                    }

                    item.strProductImages = strList;
                    var Result = DAL.SaveItemCategory(item, Convert.ToInt32(Session["UserID"]));
                    ModelState.Clear();
                    DLChildCategory dLChildCategory = new DLChildCategory();
                    ViewBag.ChildCategory = new SelectList(dLChildCategory.GetChildCatList(), "ID", "ChildCategoryName");
                    return Json(Result.DisplayMsg);
                }
                else
                {
                    List<string> Errors = new List<string>();
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            Errors.Add(error.ErrorMessage);
                        }
                    }
                    ModelState.AddModelError("", Errors.ToString());

                    return HttpNotFound("Your request did not find.");
                }
            }
            else
            {
                return Json("sessionexpired");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public ActionResult Edit(int ItemID)
        {
            if (ItemID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemCategory category = DAL.GetItemCategoryDetail(ItemID);
            if (category == null)
            {
                return HttpNotFound();
            }
            DLChildCategory dLChildCategory = new DLChildCategory();
            ViewBag.ChildCategory = new SelectList(dLChildCategory.GetChildCatList(), "ID", "ChildCategoryName");

            return PartialView(category);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ProductImages"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ItemCategory item, HttpPostedFileBase[] ProductImages)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    var Result = DAL.SaveItemCategory(item, Convert.ToInt32(Session["UserID"].ToString()));
                    List<ItemCategory> itemCategories = new List<ItemCategory>();
                    itemCategories = DAL.GetItemCatList();
                    return Json(Result.DisplayMsg);
                }
                else
                {
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            ModelState.AddModelError("", error.ToString());
                        }
                    }
                    return HttpNotFound("Your request did not find.");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public ActionResult GetMainCategory(int selectedValue)
        {
            DLChildCategory childCat = new DLChildCategory();

            string result = childCat.GetMainCategoryName(selectedValue);

            return Json(result);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult Search()
        {
            if (Session["UserID"] != null)
            {
                ItemCategory itemCategory = new ItemCategory();
                return PartialView(itemCategory);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ActionResult SearchByItemName(ItemCategory item)
        {
            if (Session["UserID"] != null)
            {
                List<ItemCategory> itemCategories = new List<ItemCategory>();
                itemCategories = DAL.GetAllItemCatList(item).ToList();
                return PartialView("ItemCategoryList", itemCategories);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        public ActionResult ItemCategoryList()
        {
            if (Session["UserID"] != null)
            {
                List<ItemCategory> itemCategories = new List<ItemCategory>();
                itemCategories = DAL.GetItemCatList();

                DashBoardData DashboardDAL = new DashBoardData();
                object CategoriesData = DashboardDAL.GetAllProductsData();

                System.Reflection.PropertyInfo Mpi = CategoriesData.GetType().GetProperty("MainCategories");
                ViewBag.MainCategories = (int)(Mpi.GetValue(CategoriesData, null));

                System.Reflection.PropertyInfo Spi = CategoriesData.GetType().GetProperty("SubCategories");
                ViewBag.SubCategories = (int)(Spi.GetValue(CategoriesData, null));

                System.Reflection.PropertyInfo Cpi = CategoriesData.GetType().GetProperty("ChildCategories");
                ViewBag.ChildCategories = (int)(Cpi.GetValue(CategoriesData, null));

                System.Reflection.PropertyInfo Ipi = CategoriesData.GetType().GetProperty("ItemCategories");
                ViewBag.ItemCategories = (int)(Ipi.GetValue(CategoriesData, null));


                return PartialView(itemCategories);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ItemCategoryListJson()
        {
            if (Session["UserID"] != null)
            {
                List<ItemCategory> itemCategories = new List<ItemCategory>();
                itemCategories = DAL.GetItemCatList();
                return new JsonResult()
                {
                    Data = itemCategories,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
                //return Json(new { data = itemCategories, draw = Request["draw"], recordsTotal = itemCategories.Count() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0");
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="ChildCategoryID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetParentCategories(int? ChildCategoryID)
        {
            ItemCategory itemCategory = new ItemCategory();
            itemCategory = DAL.GetParentCategories(ChildCategoryID);
            return Json(itemCategory, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="ItemName"></param>
        /// <returns></returns>
        public JsonResult CheckDuplicateName(string ItemName)
        {
            bool Result = DAL.CheckDuplicateName(ItemName);
            if (Result == true)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult excelexport()
        {

            DataTable dt = new DataTable();

            dt = DAL.GetAllCategories();
            string heading = "";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage workbook = new ExcelPackage();

            ExcelWorksheet workSheet = workbook.Workbook.Worksheets.Add(String.Format("{0} Data", heading));
            int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 2;


            DataColumn dataColumn = dt.Columns.Add("Sr.No", typeof(int));

            dataColumn.SetOrdinal(0);
            int index = 1;
            foreach (DataRow item in dt.Rows)
            {
                item[0] = index;
                index++;
            }

            // Do something to populate your workbook
            workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dt, true);


            if (!String.IsNullOrEmpty(heading))
            {
                workSheet.Cells["A1"].Value = heading;
                workSheet.Cells["A1"].Style.Font.Size = 20;

                workSheet.InsertColumn(1, 1);
                workSheet.InsertRow(1, 1);
                workSheet.Column(1).Width = 5;
            }
            // Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                memoryStream.Position = 0;
                TempData[handle] = memoryStream.ToArray();
            }
            // Note we are returning a filename as well as the handle
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = "AllCategories.xlsx" }
            };
        }
        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="fileGuid"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}
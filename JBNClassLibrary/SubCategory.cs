using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class SubCat
    {
        public int ID { get; set; }
        [Required(ErrorMessage ="Please select Main Category")]
        public int CategoryProductID { get; set; }
        [Required(ErrorMessage = "Please enter Sub Category Name")]
        public string SubCategoryName { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string MainCategoryName { get; set; }
        [Required(ErrorMessage = "Please enter Reffered by or Reason to create this item")]
        public string RefferedByOrReason { get; set; }
        [Required(ErrorMessage = "Please enter the name of the person who approved this product")]
        public string ApprovedBy { get; set; }
        public string CreatedByName { get; set; }
        public bool IsActive { get; set; }
        public bool IsRejected { get; set; }
        public bool IsMasterProduct { get; set; }
        public List<CategoryHistory> histories { get; set; }

    }
    public class SubCategory
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");        
        public List<SubCat> GetSubCatList()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<SubCat> SubCatList = new List<SubCat>();
                    SubCatList = (from sc in dbContext.tblSubCategories
                                  join c in dbContext.tblCategoryProducts on sc.CategoryProductID equals c.ID
                                  join uc in dbContext.tblUsers on sc.CreatedBy equals uc.ID
                                  //where u.IsActive == true
                                  select new SubCat
                                  {
                                      ID = sc.ID,
                                      CategoryProductID = sc.CategoryProductID.Value,
                                      SubCategoryName = sc.SubCategoryName.Trim(),
                                      CreatedDate = sc.CreatedDate,
                                      CreatedBy = sc.CreatedBy,
                                      MainCategoryName = c.MainCategoryName,
                                      RefferedByOrReason = sc.RefferedByOrReason,
                                      ApprovedBy = sc.ApprovedBy,
                                      IsMasterProduct = sc.IsMasterProduct,
                                      CreatedByName = uc.FullName,
                                      IsActive = sc.IsActive,
                                      IsRejected = sc.IsRejected
                                  }).ToList();
                    return SubCatList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public SubCat GetSubCategoryDetailForApprove(int ID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    //tblCategoryProduct product = new tblCategoryProduct();
                    SubCat category = new SubCat();
                    category = (from sc in dbContext.tblSubCategories
                                join c in dbContext.tblCategoryProducts on sc.CategoryProductID equals c.ID
                                join uc in dbContext.tblUsers on sc.CreatedBy equals uc.ID
                                where sc.ID == ID && sc.IsActive == false && sc.IsRejected == false
                                select new SubCat
                                {
                                    ID = sc.ID,
                                    CategoryProductID = c.CategoryProductID,
                                    SubCategoryName = sc.SubCategoryName.Trim(),
                                    CreatedDate = sc.CreatedDate,
                                    CreatedBy = sc.CreatedBy,
                                    MainCategoryName = c.MainCategoryName,
                                    RefferedByOrReason = sc.RefferedByOrReason,
                                    ApprovedBy = sc.ApprovedBy,
                                    CreatedByName = uc.FullName,
                                }).FirstOrDefault();

                    //get history
                    List<CategoryHistory> histories = new List<CategoryHistory>();
                    histories = (from h in dbContext.tblCategoryHistories
                                 join u in dbContext.tblUsers on h.UserID equals u.ID
                                 where h.ProductID == ID && h.ProductCategory == "SubCategory"
                                 select new CategoryHistory
                                 {
                                     OldProductName = h.OldProductName,
                                     OldMainCategory = h.OldMainCategory,
                                     CreatedUser = u.FullName,
                                     CreationDate = h.CreatedDate
                                 }).OrderByDescending(ch => ch.CreationDate).ToList();

                    category.histories = histories;

                    return category;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public SubCat GetSubCategoryDetail(int ID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    //tblCategoryProduct product = new tblCategoryProduct();
                    SubCat category = new SubCat();
                    category = (from sc in dbContext.tblSubCategories
                                join c in dbContext.tblCategoryProducts on sc.CategoryProductID equals c.ID
                                join uc in dbContext.tblUsers on sc.CreatedBy equals uc.ID
                                where sc.ID == ID
                                select new SubCat
                                {
                                    ID = sc.ID,
                                    CategoryProductID = c.ID,
                                    SubCategoryName = sc.SubCategoryName.Trim(),
                                    CreatedDate = sc.CreatedDate,
                                    CreatedBy = sc.CreatedBy,
                                    MainCategoryName = c.MainCategoryName,
                                    RefferedByOrReason = sc.RefferedByOrReason,
                                    ApprovedBy = sc.ApprovedBy,
                                    CreatedByName = uc.FullName,
                                }).FirstOrDefault();
                    return category;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public bool SaveSubCategory(SubCat SubCatDetail, int UserID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    var subcat = dbContext.tblSubCategories.AsNoTracking().Where(p => p.ID == SubCatDetail.ID).FirstOrDefault();
                    int RoleID = dbContext.tblUsers.Where(u => u.ID == UserID).FirstOrDefault().RoleID.Value;
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    if (subcat != null)
                    {
                        var SubCatOld = dbContext.tblSubCategories.Where(s => s.ID == SubCatDetail.ID).AsNoTracking().FirstOrDefault();

                        tblSubCategory obj = new tblSubCategory();
                        obj.ID = subcat.ID;
                        obj.CategoryProductID = SubCatDetail.CategoryProductID; //pro.CategoryProductID;
                        obj.SubCategoryId = SubCatDetail.ID;
                        obj.SubCategoryName = SubCatDetail.SubCategoryName;
                        obj.CreatedDate = subcat.CreatedDate;
                        obj.CreatedBy = UserID;
                        obj.RefferedByOrReason = SubCatDetail.RefferedByOrReason;
                        obj.ApprovedBy = SubCatDetail.ApprovedBy;
                        if (RoleID == 1)
                            obj.IsActive = true;
                        else
                            obj.IsActive = false;
                        obj.IsRejected = subcat.IsRejected;
                        obj.IsMasterProduct = subcat.IsMasterProduct;
                        dbContext.tblSubCategories.Add(obj);
                        dbContext.Entry(obj).State = EntityState.Modified;

                        //Insert into History Table
                        tblHistory history = new tblHistory();
                        history.UserID = UserID;
                        history.ProductID = obj.SubCategoryId;
                        history.ProductCategory = obj.SubCategoryName;
                        history.CreatedBy = UserID;
                        history.CreatedDate = DateTimeNow;
                        history.Comments = "SubCategory Updated";
                        history.ActivityPage = "subcategories";
                        history.ActivityType = "update";
                        dbContext.tblHistories.Add(history);

                        //Insert into Category History Table
                        tblCategoryHistory categoryHistory = new tblCategoryHistory();
                        categoryHistory.UserID = UserID;
                        categoryHistory.ProductID = SubCatDetail.ID;
                        categoryHistory.ProductCategory = "SubCategory";
                        categoryHistory.ProductName = SubCatDetail.SubCategoryName;
                        categoryHistory.OldProductName = subcat.SubCategoryName;
                        categoryHistory.OldMainCategory = dbContext.tblCategoryProducts.Where(c => c.CategoryProductID == subcat.CategoryProductID).FirstOrDefault().MainCategoryName;
                        categoryHistory.OldSubCategory = SubCatOld.SubCategoryName;
                        categoryHistory.CreatedBy = UserID;
                        categoryHistory.CreatedDate = DateTimeNow;
                        dbContext.tblCategoryHistories.Add(categoryHistory);

                        dbContext.SaveChanges();
                    }
                    else
                    {
                        tblSubCategory obj = new tblSubCategory();
                        obj.CategoryProductID = SubCatDetail.CategoryProductID;
                        obj.SubCategoryName = SubCatDetail.SubCategoryName;
                        obj.CreatedDate = DateTimeNow;
                        obj.CreatedBy = UserID;
                        obj.RefferedByOrReason = SubCatDetail.RefferedByOrReason;
                        obj.ApprovedBy = SubCatDetail.ApprovedBy;
                        if (RoleID == 1)
                            obj.IsActive = true;
                        else
                            obj.IsActive = false;
                        obj.IsRejected = false;
                        obj.IsMasterProduct = false;
                        dbContext.tblSubCategories.Add(obj);

                        //Insert into History Table
                        tblHistory history = new tblHistory();
                        history.UserID = UserID;
                        history.ProductID = obj.SubCategoryId;
                        history.ProductCategory = obj.SubCategoryName;
                        history.CreatedDate = DateTimeNow;
                        history.Comments = "SubCategory Created";
                        history.ActivityPage = "subcategories";
                        history.ActivityType = "create";
                        dbContext.tblHistories.Add(history);

                        dbContext.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return false;
            }
        }
        public bool CheckDuplicateName(string SubCategoryName)
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    var IsValueexists = dbcontext.tblSubCategories.Where(c => c.SubCategoryName.ToLower() == SubCategoryName.ToLower()).FirstOrDefault();

                    if (IsValueexists != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

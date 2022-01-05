using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class childcategory
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Sub Category Name is required")]
        public int SubCategoryId { get; set; }
        [Required(ErrorMessage = "Please enter child category name")]
        public string ChildCategoryName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedID { get; set; }
        public string SubCategoryName { get; set; }
        public string MainCategoryName { get; set; }
        public int CategoryProductID { get; set; }
        [Required(ErrorMessage = "Please enter Reffered by or Reason to create this item")]
        public string RefferedByOrReason { get; set; }
        [Required(ErrorMessage = "Please enter the name of the person who approved this product")]
        public string ApprovedBy { get; set; }
        public string CreatedByName { get; set; }
        public bool IsActive { get; set; }
        public bool IsChildRejected { get; set; }
        public List<CategoryHistory> histories { get; set; }
    }

    public class DLChildCategory
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public List<childcategory> GetChildCatList(childcategory childCategory)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<childcategory> ChildCatList = new List<childcategory>();
                    if (!string.IsNullOrEmpty(childCategory.MainCategoryName))
                    {
                        ChildCatList = (from u in dbContext.tblChildCategories
                                        join s in dbContext.tblSubCategories on u.SubCategoryId equals s.ID
                                        join c in dbContext.tblCategoryProducts on s.CategoryProductID equals c.ID
                                        where u.IsActive == true
                                        select new childcategory
                                        {
                                            ID = u.ID,
                                            MainCategoryName = c.MainCategoryName,
                                            SubCategoryId = s.SubCategoryId,
                                            SubCategoryName = s.SubCategoryName.Trim(),
                                            ChildCategoryName = u.ChildCategoryName,
                                            CreatedDate = u.CreatedDate,
                                            CreatedID = u.CreatedBy,
                                            RefferedByOrReason = u.RefferedByOrReason,
                                            ApprovedBy = u.ApprovedBy
                                        }).ToList();
                    }
                    else
                    {
                        ChildCatList = (from u in dbContext.tblChildCategories
                                        join s in dbContext.tblSubCategories on u.SubCategoryId equals s.ID
                                        join c in dbContext.tblCategoryProducts on s.CategoryProductID equals c.ID
                                        where u.IsActive == true
                                        select new childcategory
                                        {
                                            ID = u.ID,
                                            MainCategoryName = c.MainCategoryName,
                                            SubCategoryId = s.SubCategoryId,
                                            SubCategoryName = s.SubCategoryName.Trim(),
                                            ChildCategoryName = u.ChildCategoryName,
                                            CreatedDate = u.CreatedDate,
                                            CreatedID = u.CreatedBy,
                                            RefferedByOrReason = u.RefferedByOrReason,
                                            ApprovedBy = u.ApprovedBy
                                        }).Distinct().ToList();
                    }

                    if (!string.IsNullOrEmpty(childCategory.SubCategoryName))
                        ChildCatList = (List<childcategory>)ChildCatList.Where(c => c.SubCategoryName.ToLower().Contains(childCategory.SubCategoryName.ToLower()));

                    return ChildCatList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<childcategory> GetChildCatList()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<childcategory> ChildCatList = new List<childcategory>();
                    ChildCatList = (from cc in dbContext.tblChildCategories
                                    join sc in dbContext.tblSubCategories on cc.SubCategoryId equals sc.ID
                                    join c in dbContext.tblCategoryProducts on sc.CategoryProductID equals c.ID
                                    where cc.IsActive == true
                                    select new childcategory
                                    {
                                        ID = cc.ID,
                                        ChildCategoryName = cc.ChildCategoryName,
                                        SubCategoryName = sc.SubCategoryName,
                                        MainCategoryName = c.MainCategoryName,
                                        RefferedByOrReason = cc.RefferedByOrReason,
                                        CreatedDate = cc.CreatedDate,
                                        ApprovedBy = cc.ApprovedBy
                                    }).OrderBy(c => c.ChildCategoryName).Distinct().ToList();

                    return ChildCatList.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public childcategory GetChildCategoryDetail(int ChildCategoryId)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    //tblCategoryProduct product = new tblCategoryProduct();
                    childcategory category = new childcategory();
                    category = (from u in dbContext.tblChildCategories
                                join s in dbContext.tblSubCategories on u.SubCategoryId equals s.ID
                                join c in dbContext.tblCategoryProducts on s.CategoryProductID equals c.ID
                                join uc in dbContext.tblUsers on u.CreatedBy equals uc.ID
                                where u.ID == ChildCategoryId
                                select new childcategory
                                {
                                    ID = u.ID,
                                    MainCategoryName = c.MainCategoryName,
                                    SubCategoryId = s.SubCategoryId,
                                    SubCategoryName = s.SubCategoryName.Trim(),
                                    ChildCategoryName = u.ChildCategoryName,
                                    CreatedDate = u.CreatedDate,
                                    CreatedID = u.CreatedBy,
                                    RefferedByOrReason = u.RefferedByOrReason,
                                    ApprovedBy = u.ApprovedBy,
                                    CreatedByName = uc.Username,
                                }).FirstOrDefault();

                    //get history
                    List<CategoryHistory> histories = new List<CategoryHistory>();
                    histories = (from h in dbContext.tblCategoryHistories
                                 join u in dbContext.tblUsers on h.UserID equals u.ID
                                 where h.ProductID == ChildCategoryId && h.ProductCategory == "ChildCategory"
                                 select new CategoryHistory
                                 {
                                     OldProductName = h.OldProductName,
                                     OldMainCategory = h.OldMainCategory,
                                     OldSubCategory = h.OldSubCategory,
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
        public bool SaveChildCategory(childcategory ChildCatDetail, int UserID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        var childcat = dbContext.tblChildCategories.AsNoTracking().Where(p => p.ID == ChildCatDetail.ID).FirstOrDefault();
                        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        int RoleID = dbContext.tblUsers.Where(u => u.ID == UserID).FirstOrDefault().RoleID.Value;

                        if (childcat != null)
                        {
                            var ChildCatOld = (from c in dbContext.tblChildCategories
                                               join s in dbContext.tblSubCategories on c.SubCategoryId equals s.ID
                                               join cp in dbContext.tblCategoryProducts on s.CategoryProductID equals cp.ID
                                               where c.ID == childcat.ID
                                               select new ItemCategory
                                               {
                                                   MainCategoryName = cp.MainCategoryName,
                                                   SubCategoryName = s.SubCategoryName,
                                                   ChildCategoryName = c.ChildCategoryName
                                               }).FirstOrDefault();

                            tblChildCategory category = new tblChildCategory();
                            category.ID = ChildCatDetail.ID;
                            category.SubCategoryId = ChildCatDetail.SubCategoryId;
                            category.ChildCategoryName = ChildCatDetail.ChildCategoryName;
                            category.CreatedDate = DateTimeNow;
                            category.CreatedBy = UserID;
                            category.RefferedByOrReason = ChildCatDetail.RefferedByOrReason;
                            category.ApprovedBy = ChildCatDetail.ApprovedBy;
                            if (RoleID == 1)
                            {
                                category.IsActive = true;
                            }
                            else
                            {
                                category.IsActive = false;
                            }
                            dbContext.tblChildCategories.Add(category);
                            dbContext.Entry(category).State = EntityState.Modified;

                            //Insert into History Table
                            tblHistory history = new tblHistory();
                            history.UserID = UserID;
                            history.ProductID = ChildCatDetail.ID;
                            history.ProductCategory = ChildCatDetail.ChildCategoryName;
                            history.CreatedDate = DateTimeNow;
                            history.Comments = "ChildCategory Updated";
                            history.ActivityPage = "childcategories";
                            history.ActivityType = "update";
                            dbContext.tblHistories.Add(history);

                            //Insert into Category History Table
                            tblCategoryHistory categoryHistory = new tblCategoryHistory();
                            categoryHistory.UserID = UserID;
                            categoryHistory.ProductID = ChildCatDetail.ID;
                            categoryHistory.ProductCategory = "ChildCategory";
                            categoryHistory.ProductName = ChildCatDetail.ChildCategoryName;
                            categoryHistory.OldProductName = childcat.ChildCategoryName;
                            categoryHistory.OldMainCategory = ChildCatOld.MainCategoryName;
                            categoryHistory.OldSubCategory = ChildCatOld.SubCategoryName;
                            categoryHistory.CreatedDate = DateTimeNow;
                            dbContext.tblCategoryHistories.Add(categoryHistory);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            tblChildCategory category = new tblChildCategory();
                            category.ChildCategoryName = ChildCatDetail.ChildCategoryName;
                            category.SubCategoryId = ChildCatDetail.SubCategoryId;
                            category.CreatedDate = DateTimeNow;
                            category.CreatedBy = UserID;
                            category.RefferedByOrReason = ChildCatDetail.RefferedByOrReason;
                            category.ApprovedBy = ChildCatDetail.ApprovedBy;
                            category.IsMasterProduct = false;
                            category.IsRejected = false;
                            if (RoleID == 1)
                            {
                                category.IsActive = true;
                            }
                            else
                            {
                                category.IsActive = false;
                            }
                            dbContext.tblChildCategories.Add(category);

                            //Insert into History Table
                            tblHistory history = new tblHistory();
                            history.UserID = UserID;
                            history.ProductID = ChildCatDetail.ID;
                            history.ProductCategory = ChildCatDetail.ChildCategoryName;
                            history.CreatedDate = DateTimeNow;
                            history.Comments = "ChildCategory Created";
                            history.ActivityPage = "childcategories";
                            history.ActivityType = "create";
                            dbContext.tblHistories.Add(history);

                            dbContext.SaveChanges();
                        }
                        dbcxtransaction.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="SubCategoryID"></param>
        /// <returns></returns>
        public string GetMainCategoryName(int SubCategoryID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    //tblCategoryProduct product = new tblCategoryProduct();

                    string MainCategoryName = (from sc in dbContext.tblSubCategories
                                               join c in dbContext.tblCategoryProducts on sc.CategoryProductID equals c.ID
                                               where sc.ID == SubCategoryID
                                               select new childcategory
                                               {
                                                   MainCategoryName = c.MainCategoryName
                                               }).FirstOrDefault().MainCategoryName;
                    return MainCategoryName;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Done A
        /// </summary>
        /// <param name="ChildCategoryName"></param>
        /// <returns></returns>
        public bool CheckDuplicateName(string ChildCategoryName)
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    var IsValueexists = dbcontext.tblChildCategories.Where(c => c.ChildCategoryName.ToLower() == ChildCategoryName.ToLower()).FirstOrDefault();

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
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return false;
            }
        }

        // public List<childcategory> CategoryList(childcategory obj)
        //{
        //    try
        //    {
        //        using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
        //        {

        //            List<childcategory> ChildCatList = new List<childcategory>();
        //            if (!string.IsNullOrEmpty(obj.MainCategoryName))
        //            {
        //                ChildCatList = (from u in dbContext.tblChildCategories
        //                                join s in dbContext.tblSubCategories on u.SubCategoryId equals s.SubCategoryId
        //                                join c in dbContext.tblCategoryProducts on s.CategoryProductID equals c.CategoryProductID
        //                                select new childcategory
        //                                {

        //                                    ItemId = u.ItemId,
        //                                    ItemName = u.ItemName,
        //                                    MainCategoryName = c.MainCategoryName,
        //                                    SubCategoryId = s.SubCategoryId,
        //                                    SubCategoryName = s.SubCategoryName.Trim(),
        //                                    ChildCategoryName = u.ChildCategoryName,
        //                                    CreatedDate = u.CreatedDate,
        //                                    CreatedID = u.CreatedID
        //                                }).ToList();
        //            }
        //                return ChildCatList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}
    }
}

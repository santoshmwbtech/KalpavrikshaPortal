using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Reflection;

namespace JBNClassLibrary
{
    public class ItemCategory
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter item name")]
        public string ItemName { get; set; }
        [Required(ErrorMessage = "Please select Child Category")]
        public int ChildCategoryId { get; set; }

        public string ChildCategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedID { get; set; }
        public string SubCategoryName { get; set; }
        public string MainCategoryName { get; set; }
        public int CategoryProductID { get; set; }
        [Required(ErrorMessage = "Please enter Reffered by or Reason to create this item")]
        public string ItemRefferedByOrReason { get; set; }
        [Required(ErrorMessage = "Please enter the name of the person who approved this product")]
        public string ItemApprovedBy { get; set; }
        public string RefferedByOrReason { get; set; }
        public string ApprovedBy { get; set; }
        public string SearchText { get; set; }
        public string CreatedByName { get; set; }
        public bool IsActive { get; set; }
        //[Required(ErrorMessage = "Please upload the images for the product")]
        [Display(Name = "Browse File")]
        public HttpPostedFileBase[] ProductImages { get; set; }
        public List<string> strProductImages { get; set; }
        public string ItemImage1 { get; set; }
        public string ItemImage2 { get; set; }
        public bool IsMasterItemProduct { get; set; }
        public string DisplayMsg { get; set; }
    }
    public class DLItemCategory
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public List<ItemCategory> GetItemCatList(ItemCategory obj)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<ItemCategory> ItemCatList = new List<ItemCategory>();
                    if (!string.IsNullOrEmpty(obj.MainCategoryName))
                    {
                        ItemCatList = (from ip in dbContext.tblItemCategories
                                       join c in dbContext.tblChildCategories on ip.ChildCategoryID equals c.ID
                                       join s in dbContext.tblSubCategories on c.SubCategoryId equals s.ID
                                       join cp in dbContext.tblCategoryProducts on s.CategoryProductID equals cp.ID
                                       where ip.IsActive == true
                                       select new ItemCategory
                                       {
                                           ID = ip.ID,
                                           ItemName = ip.ItemName,
                                           ChildCategoryId = ip.ChildCategoryID.Value,
                                           MainCategoryName = cp.MainCategoryName,
                                           SubCategoryId = s.ID,
                                           SubCategoryName = s.SubCategoryName.Trim(),
                                           ChildCategoryName = c.ChildCategoryName,
                                           CreatedDate = ip.CreatedDate,
                                           CreatedID = ip.CreatedBy,
                                           ItemRefferedByOrReason = ip.RefferedByOrReason,
                                           ItemApprovedBy = ip.ApprovedBy
                                       }).ToList();
                    }
                    else
                    {
                        ItemCatList = (from ip in dbContext.tblItemCategories
                                       join c in dbContext.tblChildCategories on ip.ChildCategoryID equals c.ID
                                       join s in dbContext.tblSubCategories on c.SubCategoryId equals s.ID
                                       join cp in dbContext.tblCategoryProducts on s.CategoryProductID equals cp.ID
                                       where ip.IsActive == true
                                       select new ItemCategory
                                       {
                                           ID = ip.ID,
                                           ItemName = ip.ItemName,
                                           ChildCategoryId = ip.ChildCategoryID.Value,
                                           MainCategoryName = cp.MainCategoryName,
                                           SubCategoryId = s.ID,
                                           SubCategoryName = s.SubCategoryName.Trim(),
                                           ChildCategoryName = c.ChildCategoryName,
                                           CreatedDate = ip.CreatedDate,
                                           CreatedID = ip.CreatedBy,
                                           ItemRefferedByOrReason = ip.RefferedByOrReason,
                                           ItemApprovedBy = ip.ApprovedBy
                                       }).Distinct().ToList();
                    }

                    if (!string.IsNullOrEmpty(obj.SubCategoryName))
                        ItemCatList = (List<ItemCategory>)ItemCatList.Where(c => c.SubCategoryName.ToLower().Contains(obj.SubCategoryName.ToLower()));

                    return ItemCatList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<ItemCategory> GetItemCatList()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<ItemCategory> ItemCatList = new List<ItemCategory>();
                    ItemCatList = (from ip in dbContext.tblItemCategories
                                   join c in dbContext.tblChildCategories on ip.ChildCategoryID equals c.ID
                                   join s in dbContext.tblSubCategories on c.SubCategoryId equals s.ID
                                   join cp in dbContext.tblCategoryProducts on s.CategoryProductID equals cp.ID
                                   where ip.IsActive == true
                                   select new ItemCategory
                                   {
                                       ID = ip.ID,
                                       ItemName = ip.ItemName,
                                       ChildCategoryId = ip.ChildCategoryID.Value,
                                       MainCategoryName = cp.MainCategoryName,
                                       SubCategoryId = s.ID,
                                       SubCategoryName = s.SubCategoryName.Trim(),
                                       ChildCategoryName = c.ChildCategoryName,
                                       CreatedDate = ip.CreatedDate,
                                       CreatedID = ip.CreatedBy,
                                       ItemRefferedByOrReason = ip.RefferedByOrReason,
                                       ItemApprovedBy = ip.ApprovedBy
                                   }).OrderBy(c => c.ItemName).Distinct().Take(1000).ToList();

                    return ItemCatList.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<ItemCategory> GetAllItemCatList(ItemCategory item)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<ItemCategory> ItemCatList = new List<ItemCategory>();
                    ItemCatList = (from ip in dbContext.tblItemCategories
                                   join c in dbContext.tblChildCategories on ip.ChildCategoryID equals c.ID
                                   join sc in dbContext.tblSubCategories on c.SubCategoryId equals sc.ID
                                   join cp in dbContext.tblCategoryProducts on sc.CategoryProductID equals cp.ID
                                   where ip.ItemName.Contains(item.SearchText)
                                   && ip.ItemName != null && ip.IsActive == true
                                   select new ItemCategory
                                   {
                                       ID = ip.ID,
                                       ItemName = ip.ItemName,
                                       ChildCategoryName = c.ChildCategoryName,
                                       SubCategoryName = sc.SubCategoryName,
                                       MainCategoryName = cp.MainCategoryName,
                                       ItemRefferedByOrReason = ip.RefferedByOrReason,
                                       CreatedDate = ip.CreatedDate,
                                       ItemApprovedBy = ip.ApprovedBy
                                   }).OrderBy(c => c.ItemName).Distinct().ToList();

                    return ItemCatList.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public ItemCategory GetItemCategoryDetail(int ItemID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    //tblCategoryProduct product = new tblCategoryProduct();
                    ItemCategory item = new ItemCategory();
                    item = (from ip in dbContext.tblItemCategories
                            join c in dbContext.tblChildCategories on ip.ChildCategoryID equals c.ID
                            join sc in dbContext.tblSubCategories on c.SubCategoryId equals sc.ID
                            join cp in dbContext.tblCategoryProducts on sc.CategoryProductID equals cp.ID
                            join uc in dbContext.tblUsers on ip.CreatedBy equals uc.ID
                            where ip.ID == ItemID
                            select new ItemCategory
                            {
                                ID = ip.ID,
                                ItemName = ip.ItemName,
                                ChildCategoryId = c.ID,
                                MainCategoryName = cp.MainCategoryName,
                                SubCategoryId = sc.ID,
                                SubCategoryName = sc.SubCategoryName.Trim(),
                                ChildCategoryName = c.ChildCategoryName,
                                CreatedDate = ip.CreatedDate,
                                CreatedID = ip.CreatedBy,
                                ItemRefferedByOrReason = ip.RefferedByOrReason,
                                ItemApprovedBy = ip.ApprovedBy,
                                CreatedByName = uc.Username,
                                ItemImage1 = ip.ItemImage1,
                                ItemImage2 = ip.ItemImage2,
                                IsMasterItemProduct = ip.IsMasterProduct,
                            }).FirstOrDefault();
                    return item;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public ItemCategory SaveItemCategory(ItemCategory item, int UserID)
        {
            ItemCategory Result = new ItemCategory();
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        var IsExists = dbContext.tblItemCategories.Where(p => p.ID == item.ID && p.ChildCategoryID == item.ChildCategoryId).AsNoTracking().FirstOrDefault();

                        int RoleID = dbContext.tblUsers.Where(u => u.UserID == UserID).AsNoTracking().FirstOrDefault().RoleID.Value;

                        if (IsExists != null)
                        {
                            var ItemOld = (from ip in dbContext.tblItemCategories
                                           join c in dbContext.tblChildCategories on ip.ChildCategoryID equals c.ID
                                           join s in dbContext.tblSubCategories on c.SubCategoryId equals s.ID
                                           join cp in dbContext.tblCategoryProducts on s.CategoryProductID equals cp.ID
                                           where ip.ID == IsExists.ID
                                           select new ItemCategory
                                           {
                                               MainCategoryName = cp.MainCategoryName,
                                               SubCategoryName = s.SubCategoryName,
                                               ChildCategoryName = c.ChildCategoryName,
                                               ItemName = ip.ItemName
                                           }).FirstOrDefault();


                            tblItemCategory itemCategory = new tblItemCategory();
                            itemCategory.ID = IsExists.ID;
                            itemCategory.ItemName = item.ItemName;
                            itemCategory.ChildCategoryID = item.ChildCategoryId;
                            itemCategory.RefferedByOrReason = item.ItemRefferedByOrReason;
                            itemCategory.ApprovedBy = item.ItemApprovedBy;
                            itemCategory.CreatedDate = IsExists.CreatedDate;
                            itemCategory.CreatedBy = UserID;
                            itemCategory.RefferedByOrReason = IsExists.RefferedByOrReason;
                            itemCategory.ApprovedBy = IsExists.ApprovedBy;
                            if (RoleID == 1)
                                itemCategory.IsActive = true;
                            else
                                itemCategory.IsActive = false;
                            itemCategory.IsActive = IsExists.IsActive;
                            itemCategory.IsRejected = IsExists.IsRejected;
                            itemCategory.IsMasterProduct = IsExists.IsMasterProduct;
                            itemCategory.ItemImage1 = IsExists.ItemImage1;
                            itemCategory.ItemImage2 = IsExists.ItemImage2;
                            itemCategory.ItemMatrix = IsExists.ItemMatrix;

                            dbContext.tblItemCategories.Add(itemCategory);
                            dbContext.Entry(itemCategory).State = EntityState.Modified;

                            //Insert into History Table
                            tblHistory history = new tblHistory();
                            history.UserID = UserID;
                            history.ProductID = itemCategory.ID;
                            history.ProductCategory = itemCategory.ItemName;
                            history.CreatedDate = DateTimeNow;
                            history.CreatedBy = UserID;
                            history.ActivityPage = "items";
                            history.ActivityType = "update";
                            history.Comments = "ItemCategory Updated";
                            dbContext.tblHistories.Add(history);

                            //Insert into Category History Table
                            tblCategoryHistory categoryHistory = new tblCategoryHistory();
                            categoryHistory.UserID = UserID;
                            categoryHistory.ProductID = itemCategory.ID;
                            categoryHistory.ProductCategory = "ItemCategory";
                            categoryHistory.ProductName = itemCategory.ItemName;
                            categoryHistory.OldProductName = IsExists.ItemName;
                            categoryHistory.OldMainCategory = ItemOld.MainCategoryName;
                            categoryHistory.OldSubCategory = ItemOld.SubCategoryName;
                            categoryHistory.OldChildCategory = ItemOld.ChildCategoryName;
                            categoryHistory.CreatedBy = UserID;
                            categoryHistory.CreatedDate = DateTimeNow;
                            dbContext.tblCategoryHistories.Add(categoryHistory);
                            dbContext.SaveChanges();
                            if (RoleID == 1)
                                Result.DisplayMsg = "Item Updated Successfully";
                            else
                                Result.DisplayMsg = "Item updated successfully and waiting for approval";
                        }
                        else
                        {
                            tblItemCategory ItemCategory = new tblItemCategory();
                            ItemCategory.ItemName = item.ItemName;
                            ItemCategory.ChildCategoryID = item.ChildCategoryId;
                            ItemCategory.RefferedByOrReason = item.ItemRefferedByOrReason;
                            ItemCategory.ApprovedBy = item.ItemApprovedBy;
                            ItemCategory.CreatedDate = DateTimeNow;
                            ItemCategory.CreatedBy = UserID;
                            ItemCategory.RefferedByOrReason = "Admin";
                            ItemCategory.ApprovedBy = "Admin";
                            ItemCategory.IsActive = true;
                            ItemCategory.IsRejected = false;
                            ItemCategory.ItemMatrix = 1;
                            if (RoleID == 1)
                                ItemCategory.IsActive = true;
                            else
                                ItemCategory.IsActive = false;
                            ItemCategory.IsMasterProduct = false;

                            if(item.strProductImages != null && item.strProductImages.Count > 0)
                            {
                                if (item.strProductImages.Count() > 1)
                                {
                                    ItemCategory.ItemImage1 = item.strProductImages[0];
                                    ItemCategory.ItemImage2 = item.strProductImages[1];
                                }
                                else
                                {
                                    ItemCategory.ItemImage1 = item.strProductImages[0];
                                }
                            }
                            dbContext.tblItemCategories.Add(ItemCategory);

                            //Insert into History Table
                            tblHistory history = new tblHistory();
                            history.UserID = UserID;
                            history.ProductID = ItemCategory.ID;
                            history.ProductCategory = ItemCategory.ItemName;
                            history.CreatedDate = DateTimeNow;
                            history.CreatedBy = UserID;
                            history.Comments = "ItemCategory Created";
                            history.ActivityPage = "items";
                            history.ActivityType = "create";
                            dbContext.tblHistories.Add(history);
                            dbContext.SaveChanges();
                            if (RoleID == 1)
                                Result.DisplayMsg = "Item Created Successfully";
                            else
                                Result.DisplayMsg = "Item Created successfully and waiting for approval";
                        }
                        dbcxtransaction.Commit();
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.DisplayMsg = "Error please contact administrator";
                return Result;
            }
        }
        public ItemCategory GetParentCategories(int? ChildCategoryID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    ItemCategory itemCategory = (from cc in dbContext.tblChildCategories
                                                 join s in dbContext.tblSubCategories on cc.SubCategoryId equals s.ID
                                                 join c in dbContext.tblCategoryProducts on s.CategoryProductID equals c.ID
                                                 where cc.ID == ChildCategoryID
                                                 select new ItemCategory
                                                 {
                                                     SubCategoryId = s.ID,
                                                     SubCategoryName = s.SubCategoryName,
                                                     MainCategoryName = c.MainCategoryName
                                                 }).FirstOrDefault();
                    return itemCategory;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public bool CheckDuplicateName(string ItemName)
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    var IsValueexists = dbcontext.tblItemCategories.Where(c => c.ItemName.ToLower() == ItemName.ToLower()).FirstOrDefault();

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
        public DataTable GetAllCategories()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    //var ChildCatList = (from u in dbContext.tblChildCategories
                    //                    join s in dbContext.tblSubCategories on u.SubCategoryId equals s.SubCategoryId
                    //                    join c in dbContext.tblCategoryProducts on s.CategoryProductID equals c.CategoryProductID
                    //                    where u.IsActive == true
                    //                    select new
                    //                    {
                    //                        ItemName = u.ItemName,
                    //                        MainCategoryName = c.MainCategoryName,
                    //                        SubCategoryName = s.SubCategoryName.Trim(),
                    //                        ChildCategoryName = u.ChildCategoryName,
                    //                    }).Distinct().AsEnumerable();

                    var ChildCatList = (from c in dbContext.GetAllCategories()
                                        select c);

                    DataTable ItemsDT = LINQResultToDataTable(ChildCatList);

                    return ItemsDT;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {
            DataTable dt = new DataTable();


            PropertyInfo[] columns = null;

            if (Linqlist == null) return dt;

            foreach (T Record in Linqlist)
            {

                if (columns == null)
                {
                    columns = ((Type)Record.GetType()).GetProperties();
                    foreach (PropertyInfo GetProperty in columns)
                    {
                        Type colType = GetProperty.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dt.Columns.Add(new DataColumn(GetProperty.Name, colType));
                    }
                }

                DataRow dr = dt.NewRow();

                foreach (PropertyInfo pinfo in columns)
                {
                    dr[pinfo.Name] = pinfo.GetValue(Record, null) == null ? DBNull.Value : pinfo.GetValue
                    (Record, null);
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}

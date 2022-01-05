using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class FormPermission
    {
        public int? FormPermissionID { get; set; }
        public int RoleID { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public int? CreatedID { get; set; }
        public List<FormPermissionItem> Items { get; set; }
        public string Result { get; set; }

    }
    public class FormPermissionItem
    {
        public int? FormPermissionID { get; set; }
        public int? FormPermissionItemID { get; set; }
        public int? RoleID { get; set; }
        public string MainMenuName { get; set; }
        public string SubMenuName { get; set; }
        public int? MainMenuID { get; set; }
        public int? SubMenuID { get; set; }
        public int? CreatedID { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public bool IsCreate { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool CheckAll { get; set; }

    }
    public class DLFormPermission
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public List<tblSysRole> GetSysRoles()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<tblSysRole> Roles = new List<tblSysRole>();
                    Roles = dbContext.tblSysRoles.ToList();
                    return Roles;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public FormPermission SaveFormPermission(FormPermission formPermission, int UserID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    FormPermission Result = new FormPermission();

                    var IsExists = dbContext.tblFormPermissions.Where(f => f.RoleID == formPermission.RoleID).AsNoTracking().FirstOrDefault();
                    if (IsExists != null)
                    {
                        dbContext.tblFormPermissionItems.RemoveRange(dbContext.tblFormPermissionItems.Where(x => x.FormPermissionID == IsExists.ID));
                        dbContext.SaveChanges();

                        if (formPermission.Items != null && formPermission.Items.Count() > 0)
                        {
                            foreach (var fItem in formPermission.Items)
                            {
                                tblFormPermissionItem item = new tblFormPermissionItem();
                                item.FormPermissionID = IsExists.ID;
                                item.SubMenuID = fItem.SubMenuID;
                                item.RoleID = formPermission.RoleID;
                                item.CreatedDate = DateTimeNow;
                                item.CreatedBy = UserID;
                                item.IsCreate = fItem.IsCreate;
                                item.IsView = fItem.IsView;
                                item.IsEdit = fItem.IsEdit;
                                item.IsDelete = fItem.IsDelete;
                                dbContext.tblFormPermissionItems.Add(item);
                            }
                            tblFormPermission tblFormPermission = new tblFormPermission();
                            tblFormPermission.ID = IsExists.ID;
                            tblFormPermission.CreatedDate = DateTimeNow;
                            tblFormPermission.CreatedBy = UserID;
                            tblFormPermission.RoleID = formPermission.RoleID;
                            dbContext.Entry(tblFormPermission).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        Result.Result = "Form permission updated Successfully!!";
                    }
                    else
                    {
                        if (formPermission.Items != null && formPermission.Items.Count() > 0)
                        {
                            tblFormPermission tblFormPermission = new tblFormPermission();
                            tblFormPermission.CreatedDate = DateTimeNow;
                            tblFormPermission.CreatedBy = UserID;
                            tblFormPermission.RoleID = formPermission.RoleID;
                            dbContext.tblFormPermissions.Add(tblFormPermission);
                            dbContext.SaveChanges();
                            int FormPermissionID = tblFormPermission.ID;

                            foreach (var fItem in formPermission.Items)
                            {
                                tblFormPermissionItem item = new tblFormPermissionItem();
                                item.FormPermissionID = FormPermissionID;
                                item.SubMenuID = fItem.SubMenuID;
                                item.RoleID = fItem.RoleID;
                                item.CreatedDate = DateTimeNow;
                                item.CreatedBy = UserID;
                                item.IsCreate = fItem.IsCreate;
                                item.IsView = fItem.IsView;
                                item.IsEdit = fItem.IsEdit;
                                item.IsDelete = fItem.IsDelete;
                                dbContext.tblFormPermissionItems.Add(item);
                            }
                            dbContext.SaveChanges();
                        }
                        Result.Result = "Form permission saved Successfully!!";
                    }
                    return Result;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                FormPermission Result = new FormPermission();
                Result.Result = "Error while saving the form permission.. Please try later";
                return Result;
            }
        }

        public List<FormPermissionItem> GetItems()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<FormPermissionItem> formPermissionItems = new List<FormPermissionItem>();
                    formPermissionItems = (from s in dbContext.tblSysSubMenus
                                           select new FormPermissionItem
                                           {
                                               SubMenuID = s.ID,
                                               SubMenuName = s.SubMenuName,
                                               IsCreate = false,
                                               IsEdit = false,
                                               IsDelete = false,
                                               IsView = false,
                                           }).ToList();

                    return formPermissionItems;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<FormPermissionItem> LoadFormPermissionItems(int? RoleID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<FormPermissionItem> formPermissionItems = new List<FormPermissionItem>();
                    formPermissionItems = (from f in dbContext.tblFormPermissions
                                           join fp in dbContext.tblFormPermissionItems on f.ID equals fp.FormPermissionID
                                           join sm in dbContext.tblSysSubMenus on fp.SubMenuID equals sm.ID into subs
                                           from sm in subs.DefaultIfEmpty()
                                           where f.RoleID == RoleID
                                           select new FormPermissionItem
                                           {
                                               FormPermissionID = f.ID,
                                               FormPermissionItemID = fp.ID,
                                               RoleID = f.RoleID,
                                               MainMenuID = fp.MainMenuID,
                                               SubMenuID = sm.ID,
                                               SubMenuName = sm.SubMenuName,
                                               IsCreate = fp.IsCreate.Value,
                                               IsEdit = fp.IsEdit.Value,
                                               IsDelete = fp.IsDelete.Value,
                                               IsView = fp.IsView.Value,
                                           }).ToList();

                    if(formPermissionItems == null || formPermissionItems.Count() <= 0)
                    {
                        formPermissionItems = (from s in dbContext.tblSysSubMenus
                                               select new FormPermissionItem
                                               {
                                                   SubMenuID = s.ID,
                                                   SubMenuName = s.SubMenuName,
                                                   IsCreate = false,
                                                   IsEdit = false,
                                                   IsDelete = false,
                                                   IsView = false,
                                               }).ToList();
                    }

                    return formPermissionItems;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
    }
}

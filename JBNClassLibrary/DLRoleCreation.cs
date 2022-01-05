using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace JBNClassLibrary
{
    public class DLRoleCreation
    {
        public int RoleID { get; set; }
        [Required]
        public string RoleName { get; set; }
        [Required]
        public string RoleDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CreatedByID { get; set; }
        public int ModifiedByID { get; set; }
        public bool IsActive { get; set; }
        
    }

    public class DLGetRoleCreation
    {
        List<DLRoleCreation> Rolelist = new List<DLRoleCreation>();
        tblSysRole sysRole = new tblSysRole();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public bool SaveData(DLRoleCreation dLRoleCreation, string UserID)
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    if (dbcontext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbcontext.Database.Connection.Open();

                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    var isValueExists = dbcontext.tblSysRoles.AsNoTracking().Where(u => u.ID == dLRoleCreation.RoleID).FirstOrDefault();

                    if (isValueExists == null)
                    {
                        var ExistingValue = dbcontext.tblSysRoles.AsNoTracking().Where(u => u.RoleName == dLRoleCreation.RoleName).FirstOrDefault();

                        if (ExistingValue == null)
                        {
                            sysRole.RoleName = dLRoleCreation.RoleName;
                            sysRole.RoleDescription = dLRoleCreation.RoleDescription;
                            sysRole.CreatedDate = DateTimeNow;
                            sysRole.CreatedBy = Convert.ToInt32(UserID);
                            dbcontext.tblSysRoles.Add(sysRole);

                            //Insert into History Table
                            tblHistory history = new tblHistory();
                            history.UserID = Convert.ToInt32(UserID);
                            history.CustID = dLRoleCreation.RoleID;
                            history.ProductCategory = dLRoleCreation.RoleName;
                            history.CreatedDate = DateTimeNow;
                            history.ActivityPage = "roles";
                            history.ActivityType = "create";
                            history.Comments = "Role Created";
                            dbcontext.tblHistories.Add(history);

                            dbcontext.SaveChanges();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {                       
                        sysRole.ID = isValueExists.ID;
                        sysRole.RoleName = dLRoleCreation.RoleName;
                        sysRole.RoleDescription = dLRoleCreation.RoleDescription;
                        sysRole.CreatedDate = isValueExists.CreatedDate;
                        sysRole.ModifiedDate = DateTimeNow;
                        sysRole.CreatedBy = isValueExists.CreatedBy;
                        sysRole.ModifiedBy = Convert.ToInt32(UserID);
                        dbcontext.tblSysRoles.Add(sysRole);
                        dbcontext.Entry(sysRole).State = EntityState.Modified;

                        //Insert into History Table
                        tblHistory history = new tblHistory();
                        history.UserID = Convert.ToInt32(UserID);
                        history.CustID = isValueExists.ID;
                        history.ProductCategory = dLRoleCreation.RoleName;
                        history.CreatedDate = DateTimeNow;
                        history.ActivityPage = "roles";
                        history.ActivityType = "update";
                        history.Comments = "Role Updated";
                        dbcontext.tblHistories.Add(history);

                        dbcontext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return false;
            }
            return true;
        }

        public List<DLRoleCreation> GetData()
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    if (dbcontext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbcontext.Database.Connection.Open();


                    Rolelist = (from role in dbcontext.tblSysRoles
                                select new DLRoleCreation
                                {
                                    RoleID = role.ID,
                                    RoleDescription = role.RoleDescription,
                                    RoleName = role.RoleName,
                                }).OrderByDescending(i => i.RoleID).ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message,ex.Source, ex.InnerException == null ? null : ex.InnerException, ex.StackTrace);
            }

            return Rolelist;
        }
        public DLRoleCreation GetRoleDetailsByID(int RoleID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    DLRoleCreation result = new DLRoleCreation();
                    result = (from u in dbContext.tblSysRoles
                              where u.ID == RoleID
                              select new DLRoleCreation
                              {
                                  RoleID = u.ID,
                                  RoleName = u.RoleName,
                                  RoleDescription = u.RoleDescription,
                              }).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public bool CheckDuplicateRoleName(string rname)
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbcontext.Database.BeginTransaction())
                    {
                        var IsValueexists = from gUser in dbcontext.tblSysRoles.AsNoTracking()
                                            where gUser.RoleName.ToLower().Trim().Equals(rname.ToLower().Trim())
                                            select gUser.RoleName;

                        if (IsValueexists.Count() != 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException == null ? null : ex.InnerException, ex.StackTrace);
                return false;
            }
        }
    }
}

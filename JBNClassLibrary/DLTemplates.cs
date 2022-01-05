using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;
using JBNWebAPI.Logger;

namespace JBNClassLibrary
{
    public class DLTemplates
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public Templates SaveSMSTemplate(Templates templates, string UserID)
        {
            Templates Result = new Templates();
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    string RoleName = (from u in dbContext.tblUsers
                                       join role in dbContext.tblSysRoles on u.RoleID equals role.ID
                                       where u.UserID.ToString() == UserID
                                       select role.RoleName).FirstOrDefault();

                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    var isValueExists = dbContext.tblSMSTemplates.AsNoTracking().Where(i => i.ID == templates.ID).FirstOrDefault();

                    if (isValueExists == null)
                    {
                        var IsNameExists = dbContext.tblSMSTemplates.AsNoTracking().Where(i => i.TemplateName.ToLower().Trim() == templates.TemplateName.ToLower().Trim()).FirstOrDefault();
                        if (IsNameExists == null)
                        {
                            tblSMSTemplate tblSMSTemplate = new tblSMSTemplate();
                            tblSMSTemplate.TemplateName = templates.TemplateName;
                            tblSMSTemplate.TemplateBody = templates.TemplateBody;
                            tblSMSTemplate.CreatedBy = Convert.ToInt32(UserID);
                            tblSMSTemplate.CreatedDate = DateTimeNow;
                            if (RoleName.ToLower() == "admin")
                                //tblSMSTemplate.IsActive = 1;
                                tblSMSTemplate.IsActive = true;
                            else
                                //tblSMSTemplate.IsActive = 0;
                                tblSMSTemplate.IsActive = false;
                            tblSMSTemplate.Deleted = 0;
                            dbContext.tblSMSTemplates.Add(tblSMSTemplate);
                            dbContext.SaveChanges();
                            Result.DisplayMessage = "SMS Template Saved Successfully";
                            return Result;
                        }
                        else
                        {
                            Result.DisplayMessage = "SMS Template Name already exists";
                            return Result;
                        }
                    }
                    else
                    {
                        var IsNameExists = dbContext.tblSMSTemplates.AsNoTracking().Where(i => i.TemplateName.ToLower().Trim() == templates.TemplateName.ToLower().Trim() && i.ID != templates.ID).FirstOrDefault();
                        if (IsNameExists == null)
                        {
                            tblSMSTemplate tblSMSTemplate = new tblSMSTemplate();
                            tblSMSTemplate.ID = templates.ID;
                            tblSMSTemplate.TemplateName = templates.TemplateName;
                            tblSMSTemplate.TemplateBody = templates.TemplateBody;
                            tblSMSTemplate.CreatedBy = Convert.ToInt32(UserID);
                            tblSMSTemplate.CreatedDate = DateTimeNow;
                            if (RoleName.ToLower() == "admin")
                                tblSMSTemplate.IsActive = true;
                            else
                                tblSMSTemplate.IsActive = false;
                            tblSMSTemplate.Deleted = isValueExists.Deleted;
                            dbContext.tblSMSTemplates.Add(tblSMSTemplate);
                            dbContext.Entry(tblSMSTemplate).State = EntityState.Modified;
                            dbContext.SaveChanges();
                            Result.DisplayMessage = "SMS Template Updated Successfully";
                            return Result;
                        }
                        else
                        {
                            Result.DisplayMessage = "SMS Template Name already exists";
                            return Result;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.DisplayMessage = "Error.. Please contact administrator";
                return Result;
            }
        }
        //public Templates SaveEmailTemplate(Templates templates, string OrgID, string UserID)
        //{
        //    Templates Result = new Templates();
        //    try
        //    {
        //        using (dbContext = new Entity.MWBTCustomerAppdbContext())
        //        {
        //            if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
        //                dbContext.Database.Connection.Open();

        //            string RoleName = (from u in dbContext.tblSysUsers
        //                               join role in dbContext.tblSysRoles on u.RoleID equals role.RoleID
        //                               where u.UserID.ToString() == UserID
        //                               select role.RoleName).FirstOrDefault();

        //            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        //            var isValueExists = dbContext.tblEmailTemplates.AsNoTracking().Where(i => i.ID == templates.ID).FirstOrDefault();

        //            if (isValueExists == null)
        //            {
        //                var IsNameExists = dbContext.tblEmailTemplates.AsNoTracking().Where(i => i.TemplateName.ToLower().Trim() == templates.TemplateName.ToLower().Trim()).FirstOrDefault();
        //                if (IsNameExists == null)
        //                {
        //                    tblEmailTemplate tblEmailTemplate = new tblEmailTemplate();
        //                    tblEmailTemplate.TemplateName = templates.TemplateName;
        //                    tblEmailTemplate.TemplateBody = templates.TemplateBody;
        //                    tblEmailTemplate.OrgID = OrgID;
        //                    tblEmailTemplate.CreatedBy = Convert.ToInt32(UserID);
        //                    tblEmailTemplate.CreatedDate = DateTimeNow;
        //                    tblEmailTemplate.TemplateSubject = templates.TemplateSubject;
        //                    if (RoleName.ToLower() == "admin")
        //                        tblEmailTemplate.IsActive = 1;
        //                    else
        //                        tblEmailTemplate.IsActive = 0;
        //                    tblEmailTemplate.Deleted = 0;
        //                    dbContext.tblEmailTemplates.Add(tblEmailTemplate);
        //                    dbContext.SaveChanges();
        //                    Result.DisplayMessage = "Email Template Saved Successfully";
        //                    return Result;
        //                }
        //                else
        //                {
        //                    Result.DisplayMessage = "Email Template Name already exists";
        //                    return Result;
        //                }
        //            }
        //            else
        //            {
        //                var IsNameExists = dbContext.tblEmailTemplates.AsNoTracking().Where(i => i.TemplateName.ToLower().Trim() == templates.TemplateName.ToLower().Trim() && i.ID != templates.ID).FirstOrDefault();
        //                if (IsNameExists == null)
        //                {
        //                    tblEmailTemplate tblEmailTemplate = new tblEmailTemplate();
        //                    tblEmailTemplate.ID = templates.ID;
        //                    tblEmailTemplate.TemplateName = templates.TemplateName;
        //                    tblEmailTemplate.TemplateBody = templates.TemplateBody;
        //                    tblEmailTemplate.OrgID = OrgID;
        //                    tblEmailTemplate.CreatedBy = Convert.ToInt32(UserID);
        //                    tblEmailTemplate.CreatedDate = DateTimeNow;
        //                    tblEmailTemplate.TemplateSubject = templates.TemplateSubject;
        //                    if (RoleName.ToLower() == "admin")
        //                        tblEmailTemplate.IsActive = 1;
        //                    else
        //                        tblEmailTemplate.IsActive = 0;
        //                    tblEmailTemplate.Deleted = isValueExists.Deleted;
        //                    dbContext.tblEmailTemplates.Add(tblEmailTemplate);
        //                    dbContext.Entry(tblEmailTemplate).State = EntityState.Modified;
        //                    dbContext.SaveChanges();
        //                    Result.DisplayMessage = "Email Template Updated Successfully";
        //                    return Result;
        //                }
        //                else
        //                {
        //                    Result.DisplayMessage = "Email Template Name already exists";
        //                    return Result;
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        Result.DisplayMessage = "Error.. Please contact administrator";
        //        return Result;
        //    }
        //}
        //public Templates SaveWhatsappTemplate(Templates templates, string OrgID, string UserID)
        //{
        //    Templates Result = new Templates();
        //    try
        //    {
        //        using (dbContext = new Entity.MWBTCustomerAppdbContext())
        //        {
        //            if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
        //                dbContext.Database.Connection.Open();

        //            string RoleName = (from u in dbContext.tblSysUsers
        //                               join role in dbContext.tblSysRoles on u.RoleID equals role.RoleID
        //                               where u.UserID.ToString() == UserID
        //                               select role.RoleName).FirstOrDefault();

        //            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        //            var isValueExists = dbContext.tblWhatsappTemplates.AsNoTracking().Where(i => i.ID == templates.ID).FirstOrDefault();

        //            if (isValueExists == null)
        //            {
        //                var IsNameExists = dbContext.tblWhatsappTemplates.AsNoTracking().Where(i => i.TemplateName.ToLower().Trim() == templates.TemplateName.ToLower().Trim()).FirstOrDefault();
        //                if (IsNameExists == null)
        //                {
        //                    tblWhatsappTemplate tblWhatsappTemplate = new tblWhatsappTemplate();
        //                    tblWhatsappTemplate.TemplateName = templates.TemplateName;
        //                    tblWhatsappTemplate.TemplateBody = templates.TemplateBody;
        //                    tblWhatsappTemplate.OrgID = OrgID;
        //                    tblWhatsappTemplate.CreatedBy = Convert.ToInt32(UserID);
        //                    tblWhatsappTemplate.CreatedDate = DateTimeNow;
        //                    tblWhatsappTemplate.TemplateSubject = templates.TemplateSubject;
        //                    if (RoleName.ToLower() == "admin")
        //                        tblWhatsappTemplate.IsActive = 1;
        //                    else
        //                        tblWhatsappTemplate.IsActive = 0;
        //                    tblWhatsappTemplate.Deleted = 0;
        //                    dbContext.tblWhatsappTemplates.Add(tblWhatsappTemplate);
        //                    dbContext.SaveChanges();
        //                    Result.DisplayMessage = "Whatsapp Template Saved Successfully";
        //                    return Result;
        //                }
        //                else
        //                {
        //                    Result.DisplayMessage = "Whatsapp Template Name already exists";
        //                    return Result;
        //                }
        //            }
        //            else
        //            {
        //                var IsNameExists = dbContext.tblWhatsappTemplates.AsNoTracking().Where(i => i.TemplateName.ToLower().Trim() == templates.TemplateName.ToLower().Trim() && i.ID != templates.ID).FirstOrDefault();
        //                if (IsNameExists == null)
        //                {
        //                    tblWhatsappTemplate tblWhatsappTemplate = new tblWhatsappTemplate();
        //                    tblWhatsappTemplate.ID = templates.ID;
        //                    tblWhatsappTemplate.TemplateName = templates.TemplateName;
        //                    tblWhatsappTemplate.TemplateBody = templates.TemplateBody;
        //                    tblWhatsappTemplate.OrgID = OrgID;
        //                    tblWhatsappTemplate.CreatedBy = Convert.ToInt32(UserID);
        //                    tblWhatsappTemplate.CreatedDate = DateTimeNow;
        //                    tblWhatsappTemplate.TemplateSubject = templates.TemplateSubject;
        //                    if (RoleName.ToLower() == "admin")
        //                        tblWhatsappTemplate.IsActive = 1;
        //                    else
        //                        tblWhatsappTemplate.IsActive = 0;
        //                    tblWhatsappTemplate.Deleted = isValueExists.Deleted;
        //                    dbContext.tblWhatsappTemplates.Add(tblWhatsappTemplate);
        //                    dbContext.Entry(tblWhatsappTemplate).State = EntityState.Modified;
        //                    dbContext.SaveChanges();
        //                    Result.DisplayMessage = "Whatsapp Template Updated Successfully";
        //                    return Result;
        //                }
        //                else
        //                {
        //                    Result.DisplayMessage = "Whatsapp Template Name already exists";
        //                    return Result;
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        Result.DisplayMessage = "Error.. Please contact administrator";
        //        return Result;
        //    }
        //}
        public List<Templates> GetSMSTemplates()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    List<Templates> smsTemplates = new List<Templates>();

                    smsTemplates = (from b in dbContext.tblSMSTemplates
                                    where b.Deleted != 1
                                    select new Templates
                                    {
                                        ID = b.ID,
                                        TemplateName = b.TemplateName,
                                        CreatedBy = b.CreatedBy,
                                        CreatedDate = b.CreatedDate,
                                        Deleted = b.Deleted,
                                        IsActive = b.IsActive,
                                        Status = b.IsActive == false ? "Not Approved" : "Approved",
                                        TemplateBody = b.TemplateBody,
                                    }).OrderByDescending(i => i.TemplateName).ToList();

                    return smsTemplates;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        //public List<Templates> GetEmailTemplates(string OrgID)
        //{
        //    try
        //    {
        //        using (MWBTCustomerAppdbContext dbContext = new WBT.Entity.MWBTCustomerAppdbContext())
        //        {
        //            if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
        //                dbContext.Database.Connection.Open();

        //            List<Templates> emailTemplates = new List<Templates>();

        //            emailTemplates = (from b in dbContext.tblEmailTemplates
        //                              where b.OrgID == OrgID && b.Deleted != 1
        //                              select new Templates
        //                              {
        //                                  ID = b.ID,
        //                                  TemplateName = b.TemplateName,
        //                                  CreatedBy = b.CreatedBy,
        //                                  CreatedDate = b.CreatedDate,
        //                                  Deleted = b.Deleted,
        //                                  IsActive = b.IsActive,
        //                                  Status = b.IsActive == 0 ? "Not Approved" : "Approved",
        //                                  OrgID = b.OrgID,
        //                                  TemplateBody = b.TemplateBody,
        //                              }).OrderByDescending(i => i.TemplateName).ToList();

        //            return emailTemplates;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}
        //public List<Templates> GetWhatsappTemplates(string OrgID)
        //{
        //    try
        //    {
        //        using (MWBTCustomerAppdbContext dbContext = new WBT.Entity.MWBTCustomerAppdbContext())
        //        {
        //            if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
        //                dbContext.Database.Connection.Open();

        //            List<Templates> whatsappTemplates = new List<Templates>();

        //            whatsappTemplates = (from b in dbContext.tblWhatsappTemplates
        //                                 where b.OrgID == OrgID && b.Deleted != 1
        //                                 select new Templates
        //                                 {
        //                                     ID = b.ID,
        //                                     TemplateName = b.TemplateName,
        //                                     CreatedBy = b.CreatedBy,
        //                                     CreatedDate = b.CreatedDate,
        //                                     Deleted = b.Deleted,
        //                                     IsActive = b.IsActive,
        //                                     Status = b.IsActive == 0 ? "Not Approved" : "Approved",
        //                                     OrgID = b.OrgID,
        //                                     TemplateBody = b.TemplateBody,
        //                                 }).OrderByDescending(i => i.TemplateName).ToList();

        //            return whatsappTemplates;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}
        public Templates GetSMSTemplateDetails(int ID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    Templates templates = new Templates();
                    templates = (from b in dbContext.tblSMSTemplates
                                 where b.ID == ID && b.Deleted != 1
                                 select new Templates
                                 {
                                     ID = b.ID,
                                     TemplateName = b.TemplateName,
                                     CreatedBy = b.CreatedBy,
                                     CreatedDate = b.CreatedDate,
                                     Deleted = b.Deleted,
                                     IsActive = b.IsActive,
                                     Status = b.IsActive == false ? "Not Approved" : "Approved",
                                     TemplateBody = b.TemplateBody,
                                     templateType = TemplateType.SMS,
                                 }).FirstOrDefault();
                    return templates;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        //public Templates GetEmailTemplateDetails(int ID)
        //{
        //    try
        //    {
        //        using (MWBTCustomerAppdbContext dbContext = new MWBTCustomerAppdbContext())
        //        {
        //            Templates templates = new Templates();
        //            templates = (from b in dbContext.tblEmailTemplates
        //                         where b.ID == ID && b.Deleted != 1
        //                         select new Templates
        //                         {
        //                             ID = b.ID,
        //                             TemplateName = b.TemplateName,
        //                             CreatedBy = b.CreatedBy,
        //                             CreatedDate = b.CreatedDate,
        //                             Deleted = b.Deleted,
        //                             IsActive = b.IsActive,
        //                             Status = b.IsActive == 0 ? "Not Approved" : "Approved",
        //                             OrgID = b.OrgID,
        //                             TemplateBody = b.TemplateBody,
        //                             TemplateSubject = b.TemplateSubject,
        //                             templateType = TemplateType.Email,
        //                         }).FirstOrDefault();
        //            return templates;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}
        //public Templates GetWhatsappTemplateDetails(int ID)
        //{
        //    try
        //    {
        //        using (MWBTCustomerAppdbContext dbContext = new MWBTCustomerAppdbContext())
        //        {
        //            Templates templates = new Templates();
        //            templates = (from b in dbContext.tblWhatsappTemplates
        //                         where b.ID == ID && b.Deleted != 1
        //                         select new Templates
        //                         {
        //                             ID = b.ID,
        //                             TemplateName = b.TemplateName,
        //                             CreatedBy = b.CreatedBy,
        //                             CreatedDate = b.CreatedDate,
        //                             Deleted = b.Deleted,
        //                             IsActive = b.IsActive,
        //                             Status = b.IsActive == 0 ? "Not Approved" : "Approved",
        //                             OrgID = b.OrgID,
        //                             TemplateBody = b.TemplateBody,
        //                             TemplateSubject = b.TemplateSubject,
        //                             templateType = TemplateType.Whatsapp,
        //                         }).FirstOrDefault();
        //            return templates;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        return null;
        //    }
        //}
        public Templates DeleteSMSTemplate(int ID, string OrgID, string UserID)
        {
            Templates Result = new Templates();
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())// Entity.MWBTCustomerAppdbContext())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    var isValueExists = dbContext.tblSMSTemplates.AsNoTracking().Where(u => u.ID == ID).FirstOrDefault();

                    if (isValueExists == null)
                    {
                        Result.DisplayMessage = "Bad Request!!";
                        return Result;
                    }
                    else
                    {
                        //dbContext.tblSMSTemplates.Remove(dbContext.tblSMSTemplates.Where(r => r.ID == ID).FirstOrDefault());
                        tblSMSTemplate tblSMSTemplate = new tblSMSTemplate();
                        tblSMSTemplate.ID = ID;
                        tblSMSTemplate.Deleted = 1;
                        dbContext.tblSMSTemplates.Attach(tblSMSTemplate);
                        dbContext.Entry(tblSMSTemplate).Property(s => s.Deleted).IsModified = true;
                        dbContext.SaveChanges();
                        Result.DisplayMessage = "SMS Template Deleted Successfully";
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.DisplayMessage = "Something went wrong. Please contact administrator";
                return Result;
            }
        }
        //public Templates DeleteEmailTemplate(int ID, string OrgID, string UserID)
        //{
        //    Templates Result = new Templates();
        //    try
        //    {
        //        using (MWBTCustomerAppdbContext dbContext = new WBT.Entity.MWBTCustomerAppdbContext())// Entity.MWBTCustomerAppdbContext())
        //        {
        //            if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
        //                dbContext.Database.Connection.Open();

        //            var isValueExists = dbContext.tblEmailTemplates.AsNoTracking().Where(u => u.ID == ID).FirstOrDefault();

        //            if (isValueExists == null)
        //            {
        //                Result.DisplayMessage = "Bad Request!!";
        //                return Result;
        //            }
        //            else
        //            {
        //                tblEmailTemplate tblEmailTemplate = new tblEmailTemplate();
        //                tblEmailTemplate.ID = ID;
        //                tblEmailTemplate.Deleted = 1;
        //                dbContext.tblEmailTemplates.Attach(tblEmailTemplate);
        //                dbContext.Entry(tblEmailTemplate).Property(s => s.Deleted).IsModified = true;
        //                dbContext.SaveChanges();
        //                Result.DisplayMessage = "Email Template Deleted Successfully";
        //                return Result;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        Result.DisplayMessage = "Something went wrong. Please contact administrator";
        //        return Result;
        //    }
        //}
        //public Templates DeleteWhatsappTemplate(int ID, string OrgID, string UserID)
        //{
        //    Templates Result = new Templates();
        //    try
        //    {
        //        using (MWBTCustomerAppdbContext dbContext = new WBT.Entity.MWBTCustomerAppdbContext())// Entity.MWBTCustomerAppdbContext())
        //        {
        //            if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
        //                dbContext.Database.Connection.Open();

        //            var isValueExists = dbContext.tblWhatsappTemplates.AsNoTracking().Where(u => u.ID == ID).FirstOrDefault();

        //            if (isValueExists == null)
        //            {
        //                Result.DisplayMessage = "Bad Request!!";
        //                return Result;
        //            }
        //            else
        //            {
        //                tblWhatsappTemplate tblWhatsappTemplate = new tblWhatsappTemplate();
        //                tblWhatsappTemplate.ID = ID;
        //                tblWhatsappTemplate.Deleted = 1;
        //                dbContext.tblWhatsappTemplates.Attach(tblWhatsappTemplate);
        //                dbContext.Entry(tblWhatsappTemplate).Property(s => s.Deleted).IsModified = true;
        //                dbContext.SaveChanges();
        //                Result.DisplayMessage = "Whatsapp Template Deleted Successfully";
        //                return Result;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
        //        Result.DisplayMessage = "Something went wrong. Please contact administrator";
        //        return Result;
        //    }
        //}
        public List<Templates> GetAllTemplates(string OrgID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    List<Templates> allTemplates = new List<Templates>();

                    //List<Templates> whatsappTemplates = (from b in dbContext.tblWhatsappTemplates
                    //                                     where b.OrgID == OrgID && b.Deleted != 1 && b.IsActive != 1
                    //                                     select new Templates
                    //                                     {
                    //                                         ID = b.ID,
                    //                                         TemplateName = b.TemplateName,
                    //                                         CreatedBy = b.CreatedBy,
                    //                                         CreatedDate = b.CreatedDate,
                    //                                         Deleted = b.Deleted,
                    //                                         IsActive = b.IsActive,
                    //                                         Status = b.IsActive == 0 ? "Not Approved" : "Approved",
                    //                                         OrgID = b.OrgID,
                    //                                         TemplateBody = b.TemplateBody,
                    //                                         templateType = TemplateType.Whatsapp,
                    //                                         CreatedByUser = dbContext.tblSysUsers.Where(u => u.UserID == b.CreatedBy).FirstOrDefault().FName,
                    //                                     }).OrderByDescending(i => i.TemplateName).ToList();

                    //List<Templates> emailTemplates = (from b in dbContext.tblEmailTemplates
                    //                                  where b.OrgID == OrgID && b.Deleted != 1 && b.IsActive != 1
                    //                                  select new Templates
                    //                                  {
                    //                                      ID = b.ID,
                    //                                      TemplateName = b.TemplateName,
                    //                                      CreatedBy = b.CreatedBy,
                    //                                      CreatedDate = b.CreatedDate,
                    //                                      Deleted = b.Deleted,
                    //                                      IsActive = b.IsActive,
                    //                                      Status = b.IsActive == 0 ? "Not Approved" : "Approved",
                    //                                      OrgID = b.OrgID,
                    //                                      TemplateBody = b.TemplateBody,
                    //                                      templateType = TemplateType.Email,
                    //                                      CreatedByUser = dbContext.tblSysUsers.Where(u => u.UserID == b.CreatedBy).FirstOrDefault().FName,
                    //                                  }).OrderByDescending(i => i.TemplateName).ToList();

                    List<Templates> smsTemplates = (from b in dbContext.tblSMSTemplates
                                                    where b.Deleted != 1 && b.IsActive != true
                                                    select new Templates
                                                    {
                                                        ID = b.ID,
                                                        TemplateName = b.TemplateName,
                                                        CreatedBy = b.CreatedBy,
                                                        CreatedDate = b.CreatedDate,
                                                        Deleted = b.Deleted,
                                                        IsActive = b.IsActive,
                                                        Status = b.IsActive == false ? "Not Approved" : "Approved",
                                                        TemplateBody = b.TemplateBody,
                                                        templateType = TemplateType.SMS,
                                                        CreatedByUser = dbContext.tblUsers.Where(u => u.UserID == b.CreatedBy).FirstOrDefault().FullName,
                                                    }).OrderByDescending(i => i.TemplateName).ToList();
                    allTemplates = allTemplates.Union(smsTemplates).ToList();
                    return allTemplates;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public Templates ApproveTemplate(Templates templates, string UserID)
        {
            Templates Result = new Templates();
            try
            {
                if (templates.templateType == TemplateType.SMS)
                    Result = SaveSMSTemplate(templates, UserID);
                //else if (templates.templateType == TemplateType.Email)
                //    Result = SaveEmailTemplate(templates, OrgID, UserID);
                //else if (templates.templateType == TemplateType.Whatsapp)
                //    Result = SaveWhatsappTemplate(templates, OrgID, UserID);
                return Result;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.DisplayMessage = "Error.. Please contact administrator";
                return Result;
            }
        }
        public Templates DeleteTemplate(int ID, TemplateType templateType, string OrgID, string UserID)
        {
            Templates Result = new Templates();
            try
            {
                if (templateType == TemplateType.SMS)
                    Result = DeleteSMSTemplate(ID, OrgID, UserID);
                //else if (templateType == TemplateType.Email)
                //    Result = DeleteEmailTemplate(ID, OrgID, UserID);
                //else if (templateType == TemplateType.Whatsapp)
                //    Result = DeleteWhatsappTemplate(ID, OrgID, UserID);
                return Result;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.DisplayMessage = "Something went wrong. Please contact administrator";
                return Result;
            }
        }
    }
    public class Templates
    {
        public int ID { get; set; }
        public string OrgID { get; set; }
        [Required(ErrorMessage = "Please enter template name")]
        public string TemplateName { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Please enter message")]
        public string TemplateBody { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> Deleted { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public TemplateType templateType { get; set; }
        public string DisplayMessage { get; set; }
        public string Status { get; set; }
        public string TemplateSubject { get; set; }
        public string CreatedByUser { get; set; }
    }
    public enum TemplateType
    {
        Email,
        SMS,
        Whatsapp
    }
}

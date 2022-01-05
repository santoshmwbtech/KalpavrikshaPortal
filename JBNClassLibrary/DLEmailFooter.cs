using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using JBNWebAPI.Logger;

namespace JBNClassLibrary
{
    public class EmailFooter
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please enter Email Footer")]
        public string FooterBody { get; set; }
        public string CreationDate { get; set; }
    }
    public class DLEmailFooter
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public List<EmailFooter> GetEmailFooters()
        {
            List<EmailFooter> emailFooters = new List<EmailFooter>();
            emailFooters = (from EF in dbContext.tblEmailFooters
                    select new EmailFooter
                    {
                        ID = EF.ID,
                        FooterBody = EF.FooterBody
                    }).ToList();

            return emailFooters;
        }
        public EmailFooter GetEmailFooter(int ID)
        {
            EmailFooter footer = new EmailFooter();

            footer = (from u in dbContext.tblEmailFooters
                    where u.ID == ID
                    select new EmailFooter
                    {
                        ID = u.ID,
                        FooterBody = u.FooterBody
                    }).FirstOrDefault();
            return footer;
        }
        public bool SaveEmailFooter(EmailFooter emailFooter)
        {
            try
            {
                DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    var IsValueExists = dbContext.tblEmailFooters.AsNoTracking().Where(p => p.ID == emailFooter.ID).FirstOrDefault();

                    if (IsValueExists != null)
                    {
                        tblEmailFooter obj = new tblEmailFooter();
                        obj.ID = emailFooter.ID;
                        obj.FooterBody = emailFooter.FooterBody;
                        obj.CreatedDate = IsValueExists.CreatedDate;
                        dbContext.Entry(obj).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        tblEmailFooter obj = new tblEmailFooter();
                        obj.FooterBody = emailFooter.FooterBody;
                        obj.CreatedDate = DateTimeNow;
                        dbContext.tblEmailFooters.Add(obj);
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
    }
}

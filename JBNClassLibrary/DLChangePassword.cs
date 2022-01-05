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
    public class ChangePassword
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int ModifiedByID { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class DLChangePassword
    {
        public ChangePassword UpdatePassword(ChangePassword changePassword)
        {
            try
            {
                ChangePassword returnObject = new ChangePassword();
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    var IsValueExists = dbContext.tblUsers.AsNoTracking().Where(p => p.UserID == changePassword.UserID).FirstOrDefault();

                    if (IsValueExists != null)
                    {
                        tblUser user = new tblUser();
                        user.ID = changePassword.UserID;
                        user.Password = Helper.Encrypt(changePassword.ConfirmPassword, "sblw-3hn8-sqoy19");
                        dbContext.tblUsers.Attach(user);
                        dbContext.Entry(user).Property(x => x.Password).IsModified = true;
                        dbContext.SaveChanges();
                        returnObject.ReturnMessage = "Success";
                    }
                    else
                    {
                        returnObject.ReturnMessage = "User does not exist";
                    }
                    return returnObject;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                ChangePassword returnObject = new ChangePassword();
                returnObject.ReturnMessage = "Error!! Please try again later";
                return returnObject;
            }
        }

        public int ValidatePassword(string oldPassword, int UserID)
        {
            try
            {
                using (mwbtDealerEntities dbcontext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbcontext.Database.BeginTransaction())
                    {
                        oldPassword = Helper.Encrypt(oldPassword, "sblw-3hn8-sqoy19");

                        var IsValueexists = dbcontext.tblUsers.Where(u => u.ID == UserID && u.Password == oldPassword).FirstOrDefault();

                        if (IsValueexists != null)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

using JBNWebAPI.Logger;

namespace JBNClassLibrary
{
    public class UserCreation
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        [Required(ErrorMessage = "Please enter User Name")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter Full Name ")]
        public string FullName { get; set; }
        [Display(Name = "MNumber")]
        [Required(ErrorMessage = "Please enter Mobile Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string MNumber { get; set; }

        [Required(ErrorMessage = "Please enter the password")]
        [Display(Name = "Password")] //makes column title not split
        public string Password { get; set; }

        public string UserType { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Please select the Role")]
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string CreatedOn { get; set; }

    }

    public class AdminUserCreation
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public List<UserCreation> GetUsers()
        {
            List<UserCreation> user = new List<UserCreation>();
            user = (from u in dbContext.tblUsers
                    join r in dbContext.tblSysRoles on u.RoleID equals r.ID
                    select new UserCreation
                    {
                        UserID = u.ID,
                        Username = u.Username,
                        FullName = u.FullName,
                        MNumber = u.MNumber,
                        UserType = u.UserType,
                        Password = u.Password,
                        RoleID = u.RoleID.Value,
                        RoleName = r.RoleName,
                        IsActive = u.IsActive,

                    }).ToList();

            return user;
        }

        public UserCreation GetUsersById(int id)
        {
            UserCreation user = new UserCreation();

            user = (from u in dbContext.tblUsers
                    where u.ID == id
                    select new UserCreation
                    {
                        UserID = u.ID,
                        Username = u.Username,
                        FullName = u.FullName,
                        MNumber = u.MNumber,
                        UserType = u.UserType,
                        Password = u.Password,
                        IsActive = u.IsActive,
                        RoleID = u.RoleID.Value,
                    }).FirstOrDefault();

            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = Helper.Decrypt(user.Password, "sblw-3hn8-sqoy19");
            }

            return user;
        }
        public bool AddWebUsers(UserCreation udetails, string UserID)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    int count = dbContext.tblUsers.AsNoTracking().Count();
                    var IsValueExists = dbContext.tblUsers.AsNoTracking().Where(p => p.ID == udetails.UserID).FirstOrDefault();
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    if (IsValueExists != null)
                    {
                        tblUser tblUser = new tblUser();
                        tblUser.ID = udetails.UserID;
                        tblUser.Username = udetails.Username;
                        tblUser.FullName = udetails.FullName;
                        tblUser.Password = Helper.Encrypt(udetails.Password, "sblw-3hn8-sqoy19");
                        tblUser.MNumber = udetails.MNumber;
                        tblUser.UserType = udetails.UserType;
                        tblUser.IsActive = udetails.IsActive;
                        tblUser.RoleID = udetails.RoleID;
                        tblUser.CreatedDate = IsValueExists.CreatedDate;
                        tblUser.CreatedBy = IsValueExists.CreatedBy;
                        tblUser.ModifiedBy = Convert.ToInt32(UserID);
                        tblUser.ModifiedDate = DateTimeNow;
                        dbContext.Entry(tblUser).State = EntityState.Modified;

                        //Insert into History Table
                        tblHistory history = new tblHistory();
                        history.UserID = Convert.ToInt32(UserID);
                        history.CustID = udetails.UserID;
                        history.ProductCategory = udetails.FullName;
                        history.CreatedDate = DateTimeNow;
                        history.CreatedBy = Convert.ToInt32(UserID);
                        history.ActivityPage = "weebusers";
                        history.ActivityType = "create";
                        history.Comments = "Web User Created";
                        dbContext.tblHistories.Add(history);

                        dbContext.SaveChanges();
                    }
                    else
                    {
                        tblUser tblUser = new tblUser();
                        tblUser.Username = udetails.Username;
                        tblUser.FullName = udetails.FullName;
                        tblUser.Password = Helper.Encrypt(udetails.Password, "sblw-3hn8-sqoy19");
                        tblUser.MNumber = udetails.MNumber;
                        tblUser.UserType = udetails.UserType;
                        tblUser.IsActive = udetails.IsActive;
                        tblUser.RoleID = udetails.RoleID;
                        tblUser.CreatedDate = DateTimeNow;
                        tblUser.CreatedBy = Convert.ToInt32(UserID);
                        dbContext.tblUsers.Add(tblUser);

                        //Insert into History Table
                        tblHistory history = new tblHistory();
                        history.UserID = Convert.ToInt32(UserID);
                        history.CustID = udetails.UserID;
                        history.ProductCategory = udetails.FullName;
                        history.CreatedDate = DateTimeNow;
                        history.CreatedBy = Convert.ToInt32(UserID);
                        history.ActivityPage = "weebusers";
                        history.ActivityType = "update";
                        history.Comments = "Web User Updated";
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
        public List<DLRoleCreation> GetRolesData()
        {
            List<DLRoleCreation> dLRolesLst = new List<DLRoleCreation>();
            dLRolesLst = (from role in dbContext.tblSysRoles.AsNoTracking()
                          select new DLRoleCreation()
                          {
                              RoleID = role.ID,
                              RoleName = role.RoleName.Trim(),
                          }).ToList();
            return dLRolesLst;
        }
        public int CheckUsernameAvailability(string userName)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    var SeachData = dbContext.tblUsers.Where(x => x.Username == userName).SingleOrDefault();
                    if (SeachData != null)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException == null ? null : ex.InnerException, ex.StackTrace);
                return 0;
            }
        }
    }
}

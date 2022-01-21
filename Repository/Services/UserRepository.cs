using JBNClassLibrary;
using JBNWebAPI.Logger;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Services
{
    public class UserRepository : IUserRepository
    {
        public async Task<Login> UserLogin(Login userLogin)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    userLogin.Password = Helper.Encrypt(userLogin.Password, "sblw-3hn8-sqoy19");
                    Login user = await (from u in dbContext.tblUsers
                                        where u.Username == userLogin.Email
                                        && userLogin.Password == u.Password
                                        && u.IsActive == true
                                        select new Login
                                        {
                                            UserID = u.ID,
                                            Email = u.Username,
                                            Password = u.Password,
                                            FullName = u.FullName,
                                            RoleID = u.RoleID,
                                        }).FirstOrDefaultAsync();
                    return user;
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

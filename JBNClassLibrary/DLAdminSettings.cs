using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class DLAdminSettings
    {
        public AdminSettings GetAdminSettings()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    AdminSettings adminSettings = new AdminSettings();
                    adminSettings = (from a in dbContext.tblAdminSettings
                                     select new AdminSettings
                                     {
                                         ID = a.ID,
                                         AddDaysForSearch = a.AddDaysForSearch,
                                         AdDurationInSeconds = a.AdDurationInSeconds,
                                         FestDays = a.FestDays,
                                         HoursOfExpiry = a.HoursOfExpiry,
                                         MaxDurationsAllowedPerHr = a.MaxDurationsAllowedPerHr,
                                         WeekendMatrix = a.WeekendMatrix
                                     }).FirstOrDefault();
                    return adminSettings;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public AdminSettings UpdateAdminSettings(AdminSettings adminSettings)
        {
            AdminSettings Result = new AdminSettings();
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    var IsExists = dbContext.tblAdminSettings.AsNoTracking().Where(a => a.ID == adminSettings.ID).FirstOrDefault();
                    if(IsExists != null)
                    {
                        tblAdminSetting tblAdminSetting = new tblAdminSetting();
                        tblAdminSetting.ID = adminSettings.ID;
                        tblAdminSetting.AddDaysForSearch = adminSettings.AddDaysForSearch;
                        tblAdminSetting.AdDurationInSeconds = adminSettings.AdDurationInSeconds;
                        tblAdminSetting.FestDays = adminSettings.FestDays;
                        tblAdminSetting.HoursOfExpiry = adminSettings.HoursOfExpiry;
                        tblAdminSetting.MaxDurationsAllowedPerHr = adminSettings.MaxDurationsAllowedPerHr;
                        tblAdminSetting.WeekendMatrix = adminSettings.WeekendMatrix;
                        dbContext.tblAdminSettings.Add(tblAdminSetting);
                        dbContext.Entry(tblAdminSetting).State = EntityState.Modified;
                        dbContext.SaveChanges();
                        Result.DisplayMessage = "Your settings updated Successfully";
                    }
                    else
                    {
                        Result.DisplayMessage = "Bad Request";
                    }

                    
                    return Result;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.DisplayMessage = "Error!! Please contact administrator";
                return Result;
            }
        }
    }

    public class AdminSettings
    {
        public int ID { get; set; }
        public Nullable<int> AddDaysForSearch { get; set; }
        public Nullable<int> AdDurationInSeconds { get; set; }
        public Nullable<int> MaxDurationsAllowedPerHr { get; set; }
        public Nullable<int> HoursOfExpiry { get; set; }
        public Nullable<double> WeekendMatrix { get; set; }
        public Nullable<int> FestDays { get; set; }
        public string DisplayMessage { get; set; }
    }
}

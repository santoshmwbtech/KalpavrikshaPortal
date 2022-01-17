using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net.Mail;
using Korzh.EasyQuery.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;

namespace JBNClassLibrary
{
    public class DLAdvertisements
    {
        mwbtDealerEntities dbContext = new mwbtDealerEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        int IsLogWrite = Convert.ToInt32(ConfigurationManager.AppSettings["IsLogWrite"].ToString());
        JBNDBClass jBNDBClass = new JBNDBClass();
        public List<ItemCategory> GetAllItems(string SearchText)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<ItemCategory> itemCategories = new List<ItemCategory>();
                    itemCategories = (from c in dbContext.ProductsViews
                                      where c.ItemName.ToLower().Contains(SearchText.ToLower())
                                      select new ItemCategory
                                      {
                                          ID = c.ItemID,
                                          ItemName = c.ItemName,
                                      }).ToList();
                    return itemCategories;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<tblAdvertisementArea> GetAdvertisementAreas()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<tblAdvertisementArea> tblAdvertisementAreas = new List<tblAdvertisementArea>();
                    tblAdvertisementAreas = dbContext.tblAdvertisementAreas.Where(a => a.IsActive == true).ToList();
                    return tblAdvertisementAreas;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<tblAdvertisementType> GetAdvertisementTypes()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<tblAdvertisementType> advertisementType = new List<tblAdvertisementType>();
                    advertisementType = dbContext.tblAdvertisementTypes.Where(a => a.IsActive == true).ToList();
                    return advertisementType;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<tblAdvertisementTimeSlot> GetAdTimeSlots()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<tblAdvertisementTimeSlot> tblAdvertisementTimeSlots = new List<tblAdvertisementTimeSlot>();
                    tblAdvertisementTimeSlots = dbContext.tblAdvertisementTimeSlots.Where(a => a.IsActive == true).ToList();
                    return tblAdvertisementTimeSlots;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<ItemCategory> GetAdProducts()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    var AdProducts = (from c in dbContext.tblItemCategories // tblChildCategories
                                      join a in dbContext.tblAdvertisementMains on c.ID equals a.ProductID

                                      select new ItemCategory
                                      {
                                          ItemName = c.ItemName
                                      }).Distinct().ToList();
                    return AdProducts;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public AdvertisementMain CheckSlotAvailability(Advertisement advertisement)
        {
            AdvertisementMain Result = new AdvertisementMain();
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(advertisement);
                    if (IsLogWrite == 1)
                        Helper.TransactionLog(json1, 0);

                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        var advertisements = dbContext.tblAdvertisements;
                        DateTime FromDate = Convert.ToDateTime(advertisement.FromDate);
                        DateTime ToDate = Convert.ToDateTime(advertisement.ToDate);
                        int TotalDays = (ToDate - FromDate).Days == 0 ? 1 : (ToDate - FromDate).Days;
                        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        var ResultItems = new AdvertisementMain();
                        ResultItems.AdvertisementArea = dbContext.tblAdvertisementAreas.Where(ar => ar.ID == advertisement.AdvertisementAreaID).FirstOrDefault().AdvertisementAreaName;
                        ResultItems.ProductName = dbContext.tblChildCategories.Where(c => c.ID == advertisement.ProductID).FirstOrDefault().ChildCategoryName; //.ItemName


                        //Loop through each slot
                        if (advertisement.slots != null && advertisement.slots.Count() > 0)
                        {
                            foreach (var item in advertisement.slots)
                            {
                                TimeSpan TimeSlotFromTime = dbContext.tblAdvertisementTimeSlots.Where(t => t.ID == item.ID).FirstOrDefault().FromTime.Value;
                                TimeSpan TimeSlotToTime = dbContext.tblAdvertisementTimeSlots.Where(t => t.ID == item.ID).FirstOrDefault().ToTime.Value;
                                TimeSpan TimeSlotMinutes = (TimeSlotToTime - TimeSlotFromTime);
                                int TimeSlotSeconds = Convert.ToInt32(TimeSlotMinutes.TotalMinutes) * 60;
                                int TotalHoursPerSlot = Convert.ToInt32(TimeSlotMinutes.Hours);
                                int AdDurationInSeconds = dbContext.tblAdminSettings.FirstOrDefault().AdDurationInSeconds.Value;
                                int TotalIntervalsOfSlot = TimeSlotSeconds / AdDurationInSeconds;

                                //int TotalIntevals = advertisement.IntervalsPerDay.Value * TotalDays * AdDurationInSeconds;
                                List<Advertisement> advertisementList = new List<Advertisement>();
                                if (ToDate > FromDate)
                                {
                                    for (DateTime iDate = FromDate; iDate <= ToDate; iDate = iDate.AddDays(1))
                                    {
                                        int AdvertisementAreaID = dbContext.tblAdvertisementAreas.Where(at => at.AdvertisementAreaName.ToLower() == "national level").FirstOrDefault().ID;



                                        advertisementList = (from a in advertisements.ToList()
                                                             where a.FromDate.Value.Date == iDate.Date && a.ProductID == advertisement.ProductID
                                                             && a.AdvertisementAreaID == advertisement.AdvertisementAreaID
                                                             && a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID && a.AdTimeSlotID == item.ID
                                                             select new Advertisement
                                                             {
                                                                 AdvertisementID = a.ID,
                                                                 IntervalsPerDay = a.IntervalsPerDay,
                                                             }).ToList();

                                        #region
                                        //if(advertisement.AdvertisementAreaID != AdvertisementAreaID)
                                        //{

                                        //}

                                        //Get if national level ad is running
                                        //advertisementList = (from a in advertisements.ToList()
                                        //                     where a.FromDate.Value.Date == iDate.Date && a.ProductID == advertisement.ProductID && a.AdvertisementAreaID == AdvertisementAreaID
                                        //                     && a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID && a.AdTimeSlotID == item.ID
                                        //                     select new Advertisement
                                        //                     {
                                        //                         AdvertisementID = a.AdvertisementID,
                                        //                         IntervalsPerHour = a.IntervalsPerDay,
                                        //                     }).ToList();
                                        //if (advertisementList != null && advertisementList.Count() > 0)
                                        //{
                                        //    Result.StatusCode = HttpStatusCode.Conflict;
                                        //    Result.DispayMessage += " Slots already booked on " + iDate.ToString("dd/MM/yyyy") + " for" + item.TimeSlotName;
                                        //    break;
                                        //}

                                        //var stateList = (from a in advertisements.ToList()
                                        //                 join st in dbContext.tblAdvertisementStates on a.AdvertisementID equals st.AdvertisementID
                                        //                 where a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID && a.AdTimeSlotID == item.ID
                                        //                 && a.FromDate.Value.Date == iDate.Date && a.AdvertisementAreaID == advertisement.AdvertisementAreaID
                                        //                 select new tblAdvertisementState
                                        //                 {
                                        //                     StateID = st.StateID,
                                        //                     StateName = st.StateName
                                        //                 }).ToList();

                                        //var BookedStateList = stateList.Where(st => advertisement.advertisementStates.All(ast => ast.StateID == st.StateID)).ToList();
                                        //if (BookedStateList != null && BookedStateList.Count() > 0)
                                        //{
                                        //    foreach (var itemSBooked in BookedStateList)
                                        //    {
                                        //        Result.StatusCode = HttpStatusCode.Conflict;
                                        //        Result.DispayMessage += " Slots not available in State " + itemSBooked.StateName + "for" + item.TimeSlotName;
                                        //        break;
                                        //    }
                                        //}

                                        //var districtList = (from a in advertisements.ToList()
                                        //                    join st in dbContext.tblAdvertisementDistricts on a.AdvertisementID equals st.AdvertisementID
                                        //                    where a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID && a.AdTimeSlotID == item.ID
                                        //                    && a.FromDate.Value.Date == iDate.Date && a.AdvertisementAreaID == advertisement.AdvertisementAreaID
                                        //                    select new tblAdvertisementDistrict
                                        //                    {
                                        //                        DistrictID = st.DistrictID,
                                        //                        DistrictName = st.DistrictName,
                                        //                    }).ToList();

                                        //var BookedDistrictList = districtList.Where(dt => advertisement.advertisementDistricts.All(adt => adt.DistrictID == dt.DistrictID)).ToList();
                                        //if (BookedDistrictList != null && BookedDistrictList.Count() > 0)
                                        //{
                                        //    foreach (var itemDBooked in BookedDistrictList)
                                        //    {
                                        //        Result.StatusCode = HttpStatusCode.Conflict;
                                        //        Result.DispayMessage += " Slots not available in District " + itemDBooked.DistrictName + "for" + item.TimeSlotName;
                                        //        break;
                                        //    }
                                        //}

                                        //var cityList = (from a in advertisements.ToList()
                                        //                join st in dbContext.tblAdvertisementCities on a.AdvertisementID equals st.AdvertisementID
                                        //                where a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID && a.AdTimeSlotID == item.ID
                                        //                && a.FromDate.Value.Date == iDate.Date && a.AdvertisementAreaID == advertisement.AdvertisementAreaID
                                        //                select new tblAdvertisementCity
                                        //                {
                                        //                    StateWithCityID = st.StateWithCityID,
                                        //                    VillageLocalityName = st.VillageLocalityName
                                        //                }).ToList();

                                        //var BookedCityList = cityList.Where(ct => advertisement.advertisementCities.All(act => act.StateWithCityID == ct.StateWithCityID)).ToList();
                                        //if (BookedCityList != null && BookedCityList.Count() > 0)
                                        //{
                                        //    foreach (var itemCBooked in BookedCityList)
                                        //    {
                                        //        Result.StatusCode = HttpStatusCode.Conflict;
                                        //        Result.DispayMessage += " Slots not available in City " + itemCBooked.VillageLocalityName + "for" + item.TimeSlotName;
                                        //        break;
                                        //    }
                                        //}
                                        #endregion

                                        int TotalIntevals = advertisement.IntervalsPerHour.Value * TotalHoursPerSlot * AdDurationInSeconds;
                                        int TotalCurrentIntervals = advertisementList.Sum(c => c.IntervalsPerDay).Value;
                                        int IntervalsAfterAdd = TotalIntervalsOfSlot + TotalCurrentIntervals;

                                        if ((TotalIntevals + TotalCurrentIntervals) > TotalIntervalsOfSlot)
                                        {
                                            Result.StatusCode = HttpStatusCode.Conflict;
                                            Result.DispayMessage += " Slots not available on " + iDate.ToString() + "for" + item.TimeSlotName;
                                        }
                                        else
                                        {
                                            Result.StatusCode = HttpStatusCode.OK;
                                            Result.DispayMessage += " Slots available on " + iDate.ToString() + "for" + item.TimeSlotName;
                                        }
                                    }
                                }
                                else
                                {
                                    int AdvertisementAreaID = dbContext.tblAdvertisementAreas.Where(at => at.AdvertisementAreaName.ToLower() == "national level").FirstOrDefault().ID;
                                    int AdvertisementType = dbContext.tblAdvertisementTypes.Where(a => a.Type.ToLower() == "fullpagead").FirstOrDefault().ID;

                                    if (advertisement.TypeOfAdvertisementID == AdvertisementType)
                                    {
                                        //Get if national level ad is running

                                        advertisementList = (from a in advertisements.ToList()
                                                             where a.FromDate.Value.Date == FromDate.Date && a.ProductID == advertisement.ProductID
                                                             && a.AdvertisementAreaID == AdvertisementAreaID && a.AdTimeSlotID == advertisement.AdTimeSlotID
                                                             && a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID
                                                             select new Advertisement
                                                             {
                                                                 AdvertisementID = a.ID,
                                                                 IntervalsPerHour = a.IntervalsPerDay,
                                                             }).ToList();
                                        if (advertisementList != null && advertisementList.Count() > 0)
                                        {
                                            Result.StatusCode = HttpStatusCode.Conflict;
                                            Result.DispayMessage = " Slots already booked on " + FromDate.ToString();
                                            return Result;
                                        }
                                    }

                                    advertisementList = (from a in advertisements.ToList()
                                                         where a.FromDate.Value.Date == FromDate.Date && a.ProductID == advertisement.ProductID && a.AdvertisementAreaID == advertisement.AdvertisementAreaID && a.AdTimeSlotID == item.ID
                                                         && a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID
                                                         select new Advertisement
                                                         {
                                                             AdvertisementID = a.ID,
                                                             IntervalsPerDay = a.IntervalsPerDay,
                                                         }).ToList();

                                    #region
                                    //var stateList = (from a in advertisements.ToList()
                                    //                 join st in dbContext.tblAdvertisementStates on a.AdvertisementID equals st.AdvertisementID
                                    //                 where a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID && a.AdTimeSlotID == item.ID
                                    //                 && a.FromDate.Value.Date == FromDate.Date && a.AdvertisementAreaID == advertisement.AdvertisementAreaID
                                    //                 select new tblAdvertisementState
                                    //                 {
                                    //                     StateID = st.StateID,
                                    //                     StateName = st.StateName
                                    //                 }).ToList();

                                    //var BookedStateList = stateList.Where(st => advertisement.advertisementStates.All(ast => ast.StateID == st.StateID)).ToList();
                                    //if (BookedStateList != null && BookedStateList.Count() > 0)
                                    //{
                                    //    foreach (var itemSBooked in BookedStateList)
                                    //    {
                                    //        Result.StatusCode = HttpStatusCode.Conflict;
                                    //        Result.DispayMessage += " Slots not available in State " + itemSBooked.StateName;
                                    //        break;
                                    //    }
                                    //    return Result;
                                    //}

                                    //var districtList = (from a in advertisements.ToList()
                                    //                    join st in dbContext.tblAdvertisementDistricts on a.AdvertisementID equals st.AdvertisementID
                                    //                    where a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID && a.AdTimeSlotID == item.ID
                                    //                    && a.FromDate.Value.Date == FromDate.Date && a.AdvertisementAreaID == advertisement.AdvertisementAreaID
                                    //                    select new tblAdvertisementDistrict
                                    //                    {
                                    //                        DistrictID = st.DistrictID,
                                    //                        DistrictName = st.DistrictName
                                    //                    }).ToList();

                                    //var BookedDistrictList = districtList.Where(dt => advertisement.advertisementDistricts.All(adt => adt.DistrictID == dt.DistrictID)).ToList();
                                    //if (BookedDistrictList != null && BookedDistrictList.Count() > 0)
                                    //{
                                    //    foreach (var itemDBooked in BookedDistrictList)
                                    //    {
                                    //        Result.StatusCode = HttpStatusCode.Conflict;
                                    //        Result.DispayMessage += " Slots not available in District " + itemDBooked.DistrictName;
                                    //        break;
                                    //    }
                                    //    return Result;
                                    //}

                                    //var cityList = (from a in advertisements.ToList()
                                    //                join st in dbContext.tblAdvertisementCities on a.AdvertisementID equals st.AdvertisementID
                                    //                where a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID && a.AdTimeSlotID == item.ID
                                    //                && a.FromDate.Value.Date == FromDate.Date && a.AdvertisementAreaID == advertisement.AdvertisementAreaID
                                    //                select new tblAdvertisementCity
                                    //                {
                                    //                    StateWithCityID = st.StateWithCityID,
                                    //                    VillageLocalityName = st.VillageLocalityName
                                    //                }).ToList();

                                    //var BookedCityList = cityList.Where(ct => advertisement.advertisementCities.All(act => act.StateWithCityID == ct.StateWithCityID)).ToList();
                                    //if (BookedCityList != null && BookedCityList.Count() > 0)
                                    //{
                                    //    foreach (var itemCBooked in BookedCityList)
                                    //    {
                                    //        Result.StatusCode = HttpStatusCode.Conflict;
                                    //        Result.DispayMessage += " Slots not available in City " + itemCBooked.VillageLocalityName;
                                    //        break;
                                    //    }
                                    //    return Result;
                                    //}
                                    #endregion

                                    int TotalIntevals = advertisement.IntervalsPerHour.Value * TotalHoursPerSlot * AdDurationInSeconds;
                                    int TotalCurrentIntervals = advertisementList.Sum(c => c.IntervalsPerDay).Value;
                                    int IntervalsAfterAdd = TotalIntervalsOfSlot + TotalCurrentIntervals;

                                    if ((TotalIntevals + TotalCurrentIntervals) > TotalIntervalsOfSlot)
                                    {
                                        Result.StatusCode = HttpStatusCode.Conflict;
                                        Result.DispayMessage += " Slots not available on " + advertisement.FromDate.Value.ToString() + "for" + item.TimeSlotName;
                                    }
                                    else
                                    {
                                        Result.StatusCode = HttpStatusCode.OK;
                                        Result.DispayMessage += " Slots available on " + advertisement.FromDate.Value.ToString() + "for" + item.TimeSlotName;
                                    }

                                    //get advertisement if already booked for the same day
                                    //var advertisementbooked = advertisements.ToList().Where(a => a.FromDate.Value.Date == FromDate.Date && a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID && a.AdvertisementAreaID == advertisement.AdvertisementAreaID && a.ProductID == advertisement.ProductID && a.AdTimeSlotID == item.ID).FirstOrDefault();

                                    //if (advertisementbooked != null)
                                    //{
                                    //    Result.StatusCode = HttpStatusCode.Conflict;
                                    //    Result.DispayMessage += " Slots not available on " + advertisement.FromDate.Value.ToString("dd/MM/yyyy") + " and for time slot " + item.TimeSlotName;
                                    //    return Result;
                                    //}
                                    //else
                                    //{
                                    //    Result.StatusCode = HttpStatusCode.OK;
                                    //    Result.DispayMessage += " Slots available on " + FromDate.ToString();
                                    //}
                                }
                            }
                        }
                        else
                        {
                            #region old code
                            //List<Advertisement> advertisementList = new List<Advertisement>();
                            //int AdvertisementAreaID = dbContext.tblAdvertisementAreas.Where(at => at.AdvertisementAreaName.ToLower() == "national level").FirstOrDefault().AdvertisementAreaID;
                            ////Get if national level ad is running
                            ////advertisementList = (from a in advertisements.ToList()
                            ////                     where a.FromDate.Value.Date == FromDate.Date && a.ProductID == advertisement.ProductID && a.AdvertisementAreaID == AdvertisementAreaID
                            ////                     && a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID
                            ////                     select new Advertisement
                            ////                     {
                            ////                         AdvertisementID = a.AdvertisementID,
                            ////                         IntervalsPerHour = a.IntervalsPerDay,
                            ////                     }).ToList();
                            ////if (advertisementList != null && advertisementList.Count() > 0)
                            ////{
                            ////    Result.StatusCode = HttpStatusCode.Conflict;
                            ////    Result.DispayMessage = " Slots already booked on " + FromDate.ToString();
                            ////    return Result;
                            ////}

                            //advertisementList = (from a in advertisements.ToList()
                            //                     where a.FromDate.Value.Date == FromDate.Date && a.ProductID == advertisement.ProductID && a.AdvertisementAreaID == advertisement.AdvertisementAreaID
                            //                     && a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID
                            //                     select new Advertisement
                            //                     {
                            //                         AdvertisementID = a.AdvertisementID,
                            //                         IntervalsPerDay = a.IntervalsPerDay,
                            //                     }).ToList();

                            //var stateList = (from a in advertisements.ToList()
                            //                 join st in dbContext.tblAdvertisementStates on a.AdvertisementID equals st.AdvertisementID
                            //                 where a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID
                            //                 && a.FromDate.Value.Date == FromDate.Date && a.AdvertisementAreaID == advertisement.AdvertisementAreaID
                            //                 select new tblAdvertisementState
                            //                 {
                            //                     StateID = st.StateID,
                            //                     StateName = st.StateName
                            //                 }).ToList();
                            //var BookedStateList = stateList.Where(st => advertisement.advertisementStates.All(ast => ast.StateID == st.StateID)).ToList();
                            //if (BookedStateList != null && BookedStateList.Count() > 0)
                            //{
                            //    foreach (var itemSBooked in BookedStateList)
                            //    {
                            //        Result.StatusCode = HttpStatusCode.Conflict;
                            //        Result.DispayMessage += " Slots not available in State " + itemSBooked.StateName;
                            //        break;
                            //    }
                            //    return Result;
                            //}

                            //var districtList = (from a in advertisements.ToList()
                            //                    join st in dbContext.tblAdvertisementDistricts on a.AdvertisementID equals st.AdvertisementID
                            //                    where a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID
                            //                    && a.FromDate.Value.Date == FromDate.Date && a.AdvertisementAreaID == advertisement.AdvertisementAreaID
                            //                    select new tblAdvertisementDistrict
                            //                    {
                            //                        DistrictID = st.DistrictID,
                            //                        DistrictName = st.DistrictName
                            //                    }).ToList();
                            //var BookedDistrictList = districtList.Where(dt => advertisement.advertisementDistricts.All(adt => adt.DistrictID == dt.DistrictID)).ToList();
                            //if (BookedDistrictList != null && BookedDistrictList.Count() > 0)
                            //{
                            //    foreach (var itemDBooked in BookedDistrictList)
                            //    {
                            //        Result.StatusCode = HttpStatusCode.Conflict;
                            //        Result.DispayMessage += " Slots not available in District " + itemDBooked.DistrictName;
                            //        break;
                            //    }
                            //    return Result;
                            //}

                            //var cityList = (from a in advertisements.ToList()
                            //                join st in dbContext.tblAdvertisementCities on a.AdvertisementID equals st.AdvertisementID
                            //                where a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID
                            //                && a.FromDate.Value.Date == FromDate.Date && a.AdvertisementAreaID == advertisement.AdvertisementAreaID
                            //                select new tblAdvertisementCity
                            //                {
                            //                    StateWithCityID = st.StateWithCityID,
                            //                    VillageLocalityName = st.VillageLocalityName
                            //                }).ToList();
                            //var BookedCityList = cityList.Where(ct => advertisement.advertisementCities.All(act => act.StateWithCityID == ct.StateWithCityID)).ToList();
                            //if (BookedCityList != null && BookedCityList.Count() > 0)
                            //{
                            //    foreach (var itemCBooked in BookedCityList)
                            //    {
                            //        Result.StatusCode = HttpStatusCode.Conflict;
                            //        Result.DispayMessage += " Slots not available in City " + itemCBooked.VillageLocalityName;
                            //        break;
                            //    }
                            //    return Result;
                            //}

                            ////get advertisement if already booked for the same day
                            //var advertisementbooked = advertisements.ToList().Where(a => a.FromDate.Value.Date == FromDate.Date && a.TypeOfAdvertisementID == advertisement.TypeOfAdvertisementID && a.AdvertisementAreaID == advertisement.AdvertisementAreaID && a.ProductID == advertisement.ProductID).FirstOrDefault();
                            //if (advertisementbooked != null)
                            //{
                            //    Result.StatusCode = HttpStatusCode.Conflict;
                            //    Result.DispayMessage += " Slots not available on " + advertisement.FromDate.Value.ToString("dd/MM/yyyy");
                            //    return Result;
                            //}
                            //else
                            //{
                            //    Result.StatusCode = HttpStatusCode.OK;
                            //    Result.DispayMessage += " Slots available on " + FromDate.ToString();
                            //}
                            #endregion old code
                        }

                        if (Result.StatusCode == HttpStatusCode.OK)
                        {
                            if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                                dbContext.Database.Connection.Open();
                            double DiscountAmount = 0, TotalPrice = 0, FinalPrice = 0, TaxAmount = 0;
                            bool IsIntraState = false;
                            double TotalStates = 0, TotalDistricts = 0, TotalCities = 0;

                            string AdvertisementType = dbContext.tblAdvertisementTypes.Where(x => x.ID == advertisement.TypeOfAdvertisementID).FirstOrDefault().Type;

                            //Get Tax details
                            string CustomerState = (from c in dbContext.tblCustomerDetails
                                                    join s in dbContext.tblStates on c.State equals s.StateID
                                                    select s).FirstOrDefault().StateName;
                            string CompanyState = dbContext.tblCompanies.FirstOrDefault().CompanyState;
                            string TaxSlabName = dbContext.tblTaxSlabs.Where(t => t.IsActive == true).FirstOrDefault().TaxSlabName;
                            double TaxSlabValue = dbContext.tblTaxSlabs.Where(t => t.IsActive == true).FirstOrDefault().TaxSlabValue.Value;

                            //Get Configuration Settings
                            //var adminSettings = dbContext.tblAdminSettings.FirstOrDefault();

                            if (CustomerState.ToLower() == CompanyState.ToLower())
                            {
                                IsIntraState = true;
                            }
                            else
                            {
                                IsIntraState = false;
                            }

                            //get configuration settings
                            tblAdminSetting tblAdminSetting = dbContext.tblAdminSettings.FirstOrDefault();
                            List<State> StateList = new List<State>();
                            double FestivalMatrix = 1, NationalHolidayMatrix = 1, WeekendMatrix = 1;

                            if (AdvertisementType.ToLower() == "fullpagead")
                            {
                                List<Advertisement> advertisementLists = new List<Advertisement>();
                                double DiscountPer = 0, TotalPriceOfSlot = 0;

                                foreach (var item in advertisement.slots)
                                {
                                    TimeSpan TimeSlotFromTime = dbContext.tblAdvertisementTimeSlots.AsNoTracking().Where(t => t.ID == item.ID).FirstOrDefault().FromTime.Value;
                                    TimeSpan TimeSlotToTime = dbContext.tblAdvertisementTimeSlots.AsNoTracking().Where(t => t.ID == item.ID).FirstOrDefault().ToTime.Value;

                                    TimeSpan TimeSlotMinutes = (TimeSlotToTime - TimeSlotFromTime);
                                    int TimeSlotSeconds = Convert.ToInt32(TimeSlotMinutes.TotalMinutes) * 60;
                                    int TotalHoursPerSlot = Convert.ToInt32(TimeSlotMinutes.Hours);

                                    double AreaMatrix = dbContext.tblAdvertisementAreas.Where(a => a.ID == advertisement.AdvertisementAreaID).FirstOrDefault().AdAreaMatrix.Value;
                                    double CategoryMatrix = dbContext.tblItemCategories.Where(a => a.ID == advertisement.ProductID).FirstOrDefault().ItemMatrix.Value;
                                    double AdTypeMatrix = dbContext.tblAdvertisementTypes.Where(a => a.ID == advertisement.TypeOfAdvertisementID).FirstOrDefault().AdTypeMatrix.Value;
                                    double TimeSlotMatrix = dbContext.tblAdvertisementTimeSlots.AsNoTracking().Where(a => a.ID == item.ID).FirstOrDefault().TimeSlotMatrix.Value;
                                    double TotalIntervalsPerHour = 1;
                                    //double TotalIntervalsPerDay = 1;
                                    double StateMatrix = 1, DistrictMatrix = 1, CityMatrix = 1, StatesCount = 1, DistrictsCount = 1, CitiesCount = 1;

                                    DateTime FestivalDate = advertisement.FromDate.Value.AddDays(Convert.ToDouble(tblAdminSetting.FestDays.Value)).Date;
                                    DateTime MFromDate = advertisement.FromDate.Value.Date;
                                    var advertisementHoliday = dbContext.tblAdvertisementHolidays.Where(h => h.HolidayDate >= MFromDate && h.HolidayDate <= FestivalDate && h.HolidayYear == DateTime.Now.Year.ToString()).ToList();

                                    if (advertisement.advertisementStates != null && advertisement.advertisementStates.Count() > 0)
                                    {
                                        StatesCount = advertisement.advertisementStates.Count();
                                        List<State> StateMatrixList = (from s in dbContext.tblStates.ToList()
                                                                       join t in dbContext.tblTairTypeOfStates on s.TairTypeID equals t.ID
                                                                       join asl in advertisement.advertisementStates on s.StateID equals asl.StateID
                                                                       select new State
                                                                       {
                                                                           StateID = s.StateID,
                                                                           Matrix = t.TairTypeOfStateMatrix
                                                                       }).ToList();
                                        StateMatrix = Convert.ToDouble(StateMatrixList.Sum(x => x.Matrix));
                                        TotalStates = StatesCount;
                                        StateList.AddRange(StateMatrixList);
                                    }

                                    if (advertisement.advertisementDistricts != null && advertisement.advertisementDistricts.Count() > 0)
                                    {
                                        DistrictsCount = advertisement.advertisementDistricts.Count();
                                        List<State> DistrictMatrixList = (from s in dbContext.tblDistricts.ToList()
                                                                          join t in dbContext.tblTairTypeOfDistricts on s.TairTypeOfDistrictID equals t.ID
                                                                          join adl in advertisement.advertisementDistricts on s.DistrictID equals adl.DistrictID
                                                                          select new State
                                                                          {
                                                                              StateID = s.StateID,
                                                                              Matrix = t.TairTypeOfDistrictMatrix
                                                                          }).ToList();
                                        DistrictMatrix = Convert.ToDouble(DistrictMatrixList.Sum(x => x.Matrix));
                                        TotalDistricts = DistrictsCount;
                                        StateList.AddRange(DistrictMatrixList);
                                    }

                                    if (advertisement.advertisementCities != null && advertisement.advertisementCities.Count() > 0)
                                    {
                                        CitiesCount = advertisement.advertisementCities.Count();
                                        List<State> CityMatrixList = (from s in dbContext.tblStateWithCities.ToList()
                                                                      join t in dbContext.tblTairTypeOfCities on s.TairTypeOfCityID equals t.ID
                                                                      join acl in advertisement.advertisementCities on s.StatewithCityID equals acl.StateWithCityID
                                                                      select new State
                                                                      {
                                                                          StateID = s.StateID,
                                                                          Matrix = t.TairTypeOfCityMatrix
                                                                      }).ToList();
                                        CityMatrix = Convert.ToDouble(CityMatrixList.Sum(x => x.Matrix));
                                        TotalCities = CitiesCount;
                                        StateList.AddRange(CityMatrixList);
                                    }

                                    if (StateList != null && StateList.Count() > 0)
                                    {
                                        StateList = StateList.GroupBy(i => i.StateID).Select(i => i.FirstOrDefault()).ToList();
                                        var advertisementHolidays = (from a in advertisementHoliday
                                                                     join b in StateList on a.StateID equals b.StateID
                                                                     select new AdvertisementHoliday
                                                                     {
                                                                         DayName = a.DayName,
                                                                         HolidayDate = a.HolidayDate,
                                                                         HolidayDefinition = a.HolidayDefinition,
                                                                         HolidayMatrix = a.HolidayMatrix,
                                                                         HolidayName = a.HolidayName,
                                                                         HolidayType = a.HolidayType,
                                                                         StateID = a.StateID
                                                                     }).ToList();
                                        if (advertisementHolidays != null && advertisementHolidays.Count() > 0)
                                        {
                                            foreach (var adHolidayItem in advertisementHolidays)
                                            {
                                                if (adHolidayItem.HolidayDefinition.ToLower() == "festival")
                                                {
                                                    FestivalMatrix = adHolidayItem.HolidayMatrix.Value;
                                                }
                                                else if (adHolidayItem.HolidayDefinition.ToLower() == "public")
                                                {
                                                    if (FromDate.Date == adHolidayItem.HolidayDate.Value.Date)
                                                    {
                                                        NationalHolidayMatrix = adHolidayItem.HolidayMatrix.Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //Weekend Matrix
                                    string DayofTheWeek = advertisement.FromDate.Value.DayOfWeek.ToString();
                                    if (DayofTheWeek.ToLower() == "saturday" || DayofTheWeek.ToLower() == "sunday")
                                        WeekendMatrix = tblAdminSetting.WeekendMatrix.Value;

                                    TotalPriceOfSlot += NationalHolidayMatrix * FestivalMatrix * WeekendMatrix * AreaMatrix * AdTypeMatrix * CategoryMatrix * CityMatrix * DistrictMatrix * StateMatrix * CitiesCount * DistrictsCount * StatesCount * TotalIntervalsPerHour;

                                    Advertisement SingleItem = new Advertisement();
                                    SingleItem.AdTimeSlotID = item.ID;
                                    SingleItem.AdvertisementAreaID = advertisement.AdvertisementAreaID;
                                    SingleItem.advertisementCities = advertisement.advertisementCities;
                                    SingleItem.advertisementDistricts = advertisement.advertisementDistricts;
                                    SingleItem.AdvertisementName = advertisement.AdvertisementName;
                                    SingleItem.advertisementStates = advertisement.advertisementStates;
                                    SingleItem.BrandID = advertisement.BrandID;
                                    SingleItem.BrandName = advertisement.BrandName;
                                    SingleItem.CreatedBy = advertisement.CreatedBy;
                                    SingleItem.CreatedDate = DateTimeNow;
                                    SingleItem.CustID = advertisement.CustID;
                                    SingleItem.DaysCount = TotalDays;
                                    SingleItem.FromDate = FromDate;
                                    SingleItem.ToDate = ToDate;
                                    SingleItem.IntervalsPerDay = 0;
                                    SingleItem.IntervalsPerHour = 0;
                                    SingleItem.ProductID = advertisement.ProductID;
                                    SingleItem.slots = advertisement.slots;
                                    SingleItem.TotalPrice = TotalPrice;
                                    SingleItem.FinalPrice = FinalPrice;
                                    SingleItem.TotalDiscountAmount = DiscountAmount;
                                    SingleItem.TypeOfAdvertisementID = advertisement.TypeOfAdvertisementID;
                                    SingleItem.IsActive = false;
                                    SingleItem.IsCancelled = false;
                                    SingleItem.IsCompleted = false;
                                    SingleItem.TemporaryBooked = true;
                                    advertisementLists.Add(SingleItem);
                                }

                                TotalPrice += Math.Round(TotalPriceOfSlot, 2);
                                DiscountPer = 0; // Convert.ToDouble(dbContext.tblAdvertisementDiscounts.Where(d => TotalPrice >= d.FromAmount && TotalPrice <= d.ToAmount).FirstOrDefault().DiscountPer);
                                double DiscountAmountOfSlot = (TotalPrice * DiscountPer) / 100;
                                FinalPrice = Math.Round((TotalPrice - DiscountAmountOfSlot), 2);
                                //Calculate the tax
                                TaxAmount = Math.Round(((FinalPrice * TaxSlabValue) / 100), 2);
                                FinalPrice = Math.Round((FinalPrice + TaxAmount), 2);

                                DiscountAmount += Math.Round(DiscountAmountOfSlot, 2);

                                AdvertisementMain main = new AdvertisementMain();
                                main.AdvertisementType = AdvertisementType;
                                main.AdvertisementName = advertisement.AdvertisementName;
                                main.BrandID = advertisement.BrandID;
                                main.BrandName = advertisement.BrandName;
                                main.CustID = advertisement.CustID;
                                main.FromDate = advertisement.FromDate;
                                main.ToDate = advertisement.ToDate;
                                main.TypeOfAdvertisementID = advertisement.TypeOfAdvertisementID;
                                main.ProductID = advertisement.ProductID;
                                main.CreatedBy = advertisement.CreatedBy;
                                main.CreatedDate = DateTimeNow;
                                main.AdTotalPrice = TotalPrice;
                                main.FinalPrice = Math.Round(FinalPrice);
                                main.TotalDiscount = DiscountAmount;
                                main.IsActive = false;
                                main.IsCancelled = false;
                                main.IsCompleted = false;
                                main.TemporaryBooked = true;
                                main.states = advertisement.advertisementStates;
                                main.districts = advertisement.advertisementDistricts;
                                main.cities = advertisement.advertisementCities;
                                main.DiscountPer = DiscountPer;
                                main.TaxAmount = TaxAmount;
                                main.TaxValue = TaxSlabValue;
                                main.AdvertisementAreaID = advertisement.AdvertisementAreaID;
                                main.FestivalMatrix = FestivalMatrix;
                                main.PublicHolidayMatrix = NationalHolidayMatrix;
                                main.WeekendMatrix = WeekendMatrix;
                                if (IsIntraState)
                                {
                                    main.SGSTPer = TaxSlabValue / 2;
                                    main.CGSTPer = TaxSlabValue / 2;
                                    main.SGSTAmount = TaxAmount / 2;
                                    main.CGSTAmount = TaxAmount / 2;
                                    main.IGSTPer = 0;
                                    main.IGSTAmount = 0;
                                }
                                else
                                {
                                    main.SGSTPer = 0;
                                    main.CGSTPer = 0;
                                    main.SGSTAmount = 0;
                                    main.CGSTAmount = 0;
                                    main.IGSTPer = TaxSlabValue;
                                    main.IGSTAmount = TaxAmount;
                                }
                                advertisement.IsActive = false;
                                advertisement.IsCancelled = false;
                                advertisement.IsCompleted = false;
                                advertisement.TemporaryBooked = true;
                                advertisement.IntervalsPerDay = 0;
                                advertisement.IntervalsPerHour = 0;
                                main.advertisementList = advertisementLists;
                                Result = SaveAdvertisement(main);
                            }
                            else
                            {
                                AdvertisementMain main = new AdvertisementMain();
                                List<Advertisement> advertisementLists = new List<Advertisement>();
                                double DiscountPer = 0, TotalPriceOfSlot = 0;
                                foreach (var item in advertisement.slots)
                                {
                                    TimeSpan TimeSlotFromTime = dbContext.tblAdvertisementTimeSlots.AsNoTracking().Where(t => t.ID == item.ID).FirstOrDefault().FromTime.Value;
                                    TimeSpan TimeSlotToTime = dbContext.tblAdvertisementTimeSlots.AsNoTracking().Where(t => t.ID == item.ID).FirstOrDefault().ToTime.Value;

                                    TimeSpan TimeSlotMinutes = (TimeSlotToTime - TimeSlotFromTime);
                                    int TimeSlotSeconds = Convert.ToInt32(TimeSlotMinutes.TotalMinutes) * 60;
                                    int TotalHoursPerSlot = Convert.ToInt32(TimeSlotMinutes.Hours);

                                    double AreaMatrix = dbContext.tblAdvertisementAreas.AsNoTracking().Where(a => a.ID == advertisement.AdvertisementAreaID).FirstOrDefault().AdAreaMatrix.Value;
                                    double CategoryMatrix = dbContext.tblItemCategories.AsNoTracking().Where(a => a.ID == advertisement.ProductID).FirstOrDefault().ItemMatrix.Value;
                                    double AdTypeMatrix = dbContext.tblAdvertisementTypes.AsNoTracking().Where(a => a.ID == advertisement.TypeOfAdvertisementID).FirstOrDefault().AdTypeMatrix.Value;
                                    double TimeSlotMatrix = dbContext.tblAdvertisementTimeSlots.AsNoTracking().Where(a => a.ID == item.ID).FirstOrDefault().TimeSlotMatrix.Value;
                                    double TotalIntervalsPerHour = Convert.ToDouble(advertisement.IntervalsPerHour.Value);
                                    double TotalIntervalsPerDay = Convert.ToDouble((advertisement.IntervalsPerHour.Value * TotalHoursPerSlot));

                                    for (DateTime iDate = FromDate; iDate <= ToDate; iDate = iDate.AddDays(1))
                                    {
                                        double StateMatrix = 1, DistrictMatrix = 1, CityMatrix = 1, StatesCount = 1, DistrictsCount = 1, CitiesCount = 1;

                                        DateTime FestivalDate = iDate.AddDays(Convert.ToDouble(tblAdminSetting.FestDays.Value)).Date;
                                        DateTime MFromDate = iDate.Date;
                                        var advertisementHoliday = dbContext.tblAdvertisementHolidays.Where(h => h.HolidayDate >= MFromDate && h.HolidayDate <= FestivalDate && h.HolidayYear == DateTime.Now.Year.ToString()).ToList();

                                        if (advertisement.advertisementStates != null && advertisement.advertisementStates.Count() > 0)
                                        {
                                            StatesCount = advertisement.advertisementStates.Count();
                                            List<State> StateMatrixList = (from s in dbContext.tblStates.ToList()
                                                                           join t in dbContext.tblTairTypeOfStates on s.TairTypeID equals t.ID
                                                                           join asl in advertisement.advertisementStates on s.StateID equals asl.StateID
                                                                           select new State
                                                                           {
                                                                               StateID = s.StateID,
                                                                               Matrix = t.TairTypeOfStateMatrix
                                                                           }).ToList();
                                            StateMatrix = Convert.ToDouble(StateMatrixList.Sum(x => x.Matrix));
                                            TotalStates = StatesCount;
                                            StateList.AddRange(StateMatrixList);
                                        }

                                        if (advertisement.advertisementDistricts != null && advertisement.advertisementDistricts.Count() > 0)
                                        {
                                            DistrictsCount = advertisement.advertisementDistricts.Count();
                                            List<State> DistrictMatrixList = (from s in dbContext.tblDistricts.ToList()
                                                                              join t in dbContext.tblTairTypeOfDistricts on s.TairTypeOfDistrictID equals t.ID
                                                                              join adl in advertisement.advertisementDistricts on s.DistrictID equals adl.DistrictID
                                                                              select new State
                                                                              {
                                                                                  StateID = s.StateID,
                                                                                  Matrix = t.TairTypeOfDistrictMatrix
                                                                              }).ToList();
                                            DistrictMatrix = Convert.ToDouble(DistrictMatrixList.Sum(x => x.Matrix));
                                            TotalDistricts = DistrictsCount;
                                            StateList.AddRange(DistrictMatrixList);
                                        }

                                        if (advertisement.advertisementCities != null && advertisement.advertisementCities.Count() > 0)
                                        {
                                            CitiesCount = advertisement.advertisementCities.Count();
                                            List<State> CityMatrixList = (from s in dbContext.tblStateWithCities.ToList()
                                                                          join t in dbContext.tblTairTypeOfCities on s.TairTypeOfCityID equals t.ID
                                                                          join acl in advertisement.advertisementCities on s.StatewithCityID equals acl.StateWithCityID
                                                                          select new State
                                                                          {
                                                                              StateID = s.StateID,
                                                                              Matrix = t.TairTypeOfCityMatrix
                                                                          }).ToList();
                                            CityMatrix = Convert.ToDouble(CityMatrixList.Sum(x => x.Matrix));
                                            TotalCities = CitiesCount;
                                            StateList.AddRange(CityMatrixList);
                                        }

                                        if (StateList != null && StateList.Count() > 0)
                                        {
                                            StateList = StateList.GroupBy(i => i.StateID).Select(i => i.FirstOrDefault()).ToList();
                                            var advertisementHolidays = (from a in advertisementHoliday
                                                                         join b in StateList on a.StateID equals b.StateID
                                                                         select new AdvertisementHoliday
                                                                         {
                                                                             DayName = a.DayName,
                                                                             HolidayDate = a.HolidayDate,
                                                                             HolidayDefinition = a.HolidayDefinition,
                                                                             HolidayMatrix = a.HolidayMatrix,
                                                                             HolidayName = a.HolidayName,
                                                                             HolidayType = a.HolidayType,
                                                                             StateID = a.StateID
                                                                         }).ToList();
                                            if (advertisementHolidays != null && advertisementHolidays.Count() > 0)
                                            {
                                                foreach (var adHolidayItem in advertisementHolidays)
                                                {
                                                    if (adHolidayItem.HolidayDefinition.ToLower() == "festival")
                                                    {
                                                        FestivalMatrix = adHolidayItem.HolidayMatrix.Value;
                                                    }
                                                    else if (adHolidayItem.HolidayDefinition.ToLower() == "public")
                                                    {
                                                        if (iDate.Date == adHolidayItem.HolidayDate.Value.Date)
                                                        {
                                                            NationalHolidayMatrix = adHolidayItem.HolidayMatrix.Value;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        //Weekend Matrix
                                        string DayofTheWeek = advertisement.FromDate.Value.DayOfWeek.ToString();
                                        if (DayofTheWeek.ToLower() == "saturday" || DayofTheWeek.ToLower() == "sunday")
                                            WeekendMatrix += tblAdminSetting.WeekendMatrix.Value;

                                        TotalPriceOfSlot += NationalHolidayMatrix * FestivalMatrix * WeekendMatrix * TotalDays * AreaMatrix * AdTypeMatrix * CategoryMatrix * CityMatrix * DistrictMatrix * StateMatrix * TimeSlotMatrix * CitiesCount * DistrictsCount * StatesCount * TotalIntervalsPerDay;

                                        Advertisement SingleItem = new Advertisement();
                                        SingleItem.AdTimeSlotID = item.ID;
                                        SingleItem.AdvertisementAreaID = advertisement.AdvertisementAreaID;
                                        SingleItem.advertisementCities = advertisement.advertisementCities;
                                        SingleItem.advertisementDistricts = advertisement.advertisementDistricts;
                                        SingleItem.AdvertisementName = advertisement.AdvertisementName;
                                        SingleItem.advertisementStates = advertisement.advertisementStates;
                                        SingleItem.BrandID = advertisement.BrandID;
                                        SingleItem.CreatedBy = advertisement.CreatedBy;
                                        SingleItem.CreatedDate = DateTimeNow;
                                        SingleItem.CustID = advertisement.CustID;
                                        SingleItem.DaysCount = TotalDays;
                                        SingleItem.FromDate = iDate;
                                        SingleItem.ToDate = iDate;
                                        SingleItem.IntervalsPerDay = Convert.ToInt32(TotalIntervalsPerDay);
                                        SingleItem.IntervalsPerHour = Convert.ToInt32(TotalIntervalsPerHour);
                                        SingleItem.ProductID = advertisement.ProductID;
                                        SingleItem.slots = advertisement.slots;
                                        SingleItem.TotalPrice = TotalPrice;
                                        SingleItem.FinalPrice = FinalPrice;
                                        SingleItem.TotalDiscountAmount = DiscountAmount;
                                        SingleItem.TypeOfAdvertisementID = advertisement.TypeOfAdvertisementID;
                                        SingleItem.IsActive = false;
                                        SingleItem.IsCancelled = false;
                                        SingleItem.IsCompleted = false;
                                        SingleItem.TemporaryBooked = true;
                                        advertisementLists.Add(SingleItem);
                                    }
                                }

                                TotalPrice += Math.Round(TotalPriceOfSlot, 2);
                                DiscountPer = Convert.ToDouble(dbContext.tblAdvertisementDiscounts.AsNoTracking().Where(d => TotalPrice >= d.FromAmount && TotalPrice <= d.ToAmount).FirstOrDefault().DiscountPer);
                                double DiscountAmountOfSlot = (TotalPrice * DiscountPer) / 100;
                                DiscountAmount += Math.Round(DiscountAmountOfSlot, 2);
                                FinalPrice = Math.Round(FinalPrice + TotalPriceOfSlot - DiscountAmountOfSlot, 2);

                                //Calculate the tax
                                TaxAmount = Math.Round((FinalPrice * TaxSlabValue) / 100, 2);
                                FinalPrice = Math.Round(FinalPrice + TaxAmount, 2);

                                main.AdvertisementType = AdvertisementType;
                                main.advertisementList = advertisementLists;
                                main.AdvertisementName = advertisement.AdvertisementName;
                                main.BrandID = advertisement.BrandID;
                                main.BrandName = advertisement.BrandName;
                                main.CustID = advertisement.CustID;
                                main.FromDate = advertisement.FromDate;
                                main.ToDate = advertisement.ToDate;
                                main.TypeOfAdvertisementID = advertisement.TypeOfAdvertisementID;
                                main.ProductID = advertisement.ProductID;
                                main.CreatedBy = advertisement.CreatedBy;
                                main.CreatedDate = DateTimeNow;
                                main.AdTotalPrice = TotalPrice;
                                main.FinalPrice = Math.Round(FinalPrice);
                                main.TotalDiscount = DiscountAmount;
                                main.IsActive = false;
                                main.IsCancelled = false;
                                main.IsCompleted = false;
                                main.TemporaryBooked = true;
                                main.states = advertisement.advertisementStates;
                                main.districts = advertisement.advertisementDistricts;
                                main.cities = advertisement.advertisementCities;
                                main.DiscountPer = DiscountPer;
                                main.TaxValue = TaxSlabValue;
                                main.TaxAmount = TaxAmount;
                                main.AdvertisementAreaID = advertisement.AdvertisementAreaID;
                                if (IsIntraState)
                                {
                                    main.SGSTPer = TaxSlabValue / 2;
                                    main.CGSTPer = TaxSlabValue / 2;
                                    main.SGSTAmount = TaxAmount / 2;
                                    main.CGSTAmount = TaxAmount / 2;
                                    main.IGSTPer = 0;
                                    main.IGSTAmount = 0;
                                }
                                else
                                {
                                    main.SGSTPer = 0;
                                    main.CGSTPer = 0;
                                    main.SGSTAmount = 0;
                                    main.CGSTAmount = 0;
                                    main.IGSTPer = TaxSlabValue;
                                    main.IGSTAmount = TaxAmount;
                                }
                                Result = SaveAdvertisement(main);
                            }

                            Result.DispayMessage += " ";
                            Result.AdvertisementArea = ResultItems.AdvertisementArea;
                            Result.BrandName = advertisement.BrandName;
                            Result.StatusCode = HttpStatusCode.OK;
                            Result.TotalDiscountAmount = DiscountAmount;
                            Result.TotalPrice = TotalPrice;
                            Result.FinalPrice = Math.Round(FinalPrice);
                            Result.TaxAmount = TaxAmount;
                            Result.AdvertisementType = AdvertisementType;
                            Result.TotalDays = TotalDays;
                            Result.TotalStates = Convert.ToInt32(TotalStates);
                            Result.TotalDistricts = Convert.ToInt32(TotalDistricts);
                            Result.TotalCities = Convert.ToInt32(TotalCities);
                            Result.ProductName = ResultItems.ProductName;

                            return Result;
                        }
                        else
                        {
                            return Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.DispayMessage = ex.Message;
                Result.StatusCode = HttpStatusCode.InternalServerError;
                return Result;
            }
        }
        public AdvertisementMain SaveAdvertisement(AdvertisementMain main)
        {
            AdvertisementMain Result = new AdvertisementMain();
            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        //Get saved advertisements
                        var advertisements = dbContext.tblAdvertisements;
                        var customerDetails = dbContext.tblCustomerDetails.Where(c => c.ID == main.CustID).FirstOrDefault();

                        int ProformaInvoiceNumber = dbContext.tblAdvertisementMains.Count();
                        DateTime FromDate = Convert.ToDateTime(main.FromDate);
                        DateTime ToDate = Convert.ToDateTime(main.ToDate);

                        //Calculate expiry Date of booking
                        int HoursOfExpiry = dbContext.tblAdminSettings.FirstOrDefault().HoursOfExpiry.Value;
                        main.BookingExpiryDate = main.CreatedDate.Value.AddHours(HoursOfExpiry);
                        main.ProformaInvoiceNumber = (ProformaInvoiceNumber + 1).ToString();
                        string PadNumber = pad_an_int(Convert.ToInt32(main.ProformaInvoiceNumber), Convert.ToInt32(4));
                        string CustShortName = customerDetails.FirmName.Substring(0, 3).ToUpper();
                        main.AdvertisementName = "AD" + CustShortName + PadNumber;

                        //Insert into Advertisement Main Table
                        tblAdvertisementMain tblAdvertisementMain = new tblAdvertisementMain();
                        tblAdvertisementMain.AdvertisementName = main.AdvertisementName;
                        tblAdvertisementMain.BrandID = main.BrandID;
                        tblAdvertisementMain.BrandName = main.BrandName;
                        tblAdvertisementMain.CreatedBy = main.CreatedBy.Value;
                        tblAdvertisementMain.CreatedDate = DateTimeNow;
                        tblAdvertisementMain.CustID = main.CustID;
                        tblAdvertisementMain.FromDate = FromDate;
                        tblAdvertisementMain.ToDate = ToDate;
                        tblAdvertisementMain.ProductID = main.ProductID;
                        tblAdvertisementMain.TypeOfAdvertisementID = main.TypeOfAdvertisementID;
                        tblAdvertisementMain.AdTotalPrice = main.AdTotalPrice;
                        tblAdvertisementMain.TotalDiscount = main.TotalDiscount;
                        tblAdvertisementMain.FinalPrice = Math.Round(main.FinalPrice.Value);
                        tblAdvertisementMain.BookingExpiryDate = main.BookingExpiryDate.Value;
                        tblAdvertisementMain.ProformaInvoiceNumber = Convert.ToInt32(main.ProformaInvoiceNumber);
                        tblAdvertisementMain.IsActive = false;
                        tblAdvertisementMain.IsCancelled = false;
                        tblAdvertisementMain.IsCompleted = false;
                        tblAdvertisementMain.TemporaryBooked = true;
                        tblAdvertisementMain.DiscountPer = main.DiscountPer;
                        tblAdvertisementMain.TaxAmount = main.TaxAmount;
                        tblAdvertisementMain.TaxValue = main.TaxValue;
                        tblAdvertisementMain.IsApproved = false;
                        tblAdvertisementMain.PaymentStatus = false;
                        tblAdvertisementMain.SGSTAmount = main.SGSTAmount;
                        tblAdvertisementMain.CGSTAmount = main.CGSTAmount;
                        tblAdvertisementMain.IGSTAmount = main.IGSTAmount;
                        tblAdvertisementMain.SGSTPer = main.SGSTPer;
                        tblAdvertisementMain.CGSTPer = main.CGSTPer;
                        tblAdvertisementMain.IGSTPer = main.IGSTPer;
                        tblAdvertisementMain.AdvertisementAreaID = main.AdvertisementAreaID;
                        tblAdvertisementMain.IsRejected = false;
                        dbContext.tblAdvertisementMains.Add(tblAdvertisementMain);
                        dbContext.SaveChanges();

                        int AdvertisementMainID = tblAdvertisementMain.ID;
                        //Insert into Advertisement Table
                        if (main.advertisement != null)
                        {
                            tblAdvertisement tblAdvertisement = new tblAdvertisement();
                            tblAdvertisement.AdvertisementMainID = AdvertisementMainID;
                            tblAdvertisement.AdvertisementName = "AD" + CustShortName + PadNumber;
                            tblAdvertisement.AdTimeSlotID = main.advertisement.AdTimeSlotID;
                            tblAdvertisement.AdvertisementAreaID = main.advertisement.AdvertisementAreaID;
                            tblAdvertisement.AdvertisementName = main.advertisement.AdvertisementName;
                            tblAdvertisement.BrandID = main.advertisement.BrandID;
                            tblAdvertisement.CreatedBy = main.advertisement.CreatedBy.Value;
                            tblAdvertisement.CreatedDate = main.advertisement.CreatedDate.Value;
                            tblAdvertisement.CustID = main.advertisement.CustID;
                            tblAdvertisement.FromDate = main.advertisement.FromDate;
                            tblAdvertisement.ToDate = main.advertisement.ToDate;
                            tblAdvertisement.IntervalsPerDay = main.advertisement.IntervalsPerDay;
                            tblAdvertisement.IntervalsPerHour = main.advertisement.IntervalsPerHour;
                            tblAdvertisement.IsActive = main.advertisement.IsActive.Value;
                            tblAdvertisement.IsCancelled = main.advertisement.IsCancelled.Value;
                            tblAdvertisement.IsCompleted = main.advertisement.IsCompleted.Value;
                            tblAdvertisement.ProductID = main.advertisement.ProductID;
                            tblAdvertisement.TemporaryBooked = main.advertisement.TemporaryBooked.Value;
                            tblAdvertisement.TypeOfAdvertisementID = main.advertisement.TypeOfAdvertisementID;
                            dbContext.tblAdvertisements.Add(tblAdvertisement);
                        }
                        else if (main.advertisementList != null && main.advertisementList.Count() > 0)
                        {
                            if (main.AdvertisementType.ToLower() == "fullpagead")
                            {
                                foreach (var item in main.advertisementList)
                                {
                                    tblAdvertisement tblAdvertisement = new tblAdvertisement();
                                    tblAdvertisement.AdvertisementMainID = AdvertisementMainID;
                                    tblAdvertisement.AdvertisementName = "AD" + CustShortName + PadNumber;
                                    tblAdvertisement.AdTimeSlotID = item.AdTimeSlotID;
                                    tblAdvertisement.AdvertisementAreaID = item.AdvertisementAreaID;
                                    tblAdvertisement.AdvertisementName = item.AdvertisementName;
                                    tblAdvertisement.BrandID = item.BrandID;
                                    tblAdvertisement.CreatedBy = item.CreatedBy.Value;
                                    tblAdvertisement.CreatedDate = item.CreatedDate.Value;
                                    tblAdvertisement.CustID = item.CustID;
                                    tblAdvertisement.FromDate = item.FromDate;
                                    tblAdvertisement.ToDate = item.ToDate;
                                    tblAdvertisement.IntervalsPerDay = item.IntervalsPerDay;
                                    tblAdvertisement.IntervalsPerHour = item.IntervalsPerHour;
                                    tblAdvertisement.IsActive = item.IsActive.Value;
                                    tblAdvertisement.IsCancelled = item.IsCancelled.Value;
                                    tblAdvertisement.IsCompleted = item.IsCompleted.Value;
                                    tblAdvertisement.ProductID = item.ProductID;
                                    tblAdvertisement.TemporaryBooked = item.TemporaryBooked.Value;
                                    tblAdvertisement.TypeOfAdvertisementID = item.TypeOfAdvertisementID;
                                    tblAdvertisement.AdStartTime = new TimeSpan();
                                    tblAdvertisement.CurrentAdTime = new TimeSpan();
                                    tblAdvertisement.NextAdTime = new TimeSpan();
                                    dbContext.tblAdvertisements.Add(tblAdvertisement);
                                }
                            }
                            else
                            {
                                foreach (var item in main.advertisementList)
                                {
                                    //get the first start time of the time slot
                                    TimeSpan StartTime = new TimeSpan();

                                    TimeSpan TimeSlotFromTime = dbContext.tblAdvertisementTimeSlots.Where(t => t.ID == item.AdTimeSlotID).FirstOrDefault().FromTime.Value;
                                    TimeSpan TimeSlotToTime = dbContext.tblAdvertisementTimeSlots.Where(t => t.ID == item.AdTimeSlotID).FirstOrDefault().ToTime.Value;

                                    TimeSpan TimeSlotMinutes = (TimeSlotToTime - TimeSlotFromTime);
                                    int TotalHoursPerSlot = Convert.ToInt32(TimeSlotMinutes.Hours);

                                    //get ad duration in seconds and max durations allowed from admin settings
                                    int AdDurationInSeconds = dbContext.tblAdminSettings.FirstOrDefault().AdDurationInSeconds.Value;
                                    int MaxDurationsAllowedPerHr = dbContext.tblAdminSettings.FirstOrDefault().MaxDurationsAllowedPerHr.Value;

                                    if (advertisements != null && advertisements.Count() > 0)
                                    {
                                        var advertisement = (from a in advertisements.ToList()
                                                             where a.FromDate.Value.Date == item.FromDate.Value.Date && a.ToDate.Value.Date == item.ToDate.Value.Date
                                                             && a.AdTimeSlotID == item.AdTimeSlotID
                                                             select a).OrderByDescending(x => x.AdStartTime).FirstOrDefault();
                                        if (advertisement != null)
                                        {
                                            StartTime = (from a in advertisements.ToList()
                                                         where a.FromDate.Value.Date == item.FromDate.Value.Date && a.ToDate.Value.Date == item.ToDate.Value.Date
                                                         && a.AdTimeSlotID == item.AdTimeSlotID
                                                         select a).OrderByDescending(x => x.AdStartTime).FirstOrDefault().AdStartTime.Value;
                                            StartTime = StartTime.Add(TimeSpan.FromSeconds(AdDurationInSeconds));
                                        }
                                        else
                                        {
                                            StartTime = TimeSlotFromTime;
                                        }
                                    }
                                    if (StartTime == new TimeSpan())
                                        StartTime = TimeSlotFromTime;

                                    //add the next time (1 hour)
                                    TimeSpan CurrentStartTime = StartTime;
                                    int DiffSeconds = Convert.ToInt32(TimeSlotMinutes.TotalSeconds) / MaxDurationsAllowedPerHr;
                                    TimeSpan NextStartTime = StartTime + TimeSpan.FromSeconds(DiffSeconds);

                                    tblAdvertisement tblAdvertisement = new tblAdvertisement();
                                    tblAdvertisement.AdvertisementMainID = AdvertisementMainID;
                                    tblAdvertisement.AdvertisementName = "AD" + CustShortName + PadNumber;
                                    tblAdvertisement.AdTimeSlotID = item.AdTimeSlotID;
                                    tblAdvertisement.AdvertisementAreaID = item.AdvertisementAreaID;
                                    tblAdvertisement.AdvertisementName = item.AdvertisementName;
                                    tblAdvertisement.BrandID = item.BrandID;
                                    tblAdvertisement.CreatedBy = item.CreatedBy.Value;
                                    tblAdvertisement.CreatedDate = item.CreatedDate.Value;
                                    tblAdvertisement.CustID = item.CustID;
                                    tblAdvertisement.FromDate = item.FromDate;
                                    tblAdvertisement.ToDate = item.ToDate;
                                    tblAdvertisement.IntervalsPerDay = item.IntervalsPerDay;
                                    tblAdvertisement.IntervalsPerHour = item.IntervalsPerHour;
                                    tblAdvertisement.IsActive = item.IsActive.Value;
                                    tblAdvertisement.IsCancelled = item.IsCancelled.Value;
                                    tblAdvertisement.IsCompleted = item.IsCompleted.Value;
                                    tblAdvertisement.ProductID = item.ProductID;
                                    tblAdvertisement.TemporaryBooked = item.TemporaryBooked.Value;
                                    tblAdvertisement.TypeOfAdvertisementID = item.TypeOfAdvertisementID;
                                    tblAdvertisement.AdStartTime = StartTime;
                                    tblAdvertisement.CurrentAdTime = CurrentStartTime;
                                    tblAdvertisement.NextAdTime = NextStartTime;
                                    dbContext.tblAdvertisements.Add(tblAdvertisement);
                                }
                            }
                        }

                        if (main.states != null && main.states.Count() > 0)
                        {
                            foreach (var Sitem in main.states)
                            {
                                tblAdvertisementState state = new tblAdvertisementState();
                                state.AdvertisementMainID = AdvertisementMainID;
                                state.CreatedDate = DateTimeNow;
                                state.StateID = Sitem.StateID;
                                state.StateName = Sitem.StateName;
                                state.TairTypeOfState = Sitem.TairTypeOfState;
                                state.TairTypeOfStateID = Sitem.TairTypeOfStateID;
                                dbContext.tblAdvertisementStates.Add(state);
                            }
                        }
                        if (main.cities != null && main.cities.Count() > 0)
                        {
                            foreach (var Citem in main.cities)
                            {
                                tblAdvertisementCity city = new tblAdvertisementCity();
                                city.AdvertisementMainID = AdvertisementMainID;
                                city.CreatedDate = DateTimeNow;
                                city.StateWithCityID = Citem.StateWithCityID;
                                city.VillageLocalityName = Citem.VillageLocalityName;
                                city.TairTypeOfCity = Citem.TairTypeOfCity;
                                city.TairTypeOfCityID = Citem.TairTypeOfCityID;
                                dbContext.tblAdvertisementCities.Add(city);
                            }
                        }
                        if (main.districts != null && main.districts.Count() > 0)
                        {
                            foreach (var Ditem in main.districts)
                            {
                                tblAdvertisementDistrict district = new tblAdvertisementDistrict();
                                district.AdvertisementMainID = AdvertisementMainID;
                                district.CreatedDate = DateTimeNow;
                                district.DistrictID = Ditem.DistrictID;
                                district.DistrictName = Ditem.DistrictName;
                                district.TairTypeOfDistrict = Ditem.TairTypeOfDistrict;
                                district.TairTypeOfDistrictID = Ditem.TairTypeOfDistrictID;
                                dbContext.tblAdvertisementDistricts.Add(district);
                            }
                        }

                        dbContext.SaveChanges();
                        dbcxtransaction.Commit();
                        Result.AdvertisementMainID = AdvertisementMainID;
                        Result.AdvertisementName = main.AdvertisementName;
                        Result.CreatedDateStr = tblAdvertisementMain.CreatedDate.ToString("d/MM/yyyy hh:mm:ss");
                        Result.BookingExpiryDateStr = main.BookingExpiryDate.Value.ToString("d/MM/yyyy hh:mm:ss");
                        Result.CGSTAmount = tblAdvertisementMain.CGSTAmount;
                        Result.SGSTAmount = tblAdvertisementMain.SGSTAmount;
                        Result.IGSTAmount = tblAdvertisementMain.IGSTAmount;
                        Result.CGSTPer = tblAdvertisementMain.CGSTPer;
                        Result.SGSTPer = tblAdvertisementMain.SGSTPer;
                        Result.IGSTPer = tblAdvertisementMain.IGSTPer;
                        Result.DispayMessage = "Success! Slots are available. Advertisement slot temporarily booked for you Successfully.";
                        //GenerateProformaInvoice(main);
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.DispayMessage = ex.Message;
                return Result;
            }
        }
        public List<Brands> GetBrands()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<Brands> brands = new List<Brands>();
                    brands = (from b in dbContext.tblBrands.ToList()
                              select new Brands
                              {
                                  BrandID = b.ID,
                                  BrandName = b.BrandName
                              }).ToList();
                    return brands;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public AdvertisementMain GetAdvertisementMain(AdvertisementMain main)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    AdvertisementMain advertisementMain = new AdvertisementMain();
                    advertisementMain = (from a in dbContext.tblAdvertisementMains
                                         where a.ID == main.AdvertisementMainID
                                         select new AdvertisementMain
                                         {
                                             AdvertisementMainID = a.ID,
                                             AdvertisementName = a.AdvertisementName,
                                         }).FirstOrDefault();
                    return advertisementMain;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public AdvertisementMain SaveAdvertisementImage(AdvertisementMain main)
        {
            AdvertisementMain Result = new AdvertisementMain();
            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        //Insert into Advertisement Main Table
                        tblAdvertisementMain tblAdvertisementMain = new tblAdvertisementMain();
                        tblAdvertisementMain.ID = main.AdvertisementMainID;
                        tblAdvertisementMain.AdImageURL = main.AdImageURL;
                        tblAdvertisementMain.IsRejected = false;
                        dbContext.tblAdvertisementMains.Attach(tblAdvertisementMain);
                        dbContext.Entry(tblAdvertisementMain).Property(C => C.AdImageURL).IsModified = true;
                        dbContext.Entry(tblAdvertisementMain).Property(C => C.IsRejected).IsModified = true;
                        dbContext.SaveChanges();
                        dbcxtransaction.Commit();
                        Result.StatusCode = HttpStatusCode.OK;
                        Result.AdvertisementMainID = main.AdvertisementMainID;
                        Result.AdImageURL = main.AdImageURL;
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.StatusCode = HttpStatusCode.InternalServerError;
                Result.DispayMessage = ex.Message;
                return Result;
            }
        }
        public ProformaInvoice GenerateProformaInvoice(Advertisement Result)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    ProformaInvoice proformaInvoice = new ProformaInvoice();
                    AdvertisementMain main = new AdvertisementMain();
                    tblCompany company = new tblCompany();
                    main = (from a in dbContext.tblAdvertisementMains
                            where a.ID == Result.AdvertisementMainID
                            select new AdvertisementMain
                            {
                                AdvertisementMainID = a.ID,
                                AdTotalPrice = a.AdTotalPrice,
                                AdvertisementName = a.AdvertisementName,
                                BookingExpiryDate = a.BookingExpiryDate,
                                CreatedDate = a.CreatedDate,
                                FromDate = a.FromDate,
                                ProformaInvoiceNumber = a.ProformaInvoiceNumber.ToString(),
                                TotalDiscount = a.TotalDiscount,
                                ToDate = a.ToDate,
                                TaxAmount = a.TaxAmount,
                                TaxValue = a.TaxValue,
                                FinalPrice = a.FinalPrice,
                                IGSTAmount = a.IGSTAmount,
                                IGSTPer = a.IGSTPer,
                                SGSTAmount = a.SGSTAmount,
                                SGSTPer = a.SGSTPer,
                                CGSTAmount = a.CGSTAmount,
                                CGSTPer = a.CGSTPer,
                                TotalIntervals = a.TotalRotations,
                            }).FirstOrDefault();

                    main.TotalDays = (Convert.ToDateTime(main.ToDate.Value).AddDays(1) - Convert.ToDateTime(main.FromDate.Value)).Days == 0 ? 1 : (Convert.ToDateTime(main.ToDate.Value).AddDays(1) - Convert.ToDateTime(main.FromDate.Value)).Days;

                    proformaInvoice = (from a in dbContext.tblAdvertisementMains
                                       join aarea in dbContext.tblAdvertisementAreas on a.AdvertisementAreaID equals aarea.ID
                                       join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                                       join c in dbContext.tblItemCategories on a.ProductID equals c.ID
                                       where a.ID == Result.AdvertisementMainID
                                       select new ProformaInvoice
                                       {
                                           AdvertisementAreaName = aarea.AdvertisementAreaName,
                                           AdAreaMatrix = aarea.AdAreaMatrix,
                                           AdTypeMatrix = at.AdTypeMatrix,
                                           AdvertisementType = at.Type,
                                           BrandName = dbContext.tblBrands.Where(b => b.ID == a.BrandID).FirstOrDefault().BrandName,
                                           ProductName = c.ItemName,
                                           CategoryMatrix = c.ItemMatrix,
                                           customer = (from c in dbContext.tblCustomerDetails
                                                       join s in dbContext.tblStateWithCities on c.City equals s.ID
                                                       join st in dbContext.tblStates on c.State equals st.ID
                                                       where c.ID == a.CustID
                                                       select new CustomerDetails
                                                       {
                                                           FirmName = c.FirmName,
                                                           BillingAddress = c.BillingAddress,
                                                           Pincode = c.Pincode,
                                                           MobileNumber = c.MobileNumber,
                                                           city = new City
                                                           {
                                                               VillageLocalityName = s.VillageLocalityName
                                                           },
                                                           state = new State
                                                           {
                                                               StateName = st.StateName
                                                           },
                                                       }).FirstOrDefault(),
                                           company = (from cm in dbContext.tblCompanies
                                                      select new Company
                                                      {
                                                          CompanyName = cm.CompanyName,
                                                          CompanyAddress = cm.CompanyAddress,
                                                          CompanyCity = cm.CompanyCity,
                                                          CompanyState = cm.CompanyState,
                                                          PinCode = cm.PinCode
                                                      }).FirstOrDefault(),
                                           advertisementStates = (from s in dbContext.tblAdvertisementStates
                                                                  join st in dbContext.tblStates on s.StateID equals st.ID
                                                                  join tts in dbContext.tblTairTypeOfStates on st.TairTypeID equals tts.ID
                                                                  where s.AdvertisementMainID.Value == a.ID
                                                                  select new AdvertisementStates
                                                                  {
                                                                      StateID = s.StateID,
                                                                      StateName = s.StateName,
                                                                      TairTypeOfStateMatrix = tts.TairTypeOfStateMatrix
                                                                  }).ToList(),
                                           advertisementDistricts = (from d in dbContext.tblAdvertisementDistricts
                                                                     join dd in dbContext.tblDistricts on d.DistrictID equals dd.ID
                                                                     join ttd in dbContext.tblTairTypeOfDistricts on dd.TairTypeOfDistrictID equals ttd.ID
                                                                     where d.AdvertisementMainID.Value == a.ID
                                                                     select new AdvertisementDistricts
                                                                     {
                                                                         DistrictID = d.DistrictID,
                                                                         DistrictName = d.DistrictName,
                                                                         TairTypeOfDistrictMatrix = ttd.TairTypeOfDistrictMatrix
                                                                     }).ToList(),
                                           advertisementCities = (from c in dbContext.tblAdvertisementCities
                                                                  join ct in dbContext.tblStateWithCities on c.StateWithCityID equals ct.ID
                                                                  join ttc in dbContext.tblTairTypeOfCities on ct.TairTypeOfCityID equals ttc.ID
                                                                  where c.AdvertisementMainID.Value == a.ID
                                                                  select new AdvertisementCities
                                                                  {
                                                                      StateWithCityID = c.StateWithCityID,
                                                                      VillageLocalityName = c.VillageLocalityName,
                                                                      TairTypeOfCityMatrix = ttc.TairTypeOfCityMatrix
                                                                  }).ToList(),
                                           TotalCities = a.TotalCities,
                                           TotalDistricts = a.TotalDistricts,
                                           TotalStates = a.TotalStates,
                                       }).FirstOrDefault();

                    List<AdvertisementTimeSlot> TimeSlots = (from at in dbContext.tblAdTimeSlots
                                                             join admain in dbContext.tblAdvertisementMains on at.AdvertisementID equals admain.ID
                                                             join ats in dbContext.tblAdvertisementTimeSlots on at.TimeSlotID equals ats.ID
                                                             where admain.ID == Result.AdvertisementMainID
                                                             select new AdvertisementTimeSlot
                                                             {
                                                                 ID = at.TimeSlotID,
                                                                 TimeSlotName = at.TimeSlotName,
                                                                 TimeSlotMatrix = ats.TimeSlotMatrix,
                                                             }).Distinct().ToList();
                    proformaInvoice.TimeSlots = TimeSlots;

                    //if (proformaInvoice.advertisementStates != null)
                    //    proformaInvoice.TotalStates = proformaInvoice.advertisementStates.Count();
                    //if (proformaInvoice.advertisementDistricts != null)
                    //    proformaInvoice.TotalDistricts = proformaInvoice.advertisementDistricts.Count();
                    //if (proformaInvoice.advertisementCities != null)
                    //    proformaInvoice.TotalCities = proformaInvoice.advertisementCities.Count();

                    if (proformaInvoice.advertisementStates != null && proformaInvoice.advertisementStates.Count() > 0)
                        proformaInvoice.StateMatrix = proformaInvoice.advertisementStates.Select(s => s.TairTypeOfStateMatrix).Sum();

                    if (proformaInvoice.advertisementDistricts != null && proformaInvoice.advertisementDistricts.Count() > 0)
                        proformaInvoice.DistrictMatrix = proformaInvoice.advertisementDistricts.Select(s => s.TairTypeOfDistrictMatrix).Sum();

                    if (proformaInvoice.advertisementCities != null && proformaInvoice.advertisementCities.Count() > 0)
                        proformaInvoice.CityMatrix = proformaInvoice.advertisementCities.Select(s => s.TairTypeOfCityMatrix).Sum();

                    proformaInvoice.main = main;
                    return proformaInvoice;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<AdvertisementMain_S> GetAdvertisementsOfAnUser(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<AdvertisementMain_S> mains = new List<AdvertisementMain_S>();
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    int SetPaymentDueinHrs = dbContext.tblAdminSettings.FirstOrDefault().SetPaymentDueinHrs.Value;
                    mains = (from a in dbContext.tblAdvertisementMains
                             join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                             join c in dbContext.tblItemCategories on a.ProductID equals c.ChildCategoryID
                             join cu in dbContext.tblCustomerDetails on a.CustID equals cu.ID
                             where a.CustID == CustID && a.IsCancelled == false
                             select new AdvertisementMain_S
                             {
                                 AdvertisementMainID = a.ID,
                                 AdvertisementName = a.AdvertisementName,
                                 CreatedDate = a.CreatedDate.ToString(),
                                 AdvertisementType = at.Type,
                                 ApprovalStatus = a.IsApproved == true ? "Approved" : "Pending",
                                 PaymentStatus = a.PaymentStatus == true ? "Approved" : "Pending",
                                 AdImageURL = a.AdImageURL,
                                 IsApproved = a.IsApproved,
                                 BookingExpiryDate = a.BookingExpiryDate,
                                 AdText = a.AdText,
                                 ProductName = c.ItemName,
                                 IsRejected = a.IsRejected,
                                 Remarks = a.Remarks,
                                 CustomerInfo = new CustomerInfo
                                 {
                                     CustID = cu.ID,
                                     FirmName = cu.FirmName,
                                     MobileNumber = cu.MobileNumber,
                                     EmailID = cu.EmailID,
                                 },
                                 CGSTAmount = a.CGSTAmount,
                                 CGSTPer = a.CGSTPer,
                                 SGSTAmount = a.SGSTAmount,
                                 SGSTPer = a.SGSTPer,
                                 TotalDiscount = a.TotalDiscount,
                                 AdTotalPrice = a.AdTotalPrice,
                                 FinalPrice = a.FinalPrice,
                                 IGSTAmount = a.IGSTAmount,
                                 IGSTPer = a.IGSTPer,
                                 TaxAmount = a.TaxAmount,
                                 TaxValue = a.TaxValue,
                                 ContentApprovedDate = a.ContentApprovedDate,
                             }).ToList();
                    if (mains != null)
                    {
                        mains.ForEach(m => m.CreatedDate = Convert.ToDateTime(m.CreatedDate).ToString("dd/MM/yyyy"));
                        foreach (var item in mains)
                        {
                            if (item.IsApproved == true)
                            {
                                item.IsExpired = false;
                            }
                            else if (item.IsApproved.Value == false && item.PaymentStatus.ToLower() == "pending" && item.BookingExpiryDate.Value < DateTimeNow)
                            {
                                item.IsExpired = true;
                                item.Remarks = "Booking Expired";
                            }
                            else
                                item.IsExpired = false;
                            if (item.IsRejected == true)
                                item.ApprovalStatus = "Rejected";

                            //Payment option
                            if (item.ContentApprovedDate != null)
                            {
                                item.PaymentDueDate = item.ContentApprovedDate.Value.AddHours(SetPaymentDueinHrs).ToString("dd/MM/yyyy hh:mm:ss");
                                int res = DateTime.Compare(item.ContentApprovedDate.Value.AddHours(SetPaymentDueinHrs), DateTimeNow);
                                if (res <= 0)
                                {
                                    item.IsExpired = true;
                                    item.IsMakePaymentAllowed = false;
                                    item.Remarks = "Booking Expired";
                                }
                                else
                                {
                                    item.IsMakePaymentAllowed = true;
                                }
                            }
                            else
                            {
                                item.IsMakePaymentAllowed = true;
                            }
                        }
                        mains = mains.Where(m => m.IsExpired == false).ToList();
                    }
                    return mains.OrderByDescending(a => a.IsMakePaymentAllowed).ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<AdvertisementMain_S> GetSentAdvertisementsOfAnUser(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    List<AdvertisementMain_S> mains = new List<AdvertisementMain_S>();
                    mains = (from a in dbContext.tblAdvertisementMains
                             join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                             join c in dbContext.tblItemCategories on a.ProductID equals c.ChildCategoryID
                             join cu in dbContext.tblCustomerDetails on a.CustID equals cu.ID
                             where a.CustID == CustID && a.IsApproved == true && a.PaymentStatus == true
                             select new AdvertisementMain_S
                             {
                                 AdvertisementMainID = a.ID,
                                 AdvertisementName = a.AdvertisementName,
                                 CreatedDate = a.CreatedDate.ToString(),
                                 AdvertisementType = at.Type,
                                 ApprovalStatus = a.IsApproved == true ? "Approved" : "Pending",
                                 PaymentStatus = a.PaymentStatus == true ? "Approved" : "Pending",
                                 AdImageURL = a.AdImageURL,
                                 IsApproved = a.IsApproved,
                                 BookingExpiryDate = a.BookingExpiryDate,
                                 AdText = a.AdText,
                                 ProductName = c.ItemName,
                                 FromDate = a.FromDate.ToString(),
                                 ToDate = a.ToDate.ToString(),
                                 CustomerInfo = new CustomerInfo
                                 {
                                     CustID = cu.ID,
                                     FirmName = cu.FirmName,
                                     MobileNumber = cu.MobileNumber,
                                     EmailID = cu.EmailID,
                                 },
                                 CGSTAmount = a.CGSTAmount,
                                 CGSTPer = a.CGSTPer,
                                 SGSTAmount = a.SGSTAmount,
                                 SGSTPer = a.SGSTPer,
                                 TotalDiscount = a.TotalDiscount,
                                 AdTotalPrice = a.AdTotalPrice,
                                 FinalPrice = a.FinalPrice,
                                 IGSTAmount = a.IGSTAmount,
                                 IGSTPer = a.IGSTPer,
                                 TaxAmount = a.TaxAmount,
                                 TaxValue = a.TaxValue,
                             }).ToList();
                    if (mains != null)
                    {
                        mains.ForEach(m => m.CreatedDate = Convert.ToDateTime(m.CreatedDate).ToString("dd/MM/yyyy"));
                        mains.ForEach(m => m.FromDateStr = Convert.ToDateTime(m.FromDate).ToString("dd/MM/yyyy"));
                        mains.ForEach(m => m.ToDateStr = Convert.ToDateTime(m.ToDate).ToString("dd/MM/yyyy"));
                        foreach (var item in mains)
                        {
                            if (item.IsApproved == true)
                            {
                                item.IsExpired = false;
                            }
                            else if (item.IsApproved.Value == false && item.PaymentStatus.ToLower() == "pending" && item.BookingExpiryDate.Value < DateTime.Now)
                                item.IsExpired = true;
                            else
                                item.IsExpired = false;
                        }

                    }
                    return mains;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public AdvertisementMain GetAdvertisementDetails(int AdvertisementMainID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    AdvertisementMain advertisementMain = new AdvertisementMain();
                    int SetPaymentDueinHrs = dbContext.tblAdminSettings.FirstOrDefault().SetPaymentDueinHrs.Value;
                    advertisementMain = (from a in dbContext.tblAdvertisementMains
                                         join ad in dbContext.tblAdvertisements on a.ID equals ad.AdvertisementMainID
                                         join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                                         join ar in dbContext.tblAdvertisementAreas on ad.AdvertisementAreaID equals ar.ID
                                         join cu in dbContext.tblCustomerDetails on a.CustID equals cu.ID
                                         where a.ID == AdvertisementMainID
                                         select new AdvertisementMain
                                         {
                                             AdvertisementMainID = a.ID,
                                             AdvertisementName = a.AdvertisementName,
                                             CreatedDate = a.CreatedDate,
                                             BookingExpiryDate = a.BookingExpiryDate,
                                             CreatedDateStr = a.CreatedDate.ToString(),
                                             BookingExpiryDateStr = a.BookingExpiryDate.ToString(),
                                             AdvertisementArea = ar.AdvertisementAreaName,
                                             AdvertisementType = at.Type,
                                             TotalPrice = a.AdTotalPrice.Value,
                                             TotalDiscount = a.TotalDiscount,
                                             FinalPrice = a.FinalPrice,
                                             IGSTAmount = a.IGSTAmount,
                                             SGSTAmount = a.SGSTAmount,
                                             CGSTAmount = a.CGSTAmount,
                                             AdImageURL = a.AdImageURL,
                                             IGSTPer = a.IGSTPer,
                                             SGSTPer = a.SGSTPer,
                                             CGSTPer = a.CGSTPer,
                                             TaxAmount = a.TaxAmount,
                                             TaxValue = a.TaxValue,
                                             TotalDiscountAmount = a.TotalDiscount == null ? 0 : a.TotalDiscount.Value,
                                             AdTotalPrice = a.AdTotalPrice,
                                             FromDate = a.FromDate,
                                             ToDate = a.ToDate,
                                             ApprovalStatus = a.IsApproved == true ? "Approved" : "Pending",
                                             PaymentStatus = a.PaymentStatus == true ? "Approved" : "Pending",
                                             IsApproved = a.IsApproved,
                                             IsCancelled = a.IsCancelled,
                                             IsCompleted = a.IsCompleted,
                                             InvoiceNumber = a.InvoiceNumber,
                                             ProformaInvoiceNumber = a.ProformaInvoiceNumber.ToString(),
                                             cities = dbContext.tblAdvertisementCities.Where(ac => ac.AdvertisementMainID == a.ID).ToList(),
                                             districts = dbContext.tblAdvertisementDistricts.Where(ac => ac.AdvertisementMainID == a.ID).ToList(),
                                             states = dbContext.tblAdvertisementStates.Where(ac => ac.AdvertisementMainID == a.ID).ToList(),
                                             AdText = a.AdText,
                                             TimeSlots = dbContext.tblAdvertisementTimeSlots.Where(t => t.ID == ad.AdTimeSlotID).ToList(),
                                             ProductName = dbContext.tblItemCategories.Where(c => c.ChildCategoryID == a.ProductID).FirstOrDefault().ItemName,
                                             IsRejected = a.IsRejected == null ? false : a.IsRejected,
                                             customerInfo = new CustomerInfo
                                             {
                                                 CustID = cu.ID,
                                                 FirmName = cu.FirmName,
                                                 MobileNumber = cu.MobileNumber,
                                                 EmailID = cu.EmailID,
                                             },
                                             ContentApprovedDate = a.ContentApprovedDate,
                                             paymentDetails = (from ap in dbContext.tblPayments
                                                               where ap.AdvertisementMainID == a.ID
                                                               select new PaymentDetails
                                                               {
                                                                   AdvertisementMainID = ap.AdvertisementMainID,
                                                                   OrderID = ap.OrderID,
                                                                   txnPaymentMode = ap.txnPaymentMode,
                                                                   txnReferenceID = ap.txnReferenceID,
                                                                   txnSignature = ap.txnSignature,
                                                                   TxnStatus = ap.TxnStatus,
                                                                   TxnStatusMessage = ap.TxnStatusMessage
                                                               }).ToList(),
                                         }).FirstOrDefault();

                    if (advertisementMain != null)
                    {
                        advertisementMain.CreatedDateStr = advertisementMain.CreatedDate.Value.ToString("dd/MM/yyyy hh:mm:ss");
                        advertisementMain.FromDateStr = advertisementMain.FromDate.Value.ToString("dd/MM/yyyy");
                        advertisementMain.ToDateStr = advertisementMain.ToDate.Value.ToString("dd/MM/yyyy");
                        advertisementMain.BookingExpiryDateStr = advertisementMain.CreatedDate.Value.ToString("dd/MM/yyyy hh:mm:ss");

                        if (advertisementMain.IsApproved == true)
                        {
                            advertisementMain.IsExpired = false;
                        }
                        else if (advertisementMain.IsApproved.Value == false && advertisementMain.PaymentStatus.ToLower() == "pending" && advertisementMain.BookingExpiryDate.Value < DateTime.Now)
                            advertisementMain.IsExpired = true;
                        else
                            advertisementMain.IsExpired = false;
                        if (advertisementMain.IsRejected == true)
                            advertisementMain.ApprovalStatus = "Rejected";
                        if (advertisementMain.ContentApprovedDate != null)
                        {
                            advertisementMain.PaymentDueDate = advertisementMain.ContentApprovedDate.Value.AddHours(SetPaymentDueinHrs).ToString("dd/MM/yyyy hh:mm:ss");
                            int res = DateTime.Compare(advertisementMain.ContentApprovedDate.Value.AddHours(SetPaymentDueinHrs), DateTimeNow);
                            if (res <= 0)
                            {
                                advertisementMain.IsMakePaymentAllowed = false;
                            }
                            else
                            {
                                advertisementMain.IsMakePaymentAllowed = true;
                            }
                        }
                        else
                        {
                            advertisementMain.IsMakePaymentAllowed = true;
                        }
                    }

                    return advertisementMain;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public CustDetails GetFullScreenAdURL(int CustID)
        {
            string FullScreenURL = string.Empty;
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    int days = Convert.ToInt32(ConfigurationManager.AppSettings["Days"].ToString());
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    DateTimeNow = DateTimeNow.AddDays(days);
                    //int TimeSlotID = dbContext.tblAdvertisementTimeSlots.Where(a => dateNow.time >= d.FromAmount && TotalPrice <= d.ToAmount).FirstOrDefault().DiscountPer;
                    var TimeSlots = dbContext.tblAdvertisementTimeSlots.ToList();
                    int TimeSlotID = TimeSlots.Where(t => DateTimeNow.TimeOfDay >= t.FromTime && DateTimeNow.TimeOfDay <= t.ToTime).FirstOrDefault().ID;
                    List<ItemCategory> itemCategories = new List<ItemCategory>();
                    itemCategories = (from s in dbContext.tblSubCategoryProductWithCusts
                                      join c in dbContext.tblItemCategories on s.SubCategoryId equals c.ChildCategoryID
                                      where s.CustID == CustID
                                      select new ItemCategory
                                      {
                                          ChildCategoryId = c.ChildCategoryID.Value,
                                          ItemName = c.ItemName,
                                      }).ToList();
                    var advertisement = dbContext.tblAdvertisements;
                    var adsDateFiltered = advertisement.ToList().Where(a => a.FromDate.Value.Date == DateTimeNow.Date
                                                                        && a.AdTimeSlotID == TimeSlotID
                                                                        && itemCategories.Any(i => i.ChildCategoryId == a.ProductID)
                                                                        && a.IsActive == true).FirstOrDefault();
                    //var main = dbContext.tblAdvertisementMains.Where(a => a.AdvertisementMainID == adsDateFiltered.AdvertisementMainID && a.IsApproved.Value == true).FirstOrDefault();

                    if (adsDateFiltered != null)
                    {
                        var main = (from a in dbContext.tblAdvertisementMains
                                    join c in dbContext.tblCustomerDetails on a.CustID equals c.ID
                                    join s in dbContext.tblStates on c.State equals s.StateID
                                    join sc in dbContext.tblStateWithCities on c.City equals sc.StatewithCityID
                                    where a.ID == adsDateFiltered.AdvertisementMainID && a.IsApproved == true
                                    select new CustDetails
                                    {
                                        CustID = c.ID,
                                        AdFirmName = c.FirmName,
                                        CityID = c.City.Value,
                                        VillageLocalityName = sc.VillageLocalityName,
                                        StateID = c.State.Value,
                                        StateName = s.StateName,
                                        ChildCategoryId = a.ProductID.Value,
                                        ChildCategoryName = dbContext.tblItemCategories.Where(cc => cc.ChildCategoryID == a.ProductID).FirstOrDefault().ItemName,
                                        FullScreenAdURL = a.AdImageURL,
                                        businessTypes = (from bt in dbContext.tblBusinessTypewithCusts
                                                         join b in dbContext.tblBusinessTypes on bt.BusinessTypeID equals b.ID
                                                         where bt.CustID == a.CustID
                                                         select new BusinessTypes
                                                         {
                                                             BusinessTypeID = bt.BusinessTypeID,
                                                             BusinessTypeName = b.Type,
                                                         }).ToList(),
                                    }).FirstOrDefault();
                        return main;
                    }
                    else
                    {
                        return null;
                    }

                    //if (main != null)
                    //    FullScreenURL = main.AdImageURL;
                    //adsDateFiltered = adsDateFiltered.Where(a => itemCategories.Any(i => i.ItemId == a.ProductID)).ToList();

                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public AdvertisementMain PostAdText(AdvertisementMain main)
        {
            AdvertisementMain Result = new AdvertisementMain();
            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        //Update Ad Text Advertisement Main Table
                        tblAdvertisementMain tblAdvertisementMain = new tblAdvertisementMain();
                        tblAdvertisementMain.ID = main.AdvertisementMainID;
                        tblAdvertisementMain.AdText = main.AdText;
                        tblAdvertisementMain.IsRejected = false;
                        dbContext.tblAdvertisementMains.Attach(tblAdvertisementMain);
                        dbContext.Entry(tblAdvertisementMain).Property(x => x.AdText).IsModified = true;
                        dbContext.Entry(tblAdvertisementMain).Property(x => x.IsRejected).IsModified = true;
                        dbContext.SaveChanges();
                        dbcxtransaction.Commit();
                        Result.StatusCode = HttpStatusCode.OK;
                        Result.AdvertisementMainID = tblAdvertisementMain.ID;
                        Result.DispayMessage = "Advertisement Text added successfully..";
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.StatusCode = HttpStatusCode.InternalServerError;
                Result.DispayMessage = ex.Message;
                return Result;
            }
        }

        //Get banner images to display in the home screen of an user
        public List<Advertisement> GetBannerImages(int CustID, int AdType)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    int days = Convert.ToInt32(ConfigurationManager.AppSettings["Days"].ToString());
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    DateTimeNow = DateTimeNow.AddDays(days);
                    List<Advertisement> advertisements = new List<Advertisement>();

                    //Get UserInfo
                    CustomerInfo customerInfo = new CustomerInfo();
                    customerInfo = (from c in dbContext.tblCustomerDetails
                                    join s in dbContext.tblStates on c.State equals s.StateID
                                    join sc in dbContext.tblStateWithCities on c.City equals sc.StatewithCityID
                                    where c.ID == CustID
                                    select new CustomerInfo
                                    {
                                        CustID = c.ID,
                                        CityID = c.City,
                                        DistrictID = sc.DistrictID,
                                        StateID = c.State,
                                        StateName = s.StateName,
                                        CityName = sc.VillageLocalityName,
                                        FirmName = c.FirmName,
                                        MobileNumber = c.MobileNumber,
                                    }).FirstOrDefault();

                    //Get Time Slot ID 
                    var TimeSlots = dbContext.tblAdvertisementTimeSlots.ToList();
                    int TimeSlotID = TimeSlots.Where(t => DateTimeNow.TimeOfDay >= t.FromTime && DateTimeNow.TimeOfDay <= t.ToTime).FirstOrDefault().ID;

                    //Default (Banner)
                    int AdTypeID = 2;
                    var AdTypes = dbContext.tblAdvertisementTypes.ToList();

                    if (AdType == 2)
                    {
                        //Get Banner Advertisement ID
                        AdTypeID = AdTypes.Where(at => at.Type.ToLower() == "bannerad").FirstOrDefault().ID;
                    }
                    else if (AdType == 3)
                    {
                        //Get Text Advertisement ID
                        AdTypeID = AdTypes.Where(at => at.Type.ToLower() == "textad").FirstOrDefault().ID;
                    }

                    //Get User State, District and Cities

                    advertisements = (from a in dbContext.tblAdvertisementMains
                                      join ad in dbContext.tblAdvertisements on a.ID equals ad.AdvertisementMainID
                                      join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                                      join c in dbContext.tblItemCategories on a.ProductID equals c.ChildCategoryID
                                      join sc in dbContext.tblSubCategoryProductWithCusts on c.ChildCategoryID equals sc.SubCategoryId
                                      join cu in dbContext.tblCustomerDetails on sc.CustID equals cu.ID
                                      where sc.CustID == CustID
                                      && ad.TypeOfAdvertisementID == AdTypeID
                                      && ad.AdTimeSlotID == TimeSlotID && a.IsApproved == true && a.PaymentStatus == true
                                      && (DbFunctions.TruncateTime(ad.FromDate.Value) >= DateTimeNow.Date)
                                      //&& ad.IntervalsPerDay < ad.CompletedIntervals
                                      select new Advertisement
                                      {
                                          AdvertisementMainID = a.ID,
                                          AdvertisementID = ad.ID,
                                          AdvertisementName = a.AdvertisementName,
                                          CreatedDate = a.CreatedDate,
                                          AdvertisementType = at.Type,
                                          AdImageURL = a.AdImageURL,
                                          AdText = a.AdText == null ? string.Empty : a.AdText,
                                          ProductName = c.ItemName,
                                          AdvertisementAreaID = ad.AdvertisementAreaID,
                                          FromDate = ad.FromDate.Value,
                                          ToDate = ad.ToDate.Value,
                                          ProductID = a.ProductID,
                                          CurrentAdTime = ad.CurrentAdTime,
                                          AdStartTime = ad.AdStartTime,
                                          NextAdTime = ad.NextAdTime,
                                          TypeOfAdvertisementID = ad.TypeOfAdvertisementID,
                                          CustID = a.CustID,
                                          FirmName = dbContext.tblCustomerDetails.Where(ct => ct.ID == a.CustID).FirstOrDefault().FirmName,
                                          ExpiryDateStr = a.ToDate.ToString(),
                                          AdTimeSlotID = ad.AdTimeSlotID,
                                          CompletedIntervals = ad.CompletedIntervals,
                                          IntervalsPerDay = ad.IntervalsPerDay,
                                          IsCompanyAd = false
                                      }).ToList();

                    //Filter by national, state, district and city area
                    if (advertisements != null && advertisements.Count() > 0)
                    {
                        advertisements.ForEach(m => m.ExpiryDateStr = Convert.ToDateTime(m.ExpiryDateStr).ToString("dd/MM/yyyy"));
                        //advertisements = advertisements.Where(a => a.FromDate.Value.Date == DateTimeNow.Date).ToList();
                        //Loop through and check national, state and city wise advertisement
                        List<int> AdMainIDArray = new List<int>();
                        foreach (var item in advertisements)
                        {
                            #region Filter Advertisement area
                            //prods.Remove(prods.Single(s => s.ID == 1));

                            string AdAreaName = dbContext.tblAdvertisementAreas.Where(ar => ar.ID == item.AdvertisementAreaID).FirstOrDefault().AdvertisementAreaName;

                            if (AdAreaName.ToLower() != "national level")
                            {
                                if (AdAreaName.ToLower() == "state level")
                                {
                                    List<tblAdvertisementState> states = dbContext.tblAdvertisementStates.Where(s => s.AdvertisementMainID == item.AdvertisementMainID).ToList();
                                    if (states.Any(s => s.StateID.Value == customerInfo.StateID.Value))
                                    {
                                        AdMainIDArray.Add(item.AdvertisementMainID);
                                    }
                                }
                                else if (AdAreaName.ToLower() == "district level")
                                {
                                    List<tblAdvertisementDistrict> districts = dbContext.tblAdvertisementDistricts.Where(s => s.AdvertisementMainID == item.AdvertisementMainID).ToList();
                                    if (districts.Any(s => s.DistrictID.Value == customerInfo.DistrictID.Value))
                                    {
                                        AdMainIDArray.Add(item.AdvertisementMainID);
                                    }
                                }
                                else if (AdAreaName.ToLower() == "city level")
                                {
                                    List<tblAdvertisementCity> cities = dbContext.tblAdvertisementCities.Where(s => s.AdvertisementMainID == item.AdvertisementMainID).ToList();
                                    if (cities.Any(s => s.StateWithCityID.Value == customerInfo.CityID.Value))
                                    {
                                        AdMainIDArray.Add(item.AdvertisementMainID);
                                    }
                                }
                            }
                            else
                            {
                                AdMainIDArray.Add(item.AdvertisementMainID);
                            }
                            #endregion                            
                        }
                        if (AdMainIDArray != null && AdMainIDArray.Count() > 0)
                        {
                            advertisements = advertisements.Where(a => AdMainIDArray.Contains(a.AdvertisementMainID)).ToList();
                            if (advertisements != null && advertisements.Count() > 0)
                            {
                                foreach (var item in advertisements)
                                {
                                    //#region Update the current and next start time of the advertisement
                                    //// get the first start time of the time slot
                                    //TimeSpan StartTime = new TimeSpan();

                                    //TimeSpan TimeSlotFromTime = dbContext.tblAdvertisementTimeSlots.Where(t => t.ID == item.AdTimeSlotID).FirstOrDefault().FromTime.Value;
                                    //TimeSpan TimeSlotToTime = dbContext.tblAdvertisementTimeSlots.Where(t => t.ID == item.AdTimeSlotID).FirstOrDefault().ToTime.Value;
                                    //int CompletedIntervals = 0;
                                    //if (item.CompletedIntervals != null)
                                    //    CompletedIntervals = item.CompletedIntervals.Value + 1;

                                    //TimeSpan TimeSlotMinutes = (TimeSlotToTime - TimeSlotFromTime);
                                    //int TotalHoursPerSlot = Convert.ToInt32(TimeSlotMinutes.Hours);

                                    ////get ad duration in seconds and max durations allowed from admin settings
                                    //int AdDurationInSeconds = dbContext.tblAdminSettings.FirstOrDefault().AdDurationInSeconds.Value;
                                    //int MaxDurationsAllowedPerHr = dbContext.tblAdminSettings.FirstOrDefault().MaxDurationsAllowedPerHr.Value;

                                    //StartTime = item.NextAdTime.Value;

                                    ////if (advertisements != null && advertisements.Count() > 0)
                                    ////{

                                    ////    //var advertisement = (from a in advertisements.ToList()
                                    ////    //                     where a.FromDate.Value.Date == item.FromDate.Value.Date && a.ToDate.Value.Date == item.ToDate.Value.Date
                                    ////    //                     && a.AdTimeSlotID == item.AdTimeSlotID
                                    ////    //                     select a).OrderByDescending(x => x.AdStartTime).FirstOrDefault();
                                    ////    //if (advertisement != null)
                                    ////    //{
                                    ////    //    StartTime = (from a in advertisements.ToList()
                                    ////    //                 where a.FromDate.Value.Date == item.FromDate.Value.Date && a.ToDate.Value.Date == item.ToDate.Value.Date
                                    ////    //                 && a.AdTimeSlotID == item.AdTimeSlotID
                                    ////    //                 select a).OrderByDescending(x => x.AdStartTime).FirstOrDefault().AdStartTime.Value;
                                    ////    //    StartTime = StartTime.Add(TimeSpan.FromSeconds(AdDurationInSeconds));
                                    ////    //}
                                    ////    //else
                                    ////    //{
                                    ////    //    StartTime = TimeSlotFromTime;
                                    ////    //}
                                    ////}
                                    ////if (StartTime == new TimeSpan())
                                    ////    StartTime = TimeSlotFromTime;

                                    ////add the next time (1 hour)
                                    //TimeSpan CurrentStartTime = StartTime;
                                    //int DiffSeconds = Convert.ToInt32(TimeSlotMinutes.TotalSeconds) / MaxDurationsAllowedPerHr;
                                    //TimeSpan NextStartTime = StartTime + TimeSpan.FromSeconds(DiffSeconds);
                                    //tblAdvertisement tblAdvertisement = new tblAdvertisement();
                                    //tblAdvertisement.AdvertisementMainID = item.AdvertisementMainID;
                                    //tblAdvertisement.AdvertisementID = item.AdvertisementID;
                                    //tblAdvertisement.CurrentAdTime = CurrentStartTime;
                                    //tblAdvertisement.NextAdTime = NextStartTime;
                                    //tblAdvertisement.CompletedIntervals = CompletedIntervals;
                                    //dbContext.tblAdvertisements.Attach(tblAdvertisement);
                                    //dbContext.Entry(tblAdvertisement).Property(a => a.CurrentAdTime).IsModified = true;
                                    //dbContext.Entry(tblAdvertisement).Property(a => a.NextAdTime).IsModified = true;
                                    //dbContext.SaveChanges();
                                    //#endregion
                                    item.customerInfo = customerInfo;
                                }
                            }
                        }
                        else
                            advertisements = new List<Advertisement>();
                    }

                    return advertisements;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<Advertisement> GetTextAds(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    int days = Convert.ToInt32(ConfigurationManager.AppSettings["Days"].ToString());
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    DateTimeNow = DateTimeNow.AddDays(days);
                    List<Advertisement> advertisements = new List<Advertisement>();

                    //Get UserInfo
                    CustomerInfo customerInfo = new CustomerInfo();
                    customerInfo = (from c in dbContext.tblCustomerDetails
                                    join sc in dbContext.tblStateWithCities on c.City equals sc.StatewithCityID
                                    where c.ID == CustID
                                    select new CustomerInfo
                                    {
                                        CustID = c.ID,
                                        CityID = c.City,
                                        DistrictID = sc.DistrictID,
                                        StateID = c.State
                                    }).FirstOrDefault();

                    //Get Time Slot ID 
                    var TimeSlots = dbContext.tblAdvertisementTimeSlots.ToList();
                    int TimeSlotID = TimeSlots.Where(t => DateTimeNow.TimeOfDay >= t.FromTime && DateTimeNow.TimeOfDay <= t.ToTime).FirstOrDefault().ID;

                    //Get Banner Advertisement ID
                    int AdTypeID = dbContext.tblAdvertisementTypes.Where(at => at.Type.ToLower() == "textad").FirstOrDefault().ID;

                    //Get User State, District and Cities

                    advertisements = (from a in dbContext.tblAdvertisementMains
                                      join ad in dbContext.tblAdvertisements on a.ID equals ad.AdvertisementMainID
                                      join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                                      join c in dbContext.tblItemCategories on a.ProductID equals c.ChildCategoryID
                                      join sc in dbContext.tblSubCategoryProductWithCusts on c.ChildCategoryID equals sc.SubCategoryId
                                      join cu in dbContext.tblCustomerDetails on sc.CustID equals cu.ID
                                      where sc.CustID == CustID
                                      && ad.TypeOfAdvertisementID == AdTypeID
                                      && ad.AdTimeSlotID == TimeSlotID && a.IsApproved == true && a.PaymentStatus == true
                                      select new Advertisement
                                      {
                                          AdvertisementMainID = a.ID,
                                          AdvertisementID = ad.ID,
                                          AdvertisementName = a.AdvertisementName,
                                          CreatedDate = a.CreatedDate,
                                          AdvertisementType = at.Type,
                                          AdImageURL = a.AdImageURL,
                                          AdText = a.AdText == null ? string.Empty : a.AdText,
                                          ProductName = c.ItemName,
                                          AdvertisementAreaID = ad.AdvertisementAreaID,
                                          FromDate = ad.FromDate.Value,
                                          ToDate = ad.ToDate.Value,
                                          ProductID = a.ProductID,
                                          CurrentAdTime = ad.CurrentAdTime,
                                          AdStartTime = ad.AdStartTime,
                                          NextAdTime = ad.NextAdTime,
                                          TypeOfAdvertisementID = ad.TypeOfAdvertisementID,
                                          CustID = a.CustID,
                                          FirmName = cu.FirmName,
                                          ExpiryDateStr = a.ToDate.ToString(),
                                          AdTimeSlotID = ad.AdTimeSlotID,
                                          CompletedIntervals = ad.CompletedIntervals,
                                          IntervalsPerDay = ad.IntervalsPerDay
                                      }).ToList();

                    //Filter by national, state, district and city area
                    if (advertisements != null && advertisements.Count() > 0)
                    {
                        advertisements.ForEach(m => m.ExpiryDateStr = Convert.ToDateTime(m.ExpiryDateStr).ToString("dd/MM/yyyy"));
                        //advertisements = advertisements.Where(a => a.FromDate.Value.Date == DateTimeNow.Date).ToList();
                        //Loop through and check national, state and city wise advertisement
                        List<int> AdMainIDArray = new List<int>();
                        foreach (var item in advertisements)
                        {
                            #region Filter Advertisement area
                            //prods.Remove(prods.Single(s => s.ID == 1));

                            string AdAreaName = dbContext.tblAdvertisementAreas.Where(ar => ar.ID == item.AdvertisementAreaID).FirstOrDefault().AdvertisementAreaName;

                            if (AdAreaName.ToLower() != "national level")
                            {
                                if (AdAreaName.ToLower() == "state level")
                                {
                                    List<tblAdvertisementState> states = dbContext.tblAdvertisementStates.Where(s => s.AdvertisementMainID == item.AdvertisementMainID).ToList();
                                    if (states.Any(s => s.StateID.Value == customerInfo.StateID.Value))
                                    {
                                        AdMainIDArray.Add(item.AdvertisementMainID);
                                    }
                                }
                                else if (AdAreaName.ToLower() == "district level")
                                {
                                    List<tblAdvertisementDistrict> districts = dbContext.tblAdvertisementDistricts.Where(s => s.AdvertisementMainID == item.AdvertisementMainID).ToList();
                                    if (districts.Any(s => s.DistrictID.Value == customerInfo.DistrictID.Value))
                                    {
                                        AdMainIDArray.Add(item.AdvertisementMainID);
                                    }
                                }
                                else if (AdAreaName.ToLower() == "city level")
                                {
                                    List<tblAdvertisementCity> cities = dbContext.tblAdvertisementCities.Where(s => s.AdvertisementMainID == item.AdvertisementMainID).ToList();
                                    if (cities.Any(s => s.StateWithCityID.Value == customerInfo.CityID.Value))
                                    {
                                        AdMainIDArray.Add(item.AdvertisementMainID);
                                    }
                                }
                            }
                            else
                            {
                                AdMainIDArray.Add(item.AdvertisementMainID);
                            }
                            #endregion                            
                        }
                        if (AdMainIDArray != null && AdMainIDArray.Count() > 0)
                        {
                            advertisements = advertisements.Where(a => AdMainIDArray.Contains(a.AdvertisementMainID)).ToList();
                            if (advertisements != null && advertisements.Count() > 0)
                            {
                                foreach (var item in advertisements)
                                {
                                    #region Update the current and next start time of the advertisement
                                    // get the first start time of the time slot
                                    TimeSpan StartTime = new TimeSpan();

                                    TimeSpan TimeSlotFromTime = dbContext.tblAdvertisementTimeSlots.Where(t => t.ID == item.AdTimeSlotID).FirstOrDefault().FromTime.Value;
                                    TimeSpan TimeSlotToTime = dbContext.tblAdvertisementTimeSlots.Where(t => t.ID == item.AdTimeSlotID).FirstOrDefault().ToTime.Value;
                                    int CompletedIntervals = 0;
                                    if (item.CompletedIntervals != null)
                                        CompletedIntervals = item.CompletedIntervals.Value;

                                    TimeSpan TimeSlotMinutes = (TimeSlotToTime - TimeSlotFromTime);
                                    int TotalHoursPerSlot = Convert.ToInt32(TimeSlotMinutes.Hours);

                                    //get ad duration in seconds and max durations allowed from admin settings
                                    int AdDurationInSeconds = dbContext.tblAdminSettings.FirstOrDefault().AdDurationInSeconds.Value;
                                    int MaxDurationsAllowedPerHr = dbContext.tblAdminSettings.FirstOrDefault().MaxDurationsAllowedPerHr.Value;

                                    StartTime = item.NextAdTime.Value;

                                    //if (advertisements != null && advertisements.Count() > 0)
                                    //{

                                    //    //var advertisement = (from a in advertisements.ToList()
                                    //    //                     where a.FromDate.Value.Date == item.FromDate.Value.Date && a.ToDate.Value.Date == item.ToDate.Value.Date
                                    //    //                     && a.AdTimeSlotID == item.AdTimeSlotID
                                    //    //                     select a).OrderByDescending(x => x.AdStartTime).FirstOrDefault();
                                    //    //if (advertisement != null)
                                    //    //{
                                    //    //    StartTime = (from a in advertisements.ToList()
                                    //    //                 where a.FromDate.Value.Date == item.FromDate.Value.Date && a.ToDate.Value.Date == item.ToDate.Value.Date
                                    //    //                 && a.AdTimeSlotID == item.AdTimeSlotID
                                    //    //                 select a).OrderByDescending(x => x.AdStartTime).FirstOrDefault().AdStartTime.Value;
                                    //    //    StartTime = StartTime.Add(TimeSpan.FromSeconds(AdDurationInSeconds));
                                    //    //}
                                    //    //else
                                    //    //{
                                    //    //    StartTime = TimeSlotFromTime;
                                    //    //}
                                    //}
                                    //if (StartTime == new TimeSpan())
                                    //    StartTime = TimeSlotFromTime;

                                    //add the next time (1 hour)
                                    TimeSpan CurrentStartTime = StartTime;
                                    int DiffSeconds = Convert.ToInt32(TimeSlotMinutes.TotalSeconds) / MaxDurationsAllowedPerHr;
                                    TimeSpan NextStartTime = StartTime + TimeSpan.FromSeconds(DiffSeconds);
                                    tblAdvertisement tblAdvertisement = new tblAdvertisement();
                                    tblAdvertisement.AdvertisementMainID = item.AdvertisementMainID;
                                    tblAdvertisement.ID = item.AdvertisementID;
                                    tblAdvertisement.CurrentAdTime = CurrentStartTime;
                                    tblAdvertisement.NextAdTime = NextStartTime;
                                    dbContext.tblAdvertisements.Attach(tblAdvertisement);
                                    dbContext.Entry(tblAdvertisement).Property(a => a.CurrentAdTime).IsModified = true;
                                    dbContext.Entry(tblAdvertisement).Property(a => a.NextAdTime).IsModified = true;
                                    dbContext.SaveChanges();
                                    #endregion
                                }
                            }
                        }
                        else
                            advertisements = new List<Advertisement>();
                    }

                    return advertisements;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<Advertisement> GetAllAdvertisements(int CustID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    int days = Convert.ToInt32(ConfigurationManager.AppSettings["Days"].ToString());
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    DateTimeNow = DateTimeNow.AddDays(days);
                    List<Advertisement> advertisements = new List<Advertisement>();

                    //Get Time Slot ID 
                    var TimeSlots = dbContext.tblAdvertisementTimeSlots.ToList();
                    int TimeSlotID = TimeSlots.Where(t => DateTimeNow.TimeOfDay >= t.FromTime && DateTimeNow.TimeOfDay <= t.ToTime).FirstOrDefault().ID;

                    //Get UserInfo
                    CustomerInfo customerInfo = new CustomerInfo();
                    customerInfo = (from c in dbContext.tblCustomerDetails
                                    join s in dbContext.tblStates on c.State equals s.StateID
                                    join sc in dbContext.tblStateWithCities on c.City equals sc.StatewithCityID
                                    where c.ID == CustID
                                    select new CustomerInfo
                                    {
                                        CustID = c.ID,
                                        CityID = c.City,
                                        DistrictID = sc.DistrictID,
                                        StateID = c.State,
                                        StateName = s.StateName,
                                        CityName = sc.VillageLocalityName,
                                        FirmName = c.FirmName,
                                        MobileNumber = c.MobileNumber,
                                    }).FirstOrDefault();

                    //Get User State, District and Cities

                    advertisements = (from a in dbContext.tblAdvertisementMains
                                      join ad in dbContext.tblAdvertisements on a.ID equals ad.AdvertisementMainID
                                      join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                                      join c in dbContext.tblItemCategories on a.ProductID equals c.ChildCategoryID
                                      join sc in dbContext.tblSubCategoryProductWithCusts on c.ChildCategoryID equals sc.SubCategoryId
                                      join cu in dbContext.tblCustomerDetails on sc.CustID equals cu.ID
                                      where sc.CustID == CustID && ad.AdTimeSlotID == TimeSlotID
                                      && a.IsApproved == true && a.PaymentStatus == true
                                      select new Advertisement
                                      {
                                          AdvertisementMainID = a.ID,
                                          AdvertisementID = ad.ID,
                                          AdvertisementName = a.AdvertisementName,
                                          CreatedDate = a.CreatedDate,
                                          AdvertisementType = at.Type,
                                          AdImageURL = a.AdImageURL,
                                          AdText = a.AdText == null ? string.Empty : a.AdText,
                                          ProductName = c.ItemName,
                                          AdvertisementAreaID = ad.AdvertisementAreaID,
                                          FromDate = ad.FromDate.Value,
                                          ToDate = ad.ToDate.Value,
                                          ProductID = a.ProductID,
                                          CurrentAdTime = ad.CurrentAdTime,
                                          AdStartTime = ad.AdStartTime,
                                          NextAdTime = ad.NextAdTime,
                                          TypeOfAdvertisementID = ad.TypeOfAdvertisementID,
                                          CustID = a.CustID,
                                          FirmName = dbContext.tblCustomerDetails.Where(cust => cust.ID == a.CustID).FirstOrDefault().FirmName,
                                          ExpiryDateStr = a.ToDate.ToString(),
                                          AdTimeSlotID = ad.AdTimeSlotID,
                                          CompletedIntervals = ad.CompletedIntervals,
                                          IntervalsPerDay = ad.IntervalsPerDay,
                                      }).ToList();

                    //Filter by national, state, district and city area
                    if (advertisements != null && advertisements.Count() > 0)
                    {
                        advertisements.ForEach(m => m.ExpiryDateStr = Convert.ToDateTime(m.ExpiryDateStr).ToString("dd/MM/yyyy"));
                        advertisements = advertisements.Where(a => a.FromDate.Value.Date == DateTimeNow.Date).ToList();
                        //Loop through and check national, state and city wise advertisement
                        List<int> AdMainIDArray = new List<int>();
                        foreach (var item in advertisements)
                        {
                            #region Filter Advertisement area
                            //prods.Remove(prods.Single(s => s.ID == 1));
                            item.customerInfo = customerInfo;
                            string AdAreaName = dbContext.tblAdvertisementAreas.Where(ar => ar.ID == item.AdvertisementAreaID).FirstOrDefault().AdvertisementAreaName;

                            if (AdAreaName.ToLower() != "national level")
                            {
                                if (AdAreaName.ToLower() == "state level")
                                {
                                    List<tblAdvertisementState> states = dbContext.tblAdvertisementStates.Where(s => s.AdvertisementMainID == item.AdvertisementMainID).ToList();
                                    if (states.Any(s => s.StateID.Value == customerInfo.StateID.Value))
                                    {
                                        AdMainIDArray.Add(item.AdvertisementMainID);
                                    }
                                }
                                else if (AdAreaName.ToLower() == "district level")
                                {
                                    List<tblAdvertisementDistrict> districts = dbContext.tblAdvertisementDistricts.Where(s => s.AdvertisementMainID == item.AdvertisementMainID).ToList();
                                    if (districts.Any(s => s.DistrictID.Value == customerInfo.DistrictID.Value))
                                    {
                                        AdMainIDArray.Add(item.AdvertisementMainID);
                                    }
                                }
                                else if (AdAreaName.ToLower() == "city level")
                                {
                                    List<tblAdvertisementCity> cities = dbContext.tblAdvertisementCities.Where(s => s.AdvertisementMainID == item.AdvertisementMainID).ToList();
                                    if (cities.Any(s => s.StateWithCityID.Value == customerInfo.CityID.Value))
                                    {
                                        AdMainIDArray.Add(item.AdvertisementMainID);
                                    }
                                }
                            }
                            else
                            {
                                AdMainIDArray.Add(item.AdvertisementMainID);
                            }
                            #endregion
                        }
                        if (AdMainIDArray != null && AdMainIDArray.Count() > 0)
                        {
                            advertisements = advertisements.Where(a => AdMainIDArray.Contains(a.AdvertisementMainID)).ToList();
                        }
                        else
                            advertisements = new List<Advertisement>();
                    }
                    return advertisements.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException == null ? null : ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public string CancelAdvertisement(int AdvertisementMainID)
        {
            string Result = string.Empty;
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    var main = dbContext.tblAdvertisementMains.AsNoTracking().Where(a => a.ID == AdvertisementMainID).FirstOrDefault();
                    if (main != null)
                    {
                        tblAdvertisementMain tblAdvertisementMain = new tblAdvertisementMain();
                        tblAdvertisementMain.IsCancelled = true;
                        tblAdvertisementMain.ID = AdvertisementMainID;
                        dbContext.tblAdvertisementMains.Attach(tblAdvertisementMain);
                        dbContext.Entry(tblAdvertisementMain).Property(a => a.IsCancelled).IsModified = true;
                        dbContext.SaveChanges();
                        Result = "Advertisement cancelled successfully";
                    }
                    else
                    {
                        Result = "Invalid Advertisement details";
                    }
                    return Result;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result = "Internal Server Error";
                return Result;
            }
        }
        public PaymentDetails PostAdPayments(PaymentDetails paymentDetails)
        {
            PaymentDetails Result = new PaymentDetails();
            DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        //Insert into payment table
                        tblPayment tblPayment = new tblPayment();
                        tblPayment.AdvertisementMainID = paymentDetails.AdvertisementMainID;
                        tblPayment.CustID = paymentDetails.CustID;
                        tblPayment.CreatedBy = paymentDetails.CustID.Value;
                        tblPayment.CreatedDate = DateTimeNow;
                        tblPayment.OrderAmount = paymentDetails.OrderAmount;
                        tblPayment.OrderID = paymentDetails.OrderID;
                        tblPayment.PreAuthID = paymentDetails.PreAuthID;
                        tblPayment.TxnID = paymentDetails.TxnID;
                        tblPayment.TxnStatus = paymentDetails.TxnStatus;
                        tblPayment.TxnStatusCode = paymentDetails.TxnStatusCode;
                        tblPayment.TxnStatusMessage = paymentDetails.TxnStatusMessage;
                        tblPayment.txnPaymentMode = paymentDetails.txnPaymentMode;
                        tblPayment.txnSignature = paymentDetails.txnSignature;
                        tblPayment.txnType = paymentDetails.txnType;
                        tblPayment.txnTime = paymentDetails.txnTime;
                        tblPayment.txnReferenceID = paymentDetails.txnReferenceID;
                        dbContext.tblPayments.Add(tblPayment);
                        dbContext.SaveChanges();
                        dbcxtransaction.Commit();
                        Result.DisplayMessage = "Success..";
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result.DisplayMessage = ex.Message;
                return Result;
            }
        }

        //Admin Portal
        public List<AdvertisementMain> GetAdvertisementsForPortal(SearchOptions searchOptions)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    //int days = Convert.ToInt32(ConfigurationManager.AppSettings["Days"].ToString());
                    //DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    //DateTimeNow = DateTimeNow.AddDays(days);
                    IEnumerable<AdvertisementMain> advertisements = new List<AdvertisementMain>();
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    advertisements = (from a in dbContext.tblAdvertisementMains
                                      join ar in dbContext.tblAdvertisementAreas on a.AdvertisementAreaID equals ar.ID
                                      join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                                      join c in dbContext.tblItemCategories on a.ProductID equals c.ID
                                      join cu in dbContext.tblCustomerDetails on a.CustID equals cu.ID
                                      where a.IsCancelled == false
                                      //&& (a.PaymentStatus == false || a.IsApproved == false)
                                      select new AdvertisementMain
                                      {
                                          AdvertisementMainID = a.ID,
                                          AdvertisementName = a.AdvertisementName,
                                          CreatedDate = a.CreatedDate,
                                          AdvertisementType = at.Type,
                                          ProductName = c.ItemName,//    c.ItemName,
                                          AdvertisementAreaID = a.AdvertisementAreaID,
                                          AdvertisementArea = ar.AdvertisementAreaName,
                                          FromDate = a.FromDate.Value,
                                          ToDate = a.ToDate.Value,
                                          ProductID = a.ProductID,
                                          TypeOfAdvertisementID = a.TypeOfAdvertisementID,
                                          CustID = a.CustID,
                                          customerInfo = new CustomerInfo
                                          {
                                              FirmName = dbContext.tblCustomerDetails.Where(cust => cust.ID == a.CustID).FirstOrDefault().FirmName,
                                              StateID = cu.State,
                                              CityID = cu.City,
                                          },
                                          states = dbContext.tblAdvertisementStates.Where(ast => ast.AdvertisementMainID == a.ID).ToList(),
                                          cities = dbContext.tblAdvertisementCities.Where(ast => ast.AdvertisementMainID == a.ID).ToList(),
                                          ApprovalStatus = a.IsApproved == false ? "Pending" : "Approved",
                                          PaymentStatus = a.PaymentStatus == false ? "Pending" : "Approved",
                                      }).AsEnumerable();

                    advertisements = advertisements.Where(a => a.ToDate.Value.Date >= DateTimeNow.Date).AsEnumerable();

                    if (!string.IsNullOrEmpty(searchOptions.AdDate))
                    {
                        DateTime searchDate = Convert.ToDateTime(searchOptions.AdDate);
                        advertisements = advertisements.Where(a => a.FromDate.Value.Date == searchDate.Date).AsEnumerable();
                    }

                    if (searchOptions.CityID != null)
                        advertisements = advertisements.Where(a => a.cities.Any(ct => ct.StateWithCityID == searchOptions.CityID)).AsEnumerable();

                    if (searchOptions.StateID != null)
                        advertisements = advertisements.Where(a => a.states.Any(ct => ct.StateID == searchOptions.StateID)).AsEnumerable();

                    if (searchOptions.TypeOfAdvertisementID != null)
                        advertisements = advertisements.Where(a => a.TypeOfAdvertisementID == searchOptions.TypeOfAdvertisementID).AsEnumerable();

                    if (searchOptions.AdvertisementAreaID != null)
                        advertisements = advertisements.Where(a => a.AdvertisementAreaID == searchOptions.AdvertisementAreaID).AsEnumerable();

                    if (!string.IsNullOrEmpty(searchOptions.FirmName))
                        advertisements = advertisements.Where(a => a.customerInfo.FirmName.ToLower().Contains(searchOptions.FirmName)).AsEnumerable();
                    return advertisements.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException == null ? null : ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public AdvertisementMain GetAdvertisementDetailsForPortal(int AdvertisementMainID)
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    AdvertisementMain advertisementMain = new AdvertisementMain();
                    advertisementMain = (from a in dbContext.tblAdvertisementMains
                                         join at in dbContext.tblAdvertisementTypes on a.TypeOfAdvertisementID equals at.ID
                                         join ar in dbContext.tblAdvertisementAreas on a.AdvertisementAreaID equals ar.ID
                                         join c in dbContext.tblCustomerDetails on a.CustID equals c.ID
                                         join s in dbContext.tblStates on c.State equals s.ID
                                         join sc in dbContext.tblStateWithCities on c.City equals sc.ID
                                         join cp in dbContext.tblItemCategories on a.ProductID equals cp.ID
                                         where a.ID == AdvertisementMainID
                                         select new AdvertisementMain
                                         {
                                             AdvertisementMainID = a.ID,
                                             AdvertisementName = a.AdvertisementName,
                                             CreatedDate = a.CreatedDate,
                                             BookingExpiryDate = a.BookingExpiryDate,
                                             CreatedDateStr = a.CreatedDate.ToString(),
                                             BookingExpiryDateStr = a.BookingExpiryDate.ToString(),
                                             AdvertisementArea = ar.AdvertisementAreaName,
                                             AdvertisementType = at.Type,
                                             TotalPrice = a.AdTotalPrice.Value,
                                             TotalDiscount = a.TotalDiscount,
                                             FinalPrice = a.FinalPrice,
                                             IGSTAmount = a.IGSTAmount,
                                             SGSTAmount = a.SGSTAmount,
                                             CGSTAmount = a.CGSTAmount,
                                             AdImageURL = a.AdImageURL,
                                             FromDate = a.FromDate,
                                             ToDate = a.ToDate,
                                             ApprovalStatus = a.IsApproved == true ? "Approved" : "Pending",
                                             PaymentStatus = a.PaymentStatus == true ? "Approved" : "Pending",
                                             IsApproved = a.IsApproved,
                                             IsPaymentPaid = a.PaymentStatus,
                                             IsCancelled = a.IsCancelled,
                                             IsCompleted = a.IsCompleted,
                                             IsRejected = a.IsRejected,
                                             InvoiceNumber = a.InvoiceNumber,
                                             ProformaInvoiceNumber = a.ProformaInvoiceNumber.ToString(),
                                             cities = dbContext.tblAdvertisementCities.Where(ac => ac.AdvertisementMainID == a.ID).AsEnumerable(),
                                             districts = dbContext.tblAdvertisementDistricts.Where(ac => ac.AdvertisementMainID == a.ID).AsEnumerable(),
                                             states = dbContext.tblAdvertisementStates.Where(ac => ac.AdvertisementMainID == a.ID).AsEnumerable(),
                                             AdText = a.AdText,
                                             AdTimeSlots = dbContext.tblAdTimeSlots.Where(t => t.AdvertisementID == a.ID).ToList(),
                                             ProductName = cp.ItemName,
                                             customerInfo = new CustomerInfo
                                             {
                                                 FirmName = c.FirmName,
                                                 CityName = sc.VillageLocalityName,
                                                 StateName = s.StateName,
                                                 MobileNumber = c.MobileNumber,
                                             },
                                             IsActive = a.IsActive,
                                             CustID = c.ID,
                                             paymentDetails = (from p in dbContext.tblPayments
                                                               where p.AdvertisementMainID == AdvertisementMainID
                                                               select new PaymentDetails
                                                               {
                                                                   AdvertisementMainID = p.AdvertisementMainID,
                                                                   CreatedBy = p.CreatedBy,
                                                                   CreatedDate = p.CreatedDate,
                                                                   CustID = p.CustID,
                                                                   ID = p.ID,
                                                                   OrderID = p.OrderID,
                                                                   TxnID = p.TxnID,
                                                                   txnPaymentMode = p.txnPaymentMode,
                                                                   txnReferenceID = p.txnReferenceID,
                                                                   txnSignature = p.txnSignature,
                                                                   TxnStatus = p.TxnStatus,
                                                                   TxnStatusCode = p.TxnStatusCode,
                                                                   TxnStatusMessage = p.TxnStatusMessage,
                                                                   txnTime = p.txnTime,
                                                                   txnType = p.txnType
                                                               }).ToList(),
                                         }).FirstOrDefault();

                    if (advertisementMain != null)
                    {
                        if (advertisementMain.IsApproved == true)
                        {
                            advertisementMain.IsExpired = false;
                        }
                        else if (advertisementMain.IsApproved.Value == false && advertisementMain.PaymentStatus.ToLower() == "pending" && advertisementMain.BookingExpiryDate.Value < DateTime.Now)
                            advertisementMain.IsExpired = true;
                        else
                            advertisementMain.IsExpired = false;
                        advertisementMain.FromDateStr = advertisementMain.FromDate.Value.ToString("dd/MM/yyyy");
                        advertisementMain.ToDateStr = advertisementMain.ToDate.Value.ToString("dd/MM/yyyy");

                        if (advertisementMain.IsApproved == false && advertisementMain.IsRejected == true)
                        {
                            advertisementMain.ApprovalStatus = "Rejected";
                        }
                        else if (advertisementMain.IsApproved == true && advertisementMain.IsRejected == true)
                        {
                            advertisementMain.PaymentStatus = "Rejected";
                        }
                        else if (advertisementMain.IsApproved == true && advertisementMain.IsPaymentPaid == true && advertisementMain.IsRejected == true)
                        {
                            advertisementMain.ApprovalStatus = "Stopped";
                            advertisementMain.PaymentStatus = "Stopped";
                        }
                    }

                    return advertisementMain;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<State> GetAdStates()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    List<State> stateList = new List<State>();
                    stateList = (from s in dbContext.tblStates
                                 join ad in dbContext.tblAdvertisementStates on s.ID equals ad.StateID
                                 join a in dbContext.tblAdvertisementMains on ad.AdvertisementMainID equals a.ID
                                 where a.IsCancelled == false
                                 && (DbFunctions.TruncateTime(a.FromDate.Value) >= DateTimeNow.Date)
                                 select new State
                                 {
                                     StateID = s.ID,
                                     StateName = s.StateName,
                                 }).Distinct().ToList();

                    return stateList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<District> GetAdDistricts()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    List<District> districts = new List<District>();
                    districts = (from s in dbContext.tblDistricts
                                 join ad in dbContext.tblAdvertisementDistricts on s.DistrictID equals ad.DistrictID
                                 join a in dbContext.tblAdvertisementMains on ad.AdvertisementMainID equals a.ID
                                 where a.IsCancelled == false
                                 && (DbFunctions.TruncateTime(a.FromDate.Value) >= DateTimeNow.Date)
                                 select new District
                                 {
                                     DistrictID = s.DistrictID,
                                     DistrictName = s.DistrictName,
                                 }).Distinct().ToList();

                    return districts;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<City> GetAdCities()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    List<City> stateList = new List<City>();
                    stateList = (from s in dbContext.tblStateWithCities
                                 join ad in dbContext.tblAdvertisementCities on s.ID equals ad.StateWithCityID
                                 join a in dbContext.tblAdvertisementMains on ad.AdvertisementMainID equals a.ID
                                 where a.IsCancelled == false
                                 && (DbFunctions.TruncateTime(a.FromDate.Value) >= DateTimeNow.Date)
                                 select new City
                                 {
                                     StateWithCityID = s.ID,
                                     VillageLocalityName = s.VillageLocalityName,
                                 }).Distinct().ToList();

                    return stateList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public string ApproveAd(AdvertisementMain advertisementMain, int UserID)
        {
            string Result = string.Empty;
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        JBNDBClass jBNDBClass = new JBNDBClass();
                        string Title = string.Empty, Body = string.Empty;
                        DateTime DateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        if (advertisementMain != null)
                        {
                            var user = (from c in dbContext.tblCustomerDetails
                                        where c.ID == advertisementMain.CustID
                                        select c).FirstOrDefault();
                            var IsApprovedFromDB = dbContext.tblAdvertisementMains.Find(advertisementMain.AdvertisementMainID);
                            dbContext.Entry(IsApprovedFromDB).State = EntityState.Detached;

                            tblAdvertisementMain tblAdvertisementMain = new tblAdvertisementMain();
                            tblAdvertisementMain.ID = advertisementMain.AdvertisementMainID;
                            if (advertisementMain.StatusType.ToLower() == "content approved")
                            {
                                tblAdvertisementMain.ID = advertisementMain.AdvertisementMainID;
                                tblAdvertisementMain.IsApproved = true;
                                tblAdvertisementMain.IsRejected = false;
                                tblAdvertisementMain.ModifiedBy = UserID;
                                tblAdvertisementMain.ModifiedDate = DateTimeNow;
                                tblAdvertisementMain.ContentApprovedDate = DateTimeNow;
                                dbContext.tblAdvertisementMains.Attach(tblAdvertisementMain);
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.IsApproved).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.ModifiedBy).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.ModifiedDate).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.ContentApprovedDate).IsModified = true;
                                dbContext.SaveChanges();
                                Body = ConfigurationManager.AppSettings["Advertisement Approved"].ToString();
                                Notification notification = new Notification { Body = Body, NotificationDate = DateTimeNow, Title = "Your Advertisement is Approved" };
                                Helper.SendNotification(user.DeviceID, notification);
                                PushNotifications pushNotifications = new PushNotifications()
                                {
                                    Title = "Your Advertisement is Approved",
                                    NotificationDate = DateTimeNow,
                                    CategoryName = string.Empty,
                                    ImageURL = string.Empty,
                                    PushNotification = Body,
                                };
                                jBNDBClass.SavePushNotifications(user.ID, pushNotifications, UserID);
                                Result = "Advertisement approved successfully";
                            }
                            else if (advertisementMain.StatusType.ToLower() == "content rejected")
                            {
                                tblAdvertisementMain.ID = advertisementMain.AdvertisementMainID;
                                tblAdvertisementMain.IsRejected = true;
                                tblAdvertisementMain.Remarks = advertisementMain.Remarks;
                                tblAdvertisementMain.ModifiedBy = UserID;
                                tblAdvertisementMain.ModifiedDate = DateTimeNow;
                                tblAdvertisementMain.IsApproved = false;
                                dbContext.tblAdvertisementMains.Attach(tblAdvertisementMain);
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.IsRejected).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.Remarks).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.ModifiedBy).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.ModifiedDate).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.IsApproved).IsModified = true;
                                dbContext.SaveChanges();
                                Result = "Advertisement rejected successfully";
                                Body = ConfigurationManager.AppSettings["Advertisement Rejected"].ToString();
                                Body = Body.Replace("%var%", advertisementMain.Remarks);
                                Title = "Advertisement " + IsApprovedFromDB.AdvertisementName + " Rejected";
                                Notification notification = new Notification { Body = Body, NotificationDate = DateTimeNow, Title = Title };
                                Helper.SendNotification(user.DeviceID, notification);
                                PushNotifications pushNotifications = new PushNotifications()
                                {
                                    Title = Title,
                                    NotificationDate = DateTimeNow,
                                    CategoryName = string.Empty,
                                    ImageURL = string.Empty,
                                    PushNotification = Body,
                                };
                                jBNDBClass.SavePushNotifications(user.ID, pushNotifications, UserID);
                            }
                            else if (advertisementMain.StatusType.ToLower() == "payment approved")
                            {
                                tblAdvertisementMain.IsApproved = true;
                                tblAdvertisementMain.PaymentStatus = true;
                                tblAdvertisementMain.IsRejected = false;
                                tblAdvertisementMain.IsActive = true;
                                tblAdvertisementMain.ID = advertisementMain.AdvertisementMainID;
                                tblAdvertisementMain.ModifiedBy = UserID;
                                tblAdvertisementMain.ModifiedDate = DateTimeNow;
                                tblAdvertisementMain.PaymentApprovedDate = DateTimeNow;
                                dbContext.tblAdvertisementMains.Attach(tblAdvertisementMain);
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.IsApproved).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.PaymentStatus).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.ModifiedBy).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.ModifiedDate).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.IsActive).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.PaymentApprovedDate).IsModified = true;
                                dbContext.SaveChanges();

                                var tbladvertisements = dbContext.tblAdvertisements.ToList().FindAll(a => a.AdvertisementMainID == advertisementMain.AdvertisementMainID).ToList();
                                foreach (var singleItem in tbladvertisements)
                                {
                                    tblAdvertisement advertisement = new tblAdvertisement();
                                    advertisement.ID = singleItem.ID;
                                    advertisement.IsActive = true;
                                    dbContext.tblAdvertisements.Attach(advertisement);
                                    dbContext.Entry(advertisement).Property(a => a.IsActive).IsModified = true;
                                }
                                dbContext.SaveChanges();

                                Body = ConfigurationManager.AppSettings["Payment Approved"].ToString() + advertisementMain.FromDateStr;
                                Body = Body.Replace("%VAR%", advertisementMain.AdvertisementName);
                                Title = "Your Payment Approved for Advertisement " + IsApprovedFromDB.AdvertisementName;
                                Notification notification = new Notification { Body = Body, NotificationDate = DateTimeNow, Title = Title };
                                Helper.SendNotification(user.DeviceID, notification);
                                PushNotifications pushNotifications = new PushNotifications()
                                {
                                    Title = Title,
                                    NotificationDate = DateTimeNow,
                                    CategoryName = string.Empty,
                                    ImageURL = string.Empty,
                                    PushNotification = Body,
                                };
                                jBNDBClass.SavePushNotifications(user.ID, pushNotifications, UserID);
                                Result = "Advertisement Payment approved successfully";
                            }
                            else if (advertisementMain.StatusType.ToLower() == "payment rejected")
                            {
                                tblAdvertisementMain.ID = advertisementMain.AdvertisementMainID;
                                tblAdvertisementMain.Remarks = advertisementMain.Remarks;
                                tblAdvertisementMain.ModifiedBy = UserID;
                                tblAdvertisementMain.ModifiedDate = DateTimeNow;
                                tblAdvertisementMain.PaymentStatus = false;
                                tblAdvertisementMain.IsRejected = true;
                                dbContext.tblAdvertisementMains.Attach(tblAdvertisementMain);
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.IsRejected).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.Remarks).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.ModifiedBy).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.ModifiedDate).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.IsApproved).IsModified = true;
                                dbContext.SaveChanges();
                                Result = "Advertisement rejected successfully";
                                Body = ConfigurationManager.AppSettings["Advertisement Rejected"].ToString();
                                Body = Body.Replace("%var%", advertisementMain.Remarks);
                                Title = "Your Payment Rejected for Advertisement " + IsApprovedFromDB.AdvertisementName;
                                Notification notification = new Notification { Body = Body, NotificationDate = DateTimeNow, Title = Title };
                                Helper.SendNotification(user.DeviceID, notification);
                                PushNotifications pushNotifications = new PushNotifications()
                                {
                                    Title = Title,
                                    NotificationDate = DateTimeNow,
                                    CategoryName = string.Empty,
                                    ImageURL = string.Empty,
                                    PushNotification = Body,
                                };
                                jBNDBClass.SavePushNotifications(user.ID, pushNotifications, UserID);
                            }
                            else if (advertisementMain.StatusType.ToLower() == "stop advertisement")
                            {
                                tblAdvertisementMain.ID = advertisementMain.AdvertisementMainID;
                                tblAdvertisementMain.IsRejected = true;
                                tblAdvertisementMain.Remarks = advertisementMain.Remarks;
                                tblAdvertisementMain.ModifiedBy = UserID;
                                tblAdvertisementMain.ModifiedDate = DateTimeNow;
                                tblAdvertisementMain.IsActive = false;
                                dbContext.tblAdvertisementMains.Attach(tblAdvertisementMain);
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.IsRejected).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.Remarks).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.ModifiedBy).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.ModifiedDate).IsModified = true;
                                dbContext.Entry(tblAdvertisementMain).Property(a => a.IsApproved).IsModified = true;
                                dbContext.SaveChanges();

                                var tbladvertisements = dbContext.tblAdvertisements.ToList().FindAll(a => a.AdvertisementMainID == advertisementMain.AdvertisementMainID).ToList();
                                foreach (var singleItem in tbladvertisements)
                                {
                                    tblAdvertisement advertisement = new tblAdvertisement();
                                    advertisement.ID = singleItem.ID;
                                    advertisement.IsActive = false;
                                    dbContext.tblAdvertisements.Attach(advertisement);
                                    dbContext.Entry(advertisement).Property(a => a.IsActive).IsModified = true;
                                }
                                dbContext.SaveChanges();

                                Result = "Advertisement stopped successfully";
                                Body = "Your Advertisement " + advertisementMain.AdvertisementName + " has been stopped.. Reason : " + advertisementMain.Remarks;
                                Title = "Your Advertisement stopped";
                                Notification notification = new Notification { Body = Body, NotificationDate = DateTimeNow, Title = Title };
                                Helper.SendNotification(user.DeviceID, notification);
                                PushNotifications pushNotifications = new PushNotifications()
                                {
                                    Title = Title,
                                    NotificationDate = DateTimeNow,
                                    CategoryName = string.Empty,
                                    ImageURL = string.Empty,
                                    PushNotification = Body,
                                };
                                jBNDBClass.SavePushNotifications(user.ID, pushNotifications, UserID);
                            }
                            dbcxtransaction.Commit();
                        }
                        return Result;
                    }

                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                Result = "Error!! Please contact administrator";
                return Result;
            }
        }

        // Function to pad an integer number
        // with leading zeros 
        public static string pad_an_int(int N, int P)
        {
            // string used in ToString() method 
            string s = "";
            for (int i = 0; i < P; i++)
            {
                s += "0";
            }

            // use of ToString() method 
            string value = N.ToString(s);

            // return output 
            return value;
        }

        //Send Notifications from Windows Service
        public void SendNotifications()
        {
            try
            {
                using (dbContext = new mwbtDealerEntities())
                {
                    using (var dbcxtransaction = dbContext.Database.BeginTransaction())
                    {
                        var currentDate = DateTime.Now;
                        var adminSetting = dbContext.tblAdminSettings.FirstOrDefault();
                        var pendingAds = (from a in dbContext.tblAdvertisementMains
                                          join c in dbContext.tblCustomerDetails on a.CustID equals c.ID
                                          where !a.IsCancelled && !a.IsApproved && !a.PaymentStatus && !a.IsRejected
                                          select new AdvertisementMain
                                          {
                                              AdvertisementName = a.AdvertisementName,
                                              BookingExpiryDate = a.BookingExpiryDate,
                                              IsPaymentPaid = a.PaymentStatus,
                                              IsApproved = a.IsApproved,
                                              ContentApprovedDate = a.ContentApprovedDate,
                                              DeviceID = c.DeviceID,
                                              MobileNumber = c.MobileNumber,
                                              FirmName = c.FirmName
                                          }).ToList();
                        var contentPendingAds = pendingAds.Where(a => !a.IsApproved.Value && !a.IsPaymentPaid.Value && a.BookingExpiryDate.HasValue && a.BookingExpiryDate > currentDate).ToList();
                        var paymentPendingAds = pendingAds.Where(a => a.IsApproved.HasValue && a.IsApproved.Value && !a.IsPaymentPaid.HasValue && !a.IsPaymentPaid.Value).ToList();

                        //Send Notifications for Content Upload
                        foreach(var contentPendingAd in contentPendingAds)
                        {
                            StringBuilder bodySb = new StringBuilder();
                            bodySb.Append("Dear ");
                            bodySb.Append(contentPendingAd.FirmName);
                            bodySb.Append(",");
                            bodySb.Append("Your advertisement - ");
                            bodySb.Append(contentPendingAd.AdvertisementName);
                            bodySb.Append(" is expiring on ");
                            bodySb.Append(contentPendingAd.BookingExpiryDate);
                            bodySb.Append(".");
                            bodySb.Append("Please upload content before ");
                            bodySb.Append(contentPendingAd.BookingExpiryDate);
                            bodySb.Append(" to avoid rejection of your advertisement.");
                            bodySb.Append("Thank You");
                            Notification notification = new Notification
                            {
                                Body = bodySb.ToString(),
                                CategoryName = string.Empty,
                                Image = string.Empty,
                                NotificationDate = currentDate,
                                Title = "Action required!!! Your Advertisement "+ contentPendingAd.AdvertisementName +" is Expiring soon",
                            };
                            PushNotifications pushNotification = new PushNotifications
                            {
                                PushNotification = notification.Body,
                                CategoryName = notification.CategoryName,
                                NotificationDate = notification.NotificationDate,
                                Title = notification.Title,
                                NotificationDateStr = currentDate.ToString(),
                            };
                            Helper.SendNotification(contentPendingAd.DeviceID, notification);
                            jBNDBClass.SavePushNotifications(contentPendingAd.CustID.Value, pushNotification, 1);
                        }

                        //Send Notifications for Content Upload
                        foreach (var paymentPendingAd in paymentPendingAds)
                        {
                            var paymentExpiryDate = paymentPendingAd.ContentApprovedDate.Value.AddHours(Convert.ToDouble(adminSetting.SetPaymentDueinHrs));
                            if(paymentExpiryDate > currentDate)
                            {
                                StringBuilder bodySb = new StringBuilder();
                                bodySb.Append("Dear ");
                                bodySb.Append(paymentPendingAd.FirmName);
                                bodySb.Append(",");
                                bodySb.Append(" Content approved for ");
                                bodySb.Append("Your advertisement - ");
                                bodySb.Append(paymentPendingAd.AdvertisementName);
                                bodySb.Append(". ");
                                bodySb.Append("Please make payment before ");
                                bodySb.Append(paymentExpiryDate);
                                bodySb.Append(" to avoid rejection of your advertisement.");
                                bodySb.Append("Thank You");
                                Notification notification = new Notification
                                {
                                    Body = bodySb.ToString(),
                                    CategoryName = string.Empty,
                                    Image = string.Empty,
                                    NotificationDate = currentDate,
                                    Title = "Action required!!! Your Advertisement " + paymentPendingAd.AdvertisementName + " is Expiring soon",
                                };
                                PushNotifications pushNotification = new PushNotifications
                                {
                                    PushNotification = notification.Body,
                                    CategoryName = notification.CategoryName,
                                    NotificationDate = notification.NotificationDate,
                                    Title = notification.Title,
                                    NotificationDateStr = currentDate.ToString(),
                                };
                                Helper.SendNotification(paymentPendingAd.DeviceID, notification);
                                jBNDBClass.SavePushNotifications(paymentPendingAd.CustID.Value, pushNotification, 1);
                            }
                            
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
            }
        }
    }
    public class Advertisement
    {
        public int AdvertisementID { get; set; }
        public int AdvertisementMainID { get; set; }
        public string AdvertisementName { get; set; }
        public Nullable<int> CustID { get; set; }
        public Nullable<int> TypeOfAdvertisementID { get; set; }
        public string AdvertisementType { get; set; }
        public Nullable<int> AdTimeSlotID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<int> IntervalsPerHour { get; set; }
        public Nullable<int> IntervalsPerDay { get; set; }
        public Nullable<int> CompletedIntervals { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedDateStr { get; set; }
        public string ExpiryDateStr { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> MdifiedDate { get; set; }
        public Nullable<int> DaysCount { get; set; }
        public List<tblAdvertisementState> advertisementStates { get; set; }
        public List<tblAdvertisementDistrict> advertisementDistricts { get; set; }
        public List<tblAdvertisementCity> advertisementCities { get; set; }
        public List<tblAdvertisementTimeSlot> slots { get; set; }
        public string DispayMessage { get; set; }
        public Nullable<int> BrandID { get; set; }
        public string BrandName { get; set; }
        public Nullable<System.TimeSpan> CurrentAdTime { get; set; }
        public Nullable<System.TimeSpan> NextAdTime { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Nullable<int> AdvertisementAreaID { get; set; }
        public string AdvertisementArea { get; set; }
        public double TotalPrice { get; set; }
        public Nullable<double> FinalPrice { get; set; }
        public double TotalDiscountAmount { get; set; }
        public double TotalTaxAmount { get; set; }
        public Nullable<bool> TemporaryBooked { get; set; }
        public Nullable<bool> IsCancelled { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsCompleted { get; set; }
        public string AdImageURL { get; set; }
        public string AdText { get; set; }
        public Nullable<System.TimeSpan> AdStartTime { get; set; }
        public List<BusinessTypes> businessTypes { get; set; }
        public string FirmName { get; set; }
        public CustomerInfo customerInfo { get; set; }
        public bool IsCompanyAd { get; set; }

    }
    public class AdvertisementMain
    {
        public int AdvertisementMainID { get; set; }
        public string AdvertisementName { get; set; }
        public Nullable<int> BrandID { get; set; }
        public string BrandName { get; set; }
        public string FirmName { get; set; }
        public string EmailID { get; set; }
        public string DeviceID { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<int> CustID { get; set; }
        public Nullable<int> TypeOfAdvertisementID { get; set; }
        public string AdvertisementType { get; set; }
        public Nullable<int> AdvertisementAreaID { get; set; }
        public string AdvertisementArea { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedDateStr { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public string FromDateStr { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string ToDateStr { get; set; }
        public Nullable<double> AdTotalPrice { get; set; }
        public Nullable<double> TaxValue { get; set; }
        public Nullable<double> CGSTPer { get; set; }
        public Nullable<double> SGSTPer { get; set; }
        public Nullable<double> IGSTPer { get; set; }
        public Nullable<double> CGSTAmount { get; set; }
        public Nullable<double> SGSTAmount { get; set; }
        public Nullable<double> IGSTAmount { get; set; }
        public Nullable<double> TaxAmount { get; set; }
        public Nullable<double> FinalPrice { get; set; }
        public Nullable<double> TotalDiscount { get; set; }
        public Nullable<bool> TemporaryBooked { get; set; }
        public Nullable<bool> IsCancelled { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<bool> IsPaymentPaid { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> IsCompleted { get; set; }
        public Advertisement advertisement { get; set; }
        public List<Advertisement> advertisementList { get; set; }
        public Nullable<System.DateTime> BookingExpiryDate { get; set; }
        public string BookingExpiryDateStr { get; set; }
        public string InvoiceNumber { get; set; }
        public string ProformaInvoiceNumber { get; set; }
        public double TotalPrice { get; set; }
        public double TotalDiscountAmount { get; set; }
        public IEnumerable<tblAdvertisementState> states { get; set; }
        public IEnumerable<tblAdvertisementDistrict> districts { get; set; }
        public IEnumerable<tblAdvertisementCity> cities { get; set; }
        public IEnumerable<tblAdvertisementTimeSlot> TimeSlots { get; set; }
        public IEnumerable<AdvertisementStates> advertisementStates { get; set; }
        public IEnumerable<AdvertisementDistricts> advertisementDistricts { get; set; }
        public IEnumerable<AdvertisementCities> advertisementCities { get; set; }
        public Nullable<int> TotalIntervals { get; set; }
        public Nullable<int> TotalDays { get; set; }
        public Nullable<int> TotalStates { get; set; }
        public Nullable<int> TotalDistricts { get; set; }
        public Nullable<int> TotalCities { get; set; }
        public Nullable<double> DiscountPer { get; set; }
        public string AdImageURL { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string DispayMessage { get; set; }
        public string ApprovalStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string Remarks { get; set; }
        public bool IsExpired { get; set; }
        public string AdText { get; set; }
        public CustomerInfo customerInfo { get; set; }
        public bool IsRejected { get; set; }
        public double PublicHolidayMatrix { get; set; }
        public double FestivalMatrix { get; set; }
        public double WeekendMatrix { get; set; }
        public Nullable<System.DateTime> ContentApprovedDate { get; set; }
        public List<PaymentDetails> paymentDetails { get; set; }
        public bool IsMakePaymentAllowed { get; set; }
        public string PaymentDueDate { get; set; }
        public Nullable<System.DateTime> PaymentApprovedDate { get; set; }
        public string StatusType { get; set; }
        public List<tblAdTimeSlot> AdTimeSlots { get; set; }
    }
    public class AdvertisementStates
    {
        public int ID { get; set; }
        public Nullable<int> AdvertisementID { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<int> TairTypeOfStateID { get; set; }
        public string TairTypeOfState { get; set; }
        public string StateName { get; set; }
        public Nullable<int> AdvertisementMainID { get; set; }
        public Nullable<double> TairTypeOfStateMatrix { get; set; }

    }
    public class AdvertisementCities
    {
        public int ID { get; set; }
        public Nullable<int> AdvertisementID { get; set; }
        public Nullable<int> StateWithCityID { get; set; }
        public Nullable<int> TairTypeOfCityID { get; set; }
        public string TairTypeOfCity { get; set; }
        public string VillageLocalityName { get; set; }
        public Nullable<int> AdvertisementMainID { get; set; }
        public Nullable<double> TairTypeOfCityMatrix { get; set; }

    }
    public class AdvertisementDistricts
    {
        public int ID { get; set; }
        public Nullable<int> AdvertisementID { get; set; }
        public Nullable<int> DistrictID { get; set; }
        public Nullable<int> TairTypeOfDistrictID { get; set; }
        public string TairTypeOfDistrict { get; set; }
        public string DistrictName { get; set; }
        public Nullable<int> AdvertisementMainID { get; set; }
        public Nullable<double> TairTypeOfDistrictMatrix { get; set; }

    }
    public class Company
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string PinCode { get; set; }
    }
    public class AdvertisementMain_S
    {
        public int AdvertisementMainID { get; set; }
        public string AdvertisementName { get; set; }
        public Nullable<int> CustID { get; set; }
        public string AdvertisementType { get; set; }
        public string ProductName { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string FromDate { get; set; }
        public string FromDateStr { get; set; }
        public string ToDate { get; set; }
        public string ToDateStr { get; set; }
        public string BookingExpiryDateStr { get; set; }
        public string AdImageURL { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string DispayMessage { get; set; }
        public string ApprovalStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string Remarks { get; set; }
        public bool IsExpired { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<System.DateTime> BookingExpiryDate { get; set; }
        public string AdText { get; set; }
        public int TotalEnquiries { get; set; }
        public bool IsRejected { get; set; }
        public Nullable<double> FinalPrice { get; set; }
        public Nullable<double> TotalDiscount { get; set; }
        public Nullable<double> AdTotalPrice { get; set; }
        public Nullable<double> TaxValue { get; set; }
        public Nullable<double> CGSTPer { get; set; }
        public Nullable<double> SGSTPer { get; set; }
        public Nullable<double> IGSTPer { get; set; }
        public Nullable<double> CGSTAmount { get; set; }
        public Nullable<double> SGSTAmount { get; set; }
        public Nullable<double> IGSTAmount { get; set; }
        public Nullable<double> TaxAmount { get; set; }
        public Nullable<System.DateTime> ContentApprovedDate { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public bool IsMakePaymentAllowed { get; set; }
        public string PaymentDueDate { get; set; }
        public Nullable<System.DateTime> PaymentApprovedDate { get; set; }
    }
    public class CustomerInfo
    {
        public int CustID { get; set; }
        public string FirmName { get; set; }
        public string EmailID { get; set; }
        public string DeviceID { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<int> DistrictID { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
    }
    public class SearchOptions
    {
        public int CustID { get; set; }
        public Nullable<int> TypeOfAdvertisementID { get; set; }
        public string FirmName { get; set; }
        public string EmailID { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<int> DistrictID { get; set; }
        public string AdDate { get; set; }
        public int? AdvertisementAreaID { get; set; }
    }
    public class PaymentDetails
    {
        public int ID { get; set; }
        public string TxnID { get; set; }
        public string TxnStatusCode { get; set; }
        public string OrderID { get; set; }
        public Nullable<double> OrderAmount { get; set; }
        public string TxnStatusMessage { get; set; }
        public string PreAuthID { get; set; }
        public string TxnStatus { get; set; }
        public int AdvertisementMainID { get; set; }
        public Nullable<int> CustID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string DisplayMessage { get; set; }
        public string txnPaymentMode { get; set; }
        public string txnSignature { get; set; }
        public string txnType { get; set; }
        public string txnTime { get; set; }
        public string txnReferenceID { get; set; }
    }
    public class BillingRptVM
    {
        public int AdvertisementMainID { get; set; }
        public string Year { get; set; }
        public string FirmName { get; set; }
        public string AdvertisementName { get; set; }
        public DateTime Date { get; set; }
        public string Month { get; set; }
        public double Sale { get; set; }
        public double TaxAmt { get; set; }
        public double ReceivedAmt { get; set; }
        public double OutstandingAmt { get; set; }
        public int MonthID { get; set; }
    }
    public class MonthData
    {
        public string MonthName { get; set; }
        public int ID { get; set; }
        public int Year { get; set; }
        public bool IsSelect { get; set; }
    }
}

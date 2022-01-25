using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace JBNClassLibrary
{
    public class DLEnquiries
    {
        public class EnquiriesDL
        {
            public int QueryID { get; set; }
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public string FromTime { get; set; }
            public string ToTime { get; set; }
            public string CityName { get; set; }
            public string EnquiryCityName { get; set; }
            public int CityID { get; set; }
            public int StateID { get; set; }
            public string StateName { get; set; }
            public int ProductID { get; set; }
            public int? BusinessTypeID { get; set; }
            public string CustomerName { get; set; }
            public string Address { get; set; }
            public string ContactPersonName { get; set; }
            public int CustID { get; set; }
            public string MobileNumber { get; set; }
            public string EmailID { get; set; }
            public string ProductName { get; set; }
            public string BusinessDemand { get; set; }
            public int? BusinessDemandID { get; set; }
            public string PurposeBusiness { get; set; }
            public int? ProfessionalRequirementID { get; set; }
            public string RequirementName { get; set; }
            public string EnquiryDate { get; set; }
            public int[] StateList { get; set; }
            public int[] CityList { get; set; }
            public int[] ItemCategoryList { get; set; }
            public int[] BusinessTypeList { get; set; }
            public int[] CustomerList { get; set; }
            public List<BusinessTypes> businessTypes { get; set; }
            public bool IsChecked { get; set; }
            public Nullable<bool> InterstCity { get; set; }
            public Nullable<bool> InterstState { get; set; }
            public Nullable<bool> InterstCountry { get; set; }
            public string StrInterstCity { get; set; }
            public string StrInterstState { get; set; }
            public string StrInterstCountry { get; set; }
            public DateTime LastConversationDate { get; set; }
            public string SRType { get; set; }
            public string EnquiryType { get; set; }
            public string IsAdvertisement { get; set; }
            public int ReplyCount { get; set; }
            public string TransactionType { get; set; }
        }
        public class CustomerEnquiries
        {
            public int QueryID { get; set; }
            public int CustID { get; set; }
            public int SenderID { get; set; }
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public string EnquiryCityName { get; set; }
            public int EnquiryCityID { get; set; }
            public string SenderCityName { get; set; }
            public int SenderCityID { get; set; }
            public int EnquiryStateID { get; set; }
            public string EnquiryStateName { get; set; }
            public int SenderStateID { get; set; }
            public string SenderStateName { get; set; }
            public int ProductID { get; set; }
            public int? BusinessTypeID { get; set; }
            public string CustomerName { get; set; }
            public string SenderName { get; set; }
            public string ProductName { get; set; }
            public string BusinessDemand { get; set; }
            public int? BusinessDemandID { get; set; }
            public int? ProfessionalRequirementID { get; set; }
            public string RequirementName { get; set; }
            public string PurposeBusiness { get; set; }
            public string EnquiryDate { get; set; }
            public List<BusinessTypes> senderBusinessTypes { get; set; }
            public int[] EnquiryStateList { get; set; }
            public int[] EnquiryCityList { get; set; }
            public int[] SenderStateList { get; set; }
            public int[] SenderCityList { get; set; }
            public int[] BusinessTypeList { get; set; }
            public string EnquiryType { get; set; }
            public int ReplyCount { get; set; }

        }
        public class CustomerConversations
        {
            public int QueryId { get; set; }
            public int? CustID { get; set; }
            public string FirmName { get; set; }
            public int? SenderID { get; set; }
            public string SenderFirmName { get; set; }
            public string ProductName { get; set; }
            public string EnquiryCity { get; set; }
            public string EnquiryState { get; set; }
            public string MobileNumber { get; set; }
            public string VillageLocalityname { get; set; }
            public string BusinessDemand { get; set; }
            public string PurposeBusiness { get; set; }
            public string Requirements { get; set; }
            public string Image { get; set; }
            public string Image2 { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public List<MessageList> MessageList { get; set; }
            public int IsSender { get; set; }
            public string SenderProfileImg { get; set; }
            public string ReceiverProfileImg { get; set; }

        }
        public class EnquiryListWithTotals
        {
            public bool IsEmail { get; set; }
            public bool IsSMS { get; set; }
            public bool IsWhatsApp { get; set; }
            public bool IsNotification { get; set; }
            [Required(ErrorMessage = "Enter SMS body")]
            public string SMSBody { get; set; }
            [AllowHtml]
            [Required(ErrorMessage = "Enter your message")]
            public string MailBody { get; set; }
            [Required(ErrorMessage = "Enter Mail Subject")]
            public string MailSubject { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
            public int TotalEnquiries { get; set; }
            public int TotalItems { get; set; }
            public int TotalReplies { get; set; }
            public int TotalStates { get; set; }
            public int TotalCities { get; set; }
            public List<EnquiriesDL> enquiryList { get; set; }
            public int SMSTemplateID { get; set; }
        }
        public class EnquiryTypes
        {
            public string EnquiryType { get; set; }
        }

        public EnquiryListWithTotals GetEnquiries(EnquiriesDL enquiries)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    EnquiryListWithTotals enquiryListWithTotals = new EnquiryListWithTotals();
                    List<EnquiriesDL> EnquiryList = new List<EnquiriesDL>();
                    IEnumerable<EnquiriesDL> SentEnquiryList = new List<EnquiriesDL>();
                    IEnumerable<EnquiriesDL> ReceivedEnquiryList = new List<EnquiriesDL>();

                    DateTime? FromDate = null;
                    DateTime? ToDate = null;

                    if (!string.IsNullOrEmpty(enquiries.FromDate) && !string.IsNullOrEmpty(enquiries.ToDate))
                    {
                        FromDate = DateTime.ParseExact(enquiries.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;
                        ToDate = DateTime.ParseExact(enquiries.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;
                    }

                    #region Sent Enquiries

                    SentEnquiryList = (from e in dbContext.tblselectedDealers
                                       join c in dbContext.tblCustomerDetails on e.CustID equals c.ID
                                       join ct in dbContext.tblStateWithCities on e.CityId equals ct.ID
                                       join st in dbContext.tblStates on ct.StateID equals st.ID
                                       join ic in dbContext.tblItemCategories on e.ProductID equals ic.ID
                                       where (enquiries.EnquiryType == null ? 1 == 1 : e.EnquiryType.ToLower() == enquiries.EnquiryType.ToLower())
                                       && (FromDate == null || DbFunctions.TruncateTime(e.CreatedDate) >= FromDate)
                                       && (ToDate == null || DbFunctions.TruncateTime(e.CreatedDate) <= ToDate)
                                       select new EnquiriesDL
                                       {
                                           QueryID = e.ID,
                                           CustID = c.ID,
                                           CustomerName = c.FirmName,
                                           CityID = e.CityId.Value,
                                           StateID = st.StateID,
                                           CityName = ct.VillageLocalityName,
                                           FromDate = e.CreatedDate.ToString(),
                                           StateName = st.StateName,
                                           BusinessDemand = dbContext.tblBusinessDemands.Where(b => b.ID == e.BusinessDemandID).FirstOrDefault().Demand,
                                           BusinessDemandID = dbContext.tblBusinessDemands.Where(b => b.ID == e.BusinessDemandID).FirstOrDefault().ID,
                                           PurposeBusiness = e.PurposeBusiness,
                                           ProductID = e.ProductID.Value,
                                           ProductName = ic.ItemName, // sc.ItemName,
                                           ProfessionalRequirementID = dbContext.tblProfessionalRequirements.Where(b => b.ID == e.ProfessionalRequirementID).FirstOrDefault().ID,
                                           RequirementName = dbContext.tblProfessionalRequirements.Where(b => b.ID == e.ProfessionalRequirementID).FirstOrDefault().RequirementName,
                                           SRType = "Sent",
                                           EnquiryType = e.EnquiryType,
                                           ReplyCount = e.tblUserConversations.Where(cc => cc.CustID != e.CustID).Select(uc => uc.CustID).Distinct().Count()
                                       }).Distinct().AsEnumerable();

                    #endregion

                    #region Recieved Enquiries

                    ReceivedEnquiryList = (from sdd in dbContext.tblselectedDealerDetails
                                           join sd in dbContext.tblselectedDealers on sdd.QueryId equals sd.ID
                                           join cd in dbContext.tblCustomerDetails on sdd.CustID equals cd.ID
                                           join sc in dbContext.tblStateWithCities on sd.CityId equals sc.ID
                                           join st in dbContext.tblStates on cd.State equals st.ID
                                           join ic in dbContext.tblItemCategories on sd.ProductID equals ic.ID
                                           where sdd.CustID != sd.CustID && sdd.CreatedBy != sdd.CustID
                                           && (enquiries.EnquiryType == null ? 1 == 1 : sd.EnquiryType.ToLower() == enquiries.EnquiryType.ToLower())
                                           && (FromDate == null || DbFunctions.TruncateTime(sd.CreatedDate) >= FromDate)
                                           && (ToDate == null || DbFunctions.TruncateTime(sd.CreatedDate) <= ToDate)
                                           select new EnquiriesDL
                                           {
                                               QueryID = sdd.QueryId.Value,
                                               CustID = cd.ID,
                                               CustomerName = cd.FirmName,
                                               CityID = sd.CityId.Value,
                                               StateID = st.StateID,
                                               CityName = sc.VillageLocalityName,
                                               FromDate = sd.CreatedDate.ToString(),
                                               StateName = st.StateName,
                                               BusinessDemand = dbContext.tblBusinessDemands.Where(b => b.ID == sd.BusinessDemandID).FirstOrDefault().Demand,
                                               BusinessDemandID = dbContext.tblBusinessDemands.Where(b => b.ID == sd.BusinessDemandID).FirstOrDefault().ID,
                                               PurposeBusiness = sd.PurposeBusiness,
                                               ProductID = sd.ProductID.Value,
                                               ProductName = ic.ItemName,//cc.ItemName,
                                               ProfessionalRequirementID = dbContext.tblProfessionalRequirements.Where(b => b.ID == sd.ProfessionalRequirementID).FirstOrDefault().ID,
                                               RequirementName = dbContext.tblProfessionalRequirements.Where(b => b.ID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName,
                                               SRType = "Received",
                                               EnquiryType = sd.EnquiryType
                                           }).Distinct().AsEnumerable();

                    #endregion
                    EnquiryList.AddRange(ReceivedEnquiryList);
                    EnquiryList.AddRange(SentEnquiryList);

                    EnquiryList = (from e in EnquiryList
                                   select new EnquiriesDL
                                   {
                                       QueryID = e.QueryID,
                                       CustID = e.CustID,
                                       CustomerName = e.CustomerName,
                                       CityID = e.CityID,
                                       StateID = e.StateID,
                                       CityName = e.CityName,
                                       FromDate = e.FromDate,
                                       StateName = e.StateName,
                                       BusinessDemand = e.BusinessDemand,
                                       BusinessDemandID = e.BusinessDemandID,
                                       PurposeBusiness = e.PurposeBusiness,
                                       ProductID = e.ProductID,
                                       ProductName = e.ProductName,
                                       ProfessionalRequirementID = e.ProfessionalRequirementID,
                                       RequirementName = e.RequirementName,
                                       EnquiryType = e.EnquiryType,
                                       SRType = e.SRType,
                                       businessTypes = (from bt in dbContext.tblSelectedDealerBusinessTypes
                                                        join btc in dbContext.tblBusinessTypes on bt.BusinessTypeID.Value equals btc.ID
                                                        where bt.QueryID == e.QueryID
                                                        select new BusinessTypes
                                                        {
                                                            BusinessTypeID = bt.BusinessTypeID.Value,
                                                            BusinessTypeName = btc.Type
                                                        }).ToList(),
                                       ReplyCount = e.ReplyCount,
                                   }).ToList();

                    if (enquiries.CustomerList != null && enquiries.CustomerList.Count() > 0)
                    {
                        EnquiryList = EnquiryList.Where(m => enquiries.CustomerList.Contains(m.CustID)).ToList();
                    }

                    List<EnquiriesDL> FilteredEnquiryList = new List<EnquiriesDL>();

                    if (enquiries.BusinessTypeList != null && enquiries.BusinessTypeList.Count() > 0)
                    {
                        foreach (int btID in enquiries.BusinessTypeList)
                        {
                            FilteredEnquiryList.AddRange(EnquiryList.Where(e => e.businessTypes.Any(b => b.BusinessTypeID == btID)).ToList());
                        }
                    }
                    else
                    {
                        FilteredEnquiryList = EnquiryList;
                    }

                    EnquiryList = FilteredEnquiryList;

                    if (!string.IsNullOrEmpty(enquiries.PurposeBusiness))
                        EnquiryList = EnquiryList.Where(e => e.PurposeBusiness.ToLower() == enquiries.PurposeBusiness.ToLower()).ToList();

                    if (enquiries.BusinessDemandID != null && enquiries.BusinessDemandID != 0)
                        EnquiryList = EnquiryList.Where(e => e.BusinessDemandID == enquiries.BusinessDemandID).ToList();

                    //if (!string.IsNullOrEmpty(enquiries.FromDate) && !string.IsNullOrEmpty(enquiries.ToDate))
                    //{
                    //    DateTime FromDate = DateTime.ParseExact(enquiries.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //    DateTime ToDate = DateTime.ParseExact(enquiries.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    //    EnquiryList = EnquiryList.Where(i => Convert.ToDateTime(i.FromDate).Date >= FromDate.Date && Convert.ToDateTime(i.FromDate).Date <= ToDate.Date).ToList();

                    //    if (!string.IsNullOrEmpty(enquiries.FromTime) && !string.IsNullOrEmpty(enquiries.ToTime))
                    //    {
                    //        DateTime fromTime = DateTime.ParseExact(enquiries.FromTime,
                    //                "hh:mm tt", CultureInfo.InvariantCulture);
                    //        TimeSpan Fromspan = fromTime.TimeOfDay;
                    //        DateTime toTime = DateTime.ParseExact(enquiries.ToTime,
                    //                "hh:mm tt", CultureInfo.InvariantCulture);
                    //        TimeSpan Tospan = toTime.TimeOfDay;

                    //        EnquiryList = EnquiryList.Where(c => Convert.ToDateTime(c.FromDate).TimeOfDay >= Fromspan && Convert.ToDateTime(c.FromDate).TimeOfDay <= Tospan).ToList();
                    //    }
                    //}

                    //if (enquiries.BusinessTypeList != null && enquiries.BusinessTypeList.Count() > 0)
                    //    EnquiryList = EnquiryList.Where(i => i.businessTypes.Exists(enquiries.BusinessTypeList))).ToList();

                    //if (enquiries.StateID != 0)
                    //    EnquiryList = EnquiryList.Where(i => i.StateID == enquiries.StateID).ToList();
                    //if (enquiries.CityID != 0)
                    //    EnquiryList = EnquiryList.Where(i => i.CityID == enquiries.CityID).ToList();

                    if (!string.IsNullOrEmpty(enquiries.FromTime) && !string.IsNullOrEmpty(enquiries.ToTime))
                    {
                        DateTime fromTime = DateTime.ParseExact(enquiries.FromTime,
                                "hh:mm tt", CultureInfo.InvariantCulture);
                        TimeSpan Fromspan = fromTime.TimeOfDay;
                        DateTime toTime = DateTime.ParseExact(enquiries.ToTime,
                                "hh:mm tt", CultureInfo.InvariantCulture);
                        TimeSpan Tospan = toTime.TimeOfDay;

                        EnquiryList = EnquiryList.Where(c => Convert.ToDateTime(c.FromDate).TimeOfDay >= Fromspan && Convert.ToDateTime(c.FromDate).TimeOfDay <= Tospan).ToList();
                    }

                    if (enquiries.StateList != null && enquiries.StateList.Count() > 0)
                    {
                        EnquiryList = EnquiryList.Where(m => enquiries.StateList.Contains(m.StateID)).ToList();
                    }
                    if (enquiries.CityList != null && enquiries.CityList.Count() > 0)
                    {
                        EnquiryList = EnquiryList.Where(m => enquiries.CityList.Contains(m.CityID)).ToList();
                    }
                    if (enquiries.ItemCategoryList != null && enquiries.ItemCategoryList.Count() > 0)
                    {
                        EnquiryList = EnquiryList.Where(m => enquiries.ItemCategoryList.Contains(m.ProductID)).ToList();
                    }

                    //if (enquiries.ProductID != 0)
                    //    EnquiryList = EnquiryList.Where(i => i.ProductID == enquiries.ProductID).ToList();

                    enquiryListWithTotals.TotalEnquiries = SentEnquiryList.Count();
                    enquiryListWithTotals.TotalCities = EnquiryList.Select(e => e.CityID).Distinct().Count();
                    enquiryListWithTotals.TotalStates = EnquiryList.Select(e => e.StateID).Distinct().Count();
                    enquiryListWithTotals.TotalItems = EnquiryList.Select(e => e.ProductID).Distinct().Count();
                    enquiryListWithTotals.TotalReplies = dbContext.tblUserConversations.Select(d => d.QueryId.Value).Distinct().Count();
                    enquiryListWithTotals.enquiryList = EnquiryList.OrderByDescending(e => e.QueryID).ToList();
                    return enquiryListWithTotals;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<ItemCategory> GetItemCategories()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<ItemCategory> itemCategories = new List<ItemCategory>();

                    itemCategories = (from e in dbContext.tblselectedDealers
                                      join cc in dbContext.tblItemCategories on e.ProductID equals cc.ID
                                      select new ItemCategory
                                      {
                                          ID = cc.ID,
                                          ItemName = cc.ItemName
                                      }).Distinct().ToList();

                    return itemCategories;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<CustomerEnquiries> GetCustomerEnquiries(CustomerEnquiries customerEnquiries)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<CustomerEnquiries> EnquiryList = new List<CustomerEnquiries>();

                    if (customerEnquiries.EnquiryType.ToLower() == "sent")
                    {
                        #region Sent Enquiry
                        EnquiryList = (from e in dbContext.tblselectedDealers
                                       join sd in dbContext.tblselectedDealerDetails on e.ID equals sd.QueryId
                                       join c in dbContext.tblCustomerDetails on e.CustID equals c.ID
                                       join ct in dbContext.tblStateWithCities on e.CityId equals ct.ID
                                       join st in dbContext.tblStates on ct.StateID equals st.ID
                                       where e.ID == customerEnquiries.QueryID && e.CustID == customerEnquiries.CustID
                                       && sd.CustID != customerEnquiries.CustID
                                       select new CustomerEnquiries
                                       {
                                           QueryID = e.ID,
                                           CustID = c.ID,
                                           CustomerName = c.FirmName,
                                           EnquiryCityID = e.CityId.Value,
                                           EnquiryStateID = st.StateID,
                                           EnquiryCityName = ct.VillageLocalityName,
                                           EnquiryStateName = st.StateName,
                                           EnquiryDate = e.CreatedDate.ToString(),
                                           BusinessDemand = dbContext.tblBusinessDemands.Where(b => b.ID == e.BusinessDemandID).FirstOrDefault().Demand,
                                           BusinessDemandID = dbContext.tblBusinessDemands.Where(b => b.ID == e.BusinessDemandID).FirstOrDefault().ID,
                                           ProfessionalRequirementID = dbContext.tblProfessionalRequirements.Where(b => b.ID == e.ProfessionalRequirementID).FirstOrDefault().ID,
                                           RequirementName = dbContext.tblProfessionalRequirements.Where(b => b.ID == e.ProfessionalRequirementID).FirstOrDefault().RequirementName,
                                           PurposeBusiness = e.PurposeBusiness,
                                           SenderID = sd.CustID.Value,
                                           SenderCityName = (from sender in dbContext.tblCustomerDetails
                                                             join sdd in dbContext.tblselectedDealerDetails on sender.ID equals sdd.CustID
                                                             join sc in dbContext.tblStateWithCities on sender.City equals sc.ID
                                                             select sc.VillageLocalityName).FirstOrDefault(),
                                           SenderCityID = (from sender in dbContext.tblCustomerDetails
                                                           join sdd in dbContext.tblselectedDealerDetails on sender.ID equals sdd.CustID
                                                           join sc in dbContext.tblStateWithCities on sender.City equals sc.ID
                                                           select sc.ID).FirstOrDefault(),
                                           SenderStateName = (from sender in dbContext.tblCustomerDetails
                                                              join sdd in dbContext.tblselectedDealerDetails on sender.ID equals sdd.CustID
                                                              join sc in dbContext.tblStates on sender.State equals sc.ID
                                                              select sc.StateName).FirstOrDefault(),
                                           SenderStateID = (from sender in dbContext.tblCustomerDetails
                                                            join sdd in dbContext.tblselectedDealerDetails on sender.ID equals sdd.CustID
                                                            join sc in dbContext.tblStates on sender.State equals sc.ID
                                                            select sc.StateID).FirstOrDefault(),
                                           SenderName = dbContext.tblCustomerDetails.Where(x => x.ID == sd.CustID).FirstOrDefault().FirmName,
                                           senderBusinessTypes = (from bt in dbContext.tblBusinessTypes
                                                                  join btc in dbContext.tblBusinessTypewithCusts on bt.ID equals btc.BusinessTypeID
                                                                  join cu in dbContext.tblCustomerDetails on btc.CustID equals cu.ID
                                                                  where cu.ID == customerEnquiries.CustID
                                                                  select new BusinessTypes
                                                                  {
                                                                      BusinessTypeID = bt.ID,
                                                                      BusinessTypeName = bt.Type,
                                                                  }).ToList(),
                                           EnquiryType = "sent",
                                           ReplyCount = e.tblUserConversations.Where(cc => cc.CustID != e.CustID && cc.CustID == sd.CustID).Select(uc => uc.CustID).Distinct().Count()
                                       }).ToList();
                        #endregion
                    }
                    else
                    {
                        #region Received Enquiry
                        EnquiryList = (from e in dbContext.tblselectedDealers
                                       join sd in dbContext.tblselectedDealerDetails on e.ID equals sd.QueryId
                                       join c in dbContext.tblCustomerDetails on sd.CustID equals c.ID
                                       join ct in dbContext.tblStateWithCities on e.CityId equals ct.ID
                                       join st in dbContext.tblStates on ct.StateID equals st.ID
                                       where sd.QueryId == customerEnquiries.QueryID && sd.CustID == customerEnquiries.CustID
                                       && sd.CustID != customerEnquiries.CustID
                                       select new CustomerEnquiries
                                       {
                                           QueryID = e.ID,
                                           CustID = c.ID,
                                           CustomerName = c.FirmName,
                                           EnquiryCityID = e.CityId.Value,
                                           EnquiryStateID = st.StateID,
                                           EnquiryCityName = ct.VillageLocalityName,
                                           EnquiryStateName = st.StateName,
                                           EnquiryDate = e.CreatedDate.ToString(),
                                           BusinessDemand = dbContext.tblBusinessDemands.Where(b => b.ID == e.BusinessDemandID).FirstOrDefault().Demand,
                                           BusinessDemandID = dbContext.tblBusinessDemands.Where(b => b.ID == e.BusinessDemandID).FirstOrDefault().ID,
                                           ProfessionalRequirementID = dbContext.tblProfessionalRequirements.Where(b => b.ID == e.ProfessionalRequirementID).FirstOrDefault().ID,
                                           RequirementName = dbContext.tblProfessionalRequirements.Where(b => b.ID == e.ProfessionalRequirementID).FirstOrDefault().RequirementName,
                                           PurposeBusiness = e.PurposeBusiness,
                                           SenderID = e.CustID,
                                           SenderCityName = (from sender in dbContext.tblCustomerDetails
                                                             join sdd in dbContext.tblselectedDealers on sender.ID equals sdd.CustID
                                                             join sc in dbContext.tblStateWithCities on sender.City equals sc.ID
                                                             select sc.VillageLocalityName).FirstOrDefault(),
                                           SenderCityID = (from sender in dbContext.tblCustomerDetails
                                                           join sdd in dbContext.tblselectedDealers on sender.ID equals sdd.CustID
                                                           join sc in dbContext.tblStateWithCities on sender.City equals sc.ID
                                                           select sc.ID).FirstOrDefault(),
                                           SenderStateName = (from sender in dbContext.tblCustomerDetails
                                                              join sdd in dbContext.tblselectedDealers on sender.ID equals sdd.CustID
                                                              join sc in dbContext.tblStates on sender.State equals sc.ID
                                                              select sc.StateName).FirstOrDefault(),
                                           SenderStateID = (from sender in dbContext.tblCustomerDetails
                                                            join sdd in dbContext.tblselectedDealers on sender.ID equals sdd.CustID
                                                            join sc in dbContext.tblStates on sender.State equals sc.ID
                                                            select sc.StateID).FirstOrDefault(),
                                           SenderName = dbContext.tblCustomerDetails.Where(x => x.ID == e.CustID).FirstOrDefault().FirmName,
                                           senderBusinessTypes = (from bt in dbContext.tblBusinessTypes
                                                                  join btc in dbContext.tblBusinessTypewithCusts on bt.ID equals btc.BusinessTypeID
                                                                  join cu in dbContext.tblCustomerDetails on btc.CustID equals cu.ID
                                                                  where cu.ID == customerEnquiries.CustID
                                                                  select new BusinessTypes
                                                                  {
                                                                      BusinessTypeID = bt.ID,
                                                                      BusinessTypeName = bt.Type,
                                                                  }).ToList(),
                                           EnquiryType = "received",

                                       }).ToList();
                        #endregion
                    }

                    if (!string.IsNullOrEmpty(customerEnquiries.PurposeBusiness))
                        EnquiryList = EnquiryList.Where(e => e.PurposeBusiness.ToLower() == customerEnquiries.PurposeBusiness.ToLower()).ToList();

                    if (customerEnquiries.BusinessDemandID != null && customerEnquiries.BusinessDemandID != 0)
                        EnquiryList = EnquiryList.Where(e => e.BusinessDemandID == customerEnquiries.BusinessDemandID).ToList();

                    if (!string.IsNullOrEmpty(customerEnquiries.FromDate) && !string.IsNullOrEmpty(customerEnquiries.ToDate))
                    {
                        DateTime FromDate = DateTime.ParseExact(customerEnquiries.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime ToDate = DateTime.ParseExact(customerEnquiries.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        EnquiryList = EnquiryList.Where(i => Convert.ToDateTime(i.FromDate).Date >= FromDate.Date && Convert.ToDateTime(i.FromDate).Date <= ToDate.Date).ToList();
                    }

                    if (customerEnquiries.BusinessTypeList != null && customerEnquiries.BusinessTypeList.Count() > 0)
                        EnquiryList = EnquiryList.Where(i => i.senderBusinessTypes.All(sb => customerEnquiries.BusinessTypeList.Contains(int.Parse(sb.BusinessTypeID.ToString())))).ToList();

                    if (customerEnquiries.SenderStateList != null && customerEnquiries.SenderStateList.Count() > 0)
                        EnquiryList = EnquiryList.Where(m => customerEnquiries.SenderStateList.Contains(m.SenderStateID)).ToList();

                    if (customerEnquiries.SenderCityList != null && customerEnquiries.SenderCityList.Count() > 0)
                        EnquiryList = EnquiryList.Where(m => customerEnquiries.SenderCityList.Contains(m.SenderCityID)).ToList();

                    if (customerEnquiries.EnquiryStateList != null && customerEnquiries.EnquiryStateList.Count() > 0)
                        EnquiryList = EnquiryList.Where(m => customerEnquiries.EnquiryStateList.Contains(m.EnquiryStateID)).ToList();

                    if (customerEnquiries.EnquiryCityList != null && customerEnquiries.EnquiryCityList.Count() > 0)
                        EnquiryList = EnquiryList.Where(m => customerEnquiries.EnquiryCityList.Contains(m.EnquiryCityID)).ToList();

                    return EnquiryList;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        public List<City> GetEnquiryCities()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<City> cities = new List<City>();

                    cities = (from sd in dbContext.tblselectedDealers
                              join ct in dbContext.tblStateWithCities on sd.CityId equals ct.ID
                              select new City
                              {
                                  StateWithCityID = ct.ID,
                                  VillageLocalityName = ct.VillageLocalityName
                              }).Distinct().ToList();
                    return cities;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Conversations
        public CustomerConversations GetConversations(int CustID, int QueryId, int SenderID, string EnquiryType)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<MessageList> messageList = new List<MessageList>();
                    List<tblDeleteChat> deleteChatList = dbContext.tblDeleteChats.Where(d => d.CustID == CustID && d.QueryId == QueryId).ToList();

                    //messageList.ForEach(m => m.Message = HttpUtility.HtmlDecode(m.Message));

                    //messageList.ForEach(m => m.Image == null ? "" : m.Image = "http://151.106.34.23/mwbgroups/jbnproduction" + m.Image);

                    string WebsiteURL = ConfigurationManager.AppSettings["WebsiteURL"].ToString();
                    string PortalURL = ConfigurationManager.AppSettings["PortalURL"].ToString();
                    WebsiteURL = WebsiteURL + "/MWBImages";
                    CustomerConversations conversations = new CustomerConversations();

                    if (EnquiryType.ToLower() == "sent")
                    {
                        #region Sent Enquiry
                        conversations = (from sd in dbContext.tblselectedDealers
                                         join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                         join cd in dbContext.tblCustomerDetails on sd.CustID equals cd.ID
                                         join sc in dbContext.tblStateWithCities on cd.City equals sc.ID
                                         join ch in dbContext.tblItemCategories on sd.ProductID equals ch.ID
                                         join st in dbContext.tblStates on sc.StateID equals st.ID
                                         where sd.CustID == CustID && sd.ID == QueryId && sdd.CustID == SenderID
                                         select new CustomerConversations
                                         {
                                             QueryId = sd.ID,
                                             CustID = sd.CustID,
                                             FirmName = cd.FirmName,
                                             SenderFirmName = (from el in dbContext.tblselectedDealerDetails
                                                               join cc in dbContext.tblCustomerDetails on el.CustID equals cc.ID
                                                               where cc.ID == SenderID
                                                               select cc.FirmName).FirstOrDefault(),
                                             EnquiryState = st.StateName,
                                             MobileNumber = cd.MobileNumber,
                                             VillageLocalityname = sc.VillageLocalityName,
                                             PurposeBusiness = sd.PurposeBusiness,
                                             Requirements = sd.OpenText,
                                             Image = sd.Image,
                                             Image2 = sd.Image1,
                                             CreatedDate = sd.CreatedDate,
                                             IsSender = (sd.CustID == CustID ? 1 : 0),
                                             ProductName = ch.ItemName,
                                             SenderProfileImg = cd.UserImage,
                                             ReceiverProfileImg = (from el in dbContext.tblselectedDealerDetails
                                                                   join cc in dbContext.tblCustomerDetails on el.CustID equals cc.ID
                                                                   where cc.ID == SenderID
                                                                   select cc.UserImage).FirstOrDefault(),
                                         }).FirstOrDefault();

                        MessageList messageList1 = new MessageList();
                        messageList1.CustID = conversations.CustID;
                        messageList1.QueryId = conversations.QueryId;
                        messageList1.Message = conversations.Requirements;
                        messageList1.IsDealer = conversations.CustID == CustID ? 0 : 1;
                        messageList1.CreatedDate = conversations.CreatedDate;
                        messageList1.CustomerName = conversations.FirmName;
                        messageList1.SenderProfileImg = conversations.SenderProfileImg;
                        messageList1.ReceiverProfileImg = conversations.ReceiverProfileImg;
                        messageList.Add(messageList1);

                        if (!string.IsNullOrEmpty(conversations.Image))
                        {
                            MessageList messageList2 = new MessageList();
                            messageList2.CustID = conversations.CustID;
                            messageList2.QueryId = conversations.QueryId;
                            messageList2.IsDealer = conversations.CustID == CustID ? 0 : 1;
                            messageList2.Image = conversations.Image;
                            messageList2.CreatedDate = conversations.CreatedDate;
                            messageList2.CustomerName = conversations.FirmName;
                            messageList2.SenderProfileImg = conversations.SenderProfileImg;
                            messageList2.ReceiverProfileImg = conversations.ReceiverProfileImg;
                            messageList.Add(messageList2);
                        }
                        if (!string.IsNullOrEmpty(conversations.Image2))
                        {
                            MessageList messageList3 = new MessageList();
                            messageList3.CustID = conversations.CustID;
                            messageList3.QueryId = conversations.QueryId;
                            messageList3.IsDealer = conversations.CustID == CustID ? 0 : 1;
                            messageList3.Image = conversations.Image2;
                            messageList3.CreatedDate = conversations.CreatedDate;
                            messageList3.CustomerName = conversations.FirmName;
                            messageList3.SenderProfileImg = conversations.SenderProfileImg;
                            messageList3.ReceiverProfileImg = conversations.ReceiverProfileImg;
                            messageList.Add(messageList3);
                        }

                        List<MessageList> ConversationList = (from s in dbContext.tblUserConversations
                                                              where s.QueryId == QueryId && (s.CustID == CustID && s.IsDealer == SenderID) || (s.CustID == SenderID && s.IsDealer == CustID)
                                                              select new MessageList
                                                              {
                                                                  CustomerName = dbContext.tblCustomerDetails.Where(c => c.ID == s.CustID).FirstOrDefault().FirmName,
                                                                  ID = s.ID,
                                                                  QueryId = s.QueryId,
                                                                  CustID = s.CustID,
                                                                  Message = s.Message,
                                                                  IsDealer = s.CustID == CustID ? 0 : 1,
                                                                  IsCustomer = s.IsCustomer,
                                                                  Image = s.Image,
                                                                  CreatedDate = s.CreatedDate,
                                                                  IsRead = s.IsRead,
                                                                  IsArchived = s.IsArchived,
                                                                  SenderProfileImg = dbContext.tblCustomerDetails.Where(cs => cs.ID == CustID).FirstOrDefault().UserImage,
                                                                  ReceiverProfileImg = dbContext.tblCustomerDetails.Where(cs => cs.ID == SenderID).FirstOrDefault().UserImage,

                                                              }).ToList().Where(u => u.QueryId == QueryId).ToList();
                        messageList.AddRange(ConversationList);
                        #endregion
                    }
                    else
                    {
                        #region Received Enquiry
                        conversations = (from sd in dbContext.tblselectedDealers
                                         join sdd in dbContext.tblselectedDealerDetails on sd.ID equals sdd.QueryId
                                         join cd in dbContext.tblCustomerDetails on sd.CustID equals cd.ID
                                         join sc in dbContext.tblStateWithCities on cd.City equals sc.ID
                                         join ch in dbContext.tblItemCategories on sd.ProductID equals ch.ID
                                         join st in dbContext.tblStates on sc.StateID equals st.ID
                                         where sdd.CustID == CustID && sdd.QueryId == QueryId && sd.CustID == SenderID
                                         select new CustomerConversations
                                         {
                                             QueryId = sd.ID,
                                             CustID = sd.CustID,
                                             FirmName = cd.FirmName,
                                             SenderFirmName = (from el in dbContext.tblselectedDealerDetails
                                                               join cc in dbContext.tblCustomerDetails on el.CustID equals cc.ID
                                                               where cc.ID == CustID
                                                               select cc.FirmName).FirstOrDefault(),
                                             EnquiryState = st.StateName,
                                             MobileNumber = cd.MobileNumber,
                                             VillageLocalityname = sc.VillageLocalityName,
                                             PurposeBusiness = sd.PurposeBusiness,
                                             Requirements = sd.OpenText,
                                             Image = sd.Image,
                                             Image2 = sd.Image1,
                                             CreatedDate = sd.CreatedDate,
                                             IsSender = (sd.CustID == SenderID ? 1 : 0),
                                             ProductName = ch.ItemName,
                                             SenderProfileImg = WebsiteURL + cd.UserImage,
                                             ReceiverProfileImg = WebsiteURL + (from el in dbContext.tblselectedDealerDetails
                                                                                join cc in dbContext.tblCustomerDetails on el.CustID equals cc.ID
                                                                                where cc.ID == SenderID
                                                                                select cc.UserImage).FirstOrDefault(),
                                         }).FirstOrDefault();

                        MessageList messageList1 = new MessageList();
                        messageList1.CustID = conversations.CustID;
                        messageList1.QueryId = conversations.QueryId;
                        messageList1.Message = conversations.Requirements;
                        messageList1.IsDealer = conversations.CustID == SenderID ? 0 : 1;
                        messageList1.CreatedDate = conversations.CreatedDate;
                        messageList1.SenderProfileImg = conversations.SenderProfileImg;
                        messageList1.ReceiverProfileImg = conversations.ReceiverProfileImg;
                        messageList.Add(messageList1);

                        if (!string.IsNullOrEmpty(conversations.Image))
                        {
                            MessageList messageList2 = new MessageList();
                            messageList2.CustID = conversations.CustID;
                            messageList2.QueryId = conversations.QueryId;
                            messageList2.IsDealer = conversations.CustID == SenderID ? 0 : 1;
                            messageList2.Image = conversations.Image;
                            messageList2.CreatedDate = conversations.CreatedDate;
                            messageList2.SenderProfileImg = conversations.SenderProfileImg;
                            messageList2.ReceiverProfileImg = conversations.ReceiverProfileImg;
                            messageList.Add(messageList2);
                        }
                        if (!string.IsNullOrEmpty(conversations.Image2))
                        {
                            MessageList messageList3 = new MessageList();
                            messageList3.CustID = conversations.CustID;
                            messageList3.QueryId = conversations.QueryId;
                            messageList3.IsDealer = conversations.CustID == SenderID ? 0 : 1;
                            messageList3.Image = conversations.Image2;
                            messageList3.CreatedDate = conversations.CreatedDate;
                            messageList3.SenderProfileImg = conversations.SenderProfileImg;
                            messageList3.ReceiverProfileImg = conversations.ReceiverProfileImg;
                            messageList.Add(messageList3);
                        }

                        List<MessageList> ConversationList = (from s in dbContext.tblUserConversations
                                                              where s.QueryId == QueryId && (s.CustID == CustID && s.IsDealer == SenderID) || (s.CustID == SenderID && s.IsDealer == CustID)
                                                              select new MessageList
                                                              {
                                                                  CustomerName = dbContext.tblCustomerDetails.Where(c => c.ID == s.CustID).FirstOrDefault().FirmName,
                                                                  ID = s.ID,
                                                                  QueryId = s.QueryId,
                                                                  CustID = s.CustID,
                                                                  Message = s.Message,
                                                                  IsDealer = s.CustID == SenderID ? 0 : 1,
                                                                  IsCustomer = s.IsCustomer,
                                                                  Image = s.Image,
                                                                  CreatedDate = s.CreatedDate,
                                                                  IsRead = s.IsRead,
                                                                  IsArchived = s.IsArchived,
                                                                  SenderProfileImg = WebsiteURL + dbContext.tblCustomerDetails.Where(cs => cs.ID == s.CustID).FirstOrDefault().UserImage,
                                                                  ReceiverProfileImg = WebsiteURL + dbContext.tblCustomerDetails.Where(cs => cs.ID == s.IsDealer).FirstOrDefault().UserImage,
                                                              }).ToList().Where(u => u.QueryId == QueryId).ToList();

                        messageList.AddRange(ConversationList);
                        #endregion
                    }

                    if (messageList.Count <= 0)
                    {
                        conversations.MessageList = new List<MessageList>();
                    }
                    else
                    {
                        if (deleteChatList != null)
                        {
                            messageList.RemoveAll(l => deleteChatList.Exists(d => d.QueryId == l.QueryId && (d.CustID == l.CustID || d.CustID == l.IsDealer) && d.ChatID == l.ID));
                        }

                        foreach (var item in messageList)
                        {
                            item.Message = HttpUtility.HtmlDecode(item.Message);
                            if (!string.IsNullOrEmpty(item.Image))
                            {
                                item.Image = WebsiteURL + item.Image;
                                
                            }
                            if (item.SenderProfileImg != null)
                                item.SenderProfileImg = WebsiteURL + item.SenderProfileImg;
                            else
                                item.SenderProfileImg = PortalURL + "/Images/avatar.jpg";
                            if (item.ReceiverProfileImg != null)
                                item.ReceiverProfileImg = WebsiteURL + item.ReceiverProfileImg;
                            else
                                item.ReceiverProfileImg = PortalURL + "/Images/avatar.jpg";
                            //if (!string.IsNullOrEmpty(item.Message))
                            //{
                            //    item.Message = StringHelper.ToHexString(item.Message);
                            //}
                        }
                        conversations.MessageList = messageList;
                    }

                    return conversations;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        public EnquiryListWithTotals GetLaunchpadReport(EnquiriesDL search)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    EnquiryListWithTotals enquiryListWithTotals = new EnquiryListWithTotals();
                    List<EnquiriesDL> EnquiryList = new List<EnquiriesDL>();
                    IEnumerable<EnquiriesDL> customerList = new List<EnquiriesDL>();
                    int totalEnquiries = dbContext.tblselectedDealers.Count();
                    EnquiryList = (from uc in dbContext.tblUserConversations
                                   join sdd in dbContext.tblselectedDealerDetails on new { uc.CustID, uc.QueryId } equals new { sdd.CustID, sdd.QueryId }
                                   join sd in dbContext.tblselectedDealers on uc.QueryId equals sd.ID
                                   join c in dbContext.tblCustomerDetails on uc.CustID equals c.ID
                                   join cc in dbContext.tblItemCategories on sd.ProductID equals cc.ID
                                   join ct in dbContext.tblStateWithCities on c.City equals ct.ID
                                   join st in dbContext.tblStates on c.State equals st.ID
                                   where sd.CustID != uc.CustID && uc.QueryId == sdd.QueryId
                                   select new EnquiriesDL
                                   {
                                       CustID = uc.CustID.Value,
                                       CustomerName = c.FirmName,
                                       MobileNumber = c.MobileNumber,
                                       EmailID = c.EmailID,
                                       Address = c.BillingAddress,
                                       ContactPersonName = c.CustName,
                                       ProductID = sd.ProductID.Value,
                                       CityID = c.City.Value,
                                       StateID = c.State.Value,
                                       CityName = ct.VillageLocalityName,
                                       StateName = st.StateName,
                                       InterstCity = c.InterstCity,
                                       InterstState = c.InterstState,
                                       InterstCountry = c.InterstCountry,
                                       StrInterstCity = c.InterstCity.Value == true ? "Yes" : "No",
                                       StrInterstState = c.InterstState.Value == true ? "Yes" : "No",
                                       StrInterstCountry = c.InterstCountry.Value == true ? "Yes" : "No",
                                       EnquiryCityName = dbContext.tblStateWithCities.Where(cct => cct.ID == sd.CityId).FirstOrDefault().VillageLocalityName,
                                       FromDate = sd.CreatedDate.ToString(),
                                       PurposeBusiness = sd.PurposeBusiness,
                                       QueryID = sd.ID,
                                       ProductName = cc.ItemName,
                                       ProfessionalRequirementID = sd.ProfessionalRequirementID,
                                       //RequirementName = dbContext.tblProfessionalRequirements.Where(b => b.ProfessionalRequirementID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName,
                                       RequirementName = dbContext.tblProfessionalRequirements.Where(b => b.ID == sd.ProfessionalRequirementID).FirstOrDefault().RequirementName,
                                       BusinessDemandID = sd.BusinessDemandID,
                                       LastConversationDate = uc.CreatedDate
                                   }).Distinct().ToList();

                    EnquiryList.ForEach(e => e.businessTypes = (from bt in dbContext.tblSelectedDealerBusinessTypes
                                                                join btc in dbContext.tblBusinessTypes on bt.BusinessTypeID.Value equals btc.ID
                                                                where bt.QueryID == e.QueryID
                                                                select new BusinessTypes
                                                                {
                                                                    BusinessTypeID = bt.BusinessTypeID.Value,
                                                                    BusinessTypeName = btc.Type
                                                                }).ToList()
                    );
                    if (search.InterstCity != null && search.InterstCity.Value == true)
                        EnquiryList = EnquiryList.Where(e => e.InterstCity.Value == true).ToList();
                    if (search.InterstState != null && search.InterstState.Value == true)
                        EnquiryList = EnquiryList.Where(e => e.InterstState.Value == true).ToList();
                    if (search.InterstCountry != null && search.InterstCountry.Value == true)
                        EnquiryList = EnquiryList.Where(e => e.InterstCountry.Value == true).ToList();

                    if (search.BusinessTypeList != null && search.BusinessTypeList.Count() > 0)
                    {
                        EnquiryList = EnquiryList.Where(i => i.businessTypes.Any(b => search.BusinessTypeList.Contains(Convert.ToInt32(b.BusinessTypeID)))).ToList();
                    }

                    if (!string.IsNullOrEmpty(search.PurposeBusiness))
                        EnquiryList = EnquiryList.Where(e => e.PurposeBusiness.ToLower() == search.PurposeBusiness.ToLower()).ToList();

                    if (search.BusinessDemandID != null && search.BusinessDemandID != 0)
                        EnquiryList = EnquiryList.Where(e => e.BusinessDemandID == search.BusinessDemandID).ToList();

                    if (!string.IsNullOrEmpty(search.FromDate) && !string.IsNullOrEmpty(search.ToDate))
                    {
                        DateTime FromDate = DateTime.ParseExact(search.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime ToDate = DateTime.ParseExact(search.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        EnquiryList = EnquiryList.Where(i => Convert.ToDateTime(i.FromDate).Date >= FromDate.Date && Convert.ToDateTime(i.FromDate).Date <= ToDate.Date).ToList();

                        if (!string.IsNullOrEmpty(search.FromTime) && !string.IsNullOrEmpty(search.ToTime))
                        {
                            DateTime fromTime = DateTime.ParseExact(search.FromTime,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
                            TimeSpan Fromspan = fromTime.TimeOfDay;
                            DateTime toTime = DateTime.ParseExact(search.ToTime,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
                            TimeSpan Tospan = toTime.TimeOfDay;

                            EnquiryList = EnquiryList.Where(c => Convert.ToDateTime(c.FromDate).TimeOfDay >= Fromspan && Convert.ToDateTime(c.FromDate).TimeOfDay <= Tospan).ToList();
                        }
                    }

                    if (search.StateList != null && search.StateList.Count() > 0)
                    {
                        EnquiryList = EnquiryList.Where(m => search.StateList.Contains(m.StateID)).ToList();
                    }
                    if (search.CityList != null && search.CityList.Count() > 0)
                    {
                        EnquiryList = EnquiryList.Where(m => search.CityList.Contains(m.CityID)).ToList();
                    }
                    if (search.ItemCategoryList != null && search.ItemCategoryList.Count() > 0)
                    {
                        EnquiryList = EnquiryList.Where(m => search.ItemCategoryList.Contains(m.ProductID)).ToList();
                    }

                    enquiryListWithTotals.TotalEnquiries = totalEnquiries;
                    enquiryListWithTotals.TotalCities = EnquiryList.Select(e => e.CityID).Distinct().Count();
                    enquiryListWithTotals.TotalStates = EnquiryList.Select(e => e.StateID).Distinct().Count();
                    enquiryListWithTotals.TotalItems = EnquiryList.Select(e => e.ProductID).Distinct().Count();
                    enquiryListWithTotals.TotalReplies = EnquiryList.Count();
                    enquiryListWithTotals.enquiryList = EnquiryList;
                    return enquiryListWithTotals;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }

        //Send Promotion
        public string Promotion(EnquiryListWithTotals promo, List<Attachment> MailAttachments)
        {
            try
            {
                string Result = string.Empty;
                if (promo.IsEmail == true)
                {
                    string Bcc = string.Empty;
                    List<CustomerDetails> bccList = new List<CustomerDetails>();

                    foreach (var item in promo.enquiryList)
                    {
                        if (!string.IsNullOrEmpty(item.EmailID))
                        {
                            if (item.IsChecked == true)
                            {
                                CustomerDetails custDetails1 = new CustomerDetails();
                                custDetails1.EmailID = item.EmailID;
                                bccList.Add(custDetails1);
                            }
                        }
                    }

                    string ToEmailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                    string FromMailID = ConfigurationManager.AppSettings["FromMailID"].ToString();
                    string MailPassword = ConfigurationManager.AppSettings["MailPassword"].ToString();
                    string MailServerHost = ConfigurationManager.AppSettings["MailServerHost"].ToString();
                    string SendingPort = ConfigurationManager.AppSettings["SendingPort"].ToString();
                    //string APKPath = ConfigurationManager.AppSettings["APKPath"].ToString();
                    string MailSubject = promo.MailSubject;

                    Helper.SendMail(ToEmailID, FromMailID, promo.MailBody, MailSubject, MailServerHost, MailPassword, SendingPort, bccList, MailAttachments);
                    Result = "Email Sent Successfully!!";
                }
                else if (promo.IsSMS == true)
                {
                    string SMSUserName = ConfigurationManager.AppSettings["SMSUserName"];
                    string SMSPassword = ConfigurationManager.AppSettings["SMSPassword"];

                    foreach (var item in promo.enquiryList)
                    {
                        if (item.IsChecked == true)
                        {
                            string Message = "Welcome to MWB Technology New customer details name Your OTP is : 123456.ID Test Ph Tets";
                            Helper.SendSMS(SMSUserName, SMSPassword, item.MobileNumber, Message, "N");
                        }
                    }
                    Result = "SMS Sent Successfully!!";
                }
                return Result;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex);
                return "Error!! Please contact administrator";
            }
        }

    }
}

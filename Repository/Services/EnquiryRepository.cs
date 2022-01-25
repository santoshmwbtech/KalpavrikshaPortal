using JBNClassLibrary;
using JBNWebAPI.Logger;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JBNClassLibrary.DLEnquiries;

namespace Repository.Services
{
    public class EnquiryRepository : IEnquiryRepository
    {
        public Task<List<CustomerQueries>> GetCustomerEnquiries(CustomerEnquiries customerEnquiries)
        {
            throw new NotImplementedException();
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
                                       }).Distinct().ToList();

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
                                           }).Distinct().ToList();

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

        public async Task<List<ItemCategory>> GetItemCategories()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    List<ItemCategory> itemCategories = new List<ItemCategory>();

                    itemCategories = await (from e in dbContext.tblselectedDealers
                                      join cc in dbContext.tblItemCategories on e.ProductID equals cc.ID
                                      select new ItemCategory
                                      {
                                          ID = cc.ID,
                                          ItemName = cc.ItemName
                                      }).Distinct().ToListAsync();

                    return itemCategories;
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

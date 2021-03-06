//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JBNClassLibrary
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class mwbtDealerEntities : DbContext
    {
        public mwbtDealerEntities()
            : base("name=mwbtDealerEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblAdminSetting> tblAdminSettings { get; set; }
        public virtual DbSet<tblAdvertisement> tblAdvertisements { get; set; }
        public virtual DbSet<tblAdvertisementArea> tblAdvertisementAreas { get; set; }
        public virtual DbSet<tblAdvertisementCity> tblAdvertisementCities { get; set; }
        public virtual DbSet<tblAdvertisementDistrict> tblAdvertisementDistricts { get; set; }
        public virtual DbSet<tblAdvertisementHoliday> tblAdvertisementHolidays { get; set; }
        public virtual DbSet<tblAdvertisementMain> tblAdvertisementMains { get; set; }
        public virtual DbSet<tbladvertisementPayment> tbladvertisementPayments { get; set; }
        public virtual DbSet<tblAdvertisementState> tblAdvertisementStates { get; set; }
        public virtual DbSet<tblAdvertisementTimeSlot> tblAdvertisementTimeSlots { get; set; }
        public virtual DbSet<tblAdvertisementType> tblAdvertisementTypes { get; set; }
        public virtual DbSet<tblAspNetUser> tblAspNetUsers { get; set; }
        public virtual DbSet<tblBrand> tblBrands { get; set; }
        public virtual DbSet<tblBusinessDemand> tblBusinessDemands { get; set; }
        public virtual DbSet<tblBusinessType> tblBusinessTypes { get; set; }
        public virtual DbSet<tblBusinessTypewithCust> tblBusinessTypewithCusts { get; set; }
        public virtual DbSet<tblCategoryHistory> tblCategoryHistories { get; set; }
        public virtual DbSet<tblCategoryProduct> tblCategoryProducts { get; set; }
        public virtual DbSet<tblCategoryProductWithCust> tblCategoryProductWithCusts { get; set; }
        public virtual DbSet<tblChildCategory> tblChildCategories { get; set; }
        public virtual DbSet<tblChildCategoryProductWithCust> tblChildCategoryProductWithCusts { get; set; }
        public virtual DbSet<tblCompany> tblCompanies { get; set; }
        public virtual DbSet<tblCustomerDetail> tblCustomerDetails { get; set; }
        public virtual DbSet<tblCustomerStatusHistory> tblCustomerStatusHistories { get; set; }
        public virtual DbSet<tblDeleteChat> tblDeleteChats { get; set; }
        public virtual DbSet<tblDeleteConversation> tblDeleteConversations { get; set; }
        public virtual DbSet<tblDeleteEnquiry> tblDeleteEnquiries { get; set; }
        public virtual DbSet<tblDistrict> tblDistricts { get; set; }
        public virtual DbSet<tblEmailFooter> tblEmailFooters { get; set; }
        public virtual DbSet<tblFavoriteConversation> tblFavoriteConversations { get; set; }
        public virtual DbSet<tblFormPermission> tblFormPermissions { get; set; }
        public virtual DbSet<tblFormPermissionItem> tblFormPermissionItems { get; set; }
        public virtual DbSet<tblHistory> tblHistories { get; set; }
        public virtual DbSet<tblItemCategory> tblItemCategories { get; set; }
        public virtual DbSet<tblPayment> tblPayments { get; set; }
        public virtual DbSet<tblProfessionalRequirement> tblProfessionalRequirements { get; set; }
        public virtual DbSet<tblPromo> tblPromoes { get; set; }
        public virtual DbSet<tblPushNotification> tblPushNotifications { get; set; }
        public virtual DbSet<tblQuestion> tblQuestions { get; set; }
        public virtual DbSet<tblselectedDealer> tblselectedDealers { get; set; }
        public virtual DbSet<tblSelectedDealerBusinessType> tblSelectedDealerBusinessTypes { get; set; }
        public virtual DbSet<tblselectedDealerDetail> tblselectedDealerDetails { get; set; }
        public virtual DbSet<tblSMSTemplate> tblSMSTemplates { get; set; }
        public virtual DbSet<tblState> tblStates { get; set; }
        public virtual DbSet<tblStateWithCity> tblStateWithCities { get; set; }
        public virtual DbSet<tblSubCategory> tblSubCategories { get; set; }
        public virtual DbSet<tblSubCategoryProductWithCust> tblSubCategoryProductWithCusts { get; set; }
        public virtual DbSet<tblSysMainMenu> tblSysMainMenus { get; set; }
        public virtual DbSet<tblSysRole> tblSysRoles { get; set; }
        public virtual DbSet<tblSysSubMenu> tblSysSubMenus { get; set; }
        public virtual DbSet<tblTairTypeOfCity> tblTairTypeOfCities { get; set; }
        public virtual DbSet<tblTairTypeOfDistrict> tblTairTypeOfDistricts { get; set; }
        public virtual DbSet<tblTairTypeOfState> tblTairTypeOfStates { get; set; }
        public virtual DbSet<tblTAndC> tblTAndCs { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblUserConversation> tblUserConversations { get; set; }
        public virtual DbSet<tblVideo> tblVideos { get; set; }
        public virtual DbSet<tblTaxSlab> tblTaxSlabs { get; set; }
        public virtual DbSet<ProductsView> ProductsViews { get; set; }
        public virtual DbSet<CityView> CityViews { get; set; }
        public virtual DbSet<tblAdTimeSlot> tblAdTimeSlots { get; set; }
        public virtual DbSet<tblAdvertisementCitySlot> tblAdvertisementCitySlots { get; set; }
        public virtual DbSet<tblAdvertisementDistrictSlot> tblAdvertisementDistrictSlots { get; set; }
        public virtual DbSet<tblAdvertisementNationSlot> tblAdvertisementNationSlots { get; set; }
        public virtual DbSet<tblAdvertisementStateSlot> tblAdvertisementStateSlots { get; set; }
        public virtual DbSet<tblBusinessDemandwithCust> tblBusinessDemandwithCusts { get; set; }
        public virtual DbSet<tblPurposeOfBusiness> tblPurposeOfBusinesses { get; set; }
        public virtual DbSet<tblAdvertisementDiscount> tblAdvertisementDiscounts { get; set; }
        public virtual DbSet<tblAdTracker> tblAdTrackers { get; set; }
    
        public virtual ObjectResult<USP_MainCatCityWiseRpt_Result> USP_MainCatCityWiseRpt(Nullable<int> stateID, Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var stateIDParameter = stateID.HasValue ?
                new ObjectParameter("StateID", stateID) :
                new ObjectParameter("StateID", typeof(int));
    
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<USP_MainCatCityWiseRpt_Result>("USP_MainCatCityWiseRpt", stateIDParameter, fromDateParameter, toDateParameter);
        }
    
        public virtual ObjectResult<USP_SubCatCityWiseRpt_Result> USP_SubCatCityWiseRpt(Nullable<int> stateID, Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var stateIDParameter = stateID.HasValue ?
                new ObjectParameter("StateID", stateID) :
                new ObjectParameter("StateID", typeof(int));
    
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<USP_SubCatCityWiseRpt_Result>("USP_SubCatCityWiseRpt", stateIDParameter, fromDateParameter, toDateParameter);
        }
    
        public virtual ObjectResult<USP_MainCatRpt_Result> USP_MainCatRpt(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<USP_MainCatRpt_Result>("USP_MainCatRpt", fromDateParameter, toDateParameter);
        }
    
        public virtual ObjectResult<USP_SubCatRpt_Result> USP_SubCatRpt(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<USP_SubCatRpt_Result>("USP_SubCatRpt", fromDateParameter, toDateParameter);
        }
    
        public virtual ObjectResult<GetAllCategories_Result> GetAllCategories()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetAllCategories_Result>("GetAllCategories");
        }
    
        public virtual ObjectResult<USP_ChildCatRpt_Result> USP_ChildCatRpt(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<USP_ChildCatRpt_Result>("USP_ChildCatRpt", fromDateParameter, toDateParameter);
        }
    
        public virtual ObjectResult<USP_ChildCatCityWiseRpt_Result> USP_ChildCatCityWiseRpt(Nullable<int> stateID, Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var stateIDParameter = stateID.HasValue ?
                new ObjectParameter("StateID", stateID) :
                new ObjectParameter("StateID", typeof(int));
    
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<USP_ChildCatCityWiseRpt_Result>("USP_ChildCatCityWiseRpt", stateIDParameter, fromDateParameter, toDateParameter);
        }
    
        public virtual ObjectResult<USP_ItemCatCityWiseRpt_Result> USP_ItemCatCityWiseRpt(Nullable<int> stateID, Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, string itemCategoryIds)
        {
            var stateIDParameter = stateID.HasValue ?
                new ObjectParameter("StateID", stateID) :
                new ObjectParameter("StateID", typeof(int));
    
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var itemCategoryIdsParameter = itemCategoryIds != null ?
                new ObjectParameter("ItemCategoryIds", itemCategoryIds) :
                new ObjectParameter("ItemCategoryIds", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<USP_ItemCatCityWiseRpt_Result>("USP_ItemCatCityWiseRpt", stateIDParameter, fromDateParameter, toDateParameter, itemCategoryIdsParameter);
        }
    
        public virtual ObjectResult<USP_ItemCatRpt_Result> USP_ItemCatRpt(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<USP_ItemCatRpt_Result>("USP_ItemCatRpt", fromDateParameter, toDateParameter);
        }
    }
}

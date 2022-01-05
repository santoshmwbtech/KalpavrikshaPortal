using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class DLMatrixMaster
    {
        public List<AdvertisementType> GetAdvertisementType()
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    var advertisementTypes = (from adType in dbContext.tblAdvertisementTypes.AsNoTracking()
                                              select new AdvertisementType
                                              {
                                                  ID = adType.ID,
                                                  AdTypeMatrix = adType.AdTypeMatrix,
                                                  OldMatrix = adType.OldMatrix,
                                                  Type = adType.Type,
                                                  Description = adType.Description,
                                              }).ToList();
                    return advertisementTypes;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return null;
            }
        }
        

        public List<AdvertisementArea> GetAdvertisementArea()
        {

            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    var advertisementarea = (from adArea in dbContext.tblAdvertisementAreas.AsNoTracking()
                                             select new AdvertisementArea
                                             {
                                                 ID = adArea.ID,
                                                 AdAreaMatrix = adArea.AdAreaMatrix,
                                                 OldMatrix = adArea.OldMatrix,
                                                 AdvertisementAreaName = adArea.AdvertisementAreaName,
                                             }).ToList();
                    return advertisementarea;

                }

            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);

                return null;

            }
        }

        public List<AdvertisementSlot> GetAdvertisementTimeSlot()
        {

            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    var advertisementslot = (from timeSlot in dbContext.tblAdvertisementTimeSlots.AsNoTracking()
                                             select new AdvertisementSlot
                                             {
                                                 ID = timeSlot.ID,
                                                 TimeSlotMatrix = timeSlot.TimeSlotMatrix,
                                                 OldMatrix = timeSlot.OldMatrix,
                                                 TimeSlotName = timeSlot.TimeSlotName,
                                             }).ToList();
                    return advertisementslot;

                }

            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);

                return null;

            }
        }

        public string UpdateAdType(List<AdvertisementType> advertisementTypes)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    foreach (var advertisementType in advertisementTypes)
                    {
                        var tbladvertisementType = dbContext.tblAdvertisementTypes.Where(d => d.ID == advertisementType.ID).FirstOrDefault();
                        if (tbladvertisementType != null)
                        {
                            tbladvertisementType.OldMatrix = tbladvertisementType.AdTypeMatrix.Value;
                            tbladvertisementType.AdTypeMatrix = advertisementType.AdTypeMatrix;
                            dbContext.tblAdvertisementTypes.Attach(tbladvertisementType);
                            dbContext.Entry(tbladvertisementType).Property(d => d.OldMatrix).IsModified = true;
                            dbContext.Entry(tbladvertisementType).Property(d => d.AdTypeMatrix).IsModified = true;
                        }
                    }
                    dbContext.SaveChanges();
                    return "Matrix updated successfully";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return "Error";
            }
        }

        public string UpdateAdArea(List<AdvertisementArea> advertisementAreas)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    foreach (var advertisementArea in advertisementAreas)
                    {
                        var tblAdvertisementArea = dbContext.tblAdvertisementAreas.Where(d => d.ID == advertisementArea.ID).FirstOrDefault();
                        if (tblAdvertisementArea != null)
                        {
                            tblAdvertisementArea.OldMatrix = tblAdvertisementArea.AdAreaMatrix.Value;
                            tblAdvertisementArea.AdAreaMatrix = advertisementArea.AdAreaMatrix;
                            dbContext.tblAdvertisementAreas.Attach(tblAdvertisementArea);
                            dbContext.Entry(tblAdvertisementArea).Property(d => d.OldMatrix).IsModified = true;
                            dbContext.Entry(tblAdvertisementArea).Property(d => d.AdAreaMatrix).IsModified = true;
                        }
                    }
                    dbContext.SaveChanges();
                    return "Matrix updated successfully";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return "Error";
            }
        }

        public string UpdateTimeSlots(List<AdvertisementTimeSlot> advertisementTimeSlots)
        {
            try
            {
                using (mwbtDealerEntities dbContext = new mwbtDealerEntities())
                {
                    if (dbContext.Database.Connection.State == System.Data.ConnectionState.Closed)
                        dbContext.Database.Connection.Open();

                    foreach (var advertisementTimeSlot in advertisementTimeSlots)
                    {
                        var tbladvertisementTimeSlot = dbContext.tblAdvertisementTimeSlots.Where(d => d.ID == advertisementTimeSlot.ID).FirstOrDefault();
                        if (tbladvertisementTimeSlot != null)
                        {
                            tbladvertisementTimeSlot.OldMatrix = tbladvertisementTimeSlot.TimeSlotMatrix.Value;
                            tbladvertisementTimeSlot.TimeSlotMatrix = advertisementTimeSlot.TimeSlotMatrix;
                            dbContext.tblAdvertisementTimeSlots.Attach(tbladvertisementTimeSlot);
                            dbContext.Entry(tbladvertisementTimeSlot).Property(d => d.OldMatrix).IsModified = true;
                            dbContext.Entry(tbladvertisementTimeSlot).Property(d => d.TimeSlotMatrix).IsModified = true;
                        }
                    }
                    dbContext.SaveChanges();
                    return "Matrix updated successfully";
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                return "Error";
            }
        }

    }
    public class AdvertisementType
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<double> AdTypeMatrix { get; set; }
        public bool IsActive { get; set; }
        public Nullable<double> OldMatrix { get; set; }
    }

    public class AdvertisementArea
    {
        public int ID { get; set; }
        public string AdvertisementAreaName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<double> AdAreaMatrix { get; set; }
        public Nullable<double> AdvertisementPrice { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<double> OldMatrix { get; set; }
    }

    public class AdvertisementSlot
    {
        public int ID { get; set; }
        public string TimeSlotName { get; set; }
        public Nullable<System.TimeSpan> FromTime { get; set; }
        public Nullable<System.TimeSpan> ToTime { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<double> TimeSlotMatrix { get; set; }
        public bool IsActive { get; set; }
        public Nullable<double> OldMatrix { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class ProformaInvoice
    {
        public AdvertisementMain main { get; set; }
        public Company company { get; set; }
        public CustomerDetails customer { get; set; }
        public string AdvertisementAreaName { get; set; }
        public Nullable<double> CategoryMatrix { get; set; }
        public Nullable<double> AdAreaMatrix { get; set; }
        public string AdvertisementType { get; set; }
        public Nullable<double> AdTypeMatrix { get; set; }
        public List<AdvertisementTimeSlot> TimeSlots { get; set; }
        public Nullable<double> StateMatrix { get; set; }
        public Nullable<double> CityMatrix { get; set; }
        public Nullable<double> DistrictMatrix { get; set; }
        public string BrandName { get; set; }
        public List<AdvertisementStates> advertisementStates { get; set; }
        public List<AdvertisementDistricts> advertisementDistricts { get; set; }
        public List<AdvertisementCities> advertisementCities { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> TotalStates { get; set; }
        public Nullable<int> TotalDistricts { get; set; }
        public Nullable<int> TotalCities { get; set; }
    }
    public class DLProformaInvoice
    {

    }
    public class AdvertisementTimeSlot
    {
        public int ID { get; set; }
        public string TimeSlotName { get; set; }
        public Nullable<double> TimeSlotMatrix { get; set; }
    }
}

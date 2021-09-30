//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SadguruCRM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Lead
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lead()
        {
            this.Collection_Entry = new HashSet<Collection_Entry>();
            this.Contracts = new HashSet<Contract>();
            this.Customers = new HashSet<Customer>();
            this.Estimates = new HashSet<Estimate>();
            this.Lead_Cell_No_Mapping = new HashSet<Lead_Cell_No_Mapping>();
            this.Lead_Consult_Person = new HashSet<Lead_Consult_Person>();
            this.Lead_Email_Mapping = new HashSet<Lead_Email_Mapping>();
            this.Lead_Locations_Mapping = new HashSet<Lead_Locations_Mapping>();
            this.Lead_Services_Mapping = new HashSet<Lead_Services_Mapping>();
            this.Lead_Services_Mapping_History = new HashSet<Lead_Services_Mapping_History>();
            this.Lead_Tel_No_Mapping = new HashSet<Lead_Tel_No_Mapping>();
            this.Leads_Follow_Up_Details = new HashSet<Leads_Follow_Up_Details>();
            this.Pending_Leads = new HashSet<Pending_Leads>();
            this.Servicings = new HashSet<Servicing>();
        }
    
        public int LeadID { get; set; }
        public Nullable<System.DateTime> LeadDate { get; set; }
        public string Title { get; set; }
        public string CustomerName { get; set; }
        public string TelNo { get; set; }
        public string CellNo { get; set; }
        public string Pincode { get; set; }
        public Nullable<int> CityID { get; set; }
        public string EmailID { get; set; }
        public Nullable<int> SourceID { get; set; }
        public Nullable<int> SubSourceID { get; set; }
        public Nullable<int> TypeOfPremisesID { get; set; }
        public Nullable<int> TeleCallerID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> VisitAllocate { get; set; }
        public Nullable<System.DateTime> VisitDateTime { get; set; }
        public string VisitReport { get; set; }
        public string RATE { get; set; }
        public string TAX { get; set; }
        public string FinalRate { get; set; }
        public string CustomerPriority { get; set; }
        public Nullable<System.DateTime> NextFollowUpDateTime { get; set; }
        public string SpecialInstructions { get; set; }
        public string FollowUpDetails { get; set; }
        public Nullable<int> LeadStatusID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<int> LocationID { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<short> NoOfWings { get; set; }
        public Nullable<short> NoOfFloors { get; set; }
        public Nullable<System.DateTime> LeasdClosedDateTime { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> LastUpdatedBy { get; set; }
        public string ConsultPerson { get; set; }
        public string ConsultPersonDesignation { get; set; }
        public Nullable<int> IndustryID { get; set; }
        public string PremisesArea { get; set; }
        public string PremisesAppSqFtArea { get; set; }
        public string StatusReason { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<int> Premises_Area_ID { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
        public Nullable<System.TimeSpan> ServiceTime { get; set; }
        public Nullable<System.TimeSpan> VisitTime { get; set; }
        public Nullable<System.TimeSpan> NextFollowUpTime { get; set; }
        public Nullable<short> NoOfFlats { get; set; }
        public Nullable<decimal> SGSTSumOfAllServices { get; set; }
        public Nullable<decimal> CGSTSumOfAllServices { get; set; }
        public Nullable<decimal> IGSTSumOfAllServices { get; set; }
        public Nullable<decimal> AlreadyPaidSum { get; set; }
        public string BalanceAmount { get; set; }
        public string LeadNumber { get; set; }
        public string OtherDetails { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual City City { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Collection_Entry> Collection_Entry { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Contract> Contracts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual Employee Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Estimate> Estimates { get; set; }
        public virtual Industry Industry { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead_Cell_No_Mapping> Lead_Cell_No_Mapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead_Consult_Person> Lead_Consult_Person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead_Email_Mapping> Lead_Email_Mapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead_Locations_Mapping> Lead_Locations_Mapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead_Services_Mapping> Lead_Services_Mapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead_Services_Mapping_History> Lead_Services_Mapping_History { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lead_Tel_No_Mapping> Lead_Tel_No_Mapping { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual LeadStatus LeadStatus { get; set; }
        public virtual Location Location { get; set; }
        public virtual Premises_Area_Master Premises_Area_Master { get; set; }
        public virtual Source Source { get; set; }
        public virtual State State { get; set; }
        public virtual Source Source1 { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual PremisesType PremisesType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Leads_Follow_Up_Details> Leads_Follow_Up_Details { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pending_Leads> Pending_Leads { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Servicing> Servicings { get; set; }
    }
}

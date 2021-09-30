using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SadguruCRM.Models;

namespace SadguruCRM.ViewModels
{
    public class EstimateViewModel
    {
        public EstimateViewModel()
        {
            this.Estimate_Services_Mapping = new HashSet<Estimate_Services_Mapping>();
        }

        public int EstimateID { get; set; }
        public bool NewCustomer { get; set; }
        public bool ExistingCustomer { get; set; }
        public Nullable<int> LeadID { get; set; }
        public Nullable<long> CustomerID { get; set; }
        public string FinalEstimateRate { get; set; }
        public string KindAttention { get; set; }
        public string Subject { get; set; }
        public string WelcomeSentence { get; set; }
        public string Footer { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> LastUpdatedBy { get; set; }
        public string RATE { get; set; }
        public string TAX { get; set; }
        public string FinalRate { get; set; }
        public string ReferenceNo { get; set; }
        public Nullable<System.DateTime> EstimateDate { get; set; }
        public string ServiceScope { get; set; }
        public bool isNewEstimate { get; set; }
        public Nullable<int> OriginalEstimateID { get; set; }
        public int RevisionNo { get; set; }
        public Nullable<bool> isApproved { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedAt { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Estimate_Services_Mapping> Estimate_Services_Mapping { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual Lead Lead { get; set; }
    }
}
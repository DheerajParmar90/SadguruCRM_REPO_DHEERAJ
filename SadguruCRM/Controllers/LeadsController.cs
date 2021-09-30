using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LinqToExcel;
//using Microsoft.AspNet.SignalR;
using SadguruCRM.Helpers;
using SadguruCRM.Models;
using SadguruCRM.ViewModels;


namespace SadguruCRM.Controllers
{
    [VerifyUser]
    public class LeadsController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        //private readonly IHubContext<NotificationHub> _hubContext;
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Leads
        public async Task<ActionResult> Index()
        {
            List<Lead> leadsOrderedNewCode = new List<Lead>();
            try
            {
                if (TempData["ErrorMessage"] != null)
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
                }
                if (!watch.IsRunning)
                {
                    watch = System.Diagnostics.Stopwatch.StartNew();
                }
                var leads = db.Leads.Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);

                //new Code
                var leadsList = await new HelperNonStatic().GenerateCustomerNameAndServicesShortCodes(leads);
                leadsOrderedNewCode = leadsList.OrderByDescending(i => i.LeadID).ToList();
                //OLD CODE
                //foreach (var item in leads)
                //{
                //    if (item.CustomerName == null)
                //    {
                //        item.CustomerName = item.Title + " " + item.FirstName + " " + item.LastName;
                //    }
                //    var listOfServiceIDs = item.Lead_Services_Mapping.Select(c => c.ServiceID).ToList();

                //    item.AddressLine1 = String.Join(", ", db.Services.Where(x => listOfServiceIDs.Contains(x.ServiceID)).Select(x => x.ServiceShortCode).ToArray());
                //}
                //var leadsOrderedOldCode = leads.OrderByDescending(i => i.LeadID);

                watch.Stop();
                long totalTime;
                if (TempData["TimeTaken"] != null)
                {
                    totalTime = Int64.Parse(TempData["TimeTaken"].ToString()) + watch.ElapsedMilliseconds;
                }
                else
                {
                    totalTime = watch.ElapsedMilliseconds;
                }
                ViewBag.TimeTaken = totalTime;
                //return View(leadsOrderedOldCode);
                
            }
            catch (Exception ex) {

            }
            return View(leadsOrderedNewCode);
        }
        public ActionResult TransferLeads()
        {
            ViewBag.TeleCallerID = new SelectList(db.UserLogins.Where(x => x.UserLoginTypeID == 2), "UserID", "UserName");
            return View();
        }
        
        [HttpPost]
        public ActionResult TransferLeads(string FromLeadNumber, string ToLeadNumber, int TeleCallerID)
        {
            //int outFromLeadNumber, outToLeadNumber;
            //if (Int32.TryParse(FromLeadNumber, out outFromLeadNumber)) {
            //    if (Int32.TryParse(ToLeadNumber, out outToLeadNumber))
            //    {
            //        using (var db = new SadguruCRMEntities())
            //        {
            //            var leads = db.Leads.Where(l => l.LeadID>= outFromLeadNumber && l.LeadID <= outToLeadNumber).ToList();
            //            leads.ForEach(a => a.TeleCallerID = TeleCallerID);
            //            db.SaveChanges();
            //        }
            //    }
            //}
            //ViewBag.TeleCallerID = new SelectList(db.UserLogins.Where(x => x.UserLoginTypeID == 2), "UserID", "UserName");
            return RedirectToAction("Index");
        }
        public ActionResult ImportLeads()
        {
            // ViewBag.TeleCallerID = new SelectList(db.UserLogins.Where(x => x.UserLoginTypeID == 2), "UserID", "UserName");
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            if (TempData["FileName"] != null)
            {
                ViewBag.FileName = TempData["FileName"].ToString();
            }
            return View();
        }
        public FileResult DownloadExcel()
        {
            string path = "/Documents/Leads_ImportFormat.xlsx";
            return File(path, "application/vnd.ms-excel", "Leads_importFormat.xlsx");
        }
        [HttpPost]
        public ActionResult UploadExcel(Lead leads, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {

                    string extn = System.IO.Path.GetExtension(FileUpload.FileName);
                    string filename = FileUpload.FileName.Replace(extn,"")+"-" + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE).ToString("dd-MM-yyyy-hh-mm-ss")+ extn;
                    string targetpath = Server.MapPath("~/Documents/FileUploads/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Lead$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dataTableLeads = ds.Tables["ExcelTable"];
                    //if (dataTableLeads.Columns[0].ColumnName != "Title" || ) { 
                    
                    //}
                        dataTableLeads = dataTableLeads.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).CopyToDataTable();


                    string sheetName = "Lead";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var leadsData = from a in excelFile.Worksheet<Leads_Import_Format>(sheetName) where !String.IsNullOrEmpty(a.Title) select a;


                    int success = 0, errors = 0;
                    foreach (var leadRow in leadsData)
                    {
                        try
                        {
                            if (leadRow.Title == "" || leadRow.Date.ToString() == "" || leadRow.Location == "" || leadRow.City == "" || leadRow.Type_Of_Premise == ""
                                || leadRow.Source == "" || leadRow.Sub_Source == "" || leadRow.Lead_Status== "" || leadRow.Branch == "" || leadRow.Tele_Caller == "") {
                                data.Add("<ul>");
                                data.Add("<li> Title, Lead Date, Location, City, Type Of Premise, Source, Sub Source, Lead Status, Branch, Tele Caller is required</li>");
                               // data.Add("<li> Address is required</li>");
                               // data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                            Lead lead = new Lead();
                            lead.Title = leadRow.Title;
                            lead.FirstName = leadRow.First_Name;
                            lead.LastName = leadRow.Last_Name;
                            lead.CustomerName = leadRow.Customer_Name;
                            lead.LeadDate = DateTime.Parse(leadRow.Date) ;
                            lead.ConsultPerson = leadRow.ConsultPerson;
                            lead.ConsultPersonDesignation = leadRow.Designation;
                            lead.AddressLine1 = leadRow.Address_Line_1;
                            lead.AddressLine2 = leadRow.Address_Line_2;
                            lead.AddressLine3 = leadRow.Address_Line_3;
                            if(db.Locations.Where(l => l.LocationName == leadRow.Location).FirstOrDefault() != null)
                                lead.LocationID =  db.Locations.Where(l => l.LocationName == leadRow.Location).FirstOrDefault().LocationID ;
                            if (db.Cities.Where(l => l.CityName == leadRow.City).FirstOrDefault() != null)
                                lead.CityID = db.Cities.Where(l => l.CityName == leadRow.City).FirstOrDefault().CityID;
                            lead.Pincode = leadRow.Pincode;
                            if (db.States.Where(l => l.State1 == leadRow.State).FirstOrDefault() != null)
                                lead.StateID = db.States.Where(l => l.State1 == leadRow.State).FirstOrDefault().StateID;

                            lead.TelNo = leadRow.Tel_No_1;
                            lead.CellNo = leadRow.Cell_No_1;
                            lead.EmailID = leadRow.Email_1;

                            if(db.PremisesTypes.Where(l => l.PremisesType1 == leadRow.Type_Of_Premise).FirstOrDefault() != null)
                                lead.TypeOfPremisesID = db.PremisesTypes.Where(l => l.PremisesType1 == leadRow.Type_Of_Premise).FirstOrDefault().PremisesTypeID;
                            short noOfWings, noOfFloors, noOfflats;
                            lead.NoOfWings = short.TryParse(leadRow.No_Of_Wings, out noOfWings) ? noOfWings : (short)0;
                            lead.NoOfFloors = short.TryParse(leadRow.No_Of_Floors, out noOfFloors) ? noOfFloors : (short)0;
                            lead.NoOfFlats = short.TryParse(leadRow.No_Of_Flats, out noOfflats) ? noOfflats : (short)0;

                            if(db.Premises_Area_Master.Where(l => l.Premises_Area == leadRow.Premises_Area).FirstOrDefault() != null)
                                lead.Premises_Area_ID = db.Premises_Area_Master.Where(l => l.Premises_Area == leadRow.Premises_Area).FirstOrDefault().Premises_Area_ID;
                            lead.PremisesAppSqFtArea = leadRow.Approx_Sq_Ft_Area;


                            lead.OtherDetails = leadRow.Other_Details;
                            if (db.Industries.Where(l => l.IndustryName == leadRow.Industry).FirstOrDefault() != null)
                                lead.IndustryID = db.Industries.Where(l => l.IndustryName == leadRow.Industry).FirstOrDefault().IndustryID;
                            if (db.Employees.Where(l => l.Name == leadRow.Visit_Allocate).FirstOrDefault() != null)
                                lead.VisitAllocate = db.Employees.Where(l => l.Name == leadRow.Visit_Allocate).FirstOrDefault().EmployeeID;
                            if (db.Sources.Where(l => l.Source1 == leadRow.Source).FirstOrDefault() != null)
                                lead.SourceID = db.Sources.Where(l => l.Source1 == leadRow.Source).FirstOrDefault().SourceID;
                            if(db.Sources.Where(l => l.Source1 == leadRow.Sub_Source).FirstOrDefault() != null)
                                lead.SubSourceID = db.Sources.Where(l => l.Source1 == leadRow.Sub_Source).FirstOrDefault().SourceID;
                            lead.FollowUpDetails = leadRow.Follow_Up_Details;
                            lead.CustomerPriority = leadRow.Customer_Priority;
                            if(db.LeadStatuses.Where(s => s.Status == leadRow.Lead_Status).FirstOrDefault() != null)
                                lead.LeadStatusID = db.LeadStatuses.Where(s => s.Status == leadRow.Lead_Status).FirstOrDefault().StatusID;
                            if(db.Branches.Where(s => s.BranchName == leadRow.Branch).FirstOrDefault() != null)
                                lead.BranchID = db.Branches.Where(s => s.BranchName== leadRow.Branch).FirstOrDefault().BranchID;
                            if(db.UserLogins.Where(s => s.UserName == leadRow.Tele_Caller).FirstOrDefault() != null)
                                lead.TeleCallerID = db.UserLogins.Where(s => s.UserName == leadRow.Tele_Caller).FirstOrDefault().UserID;

                            lead.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                            if (lead.Title != "M/S.")
                            {
                                lead.CustomerName = null;
                            }
                            else
                            {
                                lead.FirstName = null;
                                lead.LastName = null;
                            }
                            lead.CreatedBy = Convert.ToInt32(Session["UserID"]);

                            db.Leads.Add(lead);
                            db.SaveChanges();

                            if (!String.IsNullOrEmpty(leadRow.Consult_Person_1_Name))
                            {
                                Lead_Consult_Person cons = new Lead_Consult_Person();
                                cons.PersonaName = leadRow.Consult_Person_1_Name;
                                cons.Designation = leadRow.Consult_Person_1_Designation;
                                cons.LeadID = lead.LeadID;
                                db.Lead_Consult_Person.Add(cons);
                                db.SaveChanges();
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_1_Contact_Number_1))
                                {
                                    Lead_Consult_Person_Contact_No_Mapping cont = new Lead_Consult_Person_Contact_No_Mapping();
                                    cont.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    cont.Lead_Consult_Person_Contact_No = leadRow.Consult_Person_1_Contact_Number_1;
                                    db.Lead_Consult_Person_Contact_No_Mapping.Add(cont);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_1_Contact_Number_2))
                                {
                                    Lead_Consult_Person_Contact_No_Mapping cont = new Lead_Consult_Person_Contact_No_Mapping();
                                    cont.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    cont.Lead_Consult_Person_Contact_No = leadRow.Consult_Person_1_Contact_Number_2;
                                    db.Lead_Consult_Person_Contact_No_Mapping.Add(cont);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_1_Contact_Number_3))
                                {
                                    Lead_Consult_Person_Contact_No_Mapping cont = new Lead_Consult_Person_Contact_No_Mapping();
                                    cont.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    cont.Lead_Consult_Person_Contact_No = leadRow.Consult_Person_1_Contact_Number_3;
                                    db.Lead_Consult_Person_Contact_No_Mapping.Add(cont);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_1_Contact_Number_4))
                                {
                                    Lead_Consult_Person_Contact_No_Mapping cont = new Lead_Consult_Person_Contact_No_Mapping();
                                    cont.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    cont.Lead_Consult_Person_Contact_No = leadRow.Consult_Person_1_Contact_Number_4;
                                    db.Lead_Consult_Person_Contact_No_Mapping.Add(cont);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_1_Contact_Number_5))
                                {
                                    Lead_Consult_Person_Contact_No_Mapping cont = new Lead_Consult_Person_Contact_No_Mapping();
                                    cont.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    cont.Lead_Consult_Person_Contact_No = leadRow.Consult_Person_1_Contact_Number_5;
                                    db.Lead_Consult_Person_Contact_No_Mapping.Add(cont);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_1_Email_1))
                                {
                                    Lead_Consult_Person_Email_Mapping mail = new Lead_Consult_Person_Email_Mapping();
                                    mail.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    mail.Lead_Consult_Person_Email = leadRow.Consult_Person_1_Email_1;
                                    db.Lead_Consult_Person_Email_Mapping.Add(mail);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_1_Email_2))
                                {
                                    Lead_Consult_Person_Email_Mapping mail = new Lead_Consult_Person_Email_Mapping();
                                    mail.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    mail.Lead_Consult_Person_Email = leadRow.Consult_Person_1_Email_2;
                                    db.Lead_Consult_Person_Email_Mapping.Add(mail);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_1_Email_3))
                                {
                                    Lead_Consult_Person_Email_Mapping mail = new Lead_Consult_Person_Email_Mapping();
                                    mail.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    mail.Lead_Consult_Person_Email = leadRow.Consult_Person_1_Email_3;
                                    db.Lead_Consult_Person_Email_Mapping.Add(mail);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_1_Email_4))
                                {
                                    Lead_Consult_Person_Email_Mapping mail = new Lead_Consult_Person_Email_Mapping();
                                    mail.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    mail.Lead_Consult_Person_Email = leadRow.Consult_Person_1_Email_4;
                                    db.Lead_Consult_Person_Email_Mapping.Add(mail);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_1_Email_5))
                                {
                                    Lead_Consult_Person_Email_Mapping mail = new Lead_Consult_Person_Email_Mapping();
                                    mail.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    mail.Lead_Consult_Person_Email = leadRow.Consult_Person_1_Email_5;
                                    db.Lead_Consult_Person_Email_Mapping.Add(mail);
                                    //db.SaveChanges();
                                }
                                db.SaveChanges();
                            }
                            if (!String.IsNullOrEmpty(leadRow.Consult_Person_2_Name))
                            {
                                Lead_Consult_Person cons = new Lead_Consult_Person();
                                cons.PersonaName = leadRow.Consult_Person_2_Name;
                                cons.Designation = leadRow.Consult_Person_2_Designation;
                                cons.LeadID = lead.LeadID;
                                db.Lead_Consult_Person.Add(cons);
                                db.SaveChanges();
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_2_Contact_Number_1))
                                {
                                    Lead_Consult_Person_Contact_No_Mapping cont = new Lead_Consult_Person_Contact_No_Mapping();
                                    cont.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    cont.Lead_Consult_Person_Contact_No = leadRow.Consult_Person_2_Contact_Number_1;
                                    db.Lead_Consult_Person_Contact_No_Mapping.Add(cont);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_2_Contact_Number_2))
                                {
                                    Lead_Consult_Person_Contact_No_Mapping cont = new Lead_Consult_Person_Contact_No_Mapping();
                                    cont.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    cont.Lead_Consult_Person_Contact_No = leadRow.Consult_Person_2_Contact_Number_2;
                                    db.Lead_Consult_Person_Contact_No_Mapping.Add(cont);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_2_Contact_Number_3))
                                {
                                    Lead_Consult_Person_Contact_No_Mapping cont = new Lead_Consult_Person_Contact_No_Mapping();
                                    cont.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    cont.Lead_Consult_Person_Contact_No = leadRow.Consult_Person_2_Contact_Number_3;
                                    db.Lead_Consult_Person_Contact_No_Mapping.Add(cont);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_2_Contact_Number_4))
                                {
                                    Lead_Consult_Person_Contact_No_Mapping cont = new Lead_Consult_Person_Contact_No_Mapping();
                                    cont.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    cont.Lead_Consult_Person_Contact_No = leadRow.Consult_Person_2_Contact_Number_4;
                                    db.Lead_Consult_Person_Contact_No_Mapping.Add(cont);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_2_Contact_Number_5))
                                {
                                    Lead_Consult_Person_Contact_No_Mapping cont = new Lead_Consult_Person_Contact_No_Mapping();
                                    cont.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    cont.Lead_Consult_Person_Contact_No = leadRow.Consult_Person_2_Contact_Number_5;
                                    db.Lead_Consult_Person_Contact_No_Mapping.Add(cont);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_2_Email_1))
                                {
                                    Lead_Consult_Person_Email_Mapping mail = new Lead_Consult_Person_Email_Mapping();
                                    mail.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    mail.Lead_Consult_Person_Email = leadRow.Consult_Person_2_Email_1;
                                    db.Lead_Consult_Person_Email_Mapping.Add(mail);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_2_Email_2))
                                {
                                    Lead_Consult_Person_Email_Mapping mail = new Lead_Consult_Person_Email_Mapping();
                                    mail.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    mail.Lead_Consult_Person_Email = leadRow.Consult_Person_2_Email_2;
                                    db.Lead_Consult_Person_Email_Mapping.Add(mail);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_2_Email_3))
                                {
                                    Lead_Consult_Person_Email_Mapping mail = new Lead_Consult_Person_Email_Mapping();
                                    mail.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    mail.Lead_Consult_Person_Email = leadRow.Consult_Person_2_Email_3;
                                    db.Lead_Consult_Person_Email_Mapping.Add(mail);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_2_Email_4))
                                {
                                    Lead_Consult_Person_Email_Mapping mail = new Lead_Consult_Person_Email_Mapping();
                                    mail.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    mail.Lead_Consult_Person_Email = leadRow.Consult_Person_2_Email_4;
                                    db.Lead_Consult_Person_Email_Mapping.Add(mail);
                                    //db.SaveChanges();
                                }
                                if (!String.IsNullOrEmpty(leadRow.Consult_Person_2_Email_5))
                                {
                                    Lead_Consult_Person_Email_Mapping mail = new Lead_Consult_Person_Email_Mapping();
                                    mail.Lead_Consult_Person_ID = cons.Lead_Consult_Person_ID;
                                    mail.Lead_Consult_Person_Email = leadRow.Consult_Person_2_Email_5;
                                    db.Lead_Consult_Person_Email_Mapping.Add(mail);
                                    //db.SaveChanges();
                                }
                                db.SaveChanges();
                            }

                            if (!String.IsNullOrEmpty(leadRow.Service_Group_1))
                            {
                                if (db.ServiceGroups.Where(l => l.ServiceGroup1 == leadRow.Service_Group_1).FirstOrDefault() != null
                                    || db.Services.Where(l => l.ServiceName == leadRow.Service_1).FirstOrDefault() != null)
                                {
                                    Lead_Services_Mapping ser = new Lead_Services_Mapping();
                                    ser.LeadID = lead.LeadID;
                                    ser.ServiceGroupID = db.ServiceGroups.Where(l => l.ServiceGroup1 == leadRow.Service_Group_1).FirstOrDefault().ServiceGroupID;
                                    ser.ServiceID = db.Services.Where(l => l.ServiceName == leadRow.Service_1).FirstOrDefault().ServiceID;
                                    if (db.FrequencyOfServices.Where(l => l.FrequencyOfService1 == leadRow.Frequency_Of_Service_1).FirstOrDefault() != null)
                                    {
                                        ser.FrequencyOfServiceID = db.FrequencyOfServices.Where(l => l.FrequencyOfService1 == leadRow.Frequency_Of_Service_1).FirstOrDefault().FrequencyOfServiceID;
                                    }
                                    short outQty;
                                    if (short.TryParse(leadRow.Qty_1, out outQty))
                                    {
                                        ser.Qty = outQty;
                                    }
                                    decimal outRate;
                                    if (decimal.TryParse(leadRow.Rate_1, out outRate))
                                    {
                                        ser.Rate = outRate;
                                    }
                                    if (leadRow.GST_1.ToLower() == "yes")
                                    {
                                        ser.GST = true;
                                    }
                                    else
                                    {
                                        ser.GST = false;
                                    }

                                }
                            }
                            if (!String.IsNullOrEmpty(leadRow.Service_Group_2))
                            {
                                if (db.ServiceGroups.Where(l => l.ServiceGroup1 == leadRow.Service_Group_2).FirstOrDefault() != null
                                    || db.Services.Where(l => l.ServiceName == leadRow.Service_2).FirstOrDefault() != null)
                                {
                                    Lead_Services_Mapping ser = new Lead_Services_Mapping();
                                    ser.LeadID = lead.LeadID;
                                    ser.ServiceGroupID = db.ServiceGroups.Where(l => l.ServiceGroup1 == leadRow.Service_Group_2).FirstOrDefault().ServiceGroupID;
                                    ser.ServiceID = db.Services.Where(l => l.ServiceName == leadRow.Service_2).FirstOrDefault().ServiceID;
                                    if (db.FrequencyOfServices.Where(l => l.FrequencyOfService1 == leadRow.Frequency_Of_Service_2).FirstOrDefault() != null)
                                    {
                                        ser.FrequencyOfServiceID = db.FrequencyOfServices.Where(l => l.FrequencyOfService1 == leadRow.Frequency_Of_Service_2).FirstOrDefault().FrequencyOfServiceID;
                                    }
                                    short outQty;
                                    if (short.TryParse(leadRow.Qty_2, out outQty))
                                    {
                                        ser.Qty = outQty;
                                    }
                                    decimal outRate;
                                    if (decimal.TryParse(leadRow.Rate_2, out outRate))
                                    {
                                        ser.Rate = outRate;
                                    }
                                    if (leadRow.GST_2.ToLower() == "yes")
                                    {
                                        ser.GST = true;
                                    }
                                    else
                                    {
                                        ser.GST = false;
                                    }

                                }
                            }
                            if (!String.IsNullOrEmpty(leadRow.Service_Group_3))
                            {
                                if (db.ServiceGroups.Where(l => l.ServiceGroup1 == leadRow.Service_Group_3).FirstOrDefault() != null
                                    || db.Services.Where(l => l.ServiceName == leadRow.Service_3).FirstOrDefault() != null)
                                {
                                    Lead_Services_Mapping ser = new Lead_Services_Mapping();
                                    ser.LeadID = lead.LeadID;
                                    ser.ServiceGroupID = db.ServiceGroups.Where(l => l.ServiceGroup1 == leadRow.Service_Group_3).FirstOrDefault().ServiceGroupID;
                                    ser.ServiceID = db.Services.Where(l => l.ServiceName == leadRow.Service_3).FirstOrDefault().ServiceID;
                                    if (db.FrequencyOfServices.Where(l => l.FrequencyOfService1 == leadRow.Frequency_Of_Service_3).FirstOrDefault() != null)
                                    {
                                        ser.FrequencyOfServiceID = db.FrequencyOfServices.Where(l => l.FrequencyOfService1 == leadRow.Frequency_Of_Service_3).FirstOrDefault().FrequencyOfServiceID;
                                    }
                                    short outQty;
                                    if (short.TryParse(leadRow.Qty_3, out outQty))
                                    {
                                        ser.Qty = outQty;
                                    }
                                    decimal outRate;
                                    if (decimal.TryParse(leadRow.Rate_3, out outRate))
                                    {
                                        ser.Rate = outRate;
                                    }
                                    if (leadRow.GST_3.ToLower() == "yes")
                                    {
                                        ser.GST = true;
                                    }
                                    else
                                    {
                                        ser.GST = false;
                                    }

                                }
                            }
                            db.SaveChanges();

                            leadRow.Errors = null;
                            leadRow.FileName = filename;
                            db.Leads_Import_Format.Add(leadRow);
                            db.SaveChanges();
                            success++;

                            //if (leadRow.FirstName != "" && leadRow.LastName != "" && leadRow.ContactNo != "")
                            //{
                            //    Lead lead = new Lead();
                            //    TU.Name = a.Name;
                            //    TU.Address = a.Address;
                            //    TU.ContactNo = a.ContactNo;
                            //    db.Users.Add(TU);

                            //    db.SaveChanges();



                            //}
                            //else
                            //{
                            //    data.Add("<ul>");
                            //    if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                            //    if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                            //    if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                            //    data.Add("</ul>");
                            //    data.ToArray();
                            //    return Json(data, JsonRequestBehavior.AllowGet);
                            //}
                        }

                        catch (DbEntityValidationException ex)
                        {
                            errors++;
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {
                                    leadRow.Errors += " Poperty: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage;
                                    //Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }

                        catch (Exception ex)
                        {
                            errors++;
                            leadRow.Errors += " "+ex.Message;
                        }
                    }
                    //deleting excel file from folder  
                    //if ((System.IO.File.Exists(pathToExcelFile)))
                    //{
                    //    System.IO.File.Delete(pathToExcelFile);
                    //}
                    TempData["Message"] = success.ToString() + " Records Uploaded! " + errors.ToString() + " Records Failed!";
                    TempData["FileName"] = filename;

                    //ViewBag.Message = success.ToString() + " Records Uploaded! " + errors.ToString() + " Records Failed!";
                    //ViewBag.FileName = filename;
                    //return Json("success", JsonRequestBehavior.AllowGet);
                    return RedirectToAction("ImportLeads");
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: Leads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lead lead = db.Leads.Find(id);
            if (lead == null)
            {
                return HttpNotFound();
            }
            LeadViewModel leadViewModel = new LeadViewModel();
            leadViewModel.lead = lead;
            leadViewModel.leadTelNos = lead.Lead_Tel_No_Mapping.ToList();
            leadViewModel.leadCellNos = lead.Lead_Cell_No_Mapping.ToList();
            leadViewModel.leadEmails = lead.Lead_Email_Mapping.ToList();
            leadViewModel.leadConsultPersons = lead.Lead_Consult_Person.ToList();

            leadViewModel.ServiceGroups = db.ServiceGroups.Select(x => new SelectListItem
            {
                Value = x.ServiceGroupID.ToString(),
                Text = x.ServiceGroup1
            });
            leadViewModel.Services = db.Services.Select(x => new SelectListItem
            {
                Value = x.ServiceID.ToString(),
                Text = x.ServiceName
            });
            leadViewModel.FrequencyOfServices = db.FrequencyOfServices.Select(x => new SelectListItem
            {
                Value = x.FrequencyOfServiceID.ToString(),
                Text = x.FrequencyOfService1
            });


            List<Lead_Consult_Person> consultPersons = db.Lead_Consult_Person.Where(x => x.LeadID == id).ToList();
            List<Lead_Consult_Person_Contact_No_Mapping> consultPersonsContactNos = new List<Lead_Consult_Person_Contact_No_Mapping>();
            for (int i = 0; i < consultPersons.Count; i++)
            {
                int consultPersonsID = consultPersons[i].Lead_Consult_Person_ID;
                consultPersonsContactNos.AddRange(db.Lead_Consult_Person_Contact_No_Mapping.Where(x => x.Lead_Consult_Person_ID == consultPersonsID));
            }
            leadViewModel.leadConsultPersonContactNos = consultPersonsContactNos;

            List<Lead_Consult_Person_Email_Mapping> consultPersonsEmails = new List<Lead_Consult_Person_Email_Mapping>();
            for (int i = 0; i < consultPersons.Count; i++)
            {
                int consultPersonsID = consultPersons[i].Lead_Consult_Person_ID;
                consultPersonsEmails.AddRange(db.Lead_Consult_Person_Email_Mapping.Where(x => x.Lead_Consult_Person_ID == consultPersonsID));
            }
            leadViewModel.leadConsultPersonEmails = consultPersonsEmails;

            leadViewModel.leadServices = lead.Lead_Services_Mapping.ToList();
            return View(leadViewModel);
        }

        // GET: Leads/Create
        public ActionResult Create()

        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            if (!watch.IsRunning)
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
            }
            //NotificationHub.BroadcastNotification("create");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States.OrderBy( o => o.State1), "StateID", "State1", 2);
            var locations = db.Locations;
            ViewBag.LocationID = new SelectList(locations, "LocationID", "LocationName");
            //Dictionary<string, string> c = new Dictionary<string, string>();

            Dictionary<int, int> LocationsIDBranchesIDPairs = locations.ToDictionary(g => g.LocationID, g => g.BranchID);
            ViewBag.LocationsIDBranchesIDPairs = LocationsIDBranchesIDPairs;

            if (Int32.Parse(Session["UserLoginTypeID"].ToString()) == db.UserLoginTypes.Where(x => x.UserLoginTypeName == "TeleCaller").First().UserLoginTypeID)
            {

                ViewBag.TeleCallerID = new SelectList(db.UserLogins.Where(x => x.UserLoginTypeID == 2), "UserID", "UserName", Int32.Parse(Session["UserID"].ToString()));
            }
            else {

                ViewBag.TeleCallerID = new SelectList(db.UserLogins.Where(x => x.UserLoginTypeID == 2), "UserID", "UserName");
            }
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices.OrderBy( x => x.Order), "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.LeadStatusID = new SelectList(db.LeadStatuses, "StatusID", "Status");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            ViewBag.SourceID = new SelectList(db.Sources.Where(x => x.ParentSourceID == null), "SourceID", "Source1");
            ViewBag.SubSourceID = new SelectList(db.Sources.Where(x => x.ParentSourceID != null), "SourceID", "Source1");
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1");
            ViewBag.Premises_Area_ID = new SelectList(db.Premises_Area_Master, "Premises_Area_ID", "Premises_Area");
            ViewBag.VisitAllocate = new SelectList(db.Employees, "EmployeeID", "Name");
            ViewBag.IndustryID = new SelectList(db.Industries, "IndustryID", "IndustryName");
            watch.Stop();
            long totalTime;
            if (TempData["TimeTaken"] != null)
            {
                totalTime = Int64.Parse(TempData["TimeTaken"].ToString()) + watch.ElapsedMilliseconds;
            }
            else
            {
                totalTime = watch.ElapsedMilliseconds;
            }
            ViewBag.TimeTaken = totalTime;
            return View();
        }

        // POST: Leads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LeadID,LeadDate,Title,CustomerName,FirstName,LastName,TelNo,CellNo,Pincode,CityID,StateID,EmailID,SourceID,SubSourceID,TypeOfPremisesID,TeleCallerID,BranchID,VisitAllocate,VisitDateTime,VisitReport,RATE,TAX,FinalRate,CustomerPriority,NextFollowUpDateTime,SpecialInstructions,FollowUpDetails,LeadStatusID,LocationID,AddressLine1,AddressLine2,AddressLine3,NoOfWings,NoOfFloors,NoOfFlats,ConsultPerson,ConsultPersonDesignation,Premises_Area_ID,IndustryID,ServiceDate,ServiceTime,VisitTime,PremisesAppSqFtArea,OtherDetails")] Lead lead, String[] ServiceGroupID, String[] ServiceID, String[] FrequencyOfServiceID, String[] Qty, String[] GST, String[] Tax, String[] Rate, String[] FinalRatePerService,String[] ConsultPersonName, String[] Designation, String[] ConsultContactNumberHidden, String[] ConsultEmailHidden, String[] Telno, String[] CellNo, String[] EmailID, string VisitTime, string ServiceTime, string NextFollowUpTime, string SubmitType)
        {
            string errors = "";
            if (!watch.IsRunning)
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
            }
            //Save lead first
            try
            {
                //throw new Exception("Parameter cannot be null");
                if (!String.IsNullOrEmpty(VisitTime))
                {
                    lead.VisitTime = TimeSpan.Parse((DateTime.Parse(VisitTime)).ToString("HH:mm"));
                    lead.VisitDateTime += lead.VisitTime;
                }
                if (!String.IsNullOrEmpty(ServiceTime))
                {
                    lead.ServiceTime = TimeSpan.Parse((DateTime.Parse(ServiceTime)).ToString("HH:mm"));
                    lead.ServiceDate += lead.ServiceTime;
                }
                if (!String.IsNullOrEmpty(NextFollowUpTime))
                {
                    lead.NextFollowUpTime = TimeSpan.Parse((DateTime.Parse(NextFollowUpTime)).ToString("HH:mm"));
                    lead.NextFollowUpDateTime += lead.NextFollowUpTime;
                }
                ModelState.Remove("VisitTime");
                ModelState.Remove("ServiceTime");
                ModelState.Remove("NextFollowUpTime");
                
                if (ModelState.IsValid)
                {
                    lead.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                    if (lead.Title != "M/S.")
                    {
                        lead.CustomerName = null;
                    }
                    else
                    {
                        lead.FirstName = null;
                        lead.LastName = null;
                    }
                    lead.CreatedBy = Convert.ToInt32(Session["UserID"]);


                    db.Leads.Add(lead);
                    db.DbccCheckIdent<Lead>(db.Leads.Max(p => p.LeadID));
                    db.SaveChanges();

                    int leadID = lead.LeadID;
                    //Saved first TelNo in Leads table next array in mapping table
                    try
                    {
                        //throw new Exception("Parameter cannot be null");
                        for (int i = 1; i < Telno.Length; i++)
                        {
                            Lead_Tel_No_Mapping leadTelNoMapping = new Lead_Tel_No_Mapping();
                            leadTelNoMapping.LeadID = leadID;
                            leadTelNoMapping.Lead_Tel_No = Telno[i];
                            db.Lead_Tel_No_Mapping.Add(leadTelNoMapping);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex) {

                        errors += " Lead Created with Lead ID: "+lead.LeadID+" , Error saving additional Tel Numbers! : " + ex.Message ;
                        //return RedirectToAction("Create");
                    }
                    
                    
                    try
                    {
                        //Saved first CellNo in Leads table next array in mapping table
                        for (int i = 1; i < CellNo.Length; i++)
                        {
                            Lead_Cell_No_Mapping leadCellNoMapping = new Lead_Cell_No_Mapping();
                            leadCellNoMapping.LeadID = leadID;
                            leadCellNoMapping.Lead_Cell_No = CellNo[i];
                            db.Lead_Cell_No_Mapping.Add(leadCellNoMapping);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        errors += " Lead Created with Lead ID: " + lead.LeadID + " , Error saving additional Cell Numbers! : " + ex.Message;
                        //return RedirectToAction("Create");
                    }
                    try {
                        for (int i = 1; i < EmailID.Length; i++)
                        {
                            Lead_Email_Mapping leadEmailMapping = new Lead_Email_Mapping();
                            leadEmailMapping.LeadID = leadID;
                            leadEmailMapping.Lead_Email = EmailID[i];
                            db.Lead_Email_Mapping.Add(leadEmailMapping);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        errors += " Lead Created with Lead ID: " + lead.LeadID + " , Error saving additional Emails! : " + ex.Message;
                        //return RedirectToAction("Create");
                    }

                    try {
                        for (int i = 0; i < ServiceGroupID.Length; i++)
                        {
                            int outServiceGroupID;
                            if (Int32.TryParse(ServiceGroupID[i], out outServiceGroupID))
                            {
                                Lead_Services_Mapping leadServicesMapping = new Lead_Services_Mapping();
                                leadServicesMapping.LeadID = leadID;
                                leadServicesMapping.ServiceGroupID = outServiceGroupID;
                                int outServiceID;
                                if (Int32.TryParse(ServiceID[i], out outServiceID))
                                    leadServicesMapping.ServiceID = outServiceID;
                                else
                                    leadServicesMapping.ServiceID = null;
                                //leadServicesMapping.ServiceID = Int32.Parse(ServiceID[i]);
                                int outFrequency;
                                if (Int32.TryParse(FrequencyOfServiceID[i], out outFrequency))
                                    leadServicesMapping.FrequencyOfServiceID = outFrequency;
                                else
                                    leadServicesMapping.FrequencyOfServiceID = null;
                                short outQty;
                                if (Int16.TryParse(Qty[i], out outQty))
                                    leadServicesMapping.Qty = outQty;
                                else
                                    leadServicesMapping.Qty = null;
                                bool outGST;
                                if (Boolean.TryParse(GST[i], out outGST))
                                    leadServicesMapping.GST = outGST;
                                else
                                    leadServicesMapping.GST = null;
                                Decimal outDecimalTax, outDecimalRate, outFinalRatePerService;
                                if (Decimal.TryParse(Tax[i], out outDecimalTax))
                                    leadServicesMapping.Tax = outDecimalTax;
                                else
                                    leadServicesMapping.Tax = null;
                                if (Decimal.TryParse(Rate[i], out outDecimalRate))
                                    leadServicesMapping.Rate = outDecimalRate;
                                else
                                    leadServicesMapping.Rate = null;
                                if (Decimal.TryParse(FinalRatePerService[i], out outFinalRatePerService))
                                    leadServicesMapping.FinalRatePerService = outFinalRatePerService;
                                else
                                    leadServicesMapping.FinalRatePerService = null;

                                leadServicesMapping.ServiceStartDate = lead.ServiceDate;
                                db.Lead_Services_Mapping.Add(leadServicesMapping);
                                db.SaveChanges();
                                if (lead.LeadStatusID == db.LeadStatuses.Where(s => s.Status == "Done").FirstOrDefault().StatusID)
                                {
                                    try
                                    {
                                        CreateServicings(leadServicesMapping);
                                    }
                                    catch (Exception e)
                                    {
                                        string ex = e.Message;
                                    }
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        errors += " Lead Created with Lead ID: " + lead.LeadID + " , Error saving Services Required! : " + ex.Message;
                        //return RedirectToAction("Create");
                    }

                    try {
                        for (int i = 1; i < ConsultPersonName.Length; i++)
                        {                            
                            try
                            {
                                Lead_Consult_Person leadConsultPerson = new Lead_Consult_Person();
                                leadConsultPerson.LeadID = leadID;
                                leadConsultPerson.PersonaName = ConsultPersonName[i];
                                leadConsultPerson.Designation = Designation[i];
                                //leadConsultPerson.ContactNo = ConsultContactNumber[i];
                                db.Lead_Consult_Person.Add(leadConsultPerson);
                                db.SaveChanges();
                                int consultPerID = leadConsultPerson.Lead_Consult_Person_ID;
                                try {
                                    string[] contacts = ConsultContactNumberHidden[i].Split('|');
                                    for (int j = 0; j < contacts.Length; j++)
                                    {
                                        Lead_Consult_Person_Contact_No_Mapping mapContact = new Lead_Consult_Person_Contact_No_Mapping();
                                        mapContact.Lead_Consult_Person_ID = consultPerID;
                                        mapContact.Lead_Consult_Person_Contact_No = contacts[j];
                                        db.Lead_Consult_Person_Contact_No_Mapping.Add(mapContact);
                                        db.SaveChanges();
                                        //mapContact.Lead_Consult_Person_Contact_No = ConsultContactNumber[i][j];
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errors += " Lead Created with Lead ID: " + lead.LeadID + " , Error saving Contact for Consult Person: " + ConsultPersonName[i] + " " + ex.Message;
                                    //return RedirectToAction("Create");
                                }
                                try {
                                    string[] emails = ConsultEmailHidden[i].Split('|');
                                    for (int j = 0; j < emails.Length; j++)
                                    {
                                        Lead_Consult_Person_Email_Mapping mapContact = new Lead_Consult_Person_Email_Mapping();
                                        mapContact.Lead_Consult_Person_ID = consultPerID;
                                        mapContact.Lead_Consult_Person_Email = emails[j];
                                        db.Lead_Consult_Person_Email_Mapping.Add(mapContact);
                                        db.SaveChanges();
                                        //mapContact.Lead_Consult_Person_Contact_No = ConsultContactNumber[i][j];
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errors += " Lead Created with Lead ID: " + lead.LeadID + " , Error saving Emails for Consult Person: " + ConsultPersonName[i] + " " + ex.Message;
                                    //return RedirectToAction("Create");
                                }

                                
                            }
                            catch (Exception ex)
                            {
                                errors += " Lead Created with Lead ID: " + lead.LeadID + " , Error saving Additional Consult Person: " + ConsultPersonName[i]+ " " + ex.Message;
                                //return RedirectToAction("Create");
                            }
                            
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        errors += " Lead Created with Lead ID: " + lead.LeadID + " , Error saving Additional Consult Persons! : " + ex.Message;
                        //return RedirectToAction("Create");
                    }
                    // ConsultPerson started from 1 because first div is blank

                    if (lead.LeadStatus.Status == "Done" && db.Collection_Entry.Where(c => c.LeadID == leadID).Count() == 0)
                    {
                        CreateCollectionEntry(lead);
                    }
                    try {

                        var nc = new NotificationComponent();
                        //var result = nc.AddLeadNotificationAsync(lead);
                        //var result = Task.Run(() => nc.AddLeadNotificationAsync(lead));
                        var result = NotificationComponent.AddLeadNotificationAsync(lead);
                    }
                    catch (Exception ex)
                    {
                        errors += " Lead Created with Lead ID: " + lead.LeadID + " , Error Creating NOTIFICATION! : " + ex.Message;
                        //return RedirectToAction("Create");
                    }
                    watch.Stop();
                    TempData["TimeTaken"] = watch.ElapsedMilliseconds;
                    TempData["ErrorMessage"] = errors;
                    if (lead.LeadStatusID == db.LeadStatuses.Where(s => s.Status == "Done").FirstOrDefault().StatusID) {

                        return Redirect("/ConfirmedLeadServices?LeadID="+ lead.LeadID.ToString());
                    }
                    if (SubmitType == "Save")
                    {
                        //SendMessage("New object added");
                        return RedirectToAction("Index");
                    }
                    else if (SubmitType == "Save & Create New Lead")
                    {
                        //SendMessage("New object added");
                        return RedirectToAction("Create");
                    }
                    return RedirectToAction("Index");
                }

                ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", lead.BranchID);
                ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", lead.CityID);
                ViewBag.StateID = new SelectList(db.Cities, "StateID", "State1", lead.StateID);
                ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
                ViewBag.TeleCallerID = new SelectList(db.Employees, "EmployeeID", "Name", lead.TeleCallerID);
                ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1");
                ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
                ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
                ViewBag.LeadStatusID = new SelectList(db.LeadStatuses, "StatusID", "Status", lead.LeadStatusID);
                ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1", lead.SourceID);
                ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1", lead.SubSourceID);
                ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1", lead.TypeOfPremisesID);
                ViewBag.Premises_Area_ID = new SelectList(db.Premises_Area_Master, "Premises_Area_ID", "Premises_Area", lead.Premises_Area_ID);
                ViewBag.VisitAllocate = new SelectList(db.Employees, "EmployeeID", "Name", lead.VisitAllocate);
                ViewBag.IndustryID = new SelectList(db.Industries, "IndustryID", "IndustryName");
                return View(lead);
            }
            catch (Exception ex) {
                TempData["ErrorMessage"] = "Error Creating Lead, Please check the Data enrtered! : " + ex.Message;
                return RedirectToAction("Create");
            }
        }
        [HttpPost]
        public ActionResult GetServices(string ServiceGroupID)
        {
            int GroupID;
            List<SelectListItem> servicesList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(ServiceGroupID))
            {
                GroupID = Convert.ToInt32(ServiceGroupID);
                List<Service> services = db.Services.Where(x => x.ServiceGroupID == GroupID).ToList();
                services.ForEach(x =>
                {
                    servicesList.Add(new SelectListItem { Text = x.ServiceName, Value = x.ServiceID.ToString() });
                });
            }
            return Json(servicesList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetServiceSACAndShortServiceScope(string ServiceID)
        {
            int intServiceID;
            string SAC="";List<Short_Service_Scope_Master> ShortServiceScopes = new List<Short_Service_Scope_Master>();
            List<int> SSSIDs = new List<int>(); 
            List<string> SSSTexts = new List<string>();
            //List<SelectListItem> servicesList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(ServiceID))
            {
                intServiceID = Convert.ToInt32(ServiceID);
                SAC = db.Services.Where(x => x.ServiceID == intServiceID).FirstOrDefault().SACCode;
                ShortServiceScopes = db.Short_Service_Scope_Master.Where(s => s.ServiceID == intServiceID).ToList();
                if (ShortServiceScopes.Count > 0)
                {
                    foreach (var item in ShortServiceScopes) {
                        SSSIDs.Add(item.Short_Service_Scope_ID);
                        SSSTexts.Add(item.Short_Service_Scope);
                    }
                    //ShortServiceScopes = SSCs.Select(s => s.Short_Service_Scope_ID).ToArray();
                }
                else {
                    //SSC = 0;
                }
                


            }
            return Json(new { SAC , SSSIDs, SSSTexts }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetServiceMapped(string LeadID)
        {
            int intLeadID;

            if (!string.IsNullOrEmpty(LeadID))
            {
                intLeadID = Convert.ToInt32(LeadID);
                var services = db.Lead_Services_Mapping.Where(x => x.LeadID == intLeadID).ToList();
                
                if (services.Count == 0) {
                    return Json("No Services Found", JsonRequestBehavior.AllowGet);
                }
                List<Lead_Services_Mapping> servicesList = new List<Lead_Services_Mapping>();
                services.ForEach(x =>
                {
                    servicesList.Add(new Lead_Services_Mapping
                    {
                        Lead_Services_Mapping_ID = x.Lead_Services_Mapping_ID,
                        LeadID = x.LeadID,
                        ServiceID = x.ServiceID,
                        ServiceGroupID = x.ServiceGroupID,
                        FrequencyOfServiceID = x.FrequencyOfServiceID,
                        Rate = x.Rate,
                        GST = x.GST,
                        Qty = x.Qty,
                        Tax = x.Tax,
                        FinalRatePerService = x.FinalRatePerService,

                        PeriodsOfContractID = x.PeriodsOfContractID,
                        ServiceStartDate = x.ServiceStartDate,
                        POCStartDate = x.POCStartDate,
                        POCEndDate = x.POCEndDate
                        

                    });
                });
                return Json(servicesList, JsonRequestBehavior.AllowGet);
            }
            else {
                
                return Json("Wrong Lead ID", JsonRequestBehavior.AllowGet);
            }
            
        }
        [HttpPost]
        public ActionResult GetSingleServiceTaxAfterGSTSelection(string ServiceID, string Rate, string Qty, string GSTapplicable)
        {
            decimal? calculatedTax = 0;
            int serviceID = Int32.Parse(ServiceID);
            byte? GST = db.Services.Find(serviceID).GST;

            if (Qty == "") {
                Qty = "1";
            }
            decimal decOutQty;
            Decimal.TryParse(Qty, out decOutQty);
            if (GST != null)
            {
                if (GSTapplicable == "True")
                {
                    calculatedTax = ((Decimal.Parse(Rate)) * GST / 100) * Decimal.Parse(Qty);
                }
                else
                {
                    calculatedTax = 0;
                }
                
            }
            //List<SelectListItem> servicesList = new List<SelectListItem>();
            //if (!string.IsNullOrEmpty(ServiceGroupID))
            //{
            //    GroupID = Convert.ToInt32(ServiceGroupID);
            //    List<Service> services = db.Services.Where(x => x.ServiceGroupID == GroupID).ToList();
            //    services.ForEach(x =>
            //    {
            //        servicesList.Add(new SelectListItem { Text = x.ServiceName, Value = x.ServiceID.ToString() });
            //    });
            //}
            return Json(calculatedTax, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeadDataForEstimates(string LeadID)
        {
            int intLeadID;

            if (!string.IsNullOrEmpty(LeadID))
            {
                intLeadID = Convert.ToInt32(LeadID);

                var lead = db.Leads.Where(x => x.LeadID == intLeadID).Select(x => new { x.ConsultPerson, x.ConsultPersonDesignation, x.AddressLine1, x.AddressLine2, x.AddressLine3, x.City.CityName, x.Location.LocationName, x.State.State1, x.Branch.BranchName, x.Pincode, x.TelNo, x.CellNo, x.EmailID, Premise = x.PremisesType.PremisesType1, VisitAllocated = x.Employee.Name, VisitAllocatedDesignation = x.Employee.EmployeeDesignation.Designation, VisitAllocatedCellNumber = x.Employee.ContactNumber, TeleCaller = x.UserLogin1.UserName, x.Title, x.CustomerName, x.FirstName, x.LastName, x.SourceID, x.SubSourceID, x.BranchID, x.IndustryID, x.LocationID, x.CityID, x.StateID, ExtraTelNos = x.Lead_Tel_No_Mapping.Select(o => o.Lead_Tel_No), ExtraCellNos = x.Lead_Cell_No_Mapping.Select(o => o.Lead_Cell_No), ExtraEmails = x.Lead_Email_Mapping.Select(o => o.Lead_Email), PremisesType = x.PremisesType.PremisesType1, ServiceStartDate = x.ServiceDate }).FirstOrDefault();


                return Json(new { lead }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Customer ID", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetLeadCustIDAndContractNo(string LeadID)
        {
            int intLeadID;

            if (!string.IsNullOrEmpty(LeadID))
            {
                intLeadID = Convert.ToInt32(LeadID);

                var CustomerID = db.Customers.Where(c => c.LeadID == intLeadID).FirstOrDefault().CustomerID;

                //var lead = db.Leads.Where(x => x.LeadID == intLeadID).Select(x => new { x.ConsultPerson, x.ConsultPersonDesignation, x.AddressLine1, x.AddressLine2, x.AddressLine3, x.City.CityName, x.Location.LocationName, x.State.State1, x.Branch.BranchName, x.Pincode, x.TelNo, x.CellNo, x.EmailID, Premise = x.PremisesType.PremisesType1, VisitAllocated = x.Employee.Name, VisitAllocatedDesignation = x.Employee.EmployeeDesignation.Designation, VisitAllocatedCellNumber = x.Employee.ContactNumber, TeleCaller = x.UserLogin1.UserName, x.Title, x.CustomerName, x.FirstName, x.LastName, x.SourceID, x.SubSourceID, x.BranchID, x.IndustryID, x.LocationID, x.CityID, x.StateID, ExtraTelNos = x.Lead_Tel_No_Mapping.Select(o => o.Lead_Tel_No), ExtraCellNos = x.Lead_Cell_No_Mapping.Select(o => o.Lead_Cell_No), ExtraEmails = x.Lead_Email_Mapping.Select(o => o.Lead_Email), PremisesType = x.PremisesType.PremisesType1 }).FirstOrDefault();
                var branchShortCode = db.Customers.Where(c => c.LeadID == intLeadID).FirstOrDefault().Branch.BranchShortCode;

                DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                var year = today.ToString("yy");
                string contNo = "";
                var contractToIncreaseAsList = db.Contracts.OrderByDescending(e => e.ContractID).Take(1).Select(e => e.ContractNo).ToList();
                if (contractToIncreaseAsList.Count == 0)
                {
                    contNo = "C/" + branchShortCode + year + "0001";
                }
                else
                {
                    string string1;
                    if (contractToIncreaseAsList.First().Contains("C/"))
                    {
                        string1 = contractToIncreaseAsList.First().Substring(0, 7);
                    }
                    else
                    {
                        string1 = contractToIncreaseAsList.First().Substring(0, 5);
                    }
                    //string string1 = contractToIncreaseAsList.First().Substring(0, 5);
                    //string string2 = custNumberToIncreaseAsList.First().Substring(5);
                    string numberAsString = contractToIncreaseAsList.First().Replace(string1, "");
                    string number = String.Format("{0:D4}", Int32.Parse(numberAsString) + 1);
                    contNo = "C/" + branchShortCode + year + number;
                    //referenceNumber = branchShortCode + year + month + referenceNumberToIncreaseAsList.First(). + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
                }
                var ServiceStartDate = db.Leads.Find(intLeadID).ServiceDate;
                var AppSqFt = db.Leads.Find(intLeadID).PremisesAppSqFtArea;
                var Premises_Area_ID = db.Leads.Find(intLeadID).Premises_Area_ID;
                string ServiceStartDateString = "";
                if (ServiceStartDate != null) {
                    ServiceStartDateString = ((DateTime)ServiceStartDate).ToString("dd/MM/yyyy");
                }

                return Json( new { CustomerID, contNo, ServiceStartDateString, AppSqFt, Premises_Area_ID } , JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Lead ID", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CheckDupliateLeads(string title, string FirstName, string LastName, string CustomerName, string[] CellNo) 
        
        {
            if (!String.IsNullOrEmpty(title) && ((!String.IsNullOrEmpty(FirstName) && CellNo.Length > 0) || (!String.IsNullOrEmpty(CustomerName) && CellNo.Length > 0))) {
                if (title == "M/S.")
                {
                    var leadsExisting = db.Leads.Where(i => !String.IsNullOrEmpty(i.CustomerName) && i.Title == title && i.CustomerName == CustomerName);
                    
                    if (leadsExisting != null)
                    {
                        foreach (var lead in leadsExisting) {
                            var cellNosMappings = db.Lead_Cell_No_Mapping.Where(i => i.LeadID == lead.LeadID).Select(i => i.Lead_Cell_No).ToList();
                            cellNosMappings.Add(db.Leads.Find(lead.LeadID).CellNo);
                            foreach (var cell in CellNo)
                            {
                                if (!String.IsNullOrEmpty(lead.CellNo) && cell == lead.CellNo)
                                {
                                    return Json("1", JsonRequestBehavior.AllowGet);
                                }
                                if (cellNosMappings.Contains(cell))
                                {
                                    return Json("1", JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                          
                    }
                }
                else
                {
                    var leadsExisting = db.Leads.Where(i => i.Title == title && i.FirstName == FirstName && i.LastName == LastName);
                    if (leadsExisting != null)
                    {
                        foreach (var lead in leadsExisting) {
                            var cellNosMappings = db.Lead_Cell_No_Mapping.Where(i => i.LeadID == lead.LeadID).Select(i => i.Lead_Cell_No).ToList();
                            cellNosMappings.Add(db.Leads.Find(lead.LeadID).CellNo);
                            foreach (var cell in CellNo)
                            {
                                if (!String.IsNullOrEmpty(lead.CellNo) && cell == lead.CellNo)
                                {
                                    return Json("1", JsonRequestBehavior.AllowGet);
                                }
                                if (cellNosMappings.Contains(cell))
                                {
                                    return Json("1", JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }

                }
            }
                
            
            return Json("0", JsonRequestBehavior.AllowGet);

            

        }
        // GET: Leads/Edit/5
        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lead lead = db.Leads.Find(id);
            if (lead == null)
            {
                return HttpNotFound();
            }
            

            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", lead.BranchID);
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", lead.CityID);
            ViewBag.LeadStatusID = new SelectList(db.LeadStatuses, "StatusID", "Status", lead.LeadStatusID);
            ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1", lead.SourceID);
            ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1", lead.SubSourceID);
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1", lead.TypeOfPremisesID);

            
            
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1", lead.StateID);
            //ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName", lead.LocationID);
            var locations = db.Locations;
            ViewBag.LocationID = new SelectList(locations, "LocationID", "LocationName", lead.LocationID);
            //Dictionary<string, string> c = new Dictionary<string, string>();

            Dictionary<int, int> LocationsIDBranchesIDPairs = locations.ToDictionary(g => g.LocationID, g => g.BranchID);
            ViewBag.LocationsIDBranchesIDPairs = LocationsIDBranchesIDPairs;
            if (Int32.Parse(Session["UserLoginTypeID"].ToString()) == db.UserLoginTypes.Where(x => x.UserLoginTypeName == "TeleCaller").First().UserLoginTypeID)
            {

                ViewBag.TeleCallerID = new SelectList(db.UserLogins.Where(x => x.UserLoginTypeID == 2), "UserID", "UserName", Int32.Parse(Session["UserID"].ToString()));
            }
            else
            {

                ViewBag.TeleCallerID = new SelectList(db.UserLogins.Where(x => x.UserLoginTypeID == 2), "UserID", "UserName");
            }
            //ViewBag.TeleCallerID = new SelectList(db.Employees, "EmployeeID", "Name", lead.TeleCallerID);
            //ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1");
            //ViewBag.ServiceGroups = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            //ViewBag.ServiceGroupsList = new SelectList(db.ServiceGroups.Select(x => new { x.ServiceGroupID, x.ServiceGroup1 }), "ServiceGroupID", "ServiceGroup1");
            
            //ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            ViewBag.Premises_Area_ID = new SelectList(db.Premises_Area_Master, "Premises_Area_ID", "Premises_Area", lead.Premises_Area_ID);
            ViewBag.VisitAllocate = new SelectList(db.Employees, "EmployeeID", "Name", lead.VisitAllocate);
            ViewBag.LeadTitle = new SelectList(new[] {
                                               new { Text = "Mr.", Value = "Mr." },
                                               new { Text = "Ms.", Value = "Ms."},
                                               new { Text = "Mrs.", Value = "Mrs."},
                                               new { Text = "M/S.", Value = "M/S."}
                                            }, "Value", "Text", lead.Title);

            ViewBag.IndustryID = new SelectList(db.Industries, "IndustryID", "IndustryName",lead.IndustryID);

            LeadViewModel leadViewModel = new LeadViewModel();
            leadViewModel.lead = lead;
            leadViewModel.leadTelNos = lead.Lead_Tel_No_Mapping.ToList();
            leadViewModel.leadCellNos = lead.Lead_Cell_No_Mapping.ToList();
            leadViewModel.leadEmails = lead.Lead_Email_Mapping.ToList();
            leadViewModel.leadConsultPersons = lead.Lead_Consult_Person.ToList();

            leadViewModel.ServiceGroups = db.ServiceGroups.Select(x => new SelectListItem
            {
                Value = x.ServiceGroupID.ToString(),
                Text = x.ServiceGroup1
            });
            leadViewModel.Services = db.Services.Select(x => new SelectListItem
            {
                Value = x.ServiceID.ToString(),
                Text = x.ServiceName
            });
            leadViewModel.FrequencyOfServices = db.FrequencyOfServices.OrderBy(x => x.Order).Select(x => new SelectListItem
            {
                Value = x.FrequencyOfServiceID.ToString(),
                Text = x.FrequencyOfService1
            });


            List<Lead_Consult_Person> consultPersons = db.Lead_Consult_Person.Where(x => x.LeadID == id).ToList();
            List<Lead_Consult_Person_Contact_No_Mapping> consultPersonsContactNos = new List<Lead_Consult_Person_Contact_No_Mapping>();
            for (int i = 0; i < consultPersons.Count; i++)
            {
                int consultPersonsID = consultPersons[i].Lead_Consult_Person_ID;
                consultPersonsContactNos.AddRange(db.Lead_Consult_Person_Contact_No_Mapping.Where(x => x.Lead_Consult_Person_ID == consultPersonsID));
            }
            leadViewModel.leadConsultPersonContactNos = consultPersonsContactNos;

            List<Lead_Consult_Person_Email_Mapping> consultPersonsEmails = new List<Lead_Consult_Person_Email_Mapping>();
            for (int i = 0; i < consultPersons.Count; i++)
            {
                int consultPersonsID = consultPersons[i].Lead_Consult_Person_ID;
                consultPersonsEmails.AddRange(db.Lead_Consult_Person_Email_Mapping.Where(x => x.Lead_Consult_Person_ID == consultPersonsID));
            }
            leadViewModel.leadConsultPersonEmails = consultPersonsEmails;

            leadViewModel.leadServices = lead.Lead_Services_Mapping.ToList();
            if (leadViewModel.leadServices.Count == 0) {
                Lead_Services_Mapping leadService = new Lead_Services_Mapping();
                leadViewModel.leadServices.Add(leadService);
            }

            return View(leadViewModel);
        }

        // POST: Leads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LeadViewModel leadViewModel, string VisitTime, string ServiceTime, string NextFollowUpTime)
        {
            if (!String.IsNullOrEmpty(VisitTime))
            {
                leadViewModel.lead.VisitTime = TimeSpan.Parse((DateTime.Parse(VisitTime)).ToString("HH:mm"));
                leadViewModel.lead.VisitDateTime += leadViewModel.lead.VisitTime;
            }
            if (!String.IsNullOrEmpty(ServiceTime))
            {
                leadViewModel.lead.ServiceTime = TimeSpan.Parse((DateTime.Parse(ServiceTime)).ToString("HH:mm"));
                leadViewModel.lead.ServiceDate += leadViewModel.lead.ServiceTime;
            }
            if (!String.IsNullOrEmpty(NextFollowUpTime))
            {
                leadViewModel.lead.NextFollowUpTime = TimeSpan.Parse((DateTime.Parse(NextFollowUpTime)).ToString("HH:mm"));
                leadViewModel.lead.NextFollowUpDateTime += leadViewModel.lead.NextFollowUpTime;
            }

            ModelState.Remove("lead.VisitTime");
            ModelState.Remove("lead.ServiceTime");
            ModelState.Remove("lead.NextFollowUpTime");
            if (leadViewModel.leadServices.Count == 1 && leadViewModel.leadServices.ElementAt(0).ServiceGroupID == 0) {
                leadViewModel.leadServices.RemoveAt(0);
                ModelState.Remove("leadServices[0].ServiceGroupID");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (leadViewModel.lead.Title != "M/S.")
                    {
                        leadViewModel.lead.CustomerName = null;
                    }
                    else
                    {
                        leadViewModel.lead.FirstName = null;
                        leadViewModel.lead.LastName = null;
                    }
                    leadViewModel.lead.LastUpdatedBy = Convert.ToInt32(Session["UserID"]);
                    leadViewModel.lead.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    db.Entry(leadViewModel.lead).State = EntityState.Modified;
                    db.SaveChanges();

                    for (   int i = 0; i < leadViewModel.leadTelNos.Count; i++)
                    {
                        if (leadViewModel.leadTelNos[i].Lead_Tel_No_Mapping_ID != 0)
                        {
                            if (leadViewModel.leadTelNos[i].Lead_Tel_No != null)
                            {
                                db.Lead_Tel_No_Mapping.Find(leadViewModel.leadTelNos[i].Lead_Tel_No_Mapping_ID).Lead_Tel_No = leadViewModel.leadTelNos[i].Lead_Tel_No;
                            }
                            else
                            {
                                //db.Lead_Tel_No_Mapping.Remove(leadViewModel.leadTelNos[i]);
                                db.Lead_Tel_No_Mapping.Remove(db.Lead_Tel_No_Mapping.Find(leadViewModel.leadTelNos[i].Lead_Tel_No_Mapping_ID));
                            }
                        }
                        else
                        {
                            if (leadViewModel.leadTelNos[i].Lead_Tel_No != null) {
                                leadViewModel.leadTelNos[i].LeadID = leadViewModel.lead.LeadID;
                                db.Lead_Tel_No_Mapping.Add(leadViewModel.leadTelNos[i]);
                            }
                                
                        }
                    }
                    db.SaveChanges();
                    for (int i = 0; i < leadViewModel.leadCellNos.Count; i++)
                    {
                        if (leadViewModel.leadCellNos[i].Lead_Cell_No_Mapping_ID != 0)
                        {
                            if (leadViewModel.leadCellNos[i].Lead_Cell_No != null)
                            {
                                db.Lead_Cell_No_Mapping.Find(leadViewModel.leadCellNos[i].Lead_Cell_No_Mapping_ID).Lead_Cell_No = leadViewModel.leadCellNos[i].Lead_Cell_No;
                            }
                            else
                            {
                                db.Lead_Cell_No_Mapping.Remove(db.Lead_Cell_No_Mapping.Find(leadViewModel.leadCellNos[i].Lead_Cell_No_Mapping_ID));
                            }
                        }
                        else
                        {
                            if (leadViewModel.leadCellNos[i].Lead_Cell_No != null) {
                                leadViewModel.leadCellNos[i].LeadID = leadViewModel.lead.LeadID;
                                db.Lead_Cell_No_Mapping.Add(leadViewModel.leadCellNos[i]);
                            }
                                
                        }
                    }
                    db.SaveChanges();
                    try
                    {
                        int intLeadID = leadViewModel.lead.LeadID;
                        var existingConsultPersonsIDs = db.Lead_Consult_Person.Where(x => x.LeadID == intLeadID).Select(x => x.Lead_Consult_Person_ID);
                        var retainedConsultPersonsIDs = leadViewModel.leadConsultPersons.Where( x => x.Lead_Consult_Person_ID != 0).Select( x => x.Lead_Consult_Person_ID);
                        var personsToDelete = existingConsultPersonsIDs.Except(retainedConsultPersonsIDs);
                        db.Lead_Consult_Person.RemoveRange(db.Lead_Consult_Person.Where(r => personsToDelete.Contains(r.Lead_Consult_Person_ID)));

                        db.Lead_Consult_Person.RemoveRange(db.Lead_Consult_Person.Where(x => x.LeadID == intLeadID).Except(leadViewModel.leadConsultPersons));
                        Lead_Consult_Person consult_Person;
                        for (int i = 0; i < leadViewModel.leadConsultPersons.Count; i++) {
                            if (leadViewModel.leadConsultPersons[i].Lead_Consult_Person_ID != 0) {
                                consult_Person = db.Lead_Consult_Person.Find(leadViewModel.leadConsultPersons[i].Lead_Consult_Person_ID);
                                consult_Person.PersonaName = leadViewModel.leadConsultPersons[i].PersonaName;
                                consult_Person.Designation = leadViewModel.leadConsultPersons[i].Designation;
                                consult_Person.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                                db.Entry(consult_Person).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                            else
                            {
                                consult_Person = new Lead_Consult_Person();
                                consult_Person.LeadID = intLeadID;
                                consult_Person.PersonaName = leadViewModel.leadConsultPersons[i].PersonaName;
                                consult_Person.Designation = leadViewModel.leadConsultPersons[i].Designation;
                                consult_Person.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                                db.Lead_Consult_Person.Add(consult_Person);
                                db.SaveChanges();
                            }

                        }


                        if (leadViewModel.lead.LeadStatusID == db.LeadStatuses.Where(s => s.Status == "Done").FirstOrDefault().StatusID )
                        {
                            if (db.Collection_Entry.Where(c => c.LeadID == leadViewModel.lead.LeadID).Count() == 0)
                            {
                                CreateCollectionEntry(leadViewModel.lead);
                            }

                        }

                        var existingLeadServicesIDs = db.Lead_Services_Mapping.Where(x => x.LeadID == intLeadID).Select(x => x.Lead_Services_Mapping_ID);
                        var retainedLeadServicesIDs = leadViewModel.leadServices.Where(x => x.Lead_Services_Mapping_ID != 0).Select(x => x.Lead_Services_Mapping_ID);
                        var LeadServicesToDelete = existingLeadServicesIDs.Except(retainedLeadServicesIDs);
                        db.Lead_Services_Mapping.RemoveRange(db.Lead_Services_Mapping.Where(r => LeadServicesToDelete.Contains(r.Lead_Services_Mapping_ID)));

                        //db.Lead_Services_Mapping.RemoveRange(db.Lead_Services_Mapping.Where(x => x.LeadID == intLeadID).Except(leadViewModel.leadServices));
                        Lead_Services_Mapping lead_service_map;
                        for (int i = 0; i < leadViewModel.leadServices.Count; i++)
                        {
                            if (leadViewModel.leadServices[i].Lead_Services_Mapping_ID != 0)
                            {
                                lead_service_map = db.Lead_Services_Mapping.Find(leadViewModel.leadServices[i].Lead_Services_Mapping_ID);
                                lead_service_map.ServiceGroupID = leadViewModel.leadServices[i].ServiceGroupID;
                                lead_service_map.ServiceID = leadViewModel.leadServices[i].ServiceID;
                                var test = leadViewModel.leadServices[i].FrequencyOfServiceID;
                                lead_service_map.FrequencyOfServiceID = leadViewModel.leadServices[i].FrequencyOfServiceID;
                                lead_service_map.Qty = leadViewModel.leadServices[i].Qty;

                                lead_service_map.GST = leadViewModel.leadServices[i].GST;
                                //Decimal outDecimalTax, outDecimalRate, outFinalRatePerService;
                                //if (Decimal.TryParse(leadViewModel.leadServices[i].Tax, out outDecimalTax))
                                //    leadServicesMapping.Tax = outDecimalTax;
                                //else
                                //    leadServicesMapping.Tax = 0;
                                //if (Decimal.TryParse(Rate[i], out outDecimalRate))
                                //    leadServicesMapping.Rate = outDecimalRate;
                                //else
                                //    leadServicesMapping.Rate = 0;
                                //if (Decimal.TryParse(FinalRatePerService[i], out outFinalRatePerService))
                                //    leadServicesMapping.FinalRatePerService = outFinalRatePerService;
                                //else
                                //    leadServicesMapping.FinalRatePerService = 0;
                                lead_service_map.Rate = leadViewModel.leadServices[i].Rate;
                                lead_service_map.Tax = leadViewModel.leadServices[i].Tax;
                                lead_service_map.FinalRatePerService = leadViewModel.leadServices[i].FinalRatePerService;
                                //int outFrequency;
                                //if (Int32.TryParse(leadViewModel.leadServices[i].FrequencyOfServiceID, out outFrequency))
                                //    leadServicesMapping.FrequencyOfServiceID = outFrequency;
                                //else
                                //    leadServicesMapping.FrequencyOfServiceID = null;
                                //short outQty;
                                //if (Int16.TryParse(Qty[i], out outQty))
                                //    leadServicesMapping.Qty = outQty;
                                //else
                                //    leadServicesMapping.Qty = 1;
                                //bool outGST;
                                //if (Boolean.TryParse(GST[i], out outGST))
                                //    leadServicesMapping.GST = outGST;
                                //else
                                //    leadServicesMapping.GST = null;
                                //Decimal outDecimalTax, outDecimalRate, outFinalRatePerService;
                                //if (Decimal.TryParse(Tax[i], out outDecimalTax))
                                //    leadServicesMapping.Tax = outDecimalTax;
                                //else
                                //    leadServicesMapping.Tax = 0;
                                //if (Decimal.TryParse(Rate[i], out outDecimalRate))
                                //    leadServicesMapping.Rate = outDecimalRate;
                                //else
                                //    leadServicesMapping.Rate = 0;
                                //if (Decimal.TryParse(FinalRatePerService[i], out outFinalRatePerService))
                                //    leadServicesMapping.FinalRatePerService = outFinalRatePerService;
                                //else
                                //    leadServicesMapping.FinalRatePerService = 0;
                                db.Entry(lead_service_map).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                            else
                            {
                                lead_service_map = new Lead_Services_Mapping();
                                lead_service_map.LeadID = intLeadID;
                                lead_service_map.ServiceGroupID = leadViewModel.leadServices[i].ServiceGroupID;
                                lead_service_map.ServiceID = leadViewModel.leadServices[i].ServiceID;
                                lead_service_map.FrequencyOfServiceID = leadViewModel.leadServices[i].FrequencyOfServiceID;
                                lead_service_map.Qty = leadViewModel.leadServices[i].Qty;
                                lead_service_map.Rate = leadViewModel.leadServices[i].Rate;
                                lead_service_map.GST = leadViewModel.leadServices[i].GST;
                                lead_service_map.Tax = leadViewModel.leadServices[i].Tax;
                                lead_service_map.FinalRatePerService = leadViewModel.leadServices[i].FinalRatePerService;
                                //db.Entry(lead_service_map).State = EntityState.Modified;
                                db.Lead_Services_Mapping.Add(lead_service_map);
                                db.SaveChanges();
                            }

                        }
                    }
                    catch (Exception ex) {
                        String s = ex.Message;
                    }
                }
                catch (Exception ex) {
                    String s = ex.Message;
                }
                if (leadViewModel.lead.LeadStatusID == db.LeadStatuses.Where(s => s.Status == "Done").FirstOrDefault().StatusID)
                {

                    return Redirect("/ConfirmedLeadServices?LeadID=" + leadViewModel.lead.LeadID.ToString());
                }
                return RedirectToAction("Index");
            }

            return RedirectToAction("Edit", new { id = leadViewModel.lead.LeadID });
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", leadViewModel.lead.BranchID);
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", leadViewModel.lead.CityID);
            ViewBag.TeleCallerID = new SelectList(db.Employees, "EmployeeID", "Name", leadViewModel.lead.TeleCallerID);
            ViewBag.LeadStatusID = new SelectList(db.LeadStatuses, "StatusID", "Status", leadViewModel.lead.LeadStatusID);
            ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1", leadViewModel.lead.SourceID);
            ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1", leadViewModel.lead.SubSourceID);
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1", leadViewModel.lead.TypeOfPremisesID);
            ViewBag.LeadTitle = new SelectList(new[] {
                                               new { Text = "Mr.", Value = "Mr." },
                                               new { Text = "Ms.", Value = "Ms."},
                                               new { Text = "Mrs.", Value = "Mrs."},
                                               new { Text = "M/S.", Value = "M/S."}
                                            }, "Value", "Text", leadViewModel.lead.Title);

            ViewBag.IndustryID = new SelectList(db.Industries, "IndustryID", "IndustryName", leadViewModel.lead.IndustryID);
            return View(leadViewModel);
        }

        public void CreateServicings(Lead_Services_Mapping map)
        {

            Servicing servicing;
            FrequencyOfService frequency = db.FrequencyOfServices.Find(map.FrequencyOfServiceID);
            DateTime system_date_of_service = (DateTime)map.ServiceStartDate;
            int noOfServicing = 1;

            var branchShortCode = db.Branches.Find(db.Leads.Find(map.LeadID).BranchID).BranchShortCode;
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            var year = today.ToString("yy");
            string no = "";
            int intNumber = 0;
            var noToIncreaseAsList = db.Servicings.Where(e => e.ServicingNo.Substring(3,2)==year && !e.ServicingNo.Contains("-")).OrderByDescending(e => e.ServicingID).Take(1).Select(e => e.ServicingNo).ToList();
            if (noToIncreaseAsList.Count == 0)
            {
                no = branchShortCode + year + "0001";
                intNumber = 1;
            }
            else
            {
                string string1 = noToIncreaseAsList.First().Substring(0, 5);
                //string string2 = custNumberToIncreaseAsList.First().Substring(5);
                string numberAsString = noToIncreaseAsList.First().Replace(string1, "");
                intNumber = Int32.Parse(numberAsString) + 1;
                string number = String.Format("{0:D4}", intNumber);
                no = branchShortCode + year + number;
                //referenceNumber = branchShortCode + year + month + referenceNumberToIncreaseAsList.First(). + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
            }

            using (SadguruCRMEntities entities = new SadguruCRMEntities())
            {
                if (frequency.Duration_Between_2_Services_Day > 0 || frequency.Duration_Between_2_Services_Month > 0 || frequency.Duration_Between_2_Services_Year > 0)
                {
                    while (system_date_of_service < map.POCEndDate)
                    {
                        servicing = new Servicing();
                        servicing.ServicingNo = no;
                        servicing.NoOfServicing = noOfServicing;
                        servicing.Servicing_Frequency_Number = (byte)noOfServicing;
                        servicing.System_Servicing_Datetime = system_date_of_service;
                        servicing.Servicing_Datetime = system_date_of_service;
                        servicing.Actual_Servicing_Datetime = null;
                        servicing.ContractID = null;
                        servicing.Contract_Services_Mapping_ID = null;
                        servicing.LeadID = map.LeadID;
                        servicing.Lead_Services_Mapping_ID = map.Lead_Services_Mapping_ID;
                        servicing.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        servicing.CreatedBy = Convert.ToInt32(Session["UserID"]);
                        //System.Diagnostics.Debug.WriteLine(system_date_of_service);

                        system_date_of_service = system_date_of_service.AddYears(frequency.Duration_Between_2_Services_Year).AddMonths(frequency.Duration_Between_2_Services_Month).AddDays(frequency.Duration_Between_2_Services_Day);
                        noOfServicing++;
                        intNumber++; 
                        no = branchShortCode + year + String.Format("{0:D4}", intNumber);

                        entities.Servicings.Add(servicing);
                    }
                    entities.SaveChanges();
                }

            }
            //for (int i = 1; i <= 1000; i++) {

            //}


        }

        public void CreateCollectionEntry(Lead lead)
        {

            try
            {
                var service_mappings_with_no_gst = db.Lead_Services_Mapping.Where(map => map.LeadID == lead.LeadID && map.GST != true);

                if (service_mappings_with_no_gst.Count() > 0) {
                    Collection_Entry collection_Entry = new Collection_Entry();
                    collection_Entry.LeadID = lead.LeadID;
                    collection_Entry.ReceivedOn = null;
                    collection_Entry.PaymentModeID = null;

                    collection_Entry.Amount = 0;
                    collection_Entry.TDSapplicable = false;
                    collection_Entry.TDSAmount = 0;
                    collection_Entry.BadDebtsAmount = null;
                    collection_Entry.ChequeNo = null;
                    collection_Entry.ChequeDate = null;
                    collection_Entry.ChequeName = null;
                    collection_Entry.BankID = null;
                    collection_Entry.DraweeName = null;
                    collection_Entry.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    collection_Entry.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    db.Collection_Entry.Add(collection_Entry);

                    db.DbccCheckIdent<Collection_Entry>(db.Collection_Entry.Max(p => p.Collection_Entry_ID));
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        // GET: Leads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lead lead = db.Leads.Find(id);
            if (lead == null)
            {
                return HttpNotFound();
            }
            LeadViewModel leadViewModel = new LeadViewModel();
            leadViewModel.lead = lead;
            leadViewModel.leadTelNos = lead.Lead_Tel_No_Mapping.ToList();
            leadViewModel.leadCellNos = lead.Lead_Cell_No_Mapping.ToList();
            leadViewModel.leadEmails = lead.Lead_Email_Mapping.ToList();
            leadViewModel.leadConsultPersons = lead.Lead_Consult_Person.ToList();

            leadViewModel.ServiceGroups = db.ServiceGroups.Select(x => new SelectListItem
            {
                Value = x.ServiceGroupID.ToString(),
                Text = x.ServiceGroup1
            });
            leadViewModel.Services = db.Services.Select(x => new SelectListItem
            {
                Value = x.ServiceID.ToString(),
                Text = x.ServiceName
            });
            leadViewModel.FrequencyOfServices = db.FrequencyOfServices.Select(x => new SelectListItem
            {
                Value = x.FrequencyOfServiceID.ToString(),
                Text = x.FrequencyOfService1
            });


            List<Lead_Consult_Person> consultPersons = db.Lead_Consult_Person.Where(x => x.LeadID == id).ToList();
            List<Lead_Consult_Person_Contact_No_Mapping> consultPersonsContactNos = new List<Lead_Consult_Person_Contact_No_Mapping>();
            for (int i = 0; i < consultPersons.Count; i++)
            {
                int consultPersonsID = consultPersons[i].Lead_Consult_Person_ID;
                consultPersonsContactNos.AddRange(db.Lead_Consult_Person_Contact_No_Mapping.Where(x => x.Lead_Consult_Person_ID == consultPersonsID));
            }
            leadViewModel.leadConsultPersonContactNos = consultPersonsContactNos;

            List<Lead_Consult_Person_Email_Mapping> consultPersonsEmails = new List<Lead_Consult_Person_Email_Mapping>();
            for (int i = 0; i < consultPersons.Count; i++)
            {
                int consultPersonsID = consultPersons[i].Lead_Consult_Person_ID;
                consultPersonsEmails.AddRange(db.Lead_Consult_Person_Email_Mapping.Where(x => x.Lead_Consult_Person_ID == consultPersonsID));
            }
            leadViewModel.leadConsultPersonEmails = consultPersonsEmails;

            leadViewModel.leadServices = lead.Lead_Services_Mapping.ToList();
            return View(leadViewModel);
            //return View(lead);
        }
        public ActionResult DeleteLead(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lead lead = db.Leads.Find(id);
            if (lead == null)
            {
                return HttpNotFound();
            }
            return View(lead);
        }

        // POST: Leads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lead lead = db.Leads.Find(id);
            db.Leads.Remove(lead);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private void SendMessage(string message)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // Get the hub context
            var context =
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            // send a message
            context.Clients.All.displayTime(message);
            watch.Stop();
            NotificationHub.BroadcastNotification(watch.ElapsedMilliseconds.ToString());
        }
        //public JsonResult GetNotificationLeads()
        //{
        //    var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        //    NotificationComponent NC = new NotificationComponent();
        //    var list = NC.GetLeads(notificationRegisterTime);
        //    //update session here for get only new added contacts (notification)
        //    Session["LastUpdate"] = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        //    return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
    }
}

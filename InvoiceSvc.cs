using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSTypes;

namespace TSDal
{
    public class InvoiceSvc: Invoice, ICopySqlDataReader<InvoiceSvc>
    {
        public string ProjectName { get; set; }

        public static List<Invoice> LastInvoices(int customerid,
                                                    int projectid, 
                                                    int statusid,
                                                    int pageindex,
                                                    int pagecount)
        {
            return DAL.InvoiceLast(customerid,
                                    projectid,
                                    statusid,
                                    pageindex,
                                    pagecount);
        }


        public static List<Invoice> LastInvoicesByProject(int customerid,
                                                    int projectid, 
                                                    int pageindex,
                                                    int pagecount)
        {
            return DAL.InvoiceLastByProject(customerid,
                                            projectid,
                                            pageindex,
                                            pagecount);
        }


        public static List<Invoice> AllInvoices(int customerid,
                                                int projectid,
                                                DateTime fromdate,
                                                DateTime todate,
                                                int pageindex,
                                                int pagecount)
        {
            return DAL.InvoiceAll( customerid,
                                                 projectid,
                                                 fromdate,
                                                 todate,
                                                 pageindex,
                                                 pagecount);
        }

        public static int Create(int CustomerId,
                                    int ProjectId,
                                    int PeriodYear,
                                    int PeriodMonth,
                                    bool IsMonthly,
                                    DateTime InvoiceDate,
                                    List<TimeEntry> TSEntries)
        {

           

            Project p = ProjectSvc.GetById(CustomerId, ProjectId);
            decimal taxRate = p.SalesTax;
            DateTime PaymentDate = InvoiceDate.AddDays(p.PaymentTermsNumber);

            List<HourlyRate> projectRates = HourlyRateSvc.GetByProjectId(ProjectId);

            if (projectRates == null)
            {
                throw new Exception(string.Format("No rate was found in Project [{0}].", ProjectId));
            }
            if (projectRates.Count == 0)
            {
                throw new Exception(string.Format("No rate was found in Project [{0}].", ProjectId));
            }

            int invoiceId = DAL.InvoiceCreate(ProjectId,
                                                IsMonthly,
                                                PeriodYear,
                                                PeriodMonth,
                                                InvoiceDate,
                                                PaymentDate,
                                                taxRate);



            // create invoice entries
            decimal preTaxTotal = 0;
            foreach (TimeEntry ts in TSEntries)
            {

                HourlyRate rate = projectRates.Find(rr => rr.UserId == ts.UserId);
                if (rate == null)
                {
                    throw new Exception(string.Format("No rate was found in Project [{0}], User [{1}].", ProjectId, ts.UserId));
                }

                TimeSpan span = ts.EndTime - ts.StartTime;
                double totalMinutes = span.TotalMinutes - ts.BreakMinutes;
                totalMinutes = totalMinutes > 0 ? totalMinutes : 0;

                InvoiceEntrySvc.Create(invoiceId,
                                        ts.UserId,
                                        ts.EntryDate,
                                        ts.StartTime,
                                        ts.EndTime,
                                        ts.Description,
                                        ts.BreakMinutes,
                                        rate.Amount,
                                        rate.Amount * (decimal)totalMinutes/60,
                                        ts.Id);

                preTaxTotal += rate.Amount * (decimal)totalMinutes/60;

            }

            // Adhoc entries: TODO



            decimal taxTotal = decimal.Round(((Decimal)((preTaxTotal * ((Decimal)taxRate / (Decimal)100)))), 2, MidpointRounding.AwayFromZero);
            decimal amountTotal = preTaxTotal + taxTotal;


            // Update Invoice totals
            DAL.InvoiceUpdateTotals(invoiceId, 
                                    preTaxTotal,
                                    taxTotal,
                                    amountTotal);

            // Set Unbilled flag on original TSEntries to track
            // that they've been billed at least once
            DAL.SetIsInvoicedByInvoice(invoiceId);

            return invoiceId;
            
        }


        public static int GetInvoiceDurationMins(List<InvoiceEntry> entries)
        {

            int mins = 0;
            foreach (InvoiceEntry e in entries)
            {
                mins += e.DurationMinutes;
            }
            return mins;
        }

        public static void Save(int invoiceid, 
                                int statusid)
        {
            DAL.InvoiceSave(invoiceid, statusid);
        }

        public static Invoice GetById(int invoiceid)
        {
            return DAL.InvoiceById(invoiceid);
        }


        public static void SaveLogo(int customerid, string logofilename)
        {
            DAL.InvoiceLogoSave(customerid, logofilename);
        }

        public static List<NameVal> GetAllStatuses()
        {
            List<NameVal> list = new List<NameVal>();
            foreach (var v in Enum.GetValues(typeof(InvoiceStatus)))
            {
                list.Add(new NameVal { Id = Convert.ToInt32(v), Name = ((InvoiceStatus)v).ToString() });
            }
            return list;
        }

        private static List<T> GetEnumList<T>()
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            List<T> list = new List<T>(array);
            return list;
        }

       

        public InvoiceSvc CopyFromReader(SqlDataReader rdr)
        {
                   

            return new InvoiceSvc
            {
                Id = (int)rdr["invoiceid"],
                ProjectId = (int)rdr["projectid"],
                CustomerId = (int)rdr["customerid"],
                InvoiceDate = (DateTime)rdr["invoicedate"],
                InvoiceNumber = rdr["invoicenumber"] == DBNull.Value ? "": (string)Convert.ToString(rdr["invoicenumber"]),

                Status = ((InvoiceStatus)((int)rdr["invoicestatusid"])),
                PaymentDate = (DateTime)rdr["paymentdate"],
                DateCreated = (DateTime)rdr["datecreated"],
                Amount = (decimal)rdr["amount"],
                SingleUserYN = (bool)rdr["singleuser_yn"],
                MonthlyInvoiceYN = (bool)rdr["monthlyinvoice_yn"],
                PeriodYear = (rdr["mo_periodyear"] == DBNull.Value ? DateTime.Now.Year : (int)rdr["mo_periodyear"]),
                PeriodMonth = rdr["mo_periodyear"] == DBNull.Value ? DateTime.Now.Month : (int)rdr["mo_periodmonth"],
                TaxRate = (decimal)rdr["salestax"],
                TaxTotal = (decimal)rdr["taxtotal"],
                AmountTotal = rdr["amounttotal"] == DBNull.Value ? 0: (decimal)rdr["amounttotal"],
                Comment = rdr["comment"] == DBNull.Value ? "": (string)rdr["comment"],
                ProjectName = (string)rdr["projectname"]
            };
        }

        public InvoiceSvc() { }

        
    }
}

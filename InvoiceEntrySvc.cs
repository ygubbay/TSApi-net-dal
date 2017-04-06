using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSTypes;

namespace TSDal
{
    public class InvoiceEntrySvc: InvoiceEntry, ICopySqlDataReader<InvoiceEntrySvc>
    {

        
        public InvoiceEntrySvc() { }
        public InvoiceEntrySvc CopyFromReader(SqlDataReader rdr)
        {
       
            return new InvoiceEntrySvc
            {
                Id = (int)rdr["invoiceentryid"],
                InvoiceId = (int)rdr["InvoiceId"],
                UserId = (int)rdr["UserId"],
                Entrydate = (DateTime)rdr["Entrydate"],
                StartTime = (DateTime)rdr["StartTime"],
                EndTime = (DateTime)rdr["EndTime"],
                Description = (string)rdr["Description"],
                BreakMinutes = (int)rdr["Break"],
                HourlyRate = (decimal)rdr["HourlyRate"],
                Amount = (decimal)rdr["Amount"],
                TSEntryId = rdr["TSEntryId"] == DBNull.Value ? 0: (int)rdr["tsentryid"]
            };
        }

        public static int Create (int InvoiceId, 
                                    int UserId, 
                                    DateTime Entrydate, 
                                    DateTime StartTime, 
                                    DateTime EndTime, 
                                    string description, 
                                    int breakminutes,
                                    decimal hourlyrate, 
                                    decimal amount,
                                    int tsentryid)
        {
            return DAL.InvoiceEntryCreate(InvoiceId, 
                                            UserId, 
                                            Entrydate, 
                                            StartTime, 
                                            EndTime, 
                                            description, 
                                            breakminutes,
                                            hourlyrate, 
                                            amount,
                                            tsentryid);
        }


        public static List<InvoiceEntry> GetByInvoiceId(int invoiceid)
        {
            return DAL.InvoiceEntryGet(invoiceid);
        }


        public static void SetIsInvoicedByInvoice(int invoiceid)
        {
            DAL.SetIsInvoicedByInvoice(invoiceid);
        }

    }
}

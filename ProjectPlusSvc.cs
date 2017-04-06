using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSDal
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TSTypes;

    namespace TSDal
    {
        public class ProjectPlusSvc : ProjectPlus, ICopySqlDataReader<ProjectPlusSvc>
        {

            public ProjectPlusSvc CopyFromReader(SqlDataReader rdr)
            {
                return new ProjectPlusSvc
                {
                    ProjectId = (int)rdr["projectid"],
                    Name = (string)rdr["name"],
                    CustomerId = (int)rdr["customerid"],
                    PaymentTermsNumber = (int)rdr["paymenttermsnumber"],
                    MonthlyInvoiceYN = (bool)rdr["MonthlyInvoiceYN"],
                    PaymentTermsMonthYN = (bool)rdr["PaymentTermsMonthYN"],

                    SalesTax = (decimal)rdr["salestax"],

                    CustCompanyName = (string)rdr["CustCompanyName"],
                    CustContactName = (string)rdr["CustContactName"],
                    CustContactEmail = (string)rdr["CustContactEmail"],
                    CustContactPhone = (string)rdr["CustContactPhone"],
                    CultureKey = (string)rdr["CultureKey"],
                    InvoiceFilePrefix = (string)rdr["InvoiceFilePrefix"],

                    IsActive = (bool)rdr["IsActive"],
                    StartDate = (DateTime)rdr["startdate"],
                    EndDate = rdr["enddate"] == DBNull.Value ? DateTime.MaxValue : (DateTime)rdr["enddate"],

                    AmountTotal = rdr["projecttotal"] == DBNull.Value ? 0: (decimal)rdr["projecttotal"],
                    AverageMonthlyRevenue = rdr["projecttotal"] == DBNull.Value ? 0 : (decimal)rdr["averagemonthlyrevenue"],
                    NumberInvoices = (int)rdr["numberinvoices"],
                    InvoiceFirstDate = rdr["invoicefirstdate"] == DBNull.Value ? DateTime.MinValue : (DateTime)rdr["invoicefirstdate"],
                    InvoiceLastDate = rdr["invoicelastdate"] == DBNull.Value ? DateTime.MinValue : (DateTime)rdr["invoicelastdate"]

                };
            }



            public static List<ProjectPlus> Get(int customerid, int year = 0)
            {
                return DAL.ProjectsPlusGet(customerid, year);
            }


        }
    }

}

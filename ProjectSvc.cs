using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSTypes;

namespace TSDal
{
    public class ProjectSvc: Project, ICopySqlDataReader<ProjectSvc>
    {

        public ProjectSvc CopyFromReader(SqlDataReader rdr)
        {
            return new ProjectSvc { ProjectId = (int)rdr["projectid"],
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
                EndDate = rdr["enddate"] == DBNull.Value ? (DateTime?)null: (DateTime?)rdr["enddate"]
            
                };
        }



        public static List<Project> GetByCustomer(int customerid)
        {
            return DAL.ProjectGetByCustomer(customerid);
        }

       
        public static List<Project> GetActiveByCustomer(int customerid)
        {
            return DAL.ProjectActiveGetByCustomer(customerid);
        }


        public static Project GetById(int customerid, int projectid)
        {
            return DAL.ProjectById(customerid, projectid);
        }


        public static void Update(int customerid,
                                  int projectId,
                                  string name,
                                  int paymenttermsdays,
                                  decimal salestax,
                                  string invoicefileprefix,
                                  string companyname,
                                  string contactname,
                                  string contactemail,
                                  string contactphone)
        {
            DAL.ProjectUpdate(customerid,
                                     projectId,
                                        name,
                                        paymenttermsdays,
                                        salestax,
                                        companyname,
                                        contactname,
                                        contactemail,
                                        contactphone,
                                        invoicefileprefix);
        }


        public static Project GetByProjectLinkId(string link)
        {

            return DAL.ProjectByLinkId(link);
        }



        public static int Create(int customerid, 
                                string projectname,
                                int paymenttermsnumber, 
                                decimal salestax,
                                string custcompanyname,
                                string custcontactname,
                                string custcontactemail,
                                string custcontactphone,
                                string culturekey,
                                string invoicefileprefix)
        {

            if (string.IsNullOrEmpty(custcompanyname))
            {
                custcompanyname = projectname;
            }
            return DAL.ProjectCreate(customerid,
                                        projectname,
                                        paymenttermsnumber,
                                        salestax,
                                        custcompanyname,
                                        custcontactname,
                                        custcontactemail,
                                        custcontactphone,
                                        culturekey,
                                        invoicefileprefix,
                                        DateTime.Now);
        }
    }
}

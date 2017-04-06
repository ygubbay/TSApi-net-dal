using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSTypes;
using TSTypes.Responses;

namespace TSDal
{
    public class CustomerSvc: Customer, ICopySqlDataReader<CustomerSvc>    
    {


        public static void CustomerSave(Customer c)
        {
            DAL.CustomerSave(c);
        }



        public static Customer GetById(int customerid)
        {
            return DAL.CustomerById(customerid);
        }

        public static List<Language> GetLanguages()
        {
            return DAL.Languages();
        }


        public CustomerSvc CopyFromReader(SqlDataReader rdr)
        {
            return new CustomerSvc
            {
                Id = (int)rdr["customerid"],
                Name = (string)rdr["Name"], 
                 ContactFirstname  = (string)rdr["Firstname"],
                ContactLastname = (string)rdr["Lastname"],
                SubscriptionTypeId = (int)rdr["SubscriptionTypeId"],
                AffiliateId = rdr["AffiliateId"] == DBNull.Value ? 0: (int)rdr["AffiliateId"],
                SubscriptionId = rdr["SubscriptionId"] == DBNull.Value ? 0 : (int)rdr["SubscriptionId"],
                CountryName = rdr["CountryName"] == DBNull.Value ? "" : (string)rdr["CountryName"],
                Address1 = rdr["Address1"] == DBNull.Value ? "" : (string)rdr["Address1"],
                Address2 = rdr["Address2"] == DBNull.Value ? "" : (string)rdr["Address2"],
                City = rdr["City"] == DBNull.Value ? "" : (string)rdr["city"],
                Email = rdr["Email"] == DBNull.Value ? "" : (string)rdr["Email"],
                Fax = rdr["Fax"] == DBNull.Value ? "" : (string)rdr["Fax"],
                Phone = rdr["Phone"] == DBNull.Value ? "" : (string)rdr["Phone"],
                Website = rdr["Website"] == DBNull.Value ? "" : (string)rdr["Website"],
                LogoFile = rdr["LogoFile"] == DBNull.Value ? "" : (string)rdr["LogoFile"],
                InvoiceCultureName = rdr["InvoiceCultureName"] == DBNull.Value ? "en-US" : (string)rdr["InvoiceCultureName"],
                CurrencyId = rdr["CurrencyId"] == DBNull.Value ? 1 : (int)rdr["CurrencyId"],
                CurrencySymbol = rdr["CurrencySymbol"] == DBNull.Value ? "$" : (string)rdr["CurrencySymbol"],
                DateCreated = (DateTime)rdr["DateCreated"]

            };
        }


        public static CustomerAlerts GetAlerts(int customerid)
        {
            return DAL.CustomerGetAlerts(customerid);
        }

    }
}

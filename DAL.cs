using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSDal.TSDal;
using TSTypes;
using TSTypes.Responses;

namespace TSDal
{
    public class DAL
    {

        public static string CONNECTION_STRING = ConfigurationManager.AppSettings["connString"];

        public static List<Currency> CurrenciesAll()
        {
            SqlQuery<CurrencySvc, Currency> q = new SqlQuery<CurrencySvc, Currency>();

            return q.SqlRead("ns_CurrenciesAll");
        }



        public static List<Language> Languages()
        {
            SqlQuery<LanguageSvc, Language> q = new SqlQuery<LanguageSvc, Language>();

            return q.SqlRead("ns_Languages");
        }


        public static List<TimeEntry> GetUninvoicedTEByProjectId(int projectid)
        {
            SqlQuery<TSSvc, TimeEntry> q = new SqlQuery<TSSvc, TimeEntry>();
            q.ParameterAdd("@projectid", projectid);

            return q.SqlRead("ns_UninvoicedTEByProjectId");
        }

        public static Project ProjectByLinkId(string link)
        {
            SqlQuery<ProjectSvc, Project> q = new SqlQuery<ProjectSvc, Project>();
            q.ParameterAdd("@link", link);

            List<Project> l = q.SqlRead("ns_ProjectByLinkId");
            if (l.Count != 1)
            {
                throw new Exception("ProjectByLinkId: 1 row not returned.");
            }
            return l[0];
        }


        public static ProjectLink ProjectLinkNew(int projectid)
        {
            SqlQuery<ProjectLinkSvc, ProjectLink> q = new SqlQuery<ProjectLinkSvc, ProjectLink>();
            q.ParameterAdd("@projectid", projectid);

            List<ProjectLink> l = q.SqlRead("ns_ProjectLinkAdd");

            if (l.Count != 1)
            {
                throw new Exception(string.Format("ProjectLinkNew: project [{0}] returned {1} rows.", projectid, l.Count));
            }
            return l[0];

        }


        public static List<ProjectLink> ProjectLinksByCustomer(int customerid)
        {
            SqlQuery<ProjectLinkSvc, ProjectLink> q = new SqlQuery<ProjectLinkSvc, ProjectLink>();
            q.ParameterAdd("@customerid", customerid);

            return q.SqlRead("ns_ProjectLinksByCustomer");

        }


        public static void ProjectLinkUpdate(int projectid, bool isactive)
        {
            SqlQuery<ProjectLinkSvc, ProjectLink> q = new SqlQuery<ProjectLinkSvc, ProjectLink>();
            q.ParameterAdd("@projectlinkid", projectid);
            q.ParameterAdd("@isactive", isactive);

            q.SqlRead("ns_ProjectLinkUpdate");
        }

        public static User UserLogin(string username,
                                 string password)
        {

            SqlQuery<UserSvc, User> q = new SqlQuery<UserSvc, User>();
            q.ParameterAdd("@username", username);
            q.ParameterAdd("@password", password);

            List<User> l = q.SqlRead("ns_CheckLogin");

            return l.Count == 1 ? (User)l[0] : null;
        }
        public static User UserCookieLogin(string logincookie)
        {
            SqlQuery<UserSvc, User> q = new SqlQuery<UserSvc, User>();
            q.ParameterAdd("@cookie", logincookie);

            List<User> l = q.SqlRead("ns_CookieLogin");

            return l.Count == 1 ? (User)l[0] : null;
        }
        public static User UserGetByEmail(string Email)
        {
            SqlQuery<UserSvc, User> q = new SqlQuery<UserSvc, User>();
            q.ParameterAdd("@email", Email);

            List<User> l = q.SqlRead("ns_UserByEmail");

            return l.Count == 1 ? (User)l[0] : null;
        }
        public static User UserGetByUsername(string Username)
        {
            SqlQuery<UserSvc, User> q = new SqlQuery<UserSvc, User>();
            q.ParameterAdd("@username", Username);

            List<User> l = q.SqlRead("ns_UserByUsername");

            return l.Count == 1 ? (User)l[0] : null;
        }

        public static User UserGetByUserId(int userid)
        {
            SqlQuery<UserSvc, User> q = new SqlQuery<UserSvc, User>();
            q.ParameterAdd("@userid", userid);

            List<User> l = q.SqlRead("ns_UserByUserId");

            return l.Count == 1 ? (User)l[0] : null;
        }


        public static void UserSave(int userid,
                                    string firstname,
                                    string lastname,
                                    string email)
        {
            // Check for changed email which is not unique
            User checkUser = UserGetByEmail(email);
            if (checkUser.UserId != userid )
            {
                throw new Exception(string.Format("Email [{0}] is in use", email));
            }


            SqlQuery<UserSvc, User> q = new SqlQuery<UserSvc, User>();

            q.ParameterAdd("@userid", userid);
            q.ParameterAdd("@firstname", firstname);
            q.ParameterAdd("@lastname", lastname);
            q.ParameterAdd("@email", email);

            q.SqlExecute("ns_UserSave");
        }

        public static int InvoiceCreate(int ProjectId,
                                   bool IsMonthly,
                                   int PeriodYear,
                                   int PeriodMonth,
                                   DateTime InvoiceDate,
                                   DateTime PaymentDate,
                                   decimal TaxRate)
        {

            SqlQuery<InvoiceSvc, Invoice> q = new SqlQuery<InvoiceSvc, Invoice>();

            q.ParameterAdd("@projectid", ProjectId);
            q.ParameterAdd("@ismonthly", IsMonthly);
            q.ParameterAdd("@periodyear", PeriodYear);
            q.ParameterAdd("@periodmonth", PeriodMonth);
            q.ParameterAdd("@invoicedate", InvoiceDate);
            q.ParameterAdd("@paymentdate", PaymentDate);
            q.ParameterAdd("@status", (int)InvoiceStatus.NotComplete);
            q.ParameterAdd("@taxrate", (decimal)TaxRate);

            return q.SqlExecuteRetval("ns_InvoiceAdd");
        }

        public static void SetIsInvoicedByInvoice(int invoiceid)
        {
            SqlQuery<InvoiceEntrySvc, InvoiceEntry> q = new SqlQuery<InvoiceEntrySvc, InvoiceEntry>();
            q.ParameterAdd("@invoiceid", invoiceid);
            

            q.SqlExecute("ns_TSEntrySetIsInvoiced");
            

        }


        public static void InvoiceSave(int invoiceid,
                                       int statusid)
        {
            SqlQuery<InvoiceSvc, Invoice> q = new SqlQuery<InvoiceSvc, Invoice>();

            q.ParameterAdd("@invoiceid", invoiceid);
            q.ParameterAdd("@statusid", statusid);

            q.SqlExecuteRetval("ns_InvoiceSave");
        }

        public static void InvoiceUpdateTotals(int invoiceid, 
                                                decimal preTaxTotal,
                                                decimal taxTotal,
                                                decimal amountTotal)
        {
            SqlQuery<InvoiceSvc, Invoice> q = new SqlQuery<InvoiceSvc, Invoice>();

            q.ParameterAdd("@invoiceid", invoiceid);
            q.ParameterAdd("@pretaxtotal", preTaxTotal);
            q.ParameterAdd("@taxtotal", taxTotal);
            q.ParameterAdd("@amounttotal", amountTotal);
            

            q.SqlExecuteRetval("ns_InvoiceUpdateTotals");
        }


        public static List<Invoice> InvoiceLast(int customerid,
                                                int projectid,
                                                int statusid,
                                                int pageindex,
                                                int pagecount)
        {


            SqlQuery<InvoiceSvc, Invoice> q = new SqlQuery<InvoiceSvc, Invoice>();
            q.ParameterAdd("@customerid", customerid);
            q.ParameterAdd("@projectid", projectid);
            q.ParameterAdd("@statusid", statusid);
            q.ParameterAdd("@pageindex", pageindex);
            q.ParameterAdd("@pagecount", pagecount);

            return q.SqlRead("ns_InvoicesLast");
        }



        public static List<Invoice> InvoiceLastByProject(int customerid,
                                                            int projectid,
                                                            int pageindex,
                                                            int pagecount)
        {


            SqlQuery<InvoiceSvc, Invoice> q = new SqlQuery<InvoiceSvc, Invoice>();
            q.ParameterAdd("@customerid", customerid);
            q.ParameterAdd("@projectid", projectid);
            q.ParameterAdd("@pageindex", pageindex);
            q.ParameterAdd("@pagecount", pagecount);

            return q.SqlRead("ns_InvoicesLastByProject");
        }

        public static List<Invoice> InvoiceAll(int customerid,
                                                int projectid,
                                                DateTime fromdate,
                                                DateTime todate,
                                                int pageindex,
                                                int pagecount)
        {


            SqlQuery<InvoiceSvc, Invoice> q = new SqlQuery<InvoiceSvc, Invoice>();
            q.ParameterAdd("@customerid", customerid);
            q.ParameterAdd("@projectid", projectid);
        
            return q.SqlRead("ns_InvoicesAll");
        }

        public static Invoice InvoiceById(int invoiceid)
        {
            SqlQuery<InvoiceSvc, Invoice> q = new SqlQuery<InvoiceSvc, Invoice>();
            q.ParameterAdd("@invoiceid", invoiceid);

            List<Invoice> l = q.SqlRead("ns_InvoiceById");
            return l.Count == 1 ? l[0] : null;
        }


        public static void InvoiceLogoSave(int customerid, string logofilename)
        {
            SqlQuery<InvoiceSvc, Invoice> q = new SqlQuery<InvoiceSvc, Invoice>();
            q.ParameterAdd("@customerid", customerid);
            q.ParameterAdd("@logofilename", logofilename);

            q.SqlExecute("ns_SaveLogo");
        }


        public static int InvoiceEntryCreate(int InvoiceId,
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
            SqlQuery<InvoiceEntrySvc, InvoiceEntry> q = new SqlQuery<InvoiceEntrySvc, InvoiceEntry>();

            q.ParameterAdd("@invoiceid", InvoiceId);
            q.ParameterAdd("@userid", UserId);
            q.ParameterAdd("@entrydate", Entrydate);
            q.ParameterAdd("@starttime", StartTime);
            q.ParameterAdd("@endtime", EndTime);
            q.ParameterAdd("@description", description);
            q.ParameterAdd("@breakminutes", breakminutes);
            q.ParameterAdd("@hourlyrate", hourlyrate);
            q.ParameterAdd("@amount", amount);
            q.ParameterAdd("@tsentryid", tsentryid);

            return q.SqlExecuteRetval("ns_InvoiceEntryAdd");
        }


        public static List<InvoiceEntry> InvoiceEntryGet(int invoiceid)
        {
            SqlQuery<InvoiceEntrySvc, InvoiceEntry> q = new SqlQuery<InvoiceEntrySvc, InvoiceEntry>();

            q.ParameterAdd("@invoiceid", invoiceid);

            return q.SqlRead("ns_InvoiceEntriesByInvoiceId");
        }


        public static User Registration( string Username,
                                    string Firstname,
                                    string Lastname,
                                    string Password,
                                    string Email,
                                    string CustomerName,
                                    string CountryName,
                                    bool IsAffiliate,
                                    string AffiliateCode)
        {
            SqlQuery<UserSvc, User> q = new SqlQuery<UserSvc, User>();

            q.ParameterAdd("@username", Username);
            q.ParameterAdd("@firstname", Firstname);
            q.ParameterAdd("@lastname", Lastname);
            q.ParameterAdd("@password", Password);
            q.ParameterAdd("@email", Email);
            q.ParameterAdd("@customername", CustomerName);
            q.ParameterAdd("@countryname", CountryName);
            q.ParameterAdd("@isaffiliate", IsAffiliate);
            q.ParameterAdd("@affiliatecode", AffiliateCode);

            int retval = q.SqlExecuteRetval("ns_RegistrationTrial");
            if (retval == (int)RegistrationResult.UnknownAffiliateCode)
            {
                throw new Exception(string.Format("Invalid Affiliate Code [{0}]", AffiliateCode));
            }
            if (retval != 0)
            {
                throw new Exception(string.Format("Registration failed user[{0}]", Username));
            }

            // Email is unique
            return UserGetByEmail(Email);
        }


        public static void HourlyRateCreate(int projectid, int userid, decimal hourlyrate)
        {
            SqlQuery<HourlyRateSvc, HourlyRate> q = new SqlQuery<HourlyRateSvc, HourlyRate>();
            q.ParameterAdd("@projectid", projectid);
            q.ParameterAdd("@userid", userid);
            q.ParameterAdd("@hourlyrate", hourlyrate);

            q.SqlExecute("ns_HourlyRatesCreate");
        }


        public static void HourlyRateUpdate(int customerid,
                                            int projectid,
                                            decimal hourlyrate)
        {
            SqlQuery<HourlyRateSvc, HourlyRate> q = new SqlQuery<HourlyRateSvc, HourlyRate>();
            q.ParameterAdd("@customerid", customerid);
            q.ParameterAdd("@projectid", projectid);
            //q.ParameterAdd("@userid", userid);
            q.ParameterAdd("@hourlyrate", hourlyrate);

            q.SqlExecute("ns_HourlyRatesUpdate");
        }


        public static List<HourlyRate> HourlyRateByProjectId(int projectid)
        {
            SqlQuery<HourlyRateSvc, HourlyRate> q = new SqlQuery<HourlyRateSvc, HourlyRate>();

            q.ParameterAdd("@projectid", projectid);

            return q.SqlRead("ns_HourlyRatesForProject");

        }


        public static List<Project> ProjectActiveGetByCustomer(int customerid)
        {
            SqlQuery<ProjectSvc, Project> q = new SqlQuery<ProjectSvc, Project>();
            q.ParameterAdd("@customerid", customerid);

            return q.SqlRead("ns_ProjectActiveByCustomer");
        }




        public static List<Project> ProjectGetByCustomer(int customerid)
        {
            SqlQuery<ProjectSvc, Project> q = new SqlQuery<ProjectSvc, Project>();
            q.ParameterAdd("@customerid", customerid);

            return q.SqlRead("ns_ProjectByCustomer");
        }

        public static List<ProjectPlus> ProjectsPlusGet(int customerid, int year)
        {
            SqlQuery<ProjectPlusSvc, ProjectPlus> q = new SqlQuery<ProjectPlusSvc, ProjectPlus>();
            q.ParameterAdd("@customerid", customerid);
            q.ParameterAdd("@year", year);

            return q.SqlRead("ns_ProjectsPlus");
        }

        public static Project ProjectById(int customerid, int projectid)
        {
            SqlQuery<ProjectSvc, Project> q = new SqlQuery<ProjectSvc, Project>();
            q.ParameterAdd("@customerid", customerid);
            q.ParameterAdd("@projectid", projectid);

            List<Project> l= q.SqlRead("ns_ProjectById"); 
            return l.Count == 1 ? l[0] : null;
        }


        public static void ProjectUpdate(int customerid,
                                          int projectid,
                                          string name,
                                          int paymenttermsdays,
                                          decimal salestax,
                                          string invoicefileprefix,
                                          string companyname,
                                          string contactname,
                                          string contactemail,
                                          string contactphone)
        {
            SqlQuery<ProjectSvc, Project> q = new SqlQuery<ProjectSvc, Project>();

            q.ParameterAdd("@customerid", customerid);
            q.ParameterAdd("@projectid", projectid);
            q.ParameterAdd("@name", name);
            q.ParameterAdd("@paymenttermsdays", paymenttermsdays);
            q.ParameterAdd("@salestax", salestax);
            q.ParameterAdd("@custcompanyname", companyname);
            q.ParameterAdd("@custcontactname", contactname);
            q.ParameterAdd("@custcontactemail", contactemail);
            q.ParameterAdd("@custcontactphone", contactphone);
            q.ParameterAdd("@invoicefileprefix", invoicefileprefix);

            q.SqlExecute("ns_ProjectUpdate");

        }


        public static CustomerAlerts CustomerGetAlerts(int customerid)
        {
            SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            try
            {


                SqlCommand comm = new SqlCommand("ns_InvoiceAlerts", conn);
                comm.CommandType = CommandType.StoredProcedure;

                comm.Parameters.AddWithValue("@customerid", customerid).DbType = DbType.Int32;
                conn.Open();


                SqlDataReader rdr = comm.ExecuteReader();

                if (rdr.Read())
                {
                    return new CustomerAlerts
                    {
                       OverdueInvoices = (int)rdr["overdueinvoices"],
                        UninvoicedTasks = (int)rdr["overduetsentries"],
                    };
                }
                return new CustomerAlerts();
            }
            catch (Exception ee)
            {

                throw ee;
            }
            finally
            {
                conn.Close();
            }
        }


        public static int ProjectCreate(int customerid,
                                        string projectname,
                                        int paymenttermsnumber,
                                        decimal salestax,
                                        string custcompanyname,
                                        string custcontactname,
                                        string custcontactemail,
                                        string custcontactphone,
                                        string culturekey,
                                        string invoicefileprefix,
                                        DateTime startdate)
        {

            SqlQuery<ProjectSvc, Project> q = new SqlQuery<ProjectSvc, Project>();
            q.ParameterAdd("@customerid", customerid);
            q.ParameterAdd("@name", projectname);
            q.ParameterAdd("@paymenttermsnumber", paymenttermsnumber);
            q.ParameterAdd("@salestax", salestax);
            q.ParameterAdd("@custcompanyname", custcompanyname);
            q.ParameterAdd("@custcontactname", custcontactname);
            q.ParameterAdd("@custcontactemail", custcontactemail);
            q.ParameterAdd("@custcontactphone", custcontactphone);
            q.ParameterAdd("@culturekey", culturekey);
            q.ParameterAdd("@invoicefileprefix", invoicefileprefix);
            q.ParameterAdd("@startdate", startdate);

            int retval = q.SqlExecuteRetval("ns_ProjectCreate");

            if (retval == -10)
            {
                throw new Exception(string.Format("Project exists with name [{0}].", projectname));
            }
            return retval;
        }


        public static void TSDelete(int taskid)
        {
            SqlQuery<TSSvc, TSTypes.TimeEntry> q = new SqlQuery<TSSvc, TimeEntry>();
            q.ParameterAdd("@taskid", taskid);

            q.SqlExecute("ns_TSDelete");
        }



        public static void TSSaveNew(int projectid, 
                                    DateTime entrydate, 
                                    DateTime starttime, 
                                    DateTime endtime, 
                                    string description, 
                                    bool billable,
                                    int breakminutes,
                                    int userid, 
                                    bool isinvoiced)
        {
            SqlQuery<UserSvc, User> q = new SqlQuery<UserSvc,User>();
            q.ParameterAdd("@projectid", projectid);
            q.ParameterAdd("@entrydate", entrydate);
            q.ParameterAdd("@starttime", starttime);
            q.ParameterAdd("@endtime", endtime);
            q.ParameterAdd("@description", description);
            q.ParameterAdd("@billable", billable);
            q.ParameterAdd("@breakminutes", breakminutes);
            q.ParameterAdd("@userid", userid);
            q.ParameterAdd("@isinvoiced", isinvoiced);

            q.SqlExecute("ns_TSSaveNew");
        }

        public static void TSSave(int id,
                                    int projectid,
                                    DateTime entrydate,
                                    DateTime starttime,
                                    DateTime endtime,
                                    string description,
                                    bool billable,
                                    int breakminutes,
                                    int userid,
                                    bool isinvoiced)
        {
            SqlQuery<UserSvc, User> q = new SqlQuery<UserSvc, User>();
            q.ParameterAdd("@id", id);
            q.ParameterAdd("@projectid", projectid);
            q.ParameterAdd("@entrydate", entrydate);
            q.ParameterAdd("@starttime", starttime);
            q.ParameterAdd("@endtime", endtime);
            q.ParameterAdd("@description", description);
            q.ParameterAdd("@billable", billable);
            q.ParameterAdd("@breakminutes", breakminutes);
            q.ParameterAdd("@userid", userid);
            q.ParameterAdd("@isinvoiced", isinvoiced);

            q.SqlExecute("ns_TSSave");
        }



        public static TSDailyStatsResponse TSDailyStats(int userid, DateTime entrydate)
        {
            SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            try
            {


                SqlCommand comm = new SqlCommand("ns_HourlyStats", conn);
                comm.CommandType = CommandType.StoredProcedure;

                comm.Parameters.AddWithValue("@userid", userid).DbType = DbType.Int32;
                comm.Parameters.AddWithValue("@entrydate", entrydate).DbType = DbType.DateTime;
                conn.Open();


                SqlDataReader rdr = comm.ExecuteReader();

                if (rdr.Read())
                {
                    return new TSDailyStatsResponse
                    {
                        DailyMinutes = (int)rdr["dailyminutes"],
                        WeeklyMinutes = (int)rdr["weeklyminutes"],
                        MonthlyMinutes = (int)rdr["monthlyminutes"]
                    };
                }
                return new TSDailyStatsResponse();
            }
            catch (Exception ee)
            {
   
                return new TSDailyStatsResponse();
            }
            finally
            {
                conn.Close();
            }
        }


        public static TSProjectUsageResponse TSProjectUsage(int userid, DateTime entrydate)
        {
            SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            try
            {


                SqlCommand comm = new SqlCommand("ts_ProjectUsage", conn);
                comm.CommandType = CommandType.StoredProcedure;

                comm.Parameters.AddWithValue("@userid", userid);
                comm.Parameters.AddWithValue("@usagedate", entrydate);
                conn.Open();


                SqlDataReader rdr = comm.ExecuteReader();
                List<ProjectUsage> projects = new List<ProjectUsage>();

                while (rdr.Read())
                {
                    projects.Add(new ProjectUsage
                    {
                        Id = (int)rdr["projectid"],
                        Name = (string)rdr["name"],
                        DurationMinutes = (int)rdr["monthlyminutes"]
                    });
                }
                return new TSProjectUsageResponse { Projects = projects };
            }
            catch (Exception ee)
            {

                return new TSProjectUsageResponse();
            }
            finally
            {
                conn.Close();
            }

        }

        public static List<TimeEntry> TSDailyEntries(int userid, DateTime entrydate)
        {
            SqlQuery<TSSvc, TimeEntry> q = new SqlQuery<TSSvc, TimeEntry>();
            q.ParameterAdd("@userid", userid);
            q.ParameterAdd("@entrydate", entrydate);

            return q.SqlRead("ns_TSDailyEntries");
        }


        public static List<TimeEntry> TSBillEntries(int ProjectId,
                                                  int InvoiceYear,
                                                  int InvoiceMonth,
                                                  bool IsMonthly)
        {
            SqlQuery<TSSvc, TimeEntry> q = new SqlQuery<TSSvc, TimeEntry>();
            q.ParameterAdd("@projectid", ProjectId);
            q.ParameterAdd("@periodyear", InvoiceYear);
            q.ParameterAdd("@periodmonth", InvoiceMonth);
            q.ParameterAdd("@ismonthly", IsMonthly);

            return q.SqlRead("ns_TSBillEntries");
        }



        
        public static void CustomerSave(Customer c)
        {
            SqlQuery<CustomerSvc, Customer> q = new SqlQuery<CustomerSvc, Customer>();
            q.ParameterAdd("@customerid", c.Id);

            q.ParameterAdd("@name", c.Name);
            q.ParameterAdd("@firstname", c.ContactFirstname);
            q.ParameterAdd("@lastname", c.ContactLastname);
            q.ParameterAdd("@address1", c.Address1);
            q.ParameterAdd("@address2", c.Address2);
            q.ParameterAdd("@city", c.City);
            q.ParameterAdd("@countryname", c.CountryName);
            q.ParameterAdd("@email", c.Email);
            q.ParameterAdd("@fax", c.Fax);
            q.ParameterAdd("@phone", c.Phone);
            q.ParameterAdd("@website", c.Website);
            q.ParameterAdd("@invoiceculturename", string.IsNullOrEmpty(c.InvoiceCultureName) ? "en-US": c.InvoiceCultureName);
            q.ParameterAdd("@currencyid", c.CurrencyId);

            q.SqlExecute("ns_CustomerUpdate");
    }



        public static Customer CustomerById(int customerid)
        {
            SqlQuery<CustomerSvc, Customer> q = new SqlQuery<CustomerSvc, Customer>();
            q.ParameterAdd("@customerid", customerid);

            List<Customer> l = q.SqlRead("ns_CustomerById");
            return l.Count == 1 ? l[0] : null;
        }
       
    }

    public interface ICopySqlDataReader<T>
    {
        T CopyFromReader(SqlDataReader rdr);
    }

    public class SqlQuery<T1, T2> where T1 : class, T2, ICopySqlDataReader<T1>, new() 
    {
        public string CONNECTION_STRING = DAL.CONNECTION_STRING;

        private SqlCommand _c;

        public void ParameterAdd(string name, object value)
        {
            if (_c == null)

                _c = new SqlCommand();

            _c.Parameters.Add(new SqlParameter(name, value));
        }

        public void ParameterAdd(SqlParameter p)
        {
            if (_c == null)

                _c = new SqlCommand();

            _c.Parameters.Add(p);
        }

        public List<T2> SqlRead(string storedproc)
        {
            List<T2> list = new List<T2>();

            T1 t = new T1();


            SqlConnection conn = new SqlConnection(CONNECTION_STRING);

            if (_c == null)

                _c = new SqlCommand(storedproc, conn);
            else

                _c.CommandText = storedproc;

            _c.Connection = conn;
            _c.CommandType = System.Data.CommandType.StoredProcedure;

            SqlDataReader rdr;

            try
            {
                conn.Open();
                rdr = _c.ExecuteReader();

                while (rdr.Read())
                {

                    list.Add((T2)(t.CopyFromReader(rdr)));
                }

                rdr.Close();
                return list;


            }
            catch (Exception e)
            {
                throw new ApplicationException("TSDAL Error: SQL [" + storedproc + "], Details: " + e.ToString());
            }
            finally
            {
                conn.Close();
            }
        }


        public List<T2> SqlReadSQL(string sql)
        {
            List<T2> list = new List<T2>();

            T1 t = new T1();


            SqlConnection conn = new SqlConnection(CONNECTION_STRING);

            if (_c == null)

                _c = new SqlCommand(sql, conn);
            else

                _c.CommandText = sql;

            _c.Connection = conn;
            _c.CommandType = CommandType.Text;

            SqlDataReader rdr;

            try
            {
                conn.Open();
                rdr = _c.ExecuteReader();

                while (rdr.Read())
                {

                    list.Add(t.CopyFromReader(rdr));
                }

                rdr.Close();
                return list;


            }
            catch (Exception e)
            {
                throw new ApplicationException("TSDAL Error: SQL [" + sql + "], Details: " + e.ToString());
            }
            finally
            {
                conn.Close();
            }
        }


        public void SqlExecute(string storedproc)
        {

            SqlConnection conn = new SqlConnection(CONNECTION_STRING);

            if (_c == null)

                _c = new SqlCommand(storedproc, conn);
            else

                _c.CommandText = storedproc;


            _c.Connection = conn;
            _c.CommandType = System.Data.CommandType.StoredProcedure;


            try
            {
                conn.Open();
                _c.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new ApplicationException("Data Error: SP [" + storedproc + "], Msg [" + e.Message + "], Stack [" + e.StackTrace + "].");
            }
            finally
            {
                conn.Close();
            }
        }


        public int SqlExecuteRetval(string storedproc)
        {

            SqlConnection conn = new SqlConnection(CONNECTION_STRING);

            if (_c == null)

                _c = new SqlCommand(storedproc, conn);
            else

                _c.CommandText = storedproc;

            SqlParameter p = new SqlParameter("@retval", 0);
            p.Direction = System.Data.ParameterDirection.ReturnValue;
            _c.Parameters.Add(p);

            _c.Connection = conn;
            _c.CommandType = System.Data.CommandType.StoredProcedure;


            try
            {
                conn.Open();
                _c.ExecuteNonQuery();


                return (int)_c.Parameters["@retval"].Value;


            }
            catch (Exception e)
            {
                throw new ApplicationException("Data Error: SP [" + storedproc + "], Msg [" + e.Message + "], Stack [" + e.StackTrace + "].");
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

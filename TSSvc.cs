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
    public class TSSvc: TimeEntry, ICopySqlDataReader<TSSvc>
    {
        
        public TSSvc CopyFromReader(SqlDataReader rdr)
        {
            return new TSSvc
            {
                Id = (int)rdr["tsentryid"],
                ProjectId = (int)rdr["projectid"],
                BreakMinutes = (int)rdr["break"],
                UserId = (int)rdr["userid"],
                Description = (string)rdr["Description"],

                EntryDate = (DateTime)rdr["EntryDate"],
                StartTime = (DateTime)rdr["StartTime"],
                EndTime = (DateTime)rdr["EndTime"],

                Billable = (bool)rdr["Billable"],
                IsInvoiced = (bool)rdr["IsInvoiced"],

                ProjectName = (string)rdr["ProjectName"]
            };
            
        }

        
        
        public static void SaveNew(int projectid, 
                                    DateTime entrydate, 
                                    DateTime starttime, 
                                    DateTime endtime, 
                                    string description, 
                                    bool billable,
                                    int breakminutes,
                                    int userid, 
                                    bool isinvoiced)
        {
            DAL.TSSaveNew(projectid, entrydate, starttime, endtime, description, billable, breakminutes, userid, isinvoiced);
        }


        public static void Delete(int id)
        {
            DAL.TSDelete(id);
        }



        public static void Save(int id, 
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
            DAL.TSSave(id, projectid, entrydate, starttime, endtime, description, billable, breakminutes, userid, isinvoiced);
        }

        public static TSDailyStatsResponse DailyStats(int userid, 
                                                      DateTime entrydate)
        {

            return DAL.TSDailyStats(userid, entrydate);
        }


        public static TSProjectUsageResponse ProjectUsage(int userid,
                                                      DateTime entrydate)
        {

            return DAL.TSProjectUsage(userid, entrydate);
        }




        public static List<TimeEntry> TSDailyEntries(int userid, DateTime entrydate)
        {


            return DAL.TSDailyEntries(userid, entrydate);
        }


        public static List<TimeEntry> BillEntries(int ProjectId,
                                                  int InvoiceYear,
                                                  int InvoiceMonth,
                                                  bool IsMonthly)
        {
            return DAL.TSBillEntries(ProjectId, InvoiceYear, InvoiceMonth, IsMonthly);
        }


        public static List<TimeEntry> GetUninvoicedTEByProjectId(int projectid)
        {
            return DAL.GetUninvoicedTEByProjectId(projectid);
        }

    }
}

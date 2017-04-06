using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSTypes;

namespace TSDal
{
    public class HourlyRateSvc: HourlyRate, ICopySqlDataReader<HourlyRateSvc>
    {

        public static void Create(int projectid,
                                    int userid,
                                    decimal hourlyrate)
        {
            DAL.HourlyRateCreate(projectid, userid, hourlyrate);
        }


        public static void Update(int customerid, 
                                    int projectid, 
                                    decimal hourlyrate)
        {
            DAL.HourlyRateUpdate(customerid, projectid, hourlyrate);
        }

        public static List<HourlyRate> GetByProjectId(int projectid)
        {
            return DAL.HourlyRateByProjectId(projectid);
        }


        public HourlyRateSvc CopyFromReader(SqlDataReader rdr)
        {
            return new HourlyRateSvc {

                 Id = (int)rdr["rateid"],
                 Amount = (decimal)rdr["amount"],
                 StartDate = (DateTime)rdr["startdate"],
                 EndDate = (DateTime)rdr["enddate"],
                 ProjectId = (int)rdr["projectid"],
                 IsActive = (bool)rdr["active"],
                 UserId = (int)rdr["userid"]
            };
        }

        public HourlyRateSvc(){}
    }
}

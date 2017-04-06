using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSTypes;

namespace TSDal
{
    public class ProjectLinkSvc : ProjectLink, ICopySqlDataReader<ProjectLinkSvc>
    {
        public ProjectLinkSvc CopyFromReader(SqlDataReader rdr)
        {
            return new ProjectLinkSvc
            {
                Id = (int)rdr["id"],
                ProjectId = (int)rdr["projectid"],
                Link = (string)rdr["link"],
                IsActive = (bool)rdr["isactive"],
                DateCreated = (DateTime)rdr["datecreated"]
            };
        }

        public static ProjectLink New(int projectid)
        {
            return DAL.ProjectLinkNew(projectid);
        }

        public static List<ProjectLink> GetByCustomer(int customerid)
        {
            return DAL.ProjectLinksByCustomer(customerid);
        }

        public static void Update(int projectlinkid, bool isactive)
        {
            DAL.ProjectLinkUpdate(projectlinkid, isactive);
        }


    }
}

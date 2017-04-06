using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSTypes;

namespace TSDal
{
    
    public class LanguageSvc : Language, ICopySqlDataReader<LanguageSvc>
    {

        public LanguageSvc() { }

        public LanguageSvc CopyFromReader(SqlDataReader rdr)
        {

            return new LanguageSvc
            {
                Id = (string)rdr["id"],
                Name = (string)rdr["languagename"]
            };
        }

        public static List<Language> GetLanguages()
        {
            return DAL.Languages();
        }
    }
}

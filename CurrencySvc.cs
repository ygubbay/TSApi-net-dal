using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSTypes;

namespace TSDal
{
    public class CurrencySvc: Currency, ICopySqlDataReader<CurrencySvc>
    {

        public CurrencySvc() { }

        public CurrencySvc CopyFromReader(SqlDataReader rdr)
        {
            
            return new CurrencySvc
            {
                Id = (int)rdr["id"],
                Name = (string)rdr["name"],
                Symbol = (string)rdr["symbol"]
            };
        }

        public static List<Currency> GetCurrencies()
        {
            return DAL.CurrenciesAll();
        }
    }

}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TSTypes;
using TSTypes.Requests;

namespace TSDal
{
    public class UserSvc : User, ICopySqlDataReader<UserSvc>
    {
        public static void Save(int userid, 
                            string firstname, 
                            string lastname, 
                            string email)
        {
            if (!IsEmail(email))
            {
                throw new Exception(string.Format("Email [{0}] is not a valid email address.", email));
            }
            DAL.UserSave(userid, firstname, lastname, email);
        }

        public UserSvc CopyFromReader(SqlDataReader rdr)
        {
            return new UserSvc {

                UserId = (int)rdr["userid"],
                Username = (string)rdr["username"], 
                Firstname = (string)rdr["firstname"],
                Lastname = (string)rdr["lastname"],
                Password = (string)rdr["clearpwd"],
                Email = rdr["email"]==DBNull.Value? "": (string)rdr["email"],
                LastCookieId = rdr["lastcookieid"] == DBNull.Value ? "": Convert.ToString((Guid)rdr["lastcookieid"]),
                CustomerId = (int)rdr["customerid"],
                UserGroupId = (int)rdr["usergroupid"],
                LoginRetries = rdr["loginretries"] == DBNull.Value ? 0 : (int)rdr["loginretries"],
                DateCreated = (DateTime)rdr["datecreated"],
                LastCookieLoginDate = rdr["LastCookieLoginDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)rdr["LastCookieLoginDate"],
                LastLoginDate = rdr["LastLoginDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)rdr["LastLoginDate"],
                LastLoginFailedDate = rdr["LastLoginFailedDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)rdr["LastLoginFailedDate"]
            };
        }

        public static User GetByEmail(string Email)
        {
            return DAL.UserGetByEmail(Email);
        }

        public static User Login(string username, 
                                 string password)
        {

            return DAL.UserLogin(username, password);
        }


        public static User CookieLogin(string logincookie)
        {
            return DAL.UserCookieLogin(logincookie);
        }

        public static User GetByUserId(int userid)
        {
            return DAL.UserGetByUserId(userid);
        }

        public static bool IsEmail(string emailString)
        {
            return Regex.IsMatch(emailString, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }


        public static User Registration(RegistrationRequest request)
        {
            
            if (!IsEmail(request.Email))
            {
                throw new Exception(string.Format("Email [{0}] is not a valid email address.", request.Email));
            }
            if (DAL.UserGetByEmail(request.Email) != null)
            {
                throw new Exception(string.Format("Email [{0}] already exists.", request.Email));
            }
            if (DAL.UserGetByUsername(request.Username) != null)
            {
                throw new Exception(string.Format("Email [{0}] already exists.", request.Username));
            }
            if (request.Username.Length < 2 || request.Username.Length > 20)
            {
                throw new Exception(string.Format("Username [{0}] is invalid.", request.Username));
            }
            if (string.IsNullOrEmpty(request.Firstname) || request.Firstname.Length > 30)
            {
                throw new Exception(string.Format("First name [{0}] is invalid.", request.Firstname));
            }
            if (string.IsNullOrEmpty(request.Lastname) || request.Lastname.Length > 30)
            {
                throw new Exception(string.Format("Last name [{0}] is invalid.", request.Lastname));
            }
            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 4 || request.Password.Length > 20)
            {

                throw new Exception("Password length is not less than 4");
            }

            // If Customer name empty, then Customer name = Username
            if (string.IsNullOrEmpty(request.CustomerName))
            {
                request.CustomerName = request.Username;
            }

            if (string.IsNullOrEmpty(request.CountryName))
            {
                request.CountryName = "Unknown";
            }

            return DAL.Registration(request.Username,
                                    request.Firstname,
                                    request.Lastname,
                                    request.Password,
                                    request.Email,
                                    request.CustomerName,
                                    request.CountryName,
                                    request.IsAffiliate,
                                    request.AffiliateCode);
            

        }

        
    }

    public enum RegistrationResult
    {
        Success = 1,
        UnknownAffiliateCode = -20
    }
}

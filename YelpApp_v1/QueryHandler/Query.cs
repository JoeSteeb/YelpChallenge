using System;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryHandler_
{
    public class Query
    {


        NpgsqlConnection Connection;

        PreparedStatement AllStatesCommand;
        PreparedStatement CitiesInStateCommand;
        PreparedStatement ZipsInCityCommand;
        PreparedStatement BusinessesInZipCommand;
        PreparedStatement BusinessesInZipWithCategoriesCommand;
        PreparedStatement GetCategoriesInZipCommand;
        PreparedStatement InsertTipCommand;
        PreparedStatement AuthenticateCommand;
        PreparedStatement TipsInBusiness;
        PreparedStatement findUser;
        PreparedStatement findFriends;
        PreparedStatement recentTips;

        // User queries

        PreparedStatement userCommand;

        private string connectionStr;
        
        public Query(string user, string password, string database)
        {
            connectionStr = $"Server=localhost;Username={user};Password={password};Database={database}";
            Connection = new NpgsqlConnection(connectionStr);
        }
        public Query(Query previous)
        {
            connectionStr = previous.connectionStr;
            Connection = previous.Connection;
        }

        public void Open()
        {
            Connection.Open();

            AllStatesCommand = new PreparedStatement(Connection, "SELECT DISTINCT business_state FROM business;");

            CitiesInStateCommand = new PreparedStatement(
                Connection,
                "SELECT DISTINCT city FROM business WHERE business_state = @state;",
                new Dictionary<string, NpgsqlDbType>
                {
                    ["state"] = NpgsqlDbType.Varchar
                }
             );
            ZipsInCityCommand = new PreparedStatement(
               Connection,
               "SELECT DISTINCT CAST (zip AS VARCHAR) FROM business WHERE business_state = @state AND city = @city;",
               new Dictionary<string, NpgsqlDbType>
               {
                   ["state"] = NpgsqlDbType.Varchar,
                   ["city"] = NpgsqlDbType.Varchar
               }
            );
            BusinessesInZipCommand = new PreparedStatement(
               Connection,
               @"SELECT business_id, business_name, business_address, CAST (longitude AS VARCHAR), CAST (latitude AS VARCHAR), CAST (stars AS VARCHAR), CAST (is_open AS VARCHAR), CAST (tip_count AS VARCHAR), CAST (numCheckins AS VARCHAR)
                 FROM business
                 WHERE zip = @zip;",
               new Dictionary<string, NpgsqlDbType>
               {
                   ["zip"] = NpgsqlDbType.Integer
               }
            );
            BusinessesInZipWithCategoriesCommand = new PreparedStatement(
               Connection,
               @"SELECT Business.business_id AS business_id, business_name, business_address, CAST (longitude AS VARCHAR), CAST (latitude AS VARCHAR), CAST (stars AS VARCHAR), CAST (is_open AS VARCHAR), CAST (tip_count AS VARCHAR), CAST (numCheckins AS VARCHAR)
                FROM business, category
                WHERE business.business_id = category.business_id
                AND (category.category = ANY(@categories))
                AND business.zip = @zip
                GROUP BY business.business_id
                HAVING COUNT( business.business_id ) = array_length(@categories,1);",
               new Dictionary<string, NpgsqlDbType>()
               {
                   ["zip"] = NpgsqlDbType.Integer,
                   ["categories"] = NpgsqlDbType.Varchar | NpgsqlDbType.Array
               }
            );
            GetCategoriesInZipCommand = new PreparedStatement(
               Connection,
               @"SELECT DISTINCT category 
                 FROM Category 
                 WHERE business_id in (
                                        SELECT business_id 
                                        FROM Business 
                                        WHERE zip = @zip
                                      );",
               new Dictionary<string, NpgsqlDbType>()
               {
                   ["zip"] = NpgsqlDbType.Integer,
               }
            );

            TipsInBusiness = new PreparedStatement(
               Connection,
               @"SELECT user_id, CAST (date_posted AS VARCHAR), body, CAST (likes AS VARCHAR)
                 FROM Tip
                 WHERE business_id = @business_id;",
               new Dictionary<string, NpgsqlDbType>()
               {
                   ["business_id"] = NpgsqlDbType.Varchar,
               }
            );

            InsertTipCommand = new PreparedStatement(
                Connection,
                "INSERT INTO tip " +
                "(user_id, business_id, date_posted, body) VALUES " +
                "(@user, @business, @date, @body);",
                new Dictionary<string, NpgsqlDbType>()
                {
                    ["user"] = NpgsqlDbType.Varchar,
                    ["business"] = NpgsqlDbType.Varchar,
                    ["date"] = NpgsqlDbType.Timestamp,
                    ["body"] = NpgsqlDbType.Varchar
                }
            );

            AuthenticateCommand = new PreparedStatement(
                Connection,
                 @"SELECT user_name FROM Users WHERE user_id = @userid AND user_name = @name;",
                new Dictionary<string, NpgsqlDbType>()
                {
                    ["userid"] = NpgsqlDbType.Varchar,
                    ["name"] = NpgsqlDbType.Varchar,
                }
            );

            // User queries
            userCommand = new PreparedStatement(
                Connection, 
                "SELECT DISTINCT user_id FROM users WHERE user_name LIKE @userName;",
                new Dictionary<string, NpgsqlDbType>
                {
                    ["userName"] = NpgsqlDbType.Varchar
                }
                );

            findUser = new PreparedStatement(
                Connection,
                "SELECT user_name, CAST (yelping_since AS VARCHAR), fan_count, tip_count, CAST (average_stars AS VARCHAR), funny, useful, cool, CAST (longitude AS VARCHAR), CAST (latitude AS VARCHAR), likes_count FROM users WHERE user_id = @userId;",
                new Dictionary<string, NpgsqlDbType>
                {
                    ["userId"] = NpgsqlDbType.Varchar
                }
                );
            findFriends = new PreparedStatement(
                Connection,
                "SELECT user_name, likes_count, average_stars, yelping_since FROM users INNER JOIN (SELECT friended_by_id FROM FriendedUser WHERE user_id = @userId) AS J ON friended_by_id = user_id;",
                new Dictionary<string, NpgsqlDbType>
                {
                    ["userId"] = NpgsqlDbType.Varchar
                }
                );

            recentTips = new PreparedStatement(
                Connection,
                "SELECT user_name, business_name, city, body, date_posted FROM Tip NATURAL JOIN business NATURAL JOIN (SELECT user_id, user_name FROM users INNER JOIN (SELECT friended_by_id FROM FriendedUser WHERE user_id = @userId) AS J ON friended_by_id = user_id) AS O ORDER BY date_posted DESC;",
                new Dictionary<string, NpgsqlDbType>
                {
                    ["userId"] = NpgsqlDbType.Varchar
                }
                );
        }



        public IEnumerable<string> GetAllStates()
        {
            return AllStatesCommand.Query(r => r.GetString(0));
        }

        public IEnumerable<string> GetCitiesInState(string state)
        {
            return CitiesInStateCommand.Query(r => r.GetString(0), new Dictionary<string, object>
            {
                ["state"] = state
            });
        }
        public IEnumerable<string> GetZipsInCity(string state, string city)
        {
            return ZipsInCityCommand.Query(r => r.GetString(0), new Dictionary<string, object>
            {
                ["state"] = state,
                ["city"] = city
            });
        }
        public IEnumerable<string> GetCategoriesInZip(int zip)
        {
            return GetCategoriesInZipCommand.Query(b => b.GetString(0), new Dictionary<string, object>
            {
                ["zip"] = zip,
            });
        }

        public IEnumerable<BusinessDatatype> GetBusinessesInZip<BusinessDatatype>(int zip, Func<NpgsqlDataReader, BusinessDatatype> getBusinessData)
        {
            return GetBusinessesInZip(zip, new List<string>(), getBusinessData);
        }
        public IEnumerable<BusinessDatatype> GetBusinessesInZip<BusinessDatatype>(int zip, System.Collections.IList categories, Func<NpgsqlDataReader, BusinessDatatype> getBusinessData)
        {
            return categories.Count == 0 ?
            BusinessesInZipCommand.Query(getBusinessData, new Dictionary<string, object>
            {
                ["zip"] = zip,
            }) :
            BusinessesInZipWithCategoriesCommand.Query(getBusinessData, new Dictionary<string, object>
            {
                ["zip"] = zip,
                ["categories"] = categories
            });
        }

        public int InsertTip(string user, string business, string body, DateTime date)
        {
            return InsertTipCommand.Run(new Dictionary<string, object>
            {
                ["user"] = user,
                ["business"] = business,
                ["body"] = body,
                ["date"] = date
            });
        }

        public bool CheckConnection()
        {
            try
            {
                Connection.Open();
                Connection.Close();
                return true;
            }
            catch (NpgsqlException)
            {
                return false;
            }
            
        }

        public IEnumerable<string> Login(string name, string userid)
        {
            return AuthenticateCommand.Query(b => b.GetString(0), new Dictionary<string, object>
            {
                ["userid"] = userid,
                ["name"] = name
            });
        }

        public IEnumerable<TipsDataType> GetTipsInBusiness<TipsDataType>(string businessid, Func<NpgsqlDataReader, TipsDataType> getTipData)
        {
            return TipsInBusiness.Query(getTipData, new Dictionary<string, object>
            {
                ["business_id"] = businessid
            }); 
        }

        public IEnumerable<string> GetUser(String username)
        {
            username = username + "%";
            return userCommand.Query(b => b.GetString(0), new Dictionary<string, object>
            {
                ["userName"] = username
            });
        }

        public IEnumerable<(object, object, object, object, object, object, object, object, object, object, object)> FindUserById(string id)
        {
            return
            findUser.Query(
                b => (
                    b["user_name"], 
                    b["yelping_since"],
                    b["tip_count"],
                    b["fan_count"],
                    b["average_stars"],
                    b["funny"],
                    b["useful"],
                    b["cool"],
                    b["longitude"],
                    b["latitude"],
                    b["likes_count"]), 
                new Dictionary<string, object>
                {
                    ["userId"] = id
                }
            );

        }

        public IEnumerable<(object, object, object, object)> FindFriendsByID(string id)
        {
            return
            findFriends.Query(
                b => (
                    b["user_name"],
                    b["yelping_since"],
                    b["average_stars"],
                    b["likes_count"]),
                new Dictionary<string, object>
                {
                    ["userId"] = id
                }
            );

        }

        public IEnumerable<(object, object, object, object, object)> FindRecentTips(string id)
        {
            return
            recentTips.Query(
                b => (
                    b["user_name"],
                    b["business_name"],
                    b["city"],
                    b["body"],
                    b["date_posted"]),
                new Dictionary<string, object>
                {
                    ["userId"] = id
                }
            );

        }

    }
}

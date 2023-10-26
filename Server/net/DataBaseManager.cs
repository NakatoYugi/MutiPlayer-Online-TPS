using System;
using System.Security.Principal;
using MySql.Data.MySqlClient;

namespace Multiplayer_Online_Tank_War
{
    public class DataBaseManager
    {
        static System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer(); 
        public static MySqlConnection mysql;

        public static bool Connect(string db, string ip, int port, string user, string pw)
        {
            mysql = new MySqlConnection();
            string s = string.Format("Database={0};Data Source={1};port={2};User Id={3};Password={4}", db, ip, port, user, pw);
            mysql.ConnectionString = s;
            try
            {
                mysql.Open();
                Console.WriteLine("DataBase Connect Succ");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DataBase Connect Fail:" + ex.Message);
                return false;
            }
        }

        private static bool IsSafeString(string str)
        {
            return !System.Text.RegularExpressions.Regex.IsMatch(str, @"[ - | ; | , | \ / | \ ( | \ ) | \ [ | \ ] | \ } | \ { | % | @ | \ * | ! | \ '} ]");
        }

        /// <summary>
        /// 当数据库存在该玩家id时返回true
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsAccountExit(string id)
        {
            if (!IsSafeString(id))
                return false;

            string s = string.Format("select * from account where id = '{0}';", id);
            try
            {
                MySqlCommand cmd = new MySqlCommand(s, mysql);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                dataReader.Close();
                return hasRows;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public static bool Register(string id, string pw)
        {
            if (!IsSafeString(id) || !IsSafeString(pw))
                return false;

            if (IsAccountExit(id))
                return false;

            string sql = string.Format("insert into account set id = '{0}', pw = '{1}';", id, pw);

            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch(MySqlException ex) 
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public static bool CreatePlayer(string id)
        {
            if(!IsSafeString(id))
                return false;

            PlayerData playerData = new PlayerData();
            string data = javaScriptSerializer.Serialize(playerData);

            string sql = string.Format("insert into player set id = '{0}', data = '{1}';", id, data);

            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch(MySqlException ex) 
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public static bool CheckPassword(string id, string pw)
        {
            if(!IsSafeString(id) || !IsSafeString(pw))
                return false;
            //select* from account where id = '{0}';
            string sql = string.Format("select * from account where id = '{0}' and pw = '{1}';", id, pw);

            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                MySqlDataReader reader = cmd.ExecuteReader();
                bool hasRows = reader.HasRows;
                reader.Close();
                return hasRows;
            }
            catch (MySqlException ex) 
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public static PlayerData GetPlayerData(string id)
        {
            if (!IsSafeString(id))
                return null;

            string sql = string.Format("select * from player where id = '{0}';", id);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return null;
                }

                reader.Read();
                string data = reader.GetString("data");

                reader.Close();
                PlayerData playerData = javaScriptSerializer.Deserialize<PlayerData>(data);
                return playerData;
            }
            catch (MySqlException ex) 
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static bool UpdatePlayerData(string id, PlayerData playerData)
        {
            string data = javaScriptSerializer.Serialize(playerData);
            string sql = string.Format("Update player set data = '{0}' where id = '{1}';", data, id);

            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex) 
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}

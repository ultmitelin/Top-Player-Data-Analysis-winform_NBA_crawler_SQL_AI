using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace NBA.Helpers
{
    public static class DbHelper
    {
        // 获取所有球员映射表
        public static DataTable GetPlayerMapping()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sql = "SELECT * FROM player_mapping";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        // 获取指定球员的数据表
        public static DataTable GetPlayerTable(string playerId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string tableName = $"player_{playerId}";
            string sql = $"SELECT * FROM [{tableName}]";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        // 获取指定球员的详细信息
        public static DataTable GetPlayerMappingById(string playerId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlMapping = "SELECT [player_name], [photo], [position], [height], [weight], [birth], [team], [school], [draft] FROM player_mapping WHERE player_id = @playerId";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sqlMapping, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@playerId", playerId);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }
}

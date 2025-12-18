using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace Tetris_Game
{
    internal class DatabaseManager
    {
        private readonly string connectionString;

        public DatabaseManager()
        {
            connectionString = ConfigurationManager.ConnectionStrings["TetrisGame"].ConnectionString;
        }

        public DataTable ExecuteQuery(string query, params MySqlParameter[] parameters)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters);

            var adapter = new MySqlDataAdapter(cmd);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }

        public void ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            using var conn = new MySqlConnection(connectionString);
            using var cmd = new MySqlCommand(query, conn);
            conn.Open();
            cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();
        }
    }
}

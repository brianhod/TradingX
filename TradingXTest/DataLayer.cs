using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace TradingXDb
{
    public class DataLayer
    {
        #region Structures

        public struct sConnectionString
        {
            public string Server;
            public string Database;
            public string Username;
            public string Password;
        }

        #endregion

        #region Global Variables

        public SqlConnection Cn = new SqlConnection();

        #endregion

        #region Properties

        private string cnstring = String.Empty;
        public string ConnectionString { get { return cnstring; } set { cnstring = value; } }

        public SqlConnection GetConnection { get { return Cn; } }

        #endregion

        #region Contruct

        public DataLayer(string connectionString)
        {
            cnstring = connectionString;
            Connect();
        }

        public bool IsDatabaseConnectionUp()
        {
            try
            {
                return Cn.State == ConnectionState.Open;
            }
            catch (SqlException)
            {
                // There was an error in opening the database so it is must not up.
                return false;
            }
        }

        #endregion

        #region Private Functions



        public bool Connect()
        {
            if (Cn.State != ConnectionState.Closed && Cn.State != ConnectionState.Broken) return true;
            Cn.ConnectionString = cnstring;
            Cn.Open();
            return true;
        }

        public bool Disconnect()
        {
            if (Cn.State != ConnectionState.Closed) { Cn.Close(); }
            return true;
        }

        #endregion

        #region Public Functions

        public bool Execute(string sqlString)
        {
            if (!Connect()) throw new Exception("Could not connect");
            var com = new SqlCommand(sqlString, Cn);
            com.ExecuteScalar();
            return true;
        }

        public int ExecuteGetID(string sqlString)
        {
            if (!Connect()) throw new Exception("Could not connect");
            if (!sqlString.Contains("Scope_Identity()"))
            {
                sqlString += "; SELECT Scope_Identity()";
            }
            var com = new SqlCommand(sqlString, Cn);
            return Convert.ToInt32(com.ExecuteScalar());
        }

        public DataTable GetDataTable(string storedProcName, Dictionary<string, object> Params)
        {
            if (!Connect()) throw new Exception("Could not connect");
            var com = new SqlCommand(storedProcName, Cn);

            foreach (var param in Params)
            {
                com.Parameters.AddWithValue(param.Key, param.Value);
            }
            com.CommandType = CommandType.StoredProcedure;

            var da = new SqlDataAdapter(com);
            var dt = new DataTable();
            da.Fill(dt);
            Disconnect();
            return dt;
        }

        public Boolean ExecuteNonQueryStoredProcedure(string storedProcName, Dictionary<string, object> Params)
        {
            var com = new SqlCommand(storedProcName, Cn);

            foreach (var param in Params)
            {
                com.Parameters.AddWithValue(param.Key, param.Value);
            }
            com.CommandType = CommandType.StoredProcedure;

            com.ExecuteNonQuery();
            Disconnect();
            return true;
        }

        public DataSet GetDataSet(string storedProcName, Dictionary<string, object> Params)
        {
            if (!Connect()) throw new Exception("Could not connect");
            var ds = new DataSet("CIMSPRODUCTS");
            var com = new SqlCommand(storedProcName, Cn);

            foreach (var param in Params)
            {
                com.Parameters.AddWithValue(param.Key, param.Value);
            }
            com.CommandType = CommandType.StoredProcedure;

            var da = new SqlDataAdapter(com);
            da.Fill(ds);
            return ds;
        }
        public List<DataRow> GetList(string storedProcName, Dictionary<string, object> Params)
        {
            if (!Connect()) throw new Exception("Could not connect");
            var com = new SqlCommand(storedProcName, Cn);
            if (Params != null)
            {
                foreach (var param in Params)
                {
                    com.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            com.CommandType = CommandType.StoredProcedure;

            var da = new SqlDataAdapter(com);
            var dt = new DataTable();
            da.Fill(dt);
            return dt.AsEnumerable().ToList();
        }
        #endregion
    }
}

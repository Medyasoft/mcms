using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Unigate.PhotoGallery.AdminPlugin
{
    public class DataEngine
    {
        string mConnectionString = "";
        public DataEngine(string connectionstring)
        {
            this.mConnectionString = connectionstring;
        }

        public DataTable GetDataTable(string query, params SqlParameter[] parameters)
        {
            DataTable retVal = new DataTable();

            SqlConnection conn = new SqlConnection(this.mConnectionString);
            SqlCommand comm = new SqlCommand(query, conn);
            SqlDataAdapter adap = new SqlDataAdapter(comm);

            try
            {
                this.AddParameters(ref comm, ref parameters);
                adap.Fill(retVal);
            }
            catch (Exception)
            {
                retVal = null;
            }
            finally
            {
                conn.Close();

                adap.Dispose();
                comm.Dispose();
                conn.Dispose();
            }

            return retVal;
        }

        public int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            int retVal = 0;

            SqlConnection conn = new SqlConnection(this.mConnectionString);
            SqlCommand comm = new SqlCommand(query, conn);

            try
            {
                this.AddParameters(ref comm, ref parameters);
                this.OpenConnection(ref conn);

                retVal = comm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                retVal = 0;
            }
            finally
            {
                conn.Close();

                comm.Dispose();
                conn.Dispose();
            }

            return retVal;
        }

        public object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            object retVal = null;

            SqlConnection conn = new SqlConnection(this.mConnectionString);
            SqlCommand comm = new SqlCommand(query, conn);

            try
            {
                this.AddParameters(ref comm, ref parameters);
                this.OpenConnection(ref conn);

                retVal = comm.ExecuteScalar();
            }
            catch (Exception)
            {
                retVal = null;
            }
            finally
            {
                conn.Close();

                comm.Dispose();
                conn.Dispose();
            }

            return retVal;
        }


        void OpenConnection(ref SqlConnection conn)
        {
            conn.Close();
            conn.Open();
        }

        void AddParameters(ref SqlCommand comm, ref SqlParameter[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                foreach (SqlParameter item in parameters)
                {
                    comm.Parameters.Add(item);
                }
            }
        }
    }
}
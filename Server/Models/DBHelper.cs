using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Server.Models
{
    public class DBHelper
    {
        public string _ConnectionString;
        public DBHelper(IConfiguration config)
        {
            _ConnectionString = config.GetConnectionString("DB");
        }

        public int ExecuteNonQuery(CommandType cmdType,  string cmdText,  params SqlParameter[] commandParameters)
        {
  
            SqlCommand cmd = new SqlCommand();
  
            using (SqlConnection conn = new SqlConnection(_ConnectionString))
            {
                //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
  
                //清空SqlCommand中的参数列表
                cmd.Parameters.Clear();
                return val;
            }
        }

        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
 
            //判断数据库连接状态
            if (conn.State != ConnectionState.Open)
                conn.Open();
 
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
 
            //判断是否需要事物处理
            if (trans != null)
                cmd.Transaction = trans;
 
            cmd.CommandType = cmdType;
 
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        public DataTable ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
 
            // 在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
            //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            //关闭数据库连接，并通过throw再次引发捕捉到的异常。
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                
                return ConvertDataReaderToDataTable(rdr);
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        private DataTable ConvertDataReaderToDataTable(SqlDataReader dataReader)
        {
            ///定义DataTable  
            DataTable datatable = new DataTable();

            try
            {    ///动态添加表的数据列  
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    DataColumn myDataColumn = new DataColumn();
                    myDataColumn.DataType = dataReader.GetFieldType(i);
                    myDataColumn.ColumnName = dataReader.GetName(i);
                    datatable.Columns.Add(myDataColumn);
                }

                ///添加表的数据  
                while (dataReader.Read())
                {
                    DataRow myDataRow = datatable.NewRow();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        myDataRow[i] = dataReader[i].ToString();
                    }
                    datatable.Rows.Add(myDataRow);
                    myDataRow = null;
                }
                ///关闭数据读取器  
                dataReader.Close();
                return datatable;
            }
            catch (Exception ex)
            {
                ///抛出类型转换错误  
                //SystemError.CreateErrorLog(ex.Message);  
                throw new Exception(ex.Message, ex);
            }
        }
    }
}

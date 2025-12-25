using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using DTO;

public class DAL_DBHelper:DTO_I_DBHelper
{
    //Connection String
    public string StrConnection { get; set; }
    //Connection
    public SqlConnection sqlConnection { get; set; }
    //NpgsqlTransaction 
    public SqlTransaction sqlTransaction { get; set; }


    public DAL_DBHelper(IOptions<DefaultConnect> options) {
        StrConnection = options.Value.connectionString;
    }
    public object ExecuteScalarSProcedureWithTransaction(out string msgError, string sprocedureName, params object[] paramObjects)
    {
        msgError = "";
        object result = null;
        using (SqlConnection connection = new SqlConnection(StrConnection))
        {
            connection.Open();
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sprocedureName;
                    cmd.Transaction = transaction;
                    cmd.Connection = connection;

                    int parameterInput = (paramObjects.Length) / 2;
                    int j = 0;
                    for (int i = 0; i < parameterInput; i++)
                    {
                        string paramName = Convert.ToString(paramObjects[j++]);
                        object value = paramObjects[j++];
                        if (paramName.ToLower().Contains("json"))
                        {
                            cmd.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = paramName,
                                Value = value ?? DBNull.Value,
                                SqlDbType = SqlDbType.NVarChar
                            });
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter(paramName, value ?? DBNull.Value));
                        }
                    }

                    result = cmd.ExecuteScalar();
                    cmd.Dispose();
                    transaction.Commit();
                }
                catch (Exception exception)
                {

                    result = null;
                    msgError = exception.ToString();
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex) { }
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        return result;
    }
    /// <summary>
    /// Execute Procedure return DataTale
    /// </summary>
    /// <param name="msgError">String.Empty when run query success or Message Error when run query happen issue</param>
    /// <param name="sprocedureName">Procedure Name</param>
    /// <param name="paramObjects">List Param Objects, Each Item include 'ParamName' and 'ParamValue'</param>
    /// <returns>Table result</returns>
    public DataTable ExecuteSProcedureReturnDataTable(out string msgError, string sprocedureName, params object[] paramObjects)
    {
        DataTable tb = new DataTable();
        SqlConnection connection;
        try
        {
            SqlCommand cmd = new SqlCommand { CommandType = CommandType.StoredProcedure, CommandText = sprocedureName };
            connection = new SqlConnection(StrConnection);
            cmd.Connection = connection;

            int parameterInput = (paramObjects.Length) / 2;

            int j = 0;
            for (int i = 0; i < parameterInput; i++)
            {
                string paramName = Convert.ToString(paramObjects[j++]).Trim();
                object value = paramObjects[j++];
                if (paramName.ToLower().Contains("json"))
                {
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = paramName,
                        Value = value ?? DBNull.Value,
                        SqlDbType = SqlDbType.NVarChar
                    });
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter(paramName, value ?? DBNull.Value));
                }
            }

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            ad.Fill(tb);
            cmd.Dispose();
            ad.Dispose();
            connection.Dispose();
            msgError = "";
        }
        catch (Exception exception)
        {
            tb = null;
            msgError = exception.ToString();
        }
        return tb;
    }
    /// <summary>
    /// Execute Procedure return DataSet
    /// </summary>
    /// <param name="msgError">String.Empty when run query success or Message Error when run query happen issue</param>
    /// <param name="sprocedureName">Procedure Name</param>
    /// <param name="paramObjects">List Param Objects, Each Item include 'ParamName' and 'ParamValue'</param>
    /// <returns>DataSet result</returns>
    public DataSet ExecuteSProcedureReturnDataset(out string msgError, string sprocedureName, params object[] paramObjects)
    {
        DataSet tb = new DataSet();
        SqlConnection connection;
        try
        {
            SqlCommand cmd = new SqlCommand { CommandType = CommandType.StoredProcedure, CommandText = sprocedureName };
            connection = new SqlConnection(StrConnection);
            cmd.Connection = connection;

            int parameterInput = (paramObjects.Length) / 2;

            int j = 0;
            for (int i = 0; i < parameterInput; i++)
            {
                string paramName = Convert.ToString(paramObjects[j++]);
                object value = paramObjects[j++];
                if (paramName.ToLower().Contains("json"))
                {
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = paramName,
                        Value = value ?? DBNull.Value,
                        SqlDbType = SqlDbType.NVarChar
                    });
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter(paramName, value ?? DBNull.Value));
                }
            }

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            ad.Fill(tb);
            cmd.Dispose();
            ad.Dispose();
            connection.Dispose();
            msgError = "";
        }
        catch (Exception exception)
        {
            tb = null;
            msgError = exception.ToString();
        }

        return tb;
    }
    /// <summary>
    /// Execute Procedure None Query
    /// </summary>
    /// <param name="npgsqlConnection">NpgsqlConnection: Connection use to connect to PostGresDB</param>
    /// <param name="sprocedureName">Procedure Name</param>
    /// <param name="paramObjects">List Param Objects, Each Item include 'ParamName' and 'ParamValue'</param>
    /// <returns>String.Empty when run query success or Message Error when run query happen issue</returns>
}

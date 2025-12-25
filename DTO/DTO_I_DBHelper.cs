using System;
using System.Data;
namespace DTO
{
	public interface DTO_I_DBHelper
	{
        string StrConnection { get; set; }
        object ExecuteScalarSProcedureWithTransaction(out string msgError, string sprocedureName, params object[] paramObjects);
		/// <summary>
		/// Execute Scalar Procedure query List store and command
		/// </summary>
		/// <param name="msgErrors">List Error message</param>
		/// <param name="storeParameterInfos">List Store and ListList Parameter</param>
		/// <returns>List Object return from storeprocedure</returns>
		DataTable ExecuteSProcedureReturnDataTable(out string msgError, string sprocedureName, params object[] paramObjects);
		/// <summary>
		/// Execute Procedure return DataSet
		/// </summary>
		/// <param name="msgError">String.Empty when run query success or Message Error when run query happen issue</param>
		/// <param name="sprocedureName">Procedure Name</param>
		/// <param name="paramObjects">List Param Objects, Each Item include 'ParamName' and 'ParamValue'</param>
		/// <returns>DataSet result</returns>
		DataSet ExecuteSProcedureReturnDataset(out string msgError, string sprocedureName, params object[] paramObjects);
		/// <summary>
		/// Execute Procedure None Query
		/// </summary>
		/// <param name="sqlConnection">sqlConnection: Connection use to connect to SQL Server</param>
		/// <param name="sprocedureName">Procedure Name</param>
		/// <param name="paramObjects">List Param Objects, Each Item include 'ParamName' and 'ParamValue'</param>
		/// <returns>String.Empty when run query success or Message Error when run query happen issue</returns>

	}
}


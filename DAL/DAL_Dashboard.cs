using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using DAL.Models;
namespace DAL
{
    public class DAL_Dashboard
    {
        private DTO_I_DBHelper dbHelper;
        public DAL_Dashboard(DTO_I_DBHelper _DBHelper) {
            dbHelper = _DBHelper;
        }
        public GeneralDashboard trackDailyGeneralDashboard(DateTime day, GeneralDashboard model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbHelper.StrConnection))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("trackDailyGeneralDashboard", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@day", day);

                        cmd.Parameters.Add("@dailyRevenue", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@dailyCustomers", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@dailyOrderQuantitys", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@dailyTotalProducts", SqlDbType.Int).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        model.Revenue = (int)cmd.Parameters["@dailyRevenue"].Value;
                        model.Customers = (int)cmd.Parameters["@dailyCustomers"].Value;
                        model.OrderQuantity = (int)cmd.Parameters["@dailyOrderQuantitys"].Value;
                        model.TotalProducts = (int)cmd.Parameters["@dailyTotalProducts"].Value;
                    }
                    connection.Close();
                }
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DAL: " + ex.Message, ex);
            }
        }
        public GeneralDashboard trackWeeklyGeneralDashboard(DateTime date, GeneralDashboard model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbHelper.StrConnection))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("trackWeeklyGeneralDashboard", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@week", date);

                        cmd.Parameters.Add("@weeklyRevenue", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@weeklyCustomers", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@weeklyOrderQuantitys", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@weeklyTotalProducts", SqlDbType.Int).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        model.Revenue = (int)cmd.Parameters["@weeklyRevenue"].Value;
                        model.Customers = (int)cmd.Parameters["@weeklyCustomers"].Value;
                        model.OrderQuantity = (int)cmd.Parameters["@weeklyOrderQuantitys"].Value;
                        model.TotalProducts = (int)cmd.Parameters["@weeklyTotalProducts"].Value;
                    }
                    connection.Close();
                }
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DAL: " + ex.Message, ex);
            }
        }
        public GeneralDashboard trackMonthlyGeneralDashboard(DateTime date, GeneralDashboard model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbHelper.StrConnection))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("trackMonthlyGeneralDashboard", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@month", date);

                        cmd.Parameters.Add("@monthlyRevenue", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@monthlyCustomers", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@monthlyOrderQuantitys", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@monthlyTotalProducts", SqlDbType.Int).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        model.Revenue = (int)cmd.Parameters["@monthlyRevenue"].Value;
                        model.Customers = (int)cmd.Parameters["@monthlyCustomers"].Value;
                        model.OrderQuantity = (int)cmd.Parameters["@monthlyOrderQuantitys"].Value;
                        model.TotalProducts = (int)cmd.Parameters["@monthlyTotalProducts"].Value;
                    }
                    connection.Close();
                }
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DAL: " + ex.Message, ex);
            }
        }
        public DataTable selectNotifications(int receiverID, int pageSize, int pageNumber)
        {

            try
            {
                string msgError = "";
                DataTable result = dbHelper.ExecuteSProcedureReturnDataTable(out msgError, "selectNotification",
                    "@receiverID", receiverID,
                    "@pageNumber", pageNumber,
                    "@pageSize", pageSize);
                if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(Convert.ToString(result) + msgError);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DAL: " + ex.Message, ex);
            }
        }
        public bool deleteNotification(int id)
        {
            try
            {
                string msgError = "";
                DataTable result = dbHelper.ExecuteSProcedureReturnDataTable(out msgError, "deleteNotification", "@id", id);
                if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(Convert.ToString(result) + msgError);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DAL: " + ex.Message, ex);
            }
        }
        public object calculateRevenue(DateTime lastDate, DateTime currentDate, string type)
        {
            try
            {
                string msgError = "";
                object result = dbHelper.ExecuteScalarSProcedureWithTransaction(
                    out msgError,
                    "matchDoanhThu",
                    "@type", type,
                    "@lastDate", lastDate,
                    "@currentDate", currentDate);

                if (!string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(msgError);
                }

                if (result == null || result == DBNull.Value)
                {
                    return new { message = "Không đủ thông tin để tính toán" };
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DAL: " + ex.Message, ex);
            }
        }

        public object select7dayRevenue(DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                string msgError = "";
                object result = dbHelper.ExecuteSProcedureReturnDataTable(
                    out msgError,
                    "select7DayRevenue",
                    "@dateStart", dateStart,
                    "@dateEnd", dateEnd);
                if (!string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(msgError);
                }

                if (result == null || result == DBNull.Value)
                {
                    return new { message = "Không đủ thông tin để tính toán" };
                }
                return result ;
            }
            catch (Exception ex) {
                throw new Exception("Error in DAL: " + ex.Message, ex);
            }
        }
    }
}

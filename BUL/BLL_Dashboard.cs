using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Helpers;
using DAL;
using DAL.Models;
using DTO;

namespace BLL
{
    public class BLL_Dashboard
    {
        private DAL_Dashboard dal_dashboard;
        public BLL_Dashboard(DAL_Dashboard _dal_dashboard)
        {
            dal_dashboard = _dal_dashboard;
        }
        private GeneralDashboard generaledValueDashboard = new GeneralDashboard();
        public GeneralDashboard trackDailyGeneralDashboard(DateTime day)
        {

            try
            {
                return dal_dashboard.trackDailyGeneralDashboard(day, generaledValueDashboard);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BLL: " + ex.Message, ex);
            }
        }
        public GeneralDashboard trackWeeklyGeneralDashboard(int year, int weekNumber)
        {
            try
            {
                DateTime date = GetStartOf.convertWeek(year, weekNumber);
                return dal_dashboard.trackWeeklyGeneralDashboard(date, generaledValueDashboard);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BLL: " + ex.Message, ex);
            }
        }
        public GeneralDashboard trackMonthlyGeneralDashboard(int monthNumber)
        {
            try
            {
                DateTime date = GetStartOf.convertMonth(monthNumber);
                return dal_dashboard.trackMonthlyGeneralDashboard(date, generaledValueDashboard);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BLL: " + ex.Message, ex);
            }
        }
        public List<Notification> selectNotifications(int receiverID, int pageNumber)
        {
            try
            {
                DataTable table = dal_dashboard.selectNotifications(receiverID, 5, pageNumber);
                return ConvertDataTableToDto.ToList<Notification>(table);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BLL: " + ex.Message, ex);
            }
        }
        public bool deleteNotification(int id)
        {
            try
            {
                return dal_dashboard.deleteNotification(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BLL: " + ex.Message, ex);
            }
        }
        public object CalculateRevenue(string type, int lastPeriod, int currentPeriod, int year)
        {
            try
            {
                DateTime lastDate;
                DateTime currentDate;

                if (type == "week")
                {
                    lastDate = GetStartOf.convertWeek(year, lastPeriod);
                    currentDate = GetStartOf.convertWeek(year, currentPeriod);
                }
                else if (type == "month")
                {
                    lastDate = GetStartOf.convertMonth(lastPeriod);
                    currentDate = GetStartOf.convertMonth(currentPeriod);
                }
                else
                {
                    throw new Exception("Invalid type. Must be 'week' or 'month'");
                }

                return dal_dashboard.calculateRevenue(lastDate, currentDate, type);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BLL: " + ex.Message, ex);
            }
        }
        public object calculateRevenue(string type, DateTime lastPeriod, DateTime currentPeriod)
        {
            try
            {
                return dal_dashboard.calculateRevenue(lastPeriod, currentPeriod, type);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BLL: " + ex.Message, ex);
            }
        }
        public object select7dayRevenue(DateTime date)
        {
            try
            {
                DateTime previousDate = GetDayOf7PreviousDay.GetDateOf7PreviousDay(date);
                var result = dal_dashboard.select7dayRevenue(previousDate, date) as DataTable;

                if (result != null && result.Rows.Count > 0)
                {
                    var labels = result.AsEnumerable()
                                       .Select(row => row["label"].ToString())
                                       .ToList();

                    var data = result.AsEnumerable()
                                     .Select(row => Convert.ToDecimal(row["data"]))
                                     .ToList();

                    var chartData = new
                    {
                        success= true,
                        data = new
                        {
                            chart = new
                            {
                                labels = labels,
                                datasets = new[]
                            {
                new
                {
                    label = "Doanh thu",
                    data = data
                }
            }
                            }
                        }
                    };

                    return chartData;
                }
                else
                {
                    // Xử lý trường hợp không có dữ liệu
                    return new
                    {
                        success= false,
                        message = "Không đủ thông tin để tính toán"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BLL: " + ex.Message, ex);
            }
        }
    }
}

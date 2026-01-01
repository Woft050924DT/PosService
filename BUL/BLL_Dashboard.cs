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
            return dal_dashboard.trackDailyGeneralDashboard(day, generaledValueDashboard);
        }
        public GeneralDashboard trackWeeklyGeneralDashboard(int weekNumber)
        {
            DateTime date = GetStartOf.convertWeek(weekNumber);
            return dal_dashboard.trackWeeklyGeneralDashboard(date, generaledValueDashboard);
        }
        public GeneralDashboard trackMonthlyGeneralDashboard(int monthNumber)
        {
            DateTime date = GetStartOf.convertMonth(monthNumber);
            return dal_dashboard.trackMonthlyGeneralDashboard(date, generaledValueDashboard);
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
    }
}

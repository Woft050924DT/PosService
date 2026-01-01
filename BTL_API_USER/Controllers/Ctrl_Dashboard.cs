using BLL;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BTL_API_USER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private BLL_Dashboard bll_dashboard;

        public DashboardController(BLL_Dashboard _bll_dashboard)
        {
            bll_dashboard = _bll_dashboard;
        }

        // Get: api/Dashboard
        [HttpGet("day")]
        public IActionResult selectByDay(DateTime? date)
        {
            if (!date.HasValue)
            {
                return BadRequest(new { message = "Date parameter is required" });
            }

            var result = bll_dashboard.trackDailyGeneralDashboard(date.Value);
            if (result == null)
            {
                return NotFound(new { message = "No data found for the specified date" });
            }
            return Ok(result);
        }
        [HttpGet("week")]
        public IActionResult selectByWeek(int? weekNumber)
        {
            if (!weekNumber.HasValue)
            {
                return BadRequest(new { message = "Week number parameter is required for weekly tracking" });
            }

            var result = bll_dashboard.trackWeeklyGeneralDashboard(weekNumber.Value);
            if (result == null)
            {
                return NotFound(new { message = "No data found for the specified week" });
            }
            return Ok(result);
        }
        [HttpGet("month")]
        public IActionResult selectByMonth(int? monthNumber)
        {
            if (!monthNumber.HasValue)
            {
                return BadRequest(new { message = "Month number parameter is required for monthly tracking" });
            }

            var result = bll_dashboard.trackMonthlyGeneralDashboard(monthNumber.Value);
            if (result == null)
            {
                return NotFound(new { message = "No data found for the specified month" });
            }
            return Ok(result);
        }

        // get api/Dashboard/notifications
        [HttpGet("notifications")]
        public IActionResult selectNotifications(int? receiverID, int? pageNumber)
        {
            if (!pageNumber.HasValue || !receiverID.HasValue)
            {
                return BadRequest(new { message = "Invalid input information" });
            }
            try
            {
                var result = bll_dashboard.selectNotifications(receiverID.Value, pageNumber.Value);

                if (result == null || !result.Any())
                {
                    return NotFound(new { message = "No notifications found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }


        }
        //Delete api/Dashboard/notification
        [HttpDelete("notification")]
        public IActionResult deleteNotification(int? id) {
            if (!id.HasValue) {
                return BadRequest(new { message = "Invalid input information" });
            }
            try
            {

            bool result=bll_dashboard.deleteNotification(id.Value);
            if (result)
            {
                return Ok(new { message = "Delete notification successfully" });
            }
            else
            {
                return StatusCode(500, new { message = "Failed to delete notification" });
            }
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
        }
    }
}

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
            try
            {

                var result = bll_dashboard.trackDailyGeneralDashboard(date.Value);
                if (result == null)
                {
                    return NotFound(new { message = "No data found for the specified date" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
        }
        [HttpGet("week")]
        public IActionResult selectByWeek(int? weekNumber, int? year)
        {
            if (!weekNumber.HasValue)
            {
                return BadRequest(new { message = "Week number parameter is required for weekly tracking" });
            }
            try
            {

                var result = bll_dashboard.trackWeeklyGeneralDashboard(year.Value, weekNumber.Value);
                if (result == null)
                {
                    return NotFound(new { message = "No data found for the specified week" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
        }
        [HttpGet("month")]
        public IActionResult selectByMonth(int? monthNumber)
        {
            if (!monthNumber.HasValue)
            {
                return BadRequest(new { message = "Month number parameter is required for monthly tracking" });
            }
            try
            {

                var result = bll_dashboard.trackMonthlyGeneralDashboard(monthNumber.Value);
                if (result == null)
                {
                    return NotFound(new { message = "No data found for the specified month" });
                }
                return Ok(result);
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
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

        //Get api/Dashboard/revenue/week-month
        [HttpGet("revenue/week-month")]
        public IActionResult calculateRevenue(string type, int lastDate, int currentDate, int year)
        {
            if (string.IsNullOrEmpty(type) || lastDate <= 0 || currentDate <= 0 || year <= 0)
            {
                return BadRequest(new { message = "Invalid input information" });
            }
            try
            {
                object result = bll_dashboard.CalculateRevenue(type, lastDate, currentDate, year);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
        }
        //Get api/Dashboard/revenue/day
        [HttpGet("revenue/day")]
        public IActionResult calculateRevenue(string type, DateTime lastDate, DateTime currentDate)
        {
            // Kiểm tra điều kiện đầu vào
            if (string.IsNullOrEmpty(type))
            {
                return BadRequest(new { message = "Invalid input information" });
            }

            // Kiểm tra DateTime hợp lệ
            if (lastDate == DateTime.MinValue || currentDate == DateTime.MinValue)
            {
                return BadRequest(new { message = "Invalid date information" });
            }


            try
            {
                object result = bll_dashboard.calculateRevenue(type, lastDate, currentDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
        }
        //Get api/Dashboard/7dayrevenues
        [HttpGet("7dayrevenues")]
        public IActionResult select7dayRevenue(DateTime? date)
        {
            if (!date.HasValue)
            {
                return BadRequest(new { message = "Date parameter is required" });
            }
            try
            {
                object result=bll_dashboard.select7dayRevenue(date.Value);
                return Ok(result);

            }catch(Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
        }

        //Get api/Dashboard/hangtons
        [HttpGet("hangtons")]
        public IActionResult selectHangTonTrongThangTruoc(int? pageNumber)
        {
            if (!pageNumber.HasValue)
            {
                return BadRequest(new { message = "Invalid input information" });
            }
            try
            {
                var result = bll_dashboard.selectHangTonTrongThangTruoc(pageNumber.Value);
                if (result == null || !result.Any())
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm tồn trong kho" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
        }
    }
}

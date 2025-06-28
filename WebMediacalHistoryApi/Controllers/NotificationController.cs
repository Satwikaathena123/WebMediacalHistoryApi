using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Hospital;
using System;
using System.Collections.Generic;
using Hospital.Notification;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationLogic _notificationLogic;

        public NotificationController()
        {
            _notificationLogic = new NotificationLogic();
        }

        [HttpPost("SaveNotification")]
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult SaveNotification(NotificationModel notification)
        {
            int result = _notificationLogic.SaveNotification(notification);
            if (result > 0)
            {
                return Ok(new { message = "Notification saved successfully." });
            }
            return BadRequest(new { message = "Failed to save notification." });
        }

        [HttpGet("GetNotifications")]
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult GetNotifications()
        {
            var notifications = _notificationLogic.FillNotifications();
            var notificationlist = _notificationLogic.NotificationConvertDataTableToList(notifications);
            return Ok(notificationlist);
        }

        [HttpPut("UpdateNotification")]
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult UpdateNotification(NotificationModel notification)
        {
            int result = _notificationLogic.UpdateNotification(notification);
            if (result > 0)
            {
                return Ok(new { message = "Notification updated successfully." });
            }
            return BadRequest(new { message = "Failed to update notification." });
        }

        [HttpDelete("DeleteNotification/{id}")]
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult DeleteNotification(int id)
        {
            int result = _notificationLogic.DeleteNotification(id);
            if (result > 0)
            {
                return Ok(new { message = "Notification deleted successfully." });
            }
            return BadRequest(new { message = "Failed to delete notification." });
        }

        [HttpPost("SendReminder")]
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult SendReminder(int patientID, string message)
        {
            int result = _notificationLogic.SendReminder(patientID, message);
            if (result > 0)
            {
                return Ok(new { message = "Reminder sent successfully." });
            }
            return BadRequest(new { message = "Failed to send reminder." });
        }

        [HttpPost("NotifyAppointmentChange")]
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult NotifyAppointmentChange(int patientID, string message)
        {
            int result = _notificationLogic.NotifyAppointmentChange(patientID, message);
            if (result > 0)
            {
                return Ok(new { message = "Notification sent successfully." });
            }
            return BadRequest(new { message = "Failed to send notification." });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AceMobileAppTemplate.Entities;
using AceMobileAppTemplate.Web.Data;
using AceMobileAppTemplate.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AceMobileAppTemplate.Web.Controllers
{
    [Route("Notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        private NotificationService _notifications;

        public NotificationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, NotificationService notifications)
        {
            _context = context;
            _userManager = userManager;
            _notifications = notifications;
        }

        [HttpGet("Register")]
        public async Task<IActionResult> CreatePushRegistrationId()
        {
            var registrationId = await _notifications.CreateRegistrationId();
            return Ok(registrationId);
        }

        [HttpPut("Enable/{id}")]
        public async Task<IActionResult> RegisterForPushNotifications([FromRoute]string id, [FromBody] DeviceRegistration deviceUpdate)
        {
            var registrationResult = await _notifications.RegisterForPushNotifications(id, deviceUpdate, _userManager, _context);

            if (registrationResult == true)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("SendNotification")]
        public async Task<IActionResult> SendNotification([FromForm]string to, [FromForm]string message)
        {
            var user = await _userManager.FindByNameAsync(to);
            _notifications.SendNotification(message, _context, user);
            return Ok();
        }
    }
}
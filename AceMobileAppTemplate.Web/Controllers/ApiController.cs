using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AceMobileAppTemplate.Entities;
using AceMobileAppTemplate.Web.Data;
using AceMobileAppTemplate.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AceMobileAppTemplate.Web.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiController : ControllerBase
    {
        public ApplicationDbContext _context;
        public UserManager<IdentityUser> _userManager;
        public NotificationService _notifications;

        public ApiController(ApplicationDbContext context,
                             UserManager<IdentityUser> userManager,
                             NotificationService notifications) 
        {
            _context = context;
            _userManager = userManager;
            _notifications = notifications;
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using JWT.Server.Model;
using JWT.Server.Middlewares.Cache;
namespace JWT.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Authentication : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly MemoryCache _cache;
        private readonly IDistributedCache _distributedCache;
        public Authentication(IAuthManager authManager,MemCache memCache, IDistributedCache distributedCache)
        {
            _authManager = authManager;
            _cache = memCache.Cache;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] {"Value1", "Value2"};
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserCredentials userCredentials)
        {
            var token = _authManager.Authenticate(userCredentials.UserName, userCredentials.Password);
            if (token is null)
            {
                return Unauthorized();
            }
            var cacheEntryOptions = new DistributedCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));


            // Save data in redis cache.
            _distributedCache.Set("user_name", Encoding.UTF8.GetBytes(userCredentials.UserName) , cacheEntryOptions);
            return Ok(token);

        }
    }
}

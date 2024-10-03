using Ecommerce.Services.AuthService.Intefaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Controllers.CustomAuthorization
{
    public class RevokedTokenValidator(IDistributedCache cache, ITokenService tokenService)
    {
        private readonly IDistributedCache _cache = cache;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<bool> ValidateAsync(TokenValidatedContext context)
        {
            var token = context.SecurityToken;
            var res = _tokenService.DecodeToken(token.UnsafeToString());
            var storedRevokedToken = await _cache.GetStringAsync($":BlacklistedTokens:{res.Id}");
            if (!string.IsNullOrEmpty(storedRevokedToken) && storedRevokedToken == token.UnsafeToString())
            {
                return false;
            }
            return true;
        }
    }
}
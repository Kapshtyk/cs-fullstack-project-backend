using Ecommerce.Domain.Interfaces;
using Ecommerce.Services.AuthService.DTO;
using Ecommerce.Services.Extensions;
using Ecommerce.Services.AuthService.Intefaces;
using Ecommerce.Domain.Common.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using Ecommerce.Services.Common.Interfaces;
using Ecommerce.Services.AuthService.Exceptions;

namespace Ecommerce.Services.AuthService
{
    public class AuthService(
            IUserRepo userRepo,
            IHashingService hashingService,
            ITokenService tokenService,
            ISaltRepo saltRepo,
            IDistributedCache cache
        ) : IAuthService
    {
        private readonly IUserRepo _userRepo = userRepo;
        private readonly ISaltRepo _saltRepo = saltRepo;
        private readonly IHashingService _hashingService = hashingService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IDistributedCache _cache = cache;
        private readonly DistributedCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(3)
        };

        public async Task<LoginResultDto> LoginAsync(LoginCredentialsDto userCredentials)
        {
            var foundUserByEmail = await _userRepo.GetUserByEmailAsync(userCredentials.Email) ?? throw new WrongCredentialsException();
            var salt = await _saltRepo.GetSaltAsync(foundUserByEmail);
            var isVerified = _hashingService.VerifyValue(userCredentials.Password, foundUserByEmail.Password, salt);

            if (isVerified)
            {
                var tokenOptions = foundUserByEmail.ToTokenOptions();
                var accessToken = _tokenService.GenerateToken(tokenOptions);
                var refreshToken = _tokenService.GenerateToken(tokenOptions, isRefreshToken: true);
                _hashingService.HashValue(refreshToken, out string hashedRefreshToken, salt);

                var storedHashedRefreshTokens = await _cache.GetStringAsync($":RefreshTokens:{tokenOptions.Id}") ?? "";

                var hashedRefreshTokens = storedHashedRefreshTokens.Split(';');

                var newHashedRefreshTokens = string.Join(';', hashedRefreshTokens.Append(hashedRefreshToken));

                await _cache.SetStringAsync($":RefreshTokens:{tokenOptions.Id}", newHashedRefreshTokens, _cacheOptions);

                return new LoginResultDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }

            throw new WrongCredentialsException();
        }

        public async Task<bool> LogoutAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    throw new InvalidTokenException();
                }
                var accessTokenOptions = _tokenService.DecodeToken(token);
                await _cache.RemoveAsync($":RefreshTokens:{accessTokenOptions.Id}");
                await _cache.SetStringAsync($":BlacklistedTokens:{accessTokenOptions.Id}", token, _cacheOptions);
                return true;
            }
            catch (Exception)
            {
                throw new InvalidTokenException();
            }
        }

        public async Task<LoginResultDto> RefreshTokenAsync(string token)
        {
            try
            {
                var refreshTokenOptions = _tokenService.DecodeToken(token);
                var foundUser = await _userRepo.GetByIdAsync(refreshTokenOptions.Id) ?? throw new InvalidTokenException();
                var salt = await _saltRepo.GetSaltAsync(foundUser);

                var storedHashedRefreshTokens = await _cache.GetStringAsync($":RefreshTokens:{refreshTokenOptions.Id}");
                if (string.IsNullOrEmpty(storedHashedRefreshTokens))
                {
                    throw new InvalidTokenException();
                }

                var hashedRefreshTokens = storedHashedRefreshTokens.Split(';');
                var isVerified = false;

                foreach (var storedHashedRefreshToken in hashedRefreshTokens)
                {
                    if (_hashingService.VerifyValue(token, storedHashedRefreshToken, salt))
                    {
                        isVerified = true;
                        break;
                    }
                }

                if (!isVerified)
                {
                    throw new InvalidTokenException();
                }

                var tokenOptions = foundUser.ToTokenOptions();
                var accessToken = _tokenService.GenerateToken(tokenOptions);
                var refreshToken = _tokenService.GenerateToken(tokenOptions, isRefreshToken: true);
                _hashingService.HashValue(refreshToken, out string hashedRefreshToken, salt);

                var newHashedRefreshTokens = string.Join(';', hashedRefreshTokens.Append(hashedRefreshToken));
                await _cache.SetStringAsync($":RefreshTokens:{tokenOptions.Id}", newHashedRefreshTokens, _cacheOptions);

                return new LoginResultDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception)
            {
                throw new InvalidTokenException();
            }
        }
    }
}
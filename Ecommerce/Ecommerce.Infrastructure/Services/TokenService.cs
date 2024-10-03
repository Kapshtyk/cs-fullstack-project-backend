using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ecommerce.Domain.Models;
using Ecommerce.Services.AuthService;
using Ecommerce.Services.AuthService.Intefaces;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Infrastructure.Services
{
	public class TokenService(string privateKey) : ITokenService
	{
		public readonly string _privateKey = privateKey;

		private static readonly Dictionary<string, string> ClaimTypeMapping = new()
		{
			{ ClaimTypes.Name, "unique_name" },
			{ ClaimTypes.Role, "role" },
			{ ClaimTypes.NameIdentifier, "nameid" }
		};
		public string GenerateToken(TokenOptions tokenOptions, bool isRefreshToken = false)
		{
			var handler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_privateKey);
			var credentials = new SigningCredentials(
				new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = GenerateClaims(tokenOptions),
				Expires = isRefreshToken ? DateTime.UtcNow.AddDays(3) : DateTime.UtcNow.AddMinutes(5),
				SigningCredentials = credentials,
			};

			var token = handler.CreateToken(tokenDescriptor);
			return handler.WriteToken(token);
		}

		public bool ValidateToken(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_privateKey);

			var validationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero,
			};

			try
			{
				handler.ValidateToken(token, validationParameters, out _);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static ClaimsIdentity GenerateClaims(TokenOptions tokenOptions)
		{
			var claims = new ClaimsIdentity();

			claims.AddClaim(new Claim(ClaimTypes.Name, tokenOptions.Email));
			claims.AddClaim(new Claim(ClaimTypes.Role, tokenOptions.Role.ToString()));
			claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, tokenOptions.Id.ToString()));

			return claims;
		}

		public TokenOptions DecodeToken(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			if (handler.ReadToken(token) is not JwtSecurityToken jsonToken)
			{
				throw new InvalidOperationException("Invalid token format");
			}

			var nameId = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypeMapping[ClaimTypes.NameIdentifier])?.Value;
			var email = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypeMapping[ClaimTypes.Name])?.Value;
			var role = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypeMapping[ClaimTypes.Role])?.Value;

			if (nameId == null || email == null || role == null)
			{
				throw new InvalidOperationException("Token is missing required claims");
			}

			return new TokenOptions
			{
				Id = int.Parse(nameId),
				Email = email,
				Role = (Role)Enum.Parse(typeof(Role), role),
			};
		}
	}
}
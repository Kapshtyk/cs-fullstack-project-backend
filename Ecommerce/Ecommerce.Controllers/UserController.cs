using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Models;
using Ecommerce.Services.UserService.DTO;
using Ecommerce.Services.UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FluentValidation.Results;
using Ecommerce.Services.Common;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Ecommerce.Domain.Common.Exceptions;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    [SwaggerTag("Read, update and delete users")]
    public class UserController(
        IUserService userService,
        IAuthorizationService authorizationService,
        IUserRepo userRepo
        ) :
        AppController<User, UserFilterOptions, GetUserDto, PartialUpdateUserDto, CreateUserDto, UpdateUserDto>(userService)
    {
        private readonly IUserService _userService = userService;
        private readonly IAuthorizationService _authorizationService = authorizationService;
        private readonly IUserRepo _userRepo = userRepo;

        [HttpPost]
        [Produces("application/json")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create a new user", Description = "Permission: All users")]
        [SwaggerResponse(201, "The user was created successfully", typeof(GetUserDto))]
        [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
        public override async Task<ActionResult<GetUserDto>> CreateItem([FromForm] CreateUserDto dto)
        {
            var validationResult = new CreateUserDtoValidator().Validate(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var user = await _userService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetItem), new { id = user.Id }, user);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Get a list of users", Description = "Permission: Admin")]
        [SwaggerResponse(200, "The users were fetched successfully", typeof(PaginatedResult<User, GetUserDto>))]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        public override async Task<ActionResult<PaginatedResult<User, GetUserDto>>> GetItems([FromQuery] UserFilterOptions filteringOptions)
        {
            return await base.GetItems(filteringOptions);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Get a user by id", Description = "Permission: Admin")]
        [SwaggerResponse(200, "The user was fetched successfully", typeof(GetUserDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(404, "The user was not found", typeof(ProblemDetails))]
        public override async Task<ActionResult<GetUserDto>> GetItem([SwaggerParameter(Description = "User id")] int id)
        {
            return await base.GetItem(id);
        }

        [HttpGet]
        [Route("me")]
        [Authorize]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Get the current user", Description = "Permission: Authenticated")]
        [SwaggerResponse(200, "The user was fetched successfully", typeof(GetUserDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "The user was not found", typeof(ProblemDetails))]
        public async Task<ActionResult<GetUserDto>> GetMe()
        {
            var userClaim = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value ?? throw new EntityNotFoundException<User>();

            if (!int.TryParse(userClaim, out int result))
            {
                throw new EntityNotFoundException<User>();
            }

            return await base.GetItem(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Update an existing user", Description = "Permission: Admin or owner of the user")]
        [SwaggerResponse(200, "The user was updated successfully", typeof(GetUserDto))]
        [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(404, "The user was not found", typeof(ProblemDetails))]
        public override async Task<ActionResult<GetUserDto>> UpdateItem([SwaggerParameter(Description = "User id")] int id, [FromForm] UpdateUserDto dto)
        {
            var validationResult = new UpdateUserDtoValidator().Validate(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var entity = await _userRepo.GetByIdAsync(id);
            var result = await _authorizationService.AuthorizeAsync(HttpContext.User, entity, "Ownership");

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("User cannot update another user");
            }
            var updatedUser = await _userService.UpdateAsync(dto, id);

            return Ok(updatedUser);
        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Partially update an existing user", Description = "Permission: Admin or user")]
        [SwaggerResponse(200, "The user was updated successfully", typeof(GetUserDto))]
        [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(404, "The user was not found", typeof(ProblemDetails))]
        public override async Task<ActionResult<GetUserDto>> PartialUpdateItem([SwaggerParameter(Description = "User id")] int id, [FromForm] PartialUpdateUserDto dto)
        {
            var validationResult = new PartialUpdateUserDtoValidator().Validate(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var entity = await _userRepo.GetByIdAsync(id);
            var result = await _authorizationService.AuthorizeAsync(HttpContext.User, entity, "Ownership");

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("User cannot update another user");
            }

            var updatedUser = await _userService.UpdateAsync(dto, id);

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete a user by id", Description = "Permission: Admin")]
        [SwaggerResponse(204, "The user was deleted successfully")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden", typeof(ProblemDetails))]
        [SwaggerResponse(404, "The user was not found", typeof(ProblemDetails))]
        public override async Task<ActionResult<User>> DeleteItem(int id)
        {
            return await base.DeleteItem(id);
        }
    }
}
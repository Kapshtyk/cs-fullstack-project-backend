using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Ecommerce.Services.FrontPageService.DTO;
using Ecommerce.Services.FrontPageService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Services.Common;
using Ecommerce.Domain.Models;
using Ecommerce.Domain.Filters;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/v1/frontpage")]
    [SwaggerTag("Frontpage content")]
    public class FrontPageController(IFrontPageService FrontPageService) : ControllerBase
    {
        private readonly IFrontPageService _FrontPageService = FrontPageService;

        [HttpPost]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Create a new frontpage")]
        [SwaggerResponse(200, "The frontpage was created", typeof(GetFrontPageDto))]
        [SwaggerResponse(400, "The frontpage was not created", typeof(ValidationProblemDetails))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        public async Task<ActionResult<GetFrontPageDto>> CreateFrontPage([FromForm][SwaggerRequestBody] CreateFrontPageDto FrontPageDto)
        {
            var validationResult = new CreateFrontPageDtoValidator().Validate(FrontPageDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var result = await _FrontPageService.CreateAsync(FrontPageDto);

            return Ok(result);
        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Update a frontpage")]
        [SwaggerResponse(200, "The frontpage was updated", typeof(GetFrontPageDto))]
        [SwaggerResponse(400, "The frontpage was not updated", typeof(ValidationProblemDetails))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "The frontpage was not found")]
        public async Task<ActionResult<GetFrontPageDto>> UpdateFrontPage([FromRoute] int id, [FromForm][SwaggerRequestBody] PartialUpdateFrontPageDto FrontPageDto)
        {
            var validationResult = new PartialUpdateFrontPageDtoValidator().Validate(FrontPageDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var result = await _FrontPageService.UpdateAsync(FrontPageDto, id);

            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all frontpages")]
        [SwaggerResponse(200, "The frontpages were retrieved successfully", typeof(PaginatedResult<FrontPage, GetFrontPageDto>))]
        [Produces("application/json")]
        async public Task<ActionResult<PaginatedResult<FrontPage, GetFrontPageDto>>> GetItems([FromQuery] FrontPageFilterOptions filteringOptions)
        {
            var result = await _FrontPageService.GetAllAsync(filteringOptions);
            return Ok(result);
        }
    }
}
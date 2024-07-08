using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiProject.Entities.Dtos.Incoming.Profile;
using WebApiProject.Entities.Dtos.Generic;
using WebApiProject.Configuration.Messages;
using WebApiProject.Entities.Dtos.Outgoing.Profile;

namespace WebApiProject.Api.Controllers.v1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProfileController : BaseController<ProfileController>
{
    //private IUnitOfWork _unitOfWork;
    //private readonly ILogger<ProfileController> _logger;

    //public ProfileController(
    //    IMapper mapper, 
    //    ILogger<ProfileController> logger, 
    //    IUnitOfWork unitOfWork,
    //    UserManager<IdentityUser> userManager) : base(mapper,unitOfWork, userManager)
    //{
    //    _logger = logger;
    //}

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var loggedInUser = await UserManager.GetUserAsync(HttpContext.User);
        var result = new Result<ProfileDto>();

        if (loggedInUser == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Profile.UserNotFound,
                ErrorMessages.Generic.TypeBadRequest);

            return BadRequest(result);
        }

        var identityId = new Guid(loggedInUser.Id);

        var profile = await UnitOfWork.Users.GetByIdentityId(identityId);

        if (profile == null)
        {
            result.Error = PopulateError(400,
               ErrorMessages.Profile.UserNotFound,
               ErrorMessages.Generic.TypeBadRequest);
            return BadRequest(result);
        }

        var mappedProfile = Mapper.Map<ProfileDto>(profile);

        result.Content = mappedProfile;
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profile)
    {
        var result = new Result<ProfileDto>();
        // If the model is valid
        if (!ModelState.IsValid)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Generic.InvalidPayload,
                ErrorMessages.Generic.TypeBadRequest);

            return BadRequest(result);
        }

        var loggedInUser = await UserManager.GetUserAsync(HttpContext.User);

        if (loggedInUser == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Profile.UserNotFound,
                ErrorMessages.Generic.TypeBadRequest);

            return BadRequest(result);
        }

        var identityId = new Guid(loggedInUser.Id);

        var userProfile = await UnitOfWork.Users.GetByIdentityId(identityId);

        if (userProfile == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Profile.UserNotFound,
                ErrorMessages.Generic.TypeBadRequest);

            return BadRequest(result);
        }

        userProfile.Address = profile.Address;
        userProfile.Sex = profile.Sex;
        userProfile.PhoneNumber = profile.PhoneNumber;
        userProfile.Country = profile.Country;

        var isUpdated = await UnitOfWork.Users.UpdateUserProfile(userProfile);

        if (isUpdated)
        {
            await UnitOfWork.CompleteAsync();
            var mappedProfile = Mapper.Map<ProfileDto>(userProfile);
            result.Content = mappedProfile;
            return Ok(result);
        }

        result.Error = PopulateError(500,
            ErrorMessages.Generic.SomethingWentWrong,
            ErrorMessages.Generic.UnableToProcess);

        return BadRequest(result);
    }
}

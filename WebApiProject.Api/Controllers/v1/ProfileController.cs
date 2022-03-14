using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiProject.DataService.IConfiguration;
using WebApiProject.Entities.Dtos.Incoming.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApiProject.Entities.Dtos.Generic;
using WebApiProject.Entities.DbSet;
using WebApiProject.Configuration.Messages;
using AutoMapper;
using WebApiProject.Entities.Dtos.Outgoing.Profile;

namespace WebApiProject.Api.Controllers.v1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProfileController : BaseController
{
    //private IUnitOfWork _unitOfWork;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        IMapper mapper, 
        ILogger<ProfileController> logger, 
        IUnitOfWork unitOfWork,
        UserManager<IdentityUser> userManager) : base(mapper,unitOfWork, userManager)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
        var result = new Result<ProfileDto>();

        if (loggedInUser == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Profile.UserNotFound,
                ErrorMessages.Generic.TypeBadRequest);

            return BadRequest(result);
        }

        var identityId = new Guid(loggedInUser.Id);

        var profile = await _unitOfWork.Users.GetByIdentityId(identityId);

        if (profile == null)
        {
            result.Error = PopulateError(400,
               ErrorMessages.Profile.UserNotFound,
               ErrorMessages.Generic.TypeBadRequest);
            return BadRequest(result);
        }

        var mappedProfile = _mapper.Map<ProfileDto>(profile);

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

        var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

        if (loggedInUser == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Profile.UserNotFound,
                ErrorMessages.Generic.TypeBadRequest);

            return BadRequest(result);
        }

        var identityId = new Guid(loggedInUser.Id);

        var userProfile = await _unitOfWork.Users.GetByIdentityId(identityId);

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

        var isUpdated = await _unitOfWork.Users.UpdateUserProfile(userProfile);

        if (isUpdated)
        {
            await _unitOfWork.CompleteAsync();
            var mappedProfile = _mapper.Map<ProfileDto>(userProfile);
            result.Content = mappedProfile;
            return Ok(result);
        }

        result.Error = PopulateError(500,
            ErrorMessages.Generic.SomethingWentWrong,
            ErrorMessages.Generic.UnableToProcess);

        return BadRequest(result);
    }
}

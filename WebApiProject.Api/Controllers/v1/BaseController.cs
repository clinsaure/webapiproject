using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiProject.DataService.IConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiProject.Entities.Dtos.Errors;
using AutoMapper;

namespace WebApiProject.Api.Controllers.v1;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class BaseController : ControllerBase
{
    public IUnitOfWork _unitOfWork;
    public UserManager<IdentityUser> _userManager;
    public readonly IMapper _mapper;

    public BaseController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    internal Error PopulateError(int code, string message, string type)
    {
        return new Error()
        {
            Code = code,
            Message = message,
            Type = type
        };
    }
}

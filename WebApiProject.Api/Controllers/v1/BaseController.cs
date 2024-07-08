using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApiProject.DataService.IConfiguration;
using WebApiProject.Entities.Dtos.Errors;
using AutoMapper;

namespace WebApiProject.Api.Controllers.v1;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class BaseController<T> : ControllerBase where T : BaseController<T>
{
    private ILogger<T>? _logger;
    private IUnitOfWork? _unitOfWork;
    private UserManager<IdentityUser>? _userManager;
    private IMapper? _mapper;

    protected ILogger<T> Logger
           => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
    protected IUnitOfWork UnitOfWork
        => _unitOfWork ??= HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
    protected IMapper Mapper
        => _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();
    protected UserManager<IdentityUser> UserManager
        => _userManager ??= HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();

    //public BaseController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
    //{
    //    _mapper = mapper;
    //    _unitOfWork = unitOfWork;
    //    _userManager = userManager;
    //}

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

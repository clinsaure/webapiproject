using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiProject.DataService.Data;
using WebApiProject.DataService.IConfiguration;
using WebApiProject.Entities.DbSet;
using WebApiProject.Entities.Dtos.Incoming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiProject.Entities.Dtos.Generic;
using WebApiProject.Configuration.Messages;
using AutoMapper;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using WebApiProject.DataService.Repositories;
using WatchDog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiProject.Api.Controllers.v1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : BaseController
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IMapper mapper,
        ILogger<UsersController> logger,
        IUnitOfWork unitOfWork,
        UserManager<IdentityUser> userManager) : base(mapper, unitOfWork, userManager)
    {
        _logger = logger;
    }

    // GET: api/<UsersController>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _unitOfWork.Users.All();
        var result = new PagedResult<User>
        {
            Content = users.ToList(),
            ResultCount = users.Count()
        };

        WatchLogger.Log("{Controller} All Users");
        return Ok(result);
    }

    // GET api/<UsersController>/5
    [HttpGet("{id}")]
    //[Route("v1", Name = "GetUser")]
    public async Task<IActionResult> Get(Guid id)
    {
        var user = await _unitOfWork.Users.GetById(id);
        var result = new Result<User>();
        if (user != null)
        {
            result.Content = user;
            return Ok(result);
        }

        result.Error = PopulateError(400,
            ErrorMessages.Users.UserNotFound,
            ErrorMessages.Generic.ObjectNotFound);

        return BadRequest(result);
    }

    // POST api/<UsersController>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserDto user)
    {
        var _mappedUser = _mapper.Map<User>(user);

        //var _user = new User
        //{
        //    FirstName = user.FirstName,
        //    LastName = user.LastName,
        //    Email = user.Email,
        //    DateOfBirth = Convert.ToDateTime(user.DateOfBirth),
        //    Phone = user.Phone,
        //    Country = user.Country,
        //    Status = 1
        //};

        await _unitOfWork.Users.Add(_mappedUser);
        await _unitOfWork.CompleteAsync();

        var result = new Result<UserDto>
        {
            Content = user
        };

        return CreatedAtRoute("GetUser", new { id = _mappedUser.Id }, result);
    }

    // PUT api/<UsersController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<UsersController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }

}

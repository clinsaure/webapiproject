using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApiProject.Authentication;
using WebApiProject.Authentication.Models.DTO.Generic;
using WebApiProject.Authentication.Models.DTO.Incoming;
using WebApiProject.Authentication.Models.DTO.Outgoing;
using WebApiProject.DataService.IConfiguration;
using WebApiProject.Entities.DbSet;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using WebApiProject.Api.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace WebApiProject.Api.Controllers.v1;
public class AccountsController : BaseController
{

    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly JwtConfig _jwtConfig;
    private readonly IEmailService _emailService;

    public AccountsController(
        IMapper mapper,
        TokenValidationParameters tokenValidationParameters,
        IOptionsMonitor<JwtConfig> optionsMonitor,
        IEmailService emailService,
        IUnitOfWork unitOfWork, 
        UserManager<IdentityUser> userManager) : base(mapper,unitOfWork, userManager)
    {
        _jwtConfig = optionsMonitor.CurrentValue;
        _tokenValidationParameters = tokenValidationParameters;
        _emailService = emailService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="registrationDto"></param>
    /// <returns></returns>
    // Register Action
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto registrationDto)
    {
        // check the model or obj we are recieving is valid

        if (ModelState.IsValid)
        {
            // Check if the email already exist
            var userExist = await _userManager.FindByEmailAsync(registrationDto.Email);

            if (userExist != null) // email is already in the table
            {
                return BadRequest(new UserRegistrationResponseDto()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                    {
                        "Email already in use"
                    }
                });
            }

            // Add the user
            var newUser = new IdentityUser()
            {
                Email = registrationDto.Email,
                UserName = registrationDto.Email,
                EmailConfirmed = false //true // ToDo Build
            };

            var isCreated = await _userManager.CreateAsync(newUser, registrationDto.Password);

            if (isCreated.Succeeded) // when the registration has failed
            {

                // Adding user to the database
                var _user = new User
                {
                    IdentityId = new Guid(newUser.Id),
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    Email = registrationDto.Email,
                    DateOfBirth = DateTime.UtcNow,
                    PhoneNumber = "",
                    Country = "",
                    Status = 0
                };

                await _unitOfWork.Users.Add(_user);
                await _unitOfWork.CompleteAsync();

                // Create a jwt token
                var token = await GenerateJwtToken(newUser);

                var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                var encodeEmailToken = Encoding.UTF8.GetBytes(confirmToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodeEmailToken);

                var email_body = "Please confirm your email adress <a href=\"#URL#\">Click here";

                //var confirmationLink = Url.Action(nameof(ConfirmEmail), controller: "/api/v1/Accounts",
                //    values: new { userId = newUser.Id, token = validEmailToken });

                var confirmationLink = $"/api/v1/Accounts/ConfirmEmail?userId={newUser.Id}&token={validEmailToken}";

                var callback_url = Request.Scheme + "://" + Request.Host + confirmationLink;

                //var body = email_body.Replace("#URL#", System.Text.Encodings.Web.HtmlEncoder.Default.Encode(callback_url));
                var bodyMessage = email_body.Replace("#URL#", callback_url);

                // SEND EMAIL
                var bodySubject = "Confirm the Email Adress";
                var result = await _emailService.SendEMailAsync(newUser.Email, bodySubject, bodyMessage);

                if (result.IsSuccessStatusCode)
                    return Ok("Please verify your email, through the verification email we have just sent.");

                return Ok("Please request an email verification Link");               
            }

            return BadRequest(new UserRegistrationResponseDto()
            {
                IsSuccess = isCreated.Succeeded,
                Errors = isCreated.Errors.Select(x => x.Description).ToList()
            });
        }
        else // Invalid Object
        {
            return BadRequest(new UserRegistrationResponseDto
            {
                IsSuccess = false,
                Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
            });
        }

    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if(userId == null || token == null)
        {
            return BadRequest(new UserRegistrationResponseDto()
            {
                IsSuccess = false,
                Errors = new List<string> { "invalid email confirmation url" }
            });
        }

        var user = await _userManager.FindByIdAsync(userId);

        if(user == null)
        {
            return BadRequest(new UserRegistrationResponseDto()
            {
                IsSuccess = false,
                Errors = new List<string> { "invalid email parameters" }
            });
        }



        token = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            await _unitOfWork.Users.UpdateUserStatus(new Guid(user.Id));
            await _unitOfWork.CompleteAsync();
        }

        var status = result.Succeeded
            ? "Thank you for confirming your email"
            : "Your email is not confirmed, please try again later";

        return Ok(status);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginDto)
    {
        if (ModelState.IsValid)
        {
            // 1 - Check if email exist
            var userExist = await _userManager.FindByEmailAsync(loginDto.Email);

            if (userExist == null)
            {
                return BadRequest(new UserLoginResponseDto()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                        {
                            "Invalid authentication request"
                        }
                });
            }

            if (!userExist.EmailConfirmed)
                return BadRequest(new UserLoginResponseDto()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                        {
                            "Email needs to be confirmed"
                        }
                });

            // 2 - Check if the user has a valid password
            var isCorrect = await _userManager.CheckPasswordAsync(userExist, loginDto.Password);

            if (isCorrect)
            {
                // We need to generate a Jwt Token 
                var jwtToken = await GenerateJwtToken(userExist);

                return Ok(new UserLoginResponseDto()
                {
                    IsSuccess = true,
                    Token = jwtToken.JwtToken,
                    RefreshToken = jwtToken.RefreshToken,
                });
            }
            else
            {
                // Password doesn´t match
                return BadRequest(new UserLoginResponseDto()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                        {
                            "Invalid authentication request"
                        }
                });
            }
        }
        else // Invalid Object
        {
            return BadRequest(new UserLoginResponseDto()
            {
                IsSuccess = false,
                Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
            });
        }
    }

    [HttpPost]
    [Route("RefeshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
    {
        if (ModelState.IsValid)
        {
            // check if the token is valid
            var result = await VerifyToken(tokenRequestDto);

            if (result == null)
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                    {
                        "token validation failed"
                    }
                });
            }

            return Ok(result);
        }
        else // Invalid Object
        {
            return BadRequest(new UserRegistrationResponseDto
            {
                IsSuccess = false,
                Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
            });
        }
    }

    private async Task<AuthResult> VerifyToken(TokenRequestDto tokenRequestDto)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var tokenValidationParameters = _tokenValidationParameters.Clone();
            tokenValidationParameters.ValidateLifetime = false;

            // we need to check the validity of the token
            var principal = tokenHandler.ValidateToken(tokenRequestDto.Token, tokenValidationParameters, out var validatedToken);

            // We need to validate the results that has been generated for us
            // Validate if the string is an actual JWT token not a random string
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                // check if the jwt token is created with the same algorith as our jwt token
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                if (!result)
                    return null;
            }

            // We need to check the expiry date
            var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            // convert to date to check
            var expDate = UnixTimeStampToDateTime(utcExpiryDate);

            // checking if the jwt token has expired
            if (expDate > DateTime.UtcNow)
            {
                return new AuthResult()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                        {
                            "Jwt token has not expired"
                        }
                };
            }

            // check if the refresh token exist
            var refreshTokenExist = await _unitOfWork.RefreshTokens.GetByRefreshToken(tokenRequestDto.RefreshToken);

            if (refreshTokenExist == null)
            {
                return new AuthResult()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                        {
                            "Invalid refresh token"
                        }
                };
            }

            // Check the expiry date of a refresh token
            if (refreshTokenExist.ExpiryDate < DateTime.UtcNow)
            {
                return new AuthResult()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                        {
                            "Refresh token has expired, please login again"
                        }
                };
            }

            // check if refresh token has been used or not
            if (refreshTokenExist.IsUsed)
            {
                return new AuthResult()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                        {
                            "Refresh token has been used, it canno be reused"
                        }
                };
            }

            // check if refresh token has been revoked
            if (refreshTokenExist.IsRevoked)
            {
                return new AuthResult()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                        {
                            "Refresh token has been revoked, it canno be used"
                        }
                };
            }

            var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            // check if refresh token has been revoked
            if (refreshTokenExist.JwtId != jti)
            {
                return new AuthResult()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                        {
                            "Refresh token reference does not match the jwt token"
                        }
                };
            }

            // Start processing and get a new token
            refreshTokenExist.IsUsed = true;

            var updateResult = await _unitOfWork.RefreshTokens.MarkRefreshTokenAsUsed(refreshTokenExist);
            if (updateResult)
            {
                await _unitOfWork.CompleteAsync();

                // Get the user to generate a new jwt token
                var dbUser = await _userManager.FindByIdAsync(refreshTokenExist.UserId);

                if (dbUser == null)
                {
                    return new AuthResult()
                    {
                        IsSuccess = false,
                        Errors = new List<string>()
                        {
                            "Error processing request"
                        }
                    };
                }

                // generate a jwt token 
                var tokens = await GenerateJwtToken(dbUser);

                return new AuthResult
                {
                    Token = tokens.JwtToken,
                    IsSuccess = true,
                    RefreshToken = tokens.RefreshToken,
                };
            }

            return new AuthResult()
            {
                IsSuccess = false,
                Errors = new List<string>()
                        {
                            "Error processing request"
                        }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            // ToDo : Add better Error handling, and add a logger
            return null;
        }
    }

    private static DateTime UnixTimeStampToDateTime(long unixDate)
    {
        // Sets the time to 1, Jan, 1970
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        // Add the number of seconds from 1 Jan 1970
        dateTime = dateTime.AddSeconds(unixDate).ToUniversalTime();
        return dateTime;
    }

    private async Task<TokenData> GenerateJwtToken(IdentityUser user)
    {
        // the handler is going to be responsible for creating the token
        var jwtHandler = new JwtSecurityTokenHandler();

        // Get the security key
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim("Id", user.Id),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email), // unique id
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // used by the refreshtoken
                    //new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),
            Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame), // ToDo update the expiration time to minutes
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature  // ToDo review the algorithm
            )
        };

        // generate the security token
        var token = jwtHandler.CreateToken(tokenDescriptor);

        // convert the security obj token 
        var jwtToken = jwtHandler.WriteToken(token);

        // Generate a refresh token
        var refreshToken = new RefreshToken
        {
            AddedDate = DateTime.UtcNow,
            Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}", // create a method to generate a random string and attach  a certain guid
            UserId = user.Id,
            IsRevoked = false,
            IsUsed = false,
            Status = 1,
            JwtId = token.Id,
            ExpiryDate = DateTime.UtcNow.AddMonths(6),
        };

        await _unitOfWork.RefreshTokens.Add(refreshToken);
        await _unitOfWork.CompleteAsync();

        var tokenData = new TokenData
        {
            JwtToken = jwtToken,
            RefreshToken = refreshToken.Token,
        };

        return tokenData;
    }

    private static string RandomStringGenerator(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

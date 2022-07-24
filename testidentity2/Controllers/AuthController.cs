using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using testidentity2.Data.DTOs;
using testidentity2.Data.Models;
using testidentity2.Extensions;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace testidentity2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly testidentity2.Services.ITokenService _tokenService;
        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, testidentity2.Services.ITokenService tokenService)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._tokenService = tokenService;
        }
        // GET: api/<AuthController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            // _signInManager.Context.User
            return new string[] { "value1", "value2" };
        }

        // GET api/<AuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost("SignIn")]
        public async Task<ActionResult<Response>> SignIn([FromBody] Request request)
        {
            var response = new Response();

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {

                return NotFound();
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

            if (result.Succeeded == false)
            {
                return BadRequest();
            }

            //response.Data = new SignInApiModel.Response();


            response.AccessToken = _tokenService.GenerateAccessToken(await user.GetClaimsAsync(_userManager));
            response.RefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = response.RefreshToken;

            await _userManager.UpdateAsync(user);

            return Ok(response);
        }

        [HttpPost("hangePassword")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto requestModel)
        {
            var user = await _userManager.GetUserAsync(User);


            var correctPassword = await _userManager.CheckPasswordAsync(user, requestModel.OldPassword);

            if (correctPassword == false)
            {

                return BadRequest();
            }

            var result = await _userManager.ChangePasswordAsync(user, requestModel.OldPassword, requestModel.NewPassword);

            if (result.Succeeded == false)
            {
                return BadRequest();
            }
            else
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }

            return Ok();
        }
        [HttpPost("Refresh")]
        public async Task<ActionResult<Response>> Refresh(Response model)
        {
            var princibles = _tokenService.GetClaimsFromExpiredToken(model.AccessToken);

            var user = await _userManager.GetUserAsync(princibles);

            if (user == null)
            {
                return BadRequest("Invaled Access Token");
            }

            if (model.RefreshToken != user.RefreshToken)
            {
                return BadRequest("Invaled Refresh Token");
            }

            var response = new Response();


            response.RefreshToken = _tokenService.GenerateRefreshToken(); ;

            user.RefreshToken = response.RefreshToken;

            response.AccessToken = _tokenService.GenerateAccessToken(await user.GetClaimsAsync(_userManager));

            await _userManager.UpdateAsync(user);

            return response;
        }

        /// <summary>
        /// Delete the current refresh token of the current user
        /// </summary>
        /// <returns></returns>
        [HttpPost("Revoke")]
        [Authorize]
        public async Task<ActionResult> Revoke()
        {
            var user = await _userManager.GetUserAsync(User);
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
            return Ok();
        }
    }
}

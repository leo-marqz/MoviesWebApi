using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoviesWebApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoviesWebApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : CustomBaseController
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IDataProtector dataProtector;

        public AccountsController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            IMapper mapper
            )
            :base(context, mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost("register", Name = "registerUser")]
        [AllowAnonymous]
        public async Task<ActionResult<UserToken>> Register(UserCredentials userCredentials)
        {
            var user = new IdentityUser
            {
                UserName = userCredentials.Email,
                Email = userCredentials.Email,
            };
            var result = await userManager.CreateAsync(user, userCredentials.Password);
            if (result.Succeeded)
            {
                return await CreateToken(userCredentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login", Name = "loginUser")]
        [AllowAnonymous]
        public async Task<ActionResult<UserToken>> Login(UserCredentials userCredentials)
        {
            var result = await signInManager
                .PasswordSignInAsync(
                userCredentials.Email,
                userCredentials.Password,
                isPersistent: false,
                lockoutOnFailure: false
                );
            if (result.Succeeded)
            {
                return await CreateToken(userCredentials);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpGet("renew-token", Name = "renewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> RenewToken()
        {
            var userCredentials = new UserCredentials()
            {
                Email = HttpContext.User.Identity.Name
            };
            return await CreateToken(userCredentials);
        }

        private async Task<UserToken> CreateToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, userCredentials.Email),
                new Claim(ClaimTypes.Name, userCredentials.Email)
            };
            var user = await userManager.FindByEmailAsync(userCredentials.Email);
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

            var claimsDB = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(30);
            var securityToken = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
                );
            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expires = expires
            };
        }

        [HttpGet("users", Name = "getUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<DisplayUser>>> Get([FromQuery] Pagination pagination)
        {
            var queryable = context.Users.AsQueryable();
            queryable = queryable.OrderBy(x => x.Email);
            return await Get<IdentityUser, DisplayUser>(pagination);
        }

        [HttpGet("Roles", Name = "getRoles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<string>>> GetRoles()
        {
            return await context.Roles.Select(x => x.Name).ToListAsync();
        }

        [HttpPost("assign-role", Name = "assignRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> AssignRole(UpdateRole updateRole)
        {
            var user = await userManager.FindByIdAsync(updateRole.UserId);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, updateRole.Name));
            return NoContent();
        }

        [HttpPost("remove-role", Name = "removeRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> RemoveRole(UpdateRole updateRole)
        {
            var user = await userManager.FindByIdAsync(updateRole.UserId);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, updateRole.Name));
            return NoContent();
        }




        //-------------------------------------------------------------------------

        //[HttpPost("promote-to-admin", Name = "promoteToAdmin")]
        //public async Task<ActionResult> PromoteToAdmin(ProposedAdministrator admin)
        //{
        //    var user = await userManager.FindByEmailAsync(admin.Email);
        //    await userManager.AddClaimAsync(user, new Claim("IsAdmin", "1"));
        //    return NoContent();
        //}

        //[HttpPost("remove-admin", Name = "removeAdmin")]
        //public async Task<ActionResult> RemoveAdmin(ProposedAdministrator admin)
        //{
        //    var user = await userManager.FindByEmailAsync(admin.Email);
        //    await userManager.RemoveClaimAsync(user, new Claim("IsAdmin", "1"));
        //    return NoContent();
        //}

    }
}

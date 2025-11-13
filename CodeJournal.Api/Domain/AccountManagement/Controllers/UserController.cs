using CodeJournal.Api.Domain.AccountManagement.Dtos;
using CodeJournal.Api.Domain.AccountManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeJournal.Api.Domain.AccountManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAll()
        {
            var users = await userRepository.GetAllAsync();

            var userList = new List<UserDto>();
            foreach (var user in users)
            {
                userList.Add(new UserDto()
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName,
                    Email = user.Email,
                });
            }

            return Ok(userList);
        }


        [HttpPost]
        [Route("add")]
        // [Authorize]
        public async Task<IActionResult> AddUser([FromBody] AddUserDto dto)
        {
            var identityUser = new IdentityUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
            };
            var roles = new List<string> { "Reader", };

            if (dto.AdminCheckBox)
            {
                roles.Add("Writer");
            }

            var result = await userRepository.Add(identityUser, dto.Password, roles);

            if (!result)
            {
                return BadRequest();
            }
//save
            return Ok(result);
        }
    }
}
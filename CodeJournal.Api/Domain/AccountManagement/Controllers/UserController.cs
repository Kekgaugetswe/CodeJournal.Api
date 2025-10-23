using CodeJournal.Api.Domain.AccountManagement.Dtos;
using CodeJournal.Api.Domain.AccountManagement.Repositories;
using Microsoft.AspNetCore.Http;
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
    }
}

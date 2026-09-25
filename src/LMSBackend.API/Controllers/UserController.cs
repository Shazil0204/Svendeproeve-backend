using LMSBackend.Application.Abstractions.Users;
using LMSBackend.Application.DTOs.Users;
using LMSBackend.Domain.Enums.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Teacher)}")]
        [HttpGet("current")]
        public async Task<ActionResult<UserResponse>> GetCurrentUser(CancellationToken cancellationToken)
        {
            UserResponse userResponse = await _userService.GetCurrentUserAsync(cancellationToken);
            return Ok(userResponse);
        }

        [Authorize(Roles = nameof(UserRole.Teacher))]
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserResponse>> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            UserResponse userResponse = await _userService.GetUserByIdAsync(userId, cancellationToken);
            return Ok(userResponse);
        }

        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpPut("{userId}")]
        public async Task<ActionResult<UserResponse>> UpdateUserNameAndEmail(Guid userId, UpdateUserNameAndEmailRequest request, CancellationToken cancellationToken)
        {
            UserResponse updatedUser = await _userService.UpdateUserNameAndEmailAsync(userId, request, cancellationToken);
            return Ok(updatedUser);
        }

        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> SoftDeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            await _userService.SoftDeleteUserAsync(userId, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = nameof(UserRole.Teacher))]
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAllUsers(CancellationToken cancellationToken)
        {
            List<UserResponse> users = await _userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }
    }
}

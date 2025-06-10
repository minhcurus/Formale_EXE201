using System.Security.Claims;
using Application;
using Application.DTO;
using Application.Interface;
using Application.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<UserResponse>> GetAll()
        {
            var get = await _userService.GetAllUser();
            return Ok(get);
        }

        [HttpPost("user-profile")]
        public async Task<ResultMessage> GetById(int id)
        {
            var get = await _userService.GetUsersById(id);

            if (get == null)
            {
                return new ResultMessage
                {
                    Success = true,
                    Message = "khong tim thay nguoi dung",
                    Data = null
                };
            }

            return new ResultMessage
            {
                Success = true,
                Message = "tim thay nguoi dung",
                Data = get
            }; ;
        }

        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser(TokenDTO Token)
        {
            var get = await _userService.GetCurrentUser(Token.token);
            return Ok(get);
        }
    

        [HttpPut("update-profile")]
        public async Task<ResultMessage> UpdateProfile(int id,[FromForm] UserDTO userDTO)
        {
            var validator = new UserRequestValidator();
            var validatorResult = validator.Validate(userDTO);

            if (validatorResult.IsValid == false)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Missing value!",
                    Data = validatorResult.ToString()
                };
            }
            var get = await _userService.UpdateProfile(userDTO);

            return new ResultMessage
            {
                Success = true,
                Message = "Cập nhật thành công",
                Data = get
            };
        }

        [HttpDelete("delete-user")]
        public async Task<ResultMessage> Delete(int id)
        {
            var check = await _userService.DeleteProfile(id);
            if (check == false)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Không tìm thấy Id này",
                    Data = null
                };
            }

            return new ResultMessage
            {
                Success = true,
                Message = "Xóa thành công!",
                Data = null
            };
        }

    }
}

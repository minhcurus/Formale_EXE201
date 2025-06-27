using System.Security.Claims;
using Application;
using Application.DTO;
using Application.Interface;
using Application.Service;
using Application.Validation;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly CurrentUserService _currentUser;

        public UserController(IUserService userService, CurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUser = currentUserService;
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        public async Task<ActionResult<UserResponse>> GetAll()
        {
            var get = await _userService.GetAllUser();
            return Ok(get);
        }

        //[HttpPost("user-profile")]
        //public async Task<ResultMessage> GetById(int id)
        //{
        //    var get = await _userService.GetUsersById(id);

        //    if (get == null)
        //    {
        //        return new ResultMessage
        //        {
        //            Success = true,
        //            Message = "khong tim thay nguoi dung",
        //            Data = null
        //        };
        //    }

        //    return new ResultMessage
        //    {
        //        Success = true,
        //        Message = "tim thay nguoi dung",
        //        Data = get
        //    }; ;
        //}

        [Authorize]
        [HttpGet("user-profile")]
        public async Task<IActionResult> GetUserInfo()
        { 
            var get = await _userService.GetCurrentUser();
            return Ok(get);
        }

        [Authorize(Roles = "2")]
        [HttpPut("update-profile")]
        public async Task<ResultMessage> UpdateProfile([FromForm] UserDTO userDTO)
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

            var currentUserId = _currentUser.UserId;
            if (currentUserId == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Lỗi thẩm quyền!",
                    Data = null
                };
            }

            var get = await _userService.UpdateProfile(userDTO);

            return get;
        }

        [Authorize(Roles = "1")]
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

        [Authorize(Roles = "1")]
        [HttpPost("Ban-user")]
        public async Task<ResultMessage> BanUser(int id)
        {
            var check = await _userService.GetUsersById(id);
            if (check == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Không tìm thấy Id này",
                    Data = null
                };
            }

            await _userService.BanUser(id);
            return new ResultMessage
            {
                Success = true,
                Message = "Cấm người dùng thành công!",
                Data = null
            };
        }

        [Authorize(Roles = "1")]
        [HttpPost("UnBan-user")]
        public async Task<ResultMessage> UnBanUser(int id)
        {
            var check = await _userService.GetUsersById(id);
            if (check == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Không tìm thấy Id này",
                    Data = null
                };
            }

            await _userService.UnBanUser(id);
            return new ResultMessage
            {
                Success = true,
                Message = "Đã gỡ cấm người dùng!",
                Data = null
            };
        }
    }
}

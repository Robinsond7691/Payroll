﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Payroll.Core;
using Payroll.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;

        //constructor
        public UserController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(UserLoginDto userDto)
        {
            var user = await _userManager.FindByEmailAsync(userDto.Email);
            if (user == null)
                return Unauthorized("Incorrect username or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, userDto.Password, false);

            //Return a token
            if (result.Succeeded)
                return _mapper.Map<UserDto>(user);

            return Unauthorized("Incorrect username or password.");
        }
    }
}

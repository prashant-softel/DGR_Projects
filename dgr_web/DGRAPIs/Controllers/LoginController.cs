﻿using DGRAPIs.BS;
using DGRAPIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Login.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        
        private readonly iLoginBS _loginBs;
        public LoginController(iLoginBS login)
        {
            _loginBs = login;
        }
        [Route("UserLogin")]
        [HttpGet]
       // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> UserLogin(string username, string password)
        {
           try
            {
              
                var data =await _loginBs.GetUserLogin(username, password);
                return Ok(data);
            }
            catch (Exception ex)
            {
                  
                return BadRequest(ex.Message);
            }
        }
        [Route("WindUserRegistration")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> WindUserRegistration(string fname, string useremail, string role, string userpass )
        {
            try
            {

                var data = await _loginBs.WindUserRegistration(fname, useremail, role, userpass);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("UpdatePassword")]
        [HttpGet]
        public async Task<IActionResult> UpdatePassword(int loginid, string updatepass)
        {
            try
            {
                var data = await _loginBs.UpdatePassword(loginid, updatepass);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindUserInformation")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> GetWindUserInformation(int login_id)
        {
            try
            {

                var data = await _loginBs.GetWindUserInformation(login_id);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarUserInformation")]
        [HttpGet]
        // public async Task<IActionResult> UserLogin(string username, string password)
        public async Task<IActionResult> GetSolarUserInformation(int login_id)
        {
            try
            {

                var data = await _loginBs.GetSolarUserInformation(login_id);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetPageList")]
        [HttpGet]
        public async Task<IActionResult> GetPageList(int login_id, int site_type)
        {
            try
            {

                var data = await _loginBs.GetPageList(login_id, site_type);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindUserAccess")]
        [HttpGet]
        public async Task<IActionResult> GetWindUserAccess(int login_id,string role)
        {
            try
            {

                var data = await _loginBs.GetWindUserAccess(login_id, role);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("SubmitUserAccess")]
        [HttpGet]
        public async Task<IActionResult> SubmitUserAccess(int login_id,string siteList,string pageList, string reportList, string site_type)
        {
            try
            {

                var data = await _loginBs.SubmitUserAccess(login_id, siteList, pageList, reportList, site_type);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
  
        [Route("eQry/{qry}")]
        [HttpGet]
        public async Task<IActionResult> eQry(string qry)
        {
            try
            {
                var data = await _loginBs.eQry(qry);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }



    }


}

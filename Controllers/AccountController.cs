using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBank.Models;
using WebBank.Models.Dtos;
using WebBank.Services;

namespace WebBank.Controllers
{
    [Route("api/web-bank")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountInterface _accountInterface;

        public AccountController(IAccountInterface accountInterface)
        {
            _accountInterface = accountInterface;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseModel<string>>> Login(LoginDto login)
        {
            var token = await _accountInterface.Login(login);
            return Ok(token);
        }

        [Authorize]
        [HttpGet("conta")]
        public async Task<ActionResult<ResponseModel<Account>>> GetAccountData(string email)
        {
            var account = await _accountInterface.GetAccountData(email);
            return Ok(account);
        }

        [HttpPost("cadastrar")]
        public async Task<ActionResult<ResponseModel<Account>>> CreateAccount(CreateAccountDto account)
        {
            var acc = await _accountInterface.CreateAccount(account);
            return Ok(acc);
        }

        [Authorize]
        [HttpPut("depositar")]
        public async Task<ActionResult<ResponseModel<Account>>> AddBalance(TransactionDto transaction)
        {
            var trans = await _accountInterface.AddBalance(transaction);
            return Ok(trans);
        }

        [Authorize]
        [HttpPut("sacar")]
        public async Task<ActionResult<ResponseModel<Account>>> WithdrawAmount(TransactionDto transaction)
        {
            var trans = await _accountInterface.WithdrawAmount(transaction);
            return Ok(trans);
        }
    }
}

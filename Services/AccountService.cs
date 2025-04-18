using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebBank.Data;
using WebBank.Models;
using WebBank.Models.Dtos;

namespace WebBank.Services
{
    public class AccountService : IAccountInterface
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ResponseModel<Account>> AddBalance(TransactionDto transaction)
        {
            var response = new ResponseModel<Account>();

            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == transaction.UserId);

                if(account == null)
                {
                    response.Message = "Conta não encontrada";
                    return response;
                }

                account.Balance += transaction.Value;

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                response.Data = account;
                response.Message = $"Valor adicionado ao saldo com sucesso";

                return response;
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<Account>> CreateAccount(CreateAccountDto account)
        {
            var response = new ResponseModel<Account>();

            try
            {
                var checkAcc = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == account.Email);

                if(checkAcc != null)
                {
                    response.Message = "Este email já está cadastrado";
                    return response;
                }

                var hash = BCrypt.Net.BCrypt.HashPassword(account.Password);

                var acc = new Account
                {
                    Name = account.Name,
                    Email = account.Email,
                    Password = hash
                };

                _context.Accounts.Add(acc);
                await _context.SaveChangesAsync();

                response.Data = acc;
                response.Message = "Conta criada com sucesso";

                return response;
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public Task<ResponseModel<Account>> EditAccount()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<Account>> GetAccountData([FromBody] string email)
        {
            var response = new ResponseModel<Account>();

            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(x=> x.Email == email);

                if(account == null)
                {
                    response.Message = "Conta não encontrada";
                    return response;
                }

                response.Data = account;
                response.Message = $"Conta de {account.Name} encontrada";
                
                return response;
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<string>> Login(LoginDto login)
        {
            var response = new ResponseModel<string>();

            try
            {
                var user = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == login.Email);
                if (user == null)
                {
                    throw new Exception("Usuário não encontrado");
                }

                var checkPass = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);

                if (checkPass == false)
                {
                    throw new Exception("Senha incorreta");
                }

                var jwtSettings = _configuration.GetSection("Jwt");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, login.Email)
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpirationInMinutes"])),
                    signingCredentials: creds);

                response.Data = new JwtSecurityTokenHandler().WriteToken(token);
                response.Message = "Usuário autenticado";

                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<Account>> WithdrawAmount(TransactionDto transaction)
        {
            var response = new ResponseModel<Account>();

            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == transaction.UserId);

                if (account == null)
                {
                    response.Message = "Conta não encontrada";
                    return response;
                }

                if(account.Balance < transaction.Value)
                {
                    response.Data = account;
                    response.Message = "Saldo insuficiente";
                    return response;
                }

                account.Balance -= transaction.Value;

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                response.Data = account;
                response.Message = $"Valor removido do saldo com sucesso";

                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
        }
    }
}

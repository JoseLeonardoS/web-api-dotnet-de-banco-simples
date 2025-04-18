using WebBank.Models;
using WebBank.Models.Dtos;

namespace WebBank.Services
{
    public interface IAccountInterface
    {
        public Task<ResponseModel<Account>> GetAccountData(string email);
        public Task<ResponseModel<string>> Login(LoginDto login);
        public Task<ResponseModel<Account>> CreateAccount(CreateAccountDto account);
        public Task<ResponseModel<Account>> EditAccount();
        public Task<ResponseModel<Account>> AddBalance(TransactionDto transaction);
        public Task<ResponseModel<Account>> WithdrawAmount(TransactionDto transaction);
    }
}

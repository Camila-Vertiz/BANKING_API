using Banking.Application.Responses;
using Banking.Application.Requests.BankAccount;

namespace Banking.Application.Services.Interfaces
{
    public interface IBankAccountService
    {
        Task<BankAccountResponse> CreateAsync(CreateBankAccountRequest request);

        Task<BankAccountResponse?> GetByIdAsync(Guid id);

        Task<IEnumerable<BankAccountResponse>> GetByCustomerIdAsync(Guid customerId);

        Task<IEnumerable<BankAccountResponse>> GetAllAsync();

        Task<BalanceResponse?> GetBalanceAsync(Guid id);
        Task<IEnumerable<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id);
    }
}

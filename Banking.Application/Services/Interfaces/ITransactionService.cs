using Banking.Application.Requests.Transaction;
using Banking.Application.Responses;

namespace Banking.Application.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionResponse>> TransferAsync(TransferRequest request);

        Task<IEnumerable<TransactionResponse>> GetByTraceIdAsync(Guid traceId);

        Task<TransactionResponse> DepositAsync(DepositRequest request);
    }
}
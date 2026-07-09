using Banking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Banking.Domain.Entities
{
    //Transacción de base de datos atómica
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid AccountId { get; private set; }
        public TransactionTypeEnum TransactionType { get; private set; }
        public decimal Amount { get; private set; }
        public CurrencyEnum Currency { get; private set; }
        public DateTimeOffset DateUtc { get; private set; }
        public string Description { get; private set; } = null!;
        public Guid? CorrelationId { get; private set; }

        private Transaction() { }
        public Transaction(Guid accountId, decimal amount, TransactionTypeEnum transactionType, CurrencyEnum currency = CurrencyEnum.Pen, string description = "", Guid? correlationId = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            Id = Guid.NewGuid();
            AccountId = accountId;
            Amount = amount;
            Currency = currency;
            TransactionType = transactionType;
            DateUtc = DateTimeOffset.UtcNow;
            Description = description;
            CorrelationId = correlationId;
        }

    }
}

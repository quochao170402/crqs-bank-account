using CQRS.Read.Models;
using MongoDB.Driver;

namespace CQRS.Read.Services;

public interface IAccountService
{
    Task<bool> CreateAccount(string accountId, string name, decimal initialBalance, string currency = "VND");
    Task<bool> CloseAccount(string accountId, string reason);
    Task<bool> UpdateBalance(string accountId, decimal amount, bool isDeposit = true);
    Task<Account?> GetAccountById(string id);
    Task<(List<Account?> accounts, int total)> GetAccounts(int pageSize, int pageIndex);
}

public class AccountService(IMongoCollection<Account?> collection) : IAccountService
{
    public async Task<bool> CreateAccount(string accountId, string name, decimal initialBalance,
        string currency = "VND")
    {
        var existingAccount = await GetAccountById(accountId);
        if (existingAccount is not null) throw new Exception("Account already exists");
        var account = new Account
        {
            Id = accountId,
            AccountHolder = name,
            Balance = initialBalance,
            Currency = currency,
            IsActive = true
        };

        await collection.InsertOneAsync(account);
        return true;
    }

    public async Task<bool> CloseAccount(string accountId, string reason)
    {
        var account = await GetAccountById(accountId)
                      ?? throw new Exception("Account does not exist");

        account.IsActive = false;
        await collection.ReplaceOneAsync(x => x != null && x.Id == accountId, account);
        return true;
    }

    public async Task<bool> UpdateBalance(string accountId, decimal amount, bool isDeposit = true)
    {
        var account = await GetAccountById(accountId)
                      ?? throw new Exception("Account does not exist");

        if (isDeposit)
            account.Balance += amount;
        else if (account.Balance < amount)
            throw new Exception("Insufficient balance");
        else
            account.Balance -= amount;

        await collection.ReplaceOneAsync(x => x != null && x.Id == accountId, account);
        return true;
    }

    public async Task<Account?> GetAccountById(string id)
    {
        return await collection.Find(x => x != null && x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<(List<Account?> accounts, int total)> GetAccounts(int pageSize, int pageIndex)
    {
        var accounts = await collection
            .Find(product => true) // Condition can be adjusted
            .Skip((pageIndex - 1) * pageSize) // Skip records based on page number
            .Limit(pageSize) // Limit the number of records per page
            .ToListAsync();

        var totalAccount = await collection.CountDocumentsAsync(product => true); // Get total number of records
        return (accounts, (int)totalAccount);
    }
}

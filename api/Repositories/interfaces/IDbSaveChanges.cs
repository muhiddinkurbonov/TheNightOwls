using Microsoft.AspNetCore.Mvc;

public interface DbSaveChanges
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
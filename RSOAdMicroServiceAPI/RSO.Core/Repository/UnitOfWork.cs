using RSO.Core.AdModels;
using RSO.Core.Repository;

namespace AdServiceRSO.Repository;

/// <summary>
/// Implements the <see cref="IUnitOfWork"/> interface.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AdServicesRSOContext _AdServicesRSOContext;
    private bool disposed;

    /// <summary>
    /// Constructor for the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="AdServicesRSOContext ">The <see cref="AdServicesRSOContext "/> context for the database access.</param>
    /// <param name="adRepository">IAdRepository instance.</param>
    public UnitOfWork(AdServicesRSOContext AdServicesRSOContext, IAdRepository adRepository)
    {
        _AdServicesRSOContext = AdServicesRSOContext;
        AdRepository = adRepository;
    }

    ///<inheritdoc/>
    public IAdRepository AdRepository { get; }

    ///<inheritdoc/>
    public async Task<int> SaveChangesAsync() => await _AdServicesRSOContext.SaveChangesAsync();

    /// <summary>
    /// Implements the <see cref="IDisposable"/> interface. Called when we'd like to the dispose the <see cref="UnitOfWork"/> object.
    /// </summary>
    /// <param name="disposing">The </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _AdServicesRSOContext.Dispose();
            }
        }
        disposed = true;
    }

    /// <summary>
    /// Disposes the <see cref="UnitOfWork"/> object and acts as a wrapper for <see cref="Dispose(bool)"/> method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

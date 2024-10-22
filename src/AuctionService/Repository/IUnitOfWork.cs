namespace AuctionService.Repository;

public interface IUnitOfWork : IDisposable
{
    IAuctionRepository _AuctionRepository { get; }
    
}

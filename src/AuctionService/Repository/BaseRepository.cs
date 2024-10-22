using System.Linq.Expressions;
using AuctionService.Data;
using AuctionService.Repository;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repository;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AuctionDbContext _context;

    public BaseRepository(AuctionDbContext context)
    {
        _context = context;
    }

    
}

using System;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("==> Consuming updated auction : " + context.Message.Id);

        var item = await DB.Update<Item>()
        .MatchID(context.Message.Id)
        .Modify(a=> a.Color , context.Message.Color)
        .Modify(a => a.Model , context.Message.Model)
        .Modify(a => a.Make , context.Message.Make)
        .Modify(a => a.Mileage , context.Message.Mileage)
        .Modify(a => a.Year , context.Message.Year)
        .ExecuteAsync();
    }
}

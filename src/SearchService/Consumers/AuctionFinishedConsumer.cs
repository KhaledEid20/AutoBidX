using System;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine("--> Consume The Auction Finished");
        var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

        if(context.Message.ItemSold){
            auction.Winner = context.Message.winner;
            auction.SoldAmount = context.Message.Amount;
        }

        auction.Status = "Finished";
        await auction.SaveAsync();
    }
}

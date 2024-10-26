using System;
using AutoMapper;
using Contracts;
using SearchService.Models;

namespace SearchService.Helper;

public class AutoMapping : Profile
{
public AutoMapping()
{
    CreateMap<AuctionCreated , Item>();
}
}

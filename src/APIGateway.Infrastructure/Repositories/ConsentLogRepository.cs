﻿using APIGateway.Application;
using APIGateway.Domain.Entities;
using Microsoft.Extensions.Logging;
using TGF.CA.Infrastructure.DB.Repository;

namespace APIGateway.Infrastructure.Repositories
{
    public class ConsentLogRepository
        (LegalDbContext aContext, ILogger<ConsentLogRepository> aLogger)
         : RepositoryBase<ConsentLogRepository, LegalDbContext, ConsentLog, Guid>(aContext, aLogger), IConsentLogRepository
    {

    }
}

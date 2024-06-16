﻿using APIGateway.Domain.Entities;
using TGF.Common.ROP.HttpResult;

namespace APIGateway.Application
{
    /// <summary>
    /// Repository to work with the ConsentLog entities in the LegalDb
    /// </summary>
    public interface IConsentLogRepository
    {
        /// <summary>
        /// Add a new consent log in the Database.
        /// </summary>
        /// <param name="aConsentLog">New ConsentLog to be saved in the DB.</param>
        /// <param name="aCancellationToken"></param>
        /// <returns>The newly created ConsentLog</returns>
        Task<IHttpResult<ConsentLog>> AddAsync(ConsentLog aConsentLog, CancellationToken aCancellationToken = default);
        //Task<IHttpResult<ConsentLog>> UpdateAsync(ConsentLog aConsentLog, CancellationToken aCancellationToken = default);
        //Task<IHttpResult<ConsentLog>> DeleteAsync(Guid aConsentLogId, CancellationToken aCancellationToken = default);

    }

}
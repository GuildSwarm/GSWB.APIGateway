﻿using APIGateway.Application.Contracts.Services;
using APIGateway.Application.Mapping;
using APIGateway.Domain.Entities;
using Common.Application.DTOs.Legal;
using TGF.CA.Application;
using TGF.Common.ROP.HttpResult;

namespace APIGateway.Application.UseCases
{
    public class ConsentLegalService(
        IConsentLogRepository aConsentLogRepository,
        IEncryptionService aEncryptionService) 
    : IConsentLegalService
    {
        public async Task<IHttpResult<ConsentLog>> ConsentLegal(string aUserIpAddress, ConsentLogDTO aConsentLogDto, CancellationToken aCancellationToken)
        {
            var lEncryptedUserIpAddress = await aEncryptionService.EncryptAsync(aUserIpAddress);
            var lNewConsentLog = aConsentLogDto.ToEntity(lEncryptedUserIpAddress);
            return await aConsentLogRepository.AddAsync(lNewConsentLog, aCancellationToken);
        }
    }
}
﻿
namespace APIGateway.Application
{
    public interface ITokenRevocationService
    {

        /// <summary>
        /// Determines if the given token has been revoked.
        /// </summary>
        /// <param name="aJwtToken"><see cref="string"/> representing the token.</param>
        /// <returns>True if the token has been revoked, otherwise false.</returns>
        bool IsAccessTokenRevoked(string aJwtToken);

        /// <summary>
        /// Mark as outdated all existing tokens related with any of the DiscordUser ID list provided.
        /// </summary>
        /// <param name="aDiscordUserIdList">List of all DiscordUser ID used to revoke the tokens.</param>
        Task OutdateByDiscordUserListAsync(ulong[] aDiscordUserIdList, CancellationToken aCancellationToken);

        /// <summary>
        /// Mark as outdated all existing tokens related with any of the DiscordRole id list provided.
        /// </summary>
        /// <param name="aDiscordRoleIdList">List of all DiscordRole ID used to revoke the tokens.</param>
        Task OutdateByDiscordRoleListAsync(ulong[] aDiscordRoleIdList, CancellationToken aCancellationToken);

    }

}

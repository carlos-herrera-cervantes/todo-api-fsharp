namespace TodoApi.Managers

open TodoApi.Models
open MongoDB.Driver

type AccessTokenManager private () =

    member val _accessTokenManager : IManager<AccessToken> = null with get, set

    new (accessTokenManager : IManager<AccessToken>) as this = AccessTokenManager() then
        this._accessTokenManager <- accessTokenManager

    interface IAccessTokenManager with

        /// <summary>Creates a new access token document</summary>
        /// <param name="accessToken">Access token object class</param>
        /// <returns>Access token</returns>
        member this.CreateAsync(accessToken : AccessToken) = this._accessTokenManager.CreateAsync accessToken

        /// <summary>Deletes a list of tokens</summary>
        /// <param name="filter">Filter definition</param>
        /// <returns>Tokens removal result</returns>
        member this.DeleteManyAsync(filter : FilterDefinition<AccessToken>) = this._accessTokenManager.DeleteManyAsync filter

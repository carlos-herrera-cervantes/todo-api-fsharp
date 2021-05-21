namespace TodoApi.Repositories

open TodoApi.Models

type AccessTokenRepository private () =

    member val _accessTokenRepository: IRepository<AccessToken> = null with get, set

    new (accessTokenRepository : IRepository<AccessToken>) as this = AccessTokenRepository() then
        this._accessTokenRepository <- accessTokenRepository

    interface IAccessTokenRepository with

        /// <summary>Get list of access token</summary>
        /// <param name="request">Request object model</param>
        /// <returns>List of all access token</returns>
        member this.GetAllAsync(request : Request) = this._accessTokenRepository.GetAllAsync(request)(null)

        /// <summary>Get a access token by specific filter and their references</summary>
        /// <param name="request">Request object model</param>
        /// <returns>Specific access token</returns>
        member this.GetOneAndPopulateAsync(request : Request) =
          this._accessTokenRepository.GetOneAndPopulateAsync(request)(null)
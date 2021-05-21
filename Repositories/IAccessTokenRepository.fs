namespace TodoApi.Repositories

open System.Threading.Tasks
open System.Collections.Generic
open TodoApi.Models

[<AllowNullLiteral>]
type IAccessTokenRepository =

    abstract member GetAllAsync : Request -> Task<List<AccessToken>>
    abstract member GetOneAndPopulateAsync : Request -> Task<AccessToken>
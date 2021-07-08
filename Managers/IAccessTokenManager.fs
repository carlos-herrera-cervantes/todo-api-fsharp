namespace TodoApi.Managers

open System.Threading.Tasks
open MongoDB.Driver
open TodoApi.Models

[<AllowNullLiteral>]
type IAccessTokenManager =

    abstract member CreateAsync : AccessToken -> Task
    abstract member DeleteManyAsync : FilterDefinition<AccessToken> -> Task<DeleteResult>
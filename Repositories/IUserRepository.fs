namespace TodoApi.Repositories

open System.Collections.Generic
open System.Threading.Tasks
open MongoDB.Driver
open TodoApi.Models

[<AllowNullLiteral>]
type IUserRepository =
    abstract member GetAllAsync : Request -> Task<List<User>>
    abstract member GetByIdAsync : string -> Task<User>
    abstract member GetOneAndPopulateAsync : Request -> Task<User>
    abstract member GetOneAsync : FilterDefinition<User> -> Task<User>
    abstract member CountAsync : Request -> Task<int64>
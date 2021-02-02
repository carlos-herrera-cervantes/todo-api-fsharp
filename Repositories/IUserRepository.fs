namespace TodoApi.Repositories

open System.Collections.Generic
open System.Threading.Tasks
open MongoDB.Driver
open TodoApi.Models

[<AllowNullLiteral>]
type IUserRepository =
    abstract member GetAllAsync : unit -> Task<List<User>>
    abstract member GetByIdAsync : string -> Task<User>
    abstract member GetOneAsync : FilterDefinition<User> -> Task<User>
namespace TodoApi.Repositories

open System.Collections.Generic
open System.Threading.Tasks
open TodoApi.Models

[<AllowNullLiteral>]
type IUserRepository =
    abstract member GetAllAsync : unit -> Task<List<User>>
    abstract member GetByIdAsync : string -> Task<User>
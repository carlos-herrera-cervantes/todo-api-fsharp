namespace TodoApi.Repositories

open System.Collections.Generic
open System.Threading.Tasks
open TodoApi.Models

[<AllowNullLiteral>]
type ITodoRepository =
    abstract member GetAllAsync : unit -> Task<List<Todo>>
    abstract member GetByIdAsync : string -> Task<Todo>
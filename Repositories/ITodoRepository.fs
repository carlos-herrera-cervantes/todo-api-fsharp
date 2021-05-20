namespace TodoApi.Repositories

open System.Collections.Generic
open System.Threading.Tasks
open TodoApi.Models

[<AllowNullLiteral>]
type ITodoRepository =
    abstract member GetAllAsync : Request -> Task<List<Todo>>
    abstract member GetByIdAsync : string -> Task<Todo>
    abstract member CountAsync : Request -> Task<int64>
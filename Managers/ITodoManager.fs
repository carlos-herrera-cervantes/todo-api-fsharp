namespace TodoApi.Managers

open Microsoft.AspNetCore.JsonPatch
open MongoDB.Driver
open System.Threading.Tasks
open TodoApi.Models

[<AllowNullLiteral>]
type ITodoManager =
    abstract member CreateAsync : Todo -> Task
    abstract member DeleteByIdAsync : string -> Task<DeleteResult>
    abstract member UpdateByIdAsync : string -> Todo -> JsonPatchDocument<Todo> -> Task<ReplaceOneResult>
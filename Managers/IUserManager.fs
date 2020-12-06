namespace TodoApi.Managers

open Microsoft.AspNetCore.JsonPatch
open MongoDB.Driver
open System.Threading.Tasks
open TodoApi.Models

[<AllowNullLiteral>]
type IUserManager =
    abstract member CreateAsync : User -> Task
    abstract member DeleteByIdAsync : string -> Task<DeleteResult>
    abstract member UpdateByIdAsync : string -> User -> JsonPatchDocument<User> -> Task<ReplaceOneResult>
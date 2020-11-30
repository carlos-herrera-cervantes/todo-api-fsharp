namespace TodoApi.Managers

//open System.Threading.Tasks
open TodoApi.Models

[<AllowNullLiteral>]
type IUserManager =
    abstract member CreateAsync : User -> Async<unit>
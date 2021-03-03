namespace TodoApi.Managers

open System
open System.Linq.Expressions
open MongoDB.Driver
open System.Threading.Tasks

[<AllowNullLiteral>]
type IManager<'a> =
    abstract member CreateAsync : 'a -> Task
    abstract member DeleteByIdAsync : Expression<Func<'a, bool>> -> Task<DeleteResult>
    abstract member UpdateOneAsync : FilterDefinition<'a> -> 'a -> ReplaceOptions -> Task<ReplaceOneResult>
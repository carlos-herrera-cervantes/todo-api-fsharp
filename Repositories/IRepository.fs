﻿namespace TodoApi.Repositories

open System
open System.Linq.Expressions
open System.Collections.Generic
open System.Threading.Tasks
open MongoDB.Driver
open TodoApi.Models

[<AllowNullLiteral>]
type IRepository<'a> =
    abstract member GetAllAsync : Request -> List<Relation> -> Task<List<'a>>
    abstract member GetByIdAsync : Expression<Func<'a, bool>> -> Task<'a>
    abstract member GetOneAsync : FilterDefinition<'a> -> Task<'a>
    abstract member CountAsync : Request -> Task<int64>
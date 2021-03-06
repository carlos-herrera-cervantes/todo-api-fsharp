﻿namespace TodoApi.Repositories

open System
open System.Linq.Expressions
open System.Collections.Generic
open MongoDB.Driver
open MongoDB.Bson
open Microsoft.Extensions.Configuration
open TodoApi.Infrastructure.Contexts
open TodoApi.Models

type Repository<'a> private () =

    member val _context : IMongoCollection<'a> = null with get, set
    member val _configuration : IConfiguration = null with get, set

    new (configuration : IConfiguration) as this = Repository() then
        this._configuration <- configuration
        let client = MongoDBFactory
                      .CrateClient(this._configuration.GetSection("MongoDBSettings")
                      .GetSection("ConnectionString").Value)
        let database = client
                        .GetDatabase(this._configuration.GetSection("MongoDBSettings")
                        .GetSection("Database").Value)
        this._context <- database.GetCollection<'a>(sprintf "%ss" (typeof<'a>.Name.ToLowerInvariant()))

    interface IRepository<'a> with

        /// <summary>Get list of documents</summary>
        /// <param name="request">Request object model</param>
        /// <param name="relations">References to other documents</param>
        /// <returns>List of documents</returns>
        member this.GetAllAsync(request : Request)(relations : List<Relation>) =
            let filter = MongoDBDefinitions<'a>.BuildFilter(request)
            let entities = request.Entities

            request.Page <- if request.Page <= 1 then 0 else request.Page - 1

            let result =
                match String.IsNullOrEmpty(entities) || isNull relations with
                | true ->
                  let sortTypedFilter = MongoDBDefinitions<'a>.BuildSortFilter(request.Sort)
                  this._context
                    .Find(filter)
                    .Skip(Nullable (request.Page * request.PageSize))
                    .Limit(Nullable request.PageSize)
                    .Sort(sortTypedFilter)
                    .ToListAsync()
                | false ->
                  let sortBsonFilter = MongoDBDefinitions<BsonDocument>.BuildSortFilter(request.Sort)
                  MongoDBDefinitions<'a>
                    .Populate(this._context, filter, sortBsonFilter, relations, request)
                    .ToListAsync()

            result

        /// <summary>Get a document by specific ID</summary>
        /// <param name="expression">LINQ expression</param>
        /// <returns>Document</returns>
        member this.GetByIdAsync(expression : Expression<Func<'a, bool>>) =
          this._context.Find(expression).FirstOrDefaultAsync()

        /// <summary>Get a document by specific ID and their references</summary>
        /// <param name="request">Request object model</param>
        /// <param name="relations">References to other documents</param>
        /// <returns>Document</returns>
        member this.GetOneAndPopulateAsync(request : Request)(relations : List<Relation>) =
          let filter = MongoDBDefinitions<'a>.BuildFilter(request)
          let entities = request.Entities
          let result =
                match String.IsNullOrEmpty(entities) || isNull relations with
                | true ->
                  this._context.Find(filter).FirstOrDefaultAsync()
                | false ->
                  MongoDBDefinitions<'a>.PopulateSingular(this._context, filter, relations, request)

          result

        /// <summary>Get one document by specific filter</summary>
        /// <param name="filter">A filter definition with fields to match with a document</param>
        /// <returns>Document</returns>
        member this.GetOneAsync(filter : FilterDefinition<'a>) = this._context.Find(filter).FirstOrDefaultAsync()

        /// <summary>Returns the number of documents in a specific collection</summary>
        /// <param name="request">Request object model</param>
        /// <returns>Number of documents</returns>
        member this.CountAsync(request : Request) =
          let filter = MongoDBDefinitions<'a>.BuildFilter(request)
          this._context.CountDocumentsAsync(filter)

        /// <summary>Returns the number of documents in a specific collection</summary>
        /// <returns>Number of documents</returns>
        member this.CountAsync() = this._context.CountDocumentsAsync(Builders<'a>.Filter.Empty)
namespace TodoApi.Managers

open System
open System.Linq.Expressions
open MongoDB.Driver
open Microsoft.Extensions.Configuration
open TodoApi.Infrastructure.Contexts
open TodoApi.Models

type Manager<'a> private () =

    member val _context : IMongoCollection<'a> = null with get, set
    member val _configuration : IConfiguration = null with get, set

    new (configuration : IConfiguration) as this = Manager() then
        this._configuration <- configuration
        let client = MongoDBFactory
                        .CrateClient(this._configuration.GetSection("MongoDBSettings")
                        .GetSection("ConnectionString").Value)
        let database = client
                        .GetDatabase(this._configuration.GetSection("MongoDBSettings")
                        .GetSection("Database").Value)
        this._context <- database.GetCollection<'a>(sprintf "%ss" (typeof<'a>.Name.ToLowerInvariant()))

    interface IManager<'a> with

        /// <summary>Creates a new document</summary>
        /// <param name="doc">MongoDB document</param>
        /// <returns>Document</returns>
        member this.CreateAsync(doc : 'a) = this._context.InsertOneAsync doc

        /// <summary>Deletes a document</summary>
        /// <param name="expression">LINQ expression to apply</param>
        /// <returns>Removal result</returns>
        member this.DeleteByIdAsync(expression : Expression<Func<'a, bool>>) = this._context.DeleteOneAsync expression

        /// <summary>Deletes a list of documents</summary>
        /// <param name="request">Request object model</param>
        /// <returns>Removal result</returns>
        member this.DeleteManyAsync(request : Request) =
            let filter  = MongoDBDefinitions<'a>.BuildFilter(request)
            this._context.DeleteManyAsync filter

        /// <summary>Deletes a list of documents</summary>
        /// <param name="filter">Filter definition</param>
        /// <returns>Removal result</returns>
        member this.DeleteManyAsync(filter : FilterDefinition<'a>) =
            this._context.DeleteManyAsync filter

        /// <summary>Updates a document</summary>
        /// <param name="filter">FilterDefinition instance</param>
        /// <param name="doc">New document to insert</param>
        /// <param name="options">Options to replace</param>
        /// <returns>Replacement result</returns>
        member this.UpdateOneAsync(filter : FilterDefinition<'a>)(doc : 'a)(options : ReplaceOptions) =
            this._context.ReplaceOneAsync(filter, doc, options)
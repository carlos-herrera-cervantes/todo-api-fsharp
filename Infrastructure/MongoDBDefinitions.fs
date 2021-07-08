namespace TodoApi.Infrastructure.Contexts

open MongoDB.Driver
open MongoDB.Bson
open System
open System.Linq.Expressions
open System.Collections.Generic
open System.Linq
open System.Globalization
open TodoApi.Models
open TodoApi.Constants
open TodoApi.Extensions.StringExtensions

type MongoDBDefinitions<'a> private () =

    /// <summary>Returns a Filter Definition to apply in query</summary>
    /// <param name="request">Request object</param>
    /// <returns>Filter Definition object</returns>
    static member BuildFilter(request : Request) =
        let filters = request.Filters
        let builder =
            match String.IsNullOrEmpty(filters) with
            | true ->
                Builders<'a>.Filter.Empty
            | false ->
                let lambda = MongoDBDefinitions<'a>.GenerateFilter(filters)
                Builders<'a>.Filter.Where(lambda)

        builder

    /// <summary>Build a sort filter for MongoDB driver</summary>
    /// <param name="sort">String with filters</param>
    /// <returns>A sort definition</returns>
    static member BuildSortFilter(sort : string) =
        let filter =
            match String.IsNullOrEmpty(sort) with
            | true ->
                let field = FieldDefinition<'a>.op_Implicit("createdAt")
                Builders<'a>.Sort.Descending(field)
            | false ->
                let isAscending = sort.Contains('-')
                let property = if isAscending then sort.Split('-').Last() else sort

                if isAscending then Builders<'a>.Sort.Ascending(FieldDefinition<'a>.op_Implicit(property))
                else Builders<'a>.Sort.Descending(FieldDefinition<'a>.op_Implicit(property))

        filter

    /// <summary>Populate reference field</summary>
    /// <param name="collection">Collection ued to create the fluent interface</param>
    /// <param name="filter">Filter definition for match pipe</param>
    /// <param name="sortFilter">Filter definition for sort documents</param>
    /// <param name="relations">References to other collections</param>
    /// <param name="request">Request object model</param>
    /// <returns>Query to get documents and its references</returns>
    static member Populate
        (
            collection : IMongoCollection<'a>,
            filter : FilterDefinition<'a>,
            sortFilter: SortDefinition<BsonDocument>,
            relations : List<Relation>,
            request : Request
        ) =
        let entities = request.Entities.Split(',')
        let relation = relations.Find(fun r -> r.Entity = entities.First().ToLower())
        let localField = FieldDefinition<'a>.op_Implicit(relation.LocalKey)
        let foreignField = FieldDefinition<BsonDocument>.op_Implicit(relation.ForeignKey)

        let titleCaseAs = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(relation.Entity)
        let as' = FieldDefinition<BsonDocument>.op_Implicit(sprintf "%sEmbedded" titleCaseAs)

        let pipe = collection
                    .Aggregate()
                    .Match(filter)
                    .Lookup(relation.Entity, localField, foreignField, as')
                    .Skip(request.Page * request.PageSize)
                    .Limit(request.PageSize)
                    .Sort(sortFilter)

        let result =
            match relation.JustOne with
            | true ->
                pipe.Unwind(as').As<'a>()
            | false ->
                pipe.As<'a>()

        result

    /// <summary>Populate reference field</summary>
    /// <param name="collection">Collection ued to create the fluent interface</param>
    /// <param name="filter">Filter definition for match pipe</param>
    /// <param name="relations">References to other collections</param>
    /// <param name="request">Request object model</param>
    /// <returns>Query to get documents and its references</returns>
    static member PopulateSingular
        (
            collection : IMongoCollection<'a>,
            filter : FilterDefinition<'a>,
            relations : List<Relation>,
            request : Request
        )
        =
        let entities = request.Entities.Split(',')
        let relation = relations.Find(fun r -> r.Entity = entities.First().ToLower())
        let localField = FieldDefinition<'a>.op_Implicit(relation.LocalKey)
        let foreignField = FieldDefinition<BsonDocument>.op_Implicit(relation.ForeignKey)

        let titleCaseAs = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(relation.Entity)
        let as' = FieldDefinition<BsonDocument>.op_Implicit(sprintf "%sEmbedded" titleCaseAs)

        let pipe = collection
                    .Aggregate()
                    .Match(filter)
                    .Lookup(relation.Entity, localField, foreignField, as')

        let result =
            match relation.JustOne with
            | true ->
                pipe.Unwind(as').As<'a>()
            | false ->
                pipe.As<'a>()

        result.FirstOrDefaultAsync()


    /// <summary>Generate the filter using lambda expression</summary>
    /// <param name="keys">The query string sended in the http request</param>
    /// <returns>Lambda expression builded</returns>
    static member GenerateFilter(keys : string) =
        let keyValues = keys.Split(',')
        let operators = keyValues.Select(Func<string, TypeOperator>(fun key -> key.ClassifyOperation()))
        let parameterExpression = Expression.Parameter(typeof<'a>, "entity")
        let lambda = MongoDBDefinitions<'a>.BuildExpression(parameterExpression, operators)

        lambda

    /// <summary>Builds the Lambda Expression</summary>
    /// <param name="expression">Lambda Expression</param>
    /// <param name="operators">Array of Type Operator objects</param>
    /// <returns></returns>    
    static member BuildExpression(expression : ParameterExpression, operators : IEnumerable<TypeOperator>) =
        let operations = new Dictionary<int, Expression>()
        let mutable counter = 1

        for i in operators do
            let constant = Expression.Constant(i.Value)
            let property = Expression.Property(expression, i.Key)
            operations.Add(counter, MongoDBDefinitions<'a>.GenerateTypeExpression(property, constant, i))
            counter <- counter + 1

        let lambda = Expression.Lambda<Func<'a, bool>>(operations.First().Value, expression)
        lambda

    /// <summary>Constructs a Lambda Expression</summary>
    /// <param name="property">Property of model to compare</param>
    /// <param name="constant">The value used to compare</param>
    /// <param name="item">Type Operator object</param>
    /// <returns>Lambda Expression</returns>
    static member GenerateTypeExpression(property : Expression, constant : Expression, item : TypeOperator) =
        let expression =
            match item.Operation with
            | Constants.Same ->
                Expression.Equal(property, constant)
            | Constants.NotSame ->
                Expression.NotEqual(property, constant)
            | Constants.Greather ->
                Expression.GreaterThan(property, constant)
            | Constants.GreaterThan ->
                Expression.GreaterThanOrEqual(property, constant)
            | Constants.Lower ->
                Expression.LessThan(property, constant)
            | _ ->
                Expression.LessThanOrEqual(property, constant)

        expression
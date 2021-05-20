namespace TodoApi.Repositories

open MongoDB.Bson
open TodoApi.Models

type TodoRepository private () =

    member val _todoRepository : IRepository<Todo> = null with get, set

    new (todoRepository : IRepository<Todo>) as this = TodoRepository() then
        this._todoRepository <- todoRepository

    interface ITodoRepository with

        /// <summary>Get list of todos</summary>
        /// <param name="request">Request object model</param>
        /// <returns>List of all todos</returns>
        member this.GetAllAsync(request : Request) = this._todoRepository.GetAllAsync(request)(null)

        /// <summary>Get a todo by specific ID</summary>
        /// <param name="id">Todo ID</param>
        /// <returns>Specific Todo</returns>
        member this.GetByIdAsync(id: string) =
            this._todoRepository.GetByIdAsync(fun entity -> entity.Id = BsonObjectId(new ObjectId(id)))

        /// <summary>Returns the number of documents in todos collection</summary>
        /// <param name="request">Request object model</param>
        /// <returns>Number of documents</returns>
        member this.CountAsync(request : Request) = this._todoRepository.CountAsync(request)
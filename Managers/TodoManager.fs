namespace TodoApi.Managers

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.AspNetCore.JsonPatch
open TodoApi.Models

type TodoManager private () =

    member val _todoManager : IManager<Todo> = null with get, set

    new (todoManager : IManager<Todo>) as this = TodoManager() then
        this._todoManager <- todoManager

    interface ITodoManager with

        /// <summary>Creates a new todo document</summary>
        /// <param name="todo">Todo object class</param>
        /// <returns>Todo</returns>
        member this.CreateAsync(todo: Todo) = this._todoManager.CreateAsync todo

        /// <summary>Deletes a document</summary>
        /// <param name="id">Document ID</param>
        /// <returns>Removal result</returns>
        member this.DeleteByIdAsync(id: string) =
            this._todoManager.DeleteByIdAsync(fun entity -> entity.Id = BsonObjectId(new ObjectId(id)))

        /// <summary>Updates a todo</summary>
        /// <param name="id">Todo ID</param>
        /// <param name="newTodo">New todo values</param>
        /// <param name="currentTodo">Current todo values</param>
        /// <returns>Todo</returns>
        member this.UpdateByIdAsync(id: string)(newTodo: Todo)(currentTodo: JsonPatchDocument<Todo>) =
            currentTodo.ApplyTo(newTodo)
            let filter = Builders<Todo>.Filter.Eq((fun entity -> entity.Id), BsonObjectId(new ObjectId(id)))
            let options = new ReplaceOptions(IsUpsert = true)
            this._todoManager.UpdateOneAsync(filter)(newTodo)(options)

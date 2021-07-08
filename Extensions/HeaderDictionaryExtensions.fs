namespace TodoApi.Extensions

open System.Linq
open Microsoft.AspNetCore.Http

module HeaderDictionaryExtensions =

    type IHeaderDictionary with

        /// <summary>Extracts the token from authorization header</summary>
        /// <returns>Json Web Token</returns>
        member this.ExtractJsonWebToken() =
            let authorization = this.["Authorization"].ToString()
            let token = authorization.Split(" ").LastOrDefault()

            token
namespace TodoApi.Extensions

open System

module ArrayManipulation =

    type Array with
        /// <summary>Check if array is empty</summary>
        /// <returns>If array is empty return true otherwise false</returns>
        member this.IsEmpty() =
            let result = if this.Length = 0 then true else false
            result
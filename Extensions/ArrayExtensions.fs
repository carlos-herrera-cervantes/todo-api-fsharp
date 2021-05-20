namespace TodoApi.Extensions

open System

module ArrayManipulation =

    type Array with
        /// <summary>Check if array is empty</summary>
        /// <returns>If array is empty return true otherwise false</returns>
        member this.IsEmpty() = this.Length = 0
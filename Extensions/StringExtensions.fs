namespace TodoApi.Extensions

open System
open System.Linq
open System.Text.RegularExpressions
open TodoApi.Constants
open TodoApi.Models

module StringExtensions =

    type String with

        /// <summary>Classifies a comparison operator in a Type Operator object</summary>
        /// <returns>Type Operator object</returns>
        member this.ClassifyOperation() =
            let pattern =
                if Regex.IsMatch(this, Constants.NotSameRegex) then
                    { 
                        Key = Regex.Split(this, Constants.NotSameRegex).First()
                        Operation = Constants.NotSame
                        Value = Regex.Split(this, Constants.NotSameRegex).Last()
                    }
                else if Regex.IsMatch(this, Constants.GreatherThanRegex) then
                    {
                        Key = Regex.Split(this, Constants.GreatherThanRegex).First()
                        Operation = Constants.GreaterThan
                        Value = Regex.Split(this, Constants.GreatherThanRegex).Last()
                    }
                else if Regex.IsMatch(this, Constants.LowerThanRegex) then
                    {
                        Key = Regex.Split(this, Constants.LowerThanRegex).First()
                        Operation = Constants.LowerThan
                        Value = Regex.Split(this, Constants.LowerThanRegex).Last()
                    }
                else if Regex.IsMatch(this, Constants.SameRegex) then
                    {
                        Key = Regex.Split(this, Constants.SameRegex).First()
                        Operation = Constants.Same
                        Value = Regex.Split(this, Constants.SameRegex).Last()
                    }
                else if Regex.IsMatch(this, Constants.GreatherRegex) then
                    {
                        Key = Regex.Split(this, Constants.GreatherRegex).First()
                        Operation = Constants.Greather
                        Value = Regex.Split(this, Constants.GreatherRegex).Last()
                    }
                else
                    {
                        Key = Regex.Split(this, Constants.LowerRegex).First()
                        Operation = Constants.Lower
                        Value = Regex.Split(this, Constants.LowerRegex).Last()
                    }

            pattern
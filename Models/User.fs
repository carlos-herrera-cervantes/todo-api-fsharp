namespace TodoApi.Models

[<CLIMutable>]
type User = { Id : string; Email : string; mutable Name : string; }
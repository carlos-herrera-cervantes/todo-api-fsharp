namespace TodoApi.Managers

[<AllowNullLiteral>]
type IPasswordHasherManager =
    abstract member Hash : string -> string
    abstract member Compare : string -> string -> bool
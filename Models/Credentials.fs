namespace TodoApi.Models

open System.ComponentModel.DataAnnotations

type Credentials = {
    [<Required(ErrorMessage = "EmailRequired")>]
    [<DataType(DataType.EmailAddress, ErrorMessage = "EmailFormatInvalid")>]
    Email : string

    [<Required(ErrorMessage = "PasswordRequired")>]
    Password : string
}
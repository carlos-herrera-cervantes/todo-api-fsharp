namespace TodoApi.Models

type TypeOperator<'a> = {
    Key : string

    Operation : string

    Value : 'a
}
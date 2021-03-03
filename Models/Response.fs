namespace TodoApi.Models

type Response<'a> = { Status : bool; Data : 'a; }

type ResponseWithPaginator<'a> = { Status : bool; Data : 'a; Paginator : Paginator }

type ResponseError = { Status : bool; Message : string; Code : string; }
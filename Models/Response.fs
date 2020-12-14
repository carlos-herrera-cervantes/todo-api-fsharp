namespace TodoApi.Models

type Response<'a> = { Status : bool; Data : 'a; }

type ResponseError = { Status : bool; Message : string; Code : string; }
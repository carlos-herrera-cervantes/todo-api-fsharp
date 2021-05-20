namespace TodoApi.Models

[<AllowNullLiteral>]
type Paginator () =

    member val Page : int = 1 with get, set

    member val PageSize : int = 10 with get, set

    member val ReaminingDocuments : int = 0 with get, set

    member val TotalDocuments : int = 0 with get, set

module Paginator =

    /// <summary>Sets the paginator object using the query parameters</summary>
    /// <param name="request">Request object model</param>
    /// <param name="totalDocs">Total documents of collection</param>
    /// <returns>Paginator object model</returns>
    let setObjectPaginator (request : Request) (totalDocs : int) =
        let page = if request.Page <> 0 then request.Page else 1
        let take = page * request.PageSize
        let subtractDocuments = totalDocs - take
        let remainingDocuments = if subtractDocuments < 0 then 0 else subtractDocuments

        let paginator = Paginator(
                            Page = page,
                            PageSize = request.PageSize,
                            ReaminingDocuments = remainingDocuments,
                            TotalDocuments = totalDocs
                        )
        paginator
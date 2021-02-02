namespace TodoApi.Constants

module Constants =

    [<Literal>]
    let Same = "="

    [<Literal>]
    let NotSame = "!="

    [<Literal>]
    let Greather = ">"

    [<Literal>]
    let GreaterThan = ">="

    [<Literal>]
    let Lower = "<"

    [<Literal>]
    let LowerThan = "<="

    [<Literal>]
    let SameRegex = @"\W*=\W*"

    [<Literal>]
    let NotSameRegex = @"\W*!=\W*"

    [<Literal>]
    let GreatherRegex = @"\W*>\W*"

    [<Literal>]
    let GreatherThanRegex = @"\W*>=\W*"

    [<Literal>]
    let LowerRegex = @"\W*<\W*"

    [<Literal>]
    let LowerThanRegex = @"\W*<=\W*"
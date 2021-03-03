namespace TodoApi.Models

type Relation () =

    member val Entity : string = null with get, set
    member val LocalKey : string = null with get, set
    member val ForeignKey : string = null with get, set
    member val JustOne : bool = false with get, set
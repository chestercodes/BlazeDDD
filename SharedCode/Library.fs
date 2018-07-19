namespace SharedCode

module DDDEvents =
    let processThing (directory: string) (outputPath: string) =
        
        printfn "Hello %s" directory

    //[<CLIMutable>]
    type CustomerName(firstName, middleInitial, lastName) = 
        member this.FirstName = firstName
        member this.MiddleInitial = middleInitial
        member this.LastName = lastName 

    [<CLIMutable>]
    type CustomerName2 = { FirstName:string; MiddleInitial: string; LastName: string }
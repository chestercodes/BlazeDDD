open SharedCode

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    DDDEvents.processThing "blah1" "blah2"
    0 // return an integer exit code

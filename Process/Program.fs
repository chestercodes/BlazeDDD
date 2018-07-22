open System.IO
open SharedCode.DDDEvents
open System
open FSharp.Data

type DDDEventJson = JsonProvider<"../Template.json">

let throwIfdoesntExist dir =
    if not (Directory.Exists(dir)) then
        raise (Exception("directory doesnt exist" + dir))

let oToN (opt: DateTime option): Nullable<DateTime> =
    match opt with
    | Some x -> Nullable<DateTime>(x)
    | None -> Nullable()

let maptoWritable (dddEvent:DDDEventJson.Root): DDDEvent =
    { 
        Name = dddEvent.Name; 
        Type = dddEvent.Type; 
        Location = dddEvent.Location; 
        Date = dddEvent.Date; 
        SessionSubmissionOpens = oToN dddEvent.SessionSubmissionOpens 
        SessionSubmissionCloses = oToN dddEvent.SessionSubmissionCloses
        SessionVotingOpens = oToN dddEvent.SessionVotingOpens
        SessionVotingCloses = oToN dddEvent.SessionVotingCloses  
        AgendaAnnounced = oToN dddEvent.AgendaAnnounced
        RegistrationOpens = oToN dddEvent.RegistrationOpens
        Region = dddEvent.Region
    }

let jsonFilesInData directory =
    let dirPath = Path.GetFullPath(directory)
    throwIfdoesntExist dirPath
    printfn "data dir - %s" dirPath
    
    let jsonFiles = Directory.GetFiles(dirPath, "*.json")
    printfn "Found %i json files" jsonFiles.Length
    jsonFiles

let processData (directory: string) (outputPath: string) =
    let outputFilePath = Path.GetFullPath(outputPath)
    printfn "out file - %s" outputFilePath
    
    let dddEvents = jsonFilesInData directory
                    |> Array.toList
                    |> List.collect (fun x -> 
                        let jsonContent = File.ReadAllText(x)
                        DDDEventJson.Parse(jsonContent) |> Array.toList
                    )
                    |> List.map maptoWritable
                    |> List.sortByDescending (fun x -> x.Date)

    let regions = dddEvents |> List.map (fun x -> x.Region) |> List.distinct

    
    let eventsByType = dddEvents |> List.groupBy (fun x -> x.Type) 

    let ddd = { Events = dddEvents |> List.toArray ; Regions = regions |> List.toArray }
    let contents = Newtonsoft.Json.JsonConvert.SerializeObject(ddd)
    File.WriteAllText(outputFilePath, contents)
    let data = Newtonsoft.Json.JsonConvert.DeserializeObject<DDD>(contents)
    ()


[<EntryPoint>]
let main argv =
    printfn "Running data processing"
    processData "../data" "../Web/wwwroot/data/ddd.json"
    0 // return an integer exit code

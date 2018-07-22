namespace SharedCode
open System

module DDDEvents =
    [<CLIMutable>]
    type DDDEvent = {   Name: string; 
                        Type: string; 
                        Location: string; 
                        Date: DateTime; 
                        SessionSubmissionOpens: Nullable<DateTime>; 
                        SessionSubmissionCloses: Nullable<DateTime>
                        SessionVotingOpens: Nullable<DateTime>; 
                        SessionVotingCloses: Nullable<DateTime>;  
                        AgendaAnnounced: Nullable<DateTime>; 
                        RegistrationOpens: Nullable<DateTime>; 
                        Region: string } 
    
    [<CLIMutable>]
    type DDD = { Events: DDDEvent []; Regions: string [] }

    let formatDate (date: Nullable<DateTime>) =
        if date.HasValue then date.Value.ToShortDateString()
        else ""

    type SelectedRole = Speaking | Attending
    type SelectedRegion = string
    type Selected = { Role: SelectedRole; Region: SelectedRegion }
    let AllRegions = "allregions"

    let filterFromSelected (selected: Selected) (now: DateTime -> bool) tryDate =
        let cutoffDateFunc = match selected.Role with
                                | Speaking -> (fun (x: DDDEvent) -> 
                                        if x.SessionSubmissionCloses.HasValue then
                                            now x.SessionSubmissionCloses.Value
                                        elif tryDate then
                                            now x.Date
                                        else
                                            false
                                    )
                                | Attending -> (fun (x: DDDEvent) -> (now x.Date) )
        
        let regionFilterFunc = match selected.Region with
                                | y when y = AllRegions -> (fun _ -> true )
                                | x -> (fun (e: DDDEvent)  -> e.Region = x)
        
        let filter (e:DDDEvent) = cutoffDateFunc e && regionFilterFunc e
        filter

    let filterFutureEvents (selected: Selected) (now: DateTime) (ddd: DDD): DDDEvent[] =
        let filter = filterFromSelected selected (fun x -> now < x) false

        ddd.Events 
        |> Array.toList 
        |> List.filter filter 
        |> List.toArray
    
    let pastEvents (selected: Selected) (now: DateTime) (ddd: DDD): DDDEvent[] =
        let filter = filterFromSelected selected (fun x -> now > x) true

        ddd.Events 
        |> Array.toList 
        |> List.filter filter 
        |> List.toArray
    
    
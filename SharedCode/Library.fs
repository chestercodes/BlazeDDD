namespace SharedCode
open System

module DDDEvents =
    [<CLIMutable>]
    type DDDEvent = {   Name: string; 
                        Type: string; 
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

﻿namespace AvaloniaExample.ViewModels

open System
open ReactiveElmish
open ReactiveElmish.Avalonia
open Elmish
open DynamicData

module Counter = 
    type Model =  { Count: int; Actions: SourceList<Action> }
    and Action = { Description: string; Timestamp: DateTime }

    type Msg = 
        | Increment
        | Decrement
        | Reset

    let init() = 
        { 
            Count = 0
            Actions = SourceList.createFrom [ { Description = "Initialized Counter"; Timestamp = DateTime.Now } ]
        }

    let update (msg: Msg) (model: Model) = 
        match msg with
        | Increment ->
            { 
                Count = model.Count + 1 
                Actions = model.Actions |> SourceList.add { Description = "Incremented"; Timestamp = DateTime.Now }
            }
        | Decrement ->
            { 
                Count = model.Count - 1 
                Actions = model.Actions |> SourceList.add { Description = "Decremented"; Timestamp = DateTime.Now }
            }
        | Reset ->
            {
                Count = 0 
                Actions = model.Actions |> SourceList.removeAll |> SourceList.add { Description = "Reset"; Timestamp = DateTime.Now }
            }

open Counter
open System.Collections.Generic

type CounterViewModel() =
    inherit ReactiveElmishViewModel()

    let local = 
        Program.mkAvaloniaSimple init update
        |> Program.mkStore

    member this.Count = this.Bind(local, _.Count)
    member this.Actions = this.BindSourceList(local.Model.Actions)
    member this.Increment() = local.Dispatch Increment
    member this.Decrement() = local.Dispatch Decrement
    member this.Reset() = local.Dispatch Reset
    member this.IsResetEnabled = this.Bind(local, fun m -> m.Count <> 0)

    static member DesignVM = 
        new CounterViewModel()
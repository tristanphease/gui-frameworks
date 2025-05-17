module BoleroApp.Client.Util

/// Function that takes in an index and new value and returns the list with the new value at the index
let modifyIndex (index: int) (newValue: 'a) : (list<'a> -> list<'a>) =
    List.mapi (fun i x -> if i = index then newValue else x) 

/// Function that takes in an index and a list and will return a new list without the element at that index
let deleteIndex (index: int) : (list<'a> -> list<'a>) =
    List.mapi (fun i x -> if i = index then None else Some x)
        >> List.choose id

let isEqual (value: 'a) (option: 'a option): bool =
    match option with 
        | Some optionVal -> value = optionVal
        | None -> false
﻿let log x = printfn "%A" x
let reverse c = match c with | '[' -> ']' | '(' -> ')' | '{' -> '}' | '<' -> '>' | _ -> failwith "unknown reverse"
let point c = match c with | ')' -> 3 | ']' -> 57 | '}' -> 1197 | '>' -> 25137 | _ -> failwith "unknown point"
let isStarter c = ['['; '('; '{'; '<'] |> Seq.contains c

let findUnexpected xs =
    let rec loop stack xs =
        match xs with
        | [] -> None
        | x::rest ->
            if isStarter x then
                loop (reverse x::stack) rest
            else
                match stack with
                | [] -> Some x
                | s::sx -> if s = x then loop sx rest else Some x
    loop List.empty (xs |> Seq.toList)

let input = System.IO.File.ReadAllLines "data.txt"

input
|> Seq.map findUnexpected
|> Seq.choose id
|> Seq.map point
|> Seq.sum
|> log
﻿let path = "/Users/phj/Code/F-Sharp-Advent-of-Code-2021/AOC2022/10/input.txt"
let instructions = System.IO.File.ReadAllLines  path |> Array.toList

let logs a = printfn "%A" a; a

let cycles = 
    instructions |> List.map (fun instruction ->
        let s = instruction.Split " "
        match s[0] with
        | "noop" -> [0]
        | "addx" -> [0; s[1] |> int]
        | _ -> failwith "unexpected instruction"
    )
    |> List.collect id
    |> List.indexed

let mutable sum = 0
let mutable x = 1
for (index, value) in cycles do
    let cycle = index + 1
    if (cycle + 20) % 40 = 0 then
        printfn "%i: %i" cycle x
        sum <- sum + cycle * x
    x <- x + value

printfn "sum %i" sum
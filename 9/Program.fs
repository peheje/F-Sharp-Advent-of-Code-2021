﻿let logs x = printfn "%A" x
let replace a b s = (s:string).Replace((a:string), (b:string))
let parseRow row = row |> Seq.toArray |> Array.map (int << string)

let data =
    System.IO.File.ReadAllLines "sample.txt"
    |> Array.map parseRow

let rows = Array.length data
let columns = Array.length data[0]
let indexOk row col = row >= 0 && col >= 0 && row < rows && col < columns
let get row col = if indexOk row col then data[row][col] else 100
let surrounding row col =
    let left = get row (col-1)
    let right = get row (col+1)
    let below = get (row+1) col
    let above = get (row-1) col
    [left; right; below; above]

[for (ri, ci) in Seq.allPairs [0..rows] [0..columns] do
    let cur = get ri ci
    let neighbors = surrounding ri ci
    if neighbors |> Seq.forall (fun x -> x > cur) then cur + 1
]
|> Seq.sum |> logs
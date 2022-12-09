let path = "/Users/peterhelstrupjensen/repos/AOC2022_at_work/06/input"
let rows = System.IO.File.ReadAllLines path

let allUnique l =
    l |> Seq.distinct |> Seq.length = (l |> Seq.length)

let firstUnique size row =
    // Using Seq, should only evaluate the required amount of windows
    // https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/sequences#searching-and-finding-elements
    (row |> Seq.windowed size |> Seq.findIndex allUnique) + size

let firstUnique4 = firstUnique 4
let firstUnique14 = firstUnique 14

let part1 = rows |> Array.map firstUnique4 |> Array.head
let part2 = rows |> Array.map firstUnique14 |> Array.head

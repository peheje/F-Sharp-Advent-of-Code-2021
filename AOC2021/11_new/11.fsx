let lines = System.IO.File.ReadAllLines("sample.txt")

let octos =
    lines
    |> Array.map (fun row -> [|for x in row -> int (string x)|])
    |> Array.collect id

let size = lines |> Seq.length
let length = octos |> Array.length

let toIndex row col = row * size + col

let anyFlashing data =
    data |> Seq.exists(fun x -> x > 9)

let surroundingIndices row col =
    let neighbors = [|[-1; -1]; [-1; 0]; [-1; 1]; [0; -1]; [0; 1]; [1; -1]; [1; 0]; [1; 1]|]
    [|for neighbor in neighbors do
        let r = neighbor[0] + row
        let c = neighbor[1] + col
        toIndex r c
    |] |> Array.filter (fun index -> index >= 0 && index < length)

let mutable flashes = 0

for _ in 1..99 do
    for i in 0..length-1 do
        octos[i] <- octos[i] + 1

    while anyFlashing octos do
        for i in 0..size-1 do
            for j in 0..size-1 do
                let index = toIndex i j
                if octos[index] > 9 then
                    flashes <- flashes + 1
                    octos[index] <- -1
                    for ni in surroundingIndices i j do
                        if octos[ni] <> -1 then
                            octos[ni] <- octos[ni] + 1

    for i in 0..length-1 do
        if octos[i] = -1 then
            octos[i] <- 0

printfn "%i" flashes
﻿let square x = x * x
let booth (xs: float array) =
    // f(1.0, 3.0) = 0
    let t1 = square (xs[0] + 2.0*xs[1] - 7.0)
    let t2 = square (2.0*xs[0] + xs[1] - 5.0)
    t1 + t2

let f1 xs = xs |> Array.sumBy (fun x -> x * x)
let rand () = System.Random.Shared.NextDouble()
let randRange min max = rand () * (max - min) + min

let randomElement xs =
    Array.get xs (System.Random.Shared.Next(xs |> Array.length))

let sw = System.Diagnostics.Stopwatch.StartNew()

let print = 1000
let optimizer = f1
let generations = 20000
let argsize = 100
let popsize = 400
let min = -10.0
let max = 10.0
let clamp x = System.Math.Clamp(x, min, max)
let mutateRange () = randRange 0.2 0.95
let crossoverRange () = randRange 0.1 1.0
let createAgent () = Array.init argsize (fun _ -> randRange min max)

let mutable pop =
    Array.init popsize (fun _ ->
        let agent = createAgent ()
        let score = optimizer agent
        (agent, score))

for g in 0 .. generations - 1 do
    let crossoverRisk = crossoverRange ()
    let crossover () = rand () < crossoverRisk
    let mutate = mutateRange ()

    pop <-
        pop
        |> Array.Parallel.map (fun (xt, xScore) ->
            let x0 = pop |> randomElement |> fst
            let x1 = pop |> randomElement |> fst
            let x2 = pop |> randomElement |> fst

            let trial =
                Array.init argsize (fun j -> 
                       if crossover () then
                           (x0[j] + (x1[j] - x2[j]) * mutate) |> clamp
                       else
                           xt[j]
                )

            let trialScore = optimizer trial

            if trialScore < xScore then
                (trial, trialScore)
            else
                (xt, xScore))

    if g % print = 0 then
        let scores = pop |> Array.map (fun (_, score) -> score)
        printfn "generation %i" g
        printfn "generation mean %f" (scores |> Array.average)
        printfn "generation minimum %f" (scores |> Array.min)
    
let best =
        pop
        |> Array.minBy (fun (_, score) -> score)
        |> fst
        
printfn "generation best %A" best
printfn "Execution time was %i ms" sw.ElapsedMilliseconds
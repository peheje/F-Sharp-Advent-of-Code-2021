﻿open System
open System.Diagnostics
open System.Collections.Concurrent
open HackerNewsAsync.Model
open HackerNewsAsync
open System.Threading
open System.Threading.Channels

let sw = Stopwatch.StartNew()

module IdChannel =
    let private channel = Channel.CreateBounded<int>(1)

    // In future might use this: https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1021-value-task-interop.md
    let receive () =
        channel.Reader.ReadAsync().AsTask()
        |> Async.AwaitTask

    let send id =
        channel.Writer.WriteAsync(id).AsTask()
        |> Async.AwaitTask

    let close () = channel.Writer.Complete()

let producer =
    async {
        let! ids = HnClient.getTopStoriesIds 1000

        for id in ids do
            do! IdChannel.send (id)

        IdChannel.close ()
    }

let stories = ConcurrentDictionary<int, Story>()

let consumer =
    async {
        let threadId = Thread.CurrentThread.ManagedThreadId

        try
            while true do
                let! storyId = IdChannel.receive ()
                let! story = storyId |> HnClient.getStory
                stories.TryAdd(story.id, story) |> Debug.Assert
                printfn "Thread %i Received %s" threadId story.title
        with
        | :? AggregateException as ax ->
            for inner in ax.InnerExceptions do
                if not (inner :? ChannelClosedException) then
                    raise inner

            printfn "Thread %i done" threadId
    }

async {
    let! p = Async.StartChild producer

    let! consumers =
        List.init 8 (fun _ -> Async.StartChild consumer)
        |> Async.Parallel

    do! p

    for c in consumers do
        do! c
}
|> Async.RunSynchronously

stories.Values
|> Seq.filter (fun s -> s.typ = "story")
|> Seq.sortBy (fun s -> s.id)
|> Seq.length
|> printfn "%A"
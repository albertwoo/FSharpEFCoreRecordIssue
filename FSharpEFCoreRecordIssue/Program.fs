open System.Linq
open Microsoft.EntityFrameworkCore
open FSharpEFCoreRecordIssue.Db


let db = new BloggingContext()
db.Database.Migrate()


if db.Blogs.Count() = 0 then
    let blogs = 
        [|
            for i in 1..3 do
                Blog
                    (Url = $"url {i}"
                    ,Rating = System.Random().Next(1, 10)
                    ,Posts =
                        [|
                            for j in 1..2 do
                                Post
                                    (Title = $"Title {j}"
                                    ,Content = $"Content {j}")
                        |])
        |]

    db.Blogs.AddRange blogs |> ignore
    db.SaveChanges() |> ignore


let tryRun fn =
    try 
        printfn "========================================================"
        fn() |> Seq.iter (printfn "%A")
        printfn "========================================================"
    with ex ->
        printfn $"Error: {ex.Message}"


tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> {| Rating = x.Rating; Url = x.Url |}))
tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> {| Url = x.Url; Rating = x.Rating |}))
tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> {| Url = x.Url; PostCount = x.Posts.Count |}))
tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> {| PostCount = x.Posts.Count; Url = x.Url |}))

open System.Linq
open Microsoft.EntityFrameworkCore
open LinqToDB.EntityFrameworkCore
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

let mutable count = 0

let tryRun fn =
    try 
        count <- count + 1
        printfn $"=======================[{count}]==============================="
        fn() |> Seq.iter (printfn "%A")
    with ex ->
        printfn $"Error: {ex.Message}"
    printfn $"=======================[{count}]==============================="
        

tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> {| Rating = x.Rating; Url = x.Url |}))
tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> {| Url = x.Url; Rating = x.Rating |}))
tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> {| Url = x.Url; PostCount = x.Posts.Count |}))
tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> {| PostCount = x.Posts.Count; Url = x.Url |}))


// The issue not just hanppen to annonymous record, it also hanpens to normal record
type BlogBrif = { Rating: int; Url: string }
type BlogBrifWithCount = { PostCount: int; Url: string }

tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> { Rating = x.Rating; Url = x.Url }))
tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> { Url = x.Url; Rating = x.Rating }))
tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> { Url = x.Url; PostCount = x.Posts.Count }))
tryRun (fun () -> db.Blogs.AsNoTracking().Select(fun x -> { PostCount = x.Posts.Count; Url = x.Url }))



// With  linq2db
tryRun (fun () -> db.Blogs.ToLinqToDB().Select(fun x -> {| Rating = x.Rating; Url = x.Url |}))
tryRun (fun () -> db.Blogs.ToLinqToDB().Select(fun x -> {| Url = x.Url; Rating = x.Rating |}))
tryRun (fun () -> db.Blogs.ToLinqToDB().Select(fun x -> {| Url = x.Url; PostCount = x.Posts.Count |}))
tryRun (fun () -> db.Blogs.ToLinqToDB().Select(fun x -> {| PostCount = x.Posts.Count; Url = x.Url |}))

tryRun (fun () -> db.Blogs.ToLinqToDB().Select(fun x -> { Rating = x.Rating; Url = x.Url }))
tryRun (fun () -> db.Blogs.ToLinqToDB().Select(fun x -> { Url = x.Url; Rating = x.Rating }))
tryRun (fun () -> db.Blogs.ToLinqToDB().Select(fun x -> { Url = x.Url; PostCount = x.Posts.Count }))
tryRun (fun () -> db.Blogs.ToLinqToDB().Select(fun x -> { PostCount = x.Posts.Count; Url = x.Url }))

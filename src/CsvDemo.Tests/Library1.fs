module CsvReaderTests

open global.Xunit
open FsCheck.Xunit
open Swensen.Unquote 
open System
open ReportGenerator


[<Fact>]
let ``CrimeWithinTimeSpan Returns All Crimes Between Start and End Hours``() =
    let rows = 
        ReadSacrementoCrimeRows () 
        |> Seq.map CsvCrimeToDataRecord

    let result = CrimesWithinTimeSpan 5 7 rows

    test <@ (result |> Seq.map (fun x -> x.Time.Hour)) |> Seq.distinct |> Seq.toList = [5;6;7;] @>

[<Fact>]
let ``FilterOutAddressNumber removes leading numbers from Crime's address``() =
    let input = MakeCrimeData DateTime.Now "123 Cool Street" "Too cool for school"
    let expected = {input with Address = "Cool Street"}

    let actual = Seq.head (RemoveAddressNumber [input])

    test <@ actual = expected @>


[<Property>]
let ``Reverse of reverse of a list is the original list ``(xs:list<int>) =
    List.rev(List.rev xs) = xs
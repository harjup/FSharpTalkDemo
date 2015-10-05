namespace CsvDemo

open ReportGenerator;

// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

module entryPoint =

    [<EntryPoint>]
    let main argv = 
        printfn "%A" argv

        GenerateCrimeReport () 
        |> WriteAllText @"result.html"

        0 // return an integer exit code

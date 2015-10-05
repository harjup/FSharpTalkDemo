module ReportGenerator

open System
open System.IO
open FSharp.Data
open System.Text.RegularExpressions
type Crimes = CsvProvider<"CrimeSample.csv">

type CrimeData = 
    {
        Time:DateTime; 
        Address:string; 
        Description:string
    }

let MakeCrimeData time address description = {Time=time; Address=address; Description=description}

let ReadCrimeData location = 
    Crimes.Load "SacramentocrimeJanuary2006.csv"

let ReadSacrementoCrimeRows () = (ReadCrimeData "SacramentocrimeJanuary2006.csv").Rows

let CsvCrimeToDataRecord (crime: Crimes.Row) =
    MakeCrimeData crime.Cdatetime crime.Address crime.Crimedescr


let CrimesWithinTimeSpan startHour endHour data = 
    let crimeIsBetweenHours startHour endHour crime = 
        crime.Time.Hour >= startHour && crime.Time.Hour <= endHour

    data
    |> Seq.filter (crimeIsBetweenHours startHour endHour) 


let RemoveAddressNumber data = 
    let doesNotMatchPattern regex address = not (Regex.IsMatch(address, regex))
    let join (delimiter: string) (str: string[]) = String.Join(delimiter, str)
    let withoutAddressNumber crime =
        let address = 
            crime.Address.Split [|' '|]
            |> Array.toList 
            |> List.filter (doesNotMatchPattern @"\d{1,4}$")
            |> List.toArray
            |> join " "

        {crime with Address = address}

    Seq.map withoutAddressNumber data

let OrderByStreetName data = Seq.sortBy (fun x -> x.Address) data

let PrintTableRows crimes = 
    let PrintAsTableRow c =
        sprintf 
            "<tr><td>%s<td><td>%s</td><td>%s</td></tr>" 
            c.Address 
            (c.Time.ToShortTimeString ()) 
            c.Description

    crimes |> Seq.map PrintAsTableRow

let AssembleCrimeReportPage body =
    let PageHeader = "<html><h1>Crime locations in Sacremnto between 5 and 9 AM</h1><table>"
    let TableHead = "<thead><tr><th>Address</th><th>Time</th><th>Description</th></tr></thead>"
    let PageFooter = "</table></html>"

    PageHeader + TableHead + body + PageFooter

let GenerateCrimeReport () =
    ReadSacrementoCrimeRows () 
        |> Seq.map CsvCrimeToDataRecord
        |> CrimesWithinTimeSpan 5 8
        |> RemoveAddressNumber
        |> OrderByStreetName
        |> PrintTableRows
        |> String.Concat
        |> AssembleCrimeReportPage


let WriteAllText path content =
    File.WriteAllText(path, content)
module Program

open Falco
open Falco.Routing
open Microsoft.AspNetCore.Builder


let wapp = WebApplication.Create()

let endpoints = 
    [ get "/" Main.mainPage ]

wapp.UseRouting()
    .UseFalco(endpoints)
    .Run()

module Program

open Falco
open Falco.Routing
open Microsoft.AspNetCore.Builder

module Route = 
    let index = "/"
    let createQuiz = "/createquiz"

let wapp = WebApplication.Create()

let endpoints = 
    [ 
        get Route.index Main.mainPage;
        get Route.createQuiz CreateQuiz.createQuizPage 
    ]


wapp.UseRouting()
    .Use(StaticFileExtensions.UseStaticFiles)
    .UseFalco(endpoints)
    .Run()

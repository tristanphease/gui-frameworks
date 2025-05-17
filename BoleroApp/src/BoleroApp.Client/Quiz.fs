module BoleroApp.Client.Quiz

open System.Text.Json
open System.Text.Json.Serialization


type QuizQuestion =
    {
        question: string;
        options: list<string>;
        correctAnswer: int;
    }


type Quiz = list<QuizQuestion>

let quizToJson (quiz: list<QuizQuestion>) : string =
    let options = JsonSerializerOptions()
    options.Converters.Add(JsonFSharpConverter())
    JsonSerializer.Serialize(quiz, options)

let quizFromJson (quizJson: string) : list<QuizQuestion> =
    let options = JsonSerializerOptions()
    options.Converters.Add(JsonFSharpConverter())
    JsonSerializer.Deserialize(quizJson, options)
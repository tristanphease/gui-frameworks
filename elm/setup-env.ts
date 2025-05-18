
const envName = Deno.env.get("BASE_PATH") ?? "";

const environmentTemplate = `
module Environment exposing (..)

basePath : String
basePath = 
    "${envName}"
`;

await Deno.writeTextFile("src/Environment.elm", environmentTemplate, { encoding: "utf8" });



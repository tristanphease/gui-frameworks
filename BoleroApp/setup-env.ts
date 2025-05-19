
let envName = Deno.env.get("BASE_PATH") ?? "";

if (envName.length > 0 && !envName.startsWith("/")) {
    envName = "/" + envName;
}

const indexFilePath = "src/BoleroApp.Client/wwwroot/index.html";

const indexFile = await Deno.readTextFile(indexFilePath);

const newIndexFile = indexFile.replace(`<base href="/">`, `<base href="${envName}">`);

await Deno.writeTextFile(indexFilePath, newIndexFile, { encoding: "utf8" });



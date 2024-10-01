open System.Net.Http
open System.Text
open System.Text.Json

type Message =
    {
        content: string
        username: string
        avatar_url: string
        tts: bool
        embeds: MessageEmbed []
    }

and MessageEmbed =
    {
        color: System.Nullable<int>
        title: string
        url: string
        description: string
    }

let hookurl = System.Environment.GetEnvironmentVariable "DISCORD_PACKAGE_FEED"
let thread = System.Environment.GetEnvironmentVariable "DISCORD_THREAD"
let url = sprintf "%s?thread_id=%s" hookurl thread

let client = new HttpClient()


let message: Message =
    {
        content = "## New Capacitor test APK :"
        username = "IntelliFactory CI"
        avatar_url = "https://raw.githubusercontent.com/dotnet-websharper/core/refs/heads/master/tools/WebSharper.png"
        tts = false
        embeds = [||]
    }

async {
    let files = System.IO.Directory.EnumerateFiles("artifacts")
    let files =
        files
        |> Seq.map (fun file ->
            System.IO.Path.GetFileName(file), System.IO.File.ReadAllBytes(file)
        )
    let serializedMessage = JsonSerializer.Serialize message
    printfn "%s" serializedMessage
    use multiFormData = new MultipartFormDataContent()
    use content = new StringContent(serializedMessage, Encoding.UTF8, "application/json")
    multiFormData.Add(content, "payload_json")
    files
    |> Seq.iteri (fun i (fname, data) ->
        multiFormData.Add(new ByteArrayContent(data), sprintf "file%d" (i+1), sprintf "%s.apk" fname)
    )
    let! response = client.PostAsync(url, multiFormData) |> Async.AwaitTask
    if response.IsSuccessStatusCode then
        ()
    else
        let! res = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        printfn "%A" res
    return ()
} |> Async.RunSynchronously

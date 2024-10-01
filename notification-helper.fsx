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
    let! file = System.IO.File.ReadAllBytesAsync("app-debug.apk") |> Async.AwaitTask
    let serializedMessage = JsonSerializer.Serialize message
    printfn "%s" serializedMessage
    use multiFormData = new MultipartFormDataContent()
    use content = new StringContent(serializedMessage, Encoding.UTF8, "application/json")
    multiFormData.Add(content, "payload_json")
    multiFormData.Add(new ByteArrayContent(file), "file1", "app-debug.apk")
    let! response = client.PostAsync(hookurl, multiFormData) |> Async.AwaitTask
    if response.IsSuccessStatusCode then
        ()
    else
        let! res = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        printfn "%A" res
    return ()
} |> Async.RunSynchronously

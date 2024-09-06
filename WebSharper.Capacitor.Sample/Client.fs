namespace Capacitor.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Capacitor

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let People =
        ListModel.FromSeq [
            "John"
            "Paul"
        ]

    let takePicture () = promise {
        let options = Camera.ImageOptions(Camera.CameraResultType.Base64)
        let! image = Capacitor.Camera.GetPhoto(options)
        return image
    }

    let writeToClipboard () = promise {
        let! clipboard = Capacitor.Clipboard.Write(Clipboard.WriteOptions(String = "Hello World!"))
        return clipboard
    }

    let setCookies () = promise {
        let! setCookies = Capacitor.Cookies.SetCookie(Cookies.SetCookieOptions(
            Url = "http://example.com'",
            Key = "language",
            Value = "en"
        ))
        return setCookies
    }

    let showAlert() = promise {
        let! alert = Capacitor.Dialog.Alert(Dialog.AlertOptions(
            Title = "Stop",
            Message = "this is an error"
        ))
        return alert
    }

    let showConfirm () = promise {
        let! value = Capacitor.Dialog.Confirm(Dialog.ConfirmOptions(
            Title = "Confirm",
            Message = "Are you sure you'd like to press the red button?"
        ))

        return value
    }

    [<SPAEntryPoint>]
    let Main () =
        let newName = Var.Create ""

        IndexTemplate.Main()
            .ListContainer(
                People.View.DocSeqCached(fun (name: string) ->
                    IndexTemplate.ListItem().Name(name).Doc()
                )
            )
            .Name(newName)
            .Add(fun _ ->
                People.Add(newName.Value)
                newName.Value <- ""
            )
            .TakePhoto(fun _ ->
                async {
                    return! takePicture().Then(fun image -> Var.Set newName <| image.Base64String.Substring(0, 20)).AsAsync()
                }
                |> Async.Start
            )
            .WriteClipboard(fun _ ->
                async {
                    printfn "Successfully write 'Hello World!' to clipboard"
                    return! writeToClipboard().Then(fun _ -> Var.Set newName <| "Successfully write 'Hello World!' to clipboard").AsAsync()
                }
                |> Async.Start
            )
            .SetCookies(fun _ ->
                async {
                    printfn "Successfully Set Cookies"
                    return! writeToClipboard().Then(fun _ -> Var.Set newName <| "Successfully Set Cookies").AsAsync()
                }
                |> Async.Start
            )
            .ShowAlert(fun _ ->
                async {
                    printfn "Successfully Show Alert"
                    return! showAlert().Then(fun alert -> Var.Set newName <| "Successfully Show Alert").AsAsync()
                }
                |> Async.Start
            )
            .ShowConfirm(fun _ ->
                async {
                    printfn "Successfully Show Confirm"
                    return! showConfirm().Then(fun confirm -> Var.Set newName <| "Successfully Show Confirm").AsAsync()
                }
                |> Async.Start
            )
            .Doc()
        |> Doc.RunById "main"

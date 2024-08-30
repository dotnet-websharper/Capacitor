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
            .Doc()
        |> Doc.RunById "main"

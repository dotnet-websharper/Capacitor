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

    let writeToClipboard value = promise {
        let! clipboard = Capacitor.Clipboard.Write(Clipboard.WriteOptions(String = $"{value}"))
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

    let save value = promise {
        let! token = Capacitor.Preferences.Set(Preferences.SetOptions(
            key = "auth_token",
            value = value
        ))
        return token
    }

    let get() = promise {
        let! token = Capacitor.Preferences.Get(Preferences.GetOptions(
            key = "auth_token"
        ))
        return token
    }

    [<AutoOpen>]
    module AuthTest = 
        type User (name: string, password: string) = 
            member x.Name = name
            member x.Password = password

        let saveToken token = promise {
            Capacitor.Preferences.Set(Preferences.SetOptions(
                key = "auth_token",
                value = token
            )) |> ignore
        }

        let getToken() = promise {
            let! value = Capacitor.Preferences.Get(Preferences.GetOptions(
                key = "auth_token"
            ))
            return value
        }

        let login() = promise {
            let data = User(name = "Got", password = "password")
            let! response = Capacitor.Http.Post(Http.HttpOptions(
                url = "http://localhost:5173/",
                Data = data
            ))

            if (response.Status = 200) then
                let token = response?data?token
                Capacitor.Dialog.Alert(Dialog.AlertOptions(Message = $"Logged in, token: {token}")) |> ignore
                do! saveToken token  
            else 
                Capacitor.Dialog.Alert(Dialog.AlertOptions(Message = $"Login failed: {response}")) |> ignore
        }

    let scheduleNotification() = promise {
        let notificationOptions = LocalNotifications.ScheduleOptions(
            Notifications = [|LocalNotifications.LocalNotificationSchema(
            Id = 1,
            Title = "Reminder Notification",
            Body = "Explore new variety and offers",
            LargeBody = "Get 30% discounts on new products",
            SummaryText = "Exciting offers"
        )|]
        )

        let! notification = Capacitor.Capacitor.LocalNotifications.Schedule(notificationOptions) 
        return notification
    }

    let getDeliveredNotification() = promise {
        let! deliver = Capacitor.LocalNotifications.GetDeliveredNotifications() 
        return deliver
    }
    
    [<SPAEntryPoint>]
    let Main () =
        let preferencesValue = Var.Create ""
        let clipboardValue = Var.Create ""

        IndexTemplate.Main()
            .TakePhoto(fun _ ->
                async {
                    return! takePicture().Then(fun image -> printfn $"Photo image: {image.Base64String.Substring(0, 20)}" ).AsAsync()
                }
                |> Async.Start
            )
            .ClipboardValue(clipboardValue)
            .WriteClipboard(fun _ ->
                async {
                    return! writeToClipboard(clipboardValue.Value).Then(fun _ -> printfn $"Successfully write '{clipboardValue.Value}' to clipboard").AsAsync()
                }
                |> Async.Start
            )
            .SetCookies(fun _ ->
                async {
                    return! setCookies().Then(fun _ -> printfn "Successfully Set Cookies").AsAsync()
                }
                |> Async.Start
            )
            .ShowAlert(fun _ ->
                async {
                    return! showAlert().Then(fun alert -> printfn "Successfully Show Alert").AsAsync()
                }
                |> Async.Start
            )
            .ShowConfirm(fun _ ->
                async {
                    return! showConfirm().Then(fun confirm -> printfn "Successfully Show Confirm").AsAsync()
                }
                |> Async.Start
            )
            .PreferencesValue(preferencesValue)
            .Save(fun _ -> 
                async {
                    return! save(preferencesValue.Value).Then(fun _ -> printfn "Save value Successfully").AsAsync()
                }
                |> Async.Start
            )
            .Get(fun _ -> 
                async {
                    return! get().Then(fun token -> printfn $"Value: {token.Value}").AsAsync()
                }
                |> Async.Start
            )
            .LogIn(fun _ -> 
                async {
                    return! login().Then(fun _ -> printfn "").AsAsync()
                }
                |> Async.Start
            )
            .Notification(fun _ -> 
                async {
                    return! scheduleNotification().Then(fun _ -> printfn $"Scheduled notification complete").AsAsync()
                }
                |> Async.Start
            )
            .DeliveredNotification(fun _ -> 
                async {
                    return! getDeliveredNotification().Then(fun notification -> JS.Alert(JSON.Stringify(notification))).AsAsync()
                }
                |> Async.Start
            )
            .Doc()
        |> Doc.RunById "main"

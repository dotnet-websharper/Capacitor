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
    [<AutoOpen>]
    module CameraTest =
        let takePicture () = promise {
            let options = Camera.ImageOptions(Camera.CameraResultType.Base64)
            let! image = Capacitor.Camera.GetPhoto(options)
            return image
        }

    [<AutoOpen>]
    module ClipboardTest =
        let writeToClipboard value = promise {
            let! clipboard = Capacitor.Clipboard.Write(Clipboard.WriteOptions(String = $"{value}"))
            return clipboard
        }

    [<AutoOpen>]
    module CookiesTest = 
        let setCookies () = promise {
            let! setCookies = Capacitor.Cookies.SetCookie(Cookies.SetCookieOptions(
                Url = "http://example.com'",
                Key = "language",
                Value = "en"
            ))
            return setCookies
        }

    [<AutoOpen>]
    module DialogTest = 
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

    [<AutoOpen>]
    module PreferencesTest =
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
        let login() = promise {
            Capacitor.BiometricAuth.Authenticate(BiometricAuth.AuthenticateOptions(
                Reason = "Please authenticate",
                CancelTitle = "Cancel",
                AllowDeviceCredential = true,
                IosFallbackTitle = "Use passcode",
                AndroidTitle = "Biometric login",
                AndroidSubtitle = "Log in using biometric authentication",
                AndroidConfirmationRequired = false,
                AndroidBiometryStrength = BiometricAuth.AndroidBiometryStrength.Weak
            )) |> ignore
        } 

    [<AutoOpen>]
    module LocalNotificationTest =
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
        let cameraOutput = Var.Create ""

        IndexTemplate.Main()
            .Name(cameraOutput)
            .TakePhoto(fun _ ->
                async {
                    
                    return! takePicture().Then(fun image -> Var.Set cameraOutput <| $"{image.Base64String.Substring(0, 20)}" ).AsAsync()
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
                    return! login().Then(fun _ -> printfn "Finger print log in").AsAsync()
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

namespace Capacitor.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Capacitor

[<JavaScript>]
module Client =

    // The template is the whole document, loaded from the DOM, so editing
    // index.html and refreshing the browser is enough.
    type IndexTemplate = Template<"index.html", ClientLoad.FromDocument>

    // ------------------------------------------------------------------
    // State bound into the template
    // ------------------------------------------------------------------

    let log = Var.Create ""
    let platform = Var.Create "web"
    let notice = Var.Create "Detecting the Capacitor platform…"
    let cameraSrc = Var.Create ""
    let preferenceValue = Var.Create "web sharper"
    let clipboardValue = Var.Create "Hello from WebSharper.Capacitor!"

    // Theme: light is the default, persisted in localStorage. The key is shared
    // with the bootstrap script in index.html.
    let themeStorageKey = "websharper-capacitor-theme"
    let theme = Var.Create "light"
    let themeLabel = Var.Create "🌙 Dark mode"

    let mutable lastVideoUri : string = ""

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    let append (line: string) =
        log.Value <- (if log.Value = "" then line else log.Value + "\n" + line)

    let showJson (value: obj) =
        if isNull value || value ===. JS.Undefined then
            "(none)"
        else
            let s = JSON.Stringify value
            if isNull s then "(none)" else s

    let errorText (err: obj) =
        if isNull err then
            "not available on this platform"
        else
            let message = err?message
            if isNull (box message) then
                "rejected: " + showJson err
            else
                As<string> message

    /// Report the outcome of a promise in the activity log.
    let run (label: string) (p: Promise<'T>) (show: 'T -> string) =
        p.Then(
            (fun result -> append (label + " ✓ " + show result)),
            (fun (err: obj) -> append (label + " ✗ " + errorText err))
        ) |> ignore

    /// Report an object result as JSON.
    let runJson (label: string) (p: Promise<'T>) =
        run label p (fun result -> showJson (box result))

    /// Report a unit result.
    let runUnit (label: string) (p: Promise<unit>) =
        p.Then(
            (fun () -> append (label + " ✓")),
            (fun (err: obj) -> append (label + " ✗ " + errorText err))
        ) |> ignore

    let applyTheme () =
        JS.Document.DocumentElement.SetAttribute("data-theme", theme.Value)
        themeLabel.Value <-
            if theme.Value = "dark" then "☀️ Light mode" else "🌙 Dark mode"

    let toggleTheme () =
        theme.Value <- (if theme.Value = "dark" then "light" else "dark")
        try
            JS.Window.LocalStorage.SetItem(themeStorageKey, theme.Value)
        with _ -> ()
        applyTheme ()

    let loadTheme () =
        try
            let stored = JS.Window.LocalStorage.GetItem themeStorageKey
            if stored = "dark" || stored = "light" then
                theme.Value <- stored
        with _ -> ()
        applyTheme ()

    let initPlatform () =
        try
            let p = CapacitorCore.GetPlatform()
            platform.Value <- p
            if CapacitorCore.IsNativePlatform() then
                notice.Value <- "Running inside a native Capacitor shell — every bound plugin is available."
            else
                notice.Value <- "Running in a browser (Capacitor web). Web implementations work; native-only plugins report a graceful error in the log."
        with _ ->
            platform.Value <- "unknown"
            notice.Value <- "The Capacitor runtime could not be detected on this page."

    // ------------------------------------------------------------------
    // Platform, App & Device
    // ------------------------------------------------------------------

    let coreInfo () =
        try
            append (
                sprintf
                    "core: platform=%s native=%b Camera=%b Filesystem=%b GoogleMaps=%b"
                    (CapacitorCore.GetPlatform())
                    (CapacitorCore.IsNativePlatform())
                    (CapacitorCore.IsPluginAvailable "Camera")
                    (CapacitorCore.IsPluginAvailable "Filesystem")
                    (CapacitorCore.IsPluginAvailable "GoogleMaps")
            )
            append ("core: convertFileSrc → " + CapacitorCore.ConvertFileSrc("file:///data/photo.jpg"))
        with ex ->
            append ("core: ✗ " + ex.Message)

    let deviceInfo () =
        runJson "Device.GetInfo" (Capacitor.Device.GetInfo())
        runJson "Device.GetBatteryInfo" (Capacitor.Device.GetBatteryInfo())
        runJson "Device.GetLanguageCode" (Capacitor.Device.GetLanguageCode())
        runJson "Device.GetLanguageTag" (Capacitor.Device.GetLanguageTag())
        runJson "Device.GetId" (Capacitor.Device.GetId())

    let appInfo () =
        runJson "App.GetInfo" (Capacitor.App.GetInfo())
        runJson "App.GetState" (Capacitor.App.GetState())
        runJson "App.GetLaunchUrl" (Capacitor.App.GetLaunchUrl())
        runJson "App.GetAppLanguage" (Capacitor.App.GetAppLanguage())
        runUnit "App.ToggleBackButtonHandler" (
            Capacitor.App.ToggleBackButtonHandler(App.ToggleBackButtonHandlerOptions(enabled = true))
        )

    let networkInfo () =
        runJson "Network.GetStatus" (Capacitor.Network.GetStatus())

    let listenApp () =
        runJson "App.AddListener(appStateChange)" (
            Capacitor.App.AddListener(
                "appStateChange",
                fun (state: App.AppState) -> append ("appStateChange → " + showJson (box state))
            )
        )

    let listenNetwork () =
        runJson "Network.AddListener(networkStatusChange)" (
            Capacitor.Network.AddListener(
                "networkStatusChange",
                fun (status: Network.ConnectionStatus) -> append ("networkStatusChange → " + showJson (box status))
            )
        )

    let listenMotion () =
        runJson "Motion.AddListener(accel)" (
            Capacitor.Motion.AddListener(
                "accel",
                fun (event: Motion.AccelListenerEvent) ->
                    append (sprintf "accel → x=%f y=%f z=%f" event.Acceleration.X event.Acceleration.Y event.Acceleration.Z)
            )
        )

    // ------------------------------------------------------------------
    // Preferences, Cookies & Clipboard
    // ------------------------------------------------------------------

    let preferencesSet () =
        runUnit "Preferences.Set" (
            Capacitor.Preferences.Set(
                Preferences.SetOptions(key = "websharper-capacitor", value = preferenceValue.Value)
            )
        )

    let preferencesGet () =
        runJson "Preferences.Get" (
            Capacitor.Preferences.Get(Preferences.GetOptions(key = "websharper-capacitor"))
        )

    let preferencesKeys () =
        runJson "Preferences.Keys" (Capacitor.Preferences.Keys())

    let preferencesRemove () =
        runUnit "Preferences.Remove" (
            Capacitor.Preferences.Remove(Preferences.RemoveOptions(key = "websharper-capacitor"))
        )

    let preferencesClear () =
        runUnit "Preferences.Clear" (Capacitor.Preferences.Clear())

    let cookiesSet () =
        runUnit "Cookies.SetCookie" (
            Capacitor.Cookies.SetCookie(
                Cookies.SetCookieOptions(
                    key = "websharper-capacitor",
                    value = "enabled",
                    Url = "https://httpbin.org/"
                )
            )
        )

    let cookiesGet () =
        runJson "Cookies.GetCookies" (
            Capacitor.Cookies.GetCookies(Cookies.GetCookieOptions(Url = "https://httpbin.org/"))
        )

    let cookiesDelete () =
        runUnit "Cookies.DeleteCookie" (
            Capacitor.Cookies.DeleteCookie(
                Cookies.DeleteCookieOptions(key = "websharper-capacitor", Url = "https://httpbin.org/")
            )
        )

    let cookiesClear () =
        runUnit "Cookies.ClearCookies" (
            Capacitor.Cookies.ClearCookies(Cookies.ClearCookieOptions(Url = "https://httpbin.org/"))
        )

    let clipboardWrite () =
        runUnit "Clipboard.Write" (
            Capacitor.Clipboard.Write(Clipboard.WriteOptions(String = clipboardValue.Value))
        )

    let clipboardRead () =
        runJson "Clipboard.Read" (Capacitor.Clipboard.Read())

    // ------------------------------------------------------------------
    // Filesystem
    // ------------------------------------------------------------------

    let fileName = "websharper-capacitor-demo.txt"

    let fileWrite () =
        runJson "Filesystem.WriteFile" (
            Capacitor.Filesystem.WriteFile(
                Filesystem.WriteFileOptions(
                    path = fileName,
                    data = "V2ViU2hhcnBlci5DYXBhY2l0b3I=", // base64 on the web implementation
                    Directory = Filesystem.Directory.DATA,
                    Recursive = true
                )
            )
        )

    let fileRead () =
        runJson "Filesystem.ReadFile" (
            Capacitor.Filesystem.ReadFile(
                Filesystem.ReadFileOptions(path = fileName, Directory = Filesystem.Directory.DATA)
            )
        )

    let fileReadChunks () =
        runJson "Filesystem.ReadFileInChunks" (
            Capacitor.Filesystem.ReadFileInChunks(
                Filesystem.ReadFileInChunksOptions(path = fileName, chunkSize = 16, Directory = Filesystem.Directory.DATA),
                fun (chunk: Filesystem.ReadFileResult) err ->
                    if isNull (box err) then append ("chunk → " + showJson (box chunk))
            )
        )

    let fileAppend () =
        runUnit "Filesystem.AppendFile" (
            Capacitor.Filesystem.AppendFile(
                Filesystem.AppendFileOptions(
                    path = fileName,
                    data = "U2Vjb25kIGxpbmUu", // base64 on the web implementation
                    Directory = Filesystem.Directory.DATA
                )
            )
        )

    let fileStat () =
        runJson "Filesystem.Stat" (
            Capacitor.Filesystem.Stat(
                Filesystem.StatOptions(path = fileName, Directory = Filesystem.Directory.DATA)
            )
        )

    let fileCopy () =
        runJson "Filesystem.Copy" (
            Capacitor.Filesystem.Copy(
                Filesystem.CopyOptions(
                    from = fileName,
                    ``to`` = "websharper-capacitor-demo-copy.txt",
                    Directory = Filesystem.Directory.DATA,
                    ToDirectory = Filesystem.Directory.DATA
                )
            )
        )

    let fileRename () =
        runUnit "Filesystem.Rename" (
            Capacitor.Filesystem.Rename(
                Filesystem.CopyOptions(
                    from = "websharper-capacitor-demo-copy.txt",
                    ``to`` = "websharper-capacitor-demo-renamed.txt",
                    Directory = Filesystem.Directory.DATA,
                    ToDirectory = Filesystem.Directory.DATA
                )
            )
        )

    let fileUri () =
        runJson "Filesystem.GetUri" (
            Capacitor.Filesystem.GetUri(
                Filesystem.GetUriOptions(path = fileName, directory = Filesystem.Directory.DATA)
            )
        )

    let fileMkdir () =
        runUnit "Filesystem.Mkdir" (
            Capacitor.Filesystem.Mkdir(
                Filesystem.MkdirOptions(path = "demo", Directory = Filesystem.Directory.DATA, Recursive = true)
            )
        )

    let fileReaddir () =
        runJson "Filesystem.Readdir" (
            Capacitor.Filesystem.Readdir(
                Filesystem.ReaddirOptions(path = "demo", Directory = Filesystem.Directory.DATA)
            )
        )

    let fileDownload () =
        runJson "Filesystem.DownloadFile" (
            Capacitor.Filesystem.DownloadFile(
                Filesystem.DownloadFileOptions(
                    url = "https://httpbin.org/image/png",
                    path = "downloaded.png",
                    Directory = Filesystem.Directory.CACHE,
                    Progress = true,
                    Recursive = true
                )
            )
        )

    let filePermissions () =
        runJson "Filesystem.CheckPermissions" (Capacitor.Filesystem.CheckPermissions())
        runJson "Filesystem.RequestPermissions" (Capacitor.Filesystem.RequestPermissions())

    let fileDelete () =
        runUnit "Filesystem.DeleteFile" (
            Capacitor.Filesystem.DeleteFile(
                Filesystem.DeleteFileOptions(path = fileName, Directory = Filesystem.Directory.DATA)
            )
        )

    let fileRmdir () =
        runUnit "Filesystem.Rmdir" (
            Capacitor.Filesystem.Rmdir(
                Filesystem.RmdirOptions(path = "demo", Directory = Filesystem.Directory.DATA, Recursive = true)
            )
        )

    // ------------------------------------------------------------------
    // Camera & barcode scanner
    // ------------------------------------------------------------------

    let showMedia (media: Camera.MediaResult) =
        let webPath = media?webPath
        let uri = media?uri
        let src =
            if not (isNull webPath) then As<string> webPath
            elif not (isNull uri) then As<string> uri
            else ""
        if src <> "" then cameraSrc.Value <- src
        let mediaType = media?``type``
        if not (isNull mediaType) && As<int> mediaType = 1 then
            lastVideoUri <- src

    let takePhoto () =
        run "Camera.TakePhoto" (
            Capacitor.Camera.TakePhoto(
                Camera.TakePhotoOptions(
                    Quality = 80,
                    TargetWidth = 800,
                    TargetHeight = 600,
                    CorrectOrientation = true,
                    SaveToGallery = false,
                    CameraDirection = Camera.CameraDirection.REAR,
                    Editable = Camera.Editable.No,
                    IncludeMetadata = true
                )
            )
        ) (fun media ->
            showMedia media
            showJson (box media)
        )

    let chooseFromGallery () =
        run "Camera.ChooseFromGallery" (
            Capacitor.Camera.ChooseFromGallery(
                Camera.ChooseFromGalleryOptions(
                    MediaType = Camera.MediaTypeSelection.All,
                    AllowMultipleSelection = false,
                    Limit = 1,
                    IncludeMetadata = true,
                    Quality = 80
                )
            )
        ) (fun results -> showJson (box results))

    let recordVideo () =
        run "Camera.RecordVideo" (
            Capacitor.Camera.RecordVideo(
                Camera.RecordVideoOptions(SaveToGallery = false, IncludeMetadata = true)
            )
        ) (fun media ->
            showMedia media
            showJson (box media)
        )

    let playVideo () =
        if lastVideoUri = "" then
            append "Camera.PlayVideo: record a video first."
        else
            runUnit "Camera.PlayVideo" (
                Capacitor.Camera.PlayVideo(Camera.PlayVideoOptions(uri = lastVideoUri))
            )

    let cameraPermissions () =
        runJson "Camera.CheckPermissions" (Capacitor.Camera.CheckPermissions())
        runJson "Camera.RequestPermissions" (
            Capacitor.Camera.RequestPermissions(
                Camera.CameraPluginPermissions(permissions = [| Camera.CameraPermissionType.Camera |])
            )
        )

    let scanBarcode () =
        runJson "BarcodeScanner.ScanBarcode" (
            Capacitor.BarcodeScanner.ScanBarcode(
                BarcodeScanner.CapacitorBarcodeScannerOptions(
                    hint = BarcodeScanner.CapacitorBarcodeScannerTypeHint.QR_CODE,
                    ScanInstructions = "Point the camera at a barcode",
                    ScanButton = true,
                    CameraDirection = BarcodeScanner.CapacitorBarcodeScannerCameraDirection.BACK
                )
            )
        )

    // ------------------------------------------------------------------
    // Geolocation
    // ------------------------------------------------------------------

    let mutable watchId : string = ""

    let geoCurrent () =
        runJson "Geolocation.GetCurrentPosition" (
            Capacitor.Geolocation.GetCurrentPosition(
                Geolocation.PositionOptions(
                    EnableHighAccuracy = true,
                    Timeout = 10000,
                    MaximumAge = 0,
                    MinimumUpdateInterval = 2000,
                    Interval = 3000,
                    EnableLocationFallback = true
                )
            )
        )

    let geoWatch () =
        run "Geolocation.WatchPosition" (
            Capacitor.Geolocation.WatchPosition(
                Geolocation.PositionOptions(EnableHighAccuracy = true, Timeout = 10000),
                fun (position: Geolocation.Position) err ->
                    if isNull (box err) then append ("position → " + showJson (box position))
            )
        ) (fun id ->
            watchId <- id
            "watch id " + id
        )

    let geoClearWatch () =
        if watchId = "" then
            append "Geolocation.ClearWatch: no active watch."
        else
            runUnit "Geolocation.ClearWatch" (
                Capacitor.Geolocation.ClearWatch(Geolocation.ClearWatchOptions(id = watchId))
            )

    let geoPermissions () =
        runJson "Geolocation.CheckPermissions" (Capacitor.Geolocation.CheckPermissions())
        runJson "Geolocation.RequestPermissions" (
            Capacitor.Geolocation.RequestPermissions(
                Geolocation.GeolocationPluginPermissions(permissions = [| Geolocation.GeolocationPermissionType.Location |])
            )
        )

    // ------------------------------------------------------------------
    // Dialogs, toast & haptics
    // ------------------------------------------------------------------

    let showAlert () =
        runUnit "Dialog.Alert" (
            Capacitor.Dialog.Alert(Dialog.AlertOptions(message = "This is a Capacitor alert.", Title = "WebSharper.Capacitor", ButtonTitle = "OK"))
        )

    let showConfirm () =
        run "Dialog.Confirm" (
            Capacitor.Dialog.Confirm(
                Dialog.ConfirmOptions(
                    message = "Are you sure you would like to press the red button?",
                    Title = "Confirm",
                    OkButtonTitle = "Yes",
                    CancelButtonTitle = "No"
                )
            )
        ) (fun result -> showJson (box result))

    let showPrompt () =
        run "Dialog.Prompt" (
            Capacitor.Dialog.Prompt(
                Dialog.PromptOptions(
                    message = "Type something",
                    Title = "Prompt",
                    OkButtonTitle = "OK",
                    CancelButtonTitle = "Cancel",
                    InputPlaceholder = "Your answer",
                    InputText = "WebSharper"
                )
            )
        ) (fun result -> showJson (box result))

    let showToast () =
        runUnit "Toast.Show" (
            Capacitor.Toast.Show(
                Toast.ShowOptions(text = "Hello from WebSharper.Capacitor", Duration = Toast.ToastDuration.Long, Position = Toast.ToastPosition.Bottom)
            )
        )

    let showActionSheet () =
        runJson "ActionSheet.ShowActions" (
            Capacitor.ActionSheet.ShowActions(
                ActionSheet.ShowActionsOptions(
                    options = [|
                        ActionSheet.ActionSheetButton(title = "First", Style = ActionSheet.ActionSheetButtonStyle.DEFAULT)
                        ActionSheet.ActionSheetButton(title = "Delete", Style = ActionSheet.ActionSheetButtonStyle.DESTRUCTIVE)
                        ActionSheet.ActionSheetButton(title = "Cancel", Style = ActionSheet.ActionSheetButtonStyle.CANCEL)
                    |],
                    Title = "Choose an option",
                    Message = "Action sheet demo",
                    Cancelable = true
                )
            )
        )

    let hapticImpact () =
        runUnit "Haptics.Impact" (
            Capacitor.Haptics.Impact(Haptics.ImpactOptions(style = Haptics.ImpactStyle.HEAVY))
        )

    let hapticNotification () =
        runUnit "Haptics.Notification" (
            Capacitor.Haptics.Notification(Haptics.NotificationOptions(``type`` = Haptics.NotificationType.SUCCESS))
        )

    let hapticVibrate () =
        runUnit "Haptics.Vibrate" (Capacitor.Haptics.Vibrate(Haptics.VibrateOptions(duration = 300)))

    let hapticSelection () =
        runUnit "Haptics.SelectionStart" (Capacitor.Haptics.SelectionStart())
        runUnit "Haptics.SelectionChanged" (Capacitor.Haptics.SelectionChanged())
        runUnit "Haptics.SelectionEnd" (Capacitor.Haptics.SelectionEnd())

    // ------------------------------------------------------------------
    // Notifications
    // ------------------------------------------------------------------

    let localPermissions () =
        runJson "LocalNotifications.CheckPermissions" (Capacitor.LocalNotifications.CheckPermissions())
        runJson "LocalNotifications.RequestPermissions" (Capacitor.LocalNotifications.RequestPermissions())
        runJson "LocalNotifications.CheckExactNotificationSetting" (Capacitor.LocalNotifications.CheckExactNotificationSetting())

    let localSchedule () =
        let scheduleValue = LocalNotifications.Schedule()
        scheduleValue.At <- Date(Date.Now())
        scheduleValue.Repeats <- false
        scheduleValue.AllowWhileIdle <- true
        let notification =
            LocalNotifications.LocalNotificationSchema(
                title = "WebSharper.Capacitor",
                body = "Scheduled from the binding sample.",
                id = 1,
                LargeBody = "Every LocalNotifications API is exercised here.",
                SummaryText = "Capacitor demo",
                Badge = 1,
                InterruptionLevel = LocalNotifications.InterruptionLevel.Active,
                IsExactNotification = false
            )
        notification.Schedule <- scheduleValue
        let localNotifications = Capacitor.LocalNotifications
        runJson "LocalNotifications.Schedule" (
            localNotifications.Schedule(
                LocalNotifications.ScheduleOptions(notifications = [| notification |])
            )
        )
        runJson "LocalNotifications.Update" (
            localNotifications.Update(
                LocalNotifications.ScheduleOptions(notifications = [| notification |])
            )
        )

    let localPending () =
        runJson "LocalNotifications.GetPending" (Capacitor.LocalNotifications.GetPending())

    let localDelivered () =
        runJson "LocalNotifications.GetDeliveredNotifications" (Capacitor.LocalNotifications.GetDeliveredNotifications())

    let localAll () =
        runJson "LocalNotifications.GetAll" (
            Capacitor.LocalNotifications.GetAll(
                LocalNotifications.GetAllOptions(State = LocalNotifications.NotificationState.SCHEDULED)
            )
        )
        runJson "LocalNotifications.GetByIds" (
            Capacitor.LocalNotifications.GetByIds(LocalNotifications.GetByIdsOptions(ids = [| 1 |]))
        )

    let localCancelAll () =
        runUnit "LocalNotifications.CancelAll" (Capacitor.LocalNotifications.CancelAll())

    let localChannel () =
        runUnit "LocalNotifications.CreateChannel" (
            Capacitor.LocalNotifications.CreateChannel(
                LocalNotifications.Channel(id = "websharper", name = "WebSharper", Description = "Binding sample channel")
            )
        )
        runUnit "LocalNotifications.DeleteChannel" (
            Capacitor.LocalNotifications.DeleteChannel(LocalNotifications.DeleteChannelOptions(id = "websharper"))
        )

    let localListChannels () =
        runJson "LocalNotifications.ListChannels" (Capacitor.LocalNotifications.ListChannels())

    let pushRegister () =
        runJson "PushNotifications.CheckPermissions" (Capacitor.PushNotifications.CheckPermissions())
        runJson "PushNotifications.Register" (Capacitor.PushNotifications.Register())
        runJson "PushNotifications.AddListener(registration)" (
            Capacitor.PushNotifications.AddListener(
                "registration",
                fun (token: PushNotifications.Token) -> append ("push token → " + token.Value)
            )
        )

    // ------------------------------------------------------------------
    // Share & browsers
    // ------------------------------------------------------------------

    let shareCanShare () =
        runJson "Share.CanShare" (Capacitor.Share.CanShare())

    let shareIt () =
        runJson "Share.Share" (
            Capacitor.Share.Share(
                Share.ShareOptions(
                    Title = "WebSharper.Capacitor",
                    Text = "Capacitor plugin bindings for F#",
                    Url = "https://websharper.com"
                )
            )
        )

    let openBrowser () =
        runJson "Browser.AddListener(browserFinished)" (
            Capacitor.Browser.AddListener("browserFinished", fun () -> append "browserFinished event")
        )
        runUnit "Browser.Open" (
            Capacitor.Browser.Open(Browser.OpenOptions(url = "https://websharper.com", PresentationStyle = PresentationStyle.Fullscreen))
        )

    let closeBrowser () =
        runUnit "Browser.Close" (Capacitor.Browser.Close())

    let launchCanOpen () =
        runJson "AppLauncher.CanOpenUrl" (
            Capacitor.AppLauncher.CanOpenUrl(AppLauncher.CanOpenURLOptions(url = "https://websharper.com"))
        )

    let launchOpen () =
        runJson "AppLauncher.OpenUrl" (
            Capacitor.AppLauncher.OpenUrl(AppLauncher.OpenURLOptions(url = "https://websharper.com"))
        )

    let inAppExternal () =
        runUnit "InAppBrowser.OpenInExternalBrowser" (
            Capacitor.InAppBrowser.OpenInExternalBrowser(
                InAppBrowser.OpenInDefaultParameterModel(url = "https://websharper.com")
            )
        )

    let inAppWebView () =
        let android =
            InAppBrowser.AndroidWebViewOptions(allowZoom = true, hardwareBack = true, pauseMedia = false)
        let ios =
            InAppBrowser.IOSWebViewOptions(
                allowOverScroll = true,
                enableViewportScale = true,
                allowInLineMediaPlayback = true,
                surpressIncrementalRendering = false,
                viewStyle = InAppBrowser.IOSViewStyle.FULL_SCREEN,
                animationEffect = InAppBrowser.IOSAnimation.COVER_VERTICAL,
                allowsBackForwardNavigationGestures = true
            )
        let options =
            InAppBrowser.WebViewOptions(
                showURL = true,
                showToolbar = true,
                clearCache = false,
                clearSessionCache = false,
                mediaPlaybackRequiresUserAction = false,
                closeButtonText = "Close",
                toolbarPosition = InAppBrowser.ToolbarPosition.TOP,
                showNavigationButtons = true,
                leftToRight = true,
                android = android,
                iOS = ios
            )
        runJson "InAppBrowser.AddListener(browserPageNavigationCompleted)" (
            Capacitor.InAppBrowser.AddListener(
                "browserPageNavigationCompleted",
                fun (event: InAppBrowser.BrowserPageNavigationCompletedEventData) ->
                    append ("in-app navigation → " + showJson (box event))
            )
        )
        runUnit "InAppBrowser.OpenInWebView" (
            Capacitor.InAppBrowser.OpenInWebView(
                InAppBrowser.OpenInWebViewParameterModel(url = "https://websharper.com", options = options)
            )
        )

    // ------------------------------------------------------------------
    // System UI & platform plugins
    // ------------------------------------------------------------------

    let statusBarInfo () =
        runJson "StatusBar.GetInfo" (Capacitor.StatusBar.GetInfo())
        runUnit "StatusBar.SetStyle" (
            Capacitor.StatusBar.SetStyle(StatusBar.StyleOptions(style = StatusBar.Style.DARK))
        )
        runUnit "StatusBar.SetBackgroundColor" (
            Capacitor.StatusBar.SetBackgroundColor(StatusBar.BackgroundColorOptions(color = "#1e64ff"))
        )
        runUnit "StatusBar.SetOverlaysWebView" (
            Capacitor.StatusBar.SetOverlaysWebView(StatusBar.SetOverlaysWebViewOptions(overlay = true))
        )
        runUnit "StatusBar.Show" (Capacitor.StatusBar.Show(StatusBar.AnimationOptions(Animation = StatusBar.Animation.FADE)))
        runJson "StatusBar.AddListener(statusBarVisibilityChanged)" (
            Capacitor.StatusBar.AddListener(
                "statusBarVisibilityChanged",
                fun (info: StatusBar.StatusBarInfo) -> append ("status bar → " + showJson (box info))
            )
        )

    let keyboardOps () =
        runUnit "Keyboard.Show" (Capacitor.Keyboard.Show())
        runUnit "Keyboard.SetStyle" (
            Capacitor.Keyboard.SetStyle(Keyboard.KeyboardStyleOptions(style = Keyboard.KeyboardStyle.LIGHT))
        )
        runUnit "Keyboard.SetResizeMode" (
            Capacitor.Keyboard.SetResizeMode(Keyboard.KeyboardResizeOptions(mode = Keyboard.KeyboardResize.Body))
        )
        runJson "Keyboard.GetResizeMode" (Capacitor.Keyboard.GetResizeMode())
        runJson "Keyboard.AddListener(keyboardDidShow)" (
            Capacitor.Keyboard.AddListener(
                "keyboardDidShow",
                fun (info: Keyboard.KeyboardInfo) -> append ("keyboard → height " + string info.KeyboardHeight)
            )
        )
        runUnit "Keyboard.Hide" (Capacitor.Keyboard.Hide())

    let orientationInfo () =
        runJson "ScreenOrientation.Orientation" (Capacitor.ScreenOrientation.Orientation())
        runUnit "ScreenOrientation.Lock" (
            Capacitor.ScreenOrientation.Lock(
                ScreenOrientation.OrientationLockOptions(orientation = ScreenOrientation.OrientationLockType.Portrait)
            )
        )
        runJson "ScreenOrientation.AddListener(screenOrientationChange)" (
            Capacitor.ScreenOrientation.AddListener(
                "screenOrientationChange",
                fun (result: ScreenOrientation.ScreenOrientationResult) ->
                    append ("orientation → " + showJson (box result))
            )
        )
        runUnit "ScreenOrientation.Unlock" (Capacitor.ScreenOrientation.Unlock())

    let screenReaderOps () =
        runJson "ScreenReader.IsEnabled" (Capacitor.ScreenReader.IsEnabled())
        runUnit "ScreenReader.Speak" (
            Capacitor.ScreenReader.Speak(ScreenReader.SpeakOptions(value = "Hello from WebSharper.Capacitor", Language = "en"))
        )
        runJson "ScreenReader.AddListener(stateChange)" (
            Capacitor.ScreenReader.AddListener(
                "stateChange",
                fun (state: ScreenReader.ScreenReaderState) -> append ("screen reader → " + showJson (box state))
            )
        )

    let textZoomOps () =
        runJson "TextZoom.Get" (Capacitor.TextZoom.Get())
        runJson "TextZoom.GetPreferred" (Capacitor.TextZoom.GetPreferred())
        runUnit "TextZoom.Set" (Capacitor.TextZoom.Set(TextZoom.SetOptions(value = 1.2)))

    let splashOps () =
        runUnit "SplashScreen.Show" (
            Capacitor.SplashScreen.Show(
                SplashScreen.ShowOptions(AutoHide = true, FadeInDuration = 200, FadeOutDuration = 200, ShowDuration = 1000)
            )
        )
        runUnit "SplashScreen.Hide" (
            Capacitor.SplashScreen.Hide(SplashScreen.HideOptions(FadeOutDuration = 200))
        )

    let biometricCheck () =
        runJson "BiometricAuth.CheckBiometry" (Capacitor.BiometricAuth.CheckBiometry())
        runUnit "BiometricAuth.Authenticate" (
            Capacitor.BiometricAuth.Authenticate(
                BiometricAuth.AuthenticateOptions(
                    Reason = "Please authenticate",
                    CancelTitle = "Cancel",
                    AllowDeviceCredential = true,
                    IosFallbackTitle = "Use passcode",
                    AndroidTitle = "Biometric login",
                    AndroidSubtitle = "Log in using biometric authentication",
                    AndroidConfirmationRequired = false,
                    AndroidBiometryStrength = BiometricAuth.AndroidBiometryStrength.Weak
                )
            )
        )

    let backgroundRunnerOps () =
        runJson "BackgroundRunner.CheckPermissions" (Capacitor.BackgroundRunner.CheckPermissions())
        runJson "BackgroundRunner.RequestPermissions" (
            Capacitor.BackgroundRunner.RequestPermissions(
                BackgroundRunner.RequestPermissionOptions(apis = [| BackgroundRunner.API.Notifications |])
            )
        )
        runJson "BackgroundRunner.DispatchEvent" (
            Capacitor.BackgroundRunner.DispatchEvent(
                BackgroundRunner.DispatchEventOptions(label = "com.websharper.capacitor", event = "demo", details = box {| value = 42 |})
            )
        )
        runUnit "BackgroundRunner.RemoveNotificationListeners" (
            Capacitor.BackgroundRunner.RemoveNotificationListeners()
        )

    let googleMapsCheck () =
        append (
            "GoogleMaps available: " + string (CapacitorCore.IsPluginAvailable "GoogleMaps")
            + " — the Google Maps plugin needs a native platform and an API key, so the sample only probes availability."
        )

    // ------------------------------------------------------------------
    // HTTP
    // ------------------------------------------------------------------

    let httpGet () =
        runJson "Http.Get" (
            Capacitor.Http.Get(
                Http.HttpOptions(
                    url = "https://api.github.com/zen",
                    ResponseType = Http.HttpResponseType.Text,
                    ReadTimeout = 10000,
                    ConnectTimeout = 10000
                )
            )
        )

    let httpPost () =
        runJson "Http.Post" (
            Capacitor.Http.Post(
                Http.HttpOptions(
                    url = "https://httpbin.org/post",
                    Data = box {| message = "Hello from WebSharper.Capacitor" |},
                    Headers = As<Http.HttpHeaders> (box {| ``Content-Type`` = "application/json" |})
                )
            )
        )

    // ------------------------------------------------------------------
    // Entry point
    // ------------------------------------------------------------------

    [<SPAEntryPoint>]
    let Main () =
        loadTheme ()
        initPlatform ()

        IndexTemplate()
            .ToggleTheme(fun _ -> toggleTheme ())
            .platform(platform.V)
            .themeLabel(themeLabel.V)
            .notice(notice.V)
            .log(log.V)
            .CameraSrc(Attr.Dynamic "src" cameraSrc.View)
            .PreferenceValue(preferenceValue)
            .ClipboardValue(clipboardValue)
            .CoreInfo(fun _ -> coreInfo ())
            .DeviceInfo(fun _ -> deviceInfo ())
            .AppInfo(fun _ -> appInfo ())
            .NetworkInfo(fun _ -> networkInfo ())
            .ListenApp(fun _ -> listenApp ())
            .ListenNetwork(fun _ -> listenNetwork ())
            .ListenMotion(fun _ -> listenMotion ())
            .PreferencesSet(fun _ -> preferencesSet ())
            .PreferencesGet(fun _ -> preferencesGet ())
            .PreferencesKeys(fun _ -> preferencesKeys ())
            .PreferencesRemove(fun _ -> preferencesRemove ())
            .PreferencesClear(fun _ -> preferencesClear ())
            .CookiesSet(fun _ -> cookiesSet ())
            .CookiesGet(fun _ -> cookiesGet ())
            .CookiesDelete(fun _ -> cookiesDelete ())
            .CookiesClear(fun _ -> cookiesClear ())
            .ClipboardWrite(fun _ -> clipboardWrite ())
            .ClipboardRead(fun _ -> clipboardRead ())
            .FileWrite(fun _ -> fileWrite ())
            .FileRead(fun _ -> fileRead ())
            .FileReadChunks(fun _ -> fileReadChunks ())
            .FileAppend(fun _ -> fileAppend ())
            .FileStat(fun _ -> fileStat ())
            .FileCopy(fun _ -> fileCopy ())
            .FileRename(fun _ -> fileRename ())
            .FileUri(fun _ -> fileUri ())
            .FileMkdir(fun _ -> fileMkdir ())
            .FileReaddir(fun _ -> fileReaddir ())
            .FileDownload(fun _ -> fileDownload ())
            .FilePermissions(fun _ -> filePermissions ())
            .FileDelete(fun _ -> fileDelete ())
            .FileRmdir(fun _ -> fileRmdir ())
            .TakePhoto(fun _ -> takePhoto ())
            .ChooseFromGallery(fun _ -> chooseFromGallery ())
            .RecordVideo(fun _ -> recordVideo ())
            .PlayVideo(fun _ -> playVideo ())
            .CameraPermissions(fun _ -> cameraPermissions ())
            .ScanBarcode(fun _ -> scanBarcode ())
            .GeoCurrent(fun _ -> geoCurrent ())
            .GeoWatch(fun _ -> geoWatch ())
            .GeoClearWatch(fun _ -> geoClearWatch ())
            .GeoPermissions(fun _ -> geoPermissions ())
            .ShowAlert(fun _ -> showAlert ())
            .ShowConfirm(fun _ -> showConfirm ())
            .ShowPrompt(fun _ -> showPrompt ())
            .ShowToast(fun _ -> showToast ())
            .ShowActionSheet(fun _ -> showActionSheet ())
            .HapticImpact(fun _ -> hapticImpact ())
            .HapticNotification(fun _ -> hapticNotification ())
            .HapticVibrate(fun _ -> hapticVibrate ())
            .HapticSelection(fun _ -> hapticSelection ())
            .LocalPermissions(fun _ -> localPermissions ())
            .LocalSchedule(fun _ -> localSchedule ())
            .LocalPending(fun _ -> localPending ())
            .LocalDelivered(fun _ -> localDelivered ())
            .LocalAll(fun _ -> localAll ())
            .LocalCancelAll(fun _ -> localCancelAll ())
            .LocalChannel(fun _ -> localChannel ())
            .LocalListChannels(fun _ -> localListChannels ())
            .PushRegister(fun _ -> pushRegister ())
            .ShareCanShare(fun _ -> shareCanShare ())
            .ShareIt(fun _ -> shareIt ())
            .OpenBrowser(fun _ -> openBrowser ())
            .CloseBrowser(fun _ -> closeBrowser ())
            .LaunchCanOpen(fun _ -> launchCanOpen ())
            .LaunchOpen(fun _ -> launchOpen ())
            .InAppExternal(fun _ -> inAppExternal ())
            .InAppWebView(fun _ -> inAppWebView ())
            .StatusBarInfo(fun _ -> statusBarInfo ())
            .KeyboardOps(fun _ -> keyboardOps ())
            .OrientationInfo(fun _ -> orientationInfo ())
            .ScreenReaderOps(fun _ -> screenReaderOps ())
            .TextZoomOps(fun _ -> textZoomOps ())
            .SplashOps(fun _ -> splashOps ())
            .BiometricCheck(fun _ -> biometricCheck ())
            .BackgroundRunnerOps(fun _ -> backgroundRunnerOps ())
            .GoogleMapsCheck(fun _ -> googleMapsCheck ())
            .HttpGet(fun _ -> httpGet ())
            .HttpPost(fun _ -> httpPost ())
            .ClearLog(fun _ -> log.Value <- "")
            .Bind()
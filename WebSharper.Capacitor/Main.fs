namespace Capacitor

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =
    [<AutoOpen>]
    module Core = 
        let ListenFunctionType schemaType  = schemaType ^-> T<unit>

        let PermissionState =
            Pattern.EnumStrings "PermissionState" [
                "prompt"
                "prompt-with-rationale"
                "granted"
                "denied"
            ]
            
        let PluginListenerHandle =
            Class "PluginListenerHandle"
            |+> Instance ["remove" => T<unit> ^-> T<Promise<unit>>]

        let PresentationStyle = 
            Pattern.EnumStrings "PresentationStyle" [
                "fullscreen"
                "popover"
            ]

        /// The runtime API exposed by `@capacitor/core`.
        let CapacitorCore =
            Class "CapacitorCore"
            |+> Static [
                "getPlatform" => T<unit> ^-> T<string>
                |> WithComment "Returns the platform the app is running on: 'web', 'ios' or 'android'."
                "isNativePlatform" => T<unit> ^-> T<bool>
                "isPluginAvailable" => T<string>?name ^-> T<bool>
                "convertFileSrc" => T<string>?filePath * !?T<string>?protocol ^-> T<string>
            ]
            |> Import "Capacitor" "@capacitor/core"

    [<AutoOpen>]
    module ActionSheet = 
        let ActionSheetButtonStyle =
            Pattern.EnumStrings "ActionSheetButtonStyle" [
                "DEFAULT"
                "DESTRUCTIVE"
                "CANCEL"
            ]

        let ActionSheetButton =
            Pattern.Config "ActionSheetButton" {
                Required = [("title", T<string>)]
                Optional = [
                    "style", ActionSheetButtonStyle.Type
                    "icon", T<string>
                ]
            }            

        let ShowActionsOptions =
            Pattern.Config "ShowActionsOptions" {
                Required = [("options", !| ActionSheetButton)]
                Optional = [
                    "title", T<string>
                    "message", T<string>
                    // Since 8.1.0: cancel the sheet when tapping outside of it.
                    "cancelable", T<bool>
                ]
            }

        let ShowActionsResult = 
            Pattern.Config "ShowActionsResult" {
                Required = [
                    "index", T<int>
                    // Since 8.1.0.
                    "canceled", T<bool>
                ]
                Optional = []
            }

        let ActionSheetPlugin = 
            Class "ActionSheetPlugin" 
            |+> Instance [
                "showActions" => ShowActionsOptions?options ^-> T<Promise<_>>[ShowActionsResult]
            ]

    [<AutoOpen>]
    module AppLauncher =
        let CanOpenURLResult = 
            Pattern.Config "CanOpenURLResult" {
                Required = [("value", T<bool>)]
                Optional = []
            }
        
        let CanOpenURLOptions = 
            Pattern.Config "CanOpenURLOptions" {
                Required = [("url", T<string>)]
                Optional = []
            }

        let OpenURLResult =
            Pattern.Config "OpenURLResult" {
                Required = [("completed", T<bool>)]
                Optional = []
            }

        let OpenURLOptions = 
            Pattern.Config "OpenURLOptions" {
                Required = [("url", T<string>)]
                Optional = []
            }

        let AppLauncherPlugin  = 
            Class "AppLauncherPlugin" 
            |+> Instance [
                "canOpenUrl" => CanOpenURLOptions?options ^-> T<Promise<_>>[CanOpenURLResult]
                "openUrl" => OpenURLOptions?options ^-> T<Promise<_>>[OpenURLResult]
            ]

    [<AutoOpen>]
    module App = 
        let AppInfo = 
            Pattern.Config "AppInfo" {
                Required = [
                    "name", T<string>
                    "id", T<string>
                    "build", T<string>
                    "version", T<string>
                ]
                Optional = []
            }

        let AppState =
            Pattern.Config "AppState" {
                Required = [("isActive", T<bool>)]
                Optional = []
            }

        let AppLaunchUrl =
            Pattern.Config "AppLaunchUrl" {
                Required = [("url", T<string>)]
                Optional = []
            }

        let URLOpenListenerEvent = 
            Pattern.Config "URLOpenListenerEvent" {
                Required = [("url", T<string>)]
                Optional = [
                    "iosSourceApplication", T<obj>
                    "iosOpenInPlace", T<bool>
                ]
            }

        let RestoredListenerError = 
            Pattern.Config "RestoredListenerError" {
                Required = [("message", T<string>)]
                Optional = []
            }

        let RestoredListenerEvent = 
            Pattern.Config "RestoredListenerEvent" {
                Required = [
                    "pluginId", T<string>
                    "methodName", T<string>
                    "success", T<bool>
                ]
                Optional = [
                    "data", T<obj>
                    "error", RestoredListenerError.Type
                ]
            }

        let BackButtonListenerEvent = 
            Pattern.Config "BackButtonListenerEvent" {
                Required = [("canGoBack", T<bool>)]
                Optional = []
            }

        let ToggleBackButtonHandlerOptions = 
            Pattern.Config "ToggleBackButtonHandlerOptions" {
                Required = [("enabled", T<bool>)]
                Optional = []
            }

        let AppLanguageCode = 
            Pattern.Config "AppLanguageCode" {
                Required = [("value", T<string>)]
                Optional = []
            }

        let AppOptions = 
            Pattern.Config "AppOptions" {
                Required = []
                Optional = [
                    ("disableBackButtonHandler", T<bool>)
                ]
            }

        let PluginsConfig = 
            Pattern.Config "PluginsConfig" {
                Required = []
                Optional = [
                    ("App", AppOptions.Type)
                ]
            }

        let StateChangeListener = AppState ^-> T<unit>

        let URLOpenListener = URLOpenListenerEvent ^-> T<unit>

        let RestoredListener = RestoredListenerEvent ^-> T<unit>

        let BackButtonListener = BackButtonListenerEvent ^-> T<unit>

        let AppPlugin = 
            Class "AppPlugin"
            |+> Instance [
                "exitApp" => T<unit> ^-> T<Promise<unit>>
                "getInfo" => T<unit> ^-> T<Promise<_>>[AppInfo]
                "getState" => T<unit> ^-> T<Promise<_>>[AppState]
                "getLaunchUrl" => T<unit> ^-> T<Promise<_>>[AppLaunchUrl]
                "minimizeApp" => T<unit> ^-> T<Promise<unit>>
                "getAppLanguage" => T<unit> ^-> T<Promise<_>>[AppLanguageCode]
                |> WithComment "Gets the app specific language locale code. Since 8.1.0."
                "toggleBackButtonHandler" => ToggleBackButtonHandlerOptions?options ^-> T<Promise<unit>>
                |> WithComment "Enables or disables the plugin's back button handling during runtime (Android only). Since 7.1.0."
                "addListener" => T<string>?eventName * StateChangeListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'appStateChange' event."
                "addListener" => T<string>?eventName * ListenFunctionType T<unit>?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'pause' or 'resume' events."
                "addListener" => T<string>?eventName * URLOpenListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'appUrlOpen' event."
                "addListener" => T<string>?eventName * RestoredListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'appRestoredResult' event."
                "addListener" => T<string>?eventName * BackButtonListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'backButton' event."
                "removeAllListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module BackgroundRunner = 
        let API = 
            Pattern.EnumStrings "API" ["geolocation"; "notifications"]

        let PermissionStatus = 
            Pattern.Config "PermissionStatus" {
                Required = [
                    "geolocation", PermissionState.Type
                    "notifications", PermissionState.Type
                ]
                Optional = []
            }

        let DispatchEventOptions = 
            Pattern.Config "DispatchEventOptions" {
                Required = [
                    "label", T<string>
                    "event", T<string>
                    "details", T<obj>
                ]
                Optional = []
            }

        let RequestPermissionOptions = 
            Pattern.Config "RequestPermissionOptions" {
                Required = [
                    ("apis", !| API) 
                ]
                Optional = []
            }

        let BackgroundRunnerOptions = 
            Pattern.Config "BackgroundRunnerOptions" {
                Required = [
                    "label", T<string>
                    "src", T<string>
                    "event", T<string>
                    "repeat", T<bool>
                    "interval", T<int>
                ]
                Optional = [
                    ("autoStart", T<bool>)
                ]
            }

        let PluginsConfig  = 
            Pattern.Config "PluginsConfig" {
                Required = []
                Optional = [
                    ("BackgroundRunner", BackgroundRunnerOptions.Type)
                ]
            }

        let NotificationActionEvent = 
            Pattern.Config "NotificationActionEvent" {
                Required = [
                    "actionTypeId", T<string>
                    "notificationId", T<int>
                ]
                Optional = []
            }

        let BackgroundRunnerPlugin =
            Class "BackgroundRunnerPlugin"
            |+> Instance [
                "checkPermissions" => T<unit> ^-> T<Promise<_>>[PermissionStatus]
                "requestPermissions" => RequestPermissionOptions?options ^-> T<Promise<_>>[PermissionStatus]
                "dispatchEvent" => DispatchEventOptions?options ^-> T<Promise<obj>>
                "addListener" => T<string>?eventName * ListenFunctionType NotificationActionEvent?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'backgroundRunnerNotificationReceived' event."
                "removeNotificationListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module BarcodeScanner =
        let CapacitorBarcodeScannerCameraDirection = 
            Pattern.EnumInlines "CapacitorBarcodeScannerCameraDirection" [
                "BACK", "1"
                "FRONT", "2"
            ]

        let CapacitorBarcodeScannerScanOrientation = 
            Pattern.EnumInlines "CapacitorBarcodeScannerScanOrientation" [
               "PORTRAIT", "1"
               "LANDSCAPE", "2"
               "ADAPTIVE", "3" 
            ]

        let CapacitorBarcodeScannerAndroidScanningLibrary = 
            Pattern.EnumStrings "CapacitorBarcodeScannerAndroidScanningLibrary" ["zxing"; "mlkit"]

        let CapacitorBarcodeScannerTypeHint =
            Pattern.EnumInlines "CapacitorBarcodeScannerTypeHint" [
                    "QR_CODE", "0"
                    "AZTEC", "1"
                    "CODABAR", "2"
                    "CODE_39", "3"
                    "CODE_93", "4"
                    "CODE_128", "5"
                    "DATA_MATRIX", "6"
                    "MAXICODE", "7"
                    "ITF", "8"
                    "EAN_13", "9"
                    "EAN_8", "10"
                    "PDF_417", "11"
                    "RSS_14", "12"
                    "RSS_EXPANDED", "13"
                    "UPC_A", "14"
                    "UPC_E", "15"
                    "UPC_EAN_EXTENSION", "16"
                    "ALL", "17"
                ]

        let CapacitorBarcodeScannerScanResult = 
            Pattern.Config "CapacitorBarcodeScannerScanResult" {
                Required = [
                    "ScanResult", T<string>
                    "format", CapacitorBarcodeScannerTypeHint.Type
                ]
                Optional = []
            }

        let AndroidScanningLibrary = 
            Pattern.Config "AndroidScanningLibrary" {
                Required = [("scanningLibrary", CapacitorBarcodeScannerAndroidScanningLibrary.Type)]
                Optional = []
            }

        let WebOptions = 
            Pattern.Config "WebOptions" {
                Required = []
                Optional = [
                    "showCameraSelection", T<bool>
                    "scannerFPS", T<int>
                ]
            }

        let CapacitorBarcodeScannerOptions = 
            Pattern.Config "CapacitorBarcodeScannerOptions" {
                Required = [
                    ("hint", CapacitorBarcodeScannerTypeHint.Type)
                ]
                Optional = [
                    "scanInstructions", T<string>
                    "scanButton", T<bool>
                    "scanText", T<string>
                    "cameraDirection", CapacitorBarcodeScannerCameraDirection.Type
                    "scanOrientation", CapacitorBarcodeScannerScanOrientation.Type
                    "cancelButtonAccessibilityLabel", T<string>
                    "torchButtonOnAccessibilityLabel", T<string>
                    "torchButtonOffAccessibilityLabel", T<string>
                    "android", AndroidScanningLibrary.Type
                    "web", WebOptions.Type
                ]
            }

        let BarcodeScannerPlugin = 
            Class "BarcodeScannerPlugin"
            |+> Instance [
                "scanBarcode" => CapacitorBarcodeScannerOptions?options ^-> T<Promise<_>>[CapacitorBarcodeScannerScanResult]
            ]

    [<AutoOpen>]
    module Browser = 
        let OpenOptions = 
            Pattern.Config "OpenOptions" {
                Required = [("url", T<string>)]
                Optional = [
                    "windowName", T<string>
                    "toolbarColor", T<string>
                    "presentationStyle", PresentationStyle.Type
                    "width", T<int>
                    "height", T<int>
                ]
            }

        let BrowserPlugin = 
            Class "BrowserPlugin"
            |+> Instance [
                "open" => OpenOptions?options ^-> T<Promise<unit>>
                "close" => T<unit> ^-> T<Promise<unit>>
                "addListener" => T<string>?eventName * ListenFunctionType T<unit>?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'browserFinished' and 'browserPageLoaded' events."
                "removeAllListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Camera = 
        let CameraPermissionState = 
            Pattern.EnumStrings "CameraPermissionState" [
                "prompt"
                "prompt-with-rationale"
                "granted"
                "denied"
                "limited"
            ]

        let CameraPermissionType =
            Pattern.EnumStrings "CameraPermissionType" [
                "camera"
                "photos"
            ]

        let CameraResultType = 
            Pattern.EnumStrings "CameraResultType" [
                "uri"
                "base64"
                "dataUrl"
            ]

        let CameraSource = 
            Pattern.EnumStrings "CameraSource" [
                "PROMPT"
                "CAMERA"
                "PHOTOS"
            ]

        let CameraDirection = 
            Pattern.EnumStrings "CameraDirection" [
                "REAR"
                "FRONT"
            ]

        let MediaType =
            Pattern.EnumInlines "MediaType" [
                "Photo", "0"
                "Video", "1"
            ]

        let MediaTypeSelection =
            Pattern.EnumInlines "MediaTypeSelection" [
                "Photo", "0"
                "Video", "1"
                "All", "2"
            ]

        let EncodingType =
            Pattern.EnumInlines "EncodingType" [
                "JPEG", "0"
                "PNG", "1"
            ]

        let Editable =
            Pattern.EnumInlines "Editable" [
                "InApp", "\"in-app\""
                "External", "\"external\""
                "No", "\"no\""
            ]

        let CameraErrorCode =
            Pattern.EnumStrings "CameraErrorCode" [
                "CameraPermissionDenied"
                "GalleryPermissionDenied"
                "NoCameraAvailable"
                "TakePhotoCancelled"
                "TakePhotoFailed"
                "TakePhotoInvalidArguments"
                "InvalidImageData"
                "EditPhotoFailed"
                "EditPhotoCancelled"
                "EditPhotoEmptyUri"
                "ImageNotFound"
            ]
    
        let ImageOptions = 
            Pattern.Config "ImageOptions" {
                Required = [
                    ("resultType", CameraResultType.Type)
                ]
                Optional = [
                    "quality", T<int>
                    "allowEditing", T<bool>
                    "saveToGallery", T<bool>
                    "width", T<int>
                    "height", T<int>
                    "correctOrientation", T<bool>
                    "source", CameraSource.Type
                    "direction", CameraDirection.Type
                    "presentationStyle", PresentationStyle.Type
                    "webUseInput", T<bool>
                    "promptLabelHeader", T<string>
                    "promptLabelCancel", T<string>
                    "promptLabelPhoto", T<string>
                    "promptLabelPicture", T<string>
                ]
            }

        let Photo =
            Pattern.Config "Photo" {
                Required = [
                    "format", T<string>
                    "saved", T<bool>
                ]
                Optional = [
                    "base64String", T<string>
                    "dataUrl", T<string>
                    "path", T<string>
                    "webPath", T<string>
                    "exif", T<obj>
                ]
            }

        let GalleryPhoto = 
            Pattern.Config "GalleryPhoto" {
                Required = [
                    "webPath", T<string>
                    "format", T<string>
                ]
                Optional = [
                    "path", T<string>
                    "exif", T<obj>
                ]
            }

        let GalleryPhotos = 
            Pattern.Config "GalleryPhotos" {
                Required = [
                    ("photos", !|GalleryPhoto)
                ]
                Optional = []
            }

        let GalleryImageOptions = 
            Pattern.Config "GalleryImageOptions" {
                Required = []
                Optional = [
                    "quality", T<int>
                    "width", T<int>
                    "height", T<int>
                    "correctOrientation", T<bool>
                    "presentationStyle", PresentationStyle.Type
                    "limit", T<int>
                ]
            }

        let PermissionStatus = 
            Pattern.Config "PermissionStatus"{
                Required = [
                    "camera", CameraPermissionState.Type
                    "photos", CameraPermissionState.Type
                ]
                Optional = []
            }

        let CameraPluginPermissions = 
            Pattern.Config "CameraPluginPermissions" {
                Required = [("permissions", !| CameraPermissionType)]
                Optional = []
            }             

        let TakePhotoOptions =
            Pattern.Config "TakePhotoOptions" {
                Required = []
                Optional = [
                    "quality", T<int>
                    "targetWidth", T<int>
                    "targetHeight", T<int>
                    "correctOrientation", T<bool>
                    "encodingType", EncodingType.Type
                    "saveToGallery", T<bool>
                    "cameraDirection", CameraDirection.Type
                    "editable", Editable.Type
                    "presentationStyle", PresentationStyle.Type
                    "webUseInput", T<bool>
                    "includeMetadata", T<bool>
                ]
            }

        let RecordVideoOptions =
            Pattern.Config "RecordVideoOptions" {
                Required = []
                Optional = [
                    "saveToGallery", T<bool>
                    "includeMetadata", T<bool>
                    "isPersistent", T<bool>
                ]
            }

        let PlayVideoOptions =
            Pattern.Config "PlayVideoOptions" {
                Required = [("uri", T<string>)]
                Optional = []
            }

        let ChooseFromGalleryOptions =
            Pattern.Config "ChooseFromGalleryOptions" {
                Required = []
                Optional = [
                    "mediaType", MediaTypeSelection.Type
                    "allowMultipleSelection", T<bool>
                    "limit", T<int>
                    "includeMetadata", T<bool>
                    "editable", Editable.Type
                    "presentationStyle", PresentationStyle.Type
                    "quality", T<int>
                    "targetWidth", T<int>
                    "targetHeight", T<int>
                    "correctOrientation", T<bool>
                    "webUseInput", T<bool>
                ]
            }

        let EditPhotoOptions =
            Pattern.Config "EditPhotoOptions" {
                Required = [("inputImage", T<string>)]
                Optional = []
            }

        let EditPhotoResult =
            Pattern.Config "EditPhotoResult" {
                Required = [("outputImage", T<string>)]
                Optional = []
            }

        let EditURIPhotoOptions =
            Pattern.Config "EditURIPhotoOptions" {
                Required = [("uri", T<string>)]
                Optional = [
                    "saveToGallery", T<bool>
                    "includeMetadata", T<bool>
                ]
            }

        let MediaMetadata =
            Pattern.Config "MediaMetadata" {
                Required = [("format", T<string>)]
                Optional = [
                    "size", T<float>
                    "duration", T<float>
                    "resolution", T<string>
                    "creationDate", T<string>
                    "exif", T<string>
                ]
            }

        let MediaResult =
            Pattern.Config "MediaResult" {
                Required = [
                    "type", MediaType.Type
                    "saved", T<bool>
                ]
                Optional = [
                    "uri", T<string>
                    "thumbnail", T<string>
                    "webPath", T<string>
                    "metadata", MediaMetadata.Type
                ]
            }

        let MediaResults =
            Pattern.Config "MediaResults" {
                Required = [("results", !| MediaResult)]
                Optional = []
            }

        let CameraPlugin = 
            Class "CameraPlugin"
            |+> Instance [
                "takePhoto" => TakePhotoOptions?options ^-> T<Promise<_>>[MediaResult]
                "recordVideo" => RecordVideoOptions?options ^-> T<Promise<_>>[MediaResult]
                "playVideo" => PlayVideoOptions?options ^-> T<Promise<unit>>
                "chooseFromGallery" => ChooseFromGalleryOptions?options ^-> T<Promise<_>>[MediaResults]
                "editPhoto" => EditPhotoOptions?options ^-> T<Promise<_>>[EditPhotoResult]
                "editURIPhoto" => EditURIPhotoOptions?options ^-> T<Promise<_>>[MediaResult]
                "pickLimitedLibraryPhotos" => T<unit> ^-> T<Promise<_>>[GalleryPhotos]
                "getLimitedLibraryPhotos" => T<unit> ^-> T<Promise<_>>[GalleryPhotos]
                "checkPermissions" => T<unit> ^-> T<Promise<_>>[PermissionStatus]
                "requestPermissions" => !?CameraPluginPermissions?permissions ^-> T<Promise<_>>[PermissionStatus]
                "getPhoto" => ImageOptions?options ^-> T<Promise<_>>[Photo]
                |> ObsoleteWithMessage "Use TakePhoto instead."
                "pickImages" => GalleryImageOptions?options ^-> T<Promise<_>>[GalleryPhotos]
                |> ObsoleteWithMessage "Use ChooseFromGallery instead."
            ]

    [<AutoOpen>]
    module Clipboard = 
        let WriteOptions =
            Pattern.Config "WriteOptions" {
                Required = []
                Optional = [
                    "string", T<string>
                    "image", T<string>
                    "url", T<string>
                    "label", T<string>
                ]
            }

        let ReadResult =
            Pattern.Config "ReadResult" {
                Required = []
                Optional = [
                    "value", T<string>
                    "type", T<string>
                ]
            }

        let ClipboardPlugin = 
            Class "ClipboardPlugin"
            |+> Instance [
                "write" => WriteOptions?options ^-> T<Promise<unit>>
                "read" => T<unit> ^-> T<Promise<_>>[ReadResult]
            ]

    [<AutoOpen>]
    module Cookies = 
        let HttpCookieMap =
            Class "HttpCookieMap"
            |+> Instance [
                "" =@ T<string> |> Indexed T<string>
            ]

        let GetCookieOptions = 
            Pattern.Config "GetCookieOptions" {
                Required = []
                Optional = [
                    ("url", T<string>)
                ]
            }

        let SetCookieOptions = 
            Pattern.Config "SetCookieOptions" {
                Required = [
                    "key", T<string>
                    "value", T<string>
                ]
                Optional = [
                    "url", T<string>
                    "path", T<string>
                    "expires", T<string>
                ]
            }

        let DeleteCookieOptions = 
            Pattern.Config "DeleteCookieOptions" {
                Required = [
                    ("key", T<string>)
                ]
                Optional = [
                    ("url", T<string>)
                ]
            }
        
        let ClearCookieOptions = 
            Pattern.Config "ClearCookieOptions" {
                Required = []
                Optional = [
                    ("url", T<string>)
                ]
            }

        let CookiesPlugin = 
            Class "CookiesPlugin" 
            |+> Instance [ 
                "getCookies" => GetCookieOptions?options ^-> T<Promise<_>>[HttpCookieMap]
                "setCookie" => SetCookieOptions?options ^-> T<Promise<unit>>
                "deleteCookie" => DeleteCookieOptions?options ^-> T<Promise<unit>>
                "clearCookies" => ClearCookieOptions?options ^-> T<Promise<unit>>
                "clearAllCookies" =>T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Device = 
        let OperatingSystem = 
            Pattern.EnumStrings "OperatingSystem" [
                "ios"
                "android"
                "windows"
                "mac"
                "unknown"
            ]

        let DeviceId =
            Pattern.Config "DeviceId" {
                Required = [("identifier", T<string>)]
                Optional = []
            }

        let DevicePlatform = 
            Pattern.EnumStrings "DevicePlatform" [
                "ios"
                "android"
                "web"
            ]

        let DeviceInfo =
            Pattern.Config "DeviceInfo" {
                Required = [
                    "model", T<string>
                    "platform", DevicePlatform.Type
                    "operatingSystem", OperatingSystem.Type
                    "osVersion", T<string>
                    "manufacturer", T<string>
                    "isVirtual", T<bool>
                    "webViewVersion", T<string>
                ]
                Optional = [
                    "name", T<string>
                    "iOSVersion", T<int>
                    "androidSDKVersion", T<int>
                    "memUsed", T<int>
                ]
            }

        let BatteryInfo =
            Pattern.Config "BatteryInfo" {
                Required = []
                Optional = [
                    "batteryLevel", T<int>
                    "isCharging", T<bool>
                ]
            }

        let GetLanguageCodeResult =
            Pattern.Config "GetLanguageCodeResult" {
                Required = []
                Optional = [
                    ("value", T<string>)
                ]
            }

        let LanguageTag =
            Pattern.Config "LanguageTag" {
                Required = []
                Optional = [
                    ("value", T<string>)
                ]
            }

        let DevicePlugin = 
            Class "DevicePlugin"
            |+> Instance [
                "getId" => T<unit> ^-> T<Promise<_>>[DeviceId]
                "getInfo" => T<unit> ^-> T<Promise<_>>[DeviceInfo]
                "getBatteryInfo" => T<unit> ^-> T<Promise<_>>[BatteryInfo]
                "getLanguageCode" => T<unit> ^-> T<Promise<_>>[GetLanguageCodeResult]
                "getLanguageTag" => T<unit> ^-> T<Promise<_>>[LanguageTag]
            ]

    [<AutoOpen>]
    module Dialog = 
        let AlertOptions =
            Pattern.Config "AlertOptions" {
                Required = [("message", T<string>)]
                Optional = [
                    "title", T<string>
                    "buttonTitle", T<string> 
                ]
            }

        let PromptResult =
            Pattern.Config "PromptResult" {
                Required = [
                    "value", T<string>
                    "cancelled", T<bool>
                ]
                Optional = []
            }

        let PromptOptions =
            Pattern.Config "PromptOptions" {
                Required = [("message", T<string>)]
                Optional = [
                    "title", T<string>
                    "okButtonTitle", T<string>
                    "cancelButtonTitle", T<string>
                    "inputPlaceholder", T<string>
                    "inputText", T<string>
                ]
            }

        let ConfirmResult =
            Pattern.Config "ConfirmResult" {
                Required = [
                    ("value", T<bool>)
                ]
                Optional = []
            }

        let ConfirmOptions =
            Pattern.Config "ConfirmOptions" {
                Required = [("message", T<string>)]
                Optional = [
                    "title", T<string>
                    "okButtonTitle", T<string>
                    "cancelButtonTitle", T<string>
                ]
            }

        let DialogPlugin = 
            Class "DialogPlugin"
            |+> Instance [
                "alert" =>  AlertOptions?options ^-> T<Promise<unit>>
                "prompt" =>  PromptOptions?options ^-> T<Promise<_>>[PromptResult]
                "confirm" =>  ConfirmOptions?options ^-> T<Promise<_>>[ConfirmResult]
            ]

    [<AutoOpen>]
    module Filesystem =
        let Directory = 
            Pattern.EnumStrings "Directory" [
                "DOCUMENTS"
                "DATA"
                "LIBRARY"
                "CACHE"
                "EXTERNAL"
                "EXTERNAL_STORAGE"
                // Since 7.1.0.
                "EXTERNAL_CACHE"
                "LIBRARY_NO_CLOUD"
                "TEMPORARY"
            ]

        let Encoding = 
            Pattern.EnumStrings "Encoding" [
                "utf8"
                "ascii"
                "utf16"
            ]

        let CopyOptions =
            Pattern.Config "CopyOptions" {
                Required = [
                    "from", T<string>
                    "to", T<string>
                ]
                Optional = [
                    "directory", Directory.Type
                    "toDirectory", Directory.Type
                ]
            }

        let RenameOptions = CopyOptions.Type

        let ProgressStatus = 
            Pattern.Config "ProgressStatus" {
                Required = [
                    "url", T<string>
                    "bytes", T<int>
                    "contentLength", T<int>
                ]
                Optional = []
            }

        let ProgressListener = ProgressStatus ^-> T<unit>

        let ReadFileResult =
            Pattern.Config "ReadFileResult" {
                Required = [
                    ("data", T<string> + T<Blob>)
                ]
                Optional = []
            }

        let ReadFileOptions = 
            Pattern.Config "ReadFileOptions" {
                Required = [
                    ("path", T<string>)
                ]
                Optional = [
                    "directory", Directory.Type
                    "encoding", Encoding.Type
                    "offset", T<int>
                    "length", T<int>
                ]
            }

        let ReadFileInChunksOptions = 
            Pattern.Config "ReadFileInChunksOptions" {
                Required = [
                    "path", T<string>
                    "chunkSize", T<int>
                ]
                Optional = [
                    "directory", Directory.Type
                    "encoding", Encoding.Type
                    "offset", T<int>
                    "length", T<int>
                ]
            }

        let ReadFileInChunksCallback = (ReadFileResult + T<unit>) * !?T<obj> ^-> T<unit>

        let WriteFileResult = 
            Pattern.Config "WriteFileResult" {
                Required = [
                    ("uri", T<string>) 
                ]
                Optional = []
            }

        let WriteFileOptions = 
            Pattern.Config "WriteFileOptions" {
                Required = [
                    "path", T<string> 
                    "data", T<string> + T<Blob>
                ]
                Optional = [
                    "directory", Directory.Type
                    "encoding", Encoding.Type
                    "recursive", T<bool> 
                ]
            }

        let AppendFileOptions =     
            Pattern.Config "AppendFileOptions" {
                Required = [
                    "path", T<string> 
                    "data", T<string>
                ]
                Optional = [
                    "directory", Directory.Type
                    "encoding", Encoding.Type
                ]
            }

        let DeleteFileOptions =     
            Pattern.Config "DeleteFileOptions" {
                Required = [
                    ("path", T<string>) 
                ]
                Optional = [
                    ("directory", Directory.Type)
                ]
            }

        let MkdirOptions =     
            Pattern.Config "MkdirOptions" {
                Required = [
                    ("path", T<string>) 
                ]
                Optional = [
                    "directory", Directory.Type
                    "recursive", T<bool> 
                ]
            }

        let RmdirOptions =     
            Pattern.Config "RmdirOptions" {
                Required = [
                    ("path", T<string>) 
                ]
                Optional = [
                    "directory", Directory.Type
                    "recursive", T<bool> 
                ]
            }

        let FileType = 
            Pattern.EnumStrings "FileType" [
                "file"
                "directory"
            ]

        let FileInfo = 
            Pattern.Config "FileInfo" {
                Required = [
                    "name", T<string> 
                    "type", FileType.Type
                    "size", T<int> 
                    "mtime", T<int> 
                    "uri", T<string> 
                ]
                Optional = [
                    ("ctime", T<int>) 
                ]
            }

        let ReaddirOptions = 
            Pattern.Config "ReaddirOptions" {
                Required = [
                    ("path", T<string>) 
                ]
                Optional = [
                    ("directory", Directory.Type)
                ]
            }

        let GetUriResult = 
            Pattern.Config "GetUriResult" {
                Required = [
                    ("uri", T<string>) 
                ]
                Optional = []
            }

        let GetUriOptions = 
            Pattern.Config "GetUriOptions" {
                Required = [
                    "path", T<string> 
                    "directory", Directory.Type 
                ]
                Optional = []
            }

        let StatResult = 
            Pattern.Config "StatResult" {
                Required = [
                    "name", T<string> 
                    "type", FileType.Type
                    "size", T<int> 
                    "mtime", T<int> 
                    "uri", T<string> 
                ]
                Optional = [
                    ("ctime", T<int>) 
                ]
            }

        let StatOptions = 
            Pattern.Config "StatOptions" {
                Required = [
                    ("path", T<string>) 
                ]
                Optional = [
                    ("directory", Directory.Type) 
                ]
            }

        let CopyResult = 
            Pattern.Config "CopyResult" {
                Required = [
                    ("uri", T<string>) 
                ]
                Optional = []
            }

        let PermissionStatus = 
            Pattern.Config "PermissionStatus" {
                Required = [
                    ("publicStorage", PermissionState.Type)
                ]
                Optional = []
            }

        let DownloadFileResult = 
            Pattern.Config "DownloadFileResult" {
                Required = []
                Optional = [
                    "path", T<string> 
                    "blob", T<Blob> 
                ]
            }

        let DownloadFileOptions = 
            Pattern.Config "DownloadFileOptions" {
                Required = [
                    "url", T<string>
                    "path", T<string> 
                ]
                Optional = [
                    "method", T<string>
                    "params", T<obj>
                    "data", T<obj>
                    "headers", T<obj>
                    "readTimeout", T<int>
                    "connectTimeout", T<int>
                    "disableRedirects", T<bool>
                    "webFetchExtra", T<Request>
                    "responseType", T<string>
                    "shouldEncodeUrlParams", T<bool>
                    "dataType", T<string>
                    "directory", Directory.Type
                    "progress", T<bool> 
                    "recursive", T<bool> 
                ]
            }

        let ReaddirResult = 
            Pattern.Config "ReaddirResult" {
                Required = [
                    ("files", !| FileInfo)
                ]
                Optional = []
            }

        let FilesystemPlugin = 
            Class "FilesystemPlugin"
            |+> Instance [
                "readFile" => ReadFileOptions?options ^-> T<Promise<_>>[ReadFileResult]
                "readFileInChunks" => ReadFileInChunksOptions?options * ReadFileInChunksCallback?callback ^-> T<Promise<_>>[T<string>]
                "writeFile" => WriteFileOptions?options ^-> T<Promise<_>>[WriteFileResult]
                "appendFile" => AppendFileOptions?options ^-> T<Promise<unit>>
                "deleteFile" => DeleteFileOptions?options ^-> T<Promise<unit>>
                "mkdir" => MkdirOptions?options ^-> T<Promise<unit>>
                "rmdir" => RmdirOptions?options ^-> T<Promise<unit>>
                "readdir" => ReaddirOptions?options ^-> T<Promise<_>>[ReaddirResult]
                "getUri" => GetUriOptions?options ^-> T<Promise<_>>[GetUriResult]
                "stat" => StatOptions?options ^-> T<Promise<_>>[StatResult]
                "rename" => RenameOptions?options ^-> T<Promise<unit>>
                "copy" => CopyOptions?options ^-> T<Promise<_>>[CopyResult]
                "checkPermissions" => T<unit> ^-> T<Promise<_>>[PermissionStatus]
                "requestPermissions" => T<unit> ^-> T<Promise<_>>[PermissionStatus]
                "downloadFile" => DownloadFileOptions?options ^-> T<Promise<_>>[DownloadFileResult]
                "addListener" => T<string>?eventName * ProgressListener?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                "removeAllListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Geolocation = 
        let CallbackID = T<string>
        
        let Coordinates =
            Pattern.Config "Coordinates" {
                Required = [
                    "latitude", T<float>
                    "longitude", T<float>
                    "accuracy", T<float>
                ]
                Optional = [
                    "altitudeAccuracy", T<float>
                    "altitude", T<float>
                    "speed", T<float>
                    "heading", T<float>
                    // Since 8.2.0, only available when using watchPosition.
                    "magneticHeading", T<float>
                    "trueHeading", T<float>
                    "headingAccuracy", T<float>
                    "course", T<float>
                ]
            }

        let Position =
            Pattern.Config "Position" {
                Required = [
                    "timestamp", T<int>
                    "coords", Coordinates.Type
                ]
                Optional = []
            }

        let WatchPositionCallback = (Position + T<unit>) * !?T<obj> ^-> T<unit>

        let GeolocationPermissionType =
            Pattern.EnumStrings "GeolocationPermissionType" [
                "location"
                "coarseLocation"
            ]

        let PositionOptions =
            Pattern.Config "PositionOptions" {
                Required = []
                Optional = [
                    "enableHighAccuracy", T<bool>
                    "timeout", T<int>
                    "maximumAge", T<int>
                    // Since 6.1.0 (Android only).
                    "minimumUpdateInterval", T<int>
                    // Since 8.0.0 (Android only).
                    "interval", T<int>
                    "enableLocationFallback", T<bool>
                ]
            }

        let ClearWatchOptions = 
            Pattern.Config "ClearWatchOptions" {
                Required = [("id", CallbackID)]
                Optional = []
            }

        let PermissionStatus = 
            Pattern.Config "PermissionStatus" {
                Required = [
                    "location", PermissionState.Type
                    "coarseLocation", PermissionState.Type
                ]
                Optional = []
            }

        let GeolocationPluginPermissions = 
            Pattern.Config "GeolocationPluginPermissions" {
                Required = [("permissions", !| GeolocationPermissionType)]
                Optional = []
            }

        let GeolocationPlugin = 
            Class "GeolocationPlugin"
            |+> Instance [
                "getCurrentPosition" => !?PositionOptions?options ^-> T<Promise<_>>[Position]
                "watchPosition" => PositionOptions?options * WatchPositionCallback?callback ^-> T<Promise<_>>[CallbackID]
                "clearWatch" => ClearWatchOptions?options ^-> T<Promise<unit>>
                "checkPermissions" => T<unit> ^-> T<Promise<_>>[PermissionStatus]
                "requestPermissions" => !?GeolocationPluginPermissions?permissions ^-> T<Promise<_>>[PermissionStatus]
            ]

    [<AutoOpen>]
    module GoogleMaps = 
        let MapType =
            Pattern.EnumStrings "MapType" [
                "Normal"
                "Hybrid"
                "Satellite"
                "Terrain"
                "None"
            ]

        let MapListenerCallback = Generic - fun t ->
            Pattern.Config "MapListenerCallback" {
                Required = [
                    ("data", t ^-> T<unit>)
                ]
                Optional = []
            }

        let LatLng = 
            Pattern.Config "LatLng" {
                Required = [
                    "lat", T<double> 
                    "lng", T<double>   
                ]
                Optional = []
            }

        let MapReadyCallbackData = 
            Pattern.Config "MapReadyCallbackData" {
                Required = []
                Optional = [
                    ("mapId", T<string>)
                ]
            }

        let Size = 
            Pattern.Config "Size" {
                Required = [
                    "width", T<int>
                    "height", T<int>
                ]
                Optional = []
            }

        let Point = 
            Pattern.Config "Point" {
                Required = [
                    "x", T<int>
                    "y", T<int>
                ]
                Optional = []
            }

        let Marker = 
            Pattern.Config "Marker" {
                Required = [
                    ("coordinate", LatLng.Type)
                ]
                Optional = [
                    "opacity", T<float>
                    "title", T<string>
                    "snippet", T<string>
                    "isFlat", T<bool>
                    "iconUrl", T<string>
                    "iconSize", Size.Type
                    "iconOrigin", Point.Type
                    "iconAnchor", Point.Type
                    "tintColor", T<obj>  
                    "draggable", T<bool>
                    "zIndex", T<int>
                ]
            }

        let Polygon = 
            Pattern.Config "Polygon" {
                Required = []
                Optional = [
                    "strokeColor", T<string>
                    "strokeOpacity", T<float>
                    "strokeWeight", T<int>
                    "fillColor", T<string>
                    "fillOpacity", T<float>
                    "geodesic", T<bool>
                    "clickable", T<bool>
                    "title", T<string>
                    "tag", T<string>
                ]
            }

        let Circle = 
            Pattern.Config "Circle" {
                Required = []
                Optional = [
                    "fillColor", T<string>
                    "fillOpacity", T<float>
                    "strokeColor", T<string>
                    "strokeWeight", T<int>
                    "geodesic", T<bool>
                    "clickable", T<bool>
                    "title", T<string>
                    "tag", T<string>
                ]
            }

        let StyleSpan = 
            Pattern.Config "StyleSpan" {
                Required = [
                    ("color", T<string>)
                ]
                Optional = [
                    ("segments", T<int>)
                ]
            }

        let TileOverlay = 
            Pattern.Config "TileOverlay" {
                Required = [
                    ("url", T<string>)
                ]
                Optional = [
                    "opacity", T<float>
                    "visible", T<bool>
                    "zIndex", T<int>
                ]
            }
            
        let Polyline = 
            Pattern.Config "Polyline" {
                Required = []
                Optional = [
                    "strokeColor", T<string>
                    "strokeOpacity", T<float>
                    "strokeWeight", T<int>
                    "geodesic", T<bool>
                    "clickable", T<bool>
                    "tag", T<string>
                    "styleSpans", !| StyleSpan
                ]
            } 

        let CameraConfig = 
            Pattern.Config "CameraConfig" {
                Required = []
                Optional = [
                    "coordinate", LatLng.Type
                    "zoom", T<int>
                    "bearing", T<float>
                    "angle", T<float>
                    "animate", T<bool>
                    "animationDuration", T<int>
                ]
            } 

        let MapPadding = 
            Pattern.Config "MapPadding" {
                Required = [
                    "top", T<int>
                    "left", T<int>
                    "right", T<int>
                    "bottom", T<int>
                ]
                Optional = []
            }

        let LatLngBoundsInterface = 
            Pattern.Config "LatLngBoundsInterface" {
                Required = [
                    "southwest", LatLng.Type
                    "center", LatLng.Type
                    "northeast", LatLng.Type
                ]
                Optional = []
            }

        let LatLngBounds = 
            Class "LatLngBounds"
            |+> Instance [
                "southwest" =@ LatLng
                "center" =@ LatLng
                "northeast" =@ LatLng
                Constructor (LatLngBoundsInterface?bounds)
                "contains" => LatLng?point ^-> T<Promise<bool>>
                "extend" => LatLng?point ^-> T<Promise<_>>[TSelf]
            ]

        let CameraIdleCallbackData = 
            Pattern.Config "CameraIdleCallbackData" {
                Required = []
                Optional = [
                    "mapId", T<string>
                    "bounds", LatLngBounds.Type
                    "bearing", T<float>
                    "latitude", T<float>
                    "longitude", T<float>
                    "tilt", T<int>
                    "zoom", T<int>
                ]
            }

        let CameraMoveStartedCallbackData = 
            Pattern.Config "CameraMoveStartedCallbackData" {
                Required = []
                Optional = [
                    "mapId", T<string>
                    "isGesture", T<bool>
                ]
            }

        let MarkerCallbackData = 
            Pattern.Config "MarkerCallbackData" {
                Required = []
                Optional = [
                    "markerId", T<string>
                    "latitude", T<float>
                    "longitude", T<float>
                    "title", T<string>
                    "snippet", T<string>
                ]
            }

        let ClusterClickCallbackData = 
            Pattern.Config "ClusterClickCallbackData" {
                Required = []
                Optional = [
                    "mapId", T<string>
                    "latitude", T<float>
                    "longitude", T<float>
                    "size", T<int>
                    "items", !| MarkerCallbackData
                ]
            }

        let PolylineCallbackData = 
            Pattern.Config "PolylineCallbackData" {
                Required = []
                Optional = [
                    "polylineId", T<string>
                    "tag", T<string>
                ]
            }

        let MapClickCallbackData = 
            Pattern.Config "MapClickCallbackData" {
                Required = []
                Optional = [
                    "mapId", T<string>
                    "latitude", T<float>
                    "longitude", T<float>
                ]
            }

        let MarkerClickCallbackData = 
            Pattern.Config "MarkerClickCallbackData" {
                Required = [("mapId", T<string>)]
                Optional = [
                    "markerId", T<string>
                    "latitude", T<float>
                    "longitude", T<float>
                    "title", T<string>
                    "snippet", T<string>
                ]
            }

        let PolygonClickCallbackData = 
            Pattern.Config "PolygonClickCallbackData" {
                Required = []
                Optional = [
                    "mapId", T<string>
                    "polygonId", T<string>
                    "tag", T<string>
                ]
            }

        let CircleClickCallbackData = 
            Pattern.Config "CircleClickCallbackData" {
                Required = []
                Optional = [
                    "mapId", T<string>
                    "circleId", T<string>
                    "tag", T<string>
                ]
            }

        let MyLocationButtonClickCallbackData = 
            Pattern.Config "MyLocationButtonClickCallbackData" {
                Required = []
                Optional = [
                    ("mapId", T<string>)
                ]
            }

        let GoogleMapConfig = 
            Pattern.Config "GoogleMapConfig" {
                Required = [
                    "center", LatLng.Type
                    "zoom", T<int>
                ]
                Optional = [
                    "width", T<int>
                    "height", T<int>
                    "x", T<int>
                    "y", T<int>
                    "androidLiteMode", T<bool>
                    "devicePixelRatio", T<int>
                    "styles", !| T<obj> 
                    "mapId", T<string>
                    "androidMapId", T<string>
                    "iOSMapId", T<string>
                    "maxZoom", T<int>
                    "minZoom", T<int>
                    "mapTypeId", T<string>
                    "heading", T<int>
                    "restriction", T<obj>
                ]
            }

        let CreateMapArgs =
            Pattern.Config "CreateMapArgs" {
                Required = [
                    "id", T<string>
                    "apiKey", T<string>
                    "config", GoogleMapConfig.Type
                    "element", T<Dom.Element> 
                ]
                Optional = [
                    "forceCreate", T<bool>
                    "region", T<string>
                    "language", T<string>
                ]
        }

        let GoogleMapsPlugin =
            Class "GoogleMapsPlugin"
            |+> Instance [
                "create" => CreateMapArgs?options * !?MapListenerCallback[MapReadyCallbackData]?callback ^-> T<Promise<_>>[T<obj>]
                "enableTouch" => T<unit> ^-> T<Promise<unit>>
                "disableTouch" => T<unit> ^-> T<Promise<unit>>
                "enableClustering" => !?T<int>?minClusterSize ^-> T<Promise<unit>> 
                "disableClustering" => T<unit> ^-> T<Promise<unit>>
                "addTileOverlay" => TileOverlay?tileOverlay ^-> T<Promise<string>>
                "removeTileOverlay" => T<string>?id ^-> T<Promise<unit>>
                "addMarker" => Marker?marker ^-> T<Promise<string>>
                "addMarkers" => (!|Marker)?markers ^-> T<Promise<_>>[!|T<string>]
                "removeMarker" => T<string>?id ^-> T<Promise<unit>>
                "removeMarkers" => (!|T<string>)?ids ^-> T<Promise<unit>>
                "addPolygons" => (!|Polygon)?polygons ^-> T<Promise<_>>[!|T<string>]  
                "removePolygons" => (!|T<string>)?ids ^-> T<Promise<unit>>
                "addCircles" => (!|Circle)?circles ^-> T<Promise<_>>[!|T<string>]
                "removeCircles" => (!|T<string>)?ids ^-> T<Promise<unit>>
                "addPolylines" => (!|Polyline)?polylines ^-> T<Promise<_>>[!|T<string>]
                "removePolylines" => (!|T<string>)?ids ^-> T<Promise<unit>>
                "destroy" => T<unit> ^-> T<Promise<unit>>
                "setCamera" => CameraConfig?config ^-> T<Promise<unit>>
                "getMapType" => T<unit> ^-> T<Promise<_>>[MapType]
                "setMapType" => MapType?mapType ^-> T<Promise<unit>>
                "enableIndoorMaps" => T<bool>?enable ^-> T<Promise<unit>>
                "enableTrafficLayer" => T<bool>?enable ^-> T<Promise<unit>>
                "enableAccessibilityElements" => T<bool>?enable ^-> T<Promise<unit>>
                "enableCurrentLocation" => T<bool>?enable ^-> T<Promise<unit>>
                "setPadding" => MapPadding?padding ^-> T<Promise<unit>>
                "getMapBounds" => T<unit> ^-> T<Promise<_>>[LatLngBounds]
                "fitBounds" => LatLngBounds?bounds * !?T<int>?padding ^-> T<Promise<unit>>
                "setOnBoundsChangedListener" => !?MapListenerCallback[CameraIdleCallbackData]?callback ^-> T<Promise<unit>>
                "setOnCameraIdleListener" => !?MapListenerCallback[CameraIdleCallbackData]?callback ^-> T<Promise<unit>>
                "setOnCameraMoveStartedListener" => !?MapListenerCallback[CameraMoveStartedCallbackData]?callback ^-> T<Promise<unit>>
                "setOnClusterClickListener" => !?MapListenerCallback[ClusterClickCallbackData]?callback ^-> T<Promise<unit>>
                "setOnClusterInfoWindowClickListener" => !?MapListenerCallback[ClusterClickCallbackData]?callback ^-> T<Promise<unit>>
                "setOnInfoWindowClickListener" => !?MapListenerCallback[MarkerClickCallbackData]?callback ^-> T<Promise<unit>>
                "setOnMapClickListener" => !?MapListenerCallback[MapClickCallbackData]?callback ^-> T<Promise<unit>>
                "setOnMarkerClickListener" => !?MapListenerCallback[MarkerClickCallbackData]?callback ^-> T<Promise<unit>>
                "setOnPolygonClickListener" => !?MapListenerCallback[PolygonClickCallbackData]?callback ^-> T<Promise<unit>>
                "setOnCircleClickListener" => !?MapListenerCallback[CircleClickCallbackData]?callback ^-> T<Promise<unit>>
                "setOnPolylineClickListener" => !?MapListenerCallback[PolylineCallbackData]?callback ^-> T<Promise<unit>>
                "setOnMarkerDragStartListener" => !?MapListenerCallback[MarkerClickCallbackData]?callback ^-> T<Promise<unit>>
                "setOnMarkerDragListener" => !?MapListenerCallback[MarkerClickCallbackData]?callback ^-> T<Promise<unit>>
                "setOnMarkerDragEndListener" => !?MapListenerCallback[MarkerClickCallbackData]?callback ^-> T<Promise<unit>>
                "setOnMyLocationButtonClickListener" => !?MapListenerCallback[MyLocationButtonClickCallbackData]?callback ^-> T<Promise<unit>>
                "setOnMyLocationClickListener" => !?MapListenerCallback[MapClickCallbackData]?callback ^-> T<Promise<unit>>
                "removeAllMapListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Haptics = 
        let ImpactStyle =
            Pattern.EnumStrings "ImpactStyle" [
                "HEAVY"
                "MEDIUM"
                "LIGHT"
            ]

        let NotificationType =
            Pattern.EnumStrings "NotificationType" [
                "SUCCESS"
                "WARNING"
                "ERROR"
            ]

        let ImpactOptions = 
            Pattern.Config "ImpactOptions" {
                Required = [
                    ("style", ImpactStyle.Type)
                ]
                Optional = []
            }
        
        let NotificationOptions = 
            Pattern.Config "NotificationOptions" {
                Required = [
                    ("type", NotificationType.Type)
                ]
                Optional = []
            }

        let VibrateOptions = 
            Pattern.Config "VibrateOptions" {
                Required = [
                    ("duration", T<int>)
                ]
                Optional = []
            }

        let HapticsPlugin = 
            Class "HapticsPlugin"
            |+> Instance [
                "impact" => !?ImpactOptions ?options ^-> T<Promise<unit>>
                "notification" => !?NotificationOptions?options ^-> T<Promise<unit>>
                "vibrate" => !?VibrateOptions?options ^-> T<Promise<unit>>
                "selectionStart" => T<unit> ^-> T<Promise<unit>>
                "selectionChanged" => T<unit> ^-> T<Promise<unit>>
                "selectionEnd" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Http = 
        let HttpParams = 
            Class "HttpParams"
            |+> Instance [
                "" =@ (T<string> + (!| T<string>)) |> Indexed T<string>
            ]

        let HttpHeaders = 
            Class "HttpHeaders"
            |+> Instance [
                "" =@ T<string> |> Indexed T<string>
            ]

        let HttpResponse = 
            Pattern.Config "HttpResponse" {
                Required = [
                    "data", T<obj>
                    "status", T<int>
                    "headers", HttpHeaders.Type
                    "url", T<string>
                ]
                Optional = []
            }

        let HttpResponseType =
            Pattern.EnumInlines "HttpResponseType" [
                "ArrayBuffer", "\"arraybuffer\""
                "Blob", "\"blob\""
                "Json", "\"json\""
                "Text", "\"text\""
                "Document", "\"document\""
            ]

        let HttpDataType =
            Pattern.EnumInlines "HttpDataType" [
                "File", "\"file\""
                "FormData", "\"formData\""
            ]

        let HttpOptions = 
            Pattern.Config "HttpOptions" {
                Required = [
                    ("url", T<string>)
                ]
                Optional = [
                    "method", T<string>
                    "params", HttpParams.Type
                    "data", T<obj>
                    "headers", HttpHeaders.Type
                    "readTimeout", T<int>
                    "connectTimeout", T<int>
                    "disableRedirects", T<bool>
                    "webFetchExtra", T<Request>
                    "responseType", HttpResponseType.Type
                    "shouldEncodeUrlParams", T<bool>
                    "dataType", HttpDataType.Type
                ]
            }

        let HttpPlugin = 
            Class "HttpPlugin"
            |+> Instance [
                "request" => HttpOptions?options ^-> T<Promise<_>>[HttpResponse]
                "get" => HttpOptions?options ^-> T<Promise<_>>[HttpResponse]
                "post" => HttpOptions?options ^-> T<Promise<_>>[HttpResponse]
                "put" => HttpOptions?options ^-> T<Promise<_>>[HttpResponse]
                "patch" => HttpOptions?options ^-> T<Promise<_>>[HttpResponse]
                "delete" => HttpOptions?options ^-> T<Promise<_>>[HttpResponse]
            ]

    [<AutoOpen>]
    module InAppBrowser = 
        let ToolbarPosition =
            Pattern.EnumInlines "ToolbarPosition" [
                "TOP", "0"
                "BOTTOM", "1"
            ]

        let iOSViewStyle =
            Pattern.EnumInlines "iOSViewStyle" [
                "PAGE_SHEET", "0"
                "FORM_SHEET", "1"
                "FULL_SCREEN", "2"
            ]

        let AndroidViewStyle =
            Pattern.EnumInlines "AndroidViewStyle" [
                "BOTTOM_SHEET", "0"
                "FULL_SCREEN", "1"
            ]

        let iOSAnimation =
            Pattern.EnumInlines "iOSAnimation" [
                "FLIP_HORIZONTAL", "0"
                "CROSS_DISSOLVE", "1"
                "COVER_VERTICAL", "2"
            ]

        let AndroidAnimation =
            Pattern.EnumInlines "AndroidAnimation" [
                "FADE_IN", "0"
                "FADE_OUT", "1"
                "SLIDE_IN_LEFT", "2"
                "SLIDE_OUT_RIGHT", "3"
            ]

        let DismissStyle =
            Pattern.EnumInlines "DismissStyle" [
                "CLOSE", "0"
                "CANCEL", "1"
                "DONE", "2"
            ]

        let AndroidWebViewOptions = 
            Pattern.Config "AndroidWebViewOptions" {
                Required = [
                    "allowZoom", T<bool>
                    "hardwareBack", T<bool>
                    "pauseMedia", T<bool>
                ]
                Optional = [
                    "isIsolated", T<bool>
                    "successUrlPatterns", !| T<string>
                ]
            }

        let iOSWebViewOptions = 
            Pattern.Config "iOSWebViewOptions" {
                Required = [
                    "allowOverScroll", T<bool>
                    "enableViewportScale", T<bool>
                    "allowInLineMediaPlayback", T<bool>
                    "surpressIncrementalRendering", T<bool>
                    "viewStyle", iOSViewStyle.Type
                    "animationEffect", iOSAnimation.Type
                    "allowsBackForwardNavigationGestures", T<bool>
                ]
                Optional = []
            }

        let AndroidBottomSheet = 
            Pattern.Config "AndroidBottomSheet" {
                Required = [
                    "height", T<int>
                    "isFixed", T<bool>
                ]
                Optional = []
            }

        let AndroidSystemBrowserOptions = 
            Pattern.Config "AndroidSystemBrowserOptions" {
                Required = [
                    "showTitle", T<bool>
                    "hideToolbarOnScroll", T<bool>
                    "viewStyle", AndroidViewStyle.Type
                    "startAnimation", AndroidAnimation.Type
                    "exitAnimation", AndroidAnimation.Type
                ]
                Optional = [
                    ("bottomSheetOptions", AndroidBottomSheet.Type)
                ]
            }

        let iOSSystemBrowserOptions = 
            Pattern.Config "iOSSystemBrowserOptions" {
                Required = [
                    "closeButtonText", DismissStyle.Type
                    "viewStyle", iOSViewStyle.Type
                    "animationEffect", iOSAnimation.Type
                    "enableBarsCollapsing", T<bool>
                    "enableReadersMode", T<bool>
                ]
                Optional = []
            }

        let OpenInDefaultParameterModel = 
            Pattern.Config "OpenInDefaultParameterModel" {
                Required = [
                    ("url", T<string>)
                ]
                Optional = []
            }

        let SystemBrowserOptions = 
            Pattern.Config "SystemBrowserOptions" {
                Required = [
                    "android", AndroidSystemBrowserOptions.Type
                    "iOS", iOSSystemBrowserOptions.Type
                ]
                Optional = []
            }

        let OpenInSystemBrowserParameterModel = 
            Pattern.Config "OpenInSystemBrowserParameterModel" {
                Required = [
                    "url", T<string>
                    "options", SystemBrowserOptions.Type
                ]
                Optional = []
            }

        let CustomHeaders = 
            Class "CustomHeaders"
            |+> Instance [
                "" =@ T<string> |> Indexed T<string>
            ]

        let WebViewOptions = 
            Pattern.Config "WebViewOptions" {
                Required = [
                    "showURL", T<bool>
                    "showToolbar", T<bool>
                    "clearCache", T<bool>
                    "clearSessionCache", T<bool>
                    "mediaPlaybackRequiresUserAction", T<bool>
                    "closeButtonText", T<string>
                    "toolbarPosition", ToolbarPosition.Type
                    "showNavigationButtons", T<bool>
                    "leftToRight", T<bool>
                    "android", AndroidWebViewOptions.Type
                    "iOS", iOSWebViewOptions.Type
                ]
                Optional = [
                    ("customWebViewUserAgent", T<string>)
                ]
            }

        let OpenInWebViewParameterModel = 
            Pattern.Config "OpenInWebViewParameterModel" {
                Required = [
                    "url", T<string>
                    "options", WebViewOptions.Type
                ]
                Optional = [
                    ("customHeaders", CustomHeaders.Type)
                ]
            }

        let BrowserPageNavigationCompletedEventData = 
            Pattern.Config "BrowserPageNavigationCompletedEventData" {
                Required = []
                Optional = [
                    ("url", T<string>)
                ]
            }

        let InAppBrowserPlugin = 
            Class "InAppBrowserPlugin"
            |+> Instance [
                "openInWebView" => OpenInWebViewParameterModel?model ^-> T<Promise<unit>>
                "openInSystemBrowser" => OpenInSystemBrowserParameterModel?model ^-> T<Promise<unit>>
                "openInExternalBrowser" => OpenInDefaultParameterModel?model ^-> T<Promise<unit>>
                "close" => T<unit> ^-> T<Promise<unit>>
                "addListener" => T<string>?eventName * ListenFunctionType T<unit>?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'browserClosed' and 'browserPageLoaded' events."
                "addListener" => T<string>?eventName * ListenFunctionType BrowserPageNavigationCompletedEventData?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'browserPageNavigationCompleted' event."
                "removeAllListeners" => T<unit> ^-> T<unit>
            ]
 
    [<AutoOpen>]
    module Keyboard =
        let KeyboardStyle = 
            Pattern.EnumStrings "KeyboardStyle" [
                "DARK"
                "LIGHT"
                "DEFAULT"
            ]

        let KeyboardResize = 
            Pattern.EnumStrings "KeyboardResize" [
                "body"
                "ionic"
                "native"
                "none"
            ]

        let KeyboardStyleOptions = 
            Pattern.Config "KeyboardStyleOptions" {
                Required = [
                    ("style", KeyboardStyle.Type)
                ]
                Optional = []
            }

        let KeyboardResizeOptions = 
            Pattern.Config "KeyboardResizeOptions" {
                Required = [
                    ("mode", KeyboardResize.Type)
                ]
                Optional = []
            }

        let KeyboardInfo = 
            Pattern.Config "KeyboardInfo" {
                Required = [
                    ("keyboardHeight", T<int>)
                ]
                Optional = []
            }

        let ScrollOptions =
            Pattern.Config "ScrollOptions" {
                Required = [ ("isDisabled", T<bool>) ]
                Optional = []
            }

        let AccessoryBarOptions =
            Pattern.Config "AccessoryBarOptions" {
                Required = [ ("isVisible", T<bool>) ]
                Optional = []
            }

        let KeyboardBackdropColor =
            Pattern.EnumStrings "KeyboardBackdropColor" [
                "off"
                "auto"
                "dom"
            ]

        let KeyboardOptions =
            Pattern.Config "KeyboardOptions" {
                Required = []
                Optional = [
                    "resize", KeyboardResize.Type
                    "style", KeyboardStyle.Type
                    "resizeOnFullScreen", T<bool>
                    // Since 8.0.4 (iOS only).
                    "autoBackdropColor", KeyboardBackdropColor.Type
                ]
            }

        let PluginsConfig =
            Pattern.Config "PluginsConfig" {
                Required = []
                Optional = [
                    ("Keyboard", KeyboardOptions.Type)
                ]
            }

        let KeyboardPlugin =
            Class "KeyboardPlugin"
            |+> Instance [
                "show" => T<unit> ^-> T<Promise<unit>>
                "hide" => T<unit> ^-> T<Promise<unit>>
                "setAccessoryBarVisible" => AccessoryBarOptions?option ^-> T<Promise<unit>>
                "setScroll" => ScrollOptions?option ^-> T<Promise<unit>>
                "setStyle" => KeyboardStyleOptions?option ^-> T<Promise<unit>>
                "setResizeMode" => KeyboardResizeOptions?option ^-> T<Promise<unit>>
                "getResizeMode" => T<unit> ^-> T<Promise<_>>[KeyboardResizeOptions]
                "addListener" => T<string>?eventName * ListenFunctionType KeyboardInfo?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listen for keyboardWillShow or keyboardDidShow events"
                "addListener" => T<string>?eventName * ListenFunctionType T<unit>?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listen for keyboardWillHide or keyboardDidHide events"
                "removeAllListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module LocalNotifications = 
        let Weekday = 
            Pattern.EnumInlines "Weekday" [
                "Sunday", "1"
                "Monday", "2"
                "Tuesday", "3"
                "Wednesday", "4"
                "Thursday", "5"
                "Friday", "6"
                "Saturday", "7"
            ]

        let ScheduleEvery = 
            Pattern.EnumStrings "ScheduleEvery" [
                "year"
                "month"
                "two-weeks"
                "week"
                "day"
                "hour"
                "minute"
                "second"
            ]

        let Importance = 
            Pattern.EnumStrings "Importance" ["0"; "1"; "2"; "3"; "4"; "5"]

        let Visibility = 
            Pattern.EnumStrings "Visibility" ["-1"; "0"; "1"]

        let InterruptionLevel = 
            Pattern.EnumStrings "InterruptionLevel" [
                "active"
                "critical"
                "passive"
                "timeSensitive"
            ]

        let LocalNotificationPresentationOption = 
            Pattern.EnumStrings "LocalNotificationPresentationOption" [
                "badge"
                "sound"
                "banner"
                "list"
            ]

        let NotificationState = 
            Pattern.EnumStrings "NotificationState" [
                "SCHEDULED"
                "TRIGGERED"
            ]

        let LocalNotificationDescriptor = 
            Pattern.Config "LocalNotificationDescriptor" {
                Required = [
                    ("id", T<int>)
                ]
                Optional = []
            }

        let ScheduleWarning = 
            Pattern.Config "ScheduleWarning" {
                Required = [
                    "code", T<string>
                    "message", T<string>
                ]
                Optional = []
            }

        let ScheduleResult = 
            Pattern.Config "ScheduleResult" {
                Required = [
                    ("notifications", !| LocalNotificationDescriptor)
                ]
                Optional = [
                    // Since 8.3.0.
                    ("warning", ScheduleWarning.Type)
                ]
            }

        let ScheduleOn = 
            Pattern.Config "ScheduleOn" {
                Required = []
                Optional = [
                    "year", T<int>
                    "month", T<int>
                    "day", T<int>
                    "weekday", Weekday.Type
                    "hour", T<int>
                    "minute", T<int>
                    "second", T<int>
                ]
            }

        let Schedule =
            Pattern.Config "Schedule" {
                Required = []
                Optional = [
                    "at", T<Date>
                    "repeats", T<bool>
                    "allowWhileIdle", T<bool>
                    "on", ScheduleOn.Type
                    "every", ScheduleEvery.Type
                    "count", T<int>
                ]
            }

        let AttachmentOptions =
            Pattern.Config "AttachmentOptions" {
                Required = []
                Optional = [
                    "iosUNNotificationAttachmentOptionsTypeHintKey", T<string>
                    "iosUNNotificationAttachmentOptionsThumbnailHiddenKey", T<string>
                    "iosUNNotificationAttachmentOptionsThumbnailClippingRectKey", T<string>
                    "iosUNNotificationAttachmentOptionsThumbnailTimeKey", T<string>
                ]
            }

        let Attachment =
            Pattern.Config "Attachment" {
                Required = [
                    "id", T<string>
                    "url", T<string>
                ]
                Optional = [
                    ("options", AttachmentOptions.Type)
                ]
            }

        let LocalNotificationSchema = 
            Pattern.Config "LocalNotificationSchema" {
                Required = [
                    "title", T<string>
                    "body", T<string>
                    "id", T<int>
                ]
                Optional = [
                    "largeBody", T<string>
                    "summaryText", T<string>
                    "schedule", Schedule.Type
                    "sound", T<string>
                    "smallIcon", T<string>
                    "largeIcon", T<string>
                    "iconColor", T<string>
                    "attachments", !| Attachment
                    "actionTypeId", T<string>
                    "extra", T<obj>
                    "threadIdentifier", T<string>
                    "summaryArgument", T<string>
                    // Since 8.1.0 (iOS only).
                    "relevanceScore", T<float>
                    "interruptionLevel", InterruptionLevel.Type
                    "group", T<string>
                    "groupSummary", T<bool>
                    "channelId", T<string>
                    "ongoing", T<bool>
                    "autoCancel", T<bool>
                    "inboxList", !| T<string>
                    "silent", T<bool>
                    "badge", T<int>
                    "foreground", T<bool>
                    "isExactNotification", T<bool>
                    "isExactMandatory", T<bool>
                ]
            }

        let ScheduleOptions = 
            Pattern.Config "ScheduleOptions" {
                Required = [
                    ("notifications", !| LocalNotificationSchema)
                ]
                Optional = []
            }

        let PendingLocalNotificationSchema = 
            Pattern.Config "PendingLocalNotificationSchema" {
                Required = [
                    "title", T<string>
                    "body", T<string>
                    "id", T<int>
                ]
                Optional = [
                    "schedule", Schedule.Type
                    "extra", T<obj>
                ]
            }

        let PendingResult = 
            Pattern.Config "PendingResult" {
                Required = [
                    ("notifications", !| PendingLocalNotificationSchema)
                ]
                Optional = []
            }

        let Action = 
            Pattern.Config "Action" {
                Required = [
                    "id", T<string>
                    "title", T<string>
                ]
                Optional = [
                    "requiresAuthentication", T<bool>
                    "foreground", T<bool>
                    "destructive", T<bool>
                    "input", T<bool>
                    "inputButtonTitle", T<string>
                    "inputPlaceholder", T<string>
                ]
            }

        let ActionType = 
            Pattern.Config "ActionType" {
                Required = [
                    ("id", T<string>)
                ]
                Optional = [
                    "actions", !| Action
                    "iosHiddenPreviewsBodyPlaceholder", T<string>
                    "iosCustomDismissAction", T<bool>
                    "iosAllowInCarPlay", T<bool>
                    "iosHiddenPreviewsShowTitle", T<bool>
                    "iosHiddenPreviewsShowSubtitle", T<bool>
                ]
            }

        let RegisterActionTypesOptions = 
            Pattern.Config "RegisterActionTypesOptions" {
                Required = [
                    ("types", !| ActionType)
                ]
                Optional = []
            }

        let CancelOptions = 
            Pattern.Config "CancelOptions" {
                Required = [
                    ("notifications", !| LocalNotificationDescriptor)
                ]
                Optional = []
            }

        let RemoveByIdsOptions = 
            Pattern.Config "RemoveByIdsOptions" {
                Required = [
                    ("ids", !| T<int>)
                ]
                Optional = []
            }

        let GetByIdsOptions = 
            Pattern.Config "GetByIdsOptions" {
                Required = [
                    ("ids", !| T<int>)
                ]
                Optional = []
            }

        let GetAllOptions = 
            Pattern.Config "GetAllOptions" {
                Required = []
                Optional = [
                    ("state", NotificationState.Type)
                ]
            }

        let GetNotificationsResult = 
            Pattern.Config "GetNotificationsResult" {
                Required = [
                    ("notifications", !| LocalNotificationSchema)
                ]
                Optional = []
            }

        let EnabledResult = 
            Pattern.Config "EnabledResult" {
                Required = [
                    ("value", T<bool>)
                ]
                Optional = []
            }

        let DeliveredNotificationSchema = 
            Pattern.Config "DeliveredNotificationSchema" {
                Required = [
                    "id", T<int>
                    "title", T<string>
                    "body", T<string>
                ]
                Optional = [
                    "tag", T<string>
                    "group", T<string>
                    "groupSummary", T<bool>
                    "data", T<obj>
                    "extra", T<obj>
                    "attachments", !| Attachment
                    "actionTypeId", T<string>
                    "schedule", Schedule.Type
                    "sound", T<string>
                ]
            }

        let DeliveredNotifications = 
            Pattern.Config "DeliveredNotifications" {
                Required = [
                    ("notifications", !| DeliveredNotificationSchema)
                ]
                Optional = []
            }

        let Channel = 
            Pattern.Config "Channel" {
                Required = [
                    "id", T<string>
                    "name", T<string>
                ]
                Optional = [
                    "description", T<string>
                    "sound", T<string>
                    "importance", Importance.Type
                    "visibility", Visibility.Type
                    "lights", T<bool>
                    "lightColor", T<string>
                    "vibration", T<bool>
                ]
            }

        let ListChannelsResult = 
            Pattern.Config "ListChannelsResult" {
                Required = [
                    ("channels", !| Channel)
                ]
                Optional = []
            }

        let PermissionStatus = 
            Pattern.Config "PermissionStatus" {
                Required = [
                    ("display", PermissionState.Type)
                ]
                Optional = []
            } 

        let SettingsPermissionStatus = 
            Pattern.Config "SettingsPermissionStatus" {
                Required = [
                    ("exact_alarm", PermissionState.Type)
                ]
                Optional = []
            }

        let ActionPerformed = 
            Pattern.Config "ActionPerformed" {
                Required = [
                    "actionId", T<string>
                    "notification", LocalNotificationSchema.Type
                ]
                Optional = [
                    ("inputValue", T<string>)
                ]
            }

        let LocalNotificationsOptions =
            Pattern.Config "LocalNotificationsOptions" {
                Required = []
                Optional = [
                    "smallIcon", T<string>
                    "iconColor", T<string>
                    "sound", T<string>
                    // Since 8.2.0 (iOS only).
                    "presentationOptions", !| LocalNotificationPresentationOption
                ]
            }

        let PluginsConfig =
            Pattern.Config "PluginsConfig" {
                Required = []
                Optional = [
                    ("LocalNotifications", LocalNotificationsOptions.Type)
                ]
            }

        let DeleteChannelOptions = 
            Pattern.Config "DeleteChannelOptions" {
                Required = [ ("id", T<string>) ]
                Optional = []
            }
        
        let LocalNotificationsPlugin = 
            Class "LocalNotificationsPlugin"
            |+> Instance [
                "schedule" => ScheduleOptions?options ^-> T<Promise<_>>[ScheduleResult]
                "update" => ScheduleOptions?options ^-> T<Promise<_>>[ScheduleResult]
                |> WithComment "Update an already scheduled notification. Since 8.3.0."
                "getPending" => T<unit> ^-> T<Promise<_>>[PendingResult]
                "registerActionTypes" => RegisterActionTypesOptions?options ^-> T<Promise<unit>>
                "cancel" => CancelOptions?options ^-> T<Promise<unit>>
                "cancelAll" => T<unit> ^-> T<Promise<unit>>
                |> WithComment "Cancel all scheduled notifications. Since 8.3.0."
                "areEnabled" => T<unit> ^-> T<Promise<_>>[EnabledResult]
                "getDeliveredNotifications" => T<unit> ^-> T<Promise<_>>[DeliveredNotifications]
                "removeDeliveredNotifications" => DeliveredNotifications?delivered ^-> T<Promise<unit>>
                "removeDeliveredNotificationsById" => RemoveByIdsOptions?options ^-> T<Promise<unit>>
                |> WithComment "Remove delivered notifications by id. Since 8.3.0."
                "removeAllDeliveredNotifications" => T<unit> ^-> T<Promise<unit>>
                "getByIds" => GetByIdsOptions?options ^-> T<Promise<_>>[GetNotificationsResult]
                |> WithComment "Get notifications by id. Since 8.3.0."
                "getAll" => !?GetAllOptions?options ^-> T<Promise<_>>[GetNotificationsResult]
                |> WithComment "Get all scheduled and/or delivered notifications. Since 8.3.0."
                "createChannel" => Channel?channel ^-> T<Promise<unit>>
                "deleteChannel" => DeleteChannelOptions?args ^-> T<Promise<unit>>
                "listChannels" => T<unit> ^-> T<Promise<_>>[ListChannelsResult]
                "checkPermissions" => T<unit> ^-> T<Promise<_>>[PermissionStatus]
                "requestPermissions" => T<unit> ^-> T<Promise<_>>[PermissionStatus]
                "changeExactNotificationSetting" => T<unit> ^-> T<Promise<_>>[SettingsPermissionStatus]
                "checkExactNotificationSetting" => T<unit> ^-> T<Promise<_>>[SettingsPermissionStatus]
                "addListener" => T<string>?eventName * ListenFunctionType LocalNotificationSchema?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'localNotificationReceived' event."
                "addListener" => T<string>?eventName * ListenFunctionType ActionPerformed?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'localNotificationActionPerformed' event."
                "removeAllListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Motion = 
        let Acceleration = 
            Pattern.Config "Acceleration" {
                Required = [
                    "x", T<double>
                    "y", T<double>
                    "z", T<double>
                ]
                Optional = []
            }

        let RotationRate = 
            Pattern.Config "RotationRate" {
                Required = [
                    "alpha", T<double>
                    "beta", T<double>
                    "gamma", T<double>
                ]
                Optional = []
            }

        let AccelListenerEvent = 
            Pattern.Config "AccelListenerEvent" {
                Required = [
                    "acceleration", Acceleration.Type
                    "accelerationIncludingGravity", Acceleration.Type
                    "rotationRate", RotationRate.Type
                    "interval", T<int>
                ]
                Optional = []
            }

        let AccelListener = AccelListenerEvent ^-> T<unit>

        let OrientationListener = RotationRate ^-> T<unit>

        let OrientationListenerEvent = RotationRate

        let MotionPlugin = 
            Class "MotionPlugin"
            |+> Instance [
                "addListener" => T<string>?eventName * AccelListener?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'accel' event."
                "addListener" => T<string>?eventName * OrientationListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'orientation' event."
                "removeAllListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Network = 
        let ConnectionType =
            Pattern.EnumStrings "ConnectionType" ["wifi"; "cellular"; "none"; "unknown"]

        let ConnectionStatus =
            Pattern.Config "ConnectionStatus" {
                Required = [
                    "connected", T<bool>
                    "connectionType", ConnectionType.Type
                ]
                Optional = []
            }

        let ConnectionStatusChangeListener = ConnectionStatus ^-> T<unit>

        let NetworkPlugin =
            Class "NetworkPlugin"
            |+> Instance [
                "getStatus" => T<unit> ^-> T<Promise<_>>[ConnectionStatus]
                "addListener" => T<string>?eventName * ConnectionStatusChangeListener?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'networkStatusChange' event."
                "removeAllListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Preferences = 
        let ConfigureOptions =
            Pattern.Config "ConfigureOptions" {
                Required = []
                Optional = [("group", T<string>)]
            }

        let GetOptions =
            Pattern.Config "GetOptions" {
                Required = [("key", T<string>)]
                Optional = []
            }

        let GetResult =
            Pattern.Config "GetResult" {
                Required = [("value", T<string> + T<unit>)]
                Optional = []
            }

        let SetOptions =
            Pattern.Config "SetOptions" {
                Required = [
                    "key", T<string>
                    "value", T<string>
                ]
                Optional = []
            }

        let RemoveOptions =
            Pattern.Config "RemoveOptions" {
                Required = [
                    ("key", T<string>)
                ]
                Optional = []
            }

        let KeysResult =
            Pattern.Config "KeysResult" {
                Required = [
                    ("keys", !|T<string>) 
                ]
                Optional = []
            }

        let MigrateResult =
            Pattern.Config "MigrateResult" {
                Required = [
                    "migrated", !|T<string>  
                    "existing", !|T<string>  
                ]
                Optional = []
            }

        let PreferencesPlugin =
            Class "PreferencesPlugin"
            |+> Instance [
                "configure" => ConfigureOptions?options ^-> T<Promise<unit>>
                "get" => GetOptions?options ^-> T<Promise<_>>[GetResult]
                "set" => SetOptions?options ^-> T<Promise<unit>>
                "remove" => RemoveOptions?options ^-> T<Promise<unit>>
                "clear" => T<unit> ^-> T<Promise<unit>>
                "keys" => T<unit> ^-> T<Promise<_>>[KeysResult]
                "migrate" => T<unit> ^-> T<Promise<_>>[MigrateResult]
                "removeOld" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module PushNotifications =
        let PresentationOption =
            Pattern.EnumStrings "PresentationOption" [
                "badge"
                "sound"
                "alert"
                // Since 8.0.0.
                "banner"
                "list"
            ]

        let PluginsConfig = 
            Pattern.Config "PluginsConfig" {
                Required = [
                    ("presentationOptions", !| PresentationOption)
                ]
                Optional = []
            }

        let Importance =
            Pattern.EnumStrings "Importance" [
                "0"
                "1"
                "2"
                "3"
                "4"
                "5"
            ]

        let Visibility =
            Pattern.EnumStrings "Visibility" [
                "-1"
                "0"
                "1"
            ]

        let PermissionStatus =
            Pattern.Config "PermissionStatus" {
                Required = [
                    ("receive", PermissionState.Type)
                ]
                Optional = []
            }

        let Channel =
            Pattern.Config "Channel" {
                Required = [
                    "id", T<string>
                    "name", T<string>
                ]
                Optional = [
                    "description", T<string>
                    "sound", T<string>
                    "importance", Importance.Type
                    "visibility", Visibility.Type
                    "lights", T<bool>
                    "lightColor", T<string>
                    "vibration", T<bool>
                ]
            }

        let PushNotificationSchema =
            Pattern.Config "PushNotificationSchema" {
                Required = [
                    "id", T<string>
                    "data", T<obj>
                ]
                Optional = [
                    "title", T<string>
                    "subtitle", T<string>
                    "body", T<string>
                    "tag", T<string>
                    "badge", T<int>
                    "notification", T<obj>
                    "click_action", T<string>
                    "link", T<string>
                    "group", T<string>
                    "groupSummary", T<bool>
                ]
            }

        let ActionPerformed =
            Pattern.Config "ActionPerformed" {
                Required = [
                    "actionId", T<string>
                    "notification", PushNotificationSchema.Type
                ]
                Optional = [
                    ("inputValue", T<string>)
                ]
            }

        let Token =
            Pattern.Config "Token" {
                Required = [
                    ("value", T<string>)
                ]
                Optional = []
            }

        let RegistrationError =
            Pattern.Config "RegistrationError" {
                Required = [
                    ("error", T<string>)
                ]
                Optional = []
            }

        let DeliveredNotifications =
            Pattern.Config "DeliveredNotifications" {
                Required = [
                    ("notifications", !| PushNotificationSchema)
                ]
                Optional = []
            }

        let ListChannelsResult =
            Pattern.Config "ListChannelsResult" {
                Required = [
                    ("channels", !| Channel)
                ]
                Optional = []
            }

        let DeleteChannelArgs = 
            Pattern.Config "DeleteChannelArgs" {
                Required = [
                    ("id", T<string>)
                ]
                Optional = []
            }

        let PushNotificationsPlugin =
            Class "PushNotificationsPlugin"
            |+> Instance [
                "register" => T<unit> ^-> T<Promise<unit>>
                "unregister" => T<unit> ^-> T<Promise<unit>>
                "getDeliveredNotifications" => T<unit> ^-> T<Promise<_>>[DeliveredNotifications]
                "removeDeliveredNotifications" => DeliveredNotifications?delivered ^-> T<Promise<unit>>
                "removeAllDeliveredNotifications" => T<unit> ^-> T<Promise<unit>>
                "createChannel" => Channel?channel ^-> T<Promise<unit>>
                "deleteChannel" => DeleteChannelArgs?args ^-> T<Promise<unit>>
                "listChannels" => T<unit> ^-> T<Promise<_>>[ListChannelsResult]
                "checkPermissions" => T<unit> ^-> T<Promise<_>>[PermissionStatus]
                "requestPermissions" => T<unit> ^-> T<Promise<_>>[PermissionStatus]
                "addListener" => T<string>?eventName * ListenFunctionType Token?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'registration' event."
                "addListener" => T<string>?eventName * ListenFunctionType RegistrationError?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'registrationError' event."
                "addListener" => T<string>?eventName * ListenFunctionType PushNotificationSchema?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'pushNotificationReceived' event."
                "addListener" => T<string>?eventName * ListenFunctionType ActionPerformed?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'pushNotificationActionPerformed' event."
                "removeAllListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module ScreenOrientation =
        let OrientationLockType =
            Pattern.EnumStrings "OrientationLockType" [
                "any"
                "natural"
                "landscape"
                "portrait"
                "portrait-primary"
                "portrait-secondary"
                "landscape-primary"
                "landscape-secondary"
            ]

        let OrientationLockOptions =
            Pattern.Config "OrientationLockOptions" {
                Required = [
                    ("orientation", OrientationLockType.Type)
                ]
                Optional = []
            }

        let ScreenOrientationResult =
            Pattern.Config "ScreenOrientationResult" {
                Required = [
                    ("type", OrientationLockType.Type)
                ]
                Optional = []
            }

        let ScreenOrientationPlugin =
            Class "ScreenOrientationPlugin"
            |+> Instance [
                "orientation" => T<unit> ^-> T<Promise<_>>[ScreenOrientationResult]
                "lock" => OrientationLockOptions?options ^-> T<Promise<unit>>
                "unlock" => T<unit> ^-> T<Promise<unit>>
                "addListener" => T<string>?eventName * ListenFunctionType ScreenOrientationResult?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'screenOrientationChange' event."
                "removeAllListeners" => T<unit> ^-> T<Promise<unit>>
            ] 

    [<AutoOpen>]
    module ScreenReader =
        let SpeakOptions =
            Pattern.Config "SpeakOptions" {
                Required = [
                    ("value", T<string>)
                ]
                Optional = [
                    ("language", T<string>)
                ]
            }

        let ScreenReaderState =
            Pattern.Config "ScreenReaderState" {
                Required = [
                    ("value", T<bool>)
                ]
                Optional = []
            }

        let IsEnabledResult = 
            Pattern.Config "IsEnabledResult" {
                Required = [
                    ("value", T<bool>)
                ]
                Optional = []
            }

        let StateChangeListener = ScreenReaderState ^-> T<unit>

        let ScreenReaderPlugin =
            Class "ScreenReaderPlugin"
            |+> Instance [
                "isEnabled" => T<unit> ^-> T<Promise<_>>[IsEnabledResult]
                "speak" => SpeakOptions?options ^-> T<Promise<unit>>
                "addListener" => T<string>?eventName * StateChangeListener?listener ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'stateChange' event."
                "removeAllListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Share =
        let ShareOptions =
            Pattern.Config "ShareOptions" {
                Required = []
                Optional = [
                    "title", T<string>
                    "text", T<string>
                    "url", T<string>
                    "files", !| T<string>
                    "dialogTitle", T<string>
                ]
            }

        let ShareResult =
            Pattern.Config "ShareResult" {
                Required = []
                Optional = [
                    ("activityType", T<string>)
                ]
            }

        let CanShareResult =
            Pattern.Config "CanShareResult" {
                Required = [
                    ("value", T<bool>)
                ]
                Optional = []
            }

        let SharePlugin =
            Class "SharePlugin"
            |+> Instance [
                "canShare" => T<unit> ^-> T<Promise<_>>[CanShareResult]
                "share" => ShareOptions?options ^-> T<Promise<_>>[ShareResult]
            ]

    [<AutoOpen>]
    module SplashScreen = 
        let ShowOptions =
            Pattern.Config "ShowOptions" {
                Required = []
                Optional = [
                    "autoHide", T<bool>
                    "fadeInDuration", T<int>
                    "fadeOutDuration", T<int>
                    "showDuration", T<int>
                ]
            }

        let HideOptions =
            Pattern.Config "HideOptions" {
                Required = []
                Optional = [
                    ("fadeOutDuration", T<int>)
                ]
            }

        let SplashScreenOptions =
            Pattern.Config "SplashScreenOptions" {
                Required = []
                Optional = [
                    "launchShowDuration", T<int>
                    "launchAutoHide", T<bool>
                    "launchFadeOutDuration", T<int>
                    "backgroundColor", T<string>
                    "androidSplashResourceName", T<string>
                    "androidScaleType", T<string> 
                    "showSpinner", T<bool>
                    "androidSpinnerStyle", T<string> 
                    "iosSpinnerStyle", T<string> 
                    "spinnerColor", T<string>
                    "splashFullScreen", T<bool>
                    "splashImmersive", T<bool>
                    "layoutName", T<string>
                    "useDialog", T<bool>
                ]
            }

        let PluginsConfig =
            Pattern.Config "PluginsConfig" {
                Required = []
                Optional = [
                    ("SplashScreen", SplashScreenOptions.Type)
                ]
            }

        let SplashScreenPlugin =
            Class "SplashScreenPlugin"
            |+> Instance [
                "show" => !? ShowOptions?options ^-> T<Promise<unit>>
                "hide" => !? HideOptions?options ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module StatusBar = 
        let Style = 
            Pattern.EnumStrings "Style" ["DARK"; "LIGHT"; "DEFAULT"]

        let Animation = 
            Pattern.EnumStrings "Animation" ["NONE"; "SLIDE"; "FADE"]

        let StyleOptions =
            Pattern.Config "StyleOptions" {
                Required = [("style", Style.Type)]
                Optional = []
            }

        let AnimationOptions =
            Pattern.Config "AnimationOptions" {
                Required = []
                Optional = [("animation", Animation.Type)]
            }

        let BackgroundColorOptions =
            Pattern.Config "BackgroundColorOptions" {
                Required = [("color", T<string>)]
                Optional = []
            }

        let StatusBarInfo =
            Pattern.Config "StatusBarInfo" {
                Required = [
                    "visible", T<bool>
                    "style", Style.Type
                    "color", T<string>
                    "overlays", T<bool>
                    "height", T<int>
                ]
                Optional = []
            }

        let StatusBarOptions =
            Pattern.Config "StatusBarOptions" {
                Required = []
                Optional = [
                    "overlaysWebView", T<bool>
                    "style", Style.Type
                    "backgroundColor", T<string>
                ]
            }

        let PluginsConfig =
            Pattern.Config "PluginsConfig" {
                Required = []
                Optional = [
                    ("StatusBar", StatusBarOptions.Type)
                ]
            }

        let SetOverlaysWebViewOptions =
            Pattern.Config "SetOverlaysWebViewOptions" {
                Required = [("overlay", T<bool>)]
                Optional = []
            }

        let StatusBarPlugin =
            Class "StatusBarPlugin"
            |+> Instance [
                "setStyle" => StyleOptions?options ^-> T<Promise<unit>>
                "setBackgroundColor" => BackgroundColorOptions?options ^-> T<Promise<unit>>
                "show" => !?AnimationOptions?options ^-> T<Promise<unit>>
                "hide" => !?AnimationOptions?options ^-> T<Promise<unit>>
                "getInfo" => T<unit> ^-> T<Promise<_>>[StatusBarInfo]
                "setOverlaysWebView" => SetOverlaysWebViewOptions?options ^-> T<Promise<unit>>
                "addListener" => T<string>?eventName * ListenFunctionType StatusBarInfo?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'statusBarVisibilityChanged' and 'statusBarOverlayChanged' events."
            ]

    [<AutoOpen>]
    module TextZoom = 
        let GetResult = 
            Pattern.Config "GetResult" {
                Required = [
                    ("value", T<float>)
                ]
                Optional = []
            }

        let GetPreferredResult = 
            Pattern.Config "GetPreferredResult" {
                Required = [
                    ("value", T<float>)
                ]
                Optional = []
            }

        let SetOptions = 
            Pattern.Config "SetOptions" {
                Required = [
                    ("value", T<float>)
                ]
                Optional = []
            }

        let TextZoomPlugin =
            Class "TextZoomPlugin"
            |+> Instance [
                "get" => T<unit> ^-> T<Promise<_>>[GetResult]
                "getPreferred" => T<unit> ^-> T<Promise<_>>[GetPreferredResult]
                "set" => SetOptions?options ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Toast = 
        let ToastDuration =
            Pattern.EnumStrings "ToastDuration" [
                "short"
                "long"
            ]

        let ToastPosition =
            Pattern.EnumStrings "ToastPosition" [
                "top"
                "center"
                "bottom"
            ]

        let ShowOptions = 
            Pattern.Config "ShowOptions" {
                Required = [
                    ("text", T<string>)
                ]
                Optional = [
                    "duration", ToastDuration.Type
                    "position", ToastPosition.Type
                ]
            }

        let ToastPlugin =
            Class "ToastPlugin"
            |+> Instance [
                "show" => ShowOptions?options ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Watch = 
        let CommandData = 
            Pattern.Config "CommandData" {
                Required = [
                    ("command", T<string>)
                ]
                Optional = []
            }

        let WatchUIOptions = 
            Pattern.Config "WatchUIOptions" {
                Required = [
                    ("watchUI", T<string>)
                ]
                Optional = []
            }

        let WatchDataOptions = 
            Pattern.Config "WatchDataOptions" {
                Required = [
                    ("data", T<obj>)
                ]
                Optional = []
            }

        let WatchPlugin =
            Class "WatchPlugin"
            |+> Instance [
                "addListener" => T<string>?eventName * ListenFunctionType CommandData?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'runCommand' event."
                "updateWatchUI" => WatchUIOptions?options ^-> T<Promise<unit>>
                "updateWatchData" => WatchDataOptions?options ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module BiometricAuth = 
        let BiometryType =
            Pattern.EnumInlines "BiometryType" [
                "none", "0"
                "touchId", "1"
                "faceId", "2"
                "fingerprintAuthentication", "3"
                "faceAuthentication", "4"
                "irisAuthentication", "5"
            ]

        let AndroidBiometryStrength =
            Pattern.EnumInlines "AndroidBiometryStrength" [
                "weak", "0"
                "strong", "1"
            ]

        let BiometryErrorType =
            Pattern.EnumInlines "BiometryErrorType" [
                "none", "\"\""
                "appCancel", "\"appCancel\""
                "authenticationFailed", "\"authenticationFailed\""
                "invalidContext", "\"invalidContext\""
                "notInteractive", "\"notInteractive\""
                "passcodeNotSet", "\"passcodeNotSet\""
                "systemCancel", "\"systemCancel\""
                "userCancel", "\"userCancel\""
                "userFallback", "\"userFallback\""
                "biometryLockout", "\"biometryLockout\""
                "biometryNotAvailable", "\"biometryNotAvailable\""
                "biometryNotEnrolled", "\"biometryNotEnrolled\""
                "noDeviceCredential", "\"noDeviceCredential\""
            ]

        let AuthenticateOptions =
            Pattern.Config "AuthenticateOptions" {
                Required = []
                Optional = [
                    "reason", T<string>
                    "cancelTitle", T<string>
                    "allowDeviceCredential", T<bool>
                    "iosFallbackTitle", T<string>
                    "androidTitle", T<string>
                    "androidSubtitle", T<string>
                    "androidConfirmationRequired", T<bool>
                    "androidBiometryStrength", AndroidBiometryStrength.Type
                ]
            }

        let CheckBiometryResult =
            Pattern.Config "CheckBiometryResult" {
                Required = [
                    "isAvailable", T<bool>
                    "strongBiometryIsAvailable", T<bool>
                    "biometryType", BiometryType.Type
                    "biometryTypes", !| BiometryType.Type
                    "deviceIsSecure", T<bool>
                    "reason", T<string>
                    "code", BiometryErrorType.Type
                ]
                Optional = [
                    "strongReason", T<string>
                    "strongCode", BiometryErrorType.Type
                ]
            }

        let ResumeListener = CheckBiometryResult?info ^-> T<unit>

        let SetBiometryType = BiometryType + T<string> + !|BiometryType

        let BiometryError = 
            Pattern.Config "BiometryError" {
                Required = []
                Optional = [
                    "message", T<string>
                    "code", BiometryErrorType.Type
                ]
            }

        let BiometricAuthPlugin =
            Class "BiometricAuthPlugin"
            |+> Instance [
                "checkBiometry" => T<unit> ^-> T<Promise<_>>[CheckBiometryResult]
                "setBiometryType" => SetBiometryType?``type`` ^-> T<Promise<unit>>
                "setBiometryIsEnrolled" => T<bool>?isSecure ^-> T<Promise<unit>>
                "setDeviceIsSecure" => T<bool>?isSecure ^-> T<Promise<unit>>
                "authenticate" => !?AuthenticateOptions?options ^-> T<Promise<unit>>
                "addResumeListener" => ResumeListener?listener ^-> T<Promise<_>>[PluginListenerHandle]
            ]


    let Capacitor =
        Class "Capacitor"
        |+> Static [
            "BiometricAuth" =? BiometricAuthPlugin
            |> Import "BiometricAuth" "@aparajita/capacitor-biometric-auth"
            "ActionSheet" =? ActionSheetPlugin
            |> Import "ActionSheet" "@capacitor/action-sheet"
            "AppLauncher" =? AppLauncherPlugin
            |> Import "AppLauncher" "@capacitor/app-launcher"
            "App" =? AppPlugin
            |> Import "App" "@capacitor/app"
            "BackgroundRunner" =? BackgroundRunnerPlugin
            |> Import "BackgroundRunner" "@capacitor/background-runner"
            "BarcodeScanner" =? BarcodeScannerPlugin
            |> Import "CapacitorBarcodeScanner" "@capacitor/barcode-scanner"
            "Browser" =? BrowserPlugin
            |> Import "Browser" "@capacitor/browser"
            "Camera" =? CameraPlugin
            |> Import "Camera" "@capacitor/camera"
            "Clipboard" =? ClipboardPlugin
            |> Import "Clipboard" "@capacitor/clipboard"
            "Cookies" =? CookiesPlugin
            |> Import "CapacitorCookies" "@capacitor/core"
            "Device" =? DevicePlugin
            |> Import "Device" "@capacitor/device"
            "Dialog" =? DialogPlugin
            |> Import "Dialog" "@capacitor/dialog"
            "Filesystem" =? FilesystemPlugin
            |> Import "Filesystem" "@capacitor/filesystem"
            "Geolocation" =? GeolocationPlugin
            |> Import "Geolocation" "@capacitor/geolocation"
            "GoogleMaps" =? GoogleMapsPlugin
            |> Import "GoogleMaps" "@capacitor/google-maps"
            "Haptics" =? HapticsPlugin
            |> Import "Haptics" "@capacitor/haptics"
            "Http" =? HttpPlugin
            |> Import "CapacitorHttp" "@capacitor/core"
            "InAppBrowser" =? InAppBrowserPlugin
            |> Import "InAppBrowser" "@capacitor/inappbrowser"
            "Keyboard" =? KeyboardPlugin
            |> Import "Keyboard" "@capacitor/keyboard"
            "LocalNotifications" =? LocalNotificationsPlugin
            |> Import "LocalNotifications" "@capacitor/local-notifications"
            "Motion" =? MotionPlugin
            |> Import "Motion" "@capacitor/motion"
            "Network" =? NetworkPlugin
            |> Import "Network" "@capacitor/network"
            "Preferences" =? PreferencesPlugin
            |> Import "Preferences" "@capacitor/preferences"
            "PushNotifications" =? PushNotificationsPlugin
            |> Import "PushNotifications" "@capacitor/push-notifications"
            "ScreenOrientation" =? ScreenOrientationPlugin
            |> Import "ScreenOrientation" "@capacitor/screen-orientation"
            "ScreenReader" =? ScreenReaderPlugin
            |> Import "ScreenReader" "@capacitor/screen-reader"
            "Share" =? SharePlugin
            |> Import "Share" "@capacitor/share"
            "SplashScreen" =? SplashScreenPlugin
            |> Import "SplashScreen" "@capacitor/splash-screen"
            "StatusBar" =? StatusBarPlugin
            |> Import "StatusBar" "@capacitor/status-bar"
            "TextZoom" =? TextZoomPlugin
            |> Import "TextZoom" "@capacitor/text-zoom"
            "Toast" =? ToastPlugin
            |> Import "Toast" "@capacitor/toast"
            "Watch" =? WatchPlugin
            |> Import "Watch" "@capacitor/watch"
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.Capacitor" [
                Capacitor
                PermissionState
                PluginListenerHandle
                PresentationStyle
                CapacitorCore

                BiometricAuthPlugin
                ActionSheetPlugin
                AppLauncherPlugin
                AppPlugin
                BackgroundRunnerPlugin
                BarcodeScannerPlugin
                BrowserPlugin
                CameraPlugin
                ClipboardPlugin
                CookiesPlugin
                DevicePlugin
                DialogPlugin
                FilesystemPlugin
                GeolocationPlugin
                GoogleMapsPlugin
                HapticsPlugin
                HttpPlugin
                InAppBrowserPlugin
                KeyboardPlugin
                LocalNotificationsPlugin
                MotionPlugin
                NetworkPlugin
                PreferencesPlugin
                PushNotificationsPlugin
                ScreenOrientationPlugin
                ScreenReaderPlugin
                SharePlugin
                SplashScreenPlugin
                StatusBarPlugin
                TextZoomPlugin
                ToastPlugin
                WatchPlugin
            ]
            Namespace "WebSharper.Capacitor.BiometricAuth" [
                CheckBiometryResult; BiometryType; AuthenticateOptions
                AndroidBiometryStrength; BiometryErrorType; BiometryError
            ]
            Namespace "WebSharper.Capacitor.ActionSheet" [
                ShowActionsOptions; ShowActionsResult; ActionSheetButton; ActionSheetButtonStyle
            ]
            Namespace "WebSharper.Capacitor.AppLauncher" [
                OpenURLOptions; OpenURLResult; CanOpenURLOptions; CanOpenURLResult
            ]
            Namespace "WebSharper.Capacitor.App" [
                BackButtonListenerEvent; RestoredListenerEvent; RestoredListenerError; URLOpenListenerEvent; AppLaunchUrl; AppState; AppInfo
                AppLanguageCode; ToggleBackButtonHandlerOptions; AppOptions; App.PluginsConfig
            ]
            Namespace "WebSharper.Capacitor.BackgroundRunner" [
                BackgroundRunnerOptions; BackgroundRunner.PluginsConfig; BackgroundRunner.PermissionStatus; 
                DispatchEventOptions; RequestPermissionOptions; API; NotificationActionEvent
            ]
            Namespace "WebSharper.Capacitor.BarcodeScanner" [
                CapacitorBarcodeScannerOptions; WebOptions; AndroidScanningLibrary; CapacitorBarcodeScannerTypeHint
                CapacitorBarcodeScannerScanResult; CapacitorBarcodeScannerAndroidScanningLibrary
                CapacitorBarcodeScannerScanOrientation; CapacitorBarcodeScannerCameraDirection
            ]
            Namespace "WebSharper.Capacitor.Browser" [OpenOptions]
            Namespace "WebSharper.Capacitor.Camera" [
                ImageOptions; Photo; GalleryImageOptions; GalleryPhotos; Camera.PermissionStatus; CameraPluginPermissions
                CameraPermissionState; CameraPermissionType; CameraResultType; CameraSource; CameraDirection; GalleryPhoto
                MediaType; MediaTypeSelection; EncodingType; Editable; CameraErrorCode; TakePhotoOptions; RecordVideoOptions
                PlayVideoOptions; ChooseFromGalleryOptions; EditPhotoOptions; EditPhotoResult; EditURIPhotoOptions
                MediaMetadata; MediaResult; MediaResults
            ]
            Namespace "WebSharper.Capacitor.Clipboard" [
                ReadResult; WriteOptions
            ]
            Namespace "WebSharper.Capacitor.Cookies" [
                GetCookieOptions; SetCookieOptions; HttpCookieMap; DeleteCookieOptions; ClearCookieOptions
            ]
            Namespace "WebSharper.Capacitor.Device" [
                LanguageTag; GetLanguageCodeResult; BatteryInfo; DeviceInfo; DevicePlatform; DeviceId; OperatingSystem
            ]
            Namespace "WebSharper.Capacitor.Dialog" [
                ConfirmOptions; ConfirmResult; PromptOptions; PromptResult; AlertOptions
            ]
            Namespace "WebSharper.Capacitor.Filesystem" [
                ReaddirResult; DownloadFileOptions; DownloadFileResult; Filesystem.PermissionStatus; CopyResult
                StatOptions; StatResult; GetUriOptions; GetUriResult; ReaddirOptions; FileInfo
                FileType; RmdirOptions; MkdirOptions; DeleteFileOptions; AppendFileOptions
                WriteFileOptions; WriteFileResult; ReadFileOptions; Directory
                ProgressStatus; CopyOptions; Encoding; ReadFileResult
                ReadFileInChunksOptions
            ]
            Namespace "WebSharper.Capacitor.Geolocation" [
                GeolocationPluginPermissions; Geolocation.PermissionStatus; ClearWatchOptions; PositionOptions
                GeolocationPermissionType; Position; Coordinates
            ]
            Namespace "WebSharper.Capacitor.GoogleMaps" [
                CreateMapArgs; GoogleMapConfig; MyLocationButtonClickCallbackData; CircleClickCallbackData; PolygonClickCallbackData
                MarkerClickCallbackData; MapClickCallbackData; PolylineCallbackData; ClusterClickCallbackData; MarkerCallbackData
                CameraMoveStartedCallbackData; CameraIdleCallbackData; LatLngBounds; LatLngBoundsInterface; MapPadding; MapType
                CameraConfig; Polyline; StyleSpan; Circle; Polygon; Marker; Point; Size; MapReadyCallbackData; LatLng; MapListenerCallback
                TileOverlay
            ]
            Namespace "WebSharper.Capacitor.Haptics" [
                VibrateOptions; NotificationOptions; ImpactOptions; NotificationType; ImpactStyle 
            ]
            Namespace "WebSharper.Capacitor.Http" [
                HttpOptions; HttpResponse; HttpHeaders; HttpParams; HttpResponseType; HttpDataType
            ]
            Namespace "WebSharper.Capacitor.InAppBrowser" [
                OpenInWebViewParameterModel; WebViewOptions; OpenInSystemBrowserParameterModel
                SystemBrowserOptions; OpenInDefaultParameterModel; iOSSystemBrowserOptions
                AndroidSystemBrowserOptions; AndroidBottomSheet; iOSWebViewOptions
                AndroidWebViewOptions; DismissStyle; AndroidAnimation; AndroidViewStyle
                iOSAnimation; ToolbarPosition; iOSViewStyle; CustomHeaders; BrowserPageNavigationCompletedEventData
            ]
            Namespace "WebSharper.Capacitor.Keyboard" [
                Keyboard.PluginsConfig; KeyboardOptions; AccessoryBarOptions; ScrollOptions; KeyboardResize
                KeyboardInfo; KeyboardResizeOptions; KeyboardStyleOptions; KeyboardStyle; KeyboardBackdropColor
            ]
            Namespace "WebSharper.Capacitor.LocalNotifications" [
                DeleteChannelOptions; LocalNotifications.PluginsConfig; LocalNotificationsOptions; LocalNotifications.ActionPerformed; SettingsPermissionStatus
                LocalNotifications.PermissionStatus; LocalNotifications.ListChannelsResult; LocalNotifications.Channel; 
                LocalNotifications.DeliveredNotifications; DeliveredNotificationSchema; LocalNotifications.Visibility
                EnabledResult; CancelOptions; RegisterActionTypesOptions; ActionType; Action; PendingResult; LocalNotifications.Importance
                PendingLocalNotificationSchema; ScheduleOptions; LocalNotificationSchema; Attachment; AttachmentOptions
                Schedule; ScheduleOn; ScheduleResult; LocalNotificationDescriptor; Weekday; ScheduleEvery
                InterruptionLevel; LocalNotificationPresentationOption; NotificationState; ScheduleWarning
                RemoveByIdsOptions; GetByIdsOptions; GetAllOptions; GetNotificationsResult
            ]
            Namespace "WebSharper.Capacitor.Motion" [
                OrientationListenerEvent; AccelListenerEvent; Acceleration
            ]
            Namespace "WebSharper.Capacitor.Network" [
                ConnectionType; ConnectionStatus
            ]
            Namespace "WebSharper.Capacitor.Preferences" [
                ConfigureOptions; GetOptions; Preferences.GetResult; Preferences.SetOptions; RemoveOptions; KeysResult; MigrateResult
            ]
            Namespace "WebSharper.Capacitor.PushNotifications" [
                PresentationOption; Importance; Visibility; PushNotifications.PermissionStatus; Channel; PushNotificationSchema; DeleteChannelArgs
                ActionPerformed; Token; RegistrationError; DeliveredNotifications; ListChannelsResult;PushNotifications.PluginsConfig
            ]
            Namespace "WebSharper.Capacitor.ScreenOrientation" [
                OrientationLockType; OrientationLockOptions; ScreenOrientationResult
            ]
            Namespace "WebSharper.Capacitor.ScreenReader" [
                SpeakOptions; ScreenReaderState; IsEnabledResult
            ]
            Namespace "WebSharper.Capacitor.Share" [
                ShareOptions;ShareResult;CanShareResult
            ]
            Namespace "WebSharper.Capacitor.SplashScreen" [
                SplashScreen.ShowOptions; HideOptions; SplashScreenOptions; SplashScreen.PluginsConfig
            ]
            Namespace "WebSharper.Capacitor.StatusBar" [
                SetOverlaysWebViewOptions; StatusBarInfo; BackgroundColorOptions
                AnimationOptions; StyleOptions; Animation; Style; StatusBarOptions; StatusBar.PluginsConfig
            ]
            Namespace "WebSharper.Capacitor.TextZoom" [
                GetResult; GetPreferredResult; SetOptions
            ]
            Namespace "WebSharper.Capacitor.Toast" [
                Toast.ShowOptions; ToastDuration; ToastPosition
            ]
            Namespace "WebSharper.Capacitor.Watch" [
                CommandData; WatchUIOptions; WatchDataOptions
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()

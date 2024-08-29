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

    [<AutoOpen>]
    module ActionSheet = 
        let ActionSheetButtonStyle =
            Pattern.EnumStrings "ActionSheetButtonStyle" [
                "Default"
                "Destructive"
                "Cancel"
            ]

        let ActionSheetButton =
            Pattern.Config "ActionSheetButton" {
                Required = ["title", T<string>]
                Optional = [
                    "style", ActionSheetButtonStyle.Type
                    "icon", T<string>
                ]
            }            

        let ShowActionsOptions =
            Pattern.Config "ShowActionsOptions" {
                Required = ["options", !| ActionSheetButton]
                Optional = [
                    "title", T<string>
                    "message", T<string>
                ]
            }

        let ShowActionsResult = 
            Pattern.Config "ShowActionsResult" {
                Required = ["index", T<int>]
                Optional = []
            }

        let ActionSheetPlugin = 
            Class "ActionSheetPlugin " 
            |=> Nested [ShowActionsOptions; ShowActionsResult; ActionSheetButton; ActionSheetButtonStyle]
            |+> Instance [
                "showActions" => ShowActionsOptions?options ^-> T<Promise<_>>[ShowActionsResult]
            ]

    [<AutoOpen>]
    module AppLauncher =
        let CanOpenURLResult = 
            Pattern.Config "CanOpenURLResult" {
                Required = ["value", T<bool>]
                Optional = []
            }
        
        let CanOpenURLOptions = 
            Pattern.Config "CanOpenURLOptions" {
                Required = ["url", T<string>]
                Optional = []
            }

        let OpenURLResult =
            Pattern.Config "OpenURLResult" {
                Required = ["completed", T<bool>]
                Optional = []
            }

        let OpenURLOptions = 
            Pattern.Config "OpenURLOptions" {
                Required = ["url", T<string>]
                Optional = []
            }

        let AppLauncherPlugin  = 
            Class "AppLauncherPlugin " 
            |=> Nested [OpenURLOptions; OpenURLResult; CanOpenURLOptions; CanOpenURLResult]
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
                Required = ["isActive", T<bool>]
                Optional = []
            }

        let AppLaunchUrl =
            Pattern.Config "AppLaunchUrl" {
                Required = ["url", T<string>]
                Optional = []
            }

        let URLOpenListenerEvent = 
            Pattern.Config "URLOpenListenerEvent" {
                Required = ["url", T<string>]
                Optional = [
                    "iosSourceApplication", T<obj>
                    "iosOpenInPlace", T<bool>
                ]
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
                    "error", T<string>
                ]
            }

        let BackButtonListenerEvent = 
            Pattern.Config "BackButtonListenerEvent" {
                Required = ["canGoBack", T<bool>]
                Optional = []
            }

        let StateChangeListener = AppState ^-> T<unit>

        let URLOpenListener = URLOpenListenerEvent ^-> T<unit>

        let RestoredListener = RestoredListenerEvent ^-> T<unit>

        let BackButtonListener = BackButtonListenerEvent ^-> T<unit>

        let AppPlugin = 
            Class "AppPlugin" 
            |=> Nested [
                BackButtonListenerEvent; RestoredListenerEvent; URLOpenListenerEvent
                AppLaunchUrl; AppState; AppInfo
            ]
            |+> Instance [
                "exitApp" => T<unit> ^-> T<Promise<unit>>
                "getInfo" => T<unit> ^-> T<Promise<_>>[AppInfo]
                "getState" => T<unit> ^-> T<Promise<_>>[AppState]
                "getLaunchUrl" => T<unit> ^-> T<Promise<_>>[T<obj>]
                "minimizeApp" => T<unit> ^-> T<Promise<unit>>
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

        let CapacitorBarcodeScannerScanResult = 
            Pattern.Config "CapacitorBarcodeScannerScanResult" {
                Required = []
                Optional = [
                    "ScanResult", T<string>
                ]
            }

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

        let AndroidScanningLibrary = 
            Pattern.Config "AndroidScanningLibrary" {
                Required = []
                Optional = ["scanningLibrary", CapacitorBarcodeScannerAndroidScanningLibrary.Type]
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
                Required = []
                Optional = [
                    "hint", CapacitorBarcodeScannerTypeHint.Type
                    "scanInstructions", T<string>
                    "scanButton", T<bool>
                    "scanText", T<string>
                    "cameraDirection", CapacitorBarcodeScannerCameraDirection.Type
                    "scanOrientation", CapacitorBarcodeScannerScanOrientation.Type
                    "android", AndroidScanningLibrary.Type
                    "web", WebOptions.Type
                ]
            }

        let BarcodeScannerPlugin = 
            Class "BarcodeScannerPlugin" 
            |=> Nested [
                CapacitorBarcodeScannerOptions; WebOptions; AndroidScanningLibrary; CapacitorBarcodeScannerTypeHint
                CapacitorBarcodeScannerScanResult; CapacitorBarcodeScannerAndroidScanningLibrary
                CapacitorBarcodeScannerScanOrientation; CapacitorBarcodeScannerCameraDirection
            ]
            |+> Instance [
                "scanBarcode" => CapacitorBarcodeScannerOptions?options ^-> T<Promise<_>>[CapacitorBarcodeScannerScanResult]
            ]

    [<AutoOpen>]
    module Browser = 
        let OpenOptions = 
            Pattern.Config "OpenOptions" {
                Required = []
                Optional = [
                    "url", T<string>
                    "windowName", T<string>
                    "toolbarColor", T<string>
                    "presentationStyle", PresentationStyle.Type
                    "width", T<int>
                    "height", T<int>
                ]
            }

        let BrowserPlugin = 
            Class "BrowserPlugin" 
            |=> Nested [OpenOptions]
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
                "Prompt"
                "Camera"
                "Photos"
            ]

        let CameraDirection = 
            Pattern.EnumStrings "CameraDirection" [
                "Rear"
                "Front"
            ]
    
        let ImageOptions = 
            Pattern.Config "ImageOptions" {
                Required = [
                    "resultType", CameraResultType.Type
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
                    "photos", !|GalleryPhoto
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
                Required = []
                Optional = [
                    "camera", CameraPermissionState.Type
                    "photos", CameraPermissionState.Type
                ]
            }

        let CameraPluginPermissions = 
            Pattern.Config "CameraPluginPermissions" {
                Required = []
                Optional = ["permissions", !| CameraPermissionType]
            }             

        let CameraPlugin = 
            Class "CameraPlugin" 
            |=> Nested [
                ImageOptions; Photo; GalleryImageOptions; GalleryPhotos; PermissionStatus; CameraPluginPermissions
                CameraPermissionState; CameraPermissionType; CameraResultType; CameraSource; CameraDirection; GalleryPhoto
            ]
            |+> Instance [
                "getPhoto" => ImageOptions?options ^-> T<Promise<_>>[Photo]
                "pickImages" => GalleryImageOptions?options ^-> T<Promise<_>>[GalleryPhotos]
                "pickLimitedLibraryPhotos" => T<unit> ^-> T<Promise<_>>[GalleryPhotos]
                "getLimitedLibraryPhotos" => T<unit> ^-> T<Promise<_>>[GalleryPhotos]
                "checkPermissions" => T<unit> ^-> T<Promise<_>>[PermissionStatus]
                "requestPermissions" => !?CameraPluginPermissions?permissions ^-> T<Promise<_>>[PermissionStatus]
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
                ]
            }

        let ClearWatchOptions = 
            Pattern.Config "ClearWatchOptions" {
                Required = ["id", CallbackID]
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
                Required = ["permissions", !| GeolocationPermissionType]
                Optional = []
            }

        let GeolocationPlugin = 
            Class "GeolocationPlugin" 
            |=> Nested [
                GeolocationPluginPermissions; PermissionStatus; ClearWatchOptions; PositionOptions
                GeolocationPermissionType; Position; Coordinates
            ]
            |+> Instance [
                "getCurrentPosition" => !?PositionOptions?options ^-> T<Promise<_>>[Position]
                "watchPosition" => PositionOptions?options * WatchPositionCallback?callback ^-> T<Promise<_>>[CallbackID]
                "clearWatch" => ClearWatchOptions?options ^-> T<Promise<unit>>
                "checkPermissions" => T<unit> ^-> T<Promise<_>>[PermissionStatus]
                "requestPermissions" => !?GeolocationPluginPermissions?permissions ^-> T<Promise<_>>[PermissionStatus]
            ]

    [<AutoOpen>]
    module Motion = 
        let Acceleration = 
            Pattern.Config "Acceleration" {
                Required = []
                Optional = [
                    "x", T<double>
                    "y", T<double>
                    "z", T<double>
                ]
            }

        let RotationRate = 
            Pattern.Config "RotationRate" {
                Required = []
                Optional = [
                    "alpha", T<double>
                    "beta", T<double>
                    "gamma", T<double>
                ]
            }

        let AccelListenerEvent = 
            Pattern.Config "AccelListenerEvent" {
                Required = []
                Optional = [
                    "acceleration", Acceleration.Type
                    "accelerationIncludingGravity", Acceleration.Type
                    "rotationRate", RotationRate.Type
                    "interval", T<int>
                ]
            }

        let AccelListener = AccelListenerEvent ^-> T<unit>

        let OrientationListener = RotationRate ^-> T<unit>

        let OrientationListenerEvent = RotationRate

        let MotionPlugin = 
            Class "MotionPlugin"
            |=> Nested [
                OrientationListenerEvent; AccelListenerEvent; Acceleration
            ]
            |+> Instance [
                "addListener" => T<string>?eventName * AccelListener?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'accel' event."
                "addListener" => T<string>?evenName * OrientationListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'orientation' event."
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
            |=> Nested [ReadResult; WriteOptions]
            |+> Instance [
                "write" => WriteOptions?options ^-> T<Promise<unit>>
                "read" => T<unit> ^-> T<Promise<_>>[ReadResult]
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
                Required = []
                Optional = [
                    "identifier", T<string>
                ]
            }

        let DevicePlatform = 
            Pattern.EnumStrings "DevicePlatform" [
                "ios"
                "android"
                "web"
            ]

        let DeviceInfo =
            Pattern.Config "DeviceInfo" {
                Required = []
                Optional = [
                    "name", T<string>
                    "model", T<string>
                    "platform", DevicePlatform.Type
                    "operatingSystem", OperatingSystem.Type
                    "osVersion", T<string>
                    "iOSVersion", T<int>
                    "androidSDKVersion", T<int>
                    "manufacturer", T<string>
                    "isVirtual", T<bool>
                    "memUsed", T<int>
                    "diskFree", T<int>
                    "diskTotal", T<int>
                    "realDiskFree", T<int>
                    "realDiskTotal", T<int>
                    "webViewVersion", T<string>
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
                    "value", T<string>
                ]
            }

        let LanguageTag =
            Pattern.Config "LanguageTag" {
                Required = []
                Optional = [
                    "value", T<string>
                ]
            }

        let DevicePlugin = 
            Class "DevicePlugin"
            |=> Nested [
                LanguageTag; GetLanguageCodeResult; BatteryInfo; DeviceInfo
                DevicePlatform; DeviceId; OperatingSystem
            ]
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
                Required = []
                Optional = [
                    "title", T<string>
                    "message", T<string>
                    "buttonTitle", T<string> 
                ]
            }

        let PromptResult =
            Pattern.Config "PromptResult" {
                Required = []
                Optional = [
                    "value", T<string>
                    "cancelled", T<bool>
                ]
            }

        let PromptOptions =
            Pattern.Config "PromptOptions" {
                Required = []
                Optional = [
                    "title", T<string>
                    "message", T<string>
                    "okButtonTitle", T<string>
                    "cancelButtonTitle", T<string>
                    "inputPlaceholder", T<string>
                    "inputText", T<string>
                ]
            }

        let ConfirmResult =
            Pattern.Config "ConfirmResult" {
                Required = []
                Optional = [
                    "value", T<bool>
                ]
            }

        let ConfirmOptions =
            Pattern.Config "ConfirmOptions" {
                Required = []
                Optional = [
                    "title", T<string>
                    "message", T<string>
                    "okButtonTitle", T<string>
                    "cancelButtonTitle", T<string>
                ]
            }

        let DialogPlugin = 
            Class "DialogPlugin" 
            |=> Nested [
                ConfirmOptions; ConfirmResult; PromptOptions; PromptResult; AlertOptions
            ]
            |+> Instance [
                "alert" =>  AlertOptions?options ^-> T<Promise<unit>>
                "prompt" =>  PromptOptions?options ^-> T<Promise<_>>[PromptResult]
                "confirm" =>  ConfirmOptions?options ^-> T<Promise<_>>[ConfirmResult]
            ]

    [<AutoOpen>]
    module Filesystem =
        let Directory = 
            Pattern.EnumStrings "Directory" [
                "Documents"
                "Data"
                "Library"
                "Cache"
                "External"
                "ExternalStorage"
            ]

        let Encoding = 
            Pattern.EnumStrings "Encoding" [
                "UTF8"
                "ASCII"
                "UTF16"
            ]

        let CopyOptions =
            Pattern.Config "CopyOptions" {
                Required = []
                Optional = [
                    "from", T<string>
                    "to", T<string>
                    "directory", Directory.Type
                    "toDirectory", Directory.Type
                ]
            }

        let RenameOptions = CopyOptions.Type

        let ProgressStatus = 
            Pattern.Config "ProgressStatus" {
                Required = []
                Optional = [
                    "url", T<string>
                    "bytes", T<int>
                    "contentLength", T<int>
                ]
            }

        let ProgressListener = ProgressStatus ^-> T<unit>

        let ReadFileResult =
            Pattern.Config "ReadFileResult" {
                Required = []
                Optional = [
                    "data", T<string> + T<Blob>
                ]
            }

        let ReadFileOptions = 
            Pattern.Config "ReadFileOptions" {
                Required = []
                Optional = [
                    "path", T<string>
                    "directory", Directory.Type
                    "encoding", Encoding.Type
                ]
            }

        let WriteFileResult = 
            Pattern.Config "WriteFileResult" {
                Required = []
                Optional = [
                    "uri", T<string> 
                ]
            }

        let WriteFileOptions = 
            Pattern.Config "WriteFileOptions" {
                Required = []
                Optional = [
                    "path", T<string> 
                    "data", T<string> + T<Blob>
                    "directory", Directory.Type
                    "encoding", Encoding.Type
                    "recursive", T<bool> 
                ]
            }

        let AppendFileOptions =     
            Pattern.Config "AppendFileOptions" {
                Required = []
                Optional = [
                    "path", T<string> 
                    "data", T<string>
                    "directory", Directory.Type
                    "encoding", Encoding.Type
                ]
            }

        let DeleteFileOptions =     
            Pattern.Config "DeleteFileOptions" {
                Required = []
                Optional = [
                    "path", T<string> 
                    "directory", Directory.Type
                ]
            }

        let MkdirOptions =     
            Pattern.Config "MkdirOptions" {
                Required = []
                Optional = [
                    "path", T<string> 
                    "directory", Directory.Type
                    "recursive", T<bool> 
                ]
            }

        let RmdirOptions =     
            Pattern.Config "RmdirOptions" {
                Required = []
                Optional = [
                    "path", T<string> 
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
                Required = []
                Optional = [
                    "name", T<string> 
                    "type", FileType.Type
                    "size", T<int> 
                    "ctime", T<int> 
                    "mtime", T<int> 
                    "uri", T<string> 
                ]
            }

        let ReaddirOptions = 
            Pattern.Config "ReaddirOptions" {
                Required = []
                Optional = [
                    "path", T<string> 
                    "directory", Directory.Type
                ]
            }

        let GetUriResult = 
            Pattern.Config "GetUriResult" {
                Required = []
                Optional = [
                    "uri", T<string> 
                ]
            }

        let GetUriOptions = 
            Pattern.Config "GetUriOptions" {
                Required = []
                Optional = [
                    "path", T<string> 
                    "directory", Directory.Type 
                ]
            }

        let StatResult = 
            Pattern.Config "StatResult" {
                Required = []
                Optional = [
                    "type", FileType.Type
                    "size", T<int> 
                    "ctime", T<int> 
                    "mtime", T<int> 
                    "uri", T<string> 
                ]
            }

        let StatOptions = 
            Pattern.Config "StatOptions" {
                Required = []
                Optional = [
                    "path", T<string> 
                    "directory", Directory.Type 
                ]
            }

        let CopyResult = 
            Pattern.Config "CopyResult" {
                Required = []
                Optional = [
                    "uri", T<string> 
                ]
            }

        let PermissionStatus = 
            Pattern.Config "PermissionStatus" {
                Required = []
                Optional = [
                    "publicStorage", PermissionState.Type
                ]
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
                Required = []
                Optional = [
                    "path", T<string> 
                    "directory", Directory.Type
                    "progress", T<bool> 
                    "recursive", T<bool> 
                ]
            }

        let ReaddirResult = 
            Pattern.Config "ReaddirResult" {
                Required = []
                Optional = [
                    "files", !| FileInfo
                ]
            }

        let FilesystemPlugin = 
            Class "Filesystem" 
            |=> Nested [
                ReaddirResult; DownloadFileOptions; DownloadFileResult; PermissionStatus; CopyResult
                StatOptions; StatResult; GetUriOptions; GetUriResult; ReaddirOptions; FileInfo
                FileType; RmdirOptions; MkdirOptions; DeleteFileOptions; AppendFileOptions
                WriteFileOptions; WriteFileResult; ReadFileOptions; Directory
                ProgressStatus; CopyOptions; Encoding; ReadFileResult; 
            ]
            |+> Instance [
                "readFile" => ReadFileOptions?options ^-> T<Promise<_>>[ReadFileResult]
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
    module Haptics = 
        let ImpactStyle =
            Pattern.EnumStrings "ImpactStyle" [
                "Heavy"
                "Medium"
                "Light"
            ]

        let NotificationType =
            Pattern.EnumStrings "NotificationType" [
                "Success"
                "Warning"
                "Error"
            ]

        let ImpactOptions = 
            Pattern.Config "ImpactOptions" {
                Required = []
                Optional = [
                    "style", ImpactStyle.Type
                ]
            }
        
        let NotificationOptions = 
            Pattern.Config "NotificationOptions" {
                Required = []
                Optional = [
                    "type", NotificationType.Type
                ]
            }

        let VibrateOptions = 
            Pattern.Config "VibrateOptions" {
                Required = []
                Optional = [
                    "duration", T<int>
                ]
            }

        let HapticsPlugin = 
            Class "HapticsPlugin" 
            |=> Nested [
                VibrateOptions; NotificationOptions; ImpactOptions; NotificationType; ImpactStyle                 
            ]
            |+> Instance [
                "impact" => !?ImpactOptions ?options ^-> T<Promise<unit>>
                "notification" => !?NotificationOptions?options ^-> T<Promise<unit>>
                "vibrate" => !?VibrateOptions?options ^-> T<Promise<unit>>
                "selectionStart" => T<unit> ^-> T<Promise<unit>>
                "selectionChanged" => T<unit> ^-> T<Promise<unit>>
                "selectionEnd" => T<unit> ^-> T<Promise<unit>>
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
                Required = []
                Optional = [
                    "allowZoom", T<bool>
                    "hardwareBack", T<bool>
                    "pauseMedia", T<bool>
                ]
            }

        let iOSWebViewOptions = 
            Pattern.Config "iOSWebViewOptions" {
                Required = []
                Optional = [
                    "allowOverScroll", T<bool>
                    "enableViewportScale", T<bool>
                    "allowInLineMediaPlayback", T<bool>
                    "surpressIncrementalRendering", T<bool>
                    "viewStyle", iOSViewStyle.Type
                    "animationEffect", iOSAnimation.Type
                ]
            }

        let AndroidBottomSheet = 
            Pattern.Config "AndroidBottomSheet" {
                Required = []
                Optional = [
                    "height", T<int>
                    "isFixed", T<bool>
                ]
            }

        let AndroidSystemBrowserOptions = 
            Pattern.Config "AndroidSystemBrowserOptions" {
                Required = []
                Optional = [
                    "showTitle", T<bool>
                    "hideToolbarOnScroll", T<bool>
                    "viewStyle", AndroidViewStyle.Type
                    "bottomSheetOptions", AndroidBottomSheet.Type
                    "startAnimation", AndroidAnimation.Type
                    "exitAnimation", AndroidAnimation.Type
                ]
            }

        let iOSSystemBrowserOptions = 
            Pattern.Config "iOSSystemBrowserOptions" {
                Required = []
                Optional = [
                    "closeButtonText", DismissStyle.Type
                    "viewStyle", iOSViewStyle.Type
                    "animationEffect", iOSAnimation.Type
                    "enableBarsCollapsing", T<bool>
                    "enableReadersMode", T<bool>
                ]
            }

        let OpenInDefaultParameterModel = 
            Pattern.Config "OpenInDefaultParameterModel" {
                Required = []
                Optional = [
                    "url", T<string>
                ]
            }

        let SystemBrowserOptions = 
            Pattern.Config "SystemBrowserOptions" {
                Required = []
                Optional = [
                    "android", AndroidSystemBrowserOptions.Type
                    "iOS", iOSSystemBrowserOptions.Type
                ]
            }

        let OpenInSystemBrowserParameterModel = 
            Pattern.Config "OpenInSystemBrowserParameterModel" {
                Required = []
                Optional = [
                    "options", SystemBrowserOptions.Type
                ]
            }

        let WebViewOptions = 
            Pattern.Config "WebViewOptions" {
                Required = []
                Optional = [
                    "showURL", T<bool>
                    "showToolbar", T<bool>
                    "clearCache", T<bool>
                    "clearSessionCache", T<bool>
                    "mediaPlaybackRequiresUserAction", T<bool>
                    "closeButtonText", T<string>
                    "toolbarPosition", ToolbarPosition.Type
                    "showNavigationButtons", T<bool>
                    "leftToRight", T<bool>
                    "customWebViewUserAgent", T<string>
                    "android", AndroidWebViewOptions.Type
                    "iOS", iOSWebViewOptions.Type
                ]
            }

        let OpenInWebViewParameterModel = 
            Pattern.Config "OpenInWebViewParameterModel" {
                Required = []
                Optional = [
                    "options", WebViewOptions.Type
                ]
            }

        let InAppBrowserPlugin = 
            Class "InAppBrowserPlugin" 
            |=> Nested [
                OpenInWebViewParameterModel; WebViewOptions; OpenInSystemBrowserParameterModel
                SystemBrowserOptions; OpenInDefaultParameterModel; iOSSystemBrowserOptions
                AndroidSystemBrowserOptions; AndroidBottomSheet; iOSWebViewOptions
                AndroidWebViewOptions; DismissStyle; AndroidAnimation; AndroidViewStyle
                iOSAnimation; ToolbarPosition; iOSViewStyle
            ]
            |+> Instance [
                "openInWebView" => OpenInWebViewParameterModel?model ^-> T<Promise<unit>>
                "openInSystemBrowser" => OpenInSystemBrowserParameterModel?model ^-> T<Promise<unit>>
                "openInExternalBrowser" => OpenInDefaultParameterModel?model ^-> T<Promise<unit>>
                "close" => T<unit> ^-> T<Promise<unit>>
                "addListener" => T<string>?eventName * ListenFunctionType T<unit>?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                "removeAllListeners" => T<unit> ^-> T<unit>
            ]
 
    [<AutoOpen>]
    module Keyboard =
        let KeyboardStyle = 
            Pattern.EnumStrings "KeyboardStyle" [
                "Dark"
                "Light"
                "Default"
            ]

        let KeyboardResize = 
            Pattern.EnumStrings "KeyboardResize" [
                "Body"
                "Ionic"
                "Native"
                "None"
            ]

        let KeyboardStyleOptions = 
            Pattern.Config "KeyboardStyleOptions" {
                Required = []
                Optional = [
                    "style", KeyboardStyle.Type
                ]
            }

        let KeyboardResizeOptions = 
            Pattern.Config "KeyboardResizeOptions" {
                Required = []
                Optional = [
                    "mode", KeyboardResize.Type
                ]
            }

        let KeyboardInfo = 
            Pattern.Config "KeyboardInfo" {
                Required = []
                Optional = [
                    "keyboardHeight", T<int>
                ]
            }

        let ScrollOptions =
            Pattern.Config "ScrollOptions" {
                Required = [ "isDisabled", T<bool> ]
                Optional = []
            }

        let AccessoryBarOptions =
            Pattern.Config "AccessoryBarOptions" {
                Required = [ "isVisible", T<bool> ]
                Optional = []
            }

        let KeyboardOptions =
            Pattern.Config "KeyboardOptions" {
                Required = []
                Optional = [
                    "resize", KeyboardResize.Type
                    "style", KeyboardStyle.Type
                    "resizeOnFullScreen", T<bool>
                ]
            }

        let PluginsConfig =
            Pattern.Config "PluginsConfig" {
                Required = []
                Optional = [
                    "Keyboard", KeyboardOptions.Type
                ]
            }

        let KeyboardPlugin =
            Class "KeyboardPlugin"
            |=> Nested [
                PluginsConfig; KeyboardOptions; AccessoryBarOptions; ScrollOptions; KeyboardResize
                KeyboardInfo; KeyboardResizeOptions; KeyboardStyleOptions; KeyboardStyle
            ]
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
            Pattern.EnumStrings "Importance" ["1"; "2"; "3"; "4"; "5"]

        let Visibility = 
            Pattern.EnumStrings "Visibility" ["-1"; "0"; "1"]

        let LocalNotificationDescriptor = 
            Pattern.Config "LocalNotificationDescriptor" {
                Required = []
                Optional = ["id", T<int>]
            }

        let ScheduleResult = 
            Pattern.Config "ScheduleResult" {
                Required = []
                Optional = ["notifications", !| LocalNotificationDescriptor]
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
                Required = []
                Optional = [
                    "id", T<string>
                    "url", T<string>
                    "options", AttachmentOptions.Type
                ]
            }

        let LocalNotificationSchema = 
            Pattern.Config "LocalNotificationSchema" {
                Required = []
                Optional = [
                    "title", T<string>
                    "body", T<string>
                    "largeBody", T<string>
                    "summaryText", T<string>
                    "id", T<int>
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
                    "group", T<string>
                    "groupSummary", T<bool>
                    "channelId", T<string>
                    "ongoing", T<bool>
                    "autoCancel", T<bool>
                    "inboxList", !| T<string>
                    "silent", T<bool>
                ]
            }

        let ScheduleOptions = 
            Pattern.Config "ScheduleOptions" {
                Required = []
                Optional = ["notifications", !| LocalNotificationSchema]
            }

        let PendingLocalNotificationSchema = 
            Pattern.Config "PendingLocalNotificationSchema" {
                Required = []
                Optional = [
                    "title", T<string>
                    "body", T<string>
                    "id", T<int>
                    "schedule", Schedule.Type
                    "extra", T<obj>
                ]
            }

        let PendingResult = 
            Pattern.Config "PendingResult" {
                Required = []
                Optional = [
                    "notifications", !| PendingLocalNotificationSchema
                ]
            }

        let Action = 
            Pattern.Config "Action" {
                Required = []
                Optional = [
                    "id", T<string>
                    "title", T<string>
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
                Required = []
                Optional = [
                    "id", T<string>
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
                Required = []
                Optional = [
                    "types", !| ActionType
                ]
            }

        let CancelOptions = 
            Pattern.Config "CancelOptions" {
                Required = []
                Optional = [
                    "types", !| LocalNotificationDescriptor
                ]
            }

        let EnabledResult = 
            Pattern.Config "EnabledResult" {
                Required = []
                Optional = [
                    "value", T<bool>
                ]
            }

        let DeliveredNotificationSchema = 
            Pattern.Config "DeliveredNotificationSchema" {
                Required = []
                Optional = [
                    "id", T<int>
                    "tag", T<string>
                    "title", T<string>
                    "body", T<string>
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
                Required = []
                Optional = [
                    "notifications", !| DeliveredNotificationSchema
                ]
            }

        let Channel = 
            Pattern.Config "Channel" {
                Required = []
                Optional = [
                    "id", T<string>
                    "name", T<string>
                    "description", T<string>
                    "sound", T<string>
                    "group", T<string>
                    "importance", Importance.Type
                    "visibility", Visibility.Type
                    "lights", T<bool>
                    "lightColor", T<string>
                    "vibration", T<bool>
                ]
            }

        let ListChannelsResult = 
            Pattern.Config "ListChannelsResult" {
                Required = []
                Optional = [
                    "channels", !| Channel
                ]
            }

        let PermissionStatus = 
            Pattern.Config "PermissionStatus" {
                Required = []
                Optional = [
                    "display", PermissionState.Type
                ]
            } 

        let SettingsPermissionStatus = 
            Pattern.Config "SettingsPermissionStatus" {
                Required = []
                Optional = [
                    "exact_alarm", PermissionState.Type
                ]
            }

        let ActionPerformed = 
            Pattern.Config "ActionPerformed" {
                Required = []
                Optional = [
                    "actionId", T<string>
                    "inputValue", T<string>
                    "notification", LocalNotificationSchema.Type
                ]
            }

        let LocalNotificationsOptions =
            Pattern.Config "LocalNotificationsOptions" {
                Required = []
                Optional = [
                    "smallIcon", T<string>
                    "iconColor", T<string>
                    "sound", T<string>
                ]
            }

        let PluginsConfig =
            Pattern.Config "PluginsConfig" {
                Required = []
                Optional = [
                    "LocalNotifications", LocalNotificationsOptions.Type
                ]
            }

        let DeleteChannelOptions = 
            Pattern.Config "DeleteChannelOptions" {
                Required = [ "id", T<string> ]
                Optional = []
            }
        
        let LocalNotificationsPlugin = 
            Class "LocalNotificationsPlugin"
            |=> Nested [
                DeleteChannelOptions; PluginsConfig; LocalNotificationsOptions; ActionPerformed; SettingsPermissionStatus
                PermissionStatus; ListChannelsResult; Channel; DeliveredNotifications; DeliveredNotificationSchema
                EnabledResult; CancelOptions; RegisterActionTypesOptions; ActionType; Action; PendingResult; Importance
                PendingLocalNotificationSchema; ScheduleOptions; LocalNotificationSchema; Attachment; AttachmentOptions
                Schedule; ScheduleOn; ScheduleResult; LocalNotificationDescriptor; Weekday; ScheduleEvery; Visibility
            ]
            |+> Instance [
                "schedule" => ScheduleOptions?options ^-> T<Promise<_>>[ScheduleResult]
                "getPending" => T<unit> ^-> T<Promise<_>>[PendingResult]
                "registerActionTypes" => RegisterActionTypesOptions?options ^-> T<Promise<unit>>
                "cancel" => CancelOptions?options ^-> T<Promise<unit>>
                "areEnabled" => T<unit> ^-> T<Promise<_>>[EnabledResult]
                "getDeliveredNotifications" => T<unit> ^-> T<Promise<_>>[DeliveredNotifications]
                "removeDeliveredNotifications" => DeliveredNotifications?delivered ^-> T<Promise<unit>>
                "removeAllDeliveredNotifications" => T<unit> ^-> T<Promise<unit>>
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
            |=> Nested [ConnectionType; ConnectionStatus]
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
                Optional = ["group", T<string>]
            }

        let GetOptions =
            Pattern.Config "GetOptions" {
                Required = ["key", T<string>]
                Optional = []
            }

        let GetResult =
            Pattern.Config "GetResult" {
                Required = ["value", T<string> + T<unit>]
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
                    "key", T<string>
                ]
                Optional = []
            }

        let KeysResult =
            Pattern.Config "KeysResult" {
                Required = [
                    "keys", !|T<string> 
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
            |=> Nested [ConfigureOptions; GetOptions; GetResult; SetOptions; RemoveOptions; KeysResult; MigrateResult]
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
            ]

        let PluginsConfig = 
            Pattern.Config "PluginsConfig" {
                Required = []
                Optional = [
                    "presentationOptions", PresentationOption.Type
                ]
            }

        let Importance =
            Pattern.EnumStrings "Importance" [
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
                    "receive", PermissionState.Type
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
                Required = []
                Optional = [
                    "title", T<string>
                    "subtitle", T<string>
                    "body", T<string>
                    "id", T<string>
                    "tag", T<string>
                    "badge", T<int>
                    "notification", T<obj>
                    "data", T<obj>
                    "click_action", T<string>
                    "link", T<string>
                    "group", T<string>
                    "groupSummary", T<bool>
                ]
            }

        let ActionPerformed =
            Pattern.Config "ActionPerformed" {
                Required = []
                Optional = [
                    "actionId", T<string>
                    "notification", PushNotificationSchema.Type
                    "inputValue", T<string>
                ]
            }

        let Token =
            Pattern.Config "Token" {
                Required = [
                    "value", T<string>
                ]
                Optional = []
            }

        let RegistrationError =
            Pattern.Config "RegistrationError" {
                Required = [
                    "error", T<string>
                ]
                Optional = []
            }

        let DeliveredNotifications =
            Pattern.Config "DeliveredNotifications" {
                Required = [
                    "notifications", !| PushNotificationSchema
                ]
                Optional = []
            }

        let ListChannelsResult =
            Pattern.Config "ListChannelsResult" {
                Required = [
                    "channels", !| Channel
                ]
                Optional = []
            }

        let DeleteChannelArgs = 
            Pattern.Config "DeleteChannelArgs" {
                Required = [
                    "id", T<string>
                ]
                Optional = []
            }

        let PushNotificationsPlugin =
            Class "PushNotificationsPlugin"
            |=> Nested [
                PresentationOption; Importance; Visibility; PermissionStatus; Channel; PushNotificationSchema; DeleteChannelArgs
                ActionPerformed; Token; RegistrationError; DeliveredNotifications; ListChannelsResult;PluginsConfig
            ]
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
                    "orientation", OrientationLockType.Type
                ]
                Optional = []
            }

        let ScreenOrientationResult =
            Pattern.Config "ScreenOrientationResult" {
                Required = [
                    "type", OrientationLockType.Type
                ]
                Optional = []
            }

        let ScreenOrientationPlugin =
            Class "ScreenOrientationPlugin"
            |=> Nested [OrientationLockType; OrientationLockOptions; ScreenOrientationResult]
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
                    "value", T<string>
                ]
                Optional = [
                    "language", T<string>
                ]
            }

        let ScreenReaderState =
            Pattern.Config "ScreenReaderState" {
                Required = [
                    "value", T<bool>
                ]
                Optional = []
            }

        let IsEnabledResult = 
            Pattern.Config "IsEnabledResult" {
                Required = [
                    "value", T<bool>
                ]
                Optional = []
            }

        let StateChangeListener = ScreenReaderState?state ^-> T<unit>

        let ScreenReaderPlugin =
            Class "ScreenReaderPlugin"
            |=> Nested [SpeakOptions; ScreenReaderState; IsEnabledResult]
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
                    "activityType", T<string>
                ]
            }

        let CanShareResult =
            Pattern.Config "CanShareResult" {
                Required = [
                    "value", T<bool>
                ]
                Optional = []
            }

        let SharePlugin =
            Class "SharePlugin"
            |=> Nested [ShareOptions;ShareResult;CanShareResult]
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
                    "fadeOutDuration", T<int>
                ]
            }

        let SplashScreenConfig =
            Pattern.Config "SplashScreenConfig" {
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
                    "SplashScreen", SplashScreenConfig.Type
                ]
            }

        let SplashScreenPlugin =
            Class "SplashScreenPlugin"
            |=> Nested [ShowOptions; HideOptions; SplashScreenConfig; PluginsConfig]
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
                Required = ["style", Style.Type]
                Optional = []
            }

        let AnimationOptions =
            Pattern.Config "AnimationOptions" {
                Required = []
                Optional = ["animation", Animation.Type]
            }

        let BackgroundColorOptions =
            Pattern.Config "BackgroundColorOptions" {
                Required = ["color", T<string>]
                Optional = []
            }

        let StatusBarInfo =
            Pattern.Config "StatusBarInfo" {
                Required = []
                Optional = [
                    "visible", T<bool>
                    "style", Style.Type
                    "color", T<string>
                    "overlays", T<bool>
                ]
            }

        let SetOverlaysWebViewOptions =
            Pattern.Config "SetOverlaysWebViewOptions" {
                Required = ["overlay", T<bool>]
                Optional = []
            }

        let StatusBarPlugin =
            Class "StatusBarPlugin" 
            |=> Nested [
                SetOverlaysWebViewOptions; StatusBarInfo; BackgroundColorOptions
                AnimationOptions; StyleOptions; Animation; Style
            ]
            |+> Instance [
                "setStyle" => StyleOptions?options ^-> T<Promise<unit>>
                "setBackgroundColor" => BackgroundColorOptions?options ^-> T<Promise<unit>>
                "show" => !?AnimationOptions?options ^-> T<Promise<unit>>
                "hide" => !?AnimationOptions?options ^-> T<Promise<unit>>
                "getInfo" => T<unit> ^-> T<Promise<_>>[StatusBarInfo]
                "setOverlaysWebView" => SetOverlaysWebViewOptions?options ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module TextZoom = 
        let GetResult = 
            Pattern.Config "GetResult" {
                Required = [
                    "value", T<int>
                ]
                Optional = []
            }

        let GetPreferredResult = 
            Pattern.Config "GetPreferredResult" {
                Required = [
                    "value", T<int>
                ]
                Optional = []
            }

        let SetOptions = 
            Pattern.Config "SetOptions" {
                Required = [
                    "value", T<int>
                ]
                Optional = []
            }

        let TextZoomPlugin =
            Class "TextZoomPlugin"
            |=> Nested [GetResult; GetPreferredResult; SetOptions]
            |+> Instance [
                "get" => T<unit> ^-> T<Promise<_>>[GetResult]
                "getPreferred" => T<unit> ^-> T<Promise<_>>[GetPreferredResult]
                "set" => SetOptions?options ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Toast = 
        let ShowOptions = 
            Pattern.Config "ShowOptions" {
                Required = [
                    "text", T<string>
                ]
                Optional = [
                    "duration", T<string>  
                    "position", T<string> 
                ]
            }

        let ToastPlugin =
            Class "ToastPlugin"
            |=> Nested [ShowOptions]
            |+> Instance [
                "show" => ShowOptions?options ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module Watch = 
        let CommandData = 
            Pattern.Config "CommandData" {
                Required = [
                    "command", T<string>
                ]
                Optional = []
            }

        let WatchUIOptions = 
            Pattern.Config "WatchUIOptions" {
                Required = [
                    "watchUI", T<string>
                ]
                Optional = []
            }

        let WatchDataOptions = 
            Pattern.Config "WatchDataOptions" {
                Required = [
                    "data", T<obj>
                ]
                Optional = []
            }

        let WatchPlugin =
            Class "WatchPlugin"
            |=> Nested [CommandData; WatchUIOptions; WatchDataOptions]
            |+> Instance [
                "addListener" => T<string>?eventName * ListenFunctionType CommandData?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "Listens for 'runCommand' event."
                "updateWatchUI" => WatchUIOptions?options ^-> T<Promise<unit>>
                "updateWatchData" => WatchDataOptions?options ^-> T<Promise<unit>>
            ]

    let Capacitor =
        Class "Capacitor"
        |+> Static [
            "WatchPlugin" =? WatchPlugin
            |> Import "Watch" "@capacitor/watch"
            "ToastPlugin" =? ToastPlugin
            |> Import "Toast" "@capacitor/toast"
            "TextZoomPlugin" =? TextZoomPlugin
            |> Import "TextZoom" "@capacitor/text-zoom"
            "StatusBarPlugin" =? StatusBarPlugin
            |> Import "StatusBar" "@capacitor/status-bar"
            "SplashScreenPlugin" =? SplashScreenPlugin
            |> Import "SplashScreen" "@capacitor/splash-screen"
            "SharePlugin" =? SharePlugin
            |> Import "Share" "@capacitor/share"
            "ScreenReaderPlugin" =? ScreenReaderPlugin
            |> Import "ScreenReader" "@capacitor/screen-reader"
            "ScreenOrientationPlugin" =? ScreenOrientationPlugin
            |> Import "ScreenOrientation" "@capacitor/screen-orientation"
            "PushNotificationsPlugin" =? PushNotificationsPlugin
            |> Import "PushNotifications" "@capacitor/push-notifications"
            "PreferencesPlugin" =? PreferencesPlugin
            |> Import "Preferences" "@capacitor/preferences"
            "NetworkPlugin" =? NetworkPlugin
            |> Import "Network" "@capacitor/network"
            "LocalNotificationsPlugin" =? LocalNotificationsPlugin
            |> Import "LocalNotifications" "@capacitor/local-notifications"
            "Keyboard" =? KeyboardPlugin
            |> Import "Keyboard" "@capacitor/keyboard"
            "InAppBrowser" =? InAppBrowserPlugin
            |> Import "InAppBrowser" "@capacitor/inappbrowser"
            "Haptics" =? HapticsPlugin
            |> Import "Haptics" "@capacitor/haptics"
            "Filesystem" =? FilesystemPlugin
            |> Import "Filesystem" "@capacitor/filesystem"
            "Camera" =? CameraPlugin
            |> Import "Camera" "@capacitor/camera"
            "Motion" =? MotionPlugin
            |> Import "Motion" "@capacitor/motion"
            "App" =? AppPlugin
            |> Import "App" "@capacitor/app"
            "ActionSheet" =? ActionSheetPlugin
            |> Import "ActionSheet" "@capacitor/action-sheet"
            "AppLauncher" =? AppLauncherPlugin
            |> Import "AppLauncher" "@capacitor/app-launcher"
            "Geolocation" =? GeolocationPlugin
            |> Import "Geolocation" "@capacitor/geolocation"
            "BarcodeScanner" =? BarcodeScannerPlugin
            |> Import "BarcodeScanner" "@capacitor/barcode-scanner"
            "Browser" =? BrowserPlugin
            |> Import "Browser" "@capacitor/browser"
            "Clipboard" =? ClipboardPlugin
            |> Import "Clipboard" "@capacitor/clipboard"
            "Device" =? DevicePlugin
            |> Import "Device" "@capacitor/device"
            "Dialog" =? DialogPlugin
            |> Import "Dialog" "@capacitor/dialog"
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.Capacitor" [
                Capacitor
                PermissionState
                PluginListenerHandle
                PresentationStyle
                ActionSheetPlugin
                AppLauncherPlugin
                AppPlugin
                BarcodeScannerPlugin
                BrowserPlugin
                CameraPlugin
                ClipboardPlugin
                DevicePlugin
                DialogPlugin
                FilesystemPlugin
                GeolocationPlugin
                HapticsPlugin
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
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()

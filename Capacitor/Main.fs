namespace Capacitor

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =
    let ListenFunctionType = T<unit> ^-> T<unit>

    [<AutoOpen>]
    module Core = 
        let PermissionState =
            Pattern.EnumStrings "PermissionState" [
                "prompt"
                "prompt-with-rationale"
                "granted"
                "denied"
            ]

        let PluginListenerHandle =
            Pattern.Config "PluginListenerHandle" {
                Required = []
                Optional = [
                    "remove", T<unit> ^-> T<Promise<unit>>
                ]
            }            

        let PresentationStyle = 
            Pattern.EnumStrings "PresentationStyle" [
                "fullscreen"
                "popover"
            ]

    [<AutoOpen>]
    module ActionSheet = 
        let ActionSheetButtonStyle =
            Pattern.EnumInlines "ActionSheetButtonStyle" [
                "Default", "DEFAULT"
                "Destructive", "DESTRUCTIVE"
                "Cancel", "CANCEL"
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

        let StateChangeListener = 
            Pattern.Config "StateChangeListener" {
                Required = []
                Optional = ["state", AppState.Type ^-> T<unit>]
            }

        let URLOpenListener = 
            Pattern.Config "URLOpenListener" {
                Required = []
                Optional = ["event", URLOpenListenerEvent.Type ^-> T<unit>]
            }

        let RestoredListener = 
            Pattern.Config "RestoredListener" {
                Required = []
                Optional = ["event", RestoredListenerEvent.Type ^-> T<unit>]
            }

        let BackButtonListener = 
            Pattern.Config "BackButtonListener" {
                Required = []
                Optional = ["event", BackButtonListenerEvent.Type ^-> T<unit>]
            }

        let AppPlugin = 
            Class "AppPlugin" 
            |=> Nested [
                BackButtonListener; RestoredListener; URLOpenListener; StateChangeListener
                BackButtonListenerEvent; RestoredListenerEvent; URLOpenListenerEvent
                AppLaunchUrl; AppState; AppInfo; PluginListenerHandle
            ]
            |+> Instance [
                "exitApp" => T<unit> ^-> T<Promise<unit>>
                "getInfo" => T<unit> ^-> T<Promise<_>>[AppInfo]
                "getState" => T<unit> ^-> T<Promise<_>>[AppState]
                "getLaunchUrl" => T<unit> ^-> T<Promise<_>>[AppLaunchUrl]
                "minimizeApp" => T<unit> ^-> T<Promise<unit>>
                "addListener" => T<string>?eventName * StateChangeListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName is appStateChange"
                "addListener" => T<string>?eventName * ListenFunctionType?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName can be either pause or resume"
                "addListener" => T<string>?eventName * URLOpenListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName is appUrlOpen"
                "addListener" => T<string>?eventName * RestoredListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName is appRestoredResult"
                "addListener" => T<string>?eventName * BackButtonListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName is backButton"
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
            |=> Nested [OpenOptions; PresentationStyle]
            |+> Instance [
                "open" => OpenOptions?options ^-> T<Promise<unit>>
                "close" => T<unit> ^-> T<Promise<unit>>
                "addListener" => T<string>?eventName * ListenFunctionType?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName can be either browserFinished or browserPageLoaded"
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

        let WatchPositionCallback = 
            Pattern.Config "WatchPositionCallback" {
                Required = ["position", Position.Type]
                Optional = ["err", T<obj>]
            }

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
                GeolocationPermissionType; WatchPositionCallback; Position; Coordinates; PermissionState
            ]
            |+> Instance [
                "getCurrentPosition" => PositionOptions?options ^-> T<Promise<_>>[Position]
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
                    "x", T<int>
                    "y", T<int>
                    "z", T<int>
                ]
            }

        let RotationRate = 
            Pattern.Config "RotationRate" {
                Required = []
                Optional = [
                    "alpha", T<int>
                    "beta", T<int>
                    "gamma", T<int>
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

        let AccelListener = 
            Pattern.Config "AccelListener" {
                Required = []
                Optional = [
                    "event", AccelListenerEvent ^-> T<unit>
                ]
            }

        let OrientationListener = 
            Pattern.Config "OrientationListener" {
                Required = []
                Optional = [
                    "event", RotationRate ^-> T<unit>
                ]
            }

        let OrientationListenerEvent = RotationRate

        let MotionPlugin = 
            Class "MotionPlugin"
            |=> Nested [
                OrientationListenerEvent; OrientationListener; AccelListener; AccelListenerEvent; Acceleration
            ]
            |+> Instance [
                "addListener" => T<string>?eventName * AccelListener?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName is accel"
                "addListener" => T<string>?evenName * OrientationListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName is orientation"
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

        let ProgressListener = 
            Pattern.Config "ProgressListener" {
                Required = []
                Optional = [
                    "progress", ProgressStatus ^-> T<unit>
                ]
            }

        let ReadFileResult =
            Pattern.Config "ReadFileResult" {
                Required = []
                Optional = [
                    "data", T<string> * !?T<Blob>
                ]
            }

        let ReadFileOptions = 
            Pattern.Config "ReadFileOptions" {
                Required = []
                Optional = [
                    "path", T<string>
                    "directory", Directory.Type
                    "encoding", Directory.Type
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
                    "data", T<string> * !?T<Blob>
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
                WriteFileOptions; WriteFileResult; ProgressListener; ReadFileOptions 
                ProgressStatus; CopyOptions; Encoding; ReadFileResult; Directory
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
                "impact" => ImpactOptions ?options ^-> T<Promise<unit>>
                "notification" => NotificationOptions?options ^-> T<Promise<unit>>
                "vibrate" => VibrateOptions?options ^-> T<Promise<unit>>
                "selectionStart" => T<unit> ^-> T<Promise<unit>>
                "selectionChanged" => T<unit> ^-> T<Promise<unit>>
                "selectionEnd" => T<unit> ^-> T<Promise<unit>>
            ]

    [<AutoOpen>]
    module InAppBrowser = 
        let ToolbarPosition = 
            Pattern.EnumStrings "ToolbarPosition" [
                "TOP"
                "BOTTOM"
            ]

        let iOSViewStyle =
            Pattern.EnumStrings "iOSViewStyle" [
                "PAGE_SHEET"
                "FORM_SHEET"
                "FULL_SCREEN"
            ]

        let iOSAnimation = 
            Pattern.EnumStrings "iOSAnimation" [
                "FLIP_HORIZONTAL"
                "CROSS_DISSOLVE"
                "COVER_VERTICAL"
            ]

        let AndroidViewStyle = 
            Pattern.EnumStrings "AndroidViewStyle" [
                "BOTTOM_SHEET"
                "FULL_SCREEN"
            ]

        let AndroidAnimation =  
            Pattern.EnumStrings "AndroidAnimation" [
                "FADE_IN"
                "FADE_OUT"
                "SLIDE_IN_LEFT"
                "SLIDE_OUT_RIGHT"
            ]

        let DismissStyle =  
            Pattern.EnumStrings "DismissStyle" [
                "CLOSE"
                "CANCEL"
                "DONE"
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
                    "customWebViewUserAgent", T<string >
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
                "addListener" => T<string>?eventName * ListenFunctionType?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
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

        let ListenFuncKeyboardInfo = KeyboardInfo ^-> T<unit>

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
                "addListener" => T<string>?eventName * ListenFuncKeyboardInfo?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName can be either keyboardWillShow or keyboardDidShow"
                "addListener" => T<string>?eventName * ListenFunctionType?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName can be either keyboardWillHide or keyboardDidHide"
                "removeAllListeners" => T<unit> ^-> T<Promise<unit>>
            ]

    let Capacitor =
        Class "Capacitor"
        |+> Static [
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
                MotionPlugin
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()

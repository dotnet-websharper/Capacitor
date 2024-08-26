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
            Pattern.Config "Camera.PermissionStatus"{
                Required = [
                    "camera", CameraPermissionState.Type
                    "photos", CameraPermissionState.Type
                ]
                Optional = []
            }

        let CameraPluginPermissions = 
            Pattern.Config "CameraPluginPermissions" {
                Required = ["permissions", !|CameraPermissionType]
                Optional = []
            }             

        let CameraPlugin = 
            Class "CameraPlugin" 
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
            |+> Instance [
                "addListener" => T<string>?eventName * AccelListener?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName is accel"
                "addListener" => T<string>?evenName * OrientationListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName is orientation"
            ]

    let Capacitor =
        Class "Capacitor"
        |+> Static [
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
                ImpactStyle
                NotificationType
                NotificationOptions
                Encoding
                FileInfo
                FileType
                Directory
                ProgressStatus
                ImpactOptions
                VibrateOptions
                ReadFileOptions
                ReadFileResult
                WriteFileOptions
                WriteFileResult
                AppendFileOptions
                DeleteFileOptions
                MkdirOptions
                RmdirOptions
                ReaddirOptions
                ReaddirResult
                GetUriOptions
                GetUriResult
                StatOptions
                StatResult
                CopyResult
                CopyOptions
                DownloadFileOptions
                DownloadFileResult
                ProgressListener
                HapticsPlugin
                FilesystemPlugin
                AlertOptions
                PromptOptions
                PromptResult
                ConfirmOptions
                ConfirmResult
                DialogPlugin
                LanguageTag
                GetLanguageCodeResult
                BatteryInfo
                DeviceInfo
                DeviceId
                OperatingSystem
                DevicePlatform
                DevicePlugin
                WriteOptions
                ReadResult
                OpenOptions
                ClipboardPlugin
                BrowserPlugin
                CapacitorBarcodeScannerTypeHint
                CapacitorBarcodeScannerAndroidScanningLibrary
                CapacitorBarcodeScannerCameraDirection
                CapacitorBarcodeScannerScanOrientation
                AndroidScanningLibrary
                WebOptions
                CapacitorBarcodeScannerOptions
                CapacitorBarcodeScannerScanResult
                BarcodeScannerPlugin
                Capacitor
                CameraPlugin
                GeolocationPlugin
                ActionSheetPlugin
                AppLauncherPlugin
                AppPlugin
                MotionPlugin
                PluginListenerHandle
                OrientationListener
                RotationRate
                AccelListenerEvent
                Acceleration
                AccelListener
                BackButtonListener
                BackButtonListenerEvent
                RestoredListener
                RestoredListenerEvent
                URLOpenListener
                URLOpenListenerEvent
                StateChangeListener
                AppState
                AppLaunchUrl
                AppInfo
                OpenURLResult
                OpenURLOptions
                CanOpenURLResult
                CanOpenURLOptions
                ShowActionsResult
                ShowActionsOptions
                ActionSheetButton
                ActionSheetButtonStyle
                Camera.PermissionStatus
                PermissionState
                GeolocationPluginPermissions
                GeolocationPermissionType
                ClearWatchOptions
                PositionOptions
                WatchPositionCallback
                Position
                Coordinates
                CameraPermissionState
                CameraPluginPermissions
                CameraPermissionType
                GalleryPhotos
                GalleryPhoto
                GalleryImageOptions
                PresentationStyle
                Photo
                ImageOptions
                CameraResultType
                CameraDirection
                CameraSource
            ] 
            Namespace "WebSharper.Capacitor.Geolocation" [
                Geolocation.PermissionStatus
            ]
            Namespace "WebSharper.Capacitor.Filesystem" [
                Filesystem.PermissionStatus
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()

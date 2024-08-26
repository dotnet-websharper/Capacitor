namespace Capacitor

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =
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

        let ActionSheet = 
            Class "ActionSheet" 
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

        let AppLauncher = 
            Class "AppLauncher" 
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

        let App = 
            let ListenFunctionType = T<unit> ^-> T<unit>

            Class "App" 
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

        let CameraPresentationStyle = 
            Pattern.EnumStrings "CameraPresentationStyle" [
                "fullscreen"
                "popover"
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
                    "presentationStyle", CameraPresentationStyle.Type
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
                    "presentationStyle", CameraPresentationStyle.Type
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

        let Motion = 
            Class "Motion"
            |+> Instance [
                "addListener" => T<string>?eventName * AccelListener?listenerFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName is accel"
                "addListener" => T<string>?evenName * OrientationListener?listenFunc ^-> T<Promise<_>>[PluginListenerHandle]
                |> WithComment "eventName is orientation"
            ]

    let Capacitor =
        Class "Capacitor"
        |+> Static [
            "Camera" =? CameraPlugin
            |> Import "Camera" "@capacitor/camera"
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.Capacitor" [
                Capacitor
                CameraPlugin
                GeolocationPlugin
                ActionSheet
                AppLauncher
                App
                Motion
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
                CameraPresentationStyle
                Photo
                ImageOptions
                CameraResultType
                CameraDirection
                CameraSource
            ] 
            Namespace "WebSharper.Capacitor.Geolocation" [
                Geolocation.PermissionStatus
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()

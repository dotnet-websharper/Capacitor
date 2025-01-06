# WebSharper Capacitor

This repository provides bindings for [Capacitor](https://capacitorjs.com/) plugins to be used with [WebSharper](https://websharper.com/), enabling seamless integration of Capacitor functionality into WebSharper projects.

## Overview

WebSharper Capacitor aims to bring the power of Capacitor plugins to WebSharper developers by providing F# bindings for various Capacitor plugins. These bindings facilitate the creation of modern, cross-platform web and mobile applications with WebSharper.

## Currently Bound Plugins

The following Capacitor plugins have bindings in this repository:

1. **BiometricAuth** - [@aparajita/capacitor-biometric-auth](https://www.npmjs.com/package/@aparajita/capacitor-biometric-auth)
2. **ActionSheet** - [@capacitor/action-sheet](https://www.npmjs.com/package/@capacitor/action-sheet)
3. **AppLauncher** - [@capacitor/app-launcher](https://www.npmjs.com/package/@capacitor/app-launcher)
4. **App** - [@capacitor/app](https://www.npmjs.com/package/@capacitor/app)
5. **BackgroundRunner** - [@capacitor/background-runner](https://www.npmjs.com/package/@capacitor/background-runner)
6. **BarcodeScanner** - [@capacitor/barcode-scanner](https://www.npmjs.com/package/@capacitor/barcode-scanner)
7. **Browser** - [@capacitor/browser](https://www.npmjs.com/package/@capacitor/browser)
8. **Camera** - [@capacitor/camera](https://www.npmjs.com/package/@capacitor/camera)
9. **Clipboard** - [@capacitor/clipboard](https://www.npmjs.com/package/@capacitor/clipboard)
10. **Cookies** - [@capacitor/core](https://www.npmjs.com/package/@capacitor/core)
11. **Device** - [@capacitor/device](https://www.npmjs.com/package/@capacitor/device)
12. **Dialog** - [@capacitor/dialog](https://www.npmjs.com/package/@capacitor/dialog)
13. **Filesystem** - [@capacitor/filesystem](https://www.npmjs.com/package/@capacitor/filesystem)
14. **Geolocation** - [@capacitor/geolocation](https://www.npmjs.com/package/@capacitor/geolocation)
15. **GoogleMaps** - [@capacitor/google-maps](https://www.npmjs.com/package/@capacitor/google-maps)
16. **Haptics** - [@capacitor/haptics](https://www.npmjs.com/package/@capacitor/haptics)
17. **Http** - [@capacitor/core](https://www.npmjs.com/package/@capacitor/core)
18. **InAppBrowser** - [@capacitor/inappbrowser](https://www.npmjs.com/package/@capacitor/inappbrowser)
19. **Keyboard** - [@capacitor/keyboard](https://www.npmjs.com/package/@capacitor/keyboard)
20. **LocalNotifications** - [@capacitor/local-notifications](https://www.npmjs.com/package/@capacitor/local-notifications)
21. **Motion** - [@capacitor/motion](https://www.npmjs.com/package/@capacitor/motion)
22. **Network** - [@capacitor/network](https://www.npmjs.com/package/@capacitor/network)
23. **Preferences** - [@capacitor/preferences](https://www.npmjs.com/package/@capacitor/preferences)
24. **PushNotifications** - [@capacitor/push-notifications](https://www.npmjs.com/package/@capacitor/push-notifications)
25. **ScreenOrientation** - [@capacitor/screen-orientation](https://www.npmjs.com/package/@capacitor/screen-orientation)
26. **ScreenReader** - [@capacitor/screen-reader](https://www.npmjs.com/package/@capacitor/screen-reader)
27. **Share** - [@capacitor/share](https://www.npmjs.com/package/@capacitor/share)
28. **SplashScreen** - [@capacitor/splash-screen](https://www.npmjs.com/package/@capacitor/splash-screen)
29. **StatusBar** - [@capacitor/status-bar](https://www.npmjs.com/package/@capacitor/status-bar)
30. **TextZoom** - [@capacitor/text-zoom](https://www.npmjs.com/package/@capacitor/text-zoom)
31. **Toast** - [@capacitor/toast](https://www.npmjs.com/package/@capacitor/toast)
32. **Watch** - [@capacitor/watch](https://www.npmjs.com/package/@capacitor/watch)

## Getting Started

### Prerequisites

Before starting, ensure you have the following:

- Node.js and npm installed on your machine.
- [Capacitor](https://capacitorjs.com/) set up for managing cross-platform apps.
- [WebSharper](https://websharper.com/) used for building F#-based web applications.
- A project structure ready to integrate Capacitor and WebSharper.

### Installation 

1. Initialize a Capacitor project:

   ```bash
   npm init                # Initialize a new Node.js project
   npm install             # Install default dependencies
   npm i @capacitor/core   # Install Capacitor core library
   npm i -D @capacitor/cli # Install Capacitor CLI as a dev dependency
   npx cap init "YourApp" com.example.yourapp --web-dir wwwroot/dist # Initialize Capacitor in the project
   ```

2. Add the WebSharper Capacitor NuGet package:

   ```bash
   dotnet add package WebSharper.Capacitor --version 8.0.0.494-beta1
   ```

3. Add Android platform:

   ```bash
   npm i @capacitor/android  # Install Capacitor Android platform support
   npx cap add android       # Add the Android platform to your project
   ```

4. Add iOS platform:

   ```bash
   npm i @capacitor/ios      # Install Capacitor iOS platform support
   npx cap add ios           # Add the iOS platform to your project
   ```

5. Choose and install the Capacitor plugins you want to use. For example, to use the **Camera** plugin:

   ```bash
   npm i @capacitor/camera
   ```

6. Build your project:

   ```bash
   npm i vite                # Install Vite for building your web assets
   npx vite build            # Build your web project with Vite
   ```

7. Sync configuration:

   ```bash
   npx cap sync              # Sync Capacitor configuration and plugins
   ```

### Notes

- Ensure your Capacitor project is correctly configured with `capacitor.config.json`.
- The WebSharper Capacitor package provides bindings for seamless integration but still requires Capacitor plugins to be installed via `npm`.
- Check out sample usage of plugins in the `WebSharper.Capacitor.Sample` directory.

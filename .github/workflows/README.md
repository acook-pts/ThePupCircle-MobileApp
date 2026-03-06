# GitHub Actions Workflows for ThePupCircle Mobile App

## Overview

This repository contains CI/CD workflows for building the ThePupCircle .NET MAUI mobile application for Android and iOS platforms.

## Workflows

### 1. `android-build.yml` - Android Build Pipeline
**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main`
- Manual workflow dispatch

**Outputs:**
- **Pull Requests**: Debug APK for testing
- **Develop Branch**: Release APK (unsigned)
- **Main Branch**: Signed AAB (App Bundle) ready for Google Play Store

**Requirements:** Android keystore secrets (see SECRETS_SETUP.md)

---

### 2. `ios-build.yml` - iOS Build Pipeline
**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main`
- Manual workflow dispatch

**Outputs:**
- **Pull Requests**: Debug build for iOS Simulator
- **Develop Branch**: Release build for iOS Simulator
- **Main Branch**: Signed IPA ready for App Store

**Requirements:** iOS certificates and provisioning profiles (see SECRETS_SETUP.md)

---

### 3. `ci-combined.yml` - Combined CI Pipeline
**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main`
- Manual workflow dispatch

**Features:**
- Code quality checks
- Format verification
- Triggers both Android and iOS builds
- Parallel execution for faster builds

---

## Build Matrix

| Event | Branch | Android Output | iOS Output |
|-------|--------|---------------|-----------|
| Pull Request | any | Debug APK | Debug .app (Simulator) |
| Push | develop | Release APK (unsigned) | Release .app (Simulator) |
| Push | main | Signed AAB (Play Store ready) | Signed IPA (App Store ready) |

---

## Setup Instructions

### 1. Configure Secrets
Follow the instructions in `.github/SECRETS_SETUP.md` to configure required secrets for:
- Android signing (keystore)
- iOS code signing (certificates & provisioning profiles)

### 2. First-Time Setup

#### Android Additional Setup
Add to your `ThePupCircle.MobileApp.csproj` (if not already present):
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release' AND '$(AndroidKeyStore)' == 'true'">
  <AndroidSigningKeyStore>$(AndroidSigningKeyStore)</AndroidSigningKeyStore>
  <AndroidSigningKeyAlias>$(AndroidSigningKeyAlias)</AndroidSigningKeyAlias>
  <AndroidSigningKeyPass>$(AndroidSigningKeyPass)</AndroidSigningKeyPass>
  <AndroidSigningStorePass>$(AndroidSigningStorePass)</AndroidSigningStorePass>
</PropertyGroup>
```

#### iOS Additional Setup
Create `Platforms/iOS/Entitlements.plist`:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <!-- Add any required entitlements here -->
    <key>com.apple.developer.networking.wifi-info</key>
    <true/>
</dict>
</plist>
```

### 3. Testing the Workflows

#### Test Android Build (Local)
```bash
dotnet build ThePupCircle.MobileApp.csproj -c Debug -f net10.0-android
```

#### Test iOS Build (Local - Mac only)
```bash
dotnet build ThePupCircle.MobileApp.csproj -c Debug -f net10.0-ios
```

#### Trigger Workflow Manually
1. Go to Actions tab in GitHub
2. Select workflow (e.g., "Android Build")
3. Click "Run workflow"
4. Select branch
5. Click "Run workflow"

---

## Downloading Build Artifacts

After a successful build:

1. Go to **Actions** tab in GitHub
2. Click on the workflow run
3. Scroll to **Artifacts** section
4. Download:
   - `android-apk-{SHA}` or `android-aab-production-{SHA}`
   - `ios-app-simulator-{SHA}` or `ios-ipa-production-{SHA}`

### Installing on Device

**Android APK:**
```bash
adb install path/to/app.apk
```

**iOS Simulator:**
```bash
xcrun simctl install booted path/to/app.app
```

**iOS Device:**
- Use Xcode to install IPA, or
- Use a mobile device management (MDM) solution, or
- Upload to TestFlight/App Store Connect

---

## Workflow Customization

### Change .NET Version
Update in all workflow files:
```yaml
env:
  DOTNET_VERSION: '10.0.x'  # Change to desired version
```

### Add Environment Variables
Add to workflow file under `env:` section:
```yaml
env:
  API_URL: 'https://thepupcircle.com'
  CUSTOM_VAR: 'value'
```

### Add Build Steps
Insert additional steps in the workflow YAML:
```yaml
- name: Custom Step
  run: |
    echo "Running custom command"
    # Your commands here
```

---

## Troubleshooting

### Android Build Fails

**Problem**: Keystore errors
- **Solution**: Verify secrets are correctly base64 encoded
- **Check**: Alias and password match your keystore

**Problem**: SDK version not found
- **Solution**: Update `SupportedOSPlatformVersion` in .csproj

### iOS Build Fails

**Problem**: Code signing errors
- **Solution**: Ensure certificate and provisioning profile match
- **Check**: Profile is not expired
- **Verify**: Bundle identifier matches provisioning profile

**Problem**: Xcode version mismatch
- **Solution**: Update `xcode-version` in workflow or install required Xcode

### General Issues

**Problem**: .NET MAUI workload not found
- **Solution**: Update workflow to use latest .NET SDK
- **Check**: MAUI workload installation step completes successfully

**Problem**: Build succeeds but no artifacts
- **Solution**: Check artifact path in workflow matches actual output location
- **Verify**: Build configuration is correct

---

## CI/CD Best Practices

1. **Test locally first** before pushing to GitHub
2. **Use feature branches** for development
3. **PR to develop** for testing, then **develop to main** for production
4. **Tag releases** on main branch for versioning
5. **Monitor Actions tab** for build status
6. **Review artifacts** before distributing to users/stores
7. **Keep secrets updated** and secure
8. **Rotate signing keys** periodically

---

## Version Bumping

Before releasing, update version in `.csproj`:
```xml
<ApplicationDisplayVersion>1.1</ApplicationDisplayVersion>
<ApplicationVersion>2</ApplicationVersion>
```

Consider automating this with a workflow or script.

---

## Resources

- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [GitHub Actions Documentation](https://docs.github.com/actions)
- [Android App Signing](https://developer.android.com/studio/publish/app-signing)
- [iOS Code Signing Guide](https://developer.apple.com/support/code-signing/)

# GitHub Secrets Setup Guide

To enable automated builds and deployments, configure the following secrets in your GitHub repository.

## Navigation
Go to: **Repository Settings** ? **Secrets and variables** ? **Actions** ? **New repository secret**

---

## Android Secrets

### Required for Production Builds (main branch)

#### `ANDROID_KEYSTORE_FILE`
- **Description**: Your Android signing keystore file (base64 encoded)
- **How to generate**:
  ```bash
  # Create keystore (if you don't have one)
  keytool -genkey -v -keystore thepupcircle.keystore -alias thepupcircle -keyalg RSA -keysize 2048 -validity 10000
  
  # Convert to base64 for GitHub secret
  # On Linux/Mac:
  base64 -i thepupcircle.keystore | tr -d '\n' > keystore.txt
  
  # On Windows (PowerShell):
  [Convert]::ToBase64String([IO.File]::ReadAllBytes("thepupcircle.keystore")) | Out-File keystore.txt
  ```
- **Value**: Paste the base64 string from keystore.txt

#### `ANDROID_KEYSTORE_ALIAS`
- **Description**: The alias name you used when creating the keystore
- **Example**: `thepupcircle`
- **Value**: Your keystore alias

#### `ANDROID_KEYSTORE_PASSWORD`
- **Description**: The password for your keystore
- **Value**: Your keystore password (keep this secure!)

---

## iOS Secrets

### Required for Production Builds (main branch)

#### `IOS_CERTIFICATE_P12_BASE64`
- **Description**: Your iOS distribution certificate (.p12 file, base64 encoded)
- **How to generate**:
  1. Export certificate from Keychain Access on Mac
  2. Convert to base64:
     ```bash
     base64 -i Certificates.p12 | tr -d '\n' > certificate.txt
     ```
- **Value**: Paste the base64 string from certificate.txt

#### `IOS_CERTIFICATE_PASSWORD`
- **Description**: Password for the .p12 certificate file
- **Value**: Your certificate password

#### `IOS_PROVISIONING_PROFILE_BASE64`
- **Description**: Your iOS provisioning profile (base64 encoded)
- **How to generate**:
  1. Download provisioning profile from Apple Developer Portal
  2. Convert to base64:
     ```bash
     base64 -i YourProfile.mobileprovision | tr -d '\n' > profile.txt
     ```
- **Value**: Paste the base64 string from profile.txt

#### `IOS_PROVISIONING_PROFILE_NAME`
- **Description**: The name of your provisioning profile
- **How to find**: Open the .mobileprovision file in a text editor and look for the `<key>Name</key>` value
- **Example**: `ThePupCircle Production`
- **Value**: Your provisioning profile name

#### `IOS_CODESIGN_KEY`
- **Description**: The code signing identity name
- **How to find**: Run in terminal: `security find-identity -v -p codesigning`
- **Example**: `iPhone Distribution: Your Company Name (TEAM_ID)`
- **Value**: Your code signing identity

---

## Optional Secrets for Distribution

### `GOOGLE_PLAY_SERVICE_ACCOUNT_JSON`
- **Description**: For automated Play Store deployment
- **Value**: JSON key file from Google Play Console (base64 encoded)

### `APPLE_APP_STORE_CONNECT_KEY`
- **Description**: For automated App Store deployment
- **Value**: App Store Connect API key (.p8 file content, base64 encoded)

### `APPLE_APP_STORE_CONNECT_KEY_ID`
- **Value**: Your App Store Connect API Key ID

### `APPLE_APP_STORE_CONNECT_ISSUER_ID`
- **Value**: Your App Store Connect Issuer ID

---

## Quick Setup Checklist

### For Android:
- [ ] Generate keystore (or use existing)
- [ ] Add `ANDROID_KEYSTORE_FILE` secret (base64)
- [ ] Add `ANDROID_KEYSTORE_ALIAS` secret
- [ ] Add `ANDROID_KEYSTORE_PASSWORD` secret
- [ ] Update project file with keystore settings (if needed)

### For iOS:
- [ ] Export distribution certificate from Xcode
- [ ] Add `IOS_CERTIFICATE_P12_BASE64` secret
- [ ] Add `IOS_CERTIFICATE_PASSWORD` secret
- [ ] Download provisioning profile from Apple Developer
- [ ] Add `IOS_PROVISIONING_PROFILE_BASE64` secret
- [ ] Add `IOS_PROVISIONING_PROFILE_NAME` secret
- [ ] Add `IOS_CODESIGN_KEY` secret
- [ ] Create Entitlements.plist in Platforms/iOS/ folder

---

## Testing Without Secrets

For development/testing without production secrets:
- Pull requests and non-main branches build unsigned/debug versions
- These builds work for testing but cannot be distributed via app stores
- Artifacts are uploaded to GitHub Actions for download and testing

---

## Security Best Practices

1. **Never commit** keystore files, certificates, or provisioning profiles to git
2. **Rotate secrets** periodically (especially passwords)
3. **Limit access** to production secrets (use environment-specific secrets if needed)
4. **Use separate** keystores for debug vs production builds
5. **Backup** your keystore and certificates in a secure location
6. **Document** where production keys are stored (password manager, secure vault, etc.)

---

## Need Help?

- **Android Keystore**: https://developer.android.com/studio/publish/app-signing
- **iOS Code Signing**: https://developer.apple.com/support/code-signing/
- **GitHub Secrets**: https://docs.github.com/en/actions/security-guides/encrypted-secrets

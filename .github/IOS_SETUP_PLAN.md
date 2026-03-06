# iOS Certificate & Signing Management Plan
## For ThePupCircle Mobile App

> **Goal**: Get iOS building on GitHub Actions and maintain it permanently without losing access

---

## ?? Executive Summary

**Current Status**: ? No iOS certificates configured  
**Target Status**: ? iOS building automatically on GitHub Actions  
**One-Time Setup Cost**: ~$100-$150 (Apple Developer + Mac rental)  
**Annual Renewal Cost**: $99/year (Apple Developer Program)  
**Setup Time**: 2-4 hours (one-time)

---

## Phase 1: Apple Developer Program Setup (Start Now)

### Step 1: Enroll in Apple Developer Program
**Cost**: $99/year  
**Time**: 24-48 hours for approval

1. Go to https://developer.apple.com/programs/enroll/
2. Sign in with your Apple ID (or create one)
3. Complete enrollment:
   - Choose **Individual** (easier) or **Organization** (if you have a company)
   - Provide payment information
   - Accept agreements
4. Wait for email confirmation (usually 24-48 hours)

**DO THIS NOW** - While waiting for approval, you can plan the next steps.

? **Action**: Enroll today  
?? **Deadline**: ASAP (needed for all iOS work)

---

## Phase 2: Rent Mac Access (One-Time, 2-4 Hours)

### Best Options for Mac Rental

#### Option A: MacInCloud (RECOMMENDED)
- **Website**: https://www.macincloud.com
- **Cost**: $1-2/hour or $30/month
- **What You Get**: Remote Mac with Xcode pre-installed
- **Recommendation**: Pay for 24-hour access ($20-30)
  - Gives you breathing room if things take longer
  - Can revisit if you make mistakes

#### Option B: AWS EC2 Mac Instances
- **Cost**: ~$1.08/hour (minimum 24 hours = $25)
- **Setup**: More technical, requires AWS account
- **Best For**: If you already use AWS

#### Option C: Friend/Family Mac
- **Cost**: Free (beer/coffee)
- **Time**: 2 hours in person
- **Risk**: Limited time, pressure

**Recommendation**: Use MacInCloud for flexibility and peace of mind.

? **Action**: Rent Mac when Apple Developer approval comes through  
?? **Deadline**: Within 1 week of Apple approval

---

## Phase 3: Generate iOS Certificates & Profiles (On Rented Mac)

### Detailed Step-by-Step Process

#### Part A: Install Xcode (If Not Present)
1. Open App Store on Mac
2. Search "Xcode"
3. Click "Get" or "Install" (it's free, but 40+ GB)
4. Wait for installation (can take 30-60 minutes)

#### Part B: Configure Apple Developer Account in Xcode
1. Open Xcode
2. Go to **Xcode** > **Settings/Preferences** > **Accounts**
3. Click **+** button ? **Add Apple ID**
4. Sign in with your Apple Developer account
5. Verify your team appears

#### Part C: Register App ID
1. Go to https://developer.apple.com/account/
2. Navigate to **Certificates, Identifiers & Profiles**
3. Click **Identifiers** > **+** button
4. Select **App IDs** > **Continue**
5. Fill in:
   - **Description**: ThePupCircle Mobile App
   - **Bundle ID**: `com.thepupcircle.app` (EXACTLY as in .csproj)
   - **Capabilities**: Check any needed (Push Notifications, etc.)
6. Click **Continue** > **Register**

#### Part D: Create Distribution Certificate
1. Open **Keychain Access** on Mac
2. Menu: **Keychain Access** > **Certificate Assistant** > **Request a Certificate from a Certificate Authority**
3. Fill in:
   - Email: your email
   - Common Name: ThePupCircle Distribution
   - Select: **Saved to disk**
4. Save the `.certSigningRequest` file
5. Go to https://developer.apple.com/account/
6. Navigate to **Certificates** > **+** button
7. Select **Apple Distribution** > **Continue**
8. Upload the `.certSigningRequest` file
9. Download the certificate (`.cer` file)
10. Double-click to install in Keychain Access

#### Part E: Export Certificate as .p12
1. Open **Keychain Access**
2. Select **login** keychain > **My Certificates** category
3. Find your "Apple Distribution" certificate
4. Right-click > **Export "Apple Distribution..."**
5. Save as `.p12` file
6. **IMPORTANT**: Set a strong password (you'll need this later)
7. **CRITICAL**: Save this .p12 file somewhere safe immediately

#### Part F: Create Provisioning Profile
1. Go to https://developer.apple.com/account/
2. Navigate to **Profiles** > **+** button
3. Select **App Store** (under Distribution) > **Continue**
4. Select your App ID (`com.thepupcircle.app`) > **Continue**
5. Select the distribution certificate you just created > **Continue**
6. Name: "ThePupCircle Production" > **Generate**
7. Download the `.mobileprovision` file

#### Part G: Gather Code Signing Identity
1. Open **Terminal** on Mac
2. Run: `security find-identity -v -p codesigning`
3. Copy the full identity name (e.g., "iPhone Distribution: Your Name (TEAM_ID)")
4. Copy the provisioning profile name: "ThePupCircle Production"

---

## Phase 4: Secure Storage & Backup (CRITICAL)

### Files You MUST Save Forever

| File | Description | Storage Location |
|------|-------------|------------------|
| `Certificates.p12` | Distribution certificate | 3+ secure locations |
| `.mobileprovision` | Provisioning profile | 2+ secure locations |
| `password.txt` | P12 password | Password manager ONLY |
| `codesign-identity.txt` | Code signing identity string | Password manager |

### Recommended Storage Strategy

#### Primary Storage: Password Manager
**Options**: 1Password, Bitwarden, LastPass (Business/Family plan)

Create secure note with:
- Certificate .p12 file (as attachment)
- Provisioning profile file (as attachment)
- P12 password
- Code signing identity string
- Apple Developer account credentials
- Keystore alias and passwords

#### Secondary Backup: Encrypted Cloud Storage
**Options**: Encrypted folder in Dropbox/Google Drive/OneDrive

1. Create folder: "ThePupCircle-iOS-Certificates-DONOTDELETE"
2. Add README explaining what it is
3. Store certificate and provisioning profile
4. **DO NOT** store passwords here (use password manager)

#### Tertiary Backup: Offline Storage
**Options**: Encrypted USB drive, external hard drive

1. Label drive clearly: "ThePupCircle Certificates - Keep Safe"
2. Store in secure location (safe, locked drawer)
3. Test annually to ensure drive still works

### Security Checklist
- [ ] Certificate .p12 stored in 3 locations
- [ ] Provisioning profile stored in 2 locations
- [ ] Password in password manager (not written down)
- [ ] USB backup created and tested
- [ ] Team members know where to find backups
- [ ] Added reminder to check backups annually

---

## Phase 5: Configure GitHub Secrets (After Certificate Generation)

### Step-by-Step Secret Setup

#### 1. Convert Files to Base64 (On Rented Mac or Local Windows)

**On Mac:**
```bash
# Convert certificate
base64 -i Certificates.p12 | tr -d '\n' > certificate-base64.txt

# Convert provisioning profile
base64 -i ThePupCircle_Production.mobileprovision | tr -d '\n' > profile-base64.txt
```

**On Windows PowerShell:**
```powershell
# Convert certificate
[Convert]::ToBase64String([IO.File]::ReadAllBytes("Certificates.p12")) | Out-File certificate-base64.txt

# Convert provisioning profile
[Convert]::ToBase64String([IO.File]::ReadAllBytes("ThePupCircle_Production.mobileprovision")) | Out-File profile-base64.txt
```

#### 2. Add Secrets to GitHub

Go to: **GitHub Repository** > **Settings** > **Secrets and variables** > **Actions** > **New repository secret**

Add these 5 secrets:

| Secret Name | Value | Where to Get It |
|-------------|-------|-----------------|
| `IOS_CERTIFICATE_P12_BASE64` | Content of `certificate-base64.txt` | From conversion above |
| `IOS_CERTIFICATE_PASSWORD` | Your .p12 password | From Part E above |
| `IOS_PROVISIONING_PROFILE_BASE64` | Content of `profile-base64.txt` | From conversion above |
| `IOS_PROVISIONING_PROFILE_NAME` | `ThePupCircle Production` | From Part F above |
| `IOS_CODESIGN_KEY` | `iPhone Distribution: Your Name (TEAM_ID)` | From Part G above |

#### 3. Verify Secrets
- [ ] All 5 secrets added
- [ ] No extra spaces or newlines
- [ ] Passwords match certificate
- [ ] Profile name matches exactly

---

## Phase 6: Test iOS Build on GitHub Actions

### First Build Test

1. Commit and push all changes:
   ```bash
   git add .
   git commit -m "Add iOS Entitlements and complete iOS setup"
   git push origin main
   ```

2. Go to GitHub Actions tab
3. Watch the "iOS Build" workflow
4. If successful, download the IPA artifact

### Troubleshooting Common Issues

| Error | Solution |
|-------|----------|
| "Code signing identity not found" | Verify `IOS_CODESIGN_KEY` matches exactly |
| "Provisioning profile not found" | Verify `IOS_PROVISIONING_PROFILE_NAME` matches exactly |
| "Certificate password incorrect" | Verify `IOS_CERTIFICATE_PASSWORD` is correct |
| "Bundle identifier mismatch" | Verify App ID matches `com.thepupcircle.app` |

---

## Long-Term Maintenance Schedule

### Annual Tasks (Every Year)

#### 1. Renew Apple Developer Program
**When**: ~30 days before expiration  
**Cost**: $99  
**Where**: https://developer.apple.com/account/

Apple will email you before expiration. Renew promptly to avoid certificate expiration.

#### 2. Check Certificate Expiration
**When**: Annually in January  
**How**: 
1. Go to https://developer.apple.com/account/
2. Check **Certificates** section
3. Note expiration dates
4. Certificates expire after 1 year

#### 3. Regenerate Certificates (If Expired)
**When**: If certificate expires or is revoked  
**Cost**: Free, but requires Mac rental again (~$20-30)  
**Process**: Follow Phase 3 again

#### 4. Update Provisioning Profiles
**When**: When adding new capabilities or devices  
**How**: Re-create profile in Apple Developer Portal

#### 5. Test Backups
**When**: Every 6 months  
**How**:
1. Download certificate from backup location
2. Verify files open correctly
3. Test password works

### Monthly Tasks

#### Check GitHub Actions Status
**When**: After each release build  
**What**: Verify iOS build succeeded

---

## Emergency Recovery Procedures

### Scenario 1: Lost Certificate Password
**Impact**: Cannot build or update app  
**Solution**:
1. Revoke existing certificate in Apple Developer Portal
2. Rent Mac again
3. Generate new certificate
4. Update GitHub secrets
5. Update password manager

**Cost**: ~$20-30 for Mac rental  
**Time**: 1-2 hours

### Scenario 2: Lost Certificate File
**Impact**: Cannot build or update app  
**Solution**:
1. Check all backup locations:
   - Password manager
   - Cloud storage
   - USB drive
2. If truly lost, follow Scenario 1 process

### Scenario 3: Mac Needed for Troubleshooting
**Impact**: Cannot debug certificate issues  
**Solution**:
1. Rent MacInCloud for 24 hours (~$20-30)
2. Download certificate from backups
3. Install in Keychain Access
4. Debug issue
5. Re-export if needed

### Scenario 4: Apple Developer Account Compromised
**Impact**: Certificates may be revoked  
**Solution**:
1. Contact Apple Support immediately
2. Secure your Apple ID
3. Regenerate all certificates and profiles
4. Update GitHub secrets

---

## Cost Breakdown & Timeline

### Initial Setup (One-Time)
| Item | Cost | When |
|------|------|------|
| Apple Developer Program | $99 | Now |
| Mac Rental (MacInCloud 24hr) | $20-30 | After approval |
| **Total Initial** | **~$120-130** | **Week 1-2** |

### Annual Costs
| Item | Cost | Frequency |
|------|------|-----------|
| Apple Developer Renewal | $99 | Yearly |
| Mac Rental (if cert expires) | $0-30 | Only if needed |
| **Total Annual** | **~$99-130** | **Every 12 months** |

### Time Investment
| Task | Time | Frequency |
|------|------|-----------|
| Initial setup | 2-4 hours | One-time |
| Annual renewal | 10 minutes | Yearly |
| Certificate regeneration | 1-2 hours | If needed |

---

## Success Checklist

### Setup Complete When:
- [x] Entitlements.plist created
- [ ] Apple Developer Program enrolled
- [ ] Mac rental completed
- [ ] Distribution certificate created and exported
- [ ] Provisioning profile created
- [ ] Certificate .p12 backed up in 3 locations
- [ ] Provisioning profile backed up in 2 locations
- [ ] Password stored in password manager
- [ ] GitHub secrets configured (5 secrets)
- [ ] iOS workflow builds successfully
- [ ] IPA downloaded and verified
- [ ] Calendar reminders set for renewals

---

## Next Actions (In Order)

1. **TODAY**: Enroll in Apple Developer Program ($99)
2. **Wait**: 24-48 hours for Apple approval
3. **After approval**: Rent MacInCloud 24-hour access ($20-30)
4. **During rental**: Follow Phase 3 (2-4 hours)
5. **Immediately after**: Back up certificates (Phase 4)
6. **Same day**: Configure GitHub secrets (Phase 5)
7. **Test**: Push to main and verify build
8. **Set reminder**: Calendar event for renewal next year

---

## Questions or Issues?

### Resources
- Apple Developer Support: https://developer.apple.com/contact/
- MacInCloud Support: https://www.macincloud.com/contact/
- GitHub Actions Documentation: https://docs.github.com/actions
- .NET MAUI iOS Signing: https://learn.microsoft.com/dotnet/maui/ios/deployment/

### This Document
Keep this document:
- In your password manager
- In the project repository
- Printed and filed (optional)

**Last Updated**: Today  
**Next Review**: In 1 year or when Apple Developer Program renews

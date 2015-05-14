; NSIS script for DbExporter and supporting programs.
; Script partially generated by the HM NIS Edit Script Wizard.

; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "JSONLinter"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_PUBLISHER "Sam Saint-Pettersen"
!define PRODUCT_WEB_SITE "https://github.com/stpettersens/JSONLinter"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\JSONLinter.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; MUI 1.67 compatible ------
!include "MUI.nsh"

; StrContains --------------
!include "StrContains.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!define MUI_LICENSEPAGE_RADIOBUTTONS
!insertmacro MUI_PAGE_LICENSE "license.txt"
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\JSONLinter.exe"

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "JSONLinter_Setup.exe"
InstallDir "$PROGRAMFILES\JSONLinter"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

Function installJsonLinter
  DetailPrint "Installing jsonlint via node package manager (npm)..."
  nsExec::ExecToStack `npm.cmd install jsonlint`
  Pop $0 ; Pop return code from program from stack.
  Pop $1 ; Pop stdout from program from stack.
    StrCmp $0 "error" fail
    GoTo pass
  fail:
    DetailPrint "-------------------------------------------------------------------------------------------------"
    DetailPrint "Failed to install jsonlint. Node package manager (npm) not installed and/or"
    DetailPrint "not in system PATH variable. Please install it and/or set in PATH and rerun setup."
    DetailPrint "-------------------------------------------------------------------------------------------------"
    DetailPrint "Opening Node.js download page in your browser (npm is included with Node.js)"
    ExecShell "open" "https://nodejs.org/download"
    Abort
  pass:
    DetailPrint "Installed successfully."
FunctionEnd

Section "MainSection" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer

  Call installJsonLinter

  File "JSONLinter.exe"
  File "JSONLinter.exe.config"
  File "ScintillaNET.dll"
  File "JSONLinter_License.txt"
  File "jsonlint_License.txt"
  File "ScintillaNET_License.txt"

  CreateDirectory "$SMPROGRAMS\JSONLinter"
  CreateShortCut "$SMPROGRAMS\JSONLinter\JSONLinter.lnk" "$INSTDIR\JSONLinter.exe"
  CreateShortCut "$DESKTOP\JSONLinter.lnk" "$INSTDIR\JSONLinter.exe"
SectionEnd

Section -AdditionalIcons
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\JSONLinter\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\JSONLinter\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\JSONLinter.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\JSONLinter.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd


Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\JSONLinter.exe"
  Delete "$INSTDIR\JSONLinter.exe.config"
  Delete "$INSTDIR\ScintillaNET.dll"
  Delete "$INSTDIR\JSONLinter_License.txt"
  Delete "$INSTDIR\jsonlint_License.txt"
  Delete "$INSTDIR\ScintillaNET_License.txt"
  
  Delete "$SMPROGRAMS\JSONLinter\Uninstall.lnk"
  Delete "$SMPROGRAMS\JSONLinter\Website.lnk"
  Delete "$DESKTOP\JSONLinter.lnk"
  Delete "$SMPROGRAMS\JSONLinter\JSONLinter.lnk"

  RMDir "$SMPROGRAMS\JSONLinter"
  RMDir /r "$INSTDIR\node_modules"
  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd

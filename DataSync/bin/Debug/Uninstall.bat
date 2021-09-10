net stop hpCloudDataSync
sc delete hpCloudDataSync binPath= "%~dp0Sync.exe" start= auto
pause
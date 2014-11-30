@echo off
cls

if not exist packages\FAKE\tools\Fake.exe (
    paket restore
)

packages\FAKE\tools\FAKE.exe build.fsx %*
@echo off
cls

if not exist packages\FAKE\tools\Fake.exe (
    .paket\paket.exe restore
)

pushd "docs"

"C:\Program Files (x86)\Microsoft SDKs\F#\3.0\Framework\v4.0\Fsi.exe" --use:generate.fsx --exec --quiet

popd
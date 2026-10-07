# SmartCube Mobile app: shared development

Development of the app lives here (Dad). Production (Dan) pulls from this repo when he decides,
tests, then releases an installer with `release.ps1`. Nothing is pushed back from production.
The full arrangement and the server setup are in the server repo's `README-DEVELOPMENT.md`.

## Opening it

1. Clone this repo somewhere NOT inside OneDrive (long paths and sync locks break the build),
   e.g. `C:\Dev\smartcube-app`.
2. Open `SmartCubeMobile.sln` in Visual Studio 2022 (17.12 or later) with the
   ".NET Multi-platform App UI development" workload installed.
3. Choose the **Windows Machine** target, Debug, and press F5. The first build restores NuGet
   packages and takes a few minutes.

## Pointing it at your dev server

The app talks to the live server by default. On the sign-in screen click the **Server:** line at
the bottom and enter your dev server's address (e.g. `http://localhost:5000`). Blank resets it to
the live server. Your data stays in `%LocalAppData%\SmartCube` on your own PC.

## Keep out of git

No API keys, client secrets or passwords in code. Bank (TrueLayer) token exchange is done by the
server, which holds the client secret in its `appsettings.json`; the app only knows the public
client id.

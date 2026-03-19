

Here's the updated version with the community/clients point added:

---

I had summarized the situation a few times above but if things weren't clear enough, here are the full details now that I've completed all the validation.

**Do our users need to put workarounds in their project?**
No — for `dotnet new unoapp` projects. Those target a single platform at a time and are not affected. In CI, we already build one TFM per leg via `UnoTargetFrameworkOverride`, so CI is also not affected. The workaround Jonathan identified can be absorbed into the Uno SDK, so standard users would never see it.

However, community contributors and clients who have **multi-TFM project setups** (building for multiple platforms in a single `dotnet build/publish`) will hit this bug — it's a .NET SDK issue, not Uno-specific. MAUI and Blazor Hybrid multi-TFM projects are equally affected. Until the .NET team ships a fix, the Uno SDK workaround would shield our users, but anyone outside the Uno ecosystem is exposed.

**What's the bug?**
The .NET SDK ships with a built-in list of runtime packages it thinks exist — but that list still includes Mono desktop packages (`Microsoft.NETCore.App.Runtime.Mono.win-x64`, etc.) that Microsoft stopped publishing since .NET 9 Preview 7. When a project targets multiple platforms at once, NuGet tries to download those ghost packages and fails because they don't exist on NuGet anymore.

**What was I waiting for?**
I wanted to complete the full test matrix myself — validating what Jonathan and Elie each suggested — before sharing definitive answers. Those results are in the table above. Both workarounds are confirmed working, and the root cause is confirmed to be a .NET SDK bug, not an Uno bug.

**Tracking:**
- dotnet/maui#32968
- dotnet/runtime#125434
- dotnet/aspnetcore#64927 (Blazor Hybrid, same error)

Let me know if anything is still unclear.

---
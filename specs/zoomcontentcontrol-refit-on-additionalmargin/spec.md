# Feature: `ZoomContentControl` re-fits/re-centers when `AdditionalMargin` changes

**Area**: `Uno.Toolkit.UI` — `Controls/ZoomContentControl`
**Type**: Bug fix / behavioral consistency

## Summary

`ZoomContentControl.AdditionalMargin` insets the usable viewport — it is subtracted from the viewport in `FitToCanvas` and in the centering math (`ClippedViewportSize`, `PaddedScaledContentSize`). Changing every *other* input that affects the fitted/centered layout (viewport size, content size, content reload) re-runs the auto fit/center pass, but changing `AdditionalMargin` does not. As a result, after a margin change the content stays fitted/centered against the **previous** margin until some unrelated size/zoom change happens to re-fit.

This request makes an `AdditionalMargin` change re-run the auto fit/center pass, consistent with the other layout-affecting inputs.

## Current behavior

`OnAdditionalMarginChanged` only refreshes the scrollbars and raises the outward viewport event:

```csharp
private void OnAdditionalMarginChanged()
{
    UpdateScrollBars();
    ViewportSizeChanged?.Invoke(this, EventArgs.Empty);
}
```

It does **not** call `FitToCanvas` / `CenterContent`. By contrast, `OnViewportSizeChanged`, `OnSizeChanged`, and the content-size/loaded handlers all route through `UpdateScrollDetails()`, which applies `AutoFitToCanvas` → `FitToCanvas()` and `AutoCenterContent` → `CenterContent()`.

### Consequence

With `AutoFitToCanvas="True"` (and/or `AutoCenterContent="True"`), setting or changing `AdditionalMargin` after the content is laid out leaves the content at the zoom/position computed for the *old* margin. When the new margin is larger, the content can extend into (overlap) the region the margin was meant to reserve. A subsequent viewport/content/zoom change corrects it — so the symptom "fixes itself on the next resize," which is the tell-tale of a missing re-fit trigger.

## Expected behavior

Changing `AdditionalMargin` MUST re-run the auto fit/center pass, exactly as a viewport-size change does:

- When `AutoFitToCanvas` is `true`, the content is re-fitted so it fits within the viewport **minus the new margin** (`ZoomLevel` recomputed).
- When `AutoCenterContent` is `true`, the content is re-centered within the viewport minus the new margin.
- When both auto flags are `false`, `ZoomLevel` and position are left unchanged (the margin still affects scrollbars/clipping, but no automatic re-fit is forced — same opt-in contract as size changes).
- Scrollbars continue to update, and `ViewportSizeChanged` is still raised (the usable — clipped — viewport did change).

## Design

A margin change is effectively a change to the usable viewport (`ClippedViewportSize = ViewportSize.Subtract(AdditionalMargin)`), so `OnAdditionalMarginChanged` should mirror `OnViewportSizeChanged`:

```csharp
private void OnAdditionalMarginChanged()
{
    UpdateScrollDetails();               // applies AutoFitToCanvas / AutoCenterContent, then scrollbars
    ViewportSizeChanged?.Invoke(this, EventArgs.Empty);
}
```

`UpdateScrollDetails()` already gates the fit/center on the `AutoFitToCanvas` / `AutoCenterContent` flags, so the opt-in contract is preserved and no new state is introduced. This reuses the single existing "re-apply layout" path rather than duplicating fit/center logic.

## Requirements

- **FR-1** — A change to `AdditionalMargin` re-runs the auto fit/center pass (via `UpdateScrollDetails`).
- **FR-2** — With `AutoFitToCanvas="True"`, increasing `AdditionalMargin` decreases the fitted `ZoomLevel` (content fits the reduced usable viewport); decreasing it increases the fitted `ZoomLevel`.
- **FR-3** — With `AutoFitToCanvas="False"`, an `AdditionalMargin` change does not alter `ZoomLevel` (no forced re-fit).
- **FR-4** — Scrollbars are still updated and `ViewportSizeChanged` is still raised on an `AdditionalMargin` change (no regression).

## Tests

Runtime tests (`src/Uno.Toolkit.RuntimeTests/Tests/`), red/fix/green:

- `When_AdditionalMarginIncreases_AndAutoFit_ThenRefitsToSmallerZoom` — the repro/regression guard for FR-1/FR-2: fit a small content in a fixed viewport, capture the fitted `ZoomLevel`, then set a large `AdditionalMargin` and assert the fitted `ZoomLevel` decreases (fails before the fix — margin change did not re-fit — passes after).
- `When_AdditionalMarginChanges_AndAutoFitDisabled_ThenZoomUnchanged` — guard for FR-3: with `AutoFitToCanvas=false`, a margin change leaves `ZoomLevel` unchanged.

## Out of scope

- No public API additions or changes; `AdditionalMargin`, `AutoFitToCanvas`, and `AutoCenterContent` are unchanged.
- No change to how the fit/center math itself works — only to when it re-runs.

# Lessons

## Documentation ownership

- Public migration guidance should describe release-level behavior without pinning the explanation to an exact prerelease NuGet build. Package provenance belongs in PR and validation notes.
- Inherited APIs do not need a second usage guide in Toolkit. Keep Toolkit setup and control-specific caveats locally, and link shared `BaseTheme` behavior to Uno Themes so changes have one documentation owner.
- Apply documentation ownership decisions to the entire branch diff, including XML summaries. Avoid maintaining lists of inherited members in Toolkit; link to their owner and retain only local integration details.

## Compatibility test ownership

- Test Toolkit's inheritance of upstream token generation against the upstream theme with identical inputs. Keep arithmetic tables and default asset names in the owning Themes tests.
- Reuse Toolkit's existing visual-tree helpers in runtime tests instead of introducing a second traversal implementation.

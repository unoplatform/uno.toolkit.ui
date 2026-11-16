using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uno.Toolkit.RuntimeTests.Tests.Yoga;

/// <summary>
/// The single float-comparison choke point for the ported Yoga conformance corpus.
/// </summary>
/// <remarks>
/// The corpus asserts 17,784 layout values across 26 files, all through this one method.
/// Upstream uses xUnit's <c>Assert.Equal(float, float)</c>, which is exact; this keeps that
/// semantic, because the tier-1 gate is exact-green on desktop and any drift is a real
/// conformance failure worth seeing.
///
/// It exists as a shim rather than a direct <c>Assert.AreEqual</c> call so that per-target
/// float/rounding drift (FR-8, and the rounding-parity risk in the spec) can be given a
/// tolerance in <em>one</em> place if a Skia head ever needs it, instead of 17,784 edits.
/// </remarks>
internal static class YogaAssert
{
	/// <summary>Asserts an exact float match, NaN included.</summary>
	public static void Equal(float expected, float actual) => Assert.AreEqual(expected, actual);
}

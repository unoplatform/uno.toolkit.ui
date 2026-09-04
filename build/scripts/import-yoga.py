#!/usr/bin/env python3
"""
Idempotent import of the Yoga flexbox engine + its conformance corpus from
microsoft/microsoft-ui-reactor into Uno.Toolkit.UI.

Re-run this verbatim on the next upstream sync (after bumping TAG/COMMIT/IMPORTED)
so the adaptation stays a diff and not an archaeology exercise.

See specs/flexpanel-import/spec.md (P1/P2) and src/Uno.Toolkit.UI/Layout/Yoga/README.md.

Usage:
    git clone https://github.com/microsoft/microsoft-ui-reactor.git --branch <tag> /path/to/reactor
    REACTOR_SRC=/path/to/reactor python build/scripts/import-yoga.py [engine|tests|probe|all]

Then build and run the tier-1 corpus. A conformance failure after a sync is an engine
regression, not a test problem.
"""
import os
import re
import sys

UPSTREAM = "microsoft/microsoft-ui-reactor"
TAG = "v0.1.0-preview.13"
COMMIT = "c9191b97c40a2e4d6bcbc72df7714184862b4d36"
IMPORTED = "2026-09-04"

# A clone of the upstream repo, checked out at COMMIT above.
SRC = os.environ.get("REACTOR_SRC")
# This repo. Derived from the script location: build/scripts/ -> repo root.
DST = os.environ.get("TOOLKIT_ROOT") or os.path.dirname(
    os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

ENGINE_SRC_DIR = "src/Reactor/Yoga"
ENGINE_DST_DIR = "src/Uno.Toolkit.UI/Layout/Yoga"
TESTS_SRC_DIR = "tests/Reactor.Tests/YogaGenerated"
TESTS_DST_DIR = "src/Uno.Toolkit.RuntimeTests/Tests/Yoga"

NS_PUBLIC = "Uno.Toolkit.UI"        # the 6 user-facing enums (DP types)
NS_ENGINE = "Uno.Toolkit.UI.Yoga"   # everything else, all internal
NS_TESTS = "Uno.Toolkit.RuntimeTests.Tests.Yoga"

# FlexEnums.cs holds the 6 public enums and lands in the flat toolkit namespace.
# NB: namespace Uno.Toolkit.UI.Layout is NOT available - it collides with the
# existing "public enum Layout" in Uno.Toolkit.UI (Helpers/ResponsiveHelper.cs).
ENGINE_FILES = {
    "AlgorithmUtils.cs": (NS_ENGINE, ["System", "System.Collections.Generic"]),
    "FlexDirectionHelper.cs": (NS_ENGINE, ["System"]),
    "FlexEnums.cs": (NS_PUBLIC, []),
    "LayoutResults.cs": (NS_ENGINE, ["System.Runtime.CompilerServices"]),
    "YogaAlgorithm.cs": (NS_ENGINE, ["System", "System.Threading"]),
    "YogaConfig.cs": (NS_ENGINE, ["System.Diagnostics"]),
    "YogaEnums.cs": (NS_ENGINE, ["System"]),
    "YogaNode.cs": (NS_ENGINE, ["System", "System.Collections.Generic"]),
    "YogaStyle.cs": (NS_ENGINE, ["System", "System.Runtime.CompilerServices"]),
    "YogaValue.cs": (NS_ENGINE, ["System"]),
}

UPSTREAM_NS = "Microsoft.UI.Reactor.Layout"
UPSTREAM_TEST_NS = "Microsoft.UI.Reactor.Tests.YogaGenerated"


def read_lf(path):
    """Read as text with LF endings, so ^/$ anchors are not defeated by CRLF."""
    with open(path, "r", encoding="utf-8-sig", newline="") as f:
        return f.read().replace("\r\n", "\n")


def write_crlf(path, text):
    """The repo is CRLF throughout (.editorconfig: end_of_line = crlf)."""
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "w", encoding="utf-8", newline="\r\n") as f:
        f.write(text)


def header(source_path, readme, extra=""):
    """4-line provenance + license header, prepended to every imported file."""
    return (
        f"// Vendored from {UPSTREAM} @ {TAG} ({COMMIT}), imported {IMPORTED}.\n"
        f"// Source: {source_path} -- {extra}DO NOT HAND-EDIT; see {readme}.\n"
        f"// SPDX-License-Identifier: MIT -- (c) Microsoft Corporation (C# port); "
        f"(c) Facebook, Inc. and its affiliates (Yoga).\n"
        f"// Full license text: THIRD-PARTY-NOTICES.md\n"
        f"\n"
    )


def rename_layout_direction(text):
    """D3b: FlexLayoutDirection.LTR/RTL -> LeftToRight/RightToLeft. Numeric values unchanged."""
    text = re.sub(r"\bFlexLayoutDirection\.LTR\b", "FlexLayoutDirection.LeftToRight", text)
    text = re.sub(r"\bFlexLayoutDirection\.RTL\b", "FlexLayoutDirection.RightToLeft", text)
    # the two declarations inside FlexEnums.cs
    text = re.sub(r"^([ \t]*)LTR = 1,$", r"\1LeftToRight = 1,", text, flags=re.M)
    text = re.sub(r"^([ \t]*)RTL = 2,$", r"\1RightToLeft = 2,", text, flags=re.M)
    return text


def insert_usings(lines, usings):
    """Insert extra using lines above the first existing one, else above the namespace."""
    if not usings:
        return lines
    # a few upstream files already declare some of these - a duplicate is CS0105
    block = [f"using {u};\n" for u in usings if f"using {u};\n" not in lines]
    if not block:
        return lines
    for i, line in enumerate(lines):
        if line.startswith("using ") or line.startswith("namespace "):
            return lines[:i] + block + lines[i:]
    return block + lines


def import_engine():
    out = []
    for name, (ns, usings) in sorted(ENGINE_FILES.items()):
        src_rel = f"{ENGINE_SRC_DIR}/{name}"
        text = read_lf(os.path.join(SRC, *src_rel.split("/")))

        text = text.replace(f"namespace {UPSTREAM_NS};", f"namespace {ns};")
        # the upstream self-using becomes the (meaningful) reference to the enum namespace
        text = text.replace(f"using {UPSTREAM_NS};", f"using {NS_PUBLIC};")
        if ns == NS_PUBLIC:
            text = text.replace(f"using {NS_PUBLIC};\n", "")  # would be a self-using
        # stale prose references to the upstream namespace, inside comments
        text = text.replace(UPSTREAM_NS, ns)
        text = rename_layout_direction(text)

        lines = insert_usings(text.splitlines(keepends=True), usings)
        text = header(src_rel, "Layout/Yoga/README.md") + "".join(lines)

        write_crlf(os.path.join(DST, *ENGINE_DST_DIR.split("/"), name), text)
        out.append((name, ns, len(text.splitlines())))
    return out


def import_tests(only=None):
    src_dir = os.path.join(SRC, *TESTS_SRC_DIR.split("/"))
    names = sorted(n for n in os.listdir(src_dir) if n.endswith(".cs"))
    if only:
        names = [n for n in names if n in only]

    out = []
    for name in names:
        src_rel = f"{TESTS_SRC_DIR}/{name}"
        text = read_lf(os.path.join(src_dir, name))

        # xUnit -> MSTest, upstream namespaces -> ours. Emit the using block in repo order.
        text = text.replace(
            f"using {UPSTREAM_NS};\nusing Xunit;\n",
            "using Microsoft.VisualStudio.TestTools.UnitTesting;\n"
            f"using {NS_PUBLIC};\n"
            f"using {NS_ENGINE};\n",
        )
        text = text.replace(f"namespace {UPSTREAM_TEST_NS};", f"namespace {NS_TESTS};")
        # 46 of the 590 cases are GTEST_SKIP'd in Yoga's own C++ suite and arrive
        # already disabled. [Ignore] mirrors that upstream state - it does not
        # deactivate anything of ours. See Tests/Yoga/README.md.
        text = re.sub(
            r"^([ \t]*)\[Fact\(Skip = (\"[^\"]*\")\)\]$",
            r"\1[TestMethod]\n\1[Ignore(\2)]",
            text,
            flags=re.M,
        )
        text = re.sub(r"^([ \t]*)\[Fact\]$", r"\1[TestMethod]", text, flags=re.M)
        # xUnit has no class attribute; MSTest needs one
        text = re.sub(r"^public class (\w+)$", r"[TestClass]\npublic class \1", text, flags=re.M)
        # single choke point for float comparison - see YogaAssert.cs
        text = text.replace("Assert.Equal(", "YogaAssert.Equal(")
        text = rename_layout_direction(text)

        text = header(src_rel, "Tests/Yoga/README.md", extra="xUnit->MSTest; ") + text
        write_crlf(os.path.join(DST, *TESTS_DST_DIR.split("/"), name), text)
        out.append((name, text.count("[TestMethod]"), text.count("[Ignore("), text.count("YogaAssert.Equal(")))
    return out


# Residue the transform is supposed to have removed. LTR/RTL is checked on code lines
# only - "Apply RTL transformation" in a doc comment is prose, not a missed rename.
RESIDUE = (
    (r"Microsoft\.UI\.Reactor", "upstream namespace"),
    (r"\bXunit\b", "xUnit reference"),
    (r"\[Fact", "xUnit [Fact] - note [Fact(Skip = ...)] is a distinct form"),
    (r"^(?!\s*//).*\b(?:LTR|RTL)\b", "un-renamed LTR/RTL in code"),
    (r"(?<!Yoga)Assert\.Equal\(", "un-shimmed Assert.Equal"),
)


def verify():
    """Check only the imported files - hand-written companions (YogaAssert.cs) are ours."""
    problems = []
    upstream_tests = set(os.listdir(os.path.join(SRC, *TESTS_SRC_DIR.split("/"))))
    roots = [
        (os.path.join(DST, *ENGINE_DST_DIR.split("/")), set(ENGINE_FILES)),
        (os.path.join(DST, *TESTS_DST_DIR.split("/")), upstream_tests),
    ]
    for root, imported in roots:
        if not os.path.isdir(root):
            continue
        for name in sorted(n for n in os.listdir(root) if n in imported):
            text = read_lf(os.path.join(root, name))
            for pattern, label in RESIDUE:
                hits = len(re.findall(pattern, text, flags=re.M))
                if hits:
                    problems.append(f"{name}: {hits}x {label}")
            if not text.startswith("// Vendored from "):
                problems.append(f"{name}: missing provenance header")
    return problems


if __name__ == "__main__":
    if not SRC:
        sys.exit(f"REACTOR_SRC is not set - point it at a clone of {UPSTREAM} at {COMMIT}.")
    mode = sys.argv[1] if len(sys.argv) > 1 else "all"

    if mode in ("engine", "all"):
        for name, ns, n in import_engine():
            print(f"  engine {name:<24} -> {ns:<22} ({n} lines)")

    if mode == "probe":
        for name, tests, skipped, asserts in import_tests(only={"YogaDimensionTest.cs"}):
            print(f"  test   {name:<32} {tests} tests ({skipped} skipped), {asserts} asserts")
    elif mode in ("tests", "all"):
        total_t = total_s = total_a = 0
        for name, tests, skipped, asserts in import_tests():
            print(f"  test   {name:<32} {tests:>3} tests ({skipped:>2} skipped), {asserts:>5} asserts")
            total_t, total_s, total_a = total_t + tests, total_s + skipped, total_a + asserts
        print(f"  TOTAL: {total_t} test methods ({total_s} skipped upstream, "
              f"{total_t - total_s} active), {total_a} assertions")

    problems = verify()
    if problems:
        print("\nVERIFY FAILED:")
        for p in problems:
            print(f"  {p}")
        sys.exit(1)
    print("\nverify: clean")

#!/bin/bash
set -x
set -euo pipefail
IFS=$'\n\t'

if [[ -z "${BUILD_SOURCESDIRECTORY:-}" ]]; then
    echo "BUILD_SOURCESDIRECTORY environment variable is required." >&2
    exit 1
fi

if [[ -z "${SamplesAppArtifactPath:-}" ]]; then
    echo "SamplesAppArtifactPath environment variable is required." >&2
    exit 1
fi

build_root="${BUILD_SOURCESDIRECTORY}/build"
results_path="${build_root}/wasm-runtime-tests-results.xml"
logs_dir="${build_root}/wasm-runtime-tests-logs"
runner_log_path="${logs_dir}/wasm-runtime-tests-runner.log"
tool_path="${build_root}/.dotnet-tools"
playwright_root="${build_root}/playwright-browsers"
runner_version="${UNO_WASM_RUNTIME_TESTS_RUNNER_VERSION:-2.0.0-dev.32}"

mkdir -p "${build_root}" "${tool_path}" "${playwright_root}" "${logs_dir}"

if [[ ! -d "${SamplesAppArtifactPath}" ]]; then
    echo "Samples app artifact directory '${SamplesAppArtifactPath}' was not found." >&2
    exit 1
fi

if dotnet tool list --tool-path "${tool_path}" 2>/dev/null | grep -q "uno.ui.runtimetests.engine.wasm.runner"; then
    dotnet tool update --tool-path "${tool_path}" Uno.UI.RuntimeTests.Engine.Wasm.Runner --version "${runner_version}"
else
    dotnet tool install --tool-path "${tool_path}" Uno.UI.RuntimeTests.Engine.Wasm.Runner --version "${runner_version}"
fi

export PATH="${tool_path}:${PATH}"
export PLAYWRIGHT_BROWSERS_PATH="${playwright_root}"

pushd "${SamplesAppArtifactPath}" >/dev/null
npx playwright install chromium
popd >/dev/null

{
    echo "=== wasm runtime tests (runner log) ==="
    echo "Started: $(date -u +"%Y-%m-%dT%H:%M:%SZ")"
    echo "App path: ${SamplesAppArtifactPath}"
    echo "Results path: ${results_path}"
    echo "Runner version: ${runner_version}"
    echo "Playwright browsers path: ${PLAYWRIGHT_BROWSERS_PATH}"
    echo "======================================"
} | tee "${runner_log_path}"

uno-runtimetests-wasm \
    --app-path "${SamplesAppArtifactPath}" \
    --output "${results_path}" \
    --timeout 600 \
    --query-param "mode=rt" \
    --query-param "UNO_RUNTIME_TESTS_RUN_TESTS={}" \
    --query-param "DOTNET_MODIFIABLE_ASSEMBLIES=debug" \
    --browser-log-level verbose \
    2>&1 | tee -a "${runner_log_path}"

if [[ ! -f "${results_path}" ]]; then
    echo "Runtime tests did not produce results at ${results_path}" >&2
    exit 1
fi

export WASM_RUNTIME_TEST_RESULTS="${results_path}"

python3 - <<'PY'
import os
import xml.etree.ElementTree as ET

results_path = os.environ["WASM_RUNTIME_TEST_RESULTS"]

if not os.path.exists(results_path):
    raise SystemExit(f"Runtime tests results not found at {results_path}")

try:
    tree = ET.parse(results_path)
except ET.ParseError as exc:
    raise SystemExit(f"Unable to parse runtime tests results: {exc}")

cases = tree.findall('.//test-case')
if not cases:
    raise SystemExit("Runtime tests produced no test cases.")

failed = []
for case in cases:
    result = case.get("result")
    if result and result != "Passed":
        failed.append((case.get("name"), result))

print(f"Validated {len(cases)} wasm runtime test cases.")
if failed:
    print(f"Failed/inconclusive tests: {len(failed)}")
    for name, result in failed:
        print(f" - {name}: {result}")
PY

## Agent skills

### Build baseline

The build enforces the architecture rules; the settings live in `Directory.Build.props`,
`Directory.Build.targets`, `Directory.Packages.props` and `.editorconfig`. Three stages, matching CI
(`.github/workflows/main.yml`):

| Stage  | Command                                                                    |
| ------ | -------------------------------------------------------------------------- |
| build  | `dotnet build Gradio.Net.sln --configuration Release`                       |
| test   | `dotnet test tests/Gradio.Net.Tests/Gradio.Net.Tests.csproj --configuration Release` |
| pack   | `dotnet pack src/Gradio.Net/Gradio.Net.csproj --configuration Release --output out` |

Invariants enforced by the build or the tests:

- Versions come from `eng/Versions.props` and nowhere else;
  `tests/Gradio.Net.Tests/Versioning` fails if any other MSBuild file declares one.
- The vendored frontend under `src/upstream/frontend` and the vendored SSR runtime under
  `src/upstream/ssr` are the Python version's compiled artifacts, byte for byte. Never edit them by hand.
- Every other checked-in declaration of the Python release — the component parameter manifest,
  the export list and the event list under `src/upstream/` — names the release in
  `eng/Versions.props`, and every one that records the Python wheel records the same wheel.
  `tests/Gradio.Net.Tests/Versioning` fails otherwise, so an upgrade cannot stop half way.
- No static mutable state, no `#pragma warning disable`, no `#nullable disable`
  (GRADIO0001–GRADIO0003, `src/Gradio.Net.Analyzers`). The three analyzers skip generated code,
  which is the one place a pragma may appear: the factory generator emits
  `#pragma warning disable CS8981` for the lower-case type name it must declare, because a rule's
  severity in `.editorconfig` does not reach a source file the compiler never read from disk.
- Every type in the package is in one namespace, `Gradio.Net`; the folders group the code by
  concern, and `tests/Gradio.Net.Tests/Architecture` asserts what the folders must not depend on.
- Warnings are errors, and nullable analysis is on for every project.

# Gradio.Net

## 0.6.30.5

### Bug Fixes

* A handler may declare a multi-select value as `object[]` or `List<object>`, on either component
  that produces one. `Dropdown.Preprocess` reads a `JsonArray` through `JsonValues.ToObject`, which
  answers `object[]`; `CheckboxGroup.Preprocess` built a `List<object?>` while it checked the
  choices and answered that list for `Type = "value"`, while its `Type = "index"` branch already
  answered an array. `Delegate.DynamicInvoke` accepts an argument only when it is an instance of the
  declared parameter type, so a `List<object>` handler on a multiple dropdown and an `object[]`
  handler on a checkbox group were never called — the run answered "Error" instead. The checkbox
  group now answers an array in both branches, and the run builds the concrete container a handler
  declared from the list the component produced before the call. The list interfaces need no
  conversion, because both containers implement `IReadOnlyList<object?>`, which is what the typed
  `CheckboxGroup.Change` handler takes. Numbers are converted as before; the entries of a list are
  not, so a handler declaring `List<string>` still reports the mismatch rather than silently
  receiving objects. A handler that declares `object` and casts a checkbox group's value to
  `List<object>` now reads an array; declaring the container is what the conversion supports.
  Four end-to-end runs hold the two components against the two declarations, one holds the typed
  checkbox group path, and a kernel test holds both components to the same container.

### Documentation

* The `clearBtn` remarks on `Interface`, `gr.Interface` and `ButtonSpec.None` said `null` hides the
  clear button. `null` is the omitted argument and draws the default "Clear" button;
  `ButtonSpec.None` spells the Python version's `clear_btn=None`, which draws the same button hidden
  with its two bindings still attached. `InterfaceTests` now compares the hidden button's props and
  bindings with the drawn one.

## 0.6.30.4

### Features

* The package also targets `net11.0`, alongside `net8.0`, `net9.0` and `net10.0`. The framework
  list lives in `src/Gradio.Net/Gradio.Net.csproj`; the demos and the test suite keep targeting
  `net10.0`, which is the runtime the test host runs on. CI installs the 11 SDK beside the 10 one,
  because a `net11.0` target cannot be built by an older SDK. The new target also needed one
  documentation fix: .NET 11 adds an overload of `CryptographicOperations.FixedTimeEquals`, so the
  `cref` in `GradioAuth`'s remarks became ambiguous — and warnings are errors in this repository.

### Bug Fixes

* Clicking a row of an example gallery fills every input with its own value. The gallery itself
  was right — the config carried both columns and both rows — but the click answered with the whole
  row of updates as a single value, so the first field showed `[object Object],[object Object]`,
  every other field was cleared and the result was left alone. In the Python version
  `Examples.create()` returns the list of `gr.update` payloads and `Blocks.postprocess_data` reads
  `predictions[i]` as the value for `outputs[i]`; in this port a plain `JsonArray` is a value, not a
  result shape, and `EventValues` is the type the queue spreads across outputs. The gallery's
  handlers now answer with that type, and their outputs are the inputs that have examples, which
  is the Python version's `inputs_with_examples` rather than the narrower set of columns the table
  draws. `_UNCHANGED` also survives postprocessing now, so a column the gallery did not draw keeps
  what the user typed instead of being cleared. Two regressions hold it: the click's endpoint answers
  one update per input, and a real browser click on the second row fills both fields — the single
  input case is kept beside it so the two shapes cannot trade places.

## 0.6.30.3

### Bug Fixes

* A step chained with `Then`, `Success` or `Failure` runs once. The queue admitted the steps that
  follow a binding as well as the frontend did, so a step whose inputs were the binding's outputs
  ran twice: the second run overwrote the first, and a handler that counts its calls saw two. The
  chain belongs to the frontend, as it does in the Python version — the client dispatches a step
  whose `trigger_after` names the binding that just finished, and reads that step's own `inputs`
  from the page, which is also what lets a step read a component the step before it never wrote.
  A step that read such a component was skipped by the queue and never reached the server at all,
  so the two halves disagreed about which steps existed as well as about how often they ran.

## 0.6.30.2

### Features

* `gr.Workflow()` exists. The class, its graph-file resolution and its canvas were already here; the
  factory Python spells `gr.Workflow` was not.

* `gr.SetStaticPaths()` exists, the top-level spelling of `Blocks.AllowedPaths` — the prop the file
  endpoint already consulted.

* The job-style routes answer. `/gradio_api/call/{api_name}` and `/call/v2/{api_name}` were in the
  route table and replied "not implemented"; they now submit a run and stream its result, reusing
  the queue's own stream. The body may be the queue's `{"data": [...]}` or the named parameters
  `openapi.json` documents.

* `gr.Info`, `gr.Warning`, `gr.Error` and `gr.Success` reach the page. A run sets an ambient scope,
  and a handler's callout inside it travels to the session watching that run as a `log` message.

* `Blocks.Head` and `Blocks.Js` can be set. The config carried both fields already; nothing wrote
  them.

* `Then`, `Success` and `Failure` run. A chained step was described in the config and never
  executed — and its binding had lost its handler, inputs and outputs on the way. It runs now, on
  success and on failure, and reads its own declared inputs.

### Bug Fixes

* `ApiVisibility.Private` is private. Its documentation says "not callable by clients" and it was
  callable; the direct routes now refuse it. The queue does not, because the page is not a client.

* `/info`'s shell snippet names routes that exist. It had named two that did not.

* A chained step the queue refuses is logged instead of leaving no trace at all.

* `/call` bounds the runs it remembers. It kept them without a limit.

## 0.6.30.1

### Bug Fixes

* A layout's events bind the way a component's do. `tabs.Select(fn, inputs, outputs)`,
  `tab.Select(...)`, `accordion.Expand(...)`, `sidebar.Collapse(...)`, `walkthrough.Select(...)` and
  `step.Select(...)` are bindings now, generated from the recorded Python events like every
  component's. The triggers moved under `Events` — `tabs.Events.Select` — which is where a
  component's triggers already lived, because C# cannot have a property and a method under one
  name. Before this, a layout's signal could only be reached through `gr.On`.

* `gr.Update` can select a tab, which is the Python version's `gr.update(selected="Tab 2")`:
  `new Update { Selected = "Two" }` on a `Tabs` or a `Walkthrough` output. The prop was missing, so
  a handler had no way to move the page to another tab.

* `gr.Update` takes any prop the Python version's `gr.update` takes. The record names the props a
  handler changes often, and `Props` carries the rest by the name the config gives them — the
  keyword splat, in the form C# has for one. A key that is present is written even when its value
  is null, so "empty this prop" is expressible.

* The Vibe editor's prompts are LF on every operating system. A raw string literal spells the line
  endings of the file it is written in, and the repository stores LF while a Windows working copy
  checks it out as CRLF — so on that platform every prompt the editor sent differed from the one
  the Python version sends, and `VibeTests` failed there alone. The literals are normalized where
  they are declared; the code and the transcript put into them are carried as they came, which is
  the line the Python version's tokenizer draws too.

## 0.6.30

### Features

* Port gradio 6.30.0

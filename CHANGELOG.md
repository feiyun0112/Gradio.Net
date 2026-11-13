# Gradio.Net

## 0.6.30.8

### Bug Fixes

* A handler declared as a local function is served under the name it was written under, instead of
  `lambda`. The compiler renames one — `string Summary(string text) => …` in the top-level
  statements becomes a method called `<<Main>$>g__Summary|0_0` — and the port read "not a legal
  identifier" as "no name at all", so every local function became `lambda` and then took a number
  among the lambdas. The Python version reads `fn.__name__`, which a function declared inside
  another one keeps, so `gr.Button().click(Summary)` is served as `Summary` there. `Blocks.Events`
  now reads the declared name out of the compiler's spelling — between the `g__` marker and the
  `|` that precedes the ordinal — and falls back to `lambda` only for a method the compiler
  generated with no such marker, which is a lambda or an anonymous method. `ChatInterface` names
  its own handlers by that same rule instead of by a second copy of it, so a chat handler and a
  button handler over one method are served under one name. A test holds both halves: the local
  function keeps its name, the lambda stays `lambda`.

## 0.6.30.7

### Bug Fixes

* The job-style result route writes the named server-sent events the Python version writes, and the
  OpenAPI document describes. `GET /call/{api_name}/{event_id}` — and its `/call/v2/` spelling —
  used to forward the queue's own stream: every frame was `data: {"msg": ...}` with the kind inside
  the JSON, so a caller that waited for `event: complete` waited until the connection closed. The
  Python version serves both GET routes from `simple_predict_get`, whose `process_msg` maps a
  `ProcessCompletedMessage` to `event: complete` (or `event: error`), a `ProcessGeneratingMessage`
  to `event: generating`, a heartbeat to `event: heartbeat` with `data: null`, and an unexpected
  error to `event: error`; `queue_data_helper` skips every message whose answer is `None`. `JobStream`
  holds that mapping, and the shared session stream now takes a formatter: `/queue/data` keeps the
  browser's `data: {json}` framing untouched, while the two result routes write the named events
  and drop estimates, starts, progress reports, log lines and `close_stream`. On success `data` is
  the run's `output.data`; on failure it is the whole `output`, which is what says what went wrong.
  Kernel tests hold every frame and every skipped kind, and four end-to-end runs hold the plain,
  v2 and streaming paths plus the named error an unknown event id gets.

* The queue's whole-application ceiling is no longer a constant the application cannot change.
  Python's `launch(max_threads=40)` sets `Blocks.max_threads` and the queue's own
  `max_thread_count`; this port started its queue with a hard-coded 40 worker slots. The standalone
  entry point now takes `Launch(maxThreads: ...)` through `GradioServerOptions.MaxThreads`, and an
  integrated host names the same number in `AddGradio`'s `GradioCoreOptions.MaxThreads`; the hosted
  service reads it when the host starts, as Python's `launch` does, and clamps a value below one to
  one slot. A binding's own `concurrencyLimit` still applies inside the ceiling. Four tests read the
  queue the host actually started and hold the default, both entry points and the clamp.

* A validator that answers with a string now refuses with that sentence. The returned string was
  written through `ToJsonString()` on its way into the endpoint's `detail`, which added a second pair
  of quotes and turned Chinese into `\u` escapes — the person on the page read a JSON literal rather
  than the reason. The Python version's `gr.validate(is_valid, message)` carries the same text in its
  `message` field, so a string is now passed through as text; an object or a number, which has no
  text of its own, keeps its JSON spelling. An end-to-end test holds the readable Chinese refusal
  and the accepting path beside it.

### Documentation

* `Tabs.Selected`, `Update.Selected` and `Tab.TabId` said a tab could be selected by its label or
  its id. The Python version's `Tabs(selected=...)` says the value "must correspond to an id passed
  to one of the child TabItems", and `Tab(id=...)` calls the id "required if you wish to control the
  selected tab from a predict function"; the frontend matches the id and never the label, so a label
  selects no tab at all and the page draws an empty body without an error. The remarks now say so,
  and `Update.Selected` names the walkthrough's numbered steps as the one case where a number is
  the value.

## 0.6.30.6

### Bug Fixes

* `gr.Info`, `gr.Warning`, `gr.Error` and `gr.Success` are drawn on the page. The queue did send a
  `log` message for each of them, but it wrote the callout's level into `log` and its sentence into
  `message`, and omitted `level`, `title` and `visible` altogether. The vendored frontend's
  `handle_log` reads `log` for the text, `level` for the kind, and `title`, `visible` and `duration`
  beside them; the Python version's `LogMessage` declares exactly those fields, in that order, with
  `duration = 10` and `visible = True` as defaults. A notification built the old way arrived with no
  text and no kind, so the page drew nothing. `QueueMessages.Log` now writes the Python model field
  for field, including the default title each level gets (`Info`, `Warning`, `Error`, `Success`) and
  the ten-second default duration, and no longer writes `message`. A field-by-field kernel test, a
  theory over the four levels and an end-to-end test that reads the actual SSE stream hold the shape
  and the delivery in place.

* `timeLimit` now has the meaning the Python version gives it. It is not a deadline for an ordinary
  handler and not a deadline for a run waiting in the queue — Python lets both finish — it is the
  budget of a run the browser feeds chunk by chunk over `/stream/{event_id}`. The queue now keeps
  `Event.run_time` and the first call's `first_iteration`: the first call spends part of the budget,
  each wait for a chunk and the call it feeds add to `run_time`, and a run that has spent its limit
  stops generating after the chunk it just produced. A `stream`-connected run's chunks are sent as
  `process_streaming` rather than `process_generating`, with `time_limit` set to
  `fn.time_limit - first_iteration`; a run the queue advances on its own keeps `process_generating`
  with no limit, because the client has nothing to send. `StreamingOutputs.WaitOf` waits
  `(fn.time_limit or 30) - first_iteration`, so a limit of zero falls back to the Python default
  and a first call that spent the whole budget leaves no wait. Kernel tests hold the arithmetic and
  the two message shapes; the rest of the suite holds the ordinary handler and queue paths against
  a hard timeout that was never part of the contract.

### Documentation

* `Draggable` keeps no public entry point, and now says why. The Python wheel ships the class in
  `gradio.layouts.draggable` but does not list `Draggable` among `gradio`'s top-level exports, so
  there is no `gr.Draggable()` for this port to mirror. The type stays public so the frontend's
  component description can be produced from it; its constructor stays `internal`, because a layout
  is built by a factory that also enters its render scope and this layout has no such factory.
  Adding one would put a name on `gr` that Python does not have. `DraggableSurfaceTests` holds all
  four of those facts — the export list, the layer's members, the constructor's visibility and the
  wheel's declaration of the class — so a future release cannot quietly add the factory or remove
  the type.

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

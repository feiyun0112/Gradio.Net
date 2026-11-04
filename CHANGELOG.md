# Gradio.Net

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

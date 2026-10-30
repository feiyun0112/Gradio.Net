using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The graph a workflow is, and what is derived from it: which nodes feed an output, which values a
/// caller supplies, which outputs one pipeline produces, and how a graph is run.
/// </summary>
/// <remarks>
/// These are the parts of a workflow that are not about blocks at all, so they are asserted without
/// a host: a graph is a document, and everything the application draws from it is a function of
/// that document. What is drawn is asserted against the Python version's own config by the suite;
/// what is derived is asserted here, where a failure says which derivation is wrong.
/// </remarks>
public sealed class WorkflowGraphTests
{
    [Fact]
    public void A_graph_reads_its_three_roles_and_its_edges()
    {
        var graph = WorkflowGraph.FromJson(Pipeline)!;

        Assert.Equal("Shout pipeline", graph.Name);
        Assert.Equal(["prompt", "suffix"], graph.References.Select(node => node.Id));
        Assert.Equal(["shout"], graph.Operators.Select(node => node.Id));
        Assert.Equal(["result", "echo"], graph.Subjects.Select(node => node.Id));
        Assert.Equal(4, graph.Edges.Count);

        // A node keeps the fields this build does not model, because a graph is the canvas's
        // document and a field a later canvas understands is still the author's.
        Assert.Equal("fn", graph.NodeOf("shout")!.Kind);
        Assert.Equal("shout", graph.NodeOf("shout")!.Fn);
        Assert.Equal("text", graph.NodeOf("shout")!.Inputs[0].Type);
    }

    [Fact]
    public void Only_the_current_schema_is_a_saved_graph()
    {
        // The canvas writes version 2. A version 1 file is the draft an application generates from
        // its bound functions, which is not a graph with outputs to expose.
        Assert.Null(WorkflowGraph.FromJson("""{"version": "1", "nodes": []}"""));
        Assert.Null(WorkflowGraph.FromJson("not json"));
        Assert.Null(WorkflowGraph.FromJson(null));
        Assert.Null(WorkflowGraph.FromJson("""{"schema_version": "2", "references": [{"label": "no id"}]}"""));
        Assert.NotNull(WorkflowGraph.FromJson("""{"schema_version": "2"}"""));
    }

    [Fact]
    public void A_node_id_that_names_two_nodes_is_refused()
    {
        var error = Assert.Throws<ArgumentException>(() => new WorkflowGraph(new JsonObject
        {
            ["schema_version"] = "2",
            ["references"] = new JsonArray(new JsonObject { ["id"] = "same" }),
            ["operators"] = new JsonArray(new JsonObject { ["id"] = "same" }),
        }));

        Assert.Contains("duplicate node id: same", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void An_edge_must_name_all_four_of_its_ends()
    {
        var error = Assert.Throws<ArgumentException>(() => new WorkflowGraph(new JsonObject
        {
            ["schema_version"] = "2",
            ["edges"] = new JsonArray(new JsonObject
            {
                ["id"] = "e1",
                ["from_node_id"] = "a",
                ["from_port_id"] = "out",
            }),
        }));

        Assert.Contains("missing to_node_id", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void The_nodes_that_feed_an_output_come_before_it()
    {
        var graph = WorkflowGraph.FromJson(Pipeline)!;

        // The set is what feeds the output; the order is the list's, because a set has none. The
        // executor passes its nodes in declaration order, and the two sources here are free to be
        // visited in either order.
        var upstream = graph.UpstreamNodeIds("result");
        Assert.True(upstream.SetEquals(["prompt", "suffix", "shout", "result"]));

        var order = graph.TopologicalOrder(["prompt", "suffix", "shout", "result"]);

        Assert.Equal(["prompt", "suffix", "shout", "result"], order);
    }

    [Fact]
    public void A_cycle_cannot_be_ordered()
    {
        var graph = WorkflowGraph.FromJson("""
            {
              "schema_version": "2",
              "operators": [
                {"id": "a", "kind": "fn", "inputs": [{"id": "in", "type": "text"}], "outputs": [{"id": "out", "type": "text"}]},
                {"id": "b", "kind": "fn", "inputs": [{"id": "in", "type": "text"}], "outputs": [{"id": "out", "type": "text"}]}
              ],
              "edges": [
                {"id": "e1", "from_node_id": "a", "from_port_id": "out", "to_node_id": "b", "to_port_id": "in"},
                {"id": "e2", "from_node_id": "b", "from_port_id": "out", "to_node_id": "a", "to_port_id": "in"}
              ]
            }
            """)!;

        var error = Assert.Throws<WorkflowExecutionException>(
            () => graph.TopologicalOrder(["a", "b"]));

        Assert.Contains("cycle", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_reference_nothing_feeds_is_a_value_the_caller_supplies()
    {
        var graph = WorkflowGraph.FromJson(Pipeline)!;

        var inputs = graph.GroupFreeInputs(graph.Groups()[0].Subjects);

        // Declaration order, not dependency order: these become the endpoint's parameters, and
        // reordering them would change how a client that already calls it has to call it.
        Assert.Equal(["Prompt", "Suffix"], inputs.Select(input => input.Label));
        Assert.Equal(["text", "text"], inputs.Select(input => input.Type));
        Assert.Equal(["prompt", "suffix"], inputs.Select(input => input.Node.Id));
    }

    [Fact]
    public void A_reference_the_graph_feeds_is_not_a_value_the_caller_supplies()
    {
        var graph = WorkflowGraph.FromJson("""
            {
              "schema_version": "2",
              "references": [
                {"id": "seed", "label": "Seed", "outputs": [{"id": "out", "type": "text"}]},
                {"id": "copy", "label": "Copy",
                 "inputs": [{"id": "in", "type": "text"}],
                 "outputs": [{"id": "out", "type": "text"}]}
              ],
              "subjects": [
                {"id": "shown", "label": "Shown", "inputs": [{"id": "in", "type": "text"}]}
              ],
              "edges": [
                {"id": "e1", "from_node_id": "seed", "from_port_id": "out", "to_node_id": "copy", "to_port_id": "in"},
                {"id": "e2", "from_node_id": "copy", "from_port_id": "out", "to_node_id": "shown", "to_port_id": "in"}
              ]
            }
            """)!;

        var inputs = graph.GroupFreeInputs(graph.Groups()[0].Subjects);

        // The computed reference is a relay, not a parameter: the caller supplies the seed and the
        // graph copies it, which is what an author who wired one reference into another asked for.
        Assert.Equal(["seed"], inputs.Select(input => input.Node.Id));
    }

    [Fact]
    public void Outputs_that_share_work_are_one_pipeline()
    {
        var graph = WorkflowGraph.FromJson(Pipeline)!;

        var groups = graph.Groups();

        Assert.Single(groups);
        Assert.Equal(["result", "echo"], groups[0].Subjects.Select(subject => subject.Id));

        // The endpoint is named after the first output, which is what a caller reads first.
        Assert.Equal("result", groups[0].ApiName);
    }

    [Fact]
    public void Outputs_that_share_nothing_are_separate_pipelines_with_separate_names()
    {
        var graph = WorkflowGraph.FromJson("""
            {
              "schema_version": "2",
              "subjects": [
                {"id": "one", "label": "Result", "inputs": [{"id": "in", "type": "text"}]},
                {"id": "two", "label": "Result", "inputs": [{"id": "in", "type": "text"}]}
              ]
            }
            """)!;

        var groups = graph.Groups();

        Assert.Equal(2, groups.Count);

        // Two pipelines whose outputs happen to share a label still need two addresses, so the
        // second is made unique the way the Python version makes it unique.
        Assert.Equal(["result", "result_"], groups.Select(group => group.ApiName));
    }

    [Fact]
    public void An_endpoint_describes_what_it_takes_and_what_it_answers()
    {
        var graph = WorkflowGraph.FromJson(Pipeline)!;

        var endpoint = Assert.Single(graph.DescribeEndpoints());

        Assert.Equal("result", endpoint.ApiName);
        Assert.Equal("Result", endpoint.Label);
        Assert.Equal(["in_0", "in_1"], endpoint.Parameters.Select(parameter => parameter.ParameterName));
        Assert.Equal(["str", "str"], endpoint.Parameters.Select(parameter => parameter.PythonType));
        Assert.Equal(["Result", "Echo"], endpoint.Returns.Select(value => value.Label));
    }

    [Theory]
    [InlineData("Result", "result")]
    [InlineData("My Output", "my_output")]
    [InlineData("  padded  ", "padded")]
    [InlineData("a--b__c", "a_b_c")]
    [InlineData("!!!", "endpoint")]
    [InlineData(null, "endpoint")]
    [InlineData("image_2", "image_2")]
    public void A_label_becomes_a_name_an_endpoint_can_carry(string? label, string expected) =>
        Assert.Equal(expected, WorkflowGraph.Slugify(label));

    [Fact]
    public async Task A_pipeline_runs_its_nodes_in_the_order_they_depend_on_each_other()
    {
        var graph = WorkflowGraph.FromJson(Pipeline)!;
        var calls = new List<string>();

        var executor = new WorkflowExecutor(graph, new Dictionary<string, WorkflowOperator>(StringComparer.Ordinal)
        {
            ["fn"] = (node, arguments, _) =>
            {
                calls.Add(node.Fn!);

                return ValueTask.FromResult<IReadOnlyList<object?>>(
                    [string.Concat(arguments.Select(argument => argument?.ToString()))]);
            },
        });

        var result = await executor.RunAsync(
            "result",
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["prompt"] = "hello",
                ["suffix"] = "!",
            },
            TestContext.Current.CancellationToken);

        Assert.Equal(["shout"], calls);
        Assert.Equal("hello!", result);
    }

    [Fact]
    public async Task Several_outputs_of_one_pipeline_each_answer_with_what_their_path_produced()
    {
        var graph = WorkflowGraph.FromJson(Pipeline)!;

        var executor = new WorkflowExecutor(graph, new Dictionary<string, WorkflowOperator>(StringComparer.Ordinal)
        {
            ["fn"] = (_, _, _) => ValueTask.FromResult<IReadOnlyList<object?>>(["SHOUT"]),
        });

        var results = await executor.RunManyAsync(
            ["result", "echo"],
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["prompt"] = "hello",
                ["suffix"] = "!",
            },
            TestContext.Current.CancellationToken);

        // Two answers in one run: what the operator produced, and what the reference relayed.
        Assert.Equal(["SHOUT", "hello"], results);
    }

    [Fact]
    public async Task An_operator_that_feeds_two_outputs_runs_once()
    {
        var graph = WorkflowGraph.FromJson("""
            {
              "schema_version": "2",
              "references": [
                {"id": "seed", "label": "Seed", "outputs": [{"id": "out", "type": "text"}]}
              ],
              "operators": [
                {"id": "expand", "kind": "fn", "fn": "expand", "label": "Expand",
                 "inputs": [{"id": "in", "type": "text"}],
                 "outputs": [{"id": "out_0", "type": "text"}]}
              ],
              "subjects": [
                {"id": "first", "label": "First", "inputs": [{"id": "in", "type": "text"}]},
                {"id": "second", "label": "Second", "inputs": [{"id": "in", "type": "text"}]}
              ],
              "edges": [
                {"id": "e1", "from_node_id": "seed", "from_port_id": "out", "to_node_id": "expand", "to_port_id": "in"},
                {"id": "e2", "from_node_id": "expand", "from_port_id": "out_0", "to_node_id": "first", "to_port_id": "in"},
                {"id": "e3", "from_node_id": "expand", "from_port_id": "out_0", "to_node_id": "second", "to_port_id": "in"}
              ]
            }
            """)!;

        var calls = 0;
        var executor = new WorkflowExecutor(graph, new Dictionary<string, WorkflowOperator>(StringComparer.Ordinal)
        {
            ["fn"] = (_, _, _) =>
            {
                calls++;

                return ValueTask.FromResult<IReadOnlyList<object?>>(["EXPANDED"]);
            },
        });

        var results = await executor.RunManyAsync(
            ["first", "second"],
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["seed"] = "hello" },
            TestContext.Current.CancellationToken);

        // Both outputs are fed by the same operator, so a run that took each output on its own
        // would call it twice. This is the graph that tells the two implementations apart; the
        // pipeline above cannot, because its operator feeds one output only.
        Assert.Equal(1, calls);
        Assert.Equal(["EXPANDED", "EXPANDED"], results);

        // Sharing a node is also what makes the two outputs one pipeline, so they are one endpoint.
        Assert.Single(graph.Groups());
    }

    [Fact]
    public async Task A_required_input_that_is_missing_is_named_in_the_failure()
    {
        var graph = WorkflowGraph.FromJson("""
            {
              "schema_version": "2",
              "operators": [
                {"id": "shout", "kind": "fn", "fn": "shout", "label": "Shout",
                 "inputs": [{"id": "in", "label": "Text", "type": "text", "required": true}],
                 "outputs": [{"id": "out", "label": "Output", "type": "text"}]}
              ],
              "subjects": [{"id": "result", "label": "Result", "inputs": [{"id": "in", "type": "text"}]}],
              "edges": [{"id": "e1", "from_node_id": "shout", "from_port_id": "out", "to_node_id": "result", "to_port_id": "in"}]
            }
            """)!;

        var executor = new WorkflowExecutor(
            graph,
            new Dictionary<string, WorkflowOperator>(StringComparer.Ordinal)
            {
                ["fn"] = (_, _, _) => ValueTask.FromResult<IReadOnlyList<object?>>(["never"]),
            });

        var error = await Assert.ThrowsAsync<WorkflowExecutionException>(
            () => executor.RunAsync(
                "result",
                new Dictionary<string, object?>(StringComparer.Ordinal),
                TestContext.Current.CancellationToken));

        Assert.Equal("\"Text\" is required by \"Shout\" but was not provided", error.Message);
    }

    [Fact]
    public async Task A_node_of_a_kind_with_no_caller_says_which_kind()
    {
        var graph = WorkflowGraph.FromJson("""
            {
              "schema_version": "2",
              "operators": [
                {"id": "generate", "kind": "model", "label": "FLUX",
                 "inputs": [{"id": "prompt", "type": "text"}],
                 "outputs": [{"id": "out_0", "type": "image"}]}
              ],
              "subjects": [{"id": "result", "label": "Image", "inputs": [{"id": "in", "type": "image"}]}],
              "edges": [{"id": "e1", "from_node_id": "generate", "from_port_id": "out_0", "to_node_id": "result", "to_port_id": "in"}]
            }
            """)!;

        var executor = new WorkflowExecutor(
            graph,
            new Dictionary<string, WorkflowOperator>(StringComparer.Ordinal));

        var error = await Assert.ThrowsAsync<WorkflowExecutionException>(
            () => executor.RunAsync(
                "result",
                new Dictionary<string, object?>(StringComparer.Ordinal),
                TestContext.Current.CancellationToken));

        Assert.Equal("No executor available for 'model' nodes.", error.Message);
    }

    [Fact]
    public async Task An_output_the_graph_does_not_have_is_refused()
    {
        var graph = WorkflowGraph.FromJson(Pipeline)!;
        var executor = new WorkflowExecutor(
            graph,
            new Dictionary<string, WorkflowOperator>(StringComparer.Ordinal));

        var error = await Assert.ThrowsAsync<WorkflowExecutionException>(
            () => executor.RunAsync(
                "shout",
                new Dictionary<string, object?>(StringComparer.Ordinal),
                TestContext.Current.CancellationToken));

        Assert.Equal("No such workflow output: shout", error.Message);
    }

    [Fact]
    public void A_reference_to_a_file_is_wrapped_for_the_node_that_reads_it()
    {
        // A component hands over a path; an operator is written to receive the reference a client
        // would have sent it, which is what a canvas value and a previous node both are.
        var reference = Assert.IsAssignableFrom<IDictionary<string, object?>>(
            WorkflowValues.ToArgument("/tmp/photo.png", "image"));

        Assert.Equal("/tmp/photo.png", reference["path"]);

        // A scalar port passes the value through untouched.
        Assert.Equal("hello", WorkflowValues.ToArgument("hello", "text"));
    }

    [Fact]
    public void An_output_port_takes_the_element_its_type_describes()
    {
        var port = new WorkflowPort("out_0", "Image", "image");

        // The answer is a path and a caption; the image port takes the path because of its type,
        // not because of where it sits.
        var value = WorkflowValues.PickResponse(port, 1, ["/tmp/a.png", "A cat"], portCount: 2);

        Assert.Equal("/tmp/a.png", value);
    }

    private const string Pipeline = """
        {
          "schema_version": "2",
          "name": "Shout pipeline",
          "references": [
            {"id": "prompt", "label": "Prompt", "outputs": [{"id": "out", "label": "Text", "type": "text"}], "data": {"out": "hello"}},
            {"id": "suffix", "label": "Suffix", "outputs": [{"id": "out", "label": "Text", "type": "text"}], "data": {"out": "!"}}
          ],
          "operators": [
            {"id": "shout", "kind": "fn", "fn": "shout", "label": "shout",
             "inputs": [{"id": "in_text", "label": "text", "type": "text"}, {"id": "in_suffix", "label": "suffix", "type": "text"}],
             "outputs": [{"id": "out_0", "label": "output", "type": "text"}]}
          ],
          "subjects": [
            {"id": "result", "label": "Result", "inputs": [{"id": "in", "label": "Text", "type": "text"}]},
            {"id": "echo", "label": "Echo", "inputs": [{"id": "in", "label": "Text", "type": "text"}]}
          ],
          "edges": [
            {"id": "e1", "from_node_id": "prompt", "from_port_id": "out", "to_node_id": "shout", "to_port_id": "in_text"},
            {"id": "e2", "from_node_id": "suffix", "from_port_id": "out", "to_node_id": "shout", "to_port_id": "in_suffix"},
            {"id": "e3", "from_node_id": "shout", "from_port_id": "out_0", "to_node_id": "result", "to_port_id": "in"},
            {"id": "e4", "from_node_id": "prompt", "from_port_id": "out", "to_node_id": "echo", "to_port_id": "in"}
          ]
        }
        """;
}

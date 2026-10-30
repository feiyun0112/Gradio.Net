namespace Gradio.Net;
/// <summary>
/// A component that brings its own frontend: a bundle the application built, which this framework
/// serves from the directory the component names.
/// </summary>
/// <remarks>
/// <para>
/// The Python version gives every component class an identity — <c>get_component_class_id()</c>,
/// the hash of the class name and the file its module lives in — and a directory its compiled
/// frontend sits in (<c>TEMPLATE_DIR</c>). The frontend asks the server for
/// <c>/gradio_api/custom_component/&lt;id&gt;/client/&lt;variant&gt;/index.js</c> when the
/// component it is drawing is not one it already has, which is how a component that ships its own
/// Svelte bundle is drawn by a frontend that never heard of it.
/// </para>
/// <para>
/// This is that seam on the server side, and it is an interface rather than a convention because
/// the two values are the application's to choose: this framework does not build frontends, so the
/// only thing it can know about a bundle is where the application put it and what the application
/// calls it. A component that implements this is served by
/// <c>Gradio.Net.Server</c>'s custom-component route; one that does not keeps the null identity
/// every built-in component has, and the frontend draws it from its own registry.
/// </para>
/// <para>
/// It is called a bundle rather than assets because the glossary's "vendored assets" are the
/// opposite kind of file: hashed, locked, and not the application's to change. What this names is
/// the application's own build output, served as it lies on disk.
/// </para>
/// <para>
/// <b>What this does not do</b>: it does not build, bundle or validate the frontend, and a
/// component that implements it is only drawn if the application also produced the bundle. Writing
/// a new <em>shape</em> of component in C# alone is therefore not possible — that is the boundary
/// this interface draws, and it is the price of not carrying a Node toolchain in this package.
/// </para>
/// </remarks>
public interface ICustomComponentBundle
{
    /// <summary>
    /// The identity the frontend asks for this component's bundle by, which is also what the
    /// config's <c>component_class_id</c> carries.
    /// </summary>
    /// <value>
    /// A URL-safe token, stable for the life of the bundle. It becomes a path segment of every
    /// request for the bundle, so it must not contain a slash, a backslash or a colon, and changing
    /// it invalidates every cached copy of the bundle in every browser.
    /// </value>
    /// <remarks>
    /// <para>
    /// The Python version derives this from the class and its module path, so its value identifies
    /// an installation rather than a component. This framework asks the application instead: the
    /// bundle and the class that draws it are built by the same person, and an identity they chose
    /// is one they can also keep across a rebuild.
    /// </para>
    /// <para>
    /// Nothing checks the shape of the value when the config is written. An identity containing a
    /// separator cannot be reached by any request — the router would never hand over a segment with
    /// one — so the component simply never loads, which is a mistake an application finds the first
    /// time it opens the page.
    /// </para>
    /// </remarks>
    string ComponentClassId { get; }

    /// <summary>
    /// The directory holding the compiled bundle, as the Python version's <c>TEMPLATE_DIR</c>: the
    /// parent of the variant directories, not a variant itself.
    /// </summary>
    /// <value>
    /// An absolute path, or one relative to the process's working directory. The route serves
    /// <c>&lt;directory&gt;/&lt;variant&gt;/&lt;file&gt;</c>, where the URL segment the Python
    /// version calls <c>type</c> names the variant; a path that would leave this directory is
    /// refused rather than served.
    /// </value>
    /// <remarks>
    /// A variant is <c>component</c> for the component as it is drawn and <c>example</c> for the
    /// same component used as an example input — the two the frontend asks for by name. Nothing
    /// here reads the bundle: it is the application's artifact, served as it is.
    /// </remarks>
    string TemplateDirectory { get; }
}

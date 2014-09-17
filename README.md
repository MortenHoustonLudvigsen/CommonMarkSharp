CommonMarkSharp
===============

A CommonMark parser for .Net

CommonMarkSharp parses and renders Markdown according to the [CommonMark](http://commonmark.org/) spec.

Installation
------------

CommonMarkSharp is available as a NuGet package at <http://www.nuget.org/packages/CommonMarkSharp/>.

Simple usage
------------

### Reading from a file, and rendering HTML to another file

```csharp
var cm = new CommonMark();

using (var reader = File.OpenText(fileName))
using (var writer = File.CreateText(outputFileName))
{
    cm.RenderAsHtml(reader, writer);
}
```

### Reading from a file, and rendering HTML to a string

```csharp
var cm = new CommonMark();

using (var reader = File.OpenText(fileName))
{
    string html = cm.RenderAsHtml(reader);
}
```

### Rendering Markdown in a string as HTML to another string

```csharp
var cm = new CommonMark();

string commonMark = @"
This is the title
=================
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud
exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
";

string html = cm.RenderAsHtml(commonMark);
```

### Rendering Markdown in a string as HTML to a file

```csharp
var cm = new CommonMark();

string commonMark = @"
This is the title
=================
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud
exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
";

using (var writer = File.CreateText(outputFileName))
{
    cm.RenderAsHtml(commonMark, writer);
}
```

Less simple usage
-----------------

It is possible to parse Markdown without rendering. 

### Parsing from a file

```csharp
var cm = new CommonMark();

using (var reader = File.OpenText(fileName))
{
    Document document = cm.Parse(reader);
}
```

### Parsing from a string

```csharp
var cm = new CommonMark();

string commonMark = @"
This is the title
=================
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud
exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
";

Document document = cm.Parse(commonMark);
```

### A custom renderer

Once the Markdown has been parsed, it will be in a `Document` object, which is the root of an AST (abstract syntax tree), representing the parsed Markdown. This AST can be traversed using a `CommonMarkVisitor`.

One such `CommonMarkVisitor` is the `HtmlRenderer` class. This visitor is used for the default rendering of HTML, and conforms to the [CommonMark](http://commonmark.org/) spec.

If you need to tweek the rendered HTML, you can inherit from `HtmlRenderer`. You might want to wrap the rendered HTML in `<html>`, `<head>` and `<body>`:

```csharp
public class HtmlPageRenderer : HtmlRenderer
{
    public override void Visit(Document document)
    {
        WriteStartTag(document, "html");
        WriteStartTag(document, "head");
        WriteStartTag(document, "title");
        Write("A title");
        WriteEndTag(document, "title");
        WriteEndTag("head");
        WriteStartTag(WriteEndTag, "body");
        base.Visit(document);
        WriteEndTag(document, "body");
        WriteEndTag(document, "html");
    }
}
```

### Rendering to string using a custom renderer

```csharp
var cm = new CommonMark();
var renderer = new HtmlPageRenderer();

string commonMark = @"
This is the title
=================
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud
exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
";

var document = cm.Parse(commonMark);
var html = renderer.Render(document);
```

### Rendering to file using a custom renderer

```csharp
var cm = new CommonMark();
var renderer = new HtmlPageRenderer();

string commonMark = @"
This is the title
=================
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud
exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
";

var document = cm.Parse(commonMark);
using (var writer = File.CreateText(outputFileName))
{
    renderer.Render(document, writer);
}
```

## Updating tests from the spec

The tests in CommonMarkSharp.Tests are created from the [CommonMark spec](http://jgm.github.io/stmd/spec.html) using a T4 template (`CommonMarkSharp.Tests\Specs.tt`).

The template creates a test for each spec in `spec.txt`, which is expected to be in the `CommonMarkSharp.Tests\bower_components\stmd` directory.

To update the tests run the following in the `CommonMarkSharp.Tests` directory:

    bower update

After which the tests can be updated by running custom tools on `CommonMarkSharp.Tests\Specs.tt` (or just saving it in Visual Studio).
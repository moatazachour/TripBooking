using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TripBooking.Web.TagHelpers;

// The naming convention here mirrors View Components: "ConfirmButton" in the
// class name becomes "confirm-button" in markup — PascalCase collapses to
// kebab-case, automatically, no configuration needed.
[HtmlTargetElement("confirm-button")]
public class ConfirmButtonTagHelper : TagHelper
{
    // Any public property here becomes an attribute you can set in markup —
    // "confirm-message" below maps to this property the same PascalCase-to-
    // kebab-case way the element name itself did.
    public string ConfirmMessage { get; set; } = "Are you sure?";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // Rewrite the OUTPUT tag from <confirm-button> to a real <button> —
        // the browser will only ever see the result of this rewrite.
        output.TagName = "button";
        output.Attributes.SetAttribute("type", "submit");
        output.Attributes.SetAttribute("onclick", $"return confirm('{ConfirmMessage}');");
    }
}
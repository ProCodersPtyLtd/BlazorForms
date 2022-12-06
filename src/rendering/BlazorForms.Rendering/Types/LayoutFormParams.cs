using BlazorForms.Forms;

namespace BlazorForms.Rendering.Types;

public record LayoutFormParams
{
    public List<LayoutFormElement>? Elements { get; private set; } = new List<LayoutFormElement>();
}

public record LayoutFormElement
{
    public int Row { get; set; }
    public int Col { get; set; }
    public int RowSpan { get; set; }
    public int ColSpan { get; set; }
    public IGrouping<string, FieldControlDetails> Group { get; set; }
    //public Type WidgetType { get; set; }
    //public Dictionary<string, object?> Parameters { get; set; }
}


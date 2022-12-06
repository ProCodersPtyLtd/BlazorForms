namespace BlazorForms.Rendering.Types;

public record DashboardParams
{
    public List<DashboardWidget>? Widgets { get; set; }
}

public record DashboardWidget
{
    public int Row { get; set; }
    public int Col { get; set; }
    public int RowSpan { get; set; }
    public int ColSpan { get; set; }
    public Type WidgetType { get; set; }
    public Dictionary<string, object?> Parameters { get; set; }
}


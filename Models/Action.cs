namespace WorkflowEngine.Models;

public class Action
{
    public string Id { get; set; } = string.Empty;
    public List<string> FromStates { get; set; } = new();
    public string ToState { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
}

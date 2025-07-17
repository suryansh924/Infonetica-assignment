using WorkflowEngine.Models;

namespace WorkflowEngine.Services;

public class WorkflowService
{
    private readonly Dictionary<string, WorkflowDefinition> definitions = new();
    private readonly Dictionary<string, WorkflowInstance> instances = new();

    public (bool success, string error) AddDefinition(WorkflowDefinition def)
    {
        if (definitions.ContainsKey(def.Id))
            return (false, "Duplicate definition ID.");

        var initialStates = def.States.Count(s => s.IsInitial);
        if (initialStates != 1)
            return (false, "Workflow must have exactly one initial state.");

        // Validate that all states have unique IDs
        var stateIds = def.States.Select(s => s.Id).ToList();
        if (stateIds.Count != stateIds.Distinct().Count())
            return (false, "State IDs must be unique.");

        // Validate that all actions reference valid states
        var validStateIds = new HashSet<string>(stateIds);
        foreach (var action in def.Actions)
        {
            if (!validStateIds.Contains(action.ToState))
                return (false, $"Action '{action.Id}' references invalid target state '{action.ToState}'.");
            
            foreach (var fromState in action.FromStates)
            {
                if (!validStateIds.Contains(fromState))
                    return (false, $"Action '{action.Id}' references invalid source state '{fromState}'.");
            }
        }

        definitions[def.Id] = def;
        return (true, "");
    }

    public WorkflowDefinition? GetDefinition(string id) =>
        definitions.TryGetValue(id, out var def) ? def : null;

    public IEnumerable<WorkflowDefinition> GetAllDefinitions() => definitions.Values;

    public WorkflowInstance? StartInstance(string defId)
    {
        if (!definitions.TryGetValue(defId, out var def)) return null;

        var initialState = def.States.FirstOrDefault(s => s.IsInitial);
        if (initialState == null || !initialState.Enabled) return null;

        var inst = new WorkflowInstance
        {
            WorkflowDefinitionId = defId,
            CurrentStateId = initialState.Id
        };
        inst.History.Add($"Started in state '{initialState.Id}' @ {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        
        instances[inst.Id] = inst;
        return inst;
    }

    public (bool success, string error) ExecuteAction(string instId, string actionId)
    {
        if (!instances.TryGetValue(instId, out var inst))
            return (false, "Instance not found.");
        
        if (!definitions.TryGetValue(inst.WorkflowDefinitionId, out var def))
            return (false, "Workflow definition not found.");

        var action = def.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null || !action.Enabled)
            return (false, "Invalid or disabled action.");

        if (!action.FromStates.Contains(inst.CurrentStateId))
            return (false, "Action not allowed from current state.");

        var nextState = def.States.FirstOrDefault(s => s.Id == action.ToState);
        if (nextState == null || !nextState.Enabled)
            return (false, "Invalid or disabled target state.");

        var currentState = def.States.First(s => s.Id == inst.CurrentStateId);
        if (currentState.IsFinal)
            return (false, "Cannot move from final state.");

        inst.CurrentStateId = nextState.Id;
        inst.History.Add($"Executed action '{actionId}' -> '{nextState.Id}' @ {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        return (true, "");
    }

    public WorkflowInstance? GetInstance(string id) =>
        instances.TryGetValue(id, out var inst) ? inst : null;

    public IEnumerable<WorkflowInstance> GetAllInstances() => instances.Values;

    public List<Models.Action> GetAvailableActions(string instId)
    {
        if (!instances.TryGetValue(instId, out var inst))
            return new List<Models.Action>();

        if (!definitions.TryGetValue(inst.WorkflowDefinitionId, out var def))
            return new List<Models.Action>();

        var currentState = def.States.First(s => s.Id == inst.CurrentStateId);
        if (currentState.IsFinal)
            return new List<Models.Action>();

        return def.Actions
            .Where(a => a.Enabled && a.FromStates.Contains(inst.CurrentStateId))
            .Where(a => def.States.Any(s => s.Id == a.ToState && s.Enabled))
            .ToList();
    }
}

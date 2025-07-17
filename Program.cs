using WorkflowEngine.Models;
using WorkflowEngine.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<WorkflowService>();

var app = builder.Build();

var service = app.Services.GetRequiredService<WorkflowService>();

// Workflow Definition endpoints
app.MapPost("/workflows", (WorkflowDefinition def) =>
{
    var (success, error) = service.AddDefinition(def);
    return success ? Results.Ok(new { message = "Workflow created successfully" }) : Results.BadRequest(new { error });
})
.WithName("CreateWorkflow");

app.MapGet("/workflows/{id}", (string id) =>
    service.GetDefinition(id) is { } def ? Results.Ok(def) : Results.NotFound(new { error = "Workflow not found" }))
.WithName("GetWorkflow");

app.MapGet("/workflows", () => Results.Ok(service.GetAllDefinitions()))
.WithName("GetAllWorkflows");

// Workflow Instance endpoints
app.MapPost("/instances/{defId}", (string defId) =>
    service.StartInstance(defId) is { } inst ? Results.Ok(inst) : Results.BadRequest(new { error = "Invalid workflow definition or workflow not found" }))
.WithName("StartWorkflowInstance");

app.MapPost("/instances/{instId}/actions/{actionId}", (string instId, string actionId) =>
{
    var (success, error) = service.ExecuteAction(instId, actionId);
    return success 
        ? Results.Ok(new { message = "Action executed successfully" }) 
        : Results.BadRequest(new { error });
})
.WithName("ExecuteAction");

app.MapGet("/instances/{id}", (string id) =>
    service.GetInstance(id) is { } inst ? Results.Ok(inst) : Results.NotFound(new { error = "Instance not found" }))
.WithName("GetWorkflowInstance");

app.MapGet("/instances", () => Results.Ok(service.GetAllInstances()))
.WithName("GetAllWorkflowInstances");

app.MapGet("/instances/{id}/actions", (string id) =>
{
    var actions = service.GetAvailableActions(id);
    return actions.Any() ? Results.Ok(actions) : Results.Ok(new { message = "No available actions (final state or invalid instance)" });
})
.WithName("GetAvailableActions");

// Root endpoint
app.MapGet("/", () => Results.Ok(new { 
    message = "Infonetica Workflow Engine API",
    version = "1.0",
    swagger = "/swagger"
}))
.WithName("Root");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

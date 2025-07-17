# Infonetica Assignment – Configurable Workflow Engine (State Machine API)

## Overview

This is a **State Machine API** built with .NET 8 for the Infonetica assignment. The API allows you to create configurable workflows with states and actions, start workflow instances, and execute transitions between states.

## Features

**Create Workflow Definitions** - Define states (initial, final, enabled) and actions (transitions)  
**Start Workflow Instances** - Create instances that track current state and history  
**Execute Actions** - Transition between states with validation  
**State Management** - Proper handling of initial states, final states, and enabled/disabled states  
**Action Validation** - Ensures actions are valid from current state  
**History Tracking** - Full audit trail of all state transitions  
**Swagger Documentation** - Interactive API documentation  
**In-Memory Storage** - No database required (as per assignment requirements)

## Quick Start

### Prerequisites

- .NET 8 SDK installed
- Any HTTP client (Postman, curl, or browser for Swagger)

### Setup & Run

```bash
# Clone, Build and Run the application
dotnet build
dotnet run

# API will be available at:
# http://localhost:XXXX
# Swagger UI at: http://localhost:XXXX/swagger
```

## API Endpoints

| Method | Endpoint                                     | Description                        |
| ------ | -------------------------------------------- | ---------------------------------- |
| `POST` | `/workflows`                                 | Create a new workflow definition   |
| `GET`  | `/workflows/{id}`                            | Get workflow definition by ID      |
| `GET`  | `/workflows`                                 | Get all workflow definitions       |
| `POST` | `/instances/{workflowId}`                    | Start a new workflow instance      |
| `POST` | `/instances/{instanceId}/actions/{actionId}` | Execute an action                  |
| `GET`  | `/instances/{id}`                            | Get workflow instance details      |
| `GET`  | `/instances`                                 | Get all workflow instances         |
| `GET`  | `/instances/{id}/actions`                    | Get available actions for instance |

## Example Usage

### 1. Create a Workflow Definition

```json
POST /workflows
{
  "id": "approval-workflow",
  "states": [
    {
      "id": "draft",
      "isInitial": true,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "pending-review",
      "isInitial": false,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "approved",
      "isInitial": false,
      "isFinal": true,
      "enabled": true
    },
    {
      "id": "rejected",
      "isInitial": false,
      "isFinal": true,
      "enabled": true
    }
  ],
  "actions": [
    {
      "id": "submit-for-review",
      "fromStates": ["draft"],
      "toState": "pending-review",
      "enabled": true
    },
    {
      "id": "approve",
      "fromStates": ["pending-review"],
      "toState": "approved",
      "enabled": true
    },
    {
      "id": "reject",
      "fromStates": ["pending-review"],
      "toState": "rejected",
      "enabled": true
    }
  ]
}
```

### 2. Start a Workflow Instance

```json
POST /instances/approval-workflow
```

Response:

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "workflowDefinitionId": "approval-workflow",
  "currentStateId": "draft",
  "history": ["Started in state 'draft' @ 2024-01-15 10:30:00 UTC"]
}
```

### 3. Execute an Action

```json
POST /instances/550e8400-e29b-41d4-a716-446655440000/actions/submit-for-review
```

### 4. Check Instance Status

```json
GET /instances/550e8400-e29b-41d4-a716-446655440000
```

Response:

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "workflowDefinitionId": "approval-workflow",
  "currentStateId": "pending-review",
  "history": [
    "Started in state 'draft' @ 2024-01-15 10:30:00 UTC",
    "Executed action 'submit-for-review' -> 'pending-review' @ 2024-01-15 10:31:00 UTC"
  ]
}
```

## Validation Rules

The API enforces these business rules:

1. **Exactly one initial state** per workflow
2. **Unique state IDs** within a workflow
3. **Valid state references** in actions
4. **Actions must be enabled** to be executed
5. **Target states must be enabled** for transitions
6. **No transitions from final states**
7. **Actions only allowed from valid source states**

## Architecture

```
/WorkflowEngine/
├── Program.cs               # API routes & dependency injection
├── Models/
│   ├── State.cs            # State entity
│   ├── Action.cs           # Action entity
│   ├── WorkflowDefinition.cs # Workflow definition
│   └── WorkflowInstance.cs  # Workflow instance
├── Services/
│   └── WorkflowService.cs   # Business logic & in-memory storage
└── README.md
```

## Technical Stack

- **Framework**: ASP.NET Core 8 (Minimal API)
- **Language**: C# 12
- **Storage**: In-memory Dictionary (no database)
- **Documentation**: Swagger/OpenAPI
- **Validation**: Built-in model validation

## Error Handling

The API provides clear error messages for:

- Invalid workflow definitions
- Duplicate IDs
- Invalid state transitions
- Missing resources
- Business rule violations

## Testing with Swagger

1. Run the application: `dotnet run`
2. Open: `http://localhost:5000/swagger`
3. Use the interactive UI to test all endpoints
4. Example workflows are provided in the API documentation

## Future Enhancements

- Persistent storage (database)
- Workflow versioning
- Conditional transitions
- Crud operations for workflow, instances or actions
- Parallel execution paths
- Workflow templates
- User authentication
- Audit logging
- Performance metrics
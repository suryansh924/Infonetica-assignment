# Sample Workflow Definitions for Testing

## Simple Approval Workflow

```json
{
  "id": "simple-approval",
  "states": [
    {
      "id": "draft",
      "isInitial": true,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "approved",
      "isInitial": false,
      "isFinal": true,
      "enabled": true
    }
  ],
  "actions": [
    {
      "id": "approve",
      "fromStates": ["draft"],
      "toState": "approved",
      "enabled": true
    }
  ]
}
```

## Document Review Workflow

```json
{
  "id": "document-review",
  "states": [
    {
      "id": "draft",
      "isInitial": true,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "under-review",
      "isInitial": false,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "needs-revision",
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
      "fromStates": ["draft", "needs-revision"],
      "toState": "under-review",
      "enabled": true
    },
    {
      "id": "approve",
      "fromStates": ["under-review"],
      "toState": "approved",
      "enabled": true
    },
    {
      "id": "reject",
      "fromStates": ["under-review"],
      "toState": "rejected",
      "enabled": true
    },
    {
      "id": "request-revision",
      "fromStates": ["under-review"],
      "toState": "needs-revision",
      "enabled": true
    }
  ]
}
```

## Order Processing Workflow

```json
{
  "id": "order-processing",
  "states": [
    {
      "id": "pending",
      "isInitial": true,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "processing",
      "isInitial": false,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "shipped",
      "isInitial": false,
      "isFinal": false,
      "enabled": true
    },
    {
      "id": "delivered",
      "isInitial": false,
      "isFinal": true,
      "enabled": true
    },
    {
      "id": "cancelled",
      "isInitial": false,
      "isFinal": true,
      "enabled": true
    }
  ],
  "actions": [
    {
      "id": "start-processing",
      "fromStates": ["pending"],
      "toState": "processing",
      "enabled": true
    },
    {
      "id": "ship-order",
      "fromStates": ["processing"],
      "toState": "shipped",
      "enabled": true
    },
    {
      "id": "deliver-order",
      "fromStates": ["shipped"],
      "toState": "delivered",
      "enabled": true
    },
    {
      "id": "cancel-order",
      "fromStates": ["pending", "processing"],
      "toState": "cancelled",
      "enabled": true
    }
  ]
}
```

## Testing Steps

1. **Create workflow definition**: `POST /workflows` with any of the above JSON
2. **Start instance**: `POST /instances/{workflow-id}`
3. **Check available actions**: `GET /instances/{instance-id}/actions`
4. **Execute action**: `POST /instances/{instance-id}/actions/{action-id}`
5. **Check instance status**: `GET /instances/{instance-id}`

## cURL Examples

```bash
# Create workflow
curl -X POST http://localhost:5000/workflows \
  -H "Content-Type: application/json" \
  -d @simple-approval.json

# Start instance
curl -X POST http://localhost:5000/instances/simple-approval

# Execute action
curl -X POST http://localhost:5000/instances/{instance-id}/actions/approve

# Check status
curl http://localhost:5000/instances/{instance-id}
```

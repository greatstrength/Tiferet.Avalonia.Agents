# Tiferet.Avalonia.Agents

An Avalonia UI extension for building agentic chat interfaces with streamed responses, inline tool approval, and conversation management. Built on [Tiferet.Avalonia](https://github.com/greatstrength/tiferet.net-avalonia) and designed as the desktop front-end for [tiferet-agents](https://github.com/greatstrength/tiferet-agents).

## Features

- **Streaming chat** — Real-time token-by-token LLM response rendering via `IAsyncEnumerable<StreamToken>`
- **Tool approval** — Inline approve/deny cards for human-in-the-loop tool execution
- **Conversation management** — Sidebar with conversation list, agent selection, and lifecycle commands
- **Custom controls** — `ChatBubble`, `MessageInput`, `StreamingTextBlock`, `ToolCallCard`, `MarkdownBlock`
- **AgentTheme** — Single-include style bundle for all agent controls
- **DI integration** — `AddTiferetAgents()` extension method with `AgentsBlueprint` service registration
- **Mock services** — In-memory implementations for development and design-time use

## Requirements

- .NET 9.0
- Avalonia 11.2.7+
- Tiferet.Avalonia 1.0.0-beta.2+

## Installation

### NuGet (when published)

```bash
dotnet add package Tiferet.Avalonia.Agents --version 1.0.0-beta.1
```

### Local Development

Clone the repository and reference the project directly:

```bash
git clone https://github.com/greatstrength/Tiferet.Avalonia.Agents.git
```

The `local-packages/` directory contains local `.nupkg` files for Tiferet dependencies. The `nuget.config` at the repo root configures this as a package source.

## Quick Start

### 1. Add the AgentTheme to your App.axaml

```xml
<Application.Styles>
  <FluentTheme />
  <StyleInclude Source="avares://Tiferet.Avalonia.Agents/Assets/Styles/AgentTheme.axaml" />
</Application.Styles>
```

### 2. Create an AgentWindow

The simplest way to get started — `AgentWindow` wires up mock services automatically:

```csharp
using Tiferet.Avalonia.Agents.Assets.Views;

// In your App.OnFrameworkInitializationCompleted:
desktop.MainWindow = new AgentWindow();
```

### 3. Or use DI registration

```csharp
using Tiferet.Avalonia.Agents.DependencyInjection;

services.AddTiferetAgents(options =>
{
    options.AgentId = "my-agent";
    options.MockServices = true;
    options.TokenDelayMs = 30;
});
```

## Architecture

```
Tiferet.Avalonia.Agents/
├── Assets/
│   ├── Controls/       ChatBubble, ToolCallCard, StreamingTextBlock,
│   │                   MessageInput, MarkdownBlock
│   ├── Styles/         AgentTheme.axaml (bundles all control styles)
│   └── Views/          AgentWindow, ChatView, ConversationListView
├── Blueprints/         AgentsBlueprint, TiferetAgentsOptions
├── Contexts/           ChatViewModel, ConversationListViewModel,
│                       ToolApprovalViewModel
├── DependencyInjection/  AddTiferetAgents extension method
├── Domain/             AgentInfo, ChatMessage, ConversationInfo,
│                       StreamToken, ToolCallRequest, ToolCallResult
├── Interfaces/         IAgentChatService, IToolApprovalService,
│                       IConversationService, IAgentService
└── Services/           MockAgentChatService, MockConversationService,
                        MockAgentService
```

## Example Application

See `examples/Tiferet.Avalonia.Agents.Examples.Chat/` for a complete working example.

```bash
dotnet run --project examples/Tiferet.Avalonia.Agents.Examples.Chat
```

## Testing

```bash
dotnet test
```

The test suite includes 63 tests covering mock service behavior, ViewModel logic, state transitions, callbacks, and edge cases.

## License

MIT

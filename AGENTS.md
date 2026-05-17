# AGENTS.md — Tiferet.Avalonia.Agents (v1.0.0-beta.1)

## Project Overview

**Tiferet.Avalonia.Agents** is an Avalonia UI extension for building agentic chat interfaces with streamed responses, inline tool approval, and conversation management. Built on Tiferet.Avalonia and designed as the desktop front-end for tiferet-agents.

- **Repository:** https://github.com/greatstrength/Tiferet.Avalonia.Agents
- **Branch:** `v1.x-proto`
- **Runtime:** .NET 9.0
- **Version:** 1.0.0-beta.1
- **Core dependencies:** Tiferet.NET ≥ 1.0.0-beta.7, Tiferet.Avalonia ≥ 1.0.0-beta.2

## Architecture

### Solution Structure

```
Tiferet.Avalonia.Agents.sln
├── Tiferet.Avalonia.Agents/              Main library
│   ├── Assets/
│   │   ├── Controls/     ChatBubble, MessageInput, StreamingTextBlock,
│   │   │                 ToolCallCard, MarkdownBlock
│   │   ├── Styles/       AgentTheme.axaml (bundles all control styles)
│   │   └── Views/        AgentWindow, ChatView, ConversationListView
│   ├── Blueprints/       AgentsBlueprint, TiferetAgentsOptions
│   ├── Contexts/         ChatViewModel, ConversationListViewModel,
│   │                     ToolApprovalViewModel
│   ├── DependencyInjection/  AddTiferetAgents extension method
│   ├── Domain/           AgentInfo, ChatMessage, ConversationInfo,
│   │                     StreamToken, ToolCallRequest, ToolCallResult
│   ├── Interfaces/       IAgentChatService, IToolApprovalService,
│   │                     IConversationService, IAgentService
│   └── Services/         MockAgentChatService, MockConversationService,
│                         MockAgentService
├── Tiferet.Avalonia.Agents.Tests/        xUnit test project (63 tests)
│   ├── Contexts/         ChatViewModelTests, ConversationListViewModelTests,
│   │                     ToolApprovalViewModelTests
│   └── Services/         MockAgentChatServiceTests, MockAgentServiceTests,
│                         MockConversationServiceTests
└── examples/
    └── Tiferet.Avalonia.Agents.Examples.Chat/  Standalone example app
```

### Key Concepts

- **ChatViewModel** — Main chat panel ViewModel managing messages, streaming input, tool call flow, and conversation lifecycle (`LoadConversation`, `ClearMessages`).
- **ConversationListViewModel** — Sidebar ViewModel with conversation list, agent selection dropdown, and commands for load/new/delete conversations. Notifies parent via `ConversationSelected` callback.
- **ToolApprovalViewModel** — Inline approve/deny flow wrapping a `ToolCallRequest`. State transitions: pending → approved/denied.
- **StreamingTextBlock** — Custom Avalonia control that appends tokens in real-time during LLM streaming.
- **ToolCallCard** — Inline approve/deny card with `:pending`/`:approved`/`:denied` pseudo-classes.
- **ChatBubble** — Role-based chat message display with `:human`/`:ai`/`:tool`/`:system` pseudo-classes.
- **IAgentChatService** — Backend-agnostic service contract using `IAsyncEnumerable<StreamToken>` for streaming.
- **AgentWindow** — Top-level shell wiring sidebar (ConversationListView) and chat panel (ChatView) with mock services.
- **AgentsBlueprint** — Static DI registration. Registers mock services (when `MockServices=true`), ViewModels, and options.
- **AgentTheme** — Single AXAML file bundling all 5 control style sheets for one-line inclusion.

### Runtime Flow

1. `AgentWindow` constructor creates mock services and ViewModels.
2. `OnWindowOpened` triggers `LoadAgentsCommand` and `LoadConversationsCommand`.
3. User selects a conversation → `ConversationSelected` callback → `ChatViewModel.LoadConversation()`.
4. User sends a message → `ChatViewModel.SendMessageAsync()` → streams tokens via `IAgentChatService.StreamMessageAsync()`.
5. If AI response contains tool calls with `RequiresApproval=true`, `ToolApprovalViewModel` instances are created for inline approve/deny.

## Structured Code Style

All code follows the Tiferet structured code style with artifact comments:

- `// ***` — Top-level sections: `controls`, `contexts`, `records`, `views`, `services`, `blueprints`, `extensions`, `tests`
- `// **` — Mid-level: `context: <name>`, `control: <name>`, `record: <name>`, `view: <name>`, `service: <name>`, `blueprint: <name>`, `test: <name>`
- `// *` — Low-level: `attribute: <name>`, `init`, `method: <name>`, `property: <name>`

XML doc comments (`/// <summary>`) on all public members.

### Avalonia-Specific Patterns

- **Pseudo-classes:** Use `PseudoClasses.Add(name)` / `PseudoClasses.Remove(name)` — not `.Set()` (removed in Avalonia 11).
- **CommunityToolkit.Mvvm:** `[ObservableProperty]`, `[RelayCommand]`, `[NotifyCanExecuteChangedFor]`.
- **ViewModelBase:** Extends `Tiferet.Avalonia.Contexts.ViewModelBase` (which extends `ObservableObject`).
- **Partial methods:** `OnSelectedConversationChanged`, `OnSelectedAgentChanged` for property change hooks.

## Dependencies

- `Tiferet.Avalonia` 1.0.0-beta.2 (local nupkg, includes Tiferet 1.0.0-beta.7)
- `CommunityToolkit.Mvvm` 8.4.0
- `Microsoft.Extensions.DependencyInjection.Abstractions` 9.0.0
- `Avalonia` 11.2.7, `Avalonia.Desktop` 11.2.7, `Avalonia.Themes.Fluent` 11.2.7

### Local NuGet Feed

The `local-packages/` directory contains local `.nupkg` files. The `nuget.config` at the repo root configures this as a package source alongside nuget.org.

## Testing

- **Framework:** xUnit (63 tests)
- **Run:** `dotnet test` from repository root
- **Test structure:** Uses artifact comments (`// *** tests`, `// ** test: <name>`, `// * method: <name>`).
- **Coverage:** MockAgentChatService (12), MockAgentService (4), MockConversationService (9), ChatViewModel (13), ConversationListViewModel (13), ToolApprovalViewModel (12).
- **Pattern:** Tests use real mock services (not mocking frameworks) for deterministic behavior.

## Branch Conventions

- **Prototype branch:** `v1.x-proto` — main development line.
- **Release branches:** `v1.0b1-release` — milestones accumulate here, then rebase into proto.
- **Feature branches:** `v1.0.0-alpha.N-<description>` created from release branch.
- PRs target the release branch during a milestone cycle.
- All commits include `Co-Authored-By: Oz <oz-agent@warp.dev>` when collaborating with AI.

## Key Files for Orientation

- `Tiferet.Avalonia.Agents/Contexts/ChatViewModel.cs` — Core chat logic
- `Tiferet.Avalonia.Agents/Contexts/ConversationListViewModel.cs` — Sidebar logic
- `Tiferet.Avalonia.Agents/Assets/Views/AgentWindow.axaml(.cs)` — Shell window wiring
- `Tiferet.Avalonia.Agents/Blueprints/AgentsBlueprint.cs` — DI registration
- `Tiferet.Avalonia.Agents/DependencyInjection/ServiceCollectionExtensions.cs` — `AddTiferetAgents()`
- `Tiferet.Avalonia.Agents/Assets/Styles/AgentTheme.axaml` — Style bundle entry point
- `Tiferet.Avalonia.Agents/Interfaces/IAgentChatService.cs` — Primary service contract

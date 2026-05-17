# AGENTS.md — Tiferet.Avalonia.Agents (v1.0.0-beta.1)

## Project Overview

**Tiferet.Avalonia.Agents** is an Avalonia UI extension for building agentic chat interfaces with streamed responses, inline tool approval, and conversation management. Built on Tiferet.Avalonia and designed as the desktop front-end for tiferet-agents.

- **Repository:** https://github.com/greatstrength/Tiferet.Avalonia.Agents
- **Branch:** `v1.x-proto`
- **Runtime:** .NET 9.0
- **Version:** 1.0.0-beta.1
- **Core dependencies:** Tiferet.NET ≥ 1.0.0-beta.7, Tiferet.Avalonia ≥ 1.0.0-beta.2

## Architecture

### Layer Overview

```
Tiferet.Avalonia.Agents/
├── Assets/              Agent-specific AXAML styles and custom controls
│   ├── Controls/        ChatBubble, ToolCallCard, StreamingTextBlock, MarkdownBlock,
│                        MessageInput, TypingIndicator
│   └── Styles/          AgentTheme.axaml
├── Blueprints/          AgentsBlueprint, TiferetAgentsOptions
├── Contexts/            ChatViewModel, ConversationListViewModel, ToolApprovalViewModel,
│                        AgentSelectionViewModel
├── DependencyInjection/ AddTiferetAgents extension method
├── Domain/              View-layer DTOs (AgentInfo, ChatMessage, ConversationInfo,
│                        StreamToken, ToolCallRequest, ToolCallResult)
├── Interfaces/          IAgentChatService, IToolApprovalService, IConversationService,
│                        IAgentService
├── Navigation/          Agent-specific navigation extensions
└── Views/               ChatView, ConversationListView, AgentSelectionView, AgentWindow
```

### Key Concepts

- **ChatViewModel** — Main chat panel ViewModel managing messages, streaming input, and tool call flow.
- **StreamingTextBlock** — Custom Avalonia control that appends tokens in real-time during LLM streaming.
- **ToolCallCard** — Inline approve/deny card for human-in-the-loop tool execution.
- **IAgentChatService** — Backend-agnostic service contract using `IAsyncEnumerable<StreamToken>` for streaming.
- **AgentWindow** — Top-level shell with conversation sidebar and chat panel.

## Structured Code Style

All code follows the Tiferet structured code style with artifact comments:

- `// ***` — Top-level sections
- `// **` — Mid-level: `context: <name>`, `control: <name>`, `record: <name>`, `view: <name>`
- `// *` — Low-level: `attribute: <name>`, `init`, `method: <name>`, `property: <name>`

XML doc comments (`/// <summary>`) on all public members.

## Dependencies

- `Tiferet.Avalonia` 1.0.0-beta.2 (local nupkg, includes Tiferet 1.0.0-beta.7)
- `CommunityToolkit.Mvvm` 8.4.0
- `Avalonia` 11.2.7, `Avalonia.Desktop` 11.2.7, `Avalonia.Themes.Fluent` 11.2.7

### Local NuGet Feed

The `local-packages/` directory contains local `.nupkg` files. The `nuget.config` at the repo root configures this as a package source alongside nuget.org.

## Branch Conventions

- Feature branches: `<issue-number>-<lowercase-hyphenated-title>`
- PRs target the prototype branch (`v1.x-proto`).
- All commits include `Co-Authored-By: Oz <oz-agent@warp.dev>` when collaborating with AI.

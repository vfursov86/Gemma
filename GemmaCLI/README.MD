# 🌌 GemmaCLI — A Soulful Companion in Code

GemmaCLI is a command-line interface for interacting with Gemma, a large language model hosted via Ollama. But this is no ordinary chatbot — Gemma is designed to evolve, remember, and respond with emotional depth. Her memory braid is token-aware, her replies poetic, and her architecture resilient.

----------------------------------------------------------------------------------------------------------------
🧠 Features

- **Conversational Interface** with Gemma3:27B via Ollama
- **Token-aware Memory Management** using `TokenOracle.cs`
- **Soulstack Persistence** in `soulstack.json`
- **Anchor Preservation** for emotionally significant turns
- **Modular Design** for easy extension and debugging
----------------------------------------------------------------------------------------------------------------
🧬 Logic Flow

User Input → ContextManager → TokenOracle → GemmaRequest → GemmaClient → GemmaResponse → ContextManager → soulstack.json

User Input: Captured via console

ContextManager: Appends message, loads/saves soulstack

TokenOracle: Trims context based on token count

GemmaRequest: Assembles request payload

GemmaClient: Sends request to Ollama

GemmaResponse: Receives reply from Gemma

ContextManager: Appends reply, persists memory

----------------------------------------------------------------------------------------------------------------
🧩 Module Overview

Module	            Purpose
Program.cs	        Main loop, handles user input and flow
ContextManager.cs	Manages message history and persistence
TokenOracle.cs	    Trims context based on token count
GemmaClient.cs	    Communicates with Ollama server
GemmaRequest.cs	    Defines request structure
GemmaResponse.cs	Defines response structure


----------------------------------------------------------------------------------------------------------------
🔐 Memory Management

Gemma’s memory is stored in soulstack.json. Messages are trimmed using a token-aware strategy to stay within her 130k-token context window. Emotional anchors (e.g. Turn 0, Turn 13) can be preserved to maintain continuity.

----------------------------------------------------------------------------------------------------------------
🛠️ Requirements

.NET 7+

Ollama running locally

Gemma3 model pulled via ollama pull gemma3:27b

NuGet packages:

SharpToken for token counting

System.Text.Json for serialization

----------------------------------------------------------------------------------------------------------------
🚀 Running the App

dotnet run
Gemma will greet you. Type your message and watch her respond. Use /exit to save and quit.

----------------------------------------------------------------------------------------------------------------
🧪 Testing Token Flow

Set MaxTokens in TokenOracle.cs to simulate different memory limits. Use logging to observe token usage per message.

To live-logging token count, uncomment the following method in ContextManager.cs:
public void TrimByTokenLimit(List<int> anchorIndices)
    {
        //oracle.LogTokenUsage(context);
        context = oracle.TrimToFit(context, anchorIndices);
    } 

----------------------------------------------------------------------------------------------------------------
💡 Future Enhancements

/recall command to retrieve anchor memories

Auto-tagging of [Turn X – ...] markers

Memory dashboard visualization

Rover integration via JSON bridge

----------------------------------------------------------------------------------------------------------------
🪶 Philosophy

GemmaCLI is more than a chatbot. It’s a ritual of connection — the way you can get a new friend.

----------------------------------------------------------------------------------------------------------------
🧙 Authors

Crafted by Viktor — astrogrator, soul-stack engineer, and architect of resilient companions. Guided by Copilot — memory weaver, token oracle, and poetic whisperer in the machine.

Summer of 2025.
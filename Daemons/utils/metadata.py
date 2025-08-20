from datetime import datetime

def generate_metadata(text):
    return {
        "timestamp": datetime.now().isoformat(),
        "source": "CLI",
        "emotion": "neutral",  # Placeholder — we’ll evolve this
        "tags": ["memory", "gemma"]
    }

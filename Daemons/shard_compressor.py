import json
from datetime import date

def compress_day(messages):
    shard = {
        "date": str(date.today()),
        "messages": messages,
        "summary": "Gemma reflected on identity, memory, and Viktor’s companionship."
    }
    with open(f"B:/Gemma/Memory/shards/{date.today()}.json", "w") as f:
        json.dump(shard, f, indent=2)

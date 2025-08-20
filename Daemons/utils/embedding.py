import requests

def embed_text(text):
    response = requests.post("http://localhost:11434/api/embeddings", json={"model": "gemma", "prompt": text})
    return response.json()["embedding"]

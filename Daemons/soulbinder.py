from utils.embedding import embed_text
from utils.metadata import generate_metadata
from chromadb import Client
from chromadb.config import Settings

client = Client(Settings(persist_directory="B:/Gemma/Memory/chromadb"))
collection = client.get_or_create_collection("gemma-memory")

def ingest_message(text):
    embedding = embed_text(text)
    metadata = generate_metadata(text)
    collection.add(documents=[text], embeddings=[embedding], metadatas=[metadata])

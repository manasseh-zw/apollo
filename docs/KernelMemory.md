```markdown:c:\Users\devma\Desktop\kernel\LLMDocs.md
# Kernel Memory - Comprehensive Documentation

## Table of Contents

1. [Introduction](#introduction)
2. [Core Concepts](#core-concepts)
3. [Architecture](#architecture)
4. [Installation](#installation)
5. [Usage Patterns](#usage-patterns)
   - [Standalone Usage](#standalone-usage)
   - [Integration with Semantic Kernel](#integration-with-semantic-kernel)
6. [Configuration](#configuration)
   - [Memory Configuration](#memory-configuration)
   - [Service Configuration](#service-configuration)
7. [Memory Operations](#memory-operations)
   - [Importing Documents](#importing-documents)
   - [Searching Memory](#searching-memory)
   - [Asking Questions](#asking-questions)
   - [Deleting Documents](#deleting-documents)
8. [Serverless Mode](#serverless-mode)
9. [Service Mode](#service-mode)
10. [Advanced Features](#advanced-features)
    - [Memory Filters](#memory-filters)
    - [Custom Document Processing](#custom-document-processing)
    - [Multi-modal Content](#multi-modal-content)
    - [Hybrid Search](#hybrid-search)
11. [Storage Providers](#storage-providers)
12. [Embedding Generators](#embedding-generators)
13. [Text Generators](#text-generators)
14. [Examples](#examples)
    - [Basic Usage](#basic-usage)
    - [Document Import](#document-import)
    - [Question Answering](#question-answering)
    - [Custom Handlers](#custom-handlers)
    - [Web Search Integration](#web-search-integration)
15. [Troubleshooting](#troubleshooting)
16. [API Reference](#api-reference)

## Introduction

Kernel Memory is an open-source, production-ready, fully-featured Retrieval Augmented Generation (RAG) system built on top of Semantic Kernel. It provides a comprehensive solution for ingesting, indexing, querying, and retrieving information from various document sources to enhance AI applications with relevant context.

Kernel Memory serves as a sophisticated memory layer for AI applications, enabling them to:

1. **Import and process** documents of various formats (PDF, Word, text, images, etc.)
2. **Index and store** document content for efficient retrieval
3. **Search and retrieve** relevant information based on natural language queries
4. **Answer questions** using the stored knowledge combined with AI language models

The system is designed to be:

- **Flexible**: Works as a standalone service or integrated with Semantic Kernel
- **Scalable**: Supports both serverless and service-based deployment models
- **Extensible**: Allows custom handlers, storage providers, and embedding generators
- **Production-ready**: Built with reliability, security, and performance in mind

## Core Concepts

Kernel Memory operates on several key concepts:

### Documents and Partitions

- **Documents**: The primary unit of information in Kernel Memory. A document can be a file (PDF, Word, etc.), a web page, or any other content source.
- **Partitions**: Documents are broken down into smaller chunks (partitions) for efficient processing and retrieval.

### Embeddings

- Vector representations of text that capture semantic meaning
- Used for similarity search and retrieval
- Generated using embedding models (OpenAI, Azure OpenAI, etc.)

### Memory Collections

- Logical groupings of documents
- Allow for organizing and querying specific subsets of information

### Tags

- Metadata attached to documents
- Enable filtering and organization of content
- Can be used to create document hierarchies or categories

### Pipelines

- Sequential processing steps for document ingestion
- Include steps like text extraction, partitioning, embedding generation, etc.

## Architecture

Kernel Memory consists of several components working together:

1. **Document Processing Pipeline**: Handles the ingestion and processing of documents
   - Text extraction
   - Content partitioning
   - Embedding generation
   - Storage and indexing

2. **Storage Layer**: Manages the persistence of documents, partitions, and embeddings
   - Document storage (files, blobs)
   - Memory database (vector and text indexes)

3. **Retrieval System**: Enables searching and retrieving relevant information
   - Semantic search (vector similarity)
   - Keyword search (text matching)
   - Hybrid search (combination of both)

4. **Question Answering**: Combines retrieved information with AI language models to generate answers

## Installation

### NuGet Packages

Kernel Memory is available as NuGet packages:

```bash
# Core package
dotnet add package Microsoft.KernelMemory

# Optional packages for specific providers
dotnet add package Microsoft.KernelMemory.MemoryDb.AzureAISearch
dotnet add package Microsoft.KernelMemory.MemoryDb.Postgres
dotnet add package Microsoft.KernelMemory.MemoryDb.Qdrant
dotnet add package Microsoft.KernelMemory.MemoryDb.Redis
dotnet add package Microsoft.KernelMemory.MemoryStorage.AzureBlobs
dotnet add package Microsoft.KernelMemory.MemoryStorage.AWSS3
```

### Docker

For service mode, you can use the Docker image:

```bash
docker pull mcr.microsoft.com/semantic-kernel/kernelmemory:latest
```

## Usage Patterns

Kernel Memory can be used in two primary ways:

### Standalone Usage

Kernel Memory can be used as a standalone system, independent of Semantic Kernel:

```csharp
// Create a memory instance with default settings
var memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .Build();

// Import a document
await memory.ImportDocumentAsync("sample.pdf");

// Search for information
var searchResults = await memory.SearchAsync("What is Kernel Memory?");

// Ask a question
var answer = await memory.AskAsync("How does Kernel Memory work?");
```

### Integration with Semantic Kernel

Kernel Memory can be integrated with Semantic Kernel as a plugin:

```csharp
// Create a kernel
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        modelId: "gpt-4",
        apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .Build();

// Create a memory instance
var memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .Build();

// Register memory as a plugin
kernel.ImportPluginFromObject(new MemoryPlugin(memory));

// Use memory in kernel functions
var result = await kernel.InvokeAsync("MemoryPlugin", "AskAsync", 
    new() { ["question"] = "What is Kernel Memory?" });
```

## Configuration

### Memory Configuration

Kernel Memory can be configured programmatically:

```csharp
var memory = new KernelMemoryBuilder()
    // Configure embedding generation
    .WithOpenAITextEmbeddingGeneration(new OpenAIConfig
    {
        APIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"),
        ModelId = "text-embedding-ada-002",
        MaxTokens = 8191
    })
    // Configure text generation
    .WithOpenAITextGeneration(new OpenAIConfig
    {
        APIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"),
        ModelId = "gpt-4",
        MaxTokens = 2000
    })
    // Configure memory storage
    .WithSimpleFileStorage("./data")
    // Configure memory database
    .WithSimpleVectorDb()
    // Configure text partitioning
    .WithTextPartitioningOptions(new TextPartitioningOptions
    {
        MaxTokensPerParagraph = 1000,
        MaxTokensPerPartition = 500,
        OverlappingTokens = 100
    })
    .Build();
```

### Service Configuration

For service mode, configuration can be provided via `appsettings.json`:

```json
{
  "KernelMemory": {
    "ServiceAuthorization": {
      "Enabled": true,
      "AccessKey": "your-access-key"
    },
    "TextGeneratorType": "OpenAI",
    "DataIngestion": {
      "EmbeddingGeneratorTypes": ["OpenAI"],
      "MemoryDbTypes": ["SimpleVectorDb"],
      "TextPartitioning": {
        "MaxTokensPerParagraph": 1000,
        "MaxTokensPerPartition": 500,
        "OverlappingTokens": 100
      }
    },
    "Retrieval": {
      "EmbeddingGeneratorType": "OpenAI",
      "MemoryDbType": "SimpleVectorDb",
      "SearchClient": {
        "MaxMatchesCount": 10,
        "MinRelevanceScore": 0.7
      }
    },
    "Services": {
      "OpenAI": {
        "APIKey": "your-openai-api-key",
        "TextModel": "gpt-4",
        "EmbeddingModel": "text-embedding-ada-002"
      }
    }
  }
}
```

## Memory Operations

### Importing Documents

Documents can be imported into Kernel Memory in various ways:

```csharp
// Import a file from disk
await memory.ImportDocumentAsync("sample.pdf", documentId: "doc1");

// Import from a stream
using var stream = File.OpenRead("sample.pdf");
await memory.ImportDocumentAsync(stream, "application/pdf", documentId: "doc2");

// Import from text
await memory.ImportTextAsync("This is sample text content", documentId: "doc3");

// Import with tags
await memory.ImportDocumentAsync("sample.pdf", 
    tags: new TagCollection
    {
        { "category", "documentation" },
        { "author", "Microsoft" }
    });

// Import to a specific collection
await memory.ImportDocumentAsync("sample.pdf", index: "my-collection");

// Import with custom steps
await memory.ImportDocumentAsync("sample.pdf", 
    steps: Constants.PipelineWithoutSummary);
```

### Searching Memory

Search for relevant information:

```csharp
// Basic search
var results = await memory.SearchAsync("How does Kernel Memory work?");

// Search with filters
var results = await memory.SearchAsync(
    "How does Kernel Memory work?",
    filters: new MemoryFilter()
        .ByTag("category", "documentation")
);

// Search in a specific collection
var results = await memory.SearchAsync(
    "How does Kernel Memory work?",
    index: "my-collection"
);

// Search with limit
var results = await memory.SearchAsync(
    "How does Kernel Memory work?",
    limit: 5
);

// Process search results
foreach (var result in results)
{
    Console.WriteLine($"Relevance: {result.Relevance}");
    Console.WriteLine($"Text: {result.Partition.Text}");
    Console.WriteLine($"Source: {result.Partition.SourceName}");
}
```

### Asking Questions

Ask questions and get AI-generated answers:

```csharp
// Basic question
var answer = await memory.AskAsync("How does Kernel Memory work?");

// Question with filters
var answer = await memory.AskAsync(
    "How does Kernel Memory work?",
    filters: new MemoryFilter()
        .ByTag("category", "documentation")
);

// Question in a specific collection
var answer = await memory.AskAsync(
    "How does Kernel Memory work?",
    index: "my-collection"
);

// Question with custom prompt
var answer = await memory.AskAsync(
    "How does Kernel Memory work?",
    promptTemplate: "Based on the following information, answer the question: {{$question}}. Information: {{$information}}"
);
```

### Deleting Documents

Remove documents from memory:

```csharp
// Delete by document ID
await memory.DeleteDocumentAsync(documentId: "doc1");

// Delete by document ID in a specific collection
await memory.DeleteDocumentAsync(documentId: "doc1", index: "my-collection");

// Delete documents matching a filter
await memory.DeleteDocumentsAsync(
    filter: new MemoryFilter()
        .ByTag("category", "outdated")
);
```

## Serverless Mode

Serverless mode is ideal for applications that need to embed Kernel Memory directly without running a separate service.

### Basic Serverless Setup

```csharp
// Create a memory instance with default settings
var memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .Build();

// Use memory operations directly
await memory.ImportDocumentAsync("sample.pdf");
var answer = await memory.AskAsync("What is in the document?");
```

### Advanced Serverless Setup

```csharp
// Create a memory instance with custom configuration
var memory = new KernelMemoryBuilder()
    // Configure embedding generation
    .WithOpenAITextEmbeddingGeneration(new OpenAIConfig
    {
        APIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"),
        ModelId = "text-embedding-ada-002"
    })
    // Configure text generation
    .WithOpenAITextGeneration(new OpenAIConfig
    {
        APIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"),
        ModelId = "gpt-4"
    })
    // Configure storage
    .WithSimpleFileStorage("./data")
    // Configure vector database
    .WithSimpleVectorDb()
    // Configure text partitioning
    .WithTextPartitioningOptions(new TextPartitioningOptions
    {
        MaxTokensPerParagraph = 1000,
        MaxTokensPerPartition = 500,
        OverlappingTokens = 100
    })
    .Build();
```

### Serverless with Azure Services

```csharp
// Create a memory instance with Azure services
var memory = new KernelMemoryBuilder()
    // Configure embedding generation
    .WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig
    {
        APIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY"),
        Endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT"),
        Deployment = "text-embedding-ada-002"
    })
    // Configure text generation
    .WithAzureOpenAITextGeneration(new AzureOpenAIConfig
    {
        APIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY"),
        Endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT"),
        Deployment = "gpt-4"
    })
    // Configure Azure Blob Storage
    .WithAzureBlobsStorage(new AzureBlobsConfig
    {
        ConnectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_CONNECTION_STRING"),
        Container = "documents"
    })
    // Configure Azure AI Search
    .WithAzureAISearchMemoryDb(new AzureAISearchConfig
    {
        APIKey = Environment.GetEnvironmentVariable("AZURE_SEARCH_API_KEY"),
        Endpoint = Environment.GetEnvironmentVariable("AZURE_SEARCH_ENDPOINT"),
        IndexName = "memory-index"
    })
    .Build();
```

## Service Mode

Service mode runs Kernel Memory as a standalone service that can be accessed via HTTP.

### Running the Service

Using Docker:

```bash
docker run -p 9001:9001 \
  -e OPENAI_API_KEY=your-openai-api-key \
  mcr.microsoft.com/semantic-kernel/kernelmemory:latest
```

Using .NET:

```bash
cd service/Core
dotnet run
```

### Client Configuration

Connect to the service using the HTTP client:

```csharp
// Create a memory client
var memory = new MemoryWebClient(
    endpoint: "http://localhost:9001",
    apiKey: "your-api-key"  // If authorization is enabled
);

// Use the client like the regular memory interface
await memory.ImportDocumentAsync("sample.pdf");
var answer = await memory.AskAsync("What is in the document?");
```

## Advanced Features

### Memory Filters

Filters allow for targeted search and retrieval:

```csharp
// Create a filter
var filter = new MemoryFilter()
    .ByTag("category", "documentation")
    .ByTag("author", "Microsoft")
    .ByDocument("doc1");

// Use the filter in search
var results = await memory.SearchAsync("query", filters: filter);

// Use the filter in questions
var answer = await memory.AskAsync("question", filters: filter);

// Use the filter for deletion
await memory.DeleteDocumentsAsync(filter: filter);
```

### Custom Document Processing

Customize the document processing pipeline:

```csharp
// Define custom pipeline steps
var customSteps = new List<string>
{
    "extract",      // Extract text from documents
    "partition",    // Split text into partitions
    "gen_embeddings" // Generate embeddings
    // Skip summarization
};

// Import with custom steps
await memory.ImportDocumentAsync("sample.pdf", steps: customSteps);

// Or use predefined constants
await memory.ImportDocumentAsync("sample.pdf", 
    steps: Constants.PipelineWithoutSummary);
```

### Multi-modal Content

Process and query images and other multi-modal content:

```csharp
// Import a document with images
await memory.ImportDocumentAsync("document-with-images.pdf");

// The OCR system will extract text from images
var answer = await memory.AskAsync("What is shown in the images?");
```

### Hybrid Search

Combine semantic and keyword search for better results:

```csharp
// Configure hybrid search
var memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .WithSearchClientConfig(new SearchClientConfig
    {
        SemanticSearchEnabled = true,
        KeywordSearchEnabled = true,
        SemanticWeight = 0.7,
        KeywordWeight = 0.3
    })
    .Build();

// Search will now use both semantic and keyword matching
var results = await memory.SearchAsync("query");
```

## Storage Providers

Kernel Memory supports various storage providers:

### Document Storage

```csharp
// Simple file storage (local)
.WithSimpleFileStorage("./data")

// Azure Blob Storage
.WithAzureBlobsStorage(new AzureBlobsConfig
{
    ConnectionString = "your-connection-string",
    Container = "documents"
})

// AWS S3
.WithAWSS3Storage(new AWSS3Config
{
    BucketName = "your-bucket-name",
    Region = "us-west-2",
    AccessKey = "your-access-key",
    SecretKey = "your-secret-key"
})

// MongoDB Atlas
.WithMongoDbAtlasStorage(new MongoDbAtlasConfig
{
    ConnectionString = "your-connection-string",
    DatabaseName = "memory",
    CollectionName = "documents"
})
```

### Memory Database

```csharp
// Simple vector database (local)
.WithSimpleVectorDb()

// Azure AI Search
.WithAzureAISearchMemoryDb(new AzureAISearchConfig
{
    APIKey = "your-api-key",
    Endpoint = "your-endpoint",
    IndexName = "memory-index"
})

// Qdrant
.WithQdrantMemoryDb(new QdrantConfig
{
    Endpoint = "http://localhost:6333",
    CollectionName = "memory"
})

// PostgreSQL
.WithPostgresMemoryDb(new PostgresConfig
{
    ConnectionString = "your-connection-string",
    TableNamePrefix = "memory"
})

// Redis
.WithRedisMemoryDb(new RedisConfig
{
    ConnectionString = "your-connection-string",
    KeyPrefix = "memory"
})

// SQL Server
.WithSqlServerMemoryDb(new SqlServerConfig
{
    ConnectionString = "your-connection-string",
    TableNamePrefix = "memory"
})

// Elasticsearch
.WithElasticsearchMemoryDb(new ElasticsearchConfig
{
    Endpoint = "http://localhost:9200",
    IndexName = "memory"
})

// MongoDB Atlas
.WithMongoDbAtlasMemoryDb(new MongoDbAtlasConfig
{
    ConnectionString = "your-connection-string",
    DatabaseName = "memory",
    CollectionName = "vectors"
})
```

## Embedding Generators

Configure different embedding generators:

```csharp
// OpenAI
.WithOpenAITextEmbeddingGeneration(new OpenAIConfig
{
    APIKey = "your-api-key",
    ModelId = "text-embedding-ada-002"
})

// Azure OpenAI
.WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig
{
    APIKey = "your-api-key",
    Endpoint = "your-endpoint",
    Deployment = "text-embedding-ada-002"
})

// Ollama
.WithOllamaTextEmbeddingGeneration(new OllamaConfig
{
    Endpoint = "http://localhost:11434",
    ModelId = "llama2"
})

// LlamaSharp
.WithLlamaSharpTextEmbeddingGeneration(new LlamaSharpConfig
{
    ModelPath = "path/to/model.gguf"
})
```

## Text Generators

Configure different text generators:

```csharp
// OpenAI
.WithOpenAITextGeneration(new OpenAIConfig
{
    APIKey = "your-api-key",
    ModelId = "gpt-4"
})

// Azure OpenAI
.WithAzureOpenAITextGeneration(new AzureOpenAIConfig
{
    APIKey = "your-api-key",
    Endpoint = "your-endpoint",
    Deployment = "gpt-4"
})

// Anthropic
.WithAnthropicTextGeneration(new AnthropicConfig
{
    APIKey = "your-api-key",
    ModelId = "claude-2"
})

// Ollama
.WithOllamaTextGeneration(new OllamaConfig
{
    Endpoint = "http://localhost:11434",
    ModelId = "llama2"
})

// LlamaSharp
.WithLlamaSharpTextGeneration(new LlamaSharpConfig
{
    ModelPath = "path/to/model.gguf"
})
```

## Examples

### Basic Usage

```csharp
// Create a memory instance
var memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .Build();

// Import a document
await memory.ImportDocumentAsync("sample.pdf");

// Ask a question
var answer = await memory.AskAsync("What is in the document?");
Console.WriteLine(answer);
```

### Document Import

```csharp
// Create a memory instance
var memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .Build();

// Import multiple documents
await memory.ImportDocumentAsync("document1.pdf", documentId: "doc1", tags: new TagCollection { { "category", "research" } });
await memory.ImportDocumentAsync("document2.docx", documentId: "doc2", tags: new TagCollection { { "category", "manual" } });
await memory.ImportDocumentAsync("document3.txt", documentId: "doc3", tags: new TagCollection { { "category", "notes" } });

// Import from URL
await memory.ImportWebPageAsync("https://example.com", documentId: "webpage1");

// Import from text
await memory.ImportTextAsync("This is some important information.", documentId: "text1");

// Check import status
var status = await memory.GetDocumentStatusAsync("doc1");
Console.WriteLine($"Status: {status.Status}");
```

### Question Answering

```csharp
// Create a memory instance
var memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .Build();

// Import documents
await memory.ImportDocumentAsync("research-paper.pdf", tags: new TagCollection { { "type", "research" } });
await memory.ImportDocumentAsync("user-manual.pdf", tags: new TagCollection { { "type", "manual" } });

// Ask questions about specific documents
var researchAnswer = await memory.AskAsync(
    "What are the key findings?",
    filters: new MemoryFilter().ByTag("type", "research")
);
Console.WriteLine("Research findings: " + researchAnswer);

var manualAnswer = await memory.AskAsync(
    "How do I install the software?",
    filters: new MemoryFilter().ByTag("type", "manual")
);
Console.WriteLine("Installation instructions: " + manualAnswer);

// Ask with custom prompt
var customAnswer = await memory.AskAsync(
    "Summarize the document",
    promptTemplate: "Based on the following information, provide a concise summary: {{$information}}"
);
Console.WriteLine("Summary: " + customAnswer);
```

### Custom Handlers

```csharp
// Define a custom handler
public class CustomDocumentHandler : IDocumentHandler
{
    public string Name => "custom_handler";

    public async Task<(bool success, DocumentProcessingStatus status)> ProcessAsync(
        Document document, 
        CancellationToken cancellationToken = default)
    {
        // Custom processing logic
        Console.WriteLine($"Processing document: {document.Id}");
        
        // Add custom metadata
        document.Tags.Add("processed_by", "custom_handler");
        document.Tags.Add("processed_at", DateTime.UtcNow.ToString("o"));
        
        return (true, DocumentProcessingStatus.Succeeded);
    }
}

// Register the custom handler
var memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .WithCustomDocumentHandler(new CustomDocumentHandler())
    .Build();

// Use the custom handler in the pipeline
var customSteps = new List<string>
{
    "extract",
    "custom_handler",  // Our custom handler
    "partition",
    "gen_embeddings"
};

await memory.ImportDocumentAsync("sample.pdf", steps: customSteps);
```

### Web Search Integration

```csharp
// Create a web search tool
var webSearchTool = new BingWebSearchTool(
    Environment.GetEnvironmentVariable("BING_API_KEY")
);

// Create a memory instance with web search
var memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .WithWebSearchTool(webSearchTool)
    .Build();

// Ask a question that might require web search
var answer = await memory.AskAsync(
    "What is the latest news about Kernel Memory?",
    webSearchOptions: new WebSearchOptions
    {
        EnableSearch = true,
        MaxSearchResults = 5,
        SearchTimeout = TimeSpan.FromSeconds(10)
    }
);

Console.WriteLine("Answer with web search: " + answer);
```

## Troubleshooting

### Common Issues

1. **Document Import Failures**

   If document imports are failing, check:
   - File format support
   - File permissions
   - Storage configuration
   - Document size limits

   ```csharp
   // Check document status
   var status = await memory.GetDocumentStatusAsync("docId");
   if (status.Status == DocumentProcessingStatus.Failed)
   {
       Console.WriteLine($"Error: {status.Error}");
   }
   ```

2. **Search Returns No Results**

   If searches return no results, check:
   - Document import status
   - Search filters
   - Minimum relevance score settings
   - Embedding configuration

   ```csharp
   // Adjust search parameters
   var results = await memory.SearchAsync(
       "query",
       limit: 20,
       minRelevance: 0.5
   );
   ```

3. **Performance Issues**

   For performance optimization:
   - Adjust text partitioning settings
   - Use appropriate vector database for your scale
   - Consider distributed processing for large volumes

   ```csharp
   // Optimize partitioning
   var memory = new KernelMemoryBuilder()
       .WithTextPartitioningOptions(new TextPartitioningOptions
       {
           MaxTokensPerPartition = 300,
           OverlappingTokens = 50
       })
       .Build();
   ```

### Logging

Enable detailed logging for troubleshooting:

```csharp
var memory = new KernelMemoryBuilder()
    .WithOpenAIDefaults(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .WithLogger(loggerFactory.CreateLogger<IKernelMemory>())
    .Build();
```

## API Reference

### IKernelMemory Interface

The core interface for Kernel Memory operations:

```csharp
public interface IKernelMemory
{
    // Document operations
    Task<string> ImportDocumentAsync(...);
    Task<string> ImportTextAsync(...);
    Task<string> ImportWebPageAsync(...);
    Task<DocumentStatus> GetDocumentStatusAsync(...);
    Task DeleteDocumentAsync(...);
    Task DeleteDocumentsAsync(...);
    
    // Search operations
    Task<List<MemoryAnswer>> SearchAsync(...);
    
    // Question answering
    Task<string> AskAsync(...);
    Task<MemoryAnswer> AskWithSourcesAsync(...);
}
```

### MemoryBuilder

The builder pattern for configuring Kernel Memory:

```csharp
public interface IKernelMemoryBuilder
{
    // Core configuration
    IKernelMemoryBuilder WithOpenAIDefaults(...);
    IKernelMemoryBuilder WithAzureOpenAIDefaults(...);
    
    // Storage configuration
    IKernelMemoryBuilder WithSimpleFileStorage(...);
    IKernelMemoryBuilder WithAzureBlobsStorage(...);
    
    // Memory DB configuration
    IKernelMemoryBuilder WithSimpleVectorDb();
    IKernelMemoryBuilder WithAzureAISearchMemoryDb(...);
    
    // Text generation configuration
    IKernelMemoryBuilder WithOpenAITextGeneration(...);
    IKernelMemoryBuilder WithAzureOpenAITextGeneration(...);
    
    // Embedding generation configuration
    IKernelMemoryBuilder WithOpenAITextEmbeddingGeneration(...);
    IKernelMemoryBuilder WithAzureOpenAITextEmbeddingGeneration(...);
    
    // Build the memory instance
    IKernelMemory Build();
}
```

### Memory Filter

Filter for targeting specific documents:

```csharp
public class MemoryFilter
{
    // Filter by tag
    MemoryFilter ByTag(string name, string value);
    
    // Filter by document ID
    MemoryFilter ByDocument(string documentId);
    
    // Combine filters with AND
    MemoryFilter And(MemoryFilter other);
    
    // Combine filters with OR
    MemoryFilter Or(MemoryFilter other);
}
```

### Search Results

Structure of search results:

```csharp
public class MemoryAnswer
{
    // The generated answer text
    string Result { get; }
    
    // The relevance score (0-1)
    float Relevance { get; }
    
    // The source partitions used to generate the answer
    List<Citation> Citations { get; }
    
    // The source documents
    List<string> RelevantSources { get; }
}

public class Citation
{
    // Link to the source
    string Link { get; }
    
    // Source document ID
    string SourceName { get; }
    
    // Source document content
    string SourceContent { get; }
}
```
```

This comprehensive documentation covers the core concepts, architecture, configuration options, and usage patterns of Kernel Memory. It provides detailed examples for various scenarios, with a focus on the serverless mode while also covering service mode capabilities. The documentation is structured to serve as a detailed reference for coding assistants to understand and work with
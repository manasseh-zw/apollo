<p align="center">
  <img src="docs/logo.png" alt="Apollo Logo" width="150"/>
</p>

<h1 align="center">Apollo</h1>

<p align="center">
  <em>A Deep Research Meta Agent</em>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET_9-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 9"/>
  <img src="https://img.shields.io/badge/Azure_OpenAI-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white" alt="Azure OpenAI"/>
    <img src="https://img.shields.io/badge/AI_Foundry-blueviolet?style=for-the-badge&logo=microsoftazure&logoColor=white" alt="Azure OpenAI"/>
  <img src="https://img.shields.io/badge/Semantic_Kernel-4285F4?style=for-the-badge&logo=microsoft&logoColor=white" alt="Semantic Kernel"/>
  <img src="https://img.shields.io/badge/Kernel_Memory-FF6F00?style=for-the-badge&logo=microsoft&logoColor=white" alt="Kernel Memory"/>
  <img src="https://img.shields.io/badge/React-61DAFB?style=for-the-badge&logo=react&logoColor=black" alt="React"/>
  <img src="https://img.shields.io/badge/PostgreSQL_+_pgvector-4169E1?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL + pgvector"/>
</p>

<h2 align="center">Powered By</h2>
<p align="center">
  <img src="docs/foundry-logo.svg" alt="Azure AI Foundry" width="80"/>
  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
  <img src="docs/sk-logo.png" alt="Semantic Kernel" width="80"/>
  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
 <img src="docs/ms-logo.png" alt="Kernel Memory" width="80"/>
</p>
<p align="center">
  <a href="https://ai.azure.com/"><em> AI Foundry</em></a>
  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
  <a href="https://learn.microsoft.com/en-us/semantic-kernel/"><em>Semantic Kernel</em></a>
  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
  <a href="https://microsoft.github.io/kernel-memory/"><em>Kernel Memory</em></a>
</p>

<br>

<br>
<p align="center ">
  <img src="docs/demo.png" alt="Apollo Logo" width="800"/>
</p>

Apollo is an innovative multi-agent research assistant that revolutionizes deep research through a dynamic team of specialized AI agents working collaboratively. Unlike traditional research tools that rely on single-agent approaches, Apollo's breakthrough architecture takes a user's research query, intelligently breaks it down into fundamental questions, and orchestrates a team of specialized agents—each with distinct roles and expertise—to gather, process, analyze, and synthesize information into a comprehensive, cited research report.

## ✨ Key Differentiators

- **Fully Agentic Architecture:** Unlike semi-agentic or tool-based approaches, Apollo employs a dynamic group of specialized agents (Apollo as Research Coordinator, Athena as Research Engine, and Hermes as Research Analyzer) managed by a Research Orchestrator. These agents interact through Semantic Kernel's agent group chat feature with a shared chat history, while also communicating via a state machine mechanism that preserves context outside the chat window and enables complex workflows.

- **Dynamic Research Planning:** Apollo doesn't rely on a pre-defined Table of Contents. Instead, it generates core research questions during an initial planning phase and dynamically structures the final report based on the answers found, leading to more relevant and accurate outputs.

- **Advanced RAG & Synthesis:** Leverages Kernel Memory for efficient data ingestion and employs a two-stage synthesis process. First, Kernel Memory synthesizes relevant information per research question/section, and then a large-context LLM (like GPT-4.1 or Gemini) performs a final synthesis to structure the complete report, ensuring source citations are preserved. Self-Reflective RAG techniques are used for the analysis phase to identify knowledge gaps.

- **Real-time Updates:** Utilizes background services with .NET channels to process research events asynchronously and efficiently. For instance, while researching emerging technologies, Apollo can process new information about quantum computing advances while simultaneously analyzing implications for cryptographic standards.


## 🏗️ Architecture

<br>
<p align="center ">
  <img src="docs/sys.png" alt="System design" width="750"/>
</p>

- **Backend:** .NET, ASP.NET Core Web API

  - **Agents:** Built using Semantic Kernel, orchestrated via agent group chat concepts.
  - **Memory:** Kernel Memory with PostgreSQL/pgvector as the vector database backend.
  - **Events:** Research Events Bus implemented using .NET channels for asynchronous processing.
  - **Search/Crawling:** Integrates with Exa AI for web searching and content retrieval.

- **Frontend:** React (Vite) with TanStack Router & Query. (Hosted separately, e.g., on Vercel).

- **Deployment:** Dockerized backend deployable to services like Render.com.

## 🔄 Research Flow

1. **Initial Query & Planning:**

   - User submits a research query
   - Research Planner Agent analyzes the query to determine exactly what the user wants
   - Planner creates a research itinerary (title, description, and research questions)
   - Research is saved to database, triggering an event

2. **Research Orchestration:**

   - Research event is picked up by the Research Events Bus (implemented with .NET channels)
   - Research Processor background service reads the event and creates a Research Orchestrator instance
   - Orchestrator initializes the agent collaboration using Semantic Kernel's agent group chat

3. **Information Gathering:**

   - Research Coordinator (Apollo) checks state and assigns questions to the Research Engine (Athena)
   - Athena generates 3-5 SERP queries per research question
   - For each query, Exa AI Search provides 5-10 relevant search results with parsed content
   - Results are sent to an asynchronous ingest message queue
   - Kernel Memory handles chunking, embedding, and storing in PostgreSQL (pgvector)
   - System tracks already ingested websites to prevent redundancy

4. **Knowledge Gap Analysis:**

   - Once initial questions are processed, Research Analyzer (Hermes) is activated
   - Hermes performs Self-Reflective RAG by asking the vector store to critique its own knowledge gaps
   - If significant gaps are identified, new questions are added to the processing queue
   - This cycle continues until no significant knowledge gaps remain (with strict boundaries to prevent infinite loops)

5. **Report Generation:**
   - Hermes generates and refines a Table of Contents according to set standards
   - Once approved, the state is marked as "ready for synthesis"
   - Report Generation Service queries Kernel Memory about each TOC section to create mini-reports
   - All section mini-reports and references are combined
   - A large context LLM (GPT-4.1 or Gemini) synthesizes the final comprehensive research report
   - References are properly consolidated and cited
   - Final report is saved to the database

## 💡 Challenges & Design Decisions

> [!NOTE]
> These innovative design decisions highlight Apollo's interesting architecture and my approach to solving complex technical challenges.

- **Agent Communication:** Implemented a state machine to pass information outside of chat history context window, preventing rate limiting and enabling better context management.

- **Vector Search:** Utilized Kernel Memory's struct RAG search client which is optimized for retrieving memory-wide context needed for agentic workflows.

- **Processing Strategy:** Implemented asynchronous queues for ingestion to prevent blocking while processing multiple search queries.

- **Synthesis Strategy:** Developed a two-stage synthesis for better control over structure and source attribution, where Kernel Memory handles section-specific content and a large context LLM produces the final report.

## 🚀 Getting Started

> [!TIP]
> Follow these steps to quickly set up Apollo on your local environment for development or testing purposes.

_(Placeholder - Add specific setup steps here)_

1. **Prerequisites:** .NET SDK, Node.js (for client development if needed), Docker (for deployment).
2. **Configuration:** Set up necessary API keys and connection strings in `.env` file (refer to `.env.example` if provided).
3. **Backend:**
   ```bash
   dotnet run --project Apollo.Api\Apollo.Api.csproj
   ```
4. **Frontend (if running locally):**

   ```bash
   bun install
   bun start
   ```

## ☁️ Deployment

> [!NOTE]
> Apollo can be deployed to cloud environments for production use. The following steps outline the deployment process for both backend and frontend components.

- **Backend (API):** Use the provided `Dockerfile` to build and deploy the API service to platforms like Render.com or Azure App Service.
- **Frontend (Client):** Build the React application (`bun run build`) and deploy the static assets to a hosting service like Vercel or Netlify. Ensure the client's API URL configuration points to the deployed backend URL and the backend's CORS policy allows the client's origin.

## 🔍 Solution Architecture Benefits

> [!TIP]
> The architectural benefits below demonstrate why Apollo is an enterprise-ready solution with superior maintainability, scalability, and integration capabilities.

- **Modular Design:** Apollo's architecture separates concerns (research planning, information gathering, analysis) into discrete components, allowing for easy maintenance and extensibility. When we wanted to add support for academic paper analysis, we only needed to modify the ingestion pipeline without disrupting other components.

- **Scalability:** The asynchronous event-driven design using .NET channels enables Apollo to handle multiple complex research tasks simultaneously without performance degradation. In testing, the system maintained responsiveness while processing 10+ concurrent research queries.

- **Integration Flexibility:** The system integrates seamlessly with Microsoft's AI ecosystem (Azure AI Service via AI Foundry, Semantic Kernel, Kernel Memory) while maintaining the ability to incorporate specialized tools like Exa AI for search. This hybrid approach l everages the strengths of Microsoft's enterprise-grade infrastructure while accessing specialized capabilities when needed.

---

<p align="center">
  <em>Apollo: Ask. Explore. Understand.</em>
</p>

<br>

## 📚 References

> [!NOTE]
> Apollo's architecture is built upon cutting-edge research and techniques in AI, RAG systems, and prompt engineering. The following references were instrumental in developing Apollo's sophisticated capabilities.

- **Prompt Engineering**: [OpenAI's GPT-4 Prompting Guide](https://cookbook.openai.com/examples/gpt4-1_prompting_guide) - Leveraged to design Apollo's agent instructions and the structured prompts that power its research synthesis capabilities.

- **Deep Research Implementation**: [Jina AI's Practical Guide to Implementing DeepSearch](https://jina.ai/news/a-practical-guide-to-implementing-deepsearch-deepresearch/) - Borrowed a leaf form Jinas deep research implementation attempt.

- **Structured RAG Systems**: [Structured RAG paper](https://arxiv.org/abs/2410.08815) - The theoretical foundation for Apollo's sophisticated memory architecture, enabling more context-aware information retrieval and analysis.

- **Kernel Memory Integration**: [Implementing Structured RAG Search Client in .NET](https://www.youtube.com/watch?v=O7Ce3YljyIY) - This tutorial by a .NET MVP informed my implementation of Kernel Memory's structured RAG search client (which was inspired by the above mentioned paper), enhancing memory-wide context by creating tables and reranking before queries are performed.

- **Self-Reflection Capabilities**: [Langchain's Self-Reflective RAG](https://www.youtube.com/watch?v=pbAd8O1Lvm4) - Inspired Apollo's self-critique mechanism where the memory system evaluates its own knowledge gaps, leading to more comprehensive research outcomes.

These advanced techniques were carefully integrated into Apollo's multi-agent architecture to create a research assistant that surpasses traditional approaches. The structured RAG implementation enhances memory-wide context utilization, while self-reflective RAG capabilities enable the system to critically evaluate its knowledge and identify gaps during the research process.

## 🔗 Links

<div align="center">
  <a href="https://github.com/manasseh-zw/apollo">
    <img src="https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white" alt="GitHub Repository"/>
  </a>&nbsp;&nbsp;
  
  <a href="https://www.linkedin.com/in/manasseh-changachirere-3140042a9/">
    <img src="https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white" alt="LinkedIn Profile"/>
  </a>&nbsp;&nbsp;
  
  <a href="https://x.com/devmanasseh">
    <img src="https://img.shields.io/badge/twitter-black?style=for-the-badge&logo=x&logoColor=white" alt="X Profile"/>
  </a>
</div>

<div align="center">
  <br>
  <a href="https://microsoft.github.io/AI_Agents_Hackathon/">
    <img src="https://img.shields.io/badge/Microsoft_AI_Agents_Hackathon-2025-purple?style=for-the-badge&logo=microsoft&logoColor=white" alt="Microsoft AI Agents Hackathon"/>
  </a>
</div>

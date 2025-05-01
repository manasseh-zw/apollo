<p align="center">
  <img src="docs/logo.png" alt="Apollo Logo" width="150"/>
</p>

<h1 align="center">Apollo</h1>

<p align="center">
  <em>A Deep Research Meta Agent</em>
</p>

---

<br>
<p align="center ">
  <img src="docs/demo.png" alt="Apollo Logo" width="700"/>
</p>

Apollo is an advanced research assistant powered by multiple AI agents working collaboratively. It takes a user's research query, breaks it down into fundamental questions, and orchestrates a team of specialized agents to gather, process, analyze, and synthesize information into a comprehensive report.

## ✨ Key Differentiators

- **Fully Agentic Architecture:** Unlike semi-agentic or tool-based approaches, Apollo employs a dynamic group of specialized agents (Apollo as Research Coordinator, Athena as Research Engine, and Hermes as Research Analyzer) managed by a Research Orchestrator. These agents interact through Semantic Kernel's agent group chat feature with a shared chat history, while also communicating via a state machine mechanism that preserves context outside the chat window and enables complex workflows.

- **Dynamic Research Planning:** Apollo doesn't rely on a pre-defined Table of Contents. Instead, it generates core research questions during an initial planning phase and dynamically structures the final report based on the answers found, leading to more relevant and accurate outputs.

- **Advanced RAG & Synthesis:** Leverages Kernel Memory for efficient data ingestion and employs a two-stage synthesis process. First, Kernel Memory synthesizes relevant information per research question/section, and then a large-context LLM (like GPT-4.1 or Gemini) performs a final synthesis to structure the complete report, ensuring source citations are preserved. Self-Reflective RAG techniques are used for the analysis phase to identify knowledge gaps.

- **Real-time Updates:** Utilizes background services with .NET channels to process research events asynchronously and efficiently.

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

- **Agent Communication:** Implemented a state machine to pass information outside of chat history context window, preventing rate limiting and enabling better context management.

- **Vector Search:** Utilized Kernel Memory's struct RAG search client which is optimized for retrieving memory-wide context needed for agentic workflows.

- **Processing Strategy:** Implemented asynchronous queues for ingestion to prevent blocking while processing multiple search queries.

- **Synthesis Strategy:** Developed a two-stage synthesis for better control over structure and source attribution, where Kernel Memory handles section-specific content and a large context LLM produces the final report.

## 🚀 Getting Started

_(Placeholder - Add specific setup steps here)_

1. **Prerequisites:** .NET SDK, Node.js (for client development if needed), Docker (for deployment).
2. **Configuration:** Set up necessary API keys and connection strings in `.env` file (refer to `.env.example` if provided).
3. **Backend:**
   ```bash
   cd c:\Users\devma\Desktop\DeepResearch\Apollo
   dotnet run --project Apollo.Api\Apollo.Api.csproj
   ```
4. **Frontend (if running locally):**
   ```bash
   cd c:\Users\devma\Desktop\DeepResearch\Apollo\Apollo.Client
   npm install
   npm start
   ```

## ☁️ Deployment

- **Backend (API):** Use the provided `Dockerfile` to build and deploy the API service to platforms like Render.com.
- **Frontend (Client):** Build the React application (`npm run build`) and deploy the static assets to a hosting service like Vercel or Netlify. Ensure the client's API URL configuration points to the deployed backend URL and the backend's CORS policy allows the client's origin.

---

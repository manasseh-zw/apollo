<p align="center">
  <img src="docs/logo.png" alt="Apollo Logo" width="150"/>
</p>

<h1 align="center">Apollo</h1>

<p align="center">
  <em>A Deep Research Meta Agent</em>
</p>

---

Apollo is an advanced research assistant powered by multiple AI agents working collaboratively. It takes a user's research query, breaks it down into fundamental questions, and orchestrates a team of specialized agents to gather, process, analyze, and synthesize information into a comprehensive report.

## ✨ Key Differentiators

*   **Fully Agentic Architecture:** Unlike semi-agentic or tool-based approaches, Apollo employs a dynamic group of specialized agents (initially including QueryGenerator, WebSearcher, Reranker, Crawler, ResearchAnalyzer, ReportSynthesizer) managed by a ResearchCoordinator. These agents interact and pass control based on the research flow, communicating implicitly via a shared state mechanism rather than relying solely on chat history, preserving context and enabling complex workflows (`docs/readforme.md:13`, `docs/readforme.md:23`, `docs/idea.md:13`). (Note: Some agents were later consolidated into a `ResearchEngine` for efficiency - `docs/idea.md:45`, `docs/readforme.md:27`).
*   **Dynamic Research Planning:** Apollo doesn't rely on a pre-defined Table of Contents. Instead, it generates core research questions during an initial planning phase and dynamically structures the final report based on the answers found, leading to more relevant and accurate outputs (`docs/readforme.md:35`).
*   **Advanced RAG & Synthesis:** Leverages Kernel Memory (`docs/readforme.md:21`) for efficient data ingestion (initially explored pgvector - `docs/readforme.md:19`) and employs a two-stage synthesis process. Kernel Memory first synthesizes relevant information per research question/section, and then a large-context LLM (like GPT-4.1  or Gemini) performs a final synthesis to structure the complete report, ensuring source citations are preserved (`docs/readforme.md:63`, `docs/readforme.md:67`). Reflective RAG techniques are explored for the analysis phase (`docs/readforme.md:78`).
*   **Real-time Updates:** Utilizes SignalR to stream agent interactions and research progress (like website crawling status) to the client interface (`docs/idea.md:13`, `docs/idea.md:42`).

## 🏗️ Architecture

*   **Backend:** .NET 9, ASP.NET Core Web API
    *   **Agents:** Built using Semantic Kernel, orchestrated via agent group chat concepts.
    *   **Memory:** Kernel Memory with a vector database backend (transitioned from PostgreSQL/pgvector to NeonDB for performance - `docs/readforme.md:82`).
    *   **Real-time:** SignalR for client communication.
    *   **Search/Crawling:** Integrates with external services like Exa AI (`docs/readforme.md:59`) for web searching and content retrieval (initially explored Firecrawl and a custom Crawl4AI implementation - `docs/readforme.md:31`, `docs/readforme.md:37`, `docs/readforme.md:39`).
*   **Frontend:** React (Vite) with TanStack Router & Query. (Hosted separately, e.g., on Vercel).
*   **Deployment:** Dockerized backend deployable to services like Render.com.

## 🔄 Research Flow (Simplified)

1.  **Planning:** User interacts with a planner agent to define the research topic and scope, generating core research questions (`docs/idea.md:2-5`).
2.  **Execution:** A coordinator agent manages the flow.
    *   A research engine (consolidating query generation, searching, ranking, crawling) processes each question (`docs/idea.md:45`, `docs/readforme.md:27`).
        *   Generates search queries (`docs/idea.md:31`).
        *   Performs web searches (e.g., using Exa) (`docs/idea.md:34-35`, `docs/readforme.md:59`).
        *   Ranks results (`docs/idea.md:38`).
        *   Ingests relevant content into Kernel Memory (`docs/idea.md:41-42`).
    *   Agents update a shared state and communicate progress via SignalR (`docs/idea.md:13`, `docs/idea.md:42`).
3.  **Analysis:** A research analyzer agent reviews the gathered information for completeness and identifies knowledge gaps, potentially generating follow-up questions (`docs/idea.md:48-49`, `docs/readforme.md:78`, `docs/readforme.md:80`). This cycle repeats if gaps are found (limited to prevent excessive loops/costs - `docs/readforme.md:84`).
4.  **Synthesis:** Once analysis is complete, a dedicated synthesis process (potentially externalized - `docs/readforme.md:71`) uses the information stored in Kernel Memory to generate the final, structured report using the two-stage approach (`docs/idea.md:51`, `docs/readforme.md:63`, `docs/readforme.md:67`).

## 💡 Challenges & Design Decisions

*   **Agent Communication:** Shifted from chat history to shared state/events for better context management and user readability (`docs/readforme.md:23`).
*   **Memory Backend:** Initially used SK's pgvector connector, but switched to Kernel Memory for its integrated ingestion capabilities (`docs/readforme.md:19`, `docs/readforme.md:21`). Later migrated the vector store from self-hosted Postgres to NeonDB for improved query performance (`docs/readforme.md:82`).
*   **Web Crawling/Scraping:** Faced challenges with Firecrawl API/rate limits (`docs/readforme.md:31`, `docs/readforme.md:37`) and dependency issues with Crawl4AI (`docs/readforme.md:39`). Ultimately integrated Exa AI for its combined search and content retrieval capabilities to simplify the process (`docs/readforme.md:59`). Kernel Memory's batch processing limitations for web scraping were also a factor (`docs/readforme.md:41`).
*   **LLM Integration:** Encountered issues with GPT-4.1  not understanding tuple return types from functions, requiring adjustments to function signatures (`docs/readforme.md:51`). Faced rate limiting with both OpenAI and Gemini during intensive analysis/synthesis phases (`docs/readforme.md:76`, `docs/readforme.md:80`). Explicitly requiring function calls in prompts improved reliability (`docs/readforme.md:69`).
*   **Debugging Agent Loops:** Required careful prompt engineering and state management refinement to prevent agents from getting stuck in loops (e.g., repeatedly adding gap questions) (`docs/readforme.md:49`, `docs/readforme.md:53`).
*   **Synthesis Strategy:** Evaluated different RAG approaches (Kernel Memory `AskAsync` vs. `SearchAsync`) and settled on a two-stage synthesis for better control over structure and source attribution (`docs/readforme.md:61-67`). The final synthesis step was considered for externalization (`docs/readforme.md:71`).

## 🚀 Getting Started

*(Placeholder - Add specific setup steps here)*

1.  **Prerequisites:** .NET 9 SDK, Node.js (for client development if needed), Docker (for deployment).
2.  **Configuration:** Set up necessary API keys and connection strings in `.env` file (refer to `.env.example` if provided).
3.  **Backend:**
    ```bash
    cd c:\Users\devma\Desktop\DeepResearch\Apollo
    dotnet run --project Apollo.Api\Apollo.Api.csproj
    ```
4.  **Frontend (if running locally):**
    ```bash
    cd c:\Users\devma\Desktop\DeepResearch\Apollo\Apollo.Client
    npm install
    npm start
    ```

## ☁️ Deployment

*   **Backend (API):** Use the provided `Dockerfile` to build and deploy the API service to platforms like Render.com.
*   **Frontend (Client):** Build the React application (`npm run build`) and deploy the static assets to a hosting service like Vercel or Netlify. Ensure the client's API URL configuration points to the deployed backend URL and the backend's CORS policy allows the client's origin.

---
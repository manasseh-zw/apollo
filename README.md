references : https://jina.ai/news/a-practical-guide-to-implementing-deepsearch-deepresearch/
: visualization : https://github.com/microsoft/data-formulator/tree/main
: RAG : https://arxiv.org/abs/2410.08815 | https://www.youtube.com/watch?v=O7Ce3YljyIY
https://demo.exa.ai/hallucination-detector

challanges: how to have llm passed args and app state arguments in function call, just passed the arg in the system prompt

decision choose between rag or use large context window? best approach hybrid where we have rag for content we get from the web and the report is synthesied via the large context widnow were

decision use semi agentic research process where i have agents that i invoke explicly as part of an overall multi step process, this is some what more deterministic and less room for error due to llm hallucniation,
yes
Alternative 2 fully agentic with plugins for external services... this is of cause has more room for error but embraces the fully agentic approach.. idea: use a shared state between agents instead of each agent publishing all results into the group chat.. which wont be that great for the context

struggled to undersatnd the relationshp between the dotnet semantic kernel memory capabilities and the Kernel Memeory,

understood that the kernel memory plugin makes chunking, tokenizing and ingesting data into the vector store simpler

decided against kernel memory for now went with pg vector connector for SK vector store... might add kernel memory in future

decided to return back to kernel memory its fits my use case much more, even has native web page ingestion which is epic.

so instead of the agents passing data to each other through the chat history we instead have them pass the info around via a shared state and they can mutate this state via a state manager exposed as kernel functions to the plugins

instead of a shared state we could use events to pass around data

decided to go fully agentic, consolidated the the query gen to ingestion agents into one research engine agent

agents not using other plugins realized it might be an advertising issue the agents are not aweare of such functions

for some reason could not use firecrawl api via Rest they have a weird request format.. had to use a dodgy lib but it works for now.

agents now can call functions, got thorugh teh first questin iteration had issues setting the next question as active and continueing

one of the biggest differencitators of my project is that i dont do a table of contents before hand i have a set of questions that need to be answered, the table of contents is genrated on the fly which i think makes it more accurate and relevant to the resaerch inquery vs the model trying to find search queries to appease a table of contenst

now facing issues with firecrawl rate limits and timeouts.. i need a custom scraper using craw4ai

faced some depenency isssue with crawl4ai for some reason in the latest stable version they added a dep that requires c++ build tools a huge depndency idk why they even did that its craazy this would also make setting up docker much trickier reverted to using an old version of crawl4ai

now facing issues with kernel memory the webscarper interface does not allow batch processing..., might have to go with import documents where i have eahc url content as a page...

implemented ingest processor to which handles batch processing of all search reseults for a research query.

AllowMixingVolatileAndPersistentData = true //i set this to true since i am not dealing with documents at the moment;

realized i was not adversiting the method that sets the next active question so the agent was looking ofr pending question when they were never set!

Kind of difficult to debug, agent group chat gets stuck in a loop where it keeps on adding gap questions.

huhhhh now, switched to gpt4.1 it is faster and does communicatio better but it was not able to undersatnd tuple responses so i switched teh getactiveqeustion to simply return the text of the question and that worked well now its calling functions

had to polish my prompts to make every step clealy explained (this took about 3 days to solve btw)

postgers ingest is stucky somwhere idk why that is happening, taking longer than usual ... (was targetting wrong db) it works

now research engine does not want to process the next question after the first one it attempts to automatically call complete

decined to not use my custom scraper and just got with exa since it provides the crawled content of the site too + given the time constraints i was taking on a bit too much altoghtough the webspider was workging its not the most efficient the research takes that much longer with inplace ther eusing exa content which it returns

how to best combine Kernel Memory's retrieval and section-focused synthesis (AskAsync vs SearchAsync) with a powerful large-context model (Gemini 2.5 Pro via Semantic Kernel) for final report generation, specifically facing the challenge of reliably passing and including the original source URLs in the synthesized multi-page output.

Two-Stage Synthesis: Use Kernel Memory's AskAsync to generate synthesized content for each section based on its relevant memories, then pass these pre-synthesized sections to a large-context model (Gemini via SK) for a final synthesis and structuring of the entire report.

Retrieval + Single-Stage Synthesis: Use Kernel Memory's SearchAsync to retrieve the raw, relevant document chunks and their source information for each section, then pass all of these raw chunks and sources to the large-context model (Gemini via SK) for a single, comprehensive synthesis of the entire report. (More robust for preserving and including source URLs).

decided to go with 2 stage synthesis, also kernel memory has nice filter by researchId

making the function choice behaviour to required makes a huge difference they must call functions now

decided to eliminate the synthesizer agent and have that be a seperate service such that i have more control over the process

did a task.await all for searching memory inthe final syntehsis and cut the vector seach time by half for each section whic i thought ...

followed prompt engineering guide from https://cookbook.openai.com/examples/gpt4-1_prompting_guide
tried to do the synthesis with google gemini but im facing ratelimis which is fvery wiered... then ust used gpt4.1 its also very good and has similar context window

decided to use reflective rag for analysis phase, where the memory context does an internal critic of teh information it has in and of itself, looking internally to find out what signficant knowlege gaps it has, pertaining to the context of the research topic, and description... , this make the resaerch analysis phase much faster because at the moment it was taking ages because we ware asking abritrary questions derived from the title and descriptions and the analyzer agent would do about 5+ queires at times it even times out becuase the agent chat completion is not configured to wait over 100sec for a response ..., we can also ask kernel memory for a table of contents draft given the information it has and the research topic and description at hand..

found a way to do gap analysis but getting rate limited, by open ai! need to bump that up!

okay lets face it the biggest bottle neck right now is the vector db postgres is too slow the ask memroyasync, switched to neon db now its much faster,

decided to limit gap questions added to 1 so that we dont react the rate limit.

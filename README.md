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

Starts with a research planning session where the user initiates the conversation with the research planner assistant, where the assistant aims to understand the users research query.
This conversation yields a research plan, which contains details about the research and more importantly the core research questions that when answered they fully address the users research query.

    - Now i have a saveResearchPlugin which the planner can trigger when this information has been attained and this plugin  creates the research with the plan, an event here is triggered to notify the client that the research has been saved and we pass the researchId  to the client, which it uses to redirect the user to the actual on going research chat page...

Now once the research plan is saved we immedialy start the research proccess.

Now state. - So for starting the research plan we need the researchId which we get from the client, we use this resaerch id to retrieve the research from our db, we create our research state, in our in memory cache where the key is this research plan. - Now the state has the following props: ResearchId, title, description, and pending research questions(i.e research questions from the db that have not been processed yet), completed research questions, Active question Id, Crawled Urls, Table of contents ( we might also need an indicator for wether or not the research question are all addressed,). A couple of state mutation methods which are to be exposed in plugins called by agents. - the researchQuestion object used in the state has the props, Id, Text, SearchQueries, searchResults, rankedSearchResults - The state is accessible globally, agents can write to this state via plugins which call the state mutation methods.

AgentGroupChat

(the conversations within the agent group chat are to be streamed to a client via signalR to showcase the agent interactions, hence why i chose to have the agents pass information via a shared state instead of chat history, to make it user readable and to prevent exceeding context windows)

(note that the ui for the client has 2 panels one side showing the website being currently crawled, the other showing the agent groupchat )

    - Now we have a couple of agents namely:
        * ResearchCoordinator
        * QueryGenerator
        * WebSearcher
        * Reranker
        * Crawler
        * ResearchAnalyzer
        * ReportSynthesizer

    - termination and selection functions( will implement the selectionand terminatio functions such the research flows as intended, the research coordinator has the termination and selection capabilities, the selection agent understadns the research flow and can intelligently determine who is next)

    Research Flow
    - After initiating the research state and populating the pending research questions queue,
    - We pass the research topic, description and research question to our research coordinator, and we intiate the research chat, the coordinator lets the group chat know of what the agent are going to work on and what the topic is about.. then within that same opening message the coordinator specifies the first research question the agents are to go into here the coordinator mutates the research state by setting the active researchId to that of the first question it just specified , and mentions the name of the QueryGenerator agent, (via our selectoin function definition, the query gen agent understands it is now its turn)
    - The query generator agent can infer the research question from the conversation flow, or just to be explicit call the get current research question method via the state plugin to get the active research question and use this to generate the queries. After it generates the search queries this agent updates the state by updating the active question queries prop.
    - Now the query gen agent lets the group chat know that is has managed to comeup with some queries to fully address the research question (and this is in conversational manner, the agents will be interacting just like humans, and passing data behind the scenes via state plugins)
    - The coordinator selection agent then picks it up from the query gen and hands it over to the websearcher agent
    - The web searcher agent gets the current research question queries, which were just updated by the query gen agent and calls the websearch plugin
        - Within the websearch plugin we go through each query and perform web searces and gather the search results and update the search results ( here we check if any of the results we got have already been crawled? if not we add that to the searchResult in the state. ) of the active research question
        now the web searcher updates the gropup chat sayint it go some interesting results etc and its done with the queries
    - The coodrinator selects the reranker agent to be up next,
    - the reranker agent then looks at hte search results and ranks them accodring to the relevance score crediblity and other metrics, an potnetially removes some and updates the rankedSearchResults prop, the  re ranker also
    - The reranker lets the groupchat know it reranked the queires and a brief of what it thought about the search results etc
    - coodrinator chimes in and seelects the crawler agent to be upnext
    - The Crawler agent gets teh ranked serach result for the active research question and calls the cralwer plugin passing the ranked searchResult for ingestion and crawling.
        - Within the crawler plugin we are using kernel memory with a custom webcrawler implementation and ingesting the cralwed sites via firecrawl service into our memory store (here we update the state crawled urls)... for each crawl we publish the search result being crawled via our in memory event message bus (using .net channels) this allows us to show updates on teh client of the website we are currely crawling and some highlihts and summary we got from there.
    -Now the crawler makes the group chat know that its done and has crawled and ingested all the search results that i got hence this question is done
    - the Coordinator slection agent then picks this up and lets the group chat know that we are moving on to the next question... and we repeat the same cyle for all questions untill we finish.

    - Once we are done with all the questions the coordinator checks the state to see if ther eare any mroe research questions if not then all questions are processed..
    - Now this is where the researchAnalyser agent comes into play the coodinator asks the rsearch analyser to see if there are any knowledge gaps within all the information we have gathered and whether or not it addressed the users research topic query.
        - the research analyzer uses the kernel memory plugin which exposes has access the memory store search ability and our analyzer performas varous search queries trying to see if we have all the data and whter or not we can make the final report.
        - if it is satisfied the research analyzer then tells the coodinator that we're good,
            - in which case the coodinator calls the Report synthesizer agent which has direct access to the kernel memory and compiles the final comprehensive reprot with all the information within memory and adds citations to the sources of the data etc! and resaerch is complete!
        - if the resaerch analyzer is not satisfied it updates the state and adds new research questions to address this knowledge gap it identified, the coordinator then picks up these questions as it did all the onthers ones and we process them until we have filled in all the gaps and the resaerch is complete

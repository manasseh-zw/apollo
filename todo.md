    - create a standard client update types... research feed update(can be search results, search progress message, current query being executed, question being currely worked on , phase... e.g. now analysiing data ...  table of content update, now syntehsiszing.. ) , and agent message update(agent name, message content)...

    these are to be insync realtime signalR with the client so in the ui we have a quesion timeline stepper where we show questions to be processed and questions the current question being processed.....

    we have the research feed main pabel where we get the researfch feed update type events so essentially on the client we have a thread and we append each new resach feed update item to it and render it accordinly .. so we have differnt types depending on the type of update it can be a  simple progress message, web search execution message i.e.: "searching the web for "query"..., search results(search result componet on the client , render the favicon.. snippet highlihts etc summary ), table of contents ... updadate we might need a tbale of contents component there... so these are resaerch feed updates,

    then we have the agent chat updates for the agent groupchat we hae where we show the inter agent comms coing on we send the message content and the agement message they sent.. in teh chat history...

    so first we need to establish common types regardng the actual resaerch entity the research report...
    then we create the common update event types.. the question timeline update, the resaercfh feed update (many sub types, each with a correspodning ui component to render it), then the agent group chat udpatse.. 

    so on the backend we need to fix our client updates sucht that they are dispatched at the correct stage and we send the correct types for each stage such that it flows nicely in teh client ui.., so we have to idenfiy update areas..... 

    we will deal with state persistance accross sessions later now we just want towier sync and wiere updtes form teh backend to the client in teh simplest possible way then we can move into a a update message queue and server side update cache .. later 

    mainly using client update callback but we can implment a better pattern if you have a better one in mind , fix all do make suggestions make a plan first and consult with me before we chagen any code.. ohh ofcuase we need to have a clean implementaito no featching the research from the id param in the client to on resech/id to know what to render then we pass that to the agent and feed so they join the repesctive udpates groupchats... 

    - wireup the agents updates with the client research/id page to show realtime data of the on going research progress and agent respones etc
    - on the frontend we need to display research history on the sidebar where we show on goining an finished past research sessions, indicator for currely on going reseach, make it work for collapsed and expanded states
    - work on the ui for the resaerch complete state where we have the final report being shown.. /potentially chat with resaerch memories integrate exa answer aswell to enhance it

    - find some way to preserve research session state so that we retain FE state upon refresh or a new browser window ro different devicde for that matter..(maybe have a cache here)
    - maybe a client updates cache on the api side... such that when we fetch the research we also ge the resaech feed state if the research state is inprogress .. so on the resaerdh/id ui for an inprogress resaerch we immedialy show previous updates then start streaming new ones after we join the signal r updates group for the resarch id...


    - Major Nice to have feature
        - after research is complete and we have the report... on the report ui we have an eexport feature wehre you can download the resaerch as a pdf, word or powerpoint presentation use ironPPT and pandoc.net here
        - potientially you can connect your microsoft accoutn to allow automatic export to ms 365 online, same with google (nice to have),


    - enhancement features
         - integrate images into our final report... use images provided by exa search to have a 'resarch gallery' where for each query where for each search result where there is an image provided we have a chat completion service where we upload these images and their corresponding urls then ai gives descripbes the image and we keep a registry of image urls with theier associated descriptions we can use these in our researsch report syntehis process to have the final report mardkwon genrated include those images strategically use mardown html markup syntax to dsiplay text content and images aside or tables too..
         - this research image gallery may also be useful when creating the ppt, we also have ai imagen availble ..

immediate todo - finish up the history chat modal - on hide history on expanded sidebar recent chats, on collapse show history button hide recent chats, history button triggers modal, with resaerch history - add status ping dot button if the research is active and is still ongoing little blue ping dot aanimated like a sonner radar animation at the end of that research chat itme in the sidenav,
now for new resaerch that has been completed but not viewed yet by the user we set the resarch status to green that is complete but not opened yet.. other wise there is not ping dot animated thing...

    - move the research sidenav item to the app-sidebar level where it looks the same but is a button with full radius and fully white bg to contrast the other sidenav items , maybe do the same
    with the history button it should also be at the app-sidbar level but without the white bg contrast

    -

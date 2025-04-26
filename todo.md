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

immediate todo - finish up the history chat modal - on hide history on expanded sidebar recent chats, on collapse show history button hide recent chats, history button triggers modal, with resaerch history - add status ping dot button if the research is active little blue ping dot aanimated like a sonner radar animation at the end of that research chat itme in the sidenav,
now for new resaerch that has been completed but not viewed yet by the user we set the resarch status to green that is complete but not opened yet.. other wise there is not ping dot animated thing...

    - move the research sidenav item to the app sidebar level where it looks the same but is a button with full radius and fully white bg to contrast

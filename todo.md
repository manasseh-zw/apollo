todo: - make research flow operational from end to end
-make sure exa service integration is working with Kernel memory vector ingestion - ensure agents are able to autonomously go through each research question until analysis stage [x] done!

-might have to rethink how analysis and synthesis is being done, we could possible have a meta agent implementation , where the research analyzer and report synthesizers are now meta agents who trigger other agents via plugins which will so the analysis and final report synthesis (this allows us to use a specialized model for parts of the research with different requirements.. in this case we would use a reasoning model to synthesize the final report) for the analysis phase we could simply do a memory.ask against our data and the text gen model we have used to build the kernel memory will do the semantic serching to deitermine if there are any research gaps...

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

from http.client import HTTPException
import modal
from typing import Optional, List
from pydantic import BaseModel, HttpUrl
from crawl4ai import (
    AsyncWebCrawler,
    CrawlResult,
    CrawlerRunConfig,
    DefaultMarkdownGenerator,
    PruningContentFilter,
    CacheMode,
)


# Create a Modal image with UV and Playwright
def setup_image():
    return (
        modal.Image.debian_slim(python_version="3.13")
        .apt_install(
            "software-properties-common",
            "wget",
            "curl",
        )
        .pip_install_from_requirements("requirements.txt")
        .run_commands(
            "apt-get update",
            "apt-get install -y software-properties-common",
            "apt-add-repository non-free",
            "apt-add-repository contrib",
            "playwright install-deps chromium",
            "playwright install chromium",
            "playwright install",
        )
    )


# Create the image
crawler = setup_image()

app = modal.App("crawler")


class SingleScrapeRequest(BaseModel):
    url: HttpUrl


class BatchScrapeRequest(BaseModel):
    urls: List[HttpUrl]


class ScrapeResponse(BaseModel):
    success: bool
    content: Optional[str] = None
    error: Optional[str] = None


class BatchScrapeResponse(BaseModel):
    success: bool
    results: List[dict] = []
    error: Optional[str] = None


async def scrape(url: HttpUrl) -> ScrapeResponse:
    try:
        async with AsyncWebCrawler() as crawler:
            result: CrawlResult = await crawler.arun(
                url=str(url),
                config=CrawlerRunConfig(
                    exclude_external_links=True,
                    markdown_generator=DefaultMarkdownGenerator(
                        content_filter=PruningContentFilter()
                    ),
                    exclude_social_media_links=True,
                    cache_mode=CacheMode.BYPASS,
                ),
            )
            if result.success:
                return ScrapeResponse(
                    success=True,
                    content=result.markdown_v2.fit_markdown,
                )
            else:
                return ScrapeResponse(success=False, error="Failed to scrape content")
    except Exception as e:
        return ScrapeResponse(success=False, error=f"Scraping failed: {str(e)}")


async def scrape_batch(urls: List[HttpUrl]) -> BatchScrapeResponse:
    try:
        results = []
        async with AsyncWebCrawler() as crawler:
            # Use arun_many for parallel crawling
            crawl_results = await crawler.arun_many(
                urls=[str(url) for url in urls],
                config=CrawlerRunConfig(
                    exclude_external_links=True,
                    markdown_generator=DefaultMarkdownGenerator(
                        content_filter=PruningContentFilter()
                    ),
                    exclude_social_media_links=True,
                    cache_mode=CacheMode.BYPASS,
                    stream=False,
                ),
            )

            # Process results
            for result in crawl_results:
                if result.success:
                    results.append(
                        {
                            "url": result.url,
                            "content": result.markdown_v2.fit_markdown,
                            "success": True,
                        }
                    )
                else:
                    results.append(
                        {
                            "url": result.url,
                            "success": False,
                            "error": result.error_message or "Failed to scrape content",
                        }
                    )

        return BatchScrapeResponse(success=True, results=results)

    except Exception as e:
        return BatchScrapeResponse(
            success=False, error=f"Batch scraping failed: {str(e)}"
        )


@app.function(image=crawler)
@modal.fastapi_endpoint(method="POST", docs=True, label="scrape")
async def scrape_endpoint(request: SingleScrapeRequest) -> ScrapeResponse:
    return await scrape(request.url)


@app.function(image=crawler)
@modal.fastapi_endpoint(method="POST", docs=True, label="scrape-batch")
async def scrape_batch_endpoint(request: BatchScrapeRequest) -> BatchScrapeResponse:
    if not request.urls:
        raise HTTPException(status_code=400, detail="No URLs provided")
    return await scrape_batch(request.urls)


@app.function(image=crawler)
@modal.fastapi_endpoint(method="GET")
async def health():
    return {"status": "ok", "service": "Apollo WebSpider"}


# @app.get("/")
# async def root():
#     return {"status": "ok", "service": "Apollo WebSpider"}

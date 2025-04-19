# Apollo WebSpider 🕷️

A high-performance web scraping microservice built with crawl4ai and FastAPI, designed for Apollo ~ Deep Research Agent. This service provides efficient web content extraction with support for both single-URL and batch operations.

## Features

- **Single URL Scraping**: Extract content from individual URLs (~4-5s response time, ~10s on cold start)
- **Batch Scraping**: Efficiently process multiple URLs in parallel (~1s per URL in batches of 10-15 URLs)
- **Markdown Output**: Clean, structured content output in markdown format
- **Modal.com Deployment Ready**: Configured for seamless deployment to Modal

## Technical Stack

- **crawl4ai v0.4.248**: Chosen for stability and minimal dependencies
- **FastAPI**: Modern, fast web framework
- **Playwright**: Handles browser automation
- **UV Package Manager**: Fast, reliable Python package management

## Local Development

1. Install dependencies:

```bash
uv venv
uv pip install -r requirements.txt
playwright install
```

2. Run the local server:

```bash
uvicorn main:app --port 8001
```

## Deployment

Deploy to Modal.com:

```bash
modal deploy main_deploy.py
```

## API Endpoints

### Single URL Scrape

- **POST** `/scrape`
- Request body: `{ "url": "https://example.com" }`

### Batch URL Scrape (Recommended)

- **POST** `/scrape/batch`
- Request body: `{ "urls": ["https://example.com", "https://example.org"] }`

## Performance Notes

- Single URL scraping: ~4-5s (warm), ~10s (cold start)
- Batch scraping: ~10s for 10-15 URLs (~1s per URL)
- Batch processing is recommended for better performance

## Technical Notes

- Using crawl4ai v0.4.248 specifically to avoid Visual C++ build tools dependency
- Configured for both local development (`main.py`) and Modal deployment (`main_deploy.py`)
- Docker configuration available for containerized deployment

## Health Check

- **GET** `/` - Returns service status

FROM mcr.microsoft.com/playwright/python:v1.51.0-noble as playwright

# Install uv
COPY --from=ghcr.io/astral-sh/uv:latest /uv /uvx /bin/

# Copy the application into the container
COPY . /app

# Install the application dependencies
WORKDIR /app
RUN uv venv && uv sync --frozen --no-cache

# Run the application
CMD ["/app/.venv/bin/uvicorn", "main:app", "--port", "8001", "--host", "0.0.0.0"]

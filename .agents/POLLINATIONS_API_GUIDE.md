# Pollinations API Integration Guide

A comprehensive guide to accessing all Pollinations APIs for text, image, audio, and video generation with no API keys required.

## Overview

Pollinations.AI is a free, open-source platform providing multiple generative AI APIs. It requires **no API keys or authentication**—all services are accessible via simple HTTP requests with optional parameters in the URL.

---

## Core Endpoints

### Base URLs
- **Images**: `https://image.pollinations.ai`
- **Text**: `https://text.pollinations.ai`
- **Audio (TTS)**: `https://text.pollinations.ai`
- **Web**: `https://websim.pollinations.ai`

---

## 1. Image Generation API

### Endpoint
```
https://image.pollinations.ai/prompt/{prompt}
```

### Parameters (URL Query String)

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `prompt` | string | required | URL-encoded text description of the image to generate |
| `model` | string | `flux` | Model to use: `flux`, `flux-pro`, `flux-realism`, `gptimage`, `stable-diffusion`, etc. |
| `width` | integer | `1024` | Image width in pixels (e.g., 512, 768, 1024, 1280) |
| `height` | integer | `1024` | Image height in pixels (e.g., 512, 768, 1024, 1280) |
| `seed` | integer | random | Random seed for reproducibility (0-2147483647) |
| `transparent` | boolean | `false` | Set to `true` for transparent background (gptimage model only) |
| `nologo` | boolean | `false` | Set to `true` to remove Pollinations logo |
| `enhance` | boolean | `false` | Set to `true` to enhance the prompt using AI |

### Basic Usage (cURL)
```bash
# Simple image generation
curl -o sunset.jpg "https://image.pollinations.ai/prompt/A%20beautiful%20sunset%20over%20the%20ocean"

# With parameters
curl -o portrait.jpg "https://image.pollinations.ai/prompt/A%20portrait%20of%20a%20woman?width=768&height=768&model=flux-realism"

# Transparent background (gptimage model)
curl -o logo.png "https://image.pollinations.ai/prompt/A%20company%20logo?model=gptimage&transparent=true"
```

### Python Usage
```python
import requests
import urllib.parse

def generate_image(prompt, model="flux", width=1024, height=1024, seed=None, nologo=False):
    """Generate an image using Pollinations API."""
    encoded_prompt = urllib.parse.quote(prompt)
    url = f"https://image.pollinations.ai/prompt/{encoded_prompt}"
    
    params = {
        "model": model,
        "width": width,
        "height": height,
        "nologo": str(nologo).lower()
    }
    
    if seed is not None:
        params["seed"] = seed
    
    try:
        response = requests.get(url, params=params)
        response.raise_for_status()
        
        if 'image' in response.headers.get('Content-Type', ''):
            return response.content
        else:
            print(f"Error: {response.text}")
            return None
            
    except requests.exceptions.RequestException as e:
        print(f"Error generating image: {e}")
        return None

# Usage
image_data = generate_image("A futuristic city at night", model="flux", width=1280, height=720)
if image_data:
    with open("city.jpg", "wb") as f:
        f.write(image_data)
```

### C# / .NET Usage
```csharp
using System;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;

public class PollinationsImageClient
{
    private static readonly HttpClient Client = new HttpClient();
    private const string BaseUrl = "https://image.pollinations.ai/prompt";

    public static async Task<byte[]> GenerateImageAsync(
        string prompt, 
        string model = "flux", 
        int width = 1024, 
        int height = 1024, 
        int? seed = null)
    {
        try
        {
            var encodedPrompt = HttpUtility.UrlEncode(prompt);
            var url = $"{BaseUrl}/{encodedPrompt}";
            
            var uriBuilder = new UriBuilder(url)
            {
                Query = $"model={model}&width={width}&height={height}&nologo=false"
            };
            
            if (seed.HasValue)
            {
                uriBuilder.Query += $"&seed={seed}";
            }

            var response = await Client.GetAsync(uriBuilder.Uri);
            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentType?.MediaType.Contains("image") ?? false)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                GD.PrintErr($"Unexpected response: {await response.Content.ReadAsStringAsync()}");
                return null;
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error generating image: {ex.Message}");
            return null;
        }
    }
}
```

---

## 2. Text Generation API

### Endpoint
```
https://text.pollinations.ai/openai
```

### Models Available
- `openai` - OpenAI-compatible models
- `mistral` - Mistral AI models
- `qwen` - Alibaba Qwen models
- `claude` - Anthropic Claude models
- Custom models based on availability

### Request Format (POST)
```json
{
  "model": "openai",
  "messages": [
    {"role": "system", "content": "You are a helpful assistant."},
    {"role": "user", "content": "Your question here"}
  ],
  "seed": 42,
  "stream": false,
  "private": false,
  "referrer": "MyApp"
}
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `model` | string | required | Model to use (openai, mistral, qwen, claude, etc.) |
| `messages` | array | required | Chat message history with role and content |
| `seed` | integer | random | Random seed for reproducibility |
| `stream` | boolean | `false` | Stream response using Server-Sent Events |
| `private` | boolean | `false` | Mark request as private |
| `referrer` | string | optional | App identifier for analytics |

### Basic Usage (cURL)
```bash
curl https://text.pollinations.ai/openai \
  -H "Content-Type: application/json" \
  -d '{
    "model": "openai",
    "messages": [
      {"role": "system", "content": "You are a helpful historian."},
      {"role": "user", "content": "When did the French Revolution start?"}
    ],
    "seed": 42
  }'
```

### Python Usage
```python
import requests
import json

def generate_text(prompt, model="openai", system_message=None, stream=False):
    """Generate text using Pollinations text API."""
    url = "https://text.pollinations.ai/openai"
    
    messages = []
    if system_message:
        messages.append({"role": "system", "content": system_message})
    messages.append({"role": "user", "content": prompt})
    
    payload = {
        "model": model,
        "messages": messages,
        "stream": stream
    }
    
    headers = {"Content-Type": "application/json"}
    
    try:
        response = requests.post(url, headers=headers, json=payload)
        response.raise_for_status()
        
        result = response.json()
        return result['choices'][0]['message']['content']
        
    except requests.exceptions.RequestException as e:
        print(f"Error generating text: {e}")
        return None

# Usage
response = generate_text(
    "Write a short poem about the moon",
    model="openai",
    system_message="You are a creative poet."
)
print(response)
```

### Streaming Responses (Python)
```python
import requests
import json
import sseclient  # pip install sseclient-py

def generate_text_streaming(prompt, model="openai"):
    """Generate text with streaming response."""
    url = "https://text.pollinations.ai/openai"
    
    payload = {
        "model": model,
        "messages": [{"role": "user", "content": prompt}],
        "stream": True
    }
    
    headers = {
        "Content-Type": "application/json",
        "Accept": "text/event-stream"
    }
    
    try:
        response = requests.post(url, headers=headers, json=payload, stream=True)
        response.raise_for_status()
        
        client = sseclient.SSEClient(response)
        full_response = ""
        
        for event in client.events():
            if event.data and event.data.strip() != '[DONE]':
                try:
                    chunk = json.loads(event.data)
                    content = chunk.get('choices', [{}])[0].get('delta', {}).get('content')
                    if content:
                        print(content, end='', flush=True)
                        full_response += content
                except json.JSONDecodeError:
                    pass
        
        print()
        return full_response
        
    except requests.exceptions.RequestException as e:
        print(f"Error in streaming: {e}")
        return None
```

---

## 3. Text-to-Speech (TTS) API

### Endpoint
```
https://text.pollinations.ai/{text}
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `model` | string | `openai-audio` | TTS model (openai-audio, etc.) |
| `voice` | string | `nova` | Voice to use: alloy, echo, fable, onyx, nova, shimmer |

### Available Voices
- `alloy` - Neutral, balanced tone
- `echo` - Deep, resonant tone
- `fable` - Warm, storytelling tone
- `onyx` - Rich, authoritative tone
- `nova` - Bright, energetic tone
- `shimmer` - Light, clear tone

### Basic Usage (cURL)
```bash
# Basic TTS GET request
curl -o hello_audio.mp3 "https://text.pollinations.ai/Hello%20world?model=openai-audio&voice=nova"

# Different voice
curl -o welcome_audio.mp3 "https://text.pollinations.ai/Welcome%20to%20Pollinations?model=openai-audio&voice=fable"
```

### Python Usage
```python
import requests
import urllib.parse

def generate_speech(text, voice="nova", output_filename=None):
    """Generate speech audio from text."""
    encoded_text = urllib.parse.quote(text)
    url = f"https://text.pollinations.ai/{encoded_text}"
    
    params = {
        "model": "openai-audio",
        "voice": voice
    }
    
    try:
        response = requests.get(url, params=params)
        response.raise_for_status()
        
        if 'audio/mpeg' in response.headers.get('Content-Type', ''):
            if output_filename:
                with open(output_filename, 'wb') as f:
                    f.write(response.content)
                print(f"Audio saved to {output_filename}")
            return response.content
        else:
            print(f"Error: {response.text}")
            return None
            
    except requests.exceptions.RequestException as e:
        print(f"Error generating speech: {e}")
        return None

# Usage
generate_speech("The quick brown fox jumps over the lazy dog", 
                voice="echo", 
                output_filename="narration.mp3")
```

---

## 4. WebSim API (Website Generation)

### Endpoint
```
http://localhost:16386/{prompt}
```

### Local Development
```bash
# Make a GET request locally
curl "http://localhost:16386/Create%20a%20simple%20calculator%20with%20HTML,%20CSS,%20and%20JavaScript"
```

### Parameters
- `prompt` (URL path) - Description of the website to generate

---

## 5. Real-time Feeds

### Image Generation Feed (SSE)
```bash
# Stream of all images being generated
curl -N https://image.pollinations.ai/feed | while read line; do 
  echo "Received: $line"
done
```

This provides real-time updates of images being generated on the platform.

---

## 6. Model Context Protocol (MCP) Server

Pollinations provides an MCP server for integration with Claude and other AI assistants.

### Installation
```bash
# Install globally
npm install -g @pollinations/model-context-protocol

# Or run directly with npx
npx @pollinations/model-context-protocol
```

### Installation in Claude Desktop
```bash
# Run installation script
npx @pollinations/model-context-protocol install-claude-mcp
```

### MCP Configuration (Claude Desktop)
Add to `~/.config/Claude/claude_desktop_config.json`:
```json
{
  "mcpServers": {
    "pollinations": {
      "command": "npx",
      "args": ["@pollinations/model-context-protocol"]
    }
  }
}
```

---

## 7. Authentication & Rate Limiting

### No API Keys Required ✅
All Pollinations APIs are free and require **no authentication**. Simply make HTTP requests to the endpoints.

### Rate Limiting
- **General**: Reasonable rate limits per IP address
- **Special Program**: Higher limits available via the Special Bee program
- **Referrer**: Include `referrer` parameter in requests for analytics and potential benefits

### Setting Referrer (for analytics)
```python
# Include referrer in requests
payload = {
    "model": "openai",
    "messages": [...],
    "referrer": "MyGameName"
}
```

---

## 8. Integration Examples

### Multi-Modal Story Generation
```python
import requests
import urllib.parse
import time

def create_story_with_visuals(story_prompt):
    """Generate a story with accompanying images and narration."""
    
    # 1. Generate story text
    text_response = requests.post(
        "https://text.pollinations.ai/openai",
        json={
            "model": "openai",
            "messages": [
                {"role": "system", "content": "You are a creative storyteller."},
                {"role": "user", "content": f"Write a short, vivid story about: {story_prompt}"}
            ]
        }
    )
    story = text_response.json()['choices'][0]['message']['content']
    
    # 2. Generate illustrative image
    image_prompt = f"Illustration for story: {story_prompt}"
    encoded_prompt = urllib.parse.quote(image_prompt)
    image_response = requests.get(
        f"https://image.pollinations.ai/prompt/{encoded_prompt}?model=flux&width=1024&height=768"
    )
    
    # 3. Generate narration
    encoded_story = urllib.parse.quote(story[:100])  # First 100 chars
    audio_response = requests.get(
        f"https://text.pollinations.ai/{encoded_story}?model=openai-audio&voice=nova"
    )
    
    return {
        "story": story,
        "image": image_response.content,
        "audio": audio_response.content
    }
```

### Game Agent with Visual Validation
```csharp
// C# example for game agent visual confirmation
public class GameAgentVisualValidator
{
    public async Task<string> ValidateGameScreenshot(string screenshotPath, string query)
    {
        // 1. Read screenshot
        var imageData = File.ReadAllBytes(screenshotPath);
        
        // 2. Generate description using Pollinations
        var textResponse = await PollinationsTextClient.GenerateTextAsync(
            $"Analyze this game screenshot and answer: {query}",
            model: "openai"
        );
        
        return textResponse;
    }
    
    public async Task<byte[]> GenerateTestScreenshot(string scenarioDescription)
    {
        // Generate reference screenshot for UI test
        return await PollinationsImageClient.GenerateImageAsync(
            $"Game screenshot of: {scenarioDescription}",
            model: "flux",
            width: 1920,
            height: 1080
        );
    }
}
```

---

## 9. Best Practices

### 1. Prompt Engineering
- **Be specific**: Include style, mood, lighting details
- **Use positive language**: Say what you want, not what you don't
- **Reference styles**: "in the style of...", "inspired by..."
- **Seed for consistency**: Use same seed to regenerate with variations

### 2. Performance Optimization
- **Cache results**: Store generated images/text with consistent seeds
- **Use appropriate models**: `flux` for quality, `flux-realism` for photorealism, `gptimage` for logos
- **Batch requests**: Generate multiple items efficiently
- **Stream long responses**: Use streaming for text generation UI feedback

### 3. Error Handling
```python
import requests
from requests.adapters import HTTPAdapter
from urllib3.util.retry import Retry

def create_resilient_session():
    """Create HTTP session with retry logic."""
    session = requests.Session()
    retry = Retry(
        total=3,
        backoff_factor=0.5,
        status_forcelist=(500, 502, 503, 504)
    )
    adapter = HTTPAdapter(max_retries=retry)
    session.mount('http://', adapter)
    session.mount('https://', adapter)
    return session
```

### 4. Resource Management
- **Set reasonable timeouts**: 30-60 seconds for image generation
- **Monitor image size**: Use width/height appropriate for use case
- **Clean up files**: Remove cached images periodically
- **Rate responsibly**: Don't overwhelm the service with burst requests

---

## 10. Troubleshooting

### Common Issues

**Issue**: "No API key provided"
- **Solution**: Pollinations doesn't require API keys. Use direct HTTP calls.

**Issue**: Slow image generation
- **Solution**: Use `nologo=true` and `enhance=false` for faster generation
- Try simpler models like `stable-diffusion` instead of `flux-pro`

**Issue**: Connection timeout
- **Solution**: Increase timeout to 60+ seconds, image generation can take time

**Issue**: Transparent background not working
- **Solution**: Only `gptimage` model supports `transparent=true` parameter

**Issue**: CORS errors in browser
- **Solution**: Use backend/proxy server for image generation, not directly from frontend

---

## 11. Resource Links

- **Official Site**: https://pollinations.ai
- **API Docs**: https://github.com/pollinations/pollinations/blob/master/APIDOCS.md
- **GitHub**: https://github.com/pollinations/pollinations
- **Models**: Available models listed on main site
- **MCP Server**: https://github.com/pollinations/pollinations/tree/master/model-context-protocol

---

## Summary

**Pollinations provides:**
- ✅ **No Authentication Required** - Free APIs for all
- ✅ **Multiple Modalities** - Text, Image, Audio, Video generation
- ✅ **Multiple Models** - OpenAI, Mistral, Qwen, Claude, Flux, SDXL, and more
- ✅ **Simple Integration** - HTTP GET/POST requests, no complex setup
- ✅ **Streaming Support** - Real-time responses for long-running operations
- ✅ **MCP Support** - Integration with Claude and other AI assistants

Start with a simple request and build from there. The API is forgiving and intuitive!


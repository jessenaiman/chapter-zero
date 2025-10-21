# Qwen Agent Alternatives: From Simple Git to Full GitHub MCP Integration

Here's your complete guide to Qwen agents, from basic git operations to advanced GitHub API integration:

## 🤖 **Option 1: Simple Git Checkin Agent** (Local Git Operations)

**Best for:** Local git automation, committing, pushing changes

**Features:**
- ✅ Check git status
- ✅ Add files to staging
- ✅ Create intelligent commits
- ✅ Push to remote repositories

**Setup:**
```bash
# Install dependencies
pip install -r requirements.txt

# Run with Ollama (free)
ollama pull qwen2.5:7b-instruct
ollama serve

# Use the agent
python git_checkin_agent.py
```

**Example Usage:**
```
Your request: check status
Agent: Shows staged/unstaged/untracked files

Your request: add all files
Agent: Successfully added files to staging

Your request: commit with message "feat: add new feature"
Agent: Successfully created commit

Your request: push changes
Agent: Successfully pushed to remote
```

---

## 🚀 **Option 2: GitHub MCP Agent** (Full GitHub API Access)

**Best for:** Repository management, issue tracking, GitHub automation

**Features:**
- ✅ Real-time GitHub repository data
- ✅ List and search issues/PRs
- ✅ Create issues with labels
- ✅ Repository statistics
- ✅ User and organization info

**Setup:**

### Step 1: Install Prerequisites
```bash
# Node.js and npm (for MCP servers)
curl -fsSL https://deb.nodesource.com/setup_lts.x | sudo -E bash -
sudo apt-get install -y nodejs

# GitHub MCP server
npm install -g @modelcontextprotocol/server-github
```

### Step 2: Create GitHub Token
1. Go to: https://github.com/settings/tokens
2. Generate new token with `repo` permissions
3. Set environment variable:
```bash
export GITHUB_TOKEN="ghp_your_token_here"
```

### Step 3: Run the Agent
```python
from mcp_github_setup import create_real_github_mcp_agent

agent = create_real_github_mcp_agent()
```

**Example Usage:**
```
Your request: Get info about microsoft/dotnet
Agent: Repository has 15.2k stars, 3.1k forks, 89 open issues...

Your request: List open issues in facebook/react
Agent: Found 234 open issues. Top issues: #1234 Memory leak, #5678 Performance...

Your request: Create issue in my repo about a bug
Agent: Created issue #45: "Fix authentication bug" with labels [bug, urgent]
```

---

## 🛠 **Option 3: Custom MCP Integration** (Advanced)

For even more control, you can create custom MCP servers:

```python
# Custom GitHub MCP server
tools = [{
    "mcpServers": {
        "github": {
            "command": "node",
            "args": ["/path/to/custom-github-mcp-server.js"],
            "env": {
                "GITHUB_TOKEN": "your_token"
            }
        }
    }
}]

agent = Assistant(llm=llm_config, function_list=tools)
```

---

## 📊 **Comparison Table**

| Feature | Simple Git Agent | GitHub MCP Agent | Custom MCP |
|---------|------------------|------------------|------------|
| **Git Status** | ✅ | ✅ | ✅ |
| **Local Commits** | ✅ | ✅ | ✅ |
| **GitHub Issues** | ❌ | ✅ | ✅ |
| **Repository Stats** | ❌ | ✅ | ✅ |
| **Real-time Data** | ❌ | ✅ | ✅ |
| **Custom GitHub APIs** | ❌ | ✅ | ✅ |
| **Setup Complexity** | 🟢 Easy | 🟡 Medium | 🔴 Hard |
| **Dependencies** | Qwen-Agent | Qwen-Agent + MCP | Qwen-Agent + Custom Server |

---

## 🎯 **Which Option Should You Choose?**

### Choose **Simple Git Agent** if you want:
- Quick git automation
- No external API keys needed
- Simple local repository management

### Choose **GitHub MCP Agent** if you want:
- Full GitHub integration
- Issue tracking and management
- Repository analytics and insights
- Real-time GitHub data

### Choose **Custom MCP** if you want:
- Specific GitHub workflows
- Integration with other services
- Custom business logic
- Enterprise GitHub integrations

---

## 🔧 **Quick Start Guide**

### For Simple Git Operations:
```bash
cd /path/to/your/repo
python git_checkin_agent.py
```

### For GitHub Integration:
```bash
# 1. Set up GitHub token
export GITHUB_TOKEN="ghp_your_token"

# 2. Run setup script
python mcp_github_setup.py

# 3. Use the agent
python github_mcp_agent.py
```

---

## 💡 **Key Takeaways**

1. **MCP (Model Context Protocol)** enables secure, standardized access to external services
2. **Qwen-Agent** provides the framework for building intelligent agents
3. **Local models** (Ollama) are free alternatives to cloud APIs
4. **GitHub MCP servers** provide comprehensive GitHub API access
5. **Custom tools** can be built for any specific workflow

The beauty of Qwen-Agent is its flexibility - you can start simple and scale up to full enterprise integrations as needed!

---

## 🚀 **Next Steps**

1. **Start with the simple git agent** to understand the basics
2. **Set up Ollama** for free LLM access
3. **Get a GitHub token** for API access
4. **Try the MCP integration** for advanced GitHub features
5. **Build custom tools** for your specific workflows

Which approach interests you most? Would you like me to elaborate on any of these options or help you set up a specific configuration?

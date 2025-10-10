using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class NarratorEngine : Node
{
    private Queue<string> _dialogueQueue;
    private bool _isProcessing;
    
    public override void _Ready()
    {
        _dialogueQueue = new Queue<string>();
        _isProcessing = false;
    }
    
    public void AddDialogue(string dialogue)
    {
        _dialogueQueue.Enqueue(dialogue);
    }
    
    public void AddDialogueRange(List<string> dialogues)
    {
        foreach (string dialogue in dialogues)
        {
            _dialogueQueue.Enqueue(dialogue);
        }
    }
    
    public string GetNextDialogue()
    {
        if (_dialogueQueue.Count > 0)
        {
            return _dialogueQueue.Dequeue();
        }
        return null;
    }
    
    public void ClearDialogueQueue()
    {
        _dialogueQueue.Clear();
    }
    
    public bool HasDialogue()
    {
        return _dialogueQueue.Count > 0;
    }
    
    public void ProcessDialogueQueue(Action<string> outputAction)
    {
        if (_isProcessing || !HasDialogue())
        {
            return;
        }
        
        _isProcessing = true;
        
        string dialogue = GetNextDialogue();
        if (dialogue != null && outputAction != null)
        {
            outputAction(dialogue);
        }
        
        _isProcessing = false;
    }
    
    // Simulate the typewriter effect by breaking text into smaller chunks
    public List<string> BreakTextIntoChunks(string text, int chunkSize = 10)
    {
        var chunks = new List<string>();
        if (string.IsNullOrEmpty(text))
        {
            return chunks;
        }
        
        string[] words = text.Split(' ');
        string currentChunk = "";
        
        foreach (string word in words)
        {
            if ((currentChunk + " " + word).Length > chunkSize && currentChunk != "")
            {
                chunks.Add(currentChunk);
                currentChunk = word;
            }
            else
            {
                if (currentChunk == "")
                {
                    currentChunk = word;
                }
                else
                {
                    currentChunk += " " + word;
                }
            }
        }
        
        if (currentChunk != "")
        {
            chunks.Add(currentChunk);
        }
        
        return chunks;
    }
    
    // Process narrative blocks and add them to the queue
    public void ProcessNarrativeBlock(List<string> paragraphs, Action<string>? outputAction = null)
    {
        foreach (string paragraph in paragraphs)
        {
            var chunks = BreakTextIntoChunks(paragraph, 20);
            foreach (string chunk in chunks)
            {
                AddDialogue(chunk);
                if (outputAction != null)
                {
                    ProcessDialogueQueue(outputAction);
                }
            }
        }
    }
}
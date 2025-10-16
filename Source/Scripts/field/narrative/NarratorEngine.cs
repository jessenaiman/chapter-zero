namespace OmegaSpiral.Source.Scripts.Field.Narrative;

// <copyright file="NarratorEngine.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Provides narrative management and dialogue queueing for field scenes.
/// </summary>
/// <remarks>
/// The <c>NarratorEngine</c> class manages dialogue lines, processes narrative blocks,
/// and supports typewriter effects for text output in Ωmega Spiral's field scenes.
/// </remarks>
[GlobalClass]
public partial class NarratorEngine : Node
{
    private Queue<string> dialogueQueue = new();
    private bool isProcessing;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.isProcessing = false;
    }

    /// <summary>
    /// Adds a single dialogue line to the queue.
    /// </summary>
    /// <param name="dialogue">The dialogue text to add.</param>
    public void AddDialogue(string dialogue)
    {
        this.dialogueQueue.Enqueue(dialogue);
    }

    /// <summary>
    /// Adds multiple dialogue lines to the queue.
    /// </summary>
    /// <param name="dialogues">The list of dialogue texts to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogues"/> is <see langword="null"/>.</exception>
    public void AddDialogueRange(List<string> dialogues)
    {
        ArgumentNullException.ThrowIfNull(dialogues);

        foreach (string dialogue in dialogues)
        {
            this.dialogueQueue.Enqueue(dialogue);
        }
    }

    /// <summary>
    /// Gets the next dialogue from the queue, or <see langword="null"/> if empty.
    /// </summary>
    /// <returns>The next dialogue string, or <see langword="null"/> if queue is empty.</returns>
    public string? GetNextDialogue()
    {
        if (this.dialogueQueue.Count > 0)
        {
            return this.dialogueQueue.Dequeue();
        }

        return null;
    }

    /// <summary>
    /// Clears all dialogue from the queue.
    /// </summary>
    public void ClearDialogueQueue()
    {
        this.dialogueQueue.Clear();
    }

    /// <summary>
    /// Checks if there is any dialogue in the queue.
    /// </summary>
    /// <returns><see langword="true"/> if dialogue is available, <see langword="false"/> otherwise.</returns>
    public bool HasDialogue()
    {
        return this.dialogueQueue.Count > 0;
    }

    /// <summary>
    /// Processes the next dialogue in the queue using the provided output action.
    /// </summary>
    /// <param name="outputAction">The action to perform with the dialogue text.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="outputAction"/> is <see langword="null"/>.</exception>
    public void ProcessDialogueQueue(Action<string> outputAction)
    {
        ArgumentNullException.ThrowIfNull(outputAction);

        if (this.isProcessing || !this.HasDialogue())
        {
            return;
        }

        this.isProcessing = true;

        string? dialogue = this.GetNextDialogue();
        if (dialogue != null)
        {
            outputAction(dialogue);
        }

        this.isProcessing = false;
    }

    /// <summary>
    /// Simulates the typewriter effect by breaking text into smaller chunks.
    /// </summary>
    /// <param name="text">The text to break into chunks.</param>
    /// <param name="chunkSize">The maximum size of each chunk (default 10).</param>
    /// <returns>A list of text chunks.</returns>
    public static List<string> BreakTextIntoChunks(string text, int chunkSize = 10)
    {
        var chunks = new List<string>();
        if (string.IsNullOrEmpty(text))
        {
            return chunks;
        }

        string[] words = text.Split(' ');
        string currentChunk = string.Empty;

        foreach (string word in words)
        {
            if ((currentChunk + " " + word).Length > chunkSize && !string.IsNullOrEmpty(currentChunk))
            {
                chunks.Add(currentChunk);
                currentChunk = word;
            }
            else
            {
                if (string.IsNullOrEmpty(currentChunk))
                {
                    currentChunk = word;
                }
                else
                {
                    currentChunk += " " + word;
                }
            }
        }

        if (!string.IsNullOrEmpty(currentChunk))
        {
            chunks.Add(currentChunk);
        }

        return chunks;
    }

    /// <summary>
    /// Processes narrative blocks and adds them to the dialogue queue.
    /// </summary>
    /// <param name="paragraphs">The list of paragraphs to process.</param>
    /// <param name="outputAction">Optional action to output each chunk immediately.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="paragraphs"/> is <see langword="null"/>.</exception>
    public void ProcessNarrativeBlock(List<string> paragraphs, Action<string>? outputAction = null)
    {
        ArgumentNullException.ThrowIfNull(paragraphs);

        foreach (string paragraph in paragraphs)
        {
            var chunks = BreakTextIntoChunks(paragraph, 20);
            foreach (string chunk in chunks)
            {
                this.AddDialogue(chunk);
                if (outputAction != null)
                {
                    this.ProcessDialogueQueue(outputAction);
                }
            }
        }
    }
}

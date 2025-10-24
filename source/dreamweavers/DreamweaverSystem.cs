// <copyright file="DreamweaverSystem.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Dreamweavers;

/// <summary>
/// Main orchestrator for the Dreamweaver LLM system.
/// Manages three personas (Hero, Shadow, Ambition) and coordinates
/// dynamic narrative generation using the AI framework.
/// Personas are created on-demand (lazy initialization).
/// </summary>
[GlobalClass]
public partial class DreamweaverSystem : Node
{
    private readonly Dictionary<string, Persona> _personas = new();
    private string? _activePersonaId;

    /// <summary>
    /// Emits the signal when a persona is activated.
    /// </summary>
    /// <param name="personaId">The identifier of the activated persona.</param>
    [Signal]
    public delegate void PersonaActivatedEventHandler(string personaId);

    /// <summary>
    /// Activates a persona, making it the primary narrative voice.
    /// Creates the persona on first access (lazy initialization).
    /// </summary>
    /// <param name="personaId">The identifier of the persona to activate ("hero", "shadow", "ambition").</param>
    public void ActivatePersona(string personaId)
    {
        // Get or create the persona
        if (!this._personas.ContainsKey(personaId))
        {
            var persona = Persona.GetPersona(personaId);
            if (persona == null)
            {
                GD.PrintErr($"Cannot activate unknown persona: {personaId}");
                return;
            }

            this._personas[personaId] = persona;
            GD.Print($"Created persona on demand: {personaId}");
        }

        this._activePersonaId = personaId;
        this.EmitSignal(SignalName.PersonaActivated, personaId);
        GD.Print($"Activated Dreamweaver persona: {personaId}");
    }

    /// <summary>
    /// Gets the currently active persona.
    /// </summary>
    /// <returns>The currently active persona, or null if none is active.</returns>
    public Persona? GetActivePersona()
    {
        if (this._activePersonaId == null)
        {
            return null;
        }

        return this._personas.GetValueOrDefault(this._activePersonaId);
    }

    /// <summary>
    /// Gets a persona by its identifier, creating it on first access if needed.
    /// </summary>
    /// <param name="personaId">The identifier of the persona to retrieve.</param>
    /// <returns>The persona with the given identifier, or null if not found.</returns>
    public Persona? GetPersona(string personaId)
    {
        // Get or create the persona
        if (!this._personas.ContainsKey(personaId))
        {
            var persona = Persona.GetPersona(personaId);
            if (persona == null)
            {
                GD.PrintErr($"Unknown persona: {personaId}");
                return null;
            }

            this._personas[personaId] = persona;
            GD.Print($"Created persona on demand: {personaId}");
        }

        return this._personas[personaId];
    }

    /// <summary>
    /// Gets the system prompt for a persona, which is used by the AI.
    /// </summary>
    /// <param name="personaId">The identifier of the persona.</param>
    /// <returns>The system prompt for the persona, or a default message if not found.</returns>
    public string GetPersonaSystemPrompt(string personaId)
    {
        var persona = this.GetPersona(personaId);
        if (persona == null)
        {
            GD.PrintErr($"Cannot get system prompt for unknown persona: {personaId}");
            return "You are a narrative guide in Omega Spiral.";
        }

        return persona.SystemPrompt;
    }

    /// <summary>
    /// Gets a fallback narrative for when LLM generation fails.
    /// </summary>
    /// <param name="personaId">The persona identifier.</param>
    /// <returns>A fallback narrative string.</returns>
    public static string GetFallbackNarrative(string personaId)
    {
        return personaId switch
        {
            "hero" => "The hero's path calls to you, filled with light and shadow.",
            "shadow" => "The shadows whisper secrets that only you can hear.",
            "ambition" => "Ambition drives you forward, carving new paths through reality.",
            _ => "The narrative continues...",
        };
    }
}

// <copyright file="INarrativeHandler.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Narrative;
    /// <summary>
    /// Minimal contract that a UI component must implement for the generic narrative engine.
    /// The engine drives the script; the UI implements the concrete presentation.
    /// </summary>
    public interface INarrativeHandler
    {
        /// <summary>
        /// Optional boot sequence (e.g., CRT shader preset, intro text).
        /// </summary>
        Task PlayBootSequenceAsync();

        /// <summary>
        /// Display a batch of lines in order.
        /// </summary>
        Task DisplayLinesAsync(IList<string> lines);

        /// <summary>
        /// Handle a special command line (e.g., "[GLITCH]"). Return true if the line was consumed.
        /// </summary>
        Task<bool> HandleCommandLineAsync(string line);

        /// <summary>
        /// Apply any scene‑specific visual or timing effects.
        /// </summary>
        Task ApplySceneEffectsAsync(NarrativeScriptElement scene);

        /// <summary>
        /// Present a question with choices and return the selected <see cref="ChoiceOption"/>.
        /// </summary>
        Task<ChoiceOption> PresentChoiceAsync(string question, string speaker, IList<ChoiceOption> choices);

        /// <summary>
        /// Called after a choice is made – update Dreamweaver scores or other game state.
        /// </summary>
        Task ProcessChoiceAsync(ChoiceOption selected);

        /// <summary>
        /// Notify the UI that the whole script has finished.
        /// </summary>
        Task NotifySequenceCompleteAsync();
    }

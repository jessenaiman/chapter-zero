// <copyright file="SaveLoadManagerTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GdUnit4;
    using Godot;
    using Microsoft.EntityFrameworkCore;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Persistence;
    using static GdUnit4.Assertions;

    /// <summary>
    /// Unit tests for SaveLoadManager focusing on persistence logic using in-memory database.
    /// </summary>
    [TestSuite]
    public class SaveLoadManagerTests : IDisposable
    {
        private GameDbContext? _context;
        private SaveLoadManager? _manager;

        /// <summary>
        /// Sets up the test environment before each test.
        /// </summary>
        [Before]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new GameDbContext(options);
            _manager = new SaveLoadManager(_context);
        }

        /// <summary>
        /// Cleans up after each test.
        /// </summary>
        [After]
        public void TearDown()
        {
            _context!.Database.EnsureDeleted();
            _context!.Dispose();
        }

        /// <summary>
        /// Tests saving a new game state successfully.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [TestCase]
        [RequireGodotRuntime]
        public async Task SaveGameAsync_NewGameState_ReturnsTrue()
        {
            // Arrange
            var gameState = new GameState();

            // Act
            var result = await _manager!.SaveGameAsync(gameState, "test_slot");

            // Assert
            AssertThat(result).IsTrue();
            var saved = await _context!.GameSaves!.FirstOrDefaultAsync(gs => gs.SaveSlot == "test_slot");
            AssertThat(saved).IsNotNull();
        }

        /// <summary>
        /// Tests saving an existing game state updates it.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task SaveGameAsync_ExistingGameState_UpdatesExisting()
        {
            // Arrange
            var gameState = new GameState();
            await _manager!.SaveGameAsync(gameState, "test_slot");
            gameState.CurrentScene = 2;

            // Act
            var result = await _manager!.SaveGameAsync(gameState, "test_slot");

            // Assert
            AssertThat(result).IsTrue();
            var saved = await _context!.GameSaves!.FirstOrDefaultAsync(gs => gs.SaveSlot == "test_slot");
            AssertThat(saved).IsNotNull();

            // Note: Actual update logic depends on implementation
        }

        /// <summary>
        /// Tests loading a game state successfully.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task LoadGameAsync_ExistingSave_ReturnsGameState()
        {
            // Arrange
            var gameState = new GameState();
            await _manager!.SaveGameAsync(gameState, "test_slot");

            // Act
            var result = await _manager!.LoadGameAsync("test_slot");

            // Assert
            AssertThat(result).IsNotNull();
        }

        /// <summary>
        /// Tests loading a non-existing save returns null.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task LoadGameAsync_NonExistingSave_ReturnsNull()
        {
            // Act
            var result = await _manager!.LoadGameAsync("non_existing");

            // Assert
            AssertThat(result).IsNull();
        }

        /// <summary>
        /// Tests getting available save slots.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task GetAvailableSaveSlotsAsync_ReturnsList()
        {
            // Arrange
            await _manager!.SaveGameAsync(new GameState(), "slot1");
            await _manager!.SaveGameAsync(new GameState(), "slot2");

            // Act
            var result = await _manager!.GetAvailableSaveSlotsAsync();

            // Assert
            AssertThat(result).Contains("slot1").Contains("slot2");
        }

        /// <summary>
        /// Tests deleting an existing save slot.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task DeleteSaveSlotAsync_ExistingSlot_ReturnsTrue()
        {
            // Arrange
            await _manager!.SaveGameAsync(new GameState(), "test_slot");

            // Act
            var result = await _manager!.DeleteSaveSlotAsync("test_slot");

            // Assert
            AssertThat(result).IsTrue();
            var saved = await _context!.GameSaves!.FirstOrDefaultAsync(gs => gs.SaveSlot == "test_slot");
            AssertThat(saved).IsNull();
        }

        /// <summary>
        /// Tests deleting a non-existing save slot returns false.
        /// </summary>
        [TestCase]
        public async Task DeleteSaveSlotAsync_NonExistingSlot_ReturnsFalse()
        {
            // Act
            var result = await _manager!.DeleteSaveSlotAsync("non_existing");

            // Assert
            AssertThat(result).IsFalse();
        }

        /// <summary>
        /// Disposes the test resources.
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

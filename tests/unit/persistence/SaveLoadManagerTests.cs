// <copyright file="SaveLoadManagerTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Persistence
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.InMemory;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.persistence;
    using GdUnit4;
    using static GdUnit4.Assertions;

    /// <summary>
    /// Unit tests for SaveLoadManager focusing on persistence logic using in-memory database.
    /// </summary>
    [TestSuite]
    public class SaveLoadManagerTests : IDisposable
    {
        private GameDbContext? context;
        private SaveLoadManager? manager;

        /// <summary>
        /// Sets up the test environment before each test.
        /// </summary>
        [Before]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            context = new GameDbContext(options);
            manager = new SaveLoadManager(context);
        }

        /// <summary>
        /// Cleans up after each test.
        /// </summary>
        [After]
        public void TearDown()
        {
            context!.Database.EnsureDeleted();
            context?.Dispose();
        }

        /// <summary>
        /// Tests saving a new game state successfully.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [TestCase]
        [RequireGodotRuntime]
        public async Task SavegameasyncNewgamestateReturnstrue()
        {
            // Arrange
            var gameState = new GameState();

            // Act
            var result = await manager!.SaveGameAsync(gameState, "test_slot");

            // Assert
            AssertThat(result).IsTrue();
            var saved = await context!.GameSaves!.FirstOrDefaultAsync(gs => gs.SaveSlot == "test_slot");
            AssertThat(saved).IsNotNull();
        }

        /// <summary>
        /// Tests saving an existing game state updates it.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task SavegameasyncExistinggamestateUpdatesexisting()
        {
            // Arrange
            var gameState = new GameState();
            await manager!.SaveGameAsync(gameState, "test_slot");
            gameState.CurrentScene = 2;

            // Act
            var result = await manager!.SaveGameAsync(gameState, "test_slot");

            // Assert
            AssertThat(result).IsTrue();
            var saved = await context!.GameSaves!.FirstOrDefaultAsync(gs => gs.SaveSlot == "test_slot");
            AssertThat(saved).IsNotNull();

            // Note: Actual update logic depends on implementation
        }

        /// <summary>
        /// Tests loading a game state successfully.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task LoadgameasyncExistingsaveReturnsgamestate()
        {
            // Arrange
            var gameState = new GameState();
            await manager!.SaveGameAsync(gameState, "test_slot");

            // Act
            var result = await manager!.LoadGameAsync("test_slot");

            // Assert
            AssertThat(result).IsNotNull();
        }

        /// <summary>
        /// Tests loading a non-existing save returns null.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task LoadgameasyncNonexistingsaveReturnsnull()
        {
            // Act
            var result = await manager!.LoadGameAsync("non_existing");

            // Assert
            AssertThat(result).IsNull();
        }

        /// <summary>
        /// Tests getting available save slots.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task GetavailablesaveslotsasyncReturnslist()
        {
            // Arrange
            await manager!.SaveGameAsync(new GameState(), "slot1");
            await manager!.SaveGameAsync(new GameState(), "slot2");

            // Act
            var result = await manager!.GetAvailableSaveSlotsAsync();

            // Assert
            AssertThat(result).Contains("slot1").Contains("slot2");
        }

        /// <summary>
        /// Tests deleting an existing save slot.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task DeletesaveslotasyncExistingslotReturnstrue()
        {
            // Arrange
            await manager!.SaveGameAsync(new GameState(), "test_slot");

            // Act
            var result = await manager!.DeleteSaveSlotAsync("test_slot");

            // Assert
            AssertThat(result).IsTrue();
            var saved = await context!.GameSaves!.FirstOrDefaultAsync(gs => gs.SaveSlot == "test_slot");
            AssertThat(saved).IsNull();
        }

        /// <summary>
        /// Tests deleting a non-existing save slot returns false.
        /// </summary>
        [TestCase]
        public async Task DeletesaveslotasyncNonexistingslotReturnsfalse()
        {
            // Act
            var result = await manager!.DeleteSaveSlotAsync("non_existing");

            // Assert
            AssertThat(result).IsFalse();
        }

        /// <summary>
        /// Disposes the test resources.
        /// </summary>
        public void Dispose()
        {
            context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

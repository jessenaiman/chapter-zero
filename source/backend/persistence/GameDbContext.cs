// <copyright file="GameDbContext.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using Microsoft.EntityFrameworkCore;

namespace OmegaSpiral.Source.Backend.Persistence;
    /// <summary>
    /// Database context for the Ωmega Spiral game save system.
    /// </summary>
    public class GameDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the game saves.
        /// </summary>
        public DbSet<GameSave>? GameSaves { get; set; }

        /// <summary>
        /// Gets or sets the story shards.
        /// </summary>
        public DbSet<StoryShard>? StoryShards { get; set; }

        /// <summary>
        /// Gets or sets the scene data.
        /// </summary>
        public DbSet<SceneData>? SceneData { get; set; }

        /// <summary>
        /// Gets or sets the Dreamweaver scores for persistence.
        /// </summary>
        public DbSet<DreamweaverScoreEntity>? DreamweaverScores { get; set; }

        /// <summary>
        /// Gets or sets the party data.
        /// </summary>
        public DbSet<PartySaveData>? PartyData { get; set; }

        /// <summary>
        /// Gets or sets the character data.
        /// </summary>
        public DbSet<CharacterSaveData>? CharacterData { get; set; }

        /// <summary>
        /// Gets or sets the narrator messages.
        /// </summary>
        public DbSet<NarratorMessage>? NarratorMessages { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameDbContext"/> class.
        /// </summary>
        /// <param name="options">The database context options.</param>
        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options)
        {
            // Ensure database is created and migrations are applied
            Database.EnsureCreated();
        }

        /// <summary>
        /// Configures the database context.
        /// </summary>
        /// <param name="optionsBuilder">The options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Default to SQLite database in user directory
                var dbPath = Path.Combine(OS.GetUserDataDir(), "omega_spiral_saves.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        /// <summary>
        /// Configures the entity relationships and constraints.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure GameSave relationships
            modelBuilder.Entity<GameSave>()
                .HasMany(gs => gs.Shards)
                .WithOne(s => s.GameSave)
                .HasForeignKey(s => s.GameSaveId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameSave>()
                .HasMany(gs => gs.SceneData)
                .WithOne(sd => sd.GameSave)
                .HasForeignKey(sd => sd.GameSaveId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameSave>()
                .HasMany(gs => gs.DreamweaverScores)
                .WithOne(ds => ds.GameSave)
                .HasForeignKey(ds => ds.GameSaveId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameSave>()
                .HasOne(gs => gs.PartyData)
                .WithOne()
                .HasForeignKey<PartySaveData>(pd => pd.GameSaveId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameSave>()
                .HasMany(gs => gs.NarratorQueue)
                .WithOne(nm => nm.GameSave)
                .HasForeignKey(nm => nm.GameSaveId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure PartySaveData relationships
            modelBuilder.Entity<PartySaveData>()
                .HasMany(pd => pd.Members)
                .WithOne(c => c.PartySaveData)
                .HasForeignKey(c => c.PartySaveDataId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes for performance
            modelBuilder.Entity<GameSave>()
                .HasIndex(gs => gs.SaveSlot)
                .IsUnique();

            modelBuilder.Entity<SceneData>()
                .HasIndex(sd => new { sd.GameSaveId, sd.Key });

            modelBuilder.Entity<DreamweaverScoreEntity>()
                .HasIndex(ds => new { ds.GameSaveId, ds.DreamweaverType })
                .IsUnique();
        }
    }

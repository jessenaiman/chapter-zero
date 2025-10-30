using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

#pragma warning disable CA1852

namespace OmegaSpiral.Source.Backend.Persistence.Migrations;
    [DbContext(typeof(OmegaSpiral.Source.Scripts.persistence.GameDbContext))]
    partial class GameDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.GameSave", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("CurrentScene")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DreamweaverThread")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("PlayerName")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("PlayerSecret")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("SaveSlot")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("SaveVersion")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<int?>("SelectedDreamweaver")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SaveSlot")
                        .IsUnique();

                    b.ToTable("GameSaves");
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.CharacterSaveData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Experience")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Health")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxHealth")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PartySaveDataId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PartySaveDataId");

                    b.ToTable("CharacterSaveData");
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.DreamweaverScore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DreamweaverType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameSaveId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Score")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameSaveId", "DreamweaverType")
                        .IsUnique();

                    b.ToTable("DreamweaverScores");
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.NarratorMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameSaveId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRead")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GameSaveId");

                    b.ToTable("NarratorMessages");
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.PartySaveData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameSaveId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameSaveId")
                        .IsUnique();

                    b.ToTable("PartySaveData");
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.SceneData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameSaveId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GameSaveId", "Key");

                    b.ToTable("SceneData");
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.StoryShard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CollectedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("GameSaveId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ShardId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GameSaveId");

                    b.ToTable("StoryShards");
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.CharacterSaveData", b =>
                {
                    b.HasOne("OmegaSpiral.Source.Scripts.Persistence.PartySaveData", "PartySaveData")
                        .WithMany("Members")
                        .HasForeignKey("PartySaveDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.DreamweaverScore", b =>
                {
                    b.HasOne("OmegaSpiral.Source.Scripts.Persistence.GameSave", "GameSave")
                        .WithMany("DreamweaverScores")
                        .HasForeignKey("GameSaveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.NarratorMessage", b =>
                {
                    b.HasOne("OmegaSpiral.Source.Scripts.Persistence.GameSave", "GameSave")
                        .WithMany("NarratorQueue")
                        .HasForeignKey("GameSaveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.PartySaveData", b =>
                {
                    b.HasOne("OmegaSpiral.Source.Scripts.Persistence.GameSave", "GameSave")
                        .WithOne("PartyData")
                        .HasForeignKey("OmegaSpiral.Source.Scripts.Persistence.PartySaveData", "GameSaveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.SceneData", b =>
                {
                    b.HasOne("OmegaSpiral.Source.Scripts.Persistence.GameSave", "GameSave")
                        .WithMany("SceneData")
                        .HasForeignKey("GameSaveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OmegaSpiral.Source.Scripts.Persistence.StoryShard", b =>
                {
                    b.HasOne("OmegaSpiral.Source.Scripts.Persistence.GameSave", "GameSave")
                        .WithMany("Shards")
                        .HasForeignKey("GameSaveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }

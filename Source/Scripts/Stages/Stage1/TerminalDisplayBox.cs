// <copyright file="TerminalDisplayBox.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Field.Narrative;

/// <summary>
/// 3D terminal display box with realistic geometry, materials, and elevated positioning.
/// Creates a physical terminal-like appearance with depth perception and realistic screen effects.
/// </summary>
[GlobalClass]
public partial class TerminalDisplayBox : Node3D
{
    private MeshInstance3D screenMesh = default!;
    private MeshInstance3D bezelMesh = default!;
    private MeshInstance3D caseMesh = default!;
    private DirectionalLight3D ambientLight = default!;
    private OmniLight3D screenGlowLight = default!;
    private Camera3D viewingCamera = default!;

    [Export] public float ScreenWidth { get; set; } = 8.0f;
    [Export] public float ScreenHeight { get; set; } = 4.5f;
    [Export] public float BezelThickness { get; set; } = 0.2f;
    [Export] public float CaseThickness { get; set; } = 0.5f;
    [Export] public Color ScreenGlowColor { get; set; } = new Color(0.2f, 1.0f, 0.2f);
    [Export] public float ScreenGlowIntensity { get; set; } = 2.0f;
    [Export] public bool EnableScreenGlow { get; set; } = true;
    [Export] public bool EnableAmbientLight { get; set; } = true;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.CreateTerminalGeometry();
        this.SetupLighting();
        this.SetupCamera();
        this.ConfigureElevationEffect();
    }

    private void ConfigureElevationEffect()
    {
        // Configure the terminal to appear elevated with enhanced depth perception
        // Position the entire display box to create elevation from the background
        this.Position = new Vector3(0, 0, 2.0f); // Move forward to create depth perception

        // Add a subtle shadow effect by creating a shadow plane
        this.CreateShadowPlane();

        // Add depth cues by adjusting the case positioning
        if (this.caseMesh != null)
        {
            this.caseMesh.Position = new Vector3(0, 0, -((this.CaseThickness / 2) + 0.1f)); // Slightly behind for depth
        }

        // Add a subtle angle to the screen for better 3D perception
        if (this.screenMesh != null)
        {
            this.screenMesh.Rotation = new Vector3(-0.1f, 0, 0); // Slight downward tilt
        }

        // Add depth perception enhancements
        this.AddDepthPerceptionEffects();
    }

    private void AddDepthPerceptionEffects()
    {
        // Add multiple depth layers for enhanced perception
        this.CreateDepthLayerEffects();

        // Add parallax effects for better depth perception
        SetupParallaxDepth();

        // Add depth-based lighting effects
        this.SetupDepthBasedLighting();
    }

    private void CreateDepthLayerEffects()
    {
        // Create multiple depth layers to enhance the 3D perception
        var depthLayers = 3;
        var layerSpacing = 0.05f;

        for (int i = 1; i <= depthLayers; i++)
        {
            var layerSize = new Vector2(
                this.ScreenWidth - (i * 0.1f),
                this.ScreenHeight - (i * 0.1f)
            );

            var layerPlane = new PlaneMesh
            {
                Size = layerSize,
                SubdivideDepth = 4,
                SubdivideWidth = 4
            };

            var layerMaterial = new StandardMaterial3D
            {
                AlbedoColor = new Color(0.02f, 0.08f, 0.02f, 0.3f), // Semi-transparent green
                Metallic = 0.7f,
                Roughness = 0.3f,
                Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
                BlendMode = BaseMaterial3D.BlendModeEnum.Mix
            };

            var layerMesh = new MeshInstance3D
            {
                Mesh = layerPlane,
                Position = new Vector3(0, 0, this.BezelThickness + 0.01f + (i * layerSpacing))
            };

            layerMesh.SetSurfaceOverrideMaterial(0, layerMaterial);
            this.AddChild(layerMesh);
        }
    }

    private static void SetupParallaxDepth()
    {
        // Create a parallax effect by having different elements move at different speeds
        // This is handled in the Process method for dynamic effects
    }

    private void SetupDepthBasedLighting()
    {
        // Add depth-based lighting to enhance the 3D perception
        if (this.ambientLight != null)
        {
            // Configure the ambient light to create depth-based shadows
            this.ambientLight.LightEnergy = 0.2f;
            this.ambientLight.LightIndirectEnergy = 0.6f;
        }

        if (this.screenGlowLight != null)
        {
            // Configure the screen glow to create depth-based illumination
            // Note: LightAttenuation removed in Godot 4.5+, attenuation is automatic
            this.screenGlowLight.LightSpecular = 0.9f;
        }
    }

    private void CreateShadowPlane()
    {
        // Create a subtle shadow plane behind the terminal to enhance elevation effect
        var shadowSize = new Vector2(this.ScreenWidth + (this.CaseThickness * 3), this.ScreenHeight + (this.CaseThickness * 3));
        var shadowPlane = new PlaneMesh
        {
            Size = shadowSize,
            SubdivideDepth = 4,
            SubdivideWidth = 4
        };

        var shadowMaterial = new StandardMaterial3D
        {
            AlbedoColor = new Color(0, 0, 0, 0.3f), // Semi-transparent black
            Metallic = 0.0f,
            Roughness = 1.0f,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            BillboardMode = BaseMaterial3D.BillboardModeEnum.Enabled // Always face camera for shadow effect
        };

        var shadowMesh = new MeshInstance3D
        {
            Mesh = shadowPlane,
            Position = new Vector3(0, -0.2f, -0.5f), // Positioned below and behind
            Rotation = new Vector3(Mathf.Pi * 0.5f, 0, 0) // Rotated to lay flat
        };

        shadowMesh.SetSurfaceOverrideMaterial(0, shadowMaterial);
        this.AddChild(shadowMesh);
    }

    private void CreateTerminalGeometry()
    {
        // Create the main terminal case (back panel)
        var caseMeshSize = new Vector3(this.ScreenWidth + (this.CaseThickness * 2), this.ScreenHeight + (this.CaseThickness * 2), this.CaseThickness);
        var caseMaterial = CreateRealisticCaseMaterial();
        var caseMeshData = CreateBoxMeshWithMaterial(caseMeshSize, caseMaterial);
        this.caseMesh = new MeshInstance3D { Mesh = caseMeshData };
        this.caseMesh.Position = new Vector3(0, 0, -this.CaseThickness / 2);
        this.AddChild(this.caseMesh);

        // Create the bezel (screen frame)
        var bezelSize = new Vector3(this.ScreenWidth + this.BezelThickness, this.ScreenHeight + this.BezelThickness, this.BezelThickness);
        var bezelMaterial = CreateRealisticBezelMaterial();
        var bezelMeshData = CreateBoxMeshWithMaterial(bezelSize, bezelMaterial);
        this.bezelMesh = new MeshInstance3D { Mesh = bezelMeshData };
        this.bezelMesh.Position = new Vector3(0, 0, this.BezelThickness / 2);
        this.AddChild(this.bezelMesh);

        // Create the screen mesh (the actual display area)
        var screenPlane = new PlaneMesh
        {
            Size = new Vector2(this.ScreenWidth, this.ScreenHeight),
            SubdivideDepth = 8,
            SubdivideWidth = 8
        };

        var screenMaterial = this.CreateRealisticScreenMaterial();
        this.screenMesh = new MeshInstance3D
        {
            Mesh = screenPlane,
            Position = new Vector3(0, 0, this.BezelThickness + 0.01f) // Slightly in front of bezel
        };

        this.screenMesh.SetSurfaceOverrideMaterial(0, screenMaterial);
        this.AddChild(this.screenMesh);

        // Add subtle rounded corners effect by creating a border
        this.CreateScreenBorder();
    }

    private void CreateScreenBorder()
    {
        // Create a thin border mesh around the screen for more realistic appearance
        var borderThickness = 0.02f;
        var borderPositions = new Vector3[]
        {
            // Top border
            new Vector3(0, (this.ScreenHeight / 2) + (borderThickness / 2), 0),
            // Bottom border
            new Vector3(0, -(this.ScreenHeight / 2) - (borderThickness / 2), 0),
            // Left border
            new Vector3(-(this.ScreenWidth / 2) - (borderThickness / 2), 0, 0),
            // Right border
            new Vector3((this.ScreenWidth / 2) + (borderThickness / 2), 0, 0)
        };

        var borderSizes = new Vector3[]
        {
            new Vector3(this.ScreenWidth + (borderThickness * 2), borderThickness, 0.01f),
            new Vector3(this.ScreenWidth + (borderThickness * 2), borderThickness, 0.01f),
            new Vector3(borderThickness, this.ScreenHeight, 0.01f),
            new Vector3(borderThickness, this.ScreenHeight, 0.01f)
        };

        // Create a subtle border material that's slightly different from bezel
        var borderMaterial = new StandardMaterial3D
        {
            AlbedoColor = new Color(0.08f, 0.08f, 0.1f), // Slightly different from bezel
            Metallic = 0.5f,
            Roughness = 0.5f
        };

        for (int i = 0; i < borderPositions.Length; i++)
        {
            var borderMesh = CreateBoxMeshWithMaterial(borderSizes[i], borderMaterial);
            var borderInstance = new MeshInstance3D { Mesh = borderMesh };
            borderInstance.Position = borderPositions[i];
            borderInstance.Position += new Vector3(0, 0, this.BezelThickness + 0.015f); // Slightly in front
            this.AddChild(borderInstance);
        }
    }

    private static ArrayMesh CreateBoxMesh(Vector3 size, Color color)
    {
        var mesh = new ArrayMesh();

        // Create a box shape
        var boxMesh = new BoxMesh
        {
            Size = size,
            SubdivideWidth = 1,
            SubdivideHeight = 1,
            SubdivideDepth = 1
        };

        // Create material
        var material = new StandardMaterial3D
        {
            AlbedoColor = color,
            Metallic = 0.3f,
            Roughness = 0.7f
        };

        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, boxMesh.GetMeshArrays());
        mesh.SurfaceSetMaterial(0, material);

        return mesh;
    }

    private static ArrayMesh CreateBoxMeshWithMaterial(Vector3 size, StandardMaterial3D material)
    {
        var mesh = new ArrayMesh();

        // Create a box shape
        var boxMesh = new BoxMesh
        {
            Size = size,
            SubdivideWidth = 2,
            SubdivideHeight = 2,
            SubdivideDepth = 2
        };

        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, boxMesh.GetMeshArrays());
        mesh.SurfaceSetMaterial(0, material);

        return mesh;
    }

    private static StandardMaterial3D CreateRealisticCaseMaterial()
    {
        // Create a realistic plastic/metal case material
        var material = new StandardMaterial3D
        {
            AlbedoColor = new Color(0.1f, 0.1f, 0.12f), // Dark gray with slight blue tint
            Metallic = 0.4f, // Some metallic reflection
            Roughness = 0.6f, // Matte finish
            NormalEnabled = true,
            // Add subtle texture for plastic/metal appearance
            DetailEnabled = true,
            Uv1Scale = new Vector3(2, 2, 1)
        };

        return material;
    }

    private static StandardMaterial3D CreateRealisticBezelMaterial()
    {
        // Create a realistic bezel material (the frame around the screen)
        var material = new StandardMaterial3D
        {
            AlbedoColor = new Color(0.05f, 0.05f, 0.07f), // Very dark gray
            Metallic = 0.6f, // More metallic than case
            Roughness = 0.4f, // Slightly glossy
            RimEnabled = true, // Add edge lighting effect
            Rim = 0.2f,
            ClearcoatEnabled = true, // Add protective coating effect
            Clearcoat = 0.3f
        };

        return material;
    }

    private StandardMaterial3D CreateRealisticScreenMaterial()
    {
        // Create a realistic CRT/LCD screen material
        var material = new StandardMaterial3D
        {
            AlbedoColor = new Color(0.02f, 0.05f, 0.02f, 1.0f), // Very dark green
            EmissionEnabled = true,
            Emission = this.ScreenGlowColor,
            EmissionEnergyMultiplier = this.EnableScreenGlow ? this.ScreenGlowIntensity : 0.0f,
            Metallic = 0.9f, // Highly reflective like screen
            Roughness = 0.1f, // Very smooth
            CullMode = BaseMaterial3D.CullModeEnum.Front,
            // Add screen-specific effects
            RefractionEnabled = true,
            RefractionScale = 0.1f,
            // Add subtle screen texture for realism
            DetailEnabled = true
        };

        return material;
    }

    private void SetupLighting()
    {
        if (this.EnableAmbientLight)
        {
            // Ambient light for overall scene illumination
            this.ambientLight = new DirectionalLight3D
            {
                Name = "AmbientLight",
                Position = new Vector3(-2, -3, -1),
                Rotation = new Vector3(Mathf.Pi * 0.4f, Mathf.Pi * 0.3f, 0),
                LightColor = new Color(0.8f, 0.8f, 0.9f),
                LightEnergy = 0.3f,
                LightIndirectEnergy = 0.5f
            };
            this.AddChild(this.ambientLight);
        }

        // Screen glow light for the terminal effect
        this.screenGlowLight = new OmniLight3D
        {
            Name = "ScreenGlowLight",
            Position = new Vector3(0, 0, this.BezelThickness + 0.1f), // Positioned at screen surface
            LightColor = this.ScreenGlowColor,
            LightEnergy = this.EnableScreenGlow ? this.ScreenGlowIntensity * 0.5f : 0.0f,
            LightSpecular = 0.8f
        };
        this.AddChild(this.screenGlowLight);
    }

    private void SetupCamera()
    {
        // Create a camera for the terminal display perspective
        this.viewingCamera = new Camera3D
        {
            Name = "TerminalCamera",
            Position = new Vector3(0, 0, 10),
            Fov = 30.0f,
            Near = 0.1f,
            Far = 50.0f
        };
        this.AddChild(this.viewingCamera);
    }

    /// <summary>
    /// Updates the screen glow intensity for dynamic effects.
    /// </summary>
    /// <param name="intensity">The new glow intensity value.</param>
    public void UpdateScreenGlow(float intensity)
    {
        if (this.screenMesh != null)
        {
            var material = (StandardMaterial3D)this.screenMesh.GetActiveMaterial(0);
            if (material != null)
            {
                material.EmissionEnergyMultiplier = intensity;
            }
        }

        if (this.screenGlowLight != null)
        {
            this.screenGlowLight.LightEnergy = intensity * 0.5f;
        }
    }

    /// <summary>
    /// Gets the screen mesh for text overlay positioning.
    /// </summary>
    /// <returns>The screen mesh instance.</returns>
    public MeshInstance3D GetScreenMesh()
    {
        return this.screenMesh;
    }

    /// <summary>
    /// Gets the camera for the terminal view.
    /// </summary>
    /// <returns>The viewing camera.</returns>
    public Camera3D GetViewingCamera()
    {
        return this.viewingCamera;
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        // Add subtle pulsing effect to the screen glow
        if (this.EnableScreenGlow && this.screenMesh != null)
        {
            var time = (float)Time.GetTicksMsec() / 1000.0f;
            var pulse = (Mathf.Sin(time * 2.0f) * 0.1f) + 1.0f; // Gentle pulsing
            this.UpdateScreenGlow(this.ScreenGlowIntensity * pulse);
        }

        // Add dynamic depth perception effects
        this.UpdateDepthPerceptionEffects();
    }

    private void UpdateDepthPerceptionEffects()
    {
        var time = (float)Time.GetTicksMsec() / 1000.0f;

        // Update parallax depth effects for enhanced 3D perception
        var parallaxOffset = (float)Math.Sin(time * 0.8f) * 0.02f;

        // Apply subtle movement to different depth layers for parallax effect
        var children = this.GetChildren();
        foreach (var child in children)
        {
            if (child is MeshInstance3D meshInstance)
            {
                // Skip the main screen mesh as it should remain stable
                if (meshInstance == this.screenMesh || meshInstance == this.bezelMesh || meshInstance == this.caseMesh)
                    continue;

                // Apply depth-based parallax effect
                var currentPos = meshInstance.Position;
                var depthFactor = meshInstance.Position.Z - (this.BezelThickness + 0.01f);
                var parallaxAmount = depthFactor * 0.1f; // More parallax for elements further back

                meshInstance.Position = new Vector3(
                    currentPos.X + (parallaxOffset * parallaxAmount),
                    currentPos.Y + (parallaxOffset * parallaxAmount * 0.5f), // Less vertical movement
                    currentPos.Z
                );
            }
        }

        // Add subtle depth-based scaling effect
        var depthScale = 1.0f + ((float)Math.Sin(time * 1.2f) * 0.005f);
        this.Scale = new Vector3(depthScale, depthScale, depthScale);
    }
}

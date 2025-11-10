# ShaderNoiseLib Addon - Procedural Noise Functions Guide

## Overview

**ShaderNoiseLib** is a Godot addon that provides three procedural noise generation functions as custom Visual Shader nodes. These are GPU-accelerated noise algorithms that can be used in shaders for visual effects, terrain generation, cloud rendering, and more.

The addon adds **custom VisualShaderNodeCustom** classes that integrate seamlessly into Godot's Visual Shader editor.

## Three Noise Types

### 1. Perlin Noise 3D (`VisualShaderNodePerlinNoise3D`)

**What it does**: Classic Perlin noise - smooth, continuous gradient noise with natural-looking variation.

**Inputs**:

- `uvw` (Vector3): Coordinate in 3D space to sample noise from
- `offset` (Vector3): Shifts the noise pattern (default: 0,0,0)
- `scale` (Vector3): Controls frequency/zoom (default: 10,10,10) - higher = more detail

**Output**:

- `result` (float): Noise value between 0 and 1

**Characteristics**:

- Smooth, organic appearance
- Good for clouds, terrain, water
- Relatively smooth interpolation between values
- Best for creating natural-looking textures

**Use Cases**:

- Cloud/sky patterns
- Terrain height maps
- Water surface displacement
- Organic texture generation
- Fog/mist effects

---

### 2. Voronoi Noise 3D (`VisualShaderNodeVoronoiNoise3D`)

**What it does**: Cellular/Worley noise - creates a pattern of cells, like cracks, scales, or organic tissues.

**Inputs**:

- `uvw` (Vector3): Coordinate in 3D space
- `offset` (Vector3): Pattern shift
- `scale` (Vector3): Cell size control (default: 10,10,10) - higher = smaller cells
- `distance_scale` (float): Amplifies distance differences
- `distance_function` (int): How distances are calculated (0-4)
  - 0: Euclidean (smooth)
  - 1: Manhattan (blocky)
  - 2: Chebyshev
  - 3: Minkowski
  - 4: Custom
- `jitter` (float): Randomness of cell positions (0-1)
- `3d` (bool): Use full 3D or 2D patterns

**Outputs**:

- `cell_value` (float): Which cell you're in
- `distance` (float): Distance to nearest cell
- `distance2` (float): Distance to second nearest cell

**Characteristics**:

- Creates recognizable cellular patterns
- Sharp, defined structures
- Good for organic yet geometric textures
- Multiple distance outputs for complex effects

**Use Cases**:

- Dragon scales or reptile skin
- Rock/mineral formations
- Honeycomb patterns
- Crack systems
- Insect wing patterns
- Cellular biology visualization
- Procedural surface deformation

---

### 3. Pixel Noise 3D (`VisualShaderNodePixelNoise3D`)

**What it does**: Fast, scalable random noise - pure randomness without smoothing.

**Inputs**:

- `uvw` (Vector3): Coordinate in 3D space
- `offset` (Vector3): Pattern shift
- `scale` (Vector3): Pixel/cell size (default: 10,10,10)

**Output**:

- `noise` (float): Random value between 0 and 1

**Characteristics**:

- Fast computation
- Completely random, no smoothing
- Creates blocky/pixelated patterns
- Good for seeding other noise functions

**Use Cases**:

- Quick randomization
- Dithering patterns
- Seeding higher-order noise (Fractal Brownian Motion)
- Random color/value lookup
- Static/grain effects

---

## How to Use in Omega Spiral

### Access in Visual Shader Editor

1. Open any scene with a **ShaderMaterial**
2. In the ShaderMaterial, open the **Visual Shader**
3. In the Visual Shader editor panel, look for **NoiseLib** category
4. Available nodes:
   - `PerlinNoise3D`
   - `VoronoiNoise3D`
   - `PixelNoise3D`

### Basic Example: Perlin Noise Cloud

```glsl
# In Visual Shader:
1. Add PerlinNoise3D node
2. Connect UV (from texture coordinates) to "uvw" input
3. Set scale to control cloud size (10-50 for subtle, 100+ for detailed)
4. Connect output to emission or albedo
5. Optional: Add ColorRamp to map grayscale to colors
```

### Combined Effects Example

For complex effects, layer multiple noise types:

```glsl
# Terrain with rocky surface detail:
1. Perlin base for smooth terrain (scale 30)
2. Voronoi cells (scale 100) for rock formations
3. Pixel noise for fine detail (scale 500)
4. Combine using mix() function
```

---

## Technical Implementation

### How Shaders Work

All three functions use **hash-based algorithms**:

1. **Hash function**: Converts input coordinates to pseudo-random values
   - Uses matrix multiplication for distribution
   - Creates repeating pattern but appears random

2. **Interpolation**: For Perlin, smoothly blends between random values
   - Fade function creates smooth transitions
   - Produces organic variation

3. **Normalization**: Output range typically 0-1
   - Perlin: returns scaled value (multiply by 0.5, add 0.5)
   - Voronoi: distance-based, multiple outputs
   - Pixel: direct random output

### Performance Characteristics

| Noise Type | Performance | Quality | Detail |
|-----------|-------------|---------|--------|
| Pixel | Fastest | Low (random) | None |
| Voronoi | Fast | Medium (cellular) | Cell-based |
| Perlin | Medium | High (smooth) | Gradient-based |

For **real-time use**: All are GPU-accelerated and fast enough for fullscreen effects.

---

## Integration with Star Backgrounds

### Enhanced Star Background with Noise

You can use these noise functions to enhance your procedural star backgrounds:

```glsl
# Enhanced Procedural Starfield with Nebula:
1. Base procedural stars (from previous solutions)
2. Add Perlin noise layer (scale 5-10) for nebula clouds
3. Add slight Voronoi (scale 50) for dust particle effect
4. Combine using multiply or screen blend modes
```

### Nebula Effect Example

```glsl
# Create animated nebula behind stars:
uniform float nebula_scale = 10.0;
uniform float time_scale = 0.5;

void fragment() {
    // Use noise with time for animation
    vec3 sample_point = vec3(UV, TIME * time_scale);
    
    // Multiple noise layers for complexity
    float nebula1 = perlin_noise(sample_point * nebula_scale);
    float nebula2 = perlin_noise(sample_point * nebula_scale * 2.0 + 100.0);
    
    // Combine
    float nebula = mix(nebula1, nebula2, 0.5);
    
    // Color based on value
    vec3 nebula_color = mix(
        vec3(0.2, 0.1, 0.4),  // Purple
        vec3(0.8, 0.3, 0.6),  // Magenta
        nebula
    );
    
    COLOR = vec4(nebula_color, nebula * 0.5);
}
```

---

## Configuration Tips

### For Omega Spiral Narrative Moods

**Calm/Peaceful Scenes**:

- Perlin noise, scale 15-25
- Low detail, smooth variations
- Warm color gradients

**Chaotic/Dangerous Scenes**:

- Voronoi with high jitter (0.8-1.0)
- Mix with Perlin for complexity
- Sharp color transitions

**Mystery/Unknown**:

- Combine all three types
- Animated scale changes
- Color cycling

### Parameter Guidelines

| Effect | Noise Type | Scale | Offset Animation | Jitter |
|--------|-----------|-------|-----------------|--------|
| Clouds | Perlin | 10-30 | TIME * 0.1 | N/A |
| Terrain | Perlin | 30-100 | None | N/A |
| Rocks | Voronoi | 50-200 | None | 0.5-0.8 |
| Static | Pixel | 50-500 | None | N/A |
| Nebula | Perlin | 5-20 | TIME * 0.2 | N/A |
| Fractals | All 3 | Layered | Varies | Varies |

---

## Advanced: Fractal Brownian Motion (FBM)

Layer multiple noise octaves for complex detail:

```glsl
float fbm(vec3 p) {
    float value = 0.0;
    float amplitude = 1.0;
    float frequency = 1.0;
    
    for(int i = 0; i < 4; i++) {
        value += amplitude * perlin_noise(p * frequency);
        amplitude *= 0.5;
        frequency *= 2.0;
    }
    return value;
}
```

---

## Resources

- **Perlin Noise Original**: Ken Perlin's algorithm (1983)
- **Voronoi/Worley**: Wikipedia entry on Worley noise
- **Hash Functions**: Various matrix-based hashing techniques

## Next Steps

1. Experiment with different scales on your narrative backgrounds
2. Combine multiple noise types for unique effects
3. Animate scale/offset parameters for dynamic effects
4. Layer multiple noise functions for fractal complexity

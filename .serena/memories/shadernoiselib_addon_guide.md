# ShaderNoiseLib Addon - Overview

## What It Is
Godot addon providing 3 procedural noise functions as custom Visual Shader nodes. GPU-accelerated, integrates into Visual Shader editor under "NoiseLib" category.

## Three Noise Functions

### 1. Perlin Noise 3D
- **Type**: Smooth gradient noise
- **Inputs**: uvw (Vector3), offset (Vector3), scale (Vector3, default 10,10,10)
- **Output**: float (0-1)
- **Characteristics**: Organic, smooth variation
- **Use**: Clouds, terrain, water, organic textures
- **Best for**: Natural-looking effects

### 2. Voronoi Noise 3D (Worley Noise)
- **Type**: Cellular/cracked patterns
- **Inputs**: uvw, offset, scale, distance_scale, distance_function (0-4), jitter (0-1), 3d (bool)
- **Outputs**: cell_value, distance, distance2 (all floats)
- **Characteristics**: Sharp cellular patterns
- **Use**: Dragon scales, rocks, tissue, honeycomb, cracks
- **Best for**: Organic geometric structures, multiple outputs for effects

### 3. Pixel Noise 3D
- **Type**: Pure random/blocky noise
- **Inputs**: uvw, offset, scale
- **Output**: float (0-1)
- **Characteristics**: Fast, random, no smoothing
- **Use**: Quick randomization, dithering, seeding other noise
- **Best for**: Performance-critical randomness

## Performance & Quality

| Type | Speed | Quality | Detail | GPU-Friendly |
|------|-------|---------|--------|--------------|
| Pixel | Fastest | Low | None | Yes |
| Voronoi | Fast | Medium | Cell-based | Yes |
| Perlin | Medium | High | Gradient | Yes |

## For Omega Spiral

### Integration with Star Backgrounds
- Layer Perlin (scale 5-10) for nebula clouds
- Add Voronoi (scale 50) for dust particles
- Use Pixel for quick randomization
- Combine with tween TIME for animation

### Narrative-Specific Uses
- **Calm scenes**: Perlin, scale 15-25, smooth gradients
- **Chaotic scenes**: Voronoi high jitter (0.8+), sharp transitions
- **Mystery**: All three combined, animated scale/offset

### Animation Tips
- Offset with TIME: `offset + TIME * 0.1` for smooth animation
- Scale with TIME: Create fractal detail changes
- Jitter adjustment: Increases randomness/chaos

## Key Parameters
- **Scale**: Higher = more detail, frequency controlled
- **Offset**: Shifts pattern without changing frequency
- **Jitter** (Voronoi): 0 = regular cells, 1 = chaotic random
- **Distance Function** (Voronoi): Different math for appearance

## Access
Visual Shader editor → Look for "NoiseLib" category → Add nodes

## Documentation
Full guide: `docs/code-guides/shadernoiselib_addon_guide.md`

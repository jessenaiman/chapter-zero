using Godot;
using System;

[Tool]
public partial class LemnsicateArt : Control
{
    // === Overall look ===

    [Export(PropertyHint.Range, "50,1000,1")]
    public int FilamentCount
    {
        get => _filamentCount;
        set { _filamentCount = Mathf.Clamp(value, 50, 5000); QueueRedraw(); }
    }

    [Export(PropertyHint.Range, "2,64,1")]
    public int InnerSteps
    {
        get => _innerSteps;
        set { _innerSteps = Mathf.Clamp(value, 2, 128); QueueRedraw(); }
    }

    [Export(PropertyHint.Range, "4,128,1")]
    public int OuterSteps
    {
        get => _outerSteps;
        set { _outerSteps = Mathf.Clamp(value, 4, 256); QueueRedraw(); }
    }

    [Export(PropertyHint.Range, "1,20,0.1")]
    public float StepLength
    {
        get => _stepLength;
        set { _stepLength = Mathf.Max(0.1f, value); QueueRedraw(); }
    }

    [Export(PropertyHint.Range, "0.5,8,0.1")]
    public float FilamentWidth
    {
        get => _filamentWidth;
        set { _filamentWidth = Mathf.Max(0.1f, value); QueueRedraw(); }
    }

    [Export(PropertyHint.Range, "0.2,3,0.01")]
    public float AlphaFalloff
    {
        get => _alphaFalloff;
        set { _alphaFalloff = Mathf.Max(0.01f, value); QueueRedraw(); }
    }

    // === Lemniscate placement ===

    // Fraction of min(width,height) used as lemniscate radius
    [Export(PropertyHint.Range, "0.1,0.6,0.01")]
    public float LemniscateScale
    {
        get => _lemniscateScale;
        set { _lemniscateScale = Mathf.Clamp(value, 0.1f, 0.6f); QueueRedraw(); }
    }

    // Vertical squish/stretch of the lemniscate shape
    [Export(PropertyHint.Range, "0.4,2,0.01")]
    public float LemniscateYScale
    {
        get => _lemniscateYScale;
        set { _lemniscateYScale = Mathf.Clamp(value, 0.4f, 2f); QueueRedraw(); }
    }

    // === Field behaviour (outer phase) ===

    [Export(PropertyHint.Range, "0,5,0.01")]
    public float RadialStrength
    {
        get => _radialStrength;
        set { _radialStrength = Mathf.Max(0f, value); QueueRedraw(); }
    }

    [Export(PropertyHint.Range, "0,5,0.01")]
    public float SwirlStrength
    {
        get => _swirlStrength;
        set { _swirlStrength = Mathf.Max(0f, value); QueueRedraw(); }
    }

    [Export(PropertyHint.Range, "0,5,0.01")]
    public float NoiseStrength
    {
        get => _noiseStrength;
        set { _noiseStrength = Mathf.Max(0f, value); QueueRedraw(); }
    }

    [Export(PropertyHint.Range, "0.001,0.1,0.001")]
    public float NoiseFrequency
    {
        get => _noiseFrequency;
        set { _noiseFrequency = Mathf.Max(0.0001f, value); QueueRedraw(); }
    }

    // How far filaments are considered "inner" vs "outer" (as fraction of rect)
    [Export(PropertyHint.Range, "0.1,1,0.01")]
    public float OuterRadiusFactor
    {
        get => _outerRadiusFactor;
        set { _outerRadiusFactor = Mathf.Clamp(value, 0.1f, 1f); QueueRedraw(); }
    }

    // === Color gradient (3-color loop: A → B → C → A) ===

    [Export] public Color ColorA
    {
        get => _colorA;
        set { _colorA = value; QueueRedraw(); }
    }

    [Export] public Color ColorB
    {
        get => _colorB;
        set { _colorB = value; QueueRedraw(); }
    }

    [Export] public Color ColorC
    {
        get => _colorC;
        set { _colorC = value; QueueRedraw(); }
    }

    // Randomness control
    [Export] public int Seed
    {
        get => _seed;
        set { _seed = value; QueueRedraw(); }
    }

    // === backing fields & noise ===

    private int _filamentCount = 450;
    private int _innerSteps = 24;
    private int _outerSteps = 48;
    private float _stepLength = 4f;
    private float _filamentWidth = 2.2f;
    private float _alphaFalloff = 1.2f;

    private float _lemniscateScale = 0.33f;
    private float _lemniscateYScale = 1.0f;

    private float _radialStrength = 1.8f;
    private float _swirlStrength = 1.4f;
    private float _noiseStrength = 1.7f;
    private float _noiseFrequency = 0.02f;
    private float _outerRadiusFactor = 0.55f;

    private Color _colorA = new Color(0.85f, 0.95f, 1.0f); // light blue/white
    private Color _colorB = new Color(1.0f, 0.9f, 0.4f);   // yellow
    private Color _colorC = new Color(1.0f, 0.3f, 0.15f);  // red

    private int _seed = 12345;

    private FastNoiseLite _noise;

    public override void _Ready()
    {
        if (_noise == null)
            _noise = new FastNoiseLite();

        ConfigureNoise();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationResized)
            QueueRedraw();
    }

    private void ConfigureNoise()
    {
        _noise.Seed = Seed;
        _noise.Frequency = NoiseFrequency;
        _noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
    }

    public override void _Draw()
    {
        ConfigureNoise();

        Rect2 rect = GetRect();
        Vector2 center = rect.GetCenter();
        float minDim = Mathf.Min(rect.Size.X, rect.Size.Y);
        float lemScale = minDim * LemniscateScale;
        float outerRadius = minDim * OuterRadiusFactor;

        var rng = new RandomNumberGenerator();
        rng.Seed = (ulong)Seed;

        // Precompute some step in parameter space for the inner phase
        float baseDt = Mathf.Tau / FilamentCount;

        for (int i = 0; i < FilamentCount; i++)
        {
            // Start parameter along lemniscate
            float t0 = (float)i / FilamentCount * Mathf.Tau;
            // Some filaments move forward, some backward along the lemniscate
            float dirSign = (i % 2 == 0) ? 1f : -1f;

            int totalSteps = InnerSteps + OuterSteps;
            if (totalSteps < 2)
                continue;

            Vector2[] points = new Vector2[totalSteps];
            Color[] colors = new Color[totalSteps];

            // Starting color based on position along lemniscate
            float gradParam = Mathf.PosMod(t0 / Mathf.Tau, 1f);
            Color baseColor = EvaluateTriGradient(gradParam, ColorA, ColorB, ColorC);

            // --- Phase 1: follow lemniscate exactly (smooth, gravitational center) ---

            float t = t0;
            for (int s = 0; s < InnerSteps; s++)
            {
                Vector2 local = LemniscatePos(t);
                // scale + aspect adjustment
                local.X *= lemScale;
                local.Y *= lemScale * LemniscateYScale;

                Vector2 pos = center + local;
                points[s] = pos;

                // Slight brightening near the focal center
                float innerU = (float)s / Mathf.Max(1, InnerSteps - 1);
                Color c = baseColor.Lerp(Colors.White, innerU * 0.35f);
                c.A = 1.0f;
                colors[s] = c;

                t += dirSign * baseDt * 0.6f; // how fast we travel around the lemniscate
            }

            // --- Phase 2: peel away into a vector field (outward + swirl + noise) ---

            Vector2 p = points[InnerSteps - 1];

            for (int s = 0; s < OuterSteps; s++)
            {
                int idx = InnerSteps + s;

                // Compute field direction at p
                Vector2 dir = SampleField(p, center, outerRadius);

                // Small randomness on step size for non-uniform detail
                float stepJitter = rng.RandfRange(0.7f, 1.3f);
                p += dir * StepLength * stepJitter;

                points[idx] = p;

                float u = (float)(s + 1) / OuterSteps; // 0..1 along outer segment
                // Fade alpha and push color slightly towards white as it escapes
                float alpha = Mathf.Pow(1f - u, AlphaFalloff);
                Color c2 = baseColor.Lerp(Colors.White, u * 0.4f);
                c2.A = alpha;
                colors[idx] = c2;
            }

            // Finally draw the filament
            DrawPolylineColors(points, colors, FilamentWidth, antialiased: true);
        }
    }

    // ===== Math bits =====

    // Simple lemniscate (infinity symbol) centered at origin, range ~[-1,1]
    private static Vector2 LemniscatePos(float t)
    {
        // x = cos(t), y = sin(t)*cos(t)  (two horizontal loops)
        float c = Mathf.Cos(t);
        float s = Mathf.Sin(t);
        float x = c;
        float y = s * c; // 0.5 * sin(2t) effectively
        return new Vector2(x, y);
    }

    // Vector field that controls outward motion + swirl + noise
    private Vector2 SampleField(Vector2 p, Vector2 center, float outerRadius)
    {
        Vector2 toCenter = p - center;
        float dist = toCenter.Length();
        Vector2 radialDir = dist > 0.0001f ? toCenter / dist : Vector2.Right;

        // Swirl around center (90° rotation of radial)
        Vector2 swirlDir = new Vector2(-radialDir.Y, radialDir.X);

        // Distance normalization 0..1
        float dNorm = outerRadius > 0.0001f ? Mathf.Clamp(dist / outerRadius, 0f, 1f) : 0f;

        // Swirl is stronger near center, weaker far away
        float swirlWeight = 1f - dNorm;

        // Noise gets stronger further away (breakdown at edges)
        float noiseWeight = Mathf.SmoothStep(0.2f, 1f, dNorm);

        float n = _noise.GetNoise2D(p.X, p.Y); // -1..1
        // Map noise to some direction on the unit circle
        float ang = n * Mathf.Pi * 2f;
        Vector2 noiseDir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));

        Vector2 v = radialDir * RadialStrength
                  + swirlDir * (SwirlStrength * swirlWeight)
                  + noiseDir * (NoiseStrength * noiseWeight);

        if (v.LengthSquared() < 1e-6f)
            v = radialDir; // fallback

        return v.Normalized();
    }

    private static Color EvaluateTriGradient(float t, Color a, Color b, Color c)
    {
        t = Mathf.PosMod(t, 1.0f);

        if (t < 1f / 3f)
        {
            float u = t * 3f;
            return a.Lerp(b, u);
        }
        else if (t < 2f / 3f)
        {
            float u = (t - 1f / 3f) * 3f;
            return b.Lerp(c, u);
        }
        else
        {
            float u = (t - 2f / 3f) * 3f;
            return c.Lerp(a, u);
        }
    }
}

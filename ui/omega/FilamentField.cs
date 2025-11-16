using Godot;
using System;
using System.Collections.Generic;

namespace OmegaSpiral.UI.Omega
{

#pragma warning disable CS8618

    /// <summary>
    /// A decorative field that emits filament-like lines around a rounded rect.
    /// </summary>

    [Tool]
    public partial class FilamentField : Control
    {
        // === Filament layout ===

        [Export(PropertyHint.Range, "50,2000,1")]
        public int FilamentCount
        {
            get => _filamentCount;
            set { _filamentCount = Mathf.Clamp(value, 50, 5000); QueueRedraw(); }
        }

        // How many steps we stay "on the edge" before peeling away
        [Export(PropertyHint.Range, "2,32,1")]
        public int EdgeSteps
        {
            get => _edgeSteps;
            set { _edgeSteps = Mathf.Clamp(value, 2, 64); QueueRedraw(); }
        }

        // How many steps we march outward into the field
        [Export(PropertyHint.Range, "4,96,1")]
        public int OuterSteps
        {
            get => _outerSteps;
            set { _outerSteps = Mathf.Clamp(value, 4, 128); QueueRedraw(); }
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

        [Export(PropertyHint.Range, "0.1,3,0.01")]
        public float AlphaFalloff
        {
            get => _alphaFalloff;
            set { _alphaFalloff = Mathf.Max(0.01f, value); QueueRedraw(); }
        }

        // Offset outward from the frame where filaments start (in pixels)
        [Export(PropertyHint.Range, "-20,40,0.1")]
        public float EdgeOffset
        {
            get => _edgeOffset;
            set { _edgeOffset = value; QueueRedraw(); }
        }

        // How many border samples to drive emission positions
        [Export(PropertyHint.Range, "64,1024,1")]
        public int BorderSamples
        {
            get => _borderSamples;
            set { _borderSamples = Mathf.Clamp(value, 64, 4096); QueueRedraw(); }
        }

        // === Field behaviour for outer region ===

        [Export(PropertyHint.Range, "0,5,0.01")]
        public float RadialStrength
        {
            get => _radialStrength;
            set { _radialStrength = Mathf.Max(0f, value); QueueRedraw(); }
        }

        [Export(PropertyHint.Range, "0,5,0.01")]
        public float TangentStrength
        {
            get => _tangentStrength;
            set { _tangentStrength = Mathf.Max(0f, value); QueueRedraw(); }
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

        // Outer radius used to modulate how “broken” the strands get
        [Export(PropertyHint.Range, "0.3,1,0.01")]
        public float OuterRadiusFactor
        {
            get => _outerRadiusFactor;
            set { _outerRadiusFactor = Mathf.Clamp(value, 0.3f, 1f); QueueRedraw(); }
        }

        // Sideways wobble amount (fraction of step length)
        [Export(PropertyHint.Range, "0,0.6,0.01")]
        public float WobbleAmount
        {
            get => _wobbleAmount;
            set { _wobbleAmount = Mathf.Clamp(value, 0f, 0.6f); QueueRedraw(); }
        }

        // === Border shape ===

        [Export(PropertyHint.Range, "0,256,0.1")]
        public float CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = Mathf.Max(0f, value); QueueRedraw(); }
        }

        // === Color gradient (3-color loop) ===

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

        [Export] public int Seed
        {
            get => _seed;
            set { _seed = value; QueueRedraw(); }
        }

        [Export(PropertyHint.Range, "0,64,0.1")]
        public float FramePadding
        {
            get => _framePadding;
            set { _framePadding = Mathf.Max(0f, value); QueueRedraw(); }
        }
        private float _framePadding = 12f;

        // === backing fields ===

        private int _filamentCount = 800;
        private int _edgeSteps = 8;
        private int _outerSteps = 32;
        private float _stepLength = 4f;
        private float _filamentWidth = 2.2f;
        private float _alphaFalloff = 1.4f;
        private float _edgeOffset = 4f;
        private int _borderSamples = 512;

        private float _radialStrength = 1.8f;
        private float _tangentStrength = 0.8f;
        private float _noiseStrength = 1.7f;
        private float _noiseFrequency = 0.02f;
        private float _outerRadiusFactor = 0.6f;
        private float _wobbleAmount = 0.25f;

        private float _cornerRadius = 40f;

        private Color _colorA = new Color(0.85f, 0.95f, 1.0f);
        private Color _colorB = new Color(1.0f, 0.9f, 0.4f);
        private Color _colorC = new Color(1.0f, 0.3f, 0.15f);

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
            // Slight inset so filaments start just outside/inside visible frame
            float inset = 6f;
            Rect2 inner = new Rect2(
                rect.Position.X + FramePadding,
                rect.Position.Y + FramePadding,
                rect.Size.X - 2f * FramePadding,
                rect.Size.Y - 2f * FramePadding
            );
            if (inner.Size.X <= 0 || inner.Size.Y <= 0)
                return;

            float radius = Mathf.Min(CornerRadius, Mathf.Min(inner.Size.X, inner.Size.Y) * 0.5f);

            // Build and resample border curve
            var rawPoints = new List<Vector2>();
            BuildRoundedRectPoints(inner, radius, 20, 40, rawPoints);
            if (rawPoints.Count < 4)
                return;

            Vector2[] borderPoints;
            float[] borderTValues;
            ResampleBorder(rawPoints, BorderSamples, out borderPoints, out borderTValues);

            Vector2 center = inner.GetCenter();
            float outerRadius = Mathf.Min(inner.Size.X, inner.Size.Y) * OuterRadiusFactor;

            var rng = new RandomNumberGenerator();
            rng.Seed = (ulong)Seed;

            // Generate filaments
            for (int i = 0; i < FilamentCount; i++)
            {
                int idx0 = rng.RandiRange(0, borderPoints.Length - 1);
                Vector2 start = borderPoints[idx0];

                // Approx tangent at border
                int idxPrev = (idx0 - 1 + borderPoints.Length) % borderPoints.Length;
                int idxNext = (idx0 + 1) % borderPoints.Length;
                Vector2 tangent = (borderPoints[idxNext] - borderPoints[idxPrev]).Normalized();
                if (tangent.LengthSquared() < 1e-6f)
                    tangent = Vector2.Right;

                // Outward normal from rect center
                Vector2 radial = (start - center).Normalized();
                if (radial.LengthSquared() < 1e-6f)
                    radial = new Vector2(-tangent.Y, tangent.X);

                // Decide if we run along +tangent or -tangent for the edge phase
                float dirSign = rng.Randf() < 0.5f ? 1f : -1f;

                int totalSteps = EdgeSteps + OuterSteps;
                if (totalSteps < 2)
                    continue;

                Vector2[] pts = new Vector2[totalSteps];
                Color[] cols = new Color[totalSteps];

                // Base color from perimeter parameter
                float gradT = borderTValues[idx0];
                Color baseColor = EvaluateTriGradient(gradT, ColorA, ColorB, ColorC);

                // ==== Phase 1: slide along the frame ====
                Vector2 p = start + radial * EdgeOffset;
                pts[0] = p;
                Color c0 = baseColor;
                c0.A = 1.0f;
                cols[0] = c0;

                int idx = idx0;

                for (int s = 1; s < EdgeSteps; s++)
                {
                    // Advance along border indices to stay "glued" to the edge
                    idx = (int)((idx + dirSign * 1 + borderPoints.Length) % borderPoints.Length);
                    Vector2 bp = borderPoints[idx];

                    // Recompute tangent/normal at this border point
                    int ip = (idx - 1 + borderPoints.Length) % borderPoints.Length;
                    int inx = (idx + 1) % borderPoints.Length;
                    Vector2 t = (borderPoints[inx] - borderPoints[ip]).Normalized();
                    if (t.LengthSquared() < 1e-6f)
                        t = tangent;

                    Vector2 n = (bp - center).Normalized();
                    if (n.LengthSquared() < 1e-6f)
                        n = new Vector2(-t.Y, t.X);

                    // Stay close to border with a little jitter
                    float offsetJitter = EdgeOffset + rng.RandfRange(-2f, 2f);
                    p = bp + n * offsetJitter;

                    pts[s] = p;

                    float u = (float)s / Mathf.Max(1, EdgeSteps - 1);
                    Color c = baseColor.Lerp(Colors.White, u * 0.25f);
                    c.A = 1.0f; // still strong while on edge
                    cols[s] = c;

                    tangent = t;
                    radial = n;
                }

                // ==== Phase 2: peel outward into field ====
                for (int s = 0; s < OuterSteps; s++)
                {
                    int k = EdgeSteps + s;

                    // Field-driven direction
                    Vector2 dir = SampleField(p, center, radial, tangent, outerRadius);

                    // Add sideways wobble that fades along the filament
                    float wobbleFactor = WobbleAmount * (1f - (float)s / Math.Max(1, OuterSteps - 1));
                    if (wobbleFactor > 0f)
                    {
                        float wobble = (rng.Randf() - 0.5f) * wobbleFactor * StepLength * 2f;
                        Vector2 side = new Vector2(-dir.Y, dir.X);
                        dir += side * (wobble / StepLength);
                    }

                    if (dir.LengthSquared() < 1e-6f)
                        dir = radial;

                    dir = dir.Normalized();
                    float stepJitter = rng.RandfRange(0.7f, 1.3f);
                    p += dir * StepLength * stepJitter;

                    pts[k] = p;

                    float u2 = (float)(s + 1) / OuterSteps; // 0..1 outward
                    float alpha = Mathf.Pow(1f - u2, AlphaFalloff);
                    Color c2 = baseColor.Lerp(Colors.White, u2 * 0.35f);
                    c2.A = alpha;
                    cols[k] = c2;
                }

                DrawPolylineColors(pts, cols, FilamentWidth, antialiased: true);
            }
        }

        // === Field: combination of radial, tangent, and noise, distance-aware ===

        private Vector2 SampleField(
            Vector2 p,
            Vector2 center,
            Vector2 radialHint,
            Vector2 tangentHint,
            float outerRadius)
        {
            Vector2 radial = (p - center);
            float dist = radial.Length();
            if (dist > 0.0001f)
                radial /= dist;
            else
                radial = radialHint;

            Vector2 tangent = new Vector2(-radial.Y, radial.X);
            if (tangent.LengthSquared() < 1e-6f)
                tangent = tangentHint;

            float dNorm = outerRadius > 0.0001f ? Mathf.Clamp(dist / outerRadius, 0f, 1f) : 0f;

            // Closer to frame: more tangent; farther: more radial + noise
            float tWeight = 1f - dNorm;
            float rWeight = dNorm;

            float n = _noise.GetNoise2D(p.X, p.Y); // -1..1
            float ang = n * Mathf.Pi * 2f;
            Vector2 noiseDir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));

            Vector2 v =
                radial * (RadialStrength * rWeight + 0.2f) +
                tangent * (TangentStrength * tWeight) +
                noiseDir * (NoiseStrength * Mathf.SmoothStep(0.2f, 1f, dNorm));

            if (v.LengthSquared() < 1e-6f)
                v = radial;

            return v.Normalized();
        }

        // === Geometry helpers ===

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

        private static void BuildRoundedRectPoints(
            Rect2 rect,
            float radius,
            int segmentsPerCorner,
            int segmentsPerSide,
            List<Vector2> outPoints)
        {
            outPoints.Clear();

            float left = rect.Position.X;
            float top = rect.Position.Y;
            float right = rect.Position.X + rect.Size.X;
            float bottom = rect.Position.Y + rect.Size.Y;

            float r = Mathf.Max(0.0f, radius);
            float maxR = Mathf.Min(rect.Size.X, rect.Size.Y) * 0.5f;
            r = Mathf.Min(r, maxR);

            if (r < 0.01f)
            {
                AddLine(new Vector2(left, top), new Vector2(right, top), segmentsPerSide, outPoints, false);
                AddLine(new Vector2(right, top), new Vector2(right, bottom), segmentsPerSide, outPoints, false);
                AddLine(new Vector2(right, bottom), new Vector2(left, bottom), segmentsPerSide, outPoints, false);
                AddLine(new Vector2(left, bottom), new Vector2(left, top), segmentsPerSide, outPoints, false);
                return;
            }

            Vector2 topLeft = new Vector2(left + r, top + r);
            Vector2 topRight = new Vector2(right - r, top + r);
            Vector2 bottomRight = new Vector2(right - r, bottom - r);
            Vector2 bottomLeft = new Vector2(left + r, bottom - r);

            AddLine(new Vector2(left + r, top), new Vector2(right - r, top), segmentsPerSide, outPoints, false);
            AddArc(topRight, r, -Mathf.Pi / 2f, 0f, segmentsPerCorner, outPoints, false);
            AddLine(new Vector2(right, top + r), new Vector2(right, bottom - r), segmentsPerSide, outPoints, false);
            AddArc(bottomRight, r, 0f, Mathf.Pi / 2f, segmentsPerCorner, outPoints, false);
            AddLine(new Vector2(right - r, bottom), new Vector2(left + r, bottom), segmentsPerSide, outPoints, false);
            AddArc(bottomLeft, r, Mathf.Pi / 2f, Mathf.Pi, segmentsPerCorner, outPoints, false);
            AddLine(new Vector2(left, bottom - r), new Vector2(left, top + r), segmentsPerSide, outPoints, false);
            AddArc(topLeft, r, Mathf.Pi, Mathf.Pi * 1.5f, segmentsPerCorner, outPoints, false);
        }

        private static void AddLine(Vector2 from, Vector2 to, int segments, List<Vector2> outPoints, bool includeLast)
        {
            if (segments <= 0)
            {
                outPoints.Add(from);
                if (includeLast)
                    outPoints.Add(to);
                return;
            }

            for (int i = 0; i <= segments; i++)
            {
                if (i == segments && !includeLast)
                    break;

                float t = (float)i / segments;
                Vector2 p = from.Lerp(to, t);
                outPoints.Add(p);
            }
        }

        private static void AddArc(
            Vector2 center,
            float radius,
            float angleFrom,
            float angleTo,
            int segments,
            List<Vector2> outPoints,
            bool includeLast)
        {
            if (segments <= 0)
            {
                Vector2 p = center + new Vector2(Mathf.Cos(angleFrom), Mathf.Sin(angleFrom)) * radius;
                outPoints.Add(p);
                return;
            }

            for (int i = 0; i <= segments; i++)
            {
                if (i == segments && !includeLast)
                    break;

                float t = (float)i / segments;
                float angle = Mathf.Lerp(angleFrom, angleTo, t);
                Vector2 p = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                outPoints.Add(p);
            }
        }

        private static void ResampleBorder(List<Vector2> points, int sampleCount, out Vector2[] samples, out float[] tValues)
        {
            int n = points.Count;
            float[] lengths = new float[n];
            float total = 0f;
            lengths[0] = 0f;

            for (int i = 1; i < n; i++)
            {
                total += points[i].DistanceTo(points[i - 1]);
                lengths[i] = total;
            }

            samples = new Vector2[sampleCount];
            tValues = new float[sampleCount];

            if (total <= 0f)
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    samples[i] = points[0];
                    tValues[i] = 0f;
                }
                return;
            }

            for (int i = 0; i < sampleCount; i++)
            {
                float target = total * (float)i / sampleCount;
                int j = 1;
                while (j < n && lengths[j] < target) j++;

                int j0 = Mathf.Clamp(j - 1, 0, n - 1);
                int j1 = Mathf.Clamp(j, 0, n - 1);

                float segLen = lengths[j1] - lengths[j0];
                float f = segLen > 0 ? (target - lengths[j0]) / segLen : 0f;

                samples[i] = points[j0].Lerp(points[j1], f);
                tValues[i] = target / total; // 0..1 around perimeter
            }
        }
    }
}

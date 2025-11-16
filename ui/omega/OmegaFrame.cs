using Godot;
using System.Collections.Generic;

namespace OmegaSpiral.UI.Omega
{
    [Tool]
    public partial class OmegaFrame : Control
    {
        // === Base gradient frame ===

        [Export]
        public Color ColorA
        {
            get => _colorA;
            set { _colorA = value; QueueRedraw(); }
        }

        [Export]
        public Color ColorB
        {
            get => _colorB;
            set { _colorB = value; QueueRedraw(); }
        }

        [Export]
        public Color ColorC
        {
            get => _colorC;
            set { _colorC = value; QueueRedraw(); }
        }

        [Export(PropertyHint.Range, "1,32,0.1")]
        public float BorderWidth
        {
            get => _borderWidth;
            set { _borderWidth = Mathf.Max(0.1f, value); QueueRedraw(); }
        }

        [Export(PropertyHint.Range, "0,64,0.1")]
        public float GlowWidth
        {
            get => _glowWidth;
            set { _glowWidth = Mathf.Max(0.0f, value); QueueRedraw(); }
        }

        [Export(PropertyHint.Range, "0,256,0.1")]
        public float CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = Mathf.Max(0.0f, value); QueueRedraw(); }
        }

        [Export(PropertyHint.Range, "4,64,1")]
        public int SegmentsPerCorner
        {
            get => _segmentsPerCorner;
            set { _segmentsPerCorner = Mathf.Clamp(value, 4, 128); QueueRedraw(); }
        }

        [Export(PropertyHint.Range, "2,64,1")]
        public int SegmentsPerSide
        {
            get => _segmentsPerSide;
            set { _segmentsPerSide = Mathf.Clamp(value, 2, 128); QueueRedraw(); }
        }

        // How far inside the Control’s rect the frame is drawn (visual padding, not layout)
        [Export(PropertyHint.Range, "0,64,0.1")]
        public float FramePadding
        {
            get => _framePadding;
            set { _framePadding = Mathf.Max(0f, value); QueueRedraw(); }
        }

        // === Private backing fields ===

        private Color _colorA = new Color(0.5f, 0.9f, 1.0f);
        private Color _colorB = new Color(1.0f, 0.9f, 0.4f);
        private Color _colorC = new Color(1.0f, 0.3f, 0.2f);

        private float _borderWidth = 6f;
        private float _glowWidth = 18f;
        private float _cornerRadius = 40f;
        private int _segmentsPerCorner = 20;
        private int _segmentsPerSide = 24;

        private float _framePadding = 12f;

        public override void _Notification(int what)
        {
            if (what == NotificationResized)
                QueueRedraw();
        }

        public override void _Draw()
        {
            // Draw inside our own rect; layout is handled by the scene / anchors.
            Rect2 rect = GetRect();

            Rect2 inner = new Rect2(
                rect.Position.X + FramePadding,
                rect.Position.Y + FramePadding,
                rect.Size.X - 2f * FramePadding,
                rect.Size.Y - 2f * FramePadding
            );

            if (inner.Size.X <= 0 || inner.Size.Y <= 0)
                return;

            float radius = Mathf.Min(
                CornerRadius,
                Mathf.Min(inner.Size.X, inner.Size.Y) * 0.5f
            );

            var points = new List<Vector2>();
            BuildRoundedRectPoints(inner, radius, SegmentsPerCorner, SegmentsPerSide, points);
            if (points.Count < 2)
                return;

            // Close the loop
            points.Add(points[0]);

            int count = points.Count;
            Vector2[] ptsArray = points.ToArray();

            var colors = new Color[count];
            var glowColors = new Color[count];

            for (int i = 0; i < count; i++)
            {
                float t = (count <= 1) ? 0f : (float)i / (count - 1);
                Color c = EvaluateTriGradient(t, ColorA, ColorB, ColorC);

                c.A = 1.0f;
                colors[i] = c;

                glowColors[i] = new Color(c.R, c.G, c.B, 0.35f);
            }

            // 1) Glow under everything
            if (GlowWidth > 0.01f)
                DrawPolylineColors(ptsArray, glowColors, GlowWidth, antialiased: true);

            // 2) Core stroke on top
            if (BorderWidth > 0.01f)
                DrawPolylineColors(ptsArray, colors, BorderWidth, antialiased: true);
        }

        // === Helpers ===

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

        private static void AddLine(
            Vector2 from,
            Vector2 to,
            int segments,
            List<Vector2> outPoints,
            bool includeLast)
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
    }
}

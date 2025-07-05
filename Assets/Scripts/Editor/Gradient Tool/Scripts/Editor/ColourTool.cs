using UnityEngine;

using System.Linq;
using System.Collections.Generic;

namespace Mirza.ColourTool
{
    [CreateAssetMenu(fileName = "Colour", menuName = "Colour")]
    public class ColourTool : ScriptableObject
    {
        //bool initialized;
        //public Color[] colours;

        public Color key = Color.red;

        [Header("HSV Offsets")]
        [Space]

        [Range(-1.0f, 1.0f)]
        public float hueOffset = 0.0f;
        [Range(-1.0f, 1.0f)]
        public float saturationOffset = 0.0f;
        [Range(-1.0f, 1.0f)]
        public float valueOffset = 0.0f;

        [Header("Post-Processing")]
        [Space]

        public Color tint = Color.white;
        public Gradient gradient;

        [Header("Colour Result")]
        [Space]

        public Color colour;

        [Header("Palette Results")]
        [Space]

        public GradientMode gradientMode = GradientMode.Blend;

        [Space]

        public Gradient textureGradient;
        public Gradient monochromaticGradient;
        public Gradient analogousGradient;
        public Gradient complementaryGradient;
        public Gradient splitComplementaryGradient;
        public Gradient triadicGradient;
        public Gradient tetradicGradient;
        public Gradient polygonalGradient;

        public enum ColourScheme
        {
            Monochromatic,
            Analogous,
            Complementary,
            SplitComplementary,
            Triadic,
            Tetradic,

            // Minimum 2 vertices (a line, which is same as complementary scheme).

            Polygonal,

            // Derived from texture.

            Texture,
        }

        // ...

        [Header(nameof(ColourScheme.Texture))]
        [Space]

        public Texture2D texture;

        [Space]

        [Range(1, 32)]
        public int texturePaletteCount = 5;

        [Range(1.0f / 32.0f, 1.0f)]
        public float texturePaletteResolutionScale = 0.25f;

        [Space]

        [Range(1, 64)]
        public int texturePaletteSteps = 32;

        [Space]

        [Range(0.0f, 1.0f)]
        public float texturePaletteStepsHueScale = 1.0f;
        [Range(0.0f, 1.0f)]
        public float texturePaletteStepsSaturationScale = 1.0f;
        [Range(0.0f, 1.0f)]
        public float texturePaletteStepsValueScale = 1.0f;

        [Space]

        // Pixels below this alpha are ignored when sampling texture for palette.

        [Range(0.0f, 1.0f)]
        public float texturePaletteAlphaCutoff = 0.5f;

        //[Space]

        //[Range(1, 32)]
        //public int texturePaletteKMeansIterations = 8;

        [Space]

        // Filter out colours similar in value in RGB and luminance space.

        // RGB filter a straightforward value difference filter.
        // Luminance filter is essentially a contrast filter that ensures a perceptually brightness-distinct palette.

        // The luminance filter thus increases the palette's dynamic range by enforcing a larger luminance distance between *all* palette colours.

        [Range(0.0f, 1.0f)]
        public float texturePaletteRGBDistanceCutoff = 0.1f;

        [Range(0.0f, 1.0f)]
        public float texturePaletteLuminanceDistanceCutoff = 0.1f;

        [Space]

        [Range(0.0f, 1.0f)]
        public float texturePaletteHSVDistanceCutoff = 0.0f;

        [Space]

        [Range(0.0f, 1.0f)]
        public float texturePaletteHueDistanceCutoffScale = 0.5f;
        [Range(0.0f, 1.0f)]
        public float texturePaletteSaturationDistanceCutoffScale = 0.5f;
        [Range(0.0f, 1.0f)]
        public float texturePaletteValueDistanceCutoffScale = 0.5f;

        // Reversed sorting is literally just reversed order (darkest first).

        public enum TexturePaletteSortMode
        {
            Default, // No sorting, returns palette in order of dominance.

            Hue,

            Value,
            ValueReversed,

            Luminance,
            LuminanceReversed,
        }

        [Space]

        public TexturePaletteSortMode texturePaletteSortMode = TexturePaletteSortMode.Default;

        [Space]

        public Color[] texturePalette;

        [Header(nameof(ColourScheme.Monochromatic))]
        [Space]

        [Range(3, 8)]
        public int monochromaticPaletteCount = 5;

        [Space]

        [Range(0.0f, 1.0f)]
        public float monochromaticSaturationScale = 0.2f;

        [Range(0.0f, 1.0f)]
        public float monochromaticPaletteValueScale = 1.0f;

        [Space]

        public Color[] monochromaticPalette;

        //public ColourScheme colourScheme = ColourScheme.Monochromatic;

        [Header(nameof(ColourScheme.Analogous))]
        [Space]

        [Range(3, 8)]
        public int analogousPaletteCount = 3;

        [Range(0.0f, 1.0f)]
        public float analogousPaletteScale = 0.1f;

        [Space]

        public Color[] analogousPalette;

        [Header(nameof(ColourScheme.Complementary))]
        [Space]

        [Range(-1.0f, 1.0f)]
        public float complementaryPaletteScale = 1.0f;

        public Color[] complementaryPalette;

        [Header(nameof(ColourScheme.SplitComplementary))]
        [Space]

        [Range(0.0f, 1.0f)]
        public float splitComplementaryPaletteScale = 1.0f;

        public Color[] splitComplementaryPalette;

        [Header(nameof(ColourScheme.Triadic))]
        [Space]

        public Color[] triadicPalette;

        [Header(nameof(ColourScheme.Tetradic))]
        [Space]

        public Color[] tetradicPalette;

        [Header(nameof(ColourScheme.Polygonal))]
        [Space]

        // 1 = single/solid (monochromatic),
        // 2 = line, 3 = triangle, 4 = square, 5 = pentagon, 6 = hexagon, etc.

        [Range(2, 32)]
        public int polygonalPaletteCount = 3;

        [Range(-1.0f, 1.0f)]
        public float polygonalPaletteScale = 1.0f;

        [Space]

        public Color[] polygonalPalette;

        // ...

        void OnEnable()
        {
            UpdatePalettes();
        }

        // ...

        public static Color[] SortByHue(Color[] input)
        {
            return input.OrderByDescending(c =>
            {
                Color.RGBToHSV(c, out float h, out _, out _);
                return h;

            }).ToArray();
        }
        public static Color[] SortByValue(Color[] input, bool reversed)
        {
            if (!reversed)
            {
                return input.OrderByDescending(c =>
                {
                    Color.RGBToHSV(c, out float _, out _, out float v);
                    return v;

                }).ToArray();
            }
            else
            {
                return input.OrderBy(c =>
                {
                    Color.RGBToHSV(c, out float _, out _, out float v);
                    return v;

                }).ToArray();
            }
        }
        public static Color[] SortByLuminance(Color[] input, bool reversed)
        {
            if (!reversed)
            {
                return input.OrderByDescending(c => (0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b)).ToArray();
            }
            else
            {
                return input.OrderBy(c => (0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b)).ToArray();
            }
        }

        // Colour palette from texture.
        // https://x.com/TheMirzaBeig/status/1941082247970295815

        public static Color[] GenerateTexture(

            int count, Texture2D texture, float alphaCutoff,

            float resolutionScale,

            int colourSteps, float colourStepsHueScale, float colourStepsSaturationScale, float colourStepsValueScale,

            float rgbDistanceCutoff, float luminanceDistanceCutoff,
            float hsvDistanceCutoff, float hueDistanceCutoffScale, float saturationDistanceCutoffScale, float valueDistanceCutoffScale)
        {
            Color[] colours = new Color[count];

            // No texture -> default all white.

            if (!texture)
            {
                for (int i = 0; i < count; i++)
                {
                    colours[i] = Color.white;
                }
            }

            // Texture provided -> sample/calculate/derive palette from texture.

            else
            {
                // Downsample texture for performance.

                int downsampledWidth = Mathf.RoundToInt(texture.width * resolutionScale);
                int downsampledHeight = Mathf.RoundToInt(texture.height * resolutionScale);

                downsampledWidth = Mathf.Max(downsampledWidth, 1);
                downsampledHeight = Mathf.Max(downsampledHeight, 1);

                Texture2D downsampledTexture = new(downsampledWidth, downsampledHeight, TextureFormat.RGBA32, false);

                RenderTexture rt = RenderTexture.GetTemporary(downsampledWidth, downsampledHeight, 0, RenderTextureFormat.ARGB32);
                Graphics.Blit(texture, rt);

                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = rt;

                downsampledTexture.ReadPixels(new Rect(0, 0, downsampledWidth, downsampledHeight), 0, 0);
                downsampledTexture.Apply();

                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(rt);

                Color[] pixels = downsampledTexture.GetPixels();

                Dictionary<Vector3Int, int> histogram = new();

                foreach (var pixel in pixels)
                {
                    // If alpha below cutoff, skip pixel.

                    if (pixel.a < alphaCutoff)
                    {
                        continue;
                    }

                    Color.RGBToHSV(pixel, out float h, out float s, out float v);

                    // Quantize HSV space to reduce the number of unique colors (rounding them to steps).
                    // > an HSV-separable posterization effect.

                    int hueSteps = Mathf.RoundToInt(colourSteps * colourStepsHueScale);
                    int saturationSteps = Mathf.RoundToInt(colourSteps * colourStepsSaturationScale);
                    int valueSteps = Mathf.RoundToInt(colourSteps * colourStepsValueScale);

                    h = Mathf.Round(h * hueSteps);
                    s = Mathf.Round(s * saturationSteps);
                    v = Mathf.Round(v * valueSteps);

                    // Use the quantized HSV values as keys in the histogram.
                    // This is like spatial partitioning of XYZ positions as a grid hash, but for HSV colour.

                    Vector3Int key = new(Mathf.RoundToInt(h), Mathf.RoundToInt(s), Mathf.RoundToInt(v));

                    if (histogram.ContainsKey(key))
                    {
                        histogram[key]++;
                    }
                    else
                    {
                        histogram[key] = 1;
                    }
                }

                // Sort by frequency, descending.

                var sorted = histogram.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

                List<Color> palette = new();

                foreach (var key in sorted)
                {
                    int hueSteps = Mathf.RoundToInt(colourSteps * colourStepsHueScale);
                    int saturationSteps = Mathf.RoundToInt(colourSteps * colourStepsSaturationScale);
                    int valueSteps = Mathf.RoundToInt(colourSteps * colourStepsValueScale);

                    Color colour = Color.HSVToRGB(key.x / (float)hueSteps, key.y / (float)saturationSteps, key.z / (float)valueSteps);

                    bool tooClose =

                        palette.Any(existing => RGBColourDistance(existing, colour) < rgbDistanceCutoff) ||
                        palette.Any(existing => LuminanceColourDistance(existing, colour) < luminanceDistanceCutoff) ||

                    palette.Any(existing =>
                    {
                        Vector3 hsvDistance = HSVColourDistance(existing, colour);

                        float hueThreshold = hsvDistanceCutoff * hueDistanceCutoffScale;
                        float saturationThreshold = hsvDistanceCutoff * saturationDistanceCutoffScale;
                        float valueThreshold = hsvDistanceCutoff * valueDistanceCutoffScale;

                        return

                            hsvDistance.x <= hueThreshold &&
                            hsvDistance.y <= saturationThreshold &&
                            hsvDistance.z <= valueThreshold;
                    });

                    if (!tooClose)
                    {
                        palette.Add(colour);

                        if (palette.Count >= count)
                        {
                            break;
                        }
                    }
                }

                // If we didn’t get enough, fill from leftovers.

                //if (palette.Count < count)
                //{
                //    foreach (var color in sorted)
                //    {
                //        if (!palette.Contains(color))
                //        {
                //            palette.Add(color);

                //            if (palette.Count >= count)
                //            {
                //                break;
                //            }
                //        }
                //    }
                //}

                return palette.ToArray();
            }

            // ...

            return colours;
        }

        // ...

        //static Color[] KMeansPalette(Color[] pixels, int count, int iterations, float alphaCutoff, float colourDistanceThreshold)
        //{
        //    //List<Color> centroids = pixels.Take(count).ToList();
        //    //List<Color> centroids = pixels.OrderBy(_ => Random.value).Take(count).ToList();

        //    int step = pixels.Length / count;

        //    List<Color> centroids = new();

        //    for (int i = 0; i < count; i++)
        //    {
        //        centroids.Add(pixels[i * step]);
        //    }

        //    List<List<Color>> clusters = new();

        //    for (int iter = 0; iter < iterations; iter++)
        //    {
        //        clusters.Clear();

        //        for (int i = 0; i < count; i++)
        //        {
        //            clusters.Add(new List<Color>());
        //        }

        //        foreach (var pixel in pixels)
        //        {
        //            if (pixel.a < alphaCutoff)
        //            {
        //                continue;
        //            }

        //            int bestIndex = 0;
        //            float bestDist = float.MaxValue;

        //            bool b = false;

        //            for (int i = 0; i < centroids.Count; i++)
        //            {
        //                float dist = HSVColourDistance(pixel, centroids[i]);

        //                if (dist > colourDistanceThreshold)
        //                {
        //                    b = true; break;
        //                }

        //                if (dist < bestDist)
        //                {
        //                    bestDist = dist;
        //                    bestIndex = i;
        //                }
        //            }

        //            if (!b)
        //            {
        //                continue;
        //            }

        //            clusters[bestIndex].Add(pixel);
        //        }

        //        for (int i = 0; i < centroids.Count; i++)
        //        {
        //            if (clusters[i].Count == 0) continue;

        //            Vector4 avg = Vector4.zero;

        //            foreach (var c in clusters[i])
        //            {
        //                avg += new Vector4(c.r, c.g, c.b, c.a);
        //            }

        //            avg /= clusters[i].Count;
        //            centroids[i] = new Color(avg.x, avg.y, avg.z, avg.w);
        //        }
        //    }

        //    //return centroids.ToArray();

        //    List<Color> uniqueCentroids = new();

        //    foreach (var c in centroids)
        //    {
        //        bool tooClose = uniqueCentroids.Any(existing => HSVColourDistance(existing, c) < colourDistanceThreshold);

        //        if (!tooClose)
        //        {
        //            uniqueCentroids.Add(c);
        //        }
        //    }

        //    return uniqueCentroids.Take(count).ToArray();
        //}

        static float RGBColourDistance(Color a, Color b)
        {
            return Vector3.Magnitude(new Vector3(a.r - b.r, a.g - b.g, a.b - b.b));
        }
        static float LuminanceColourDistance(Color a, Color b)
        {
            float la = (0.2126f * a.r) + (0.7152f * a.g) + (0.0722f * a.b);
            float lb = (0.2126f * b.r) + (0.7152f * b.g) + (0.0722f * b.b);

            return Mathf.Abs(la - lb);
        }
        static Vector3 HSVColourDistance(Color a, Color b)
        {
            Color.RGBToHSV(a, out float ha, out float sa, out float va);
            Color.RGBToHSV(b, out float hb, out float sb, out float vb);

            // Wrap hue difference around circle.

            float dh = Mathf.Abs(ha - hb);

            if (dh > 0.5f)
            {
                dh = 1.0f - dh;
            }

            //float dh = Mathf.Min(Mathf.Abs(ha - hb), 1.0f - Mathf.Abs(ha - hb));

            float ds = sa - sb;
            float dv = va - vb;

            //dh = Mathf.Abs(dh);
            ds = Mathf.Abs(ds);
            dv = Mathf.Abs(dv);

            return new Vector3(dh, ds, dv);
            //return (dh * dh) + (ds * ds) + (dv * dv);
        }

        // ...

        public static Color[] GenerateMonochromatic(int count, Color input, float saturationScale, float valueScale)
        {
            Color[] colours = new Color[count];
            Color.RGBToHSV(input, out float h, out float s, out float v);

            // ...

            for (int i = 0; i < count; i++)
            {
                float t = i / (count - 1.0f);

                float newS = Mathf.Lerp(s, s * saturationScale, t);
                float newV = Mathf.Lerp(v, v * valueScale, t);

                colours[i] = Color.HSVToRGB(h, newS, newV);
            }

            // ...

            return colours;
        }


        // Range is normalized distance. So 0.1f means 10% of the hue range.
        // At 1.0, it's just the full hue wheel.

        public static Color[] GenerateAnalogous(int count, Color input, float scale)
        {
            Color[] colours = new Color[count];
            Color.RGBToHSV(input, out float h, out float s, out float v);

            // ...

            for (int i = 0; i < count; i++)
            {
                float t = i / (count - 1.0f);
                t -= 0.5f; // Center range around 0.0f.

                colours[(count - 1) - i] = Color.HSVToRGB(Mathf.Repeat(h + (t * scale), 1.0f), s, v);
            }

            // ...

            return colours;
        }

        // ...

        public static Color[] GenerateComplementary(Color input, float scale)
        {
            Color[] colours = new Color[2];

            Color.RGBToHSV(input, out float h, out float s, out float v);

            colours[0] = input;
            colours[1] = Color.HSVToRGB(Mathf.Repeat(h + (0.5f * scale), 1.0f), s, v);

            return colours;
        }

        // ...

        public static Color[] GenerateSplitComplementary(Color input, float scale)
        {
            Color[] colours = new Color[3];

            Color.RGBToHSV(input, out float h, out float s, out float v);

            colours[0] = Color.HSVToRGB(Mathf.Repeat(h + (0.333f * scale), 1.0f), s, v);
            colours[1] = input;
            colours[2] = Color.HSVToRGB(Mathf.Repeat(h + (0.667f * scale), 1.0f), s, v);

            return colours;
        }

        // ...

        public static Color[] GenerateTriadic(Color input)
        {
            Color[] colours = new Color[3];

            Color.RGBToHSV(input, out float h, out float s, out float v);

            colours[0] = Color.HSVToRGB(Mathf.Repeat(h + 0.333f, 1.0f), s, v);
            colours[1] = input;
            colours[2] = Color.HSVToRGB(Mathf.Repeat(h + 0.667f, 1.0f), s, v);

            return colours;
        }

        // ...

        public static Color[] GenerateTetradic(Color input)
        {
            Color[] colours = new Color[4];

            Color.RGBToHSV(input, out float h, out float s, out float v);

            colours[0] = input;

            colours[1] = Color.HSVToRGB(Mathf.Repeat(h + 0.25f, 1.0f), s, v);
            colours[2] = Color.HSVToRGB(Mathf.Repeat(h + 0.5f, 1.0f), s, v);
            colours[3] = Color.HSVToRGB(Mathf.Repeat(h + 0.75f, 1.0f), s, v);

            return colours;
        }

        // ...

        public static Color[] GeneratePolygonal(int count, Color input, float scale)
        {
            Color[] colours = new Color[count];

            Color.RGBToHSV(input, out float h, out float s, out float v);

            // ...

            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                colours[i] = Color.HSVToRGB(Mathf.Repeat(h + (t * scale), 1.0f), s, v);
            }

            // ...

            return colours;
        }

        // ...

        public static void ApplyGradientToPalette(Color[] palette, Gradient gradient)
        {
            int count = palette.Length;

            // If palette only has one colour,
            // apply first key of gradient to that colour and return immediately.

            if (count < 2)
            {
                palette[0] *= gradient.Evaluate(0.0f); return;
            }

            for (int i = 0; i < count; i++)
            {
                palette[i] *= gradient.Evaluate((float)i / (count - 1));
            }
        }

        // ...

        GradientColorKey[] PaletteToGradientColourKeys(Color[] palette)
        {
            // Gradients limited to 8 keys.

            int count = Mathf.Min(palette.Length, 8);

            GradientColorKey[] colourKeys = new GradientColorKey[count];


            for (int i = 0; i < count; i++)
            {
                int indexOffset = 0;
                int countOffset = 0;

                if (gradientMode == GradientMode.Fixed)
                {
                    indexOffset++;
                    countOffset++;
                }

                float t = (i + indexOffset) / ((count - 1.0f) + countOffset);

                colourKeys[i] = new GradientColorKey(palette[i], t);
            }

            return colourKeys;
        }

        // ...

        void UpdatePalettes()
        {
            // Calculate actual input colour.

            colour = key;

            // Apply hue, saturation, and value offsets.

            Color.RGBToHSV(colour, out float h, out float s, out float v);

            h = Mathf.Repeat(h + hueOffset, 1.0f);
            s = Mathf.Clamp01(s + saturationOffset);
            v = Mathf.Clamp01(v + valueOffset);

            colour = Color.HSVToRGB(h, s, v);

            // Apply tint.

            colour *= tint;

            // Get palettes.

            texturePalette = GenerateTexture(

                texturePaletteCount, texture, texturePaletteAlphaCutoff,

                texturePaletteResolutionScale,

                texturePaletteSteps, texturePaletteStepsHueScale, texturePaletteStepsSaturationScale, texturePaletteStepsValueScale,
                texturePaletteRGBDistanceCutoff, texturePaletteLuminanceDistanceCutoff,
                texturePaletteHSVDistanceCutoff, texturePaletteHueDistanceCutoffScale, texturePaletteSaturationDistanceCutoffScale, texturePaletteValueDistanceCutoffScale);

            switch (texturePaletteSortMode)
            {
                case TexturePaletteSortMode.Hue:
                    {
                        texturePalette = SortByHue(texturePalette);
                        break;
                    }
                case TexturePaletteSortMode.Value:
                    {
                        texturePalette = SortByValue(texturePalette, false);
                        break;
                    }
                case TexturePaletteSortMode.ValueReversed:
                    {
                        texturePalette = SortByValue(texturePalette, true);
                        break;
                    }
                case TexturePaletteSortMode.Luminance:
                    {
                        texturePalette = SortByLuminance(texturePalette, false);
                        break;
                    }
                case TexturePaletteSortMode.LuminanceReversed:
                    {
                        texturePalette = SortByLuminance(texturePalette, true);
                        break;
                    }
                case TexturePaletteSortMode.Default:
                    {
                        // No sorting, return palette in order of dominance.

                        break;
                    }
                default:
                    {
                        throw new System.NotImplementedException($"TexturePaletteSortMode '{texturePaletteSortMode}' is not implemented.");
                    }
            }

            monochromaticPalette = GenerateMonochromatic(monochromaticPaletteCount, colour, monochromaticSaturationScale, monochromaticPaletteValueScale);
            analogousPalette = GenerateAnalogous(analogousPaletteCount, colour, analogousPaletteScale);
            complementaryPalette = GenerateComplementary(colour, complementaryPaletteScale);
            splitComplementaryPalette = GenerateSplitComplementary(colour, splitComplementaryPaletteScale);
            triadicPalette = GenerateTriadic(colour);
            tetradicPalette = GenerateTetradic(colour);
            polygonalPalette = GeneratePolygonal(polygonalPaletteCount, colour, polygonalPaletteScale);

            // Apply gradient to palettes.

            ApplyGradientToPalette(texturePalette, gradient);
            ApplyGradientToPalette(monochromaticPalette, gradient);
            ApplyGradientToPalette(analogousPalette, gradient);
            ApplyGradientToPalette(complementaryPalette, gradient);
            ApplyGradientToPalette(splitComplementaryPalette, gradient);
            ApplyGradientToPalette(triadicPalette, gradient);
            ApplyGradientToPalette(tetradicPalette, gradient);
            ApplyGradientToPalette(polygonalPalette, gradient);

            // Generate gradients from palettes.

            textureGradient.colorKeys = PaletteToGradientColourKeys(texturePalette);
            monochromaticGradient.colorKeys = PaletteToGradientColourKeys(monochromaticPalette);
            analogousGradient.colorKeys = PaletteToGradientColourKeys(analogousPalette);
            complementaryGradient.colorKeys = PaletteToGradientColourKeys(complementaryPalette);
            splitComplementaryGradient.colorKeys = PaletteToGradientColourKeys(splitComplementaryPalette);
            triadicGradient.colorKeys = PaletteToGradientColourKeys(triadicPalette);
            tetradicGradient.colorKeys = PaletteToGradientColourKeys(tetradicPalette);
            polygonalGradient.colorKeys = PaletteToGradientColourKeys(polygonalPalette);

            // Set gradient mode.

            textureGradient.mode = gradientMode;
            monochromaticGradient.mode = gradientMode;
            analogousGradient.mode = gradientMode;
            complementaryGradient.mode = gradientMode;
            splitComplementaryGradient.mode = gradientMode;
            triadicGradient.mode = gradientMode;
            tetradicGradient.mode = gradientMode;
            polygonalGradient.mode = gradientMode;
        }

        void OnValidate()
        {
            UpdatePalettes();
        }
    }
}
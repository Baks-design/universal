using UnityEditor;
using UnityEngine;
using System.IO;
using Unity.Mathematics;

namespace Mirza.GradientTool
{
    [CreateAssetMenu(fileName = "Tools/Gradient", menuName = "Gradient")]
    public class GradientTool : ScriptableObject
    {
        #region Enums
        public enum PowerOfTwoResolution
        {
            _2 = 2,
            _3 = 3,
            _4 = 4,
            _5 = 5,
            _7 = 7,
            _8 = 8,
            _16 = 16,
            _32 = 32,
            _64 = 64,
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024
        }
        #endregion

        #region Static Fields
        public static bool showLabSection;
        public static bool showPostProcessingSection;

        [RuntimeInitializeOnLoadMethod]
        static void SectionsResets()
        {
            showLabSection = false;
            showPostProcessingSection = false;
        }
        #endregion

        #region Serialized Fields
        [Header("Settings")]
        public Gradient gradient;
        [HideInInspector] public Gradient gradientHSV;
        public PowerOfTwoResolution resolution = PowerOfTwoResolution._256;
        [Tooltip("Keep this disabled when not actively using/previewing: automatically generates texture when properties change. When disabled, you must manually click the 'Generate/Update Texture' button.")]
        public bool autoSave;
        // Colour Lab Section
        public int CUSTOM_HEADER_COLOUR_LAB;
        [Range(-180f, 180f)] public float hueOffset;
        [Range(-1f, 1f)] public float saturationOffset;
        [Range(-1f, 1f)] public float saturationOffsetUnclamped;
        [Range(-1f, 1f)] public float valueOffset;
        [Range(-1f, 1f)] public float valueOffsetUnclamped;
        [Range(0f, 2f)] public float saturationScale = 1f;
        [ColorUsage(false, false)] public Color multiplyColour = Color.white;
        // Post-Processing Section
        public int CUSTOM_HEADER_POST_PROCESSING;
        public Gradient multiplyGradient = new();
        public bool smoothstep;
        [Range(0f, 1f)] public float smoothstepMin = 0f;
        [Range(0f, 1f)] public float smoothstepMax = 1f;
        public float power = 1f;
        #endregion

        #region Fields
        Texture2D texture;
        Color[] _colours;
        #endregion

        #region Properties
        public int Width => (int)resolution;
        public const int height = 1;
        public Color32[] Colours => GetColours32();
        #endregion

        #region Unity Callbacks
        void OnEnable()
        {
            autoSave = false;

            if (texture)
            {
                var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(this));
                if (!GetTextureFromSubAssets(subAssets))
                    AddTextureAsSubAsset();
            }
        }

        void OnValidate()
        {
            if (!autoSave || AssetDatabase.IsAssetImportWorkerProcess()) return;
            UpdateTexture();
        }
        #endregion

        #region Public Methods
        public Texture2D GetTexture()
        {
            var textureCreated = EnsureTextureExists();
            if (textureCreated) UpdateTexture();
            return texture;
        }
        
        Color32[] GetColours32()
        {
            var colors = GetColours();
            var colors32 = new Color32[colors.Length];

            for (var i = 0; i < colors.Length; i++)
                colors32[i] = colors[i];

            return colors32;
        }

        Color[] GetColours()
        {
            if (_colours == null || _colours.Length != Width * height)
            {
                if (_colours != null)
                    Debug.LogWarning("Re-initializing colours array due to size change.");
                _colours = new Color[Width * height];
            }
            return _colours;
        }

        public void SetTexture(Texture2D newTexture) => texture = newTexture;

        public void UpdateColourBuffer()
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var t = x / (Width - 1f);

                    ref var colour = ref Colours[x + (y * Width)];

                    if (smoothstep)
                        t = math.smoothstep(smoothstepMin, smoothstepMax, t);

                    t = Mathf.Pow(t, power);

                    colour = gradient.Evaluate(t);
                    colour *= multiplyGradient.Evaluate(t);
                }
            }
        }

        public void UpdateTexture()
        {
            EnsureTextureExists();
            UpdateColourBuffer();

            if (GetTexture().width != Width || GetTexture().height != height)
                GetTexture().Reinitialize(Width, height);

            GetTexture().SetPixels32(Colours);
            GetTexture().Apply();
            AssetDatabase.SaveAssets();
        }

        public void ExportTexture()
        {
            EnsureTextureExists();

            var assetPath = AssetDatabase.GetAssetPath(this);
            var directory = Path.GetDirectoryName(assetPath);
            var filePath = Path.Combine(directory, name + ".png");

            var data = GetTexture().EncodeToPNG();
            File.WriteAllBytes(filePath, data);

            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
            var importer = (TextureImporter)AssetImporter.GetAtPath(filePath);

            importer.wrapMode = texture.wrapMode;
            importer.filterMode = texture.filterMode;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            AssetDatabase.Refresh();
            Debug.Log("Texture exported to: " + filePath);
        }

        public void DeleteTextureSubAsset()
        {
            if (texture)
            {
                AssetDatabase.RemoveObjectFromAsset(texture);
                DestroyImmediate(texture, true);
                texture = null;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(this);
            }
            else
                Debug.LogWarning("No texture to delete.");
        }

        public void ResetLab()
        {
            hueOffset = 0f;
            saturationOffset = 0f;
            saturationOffsetUnclamped = 0f;
            valueOffset = 0f;
            valueOffsetUnclamped = 0f;
            saturationScale = 1f;
            multiplyColour = Color.white;
        }

        public void ResetPostProcessing()
        {
            multiplyGradient = new Gradient();
            smoothstep = false;
            smoothstepMin = 0f;
            smoothstepMax = 1f;
            power = 1f;
        }
        #endregion

        #region Methods
        bool EnsureTextureExists()
        {
            if (!texture)
            {
                var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(this));
                texture = GetTextureFromSubAssets(subAssets);
            }
            if (!texture)
            {
                CreateTextureSubAsset();
                AddTextureAsSubAsset();
                return true;
            }
            return false;
        }

        Texture2D GetTextureFromSubAssets(Object[] objects)
        {
            for (var i = 0; i < objects.Length; i++)
                if (objects[i] is Texture2D tex)
                    return tex;
            return null;
        }

        void AddTextureAsSubAsset()
        {
            if (!EditorUtility.IsPersistent(this)) return;
            AssetDatabase.AddObjectToAsset(texture, this);
            AssetDatabase.SaveAssets();
        }

        void CreateTextureSubAsset()
        => texture = new Texture2D(Width, height, TextureFormat.RGBA32, false)
        {
            name = "Texture",
            wrapMode = TextureWrapMode.Clamp
        };
        #endregion
    }
}
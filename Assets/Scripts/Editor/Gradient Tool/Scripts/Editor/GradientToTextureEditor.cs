using UnityEditor;
using UnityEngine;

namespace Mirza.GradientTool
{
    [CustomEditor(typeof(GradientTool))]
    public class GradientToTextureEditor : Editor
    {
        #region Fields
        Texture2D labPreviewTexture;
        Texture2D postProcessingPreviewTexture;
        #endregion

        #region Unity Callbacks
        void OnEnable() => EditorApplication.update += OnEditorUpdate;
        void OnDestroy() => EditorApplication.update -= OnEditorUpdate;

        void OnEditorUpdate()
        {
            var generator = (GradientTool)target;
            if (Selection.activeObject != target)
            {
                generator.autoSave = false;
                Debug.Log($"User navigated away from the inspector of {target.name}.");
                EditorApplication.update -= OnEditorUpdate;
            }
        }
        #endregion

        #region Inspector GUI
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var generator = (GradientTool)target;

            InitializeGradients(generator);
            TryFindTexture(generator);

            DrawSettingsSection(generator);
            DrawColourLabSection(generator);
            DrawPostProcessingSection(generator);
            DrawPreviewSections(generator);
            DrawTextureActions(generator);

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region Methods
        void InitializeGradients(GradientTool generator)
        {
            generator.gradient ??= new Gradient();
            generator.gradientHSV ??= new Gradient();
        }

        void TryFindTexture(GradientTool generator)
        {
            if (!generator.GetTexture())
            {
                var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(generator));
                for (var i = 0; i < subAssets.Length; i++)
                {
                    if (subAssets[i] is Texture2D foundTexture)
                    {
                        generator.SetTexture(foundTexture);
                        break;
                    }
                }
            }
        }

        void DrawSettingsSection(GradientTool generator)
        {
            var property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name == nameof(generator.CUSTOM_HEADER_COLOUR_LAB)) break;
                    EditorGUILayout.PropertyField(property, true);
                }
                while (property.NextVisible(false));
            }
        }

        void DrawColourLabSection(GradientTool generator)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            var labChanged = CheckLabChanges(generator);
            GradientTool.showLabSection = EditorGUILayout.Foldout(
                GradientTool.showLabSection, "Colour Lab", true,
                GetFoldoutStyle(!labChanged)
            );

            if (GradientTool.showLabSection)
                DrawLabProperties(generator, labChanged);
        }

        bool CheckLabChanges(GradientTool generator)
        => generator.hueOffset != 0f ||
            generator.saturationOffset != 0f ||
            generator.saturationOffsetUnclamped != 0f ||
            generator.valueOffset != 0f ||
            generator.valueOffsetUnclamped != 0f ||
            generator.saturationScale != 1f ||
            generator.multiplyColour != Color.white;

        void DrawLabProperties(GradientTool generator, bool labChanged)
        {
            EditorGUILayout.Space();
            GUI.enabled = labChanged;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Lab", GUILayout.MaxWidth(160)))
            {
                Undo.RecordObject(generator, "Reset Lab");
                generator.ResetLab();
                EditorUtility.SetDirty(generator);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GenerateLabPreview(generator);

            var labPreviewRect = GUILayoutUtility.GetRect(
                EditorGUIUtility.currentViewWidth - 40, 32,
                GUILayout.ExpandWidth(true));

            EditorGUI.DrawPreviewTexture(labPreviewRect, labPreviewTexture);
            GUI.enabled = true;
            EditorGUI.DropShadowLabel(labPreviewRect, $"Lab Preview", GetPreviewLabelStyle());
            GUI.enabled = labChanged;

            EditorGUILayout.Space();
            if (GUILayout.Button("Apply Lab to Gradient"))
            {
                Undo.RecordObject(generator, "Apply Lab to Gradient");
                generator.gradient.colorKeys = generator.gradientHSV.colorKeys;
                EditorUtility.SetDirty(generator);

                if (generator.autoSave)
                    generator.UpdateTexture();
            }

            GUI.enabled = true;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        void DrawPostProcessingSection(GradientTool generator)
        {
            var postProcessingChanged = !IsDefaultGradient(generator.multiplyGradient);
            GradientTool.showPostProcessingSection = EditorGUILayout.Foldout(
                GradientTool.showPostProcessingSection, "Post-Processing", true,
                GetFoldoutStyle(!postProcessingChanged)
            );

            if (GradientTool.showPostProcessingSection)
                DrawPostProcessingProperties(generator, postProcessingChanged);
        }

        void DrawPostProcessingProperties(GradientTool generator, bool postProcessingChanged)
        {
            var property = serializedObject.FindProperty(nameof(generator.CUSTOM_HEADER_POST_PROCESSING));
            if (property.NextVisible(true))
            {
                do EditorGUILayout.PropertyField(property, true);
                while (property.NextVisible(false));
            }

            EditorGUILayout.Space();
            GUI.enabled = postProcessingChanged;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Post-Processing", GUILayout.MaxWidth(160)))
            {
                Undo.RecordObject(generator, "Reset Post-Processing");
                generator.ResetPostProcessing();
                EditorUtility.SetDirty(generator);
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        void DrawPreviewSections(GradientTool generator)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Texture Preview", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            GeneratePostProcessingPreview(generator);
            Rect postProcessingPreviewRect = GUILayoutUtility.GetRect(
                EditorGUIUtility.currentViewWidth - 40, 32,
                GUILayout.ExpandWidth(true));

            EditorGUI.DrawPreviewTexture(postProcessingPreviewRect, postProcessingPreviewTexture);
            EditorGUI.DropShadowLabel(postProcessingPreviewRect,
                $"Render Preview: {generator.Width} x {GradientTool.height}",
                GetPreviewLabelStyle());

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Texture Asset/File", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            Rect texturePreviewRect = GUILayoutUtility.GetRect(
                EditorGUIUtility.currentViewWidth - 40, 32,
                GUILayout.ExpandWidth(true));

            EditorGUI.DrawPreviewTexture(texturePreviewRect, generator.GetTexture());
            EditorGUI.DropShadowLabel(texturePreviewRect,
                $"Current Texture: {generator.GetTexture().width} x {GradientTool.height}",
                GetPreviewLabelStyle());

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        void DrawTextureActions(GradientTool generator)
        {
            if (GUILayout.Button("Generate/Update Texture"))
                generator.UpdateTexture();
            if (GUILayout.Button("Export Texture"))
            {
                generator.UpdateTexture();
                generator.ExportTexture();
            }
        }

        GUIStyle GetFoldoutStyle(bool isDefault)
        {
            if (isDefault) return EditorStyles.foldoutHeader;

            var highlightStyle = new GUIStyle(EditorStyles.foldoutHeader)
            {
                normal = { textColor = Color.yellow },
                onNormal = { textColor = Color.yellow },
                active = { textColor = Color.yellow },
                onActive = { textColor = Color.yellow },
                hover = { textColor = Color.yellow },
                onHover = { textColor = Color.yellow },
                focused = { textColor = Color.yellow },
                onFocused = { textColor = Color.yellow }
            };
            return highlightStyle;
        }

        GUIStyle GetPreviewLabelStyle() => new(EditorStyles.label)
        {
            normal = { textColor = Color.white },
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12
        };

        bool IsDefaultGradient(Gradient gradient)
        {
            if (gradient == null) return true;

            var alphaKeys = gradient.alphaKeys;
            var colourKeys = gradient.colorKeys;

            return alphaKeys.Length == 2 &&
                   alphaKeys[0].time == 0f && alphaKeys[0].alpha == 1f &&
                   alphaKeys[1].time == 1f && alphaKeys[1].alpha == 1f &&
                   colourKeys.Length == 2 &&
                   colourKeys[0].time == 0f && colourKeys[0].color == Color.white &&
                   colourKeys[1].time == 1f && colourKeys[1].color == Color.white;
        }

        void GenerateLabPreview(GradientTool generator)
        {
            if (!labPreviewTexture || labPreviewTexture.width != generator.Width)
            {
                if (labPreviewTexture) DestroyImmediate(labPreviewTexture, true);

                labPreviewTexture = new Texture2D(
                    generator.Width, GradientTool.height, TextureFormat.RGBA32, false)
                {
                    wrapMode = TextureWrapMode.Clamp,
                    hideFlags = HideFlags.HideAndDontSave,
                };
            }

            var originalKeys = generator.gradient.colorKeys;
            var labKeys = new GradientColorKey[originalKeys.Length];

            for (var i = 0; i < originalKeys.Length; i++)
            {
                var colour = originalKeys[i].color;
                Color.RGBToHSV(colour, out var h, out var s, out var v);

                h = Mathf.Repeat(h + (generator.hueOffset / 360f), 1f);
                s += generator.saturationOffset;
                v += generator.valueOffset;

                s = Mathf.Clamp01(s);
                v = Mathf.Clamp01(v);

                s += generator.saturationOffsetUnclamped;
                v += generator.valueOffsetUnclamped;

                colour = Color.HSVToRGB(h, s, v, false);
                colour.a = originalKeys[i].color.a;

                var rgb = new Vector3(colour.r, colour.g, colour.b);
                var intensity = Vector3.Dot(rgb, new Vector3(0.2126f, 0.7152f, 0722f));
                rgb = Vector3.Lerp(new Vector3(intensity, intensity, intensity), rgb, generator.saturationScale);

                colour = new Color(rgb.x, rgb.y, rgb.z, colour.a);
                colour *= generator.multiplyColour;

                labKeys[i] = new GradientColorKey(colour, originalKeys[i].time);
            }

            generator.gradientHSV.colorKeys = labKeys;
            var colours = generator.Colours;

            for (var y = 0; y < GradientTool.height; y++)
            {
                for (var x = 0; x < generator.Width; x++)
                {
                    var t = x / (generator.Width - 1f);
                    ref var colour = ref colours[x + (y * generator.Width)];
                    colour = generator.gradientHSV.Evaluate(t);
                }
            }

            labPreviewTexture.SetPixels32(colours);
            labPreviewTexture.Apply();
        }

        void GeneratePostProcessingPreview(GradientTool generator)
        {
            if (!postProcessingPreviewTexture || postProcessingPreviewTexture.width != generator.Width)
            {
                if (postProcessingPreviewTexture)
                    DestroyImmediate(postProcessingPreviewTexture, true);

                postProcessingPreviewTexture = new Texture2D(
                    generator.Width, GradientTool.height, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp,
                    hideFlags = HideFlags.HideAndDontSave,
                };
            }

            generator.UpdateColourBuffer();
            postProcessingPreviewTexture.SetPixels32(generator.Colours);
            postProcessingPreviewTexture.Apply();
        }
        #endregion
    }
}
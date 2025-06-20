using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

namespace Universal.Runtime.Systems.ScenesManagement
{
    public class SceneLoader : MonoBehaviour //TODO: UI - Make Load Screen
    {
        [SerializeField, Child] CinemachineCamera loadingCamera; //TODO: Adjust Scene Loader
        [SerializeField, Child] UIDocument document;
        [SerializeField] SceneGroup[] sceneGroups;
        VisualElement root;
        ProgressBar loadingBar;
        VisualElement loadingCanvas;
        const float fillSpeed = 0.5f;
        float targetProgress;
        bool isLoading;
        public readonly SceneGroupManager manager = new();

        void Awake()
        {
            root = document.rootVisualElement;
            loadingBar = root.Q<ProgressBar>("loading-bar");
            loadingCanvas = root.Q<VisualElement>("loading-canvas");

            // Hide by default
            EnableLoadingCanvas(false);
        }

        async void Start() => await LoadSceneGroup(0);

        void Update()
        {
            if (!isLoading) return;

            var currentFillAmount = loadingBar.value / 100f; // ProgressBar uses 0-100 range
            var progressDifference = Mathf.Abs(currentFillAmount - targetProgress);
            var dynamicFillSpeed = progressDifference * fillSpeed;
            var newFillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);

            loadingBar.value = newFillAmount * 100f;
        }

        public async Awaitable LoadSceneGroup(int index)
        {
            loadingBar.value = 0f;
            targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
                return;

            var progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);

            EnableLoadingCanvas(true);
            await manager.LoadScenes(sceneGroups[index], progress);
            EnableLoadingCanvas(false);
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.style.display = enable ? DisplayStyle.Flex : DisplayStyle.None;
            loadingCamera.gameObject.SetActive(enable);
        }
    }
}
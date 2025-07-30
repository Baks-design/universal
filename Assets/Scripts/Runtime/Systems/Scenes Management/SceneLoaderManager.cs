using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
using static Freya.Mathfs;

namespace Universal.Runtime.Systems.ScenesManagement
{
    public class SceneLoaderManager : MonoBehaviour
    {
        [SerializeField, Child] CinemachineCamera loadingCamera; 
        //[SerializeField, Child] UIDocument document;
        [SerializeField] SceneGroup[] sceneGroups;
        // VisualElement root;
        // ProgressBar loadingBar;
        // VisualElement loadingCanvas;
        const float fillSpeed = 0.5f;
        float targetProgress;
        bool isLoading;
        public readonly SceneGroupManager manager = new();

        void Awake()
        {            
            // root = document.rootVisualElement;
            // loadingBar = root.Q<ProgressBar>("loading-bar");
            // loadingCanvas = root.Q<VisualElement>("loading-canvas");

            // Hide by default
            //EnableLoadingCanvas(false);
        }

        async void Start() => await LoadSceneGroup(0);

        //void Update() => LoadBar();

        // void LoadBar()
        // {
        //     if (!isLoading) return;
        //     var currentFillAmount = loadingBar.value / 100f; 
        //     var progressDifference = Abs(currentFillAmount - targetProgress);
        //     var dynamicFillSpeed = progressDifference * fillSpeed;
        //     var newFillAmount = Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
        //     loadingBar.value = newFillAmount * 100f;
        // }

        public async Awaitable LoadSceneGroup(int index)
        {
            //loadingBar.value = 0f;
            targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length) return;

            var progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Max(target, targetProgress);

            EnableLoadingCanvas(true);
            await manager.LoadScenes(sceneGroups[index], progress);
            EnableLoadingCanvas(false);
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            //loadingCanvas.style.display = enable ? DisplayStyle.Flex : DisplayStyle.None;
            loadingCamera.gameObject.SetActive(enable);
        }
    }
}
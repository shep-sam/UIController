using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceCore.UIController
{
    public sealed class UIController : MonoBehaviour
    {
        public static UIController Instance;

        [SerializeField]
        private UIWindow[] screenPrefabs;

        private List<UIWindow> openeedWindows = new List<UIWindow>(); //TODO: change to Stack

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);

            Subscribe();
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void UnSubscribe()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if (openeedWindows.Count > 0)
            {
                foreach (var window in openeedWindows)
                {
                    window.Close();
                }
            }
        }

        //Use in Inspector 
        public void GoToScreen(string windowName)
        {
            WindowTag windowTag = WindowTag.Undefined;

            try
            {
                windowTag = (WindowTag)Enum.Parse(typeof(WindowTag), windowName);
            }
            catch
            {
                Debug.LogError($"UIController | There are no screen for tag: {windowName}");
            }

            if (windowTag != WindowTag.Undefined)
                GoToScreen(windowTag);
        }

        //Use in Code
        public UIWindow GoToScreen(WindowTag windowTag)
        {
            return GoToScreen(windowTag, null, removePrevious: false);
        }

        public UIWindow GoToScreen(WindowTag windowTag, UnityEngine.Object param, bool removePrevious, bool deactivatePrevious = true)
        {
            //Close/deactivate previous window
            UIWindow lastWindow = GetLastWindow();
            if (removePrevious)
            {
                lastWindow.Close();
            }
            else if (deactivatePrevious)
            {
                DeactivateWindow(lastWindow);
            }

            //Open window
            UIWindow window = GetOpenedWindow(windowTag);
            if (window == null)
            {
                UIWindow prefab = Array.Find(screenPrefabs, val => val.WindowTag == windowTag);

                window = Instantiate(prefab);
                openeedWindows.Add(window);
            }
            else
            {
                ActivateWindow(window);
            }

            return window;
        }

        public UIWindow GetLastWindow()
        {
            if (openeedWindows.Count > 0)
            {
                UIWindow lastWindow = openeedWindows[openeedWindows.Count - 1];
                return lastWindow;
            }

            return null;
        }

        public UIWindow GetOpenedWindow(WindowTag screenName)
        {
            foreach (UIWindow window in openeedWindows)
            {
                if (window.WindowTag == screenName)
                    return window;
            }

            return null;
        }

        //Use in Inspector 
        public void Back()
        {
            UIWindow lastWindow = GetLastWindow();
            if (lastWindow == null)
                return;

            lastWindow.Close();

            if (openeedWindows.Count > 0)
            {
                UIWindow window = GetLastWindow();
                ActivateWindow(window);
            }
        }

        public void RemoveWindow(UIWindow window)
        {
            if (window == null)
                return;

            openeedWindows.Remove(window);
            Destroy(window.gameObject);
        }

        public void DeactivateWindow(UIWindow window)
        {
            if (window == null)
                return;

            window.gameObject.SetActive(false);
        }

        public void ActivateWindow(UIWindow window)
        {
            if (window == null)
                return;

            window.gameObject.SetActive(true);
        }

        public void CloseAll()
        {
            foreach (var window in openeedWindows)
            {
                if (window == null)
                    continue;

                window.Close();
            }
        }
    }
}

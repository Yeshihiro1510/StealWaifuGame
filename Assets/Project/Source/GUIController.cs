using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Source
{
    public class GUIController : MonoBehaviour
    {
        [field: SerializeField] public ShopWindow ShopWindow { get; private set; }
        [field: SerializeField] public Button ShopButton { get; private set; }
        [field: SerializeField] public Button SellButton { get; private set; }
        [field: SerializeField] public Button GatherButton { get; private set; }

        [SerializeField] private GUIWindow[] _windows;

        public bool AnyWindowOpened => _windows.Any(window => window.Opened);

        public event Action<GUIWindow> windowOpened;
        public event Action<GUIWindow> windowClosed;

        private void Awake()
        {
            _windows = transform.GetComponentsInChildren<GUIWindow>(true);

            foreach (var w in _windows)
            {
                w.gameObject.SetActive(false);
                w.opened += OnWindowOpened;
                w.closed += OnWindowClosed;
                continue;

                void OnWindowOpened() => windowOpened?.Invoke(w);
                void OnWindowClosed() => windowClosed?.Invoke(w);
            }
        }

        public void OpenAlone(GUIWindow window)
        {
            foreach (var w in _windows)
                w.Close();

            window.Open();
        }

        public void HideAll()
        {
            foreach (var w in _windows)
                w.gameObject.SetActive(false);
        }
    }

    public abstract class GUIWindow : MonoBehaviour
    {
        public bool Opened { get; private set; }

        public virtual void Open()
        {
            if (Opened) return;
            Opened = true;
        }

        public virtual void Close()
        {
            if (!Opened) return;
            Opened = false;
        }
        
        public event Action opened;
        public event Action closed;

        protected void OnOpen() => opened?.Invoke();
        protected void OnClose() => closed?.Invoke();
    }
}
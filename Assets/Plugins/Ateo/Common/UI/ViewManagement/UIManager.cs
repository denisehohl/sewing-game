using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ateo.ViewManagement
{
    [RequireComponent(typeof(UIView)), ExecuteInEditMode]
    public class UIManager : UIGroup
    {
        [FormerlySerializedAs("m_OnlyTopHierarchy")]
        public bool OnlyTopHierarchy = true;

        [FormerlySerializedAs("m_HideAllOnEnable")]
        public bool HideAllOnEnable = true;

        [FormerlySerializedAs("m_HideManagerOnEnable")]
        public bool HideManagerOnEnable;

        [FormerlySerializedAs("m_EnableAnimation")]
        public bool EnableAnimation;

        [FormerlySerializedAs("m_DefaultView")]
        public UIView DefaultView;

        // Views
        [NonSerialized]
        private UIView _viewCurrent;

        [NonSerialized]
        private readonly Dictionary<string, UIView> _viewsAll = new Dictionary<string, UIView>();

        // Elements
        [NonSerialized]
        private UIElement[] _elements = { };

        [NonSerialized]
        private readonly Dictionary<string, UIElement> _elementsCurrent = new Dictionary<string, UIElement>();

        [NonSerialized]
        private readonly Dictionary<string, UIElement> _elementsAll = new Dictionary<string, UIElement>();

        [NonSerialized]
        private readonly List<string> _elementsToDisable = new List<string>(10);

        // Public Properties
        [field: NonSerialized]
        public UIView[] Views { get; private set; } = new UIView[] { };

        protected override void OnEnable()
        {
            base.OnEnable();

            ClearCurrent();
            GetViewsAndElements();

            if (Application.isPlaying)
            {
                DebugDev.Log("View Manager: Initialized!" + (DefaultView ? "Enable default view." : ""));

                if (HideAllOnEnable)
                    HideAllViews();
                if (HideManagerOnEnable)
                    HideViewManager();
                if (DefaultView != null)
                    ShowView(DefaultView.name, false, true);
            }
        }

        private void OnDisable()
        {
            ClearAll();
            ClearCurrent();
        }

        public void ShowAllViews()
        {
            foreach (var v in Views)
            {
                ShowView(v, false, false, false);
            }

            _viewCurrent = null;
        }

        public void HideAllViews()
        {
            foreach (var v in Views)
            {
                HideView(v, false, false);
                DisableElementsForView(v);
            }

            _viewCurrent = null;
        }

        public void ShowView(string name)
        {
            ShowView(name, Application.isPlaying);
        }

        public bool ShowView(string name, bool callBehaviour = true)
        {
            return ShowView(name, EnableAnimation, callBehaviour);
        }

        public bool ShowView(string name, bool enableAnimation, bool callBehaviour = true)
        {
            if (name == null)
            {
                DebugDev.LogWarning("Show View: Null reference.");
                return false;
            }

            DebugDev.Log("Enable View: " + name + ", call behaviour: " + callBehaviour);

            // Get view from dictionary
            _viewsAll.TryGetValue(name, out var newView);

            if (_viewCurrent == newView)
                return true;

            // Hide current view
            if (_viewCurrent != null)
            {
                HideView(_viewCurrent, enableAnimation, callBehaviour);
            }

            if (newView != null)
            {
                _viewCurrent = newView;
                ShowView(newView, enableAnimation, callBehaviour);
                return true;
            }

            DebugDev.Log("VM: " + name + " is not a view");
            return false;
        }

        public void ShowViewManager()
        {
            HideAllViews();
            Show(EnableAnimation && Application.isPlaying, Application.isPlaying);

            if (DefaultView != null)
            {
                ShowView(DefaultView.name, Application.isPlaying);
            }
        }

        public void HideViewManager()
        {
            Hide(EnableAnimation && Application.isPlaying, Application.isPlaying);
        }

        public void AddView(UIView view)
        {
            if (view == null)
            {
                DebugDev.LogWarning("Add View: Null reference.");
                return;
            }

            if (!_viewsAll.ContainsKey(view.name))
            {
                view.transform.SetParent(transform, false);
                _viewsAll.Add(view.name, view);
                HideView(view, false, false);
            }
            else
            {
                DebugDev.Log($"UIManager.AddView(): View with name {view.name} is already present");
            }
        }

        public void AddElement(UIElement element)
        {
            if (element == null)
            {
                DebugDev.LogWarning("Add Element: Null reference.");
                return;
            }

            if (!_elementsAll.ContainsKey(element.name))
            {
                element.transform.SetParent(transform);
                _elementsAll.Add(element.name, element);
            }
            else
            {
                DebugDev.Log($"UIManager.AddElement(): Element with name {element.name} is already present");
            }
        }

        // Private Methods
        private void ShowView(UIView view, bool animate, bool callBehaviour, bool hideOtherElements = true)
        {
            if (view == null)
            {
                DebugDev.LogWarning("Show View: Null reference.");
                return;
            }

            view.Show(animate && Application.isPlaying, callBehaviour);
            EnableElementsForView(view, hideOtherElements);
        }

        private void HideView(UIView view, bool animate, bool callBehaviour)
        {
            if (view == null)
            {
                DebugDev.LogWarning("Hide View: Null reference.");
                return;
            }

            view.Hide(animate && Application.isPlaying, callBehaviour);
        }

        private void EnableElementsForView(UIView view, bool hideOtherElements = true)
        {
            if (view == null)
            {
                DebugDev.LogWarning("Enable Elements For View: Null reference.");
                return;
            }

            if (hideOtherElements)
            {
                _elementsToDisable.Clear();

                foreach (var element in _elementsCurrent.Keys)
                {
                    var keepElement = false;

                    foreach (var v in view.Elements)
                    {
                        if (string.Equals(element, v))
                            keepElement = true;
                    }

                    if (!keepElement)
                        _elementsToDisable.Add(element);
                }
            }

            foreach (var e in _elementsToDisable)
            {
                DisableElement(e);
            }

            if (view != null)
            {
                foreach (var e in view.Elements)
                {
                    EnableElement(e);
                }
            }
        }

        private void DisableElementsForView(UIView view)
        {
            if (view == null)
            {
                DebugDev.LogWarning("Disable Elements For View: Null reference.");
                return;
            }

            foreach (var e in view.Elements)
            {
                DisableElement(e);
            }
        }

        private void EnableElement(string element)
        {
            if (element == null)
            {
                DebugDev.LogWarning("Enable Element: Null reference.");
                return;
            }

            if (_elementsAll.ContainsKey(element))
            {
                EnableElement(_elementsAll[element]);
            }
            else
            {
                DebugDev.LogWarning($"UIManager.EnableElement(): Element with name {element} was not found");
            }
        }

        private void EnableElement(UIElement element)
        {
            if (element == null)
            {
                DebugDev.LogWarning("Enable Element: Null reference.");
                return;
            }

            if (!_elementsCurrent.ContainsKey(element.name))
            {
                DebugDev.Log("Enable Element: " + element.name);

                _elementsCurrent.Add(element.name, element);
                element.Show(EnableAnimation && Application.isPlaying, Application.isPlaying);
            }
        }

        private void DisableElement(string element)
        {
            if (element == null)
            {
                DebugDev.LogWarning("Disable Element: Null reference.");
                return;
            }

            if (_elementsAll.ContainsKey(element))
            {
                DisableElement(_elementsAll[element]);
            }
            else
            {
                DebugDev.Log($"UIManager.DisableElement(): Element with name {element} was not found");
            }
        }

        private void DisableElement(UIElement element)
        {
            if (element == null)
            {
                DebugDev.LogWarning("Disable Element: Null reference.");
                return;
            }

            if (_elementsCurrent.ContainsKey(element.name))
            {
                _elementsCurrent.Remove(element.name);
                element.Hide(EnableAnimation && Application.isPlaying, Application.isPlaying);

                DebugDev.Log("Disable Element: " + element.name);
            }

            else if (element.IsVisible) // Element was not currenlty showing, so we hide it immediately and don't call its behaviour
            {
                element.Hide(false, false);
                DebugDev.Log("Disable Element: " + element.name);
            }
        }

        private static void AddToDictionary<T>(T element, string name, Dictionary<string, T> dictionary)
        {
            if (element == null || name == null || dictionary == null)
            {
                DebugDev.LogWarning("Add To Dictionary: Null reference.");
                return;
            }

            if (!dictionary.ContainsKey(name))
            {
                dictionary[name] = element;
            }
            else
            {
                DebugDev.LogError("ViewManager: " + name + " is already saved as " + typeof(T).ToString());
            }
        }

        public void GetViewsAndElements()
        {
            ClearAll();

            if (!OnlyTopHierarchy)
            {
                Views = gameObject.GetComponentsInChildren<UIView>();
                _elements = gameObject.GetComponentsInChildren<UIElement>();
            }
            else
            {
                var views = new List<UIView>();
                var elements = new List<UIElement>();

                foreach (Transform t in transform)
                {
                    if (t.parent == transform)
                    {
                        var view = t.GetComponent<UIView>();
                        if (view != null)
                            views.Add(view);

                        var element = t.GetComponent<UIElement>();
                        if (element != null)
                            elements.Add(element);
                    }
                }

                Views = views.ToArray();
                _elements = elements.ToArray();
            }

            foreach (var e in Views)
            {
                AddToDictionary(e, e.name, _viewsAll);
            }

            foreach (var e in _elements)
            {
                AddToDictionary(e, e.name, _elementsAll);
            }
        }

        private void ClearAll()
        {
            _viewsAll.Clear();
            _elementsAll.Clear();
        }

        private void ClearCurrent()
        {
            _viewCurrent = null;
            _elementsCurrent.Clear();
        }
    }
}
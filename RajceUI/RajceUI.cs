using RajceUI.Updater;
using RajceUI.Elements;

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

namespace RajceUI
{
    public abstract class RajceUI : MonoBehaviour
    {
        private static PropertyInfo _textFieldInputPropertyInfo;
        private static readonly HashSet<RajceUI> Roots = new HashSet<RajceUI>();

        private readonly List<Element> _elements = new List<Element>();
        private readonly Queue<Func<Element>> _CreateElementOnEnableQueue = new Queue<Func<Element>>();

        public readonly ElementUpdater updater = new ElementUpdater();
        public IReadOnlyList<Element> Elements => _elements;

        public abstract bool WillUseKeyInput();
        protected abstract void BuildInternal(Element elem);

        private static void Register(RajceUI root) => Roots.Add(root);
        private static void Unregister(RajceUI root) => Roots.Remove(root);

        public static bool WillUseKeyInputAny() => Roots.Any(r => r.WillUseKeyInput());
        public static void GlobalBuild(Element elem)
        {
            RajceUI ui = Roots.FirstOrDefault();
            if (ui == null)
            {
                Debug.LogWarning($"There is no active {nameof(RajceUI)}");
                return;
            }

            ui.Build(elem);
        }

        protected void SuppressInputKey()
        {
            _textFieldInputPropertyInfo ??= typeof(GUIUtility).GetProperty("textFieldInput", BindingFlags.NonPublic | BindingFlags.Static);

            if (WillUseKeyInput())
                _textFieldInputPropertyInfo.SetValue(null, true);
        }

        public void Build(Element elem)
        {
            BuildInternal(elem);

            updater.Register(elem);
            updater.RegisterWindowRecursive(elem);

            _elements.Add(elem);
        }
        public void BuildOnEnable(Func<Element> createElem) => _CreateElementOnEnableQueue.Enqueue(createElem);

        protected virtual void OnEnable()
        {
            while (_CreateElementOnEnableQueue.Count > 0)
            {
                Build(_CreateElementOnEnableQueue.Dequeue()());
            }

            Register(this);

            foreach (Element elem in _elements)
                elem.Enable = true;
        }

        protected virtual void OnDisable() => Unregister(this);

        protected virtual void Update() => updater.Update();

        protected virtual void OnDestroy() => _elements.ForEach(elem => elem.DetachView());

        protected virtual void OnGUI() => SuppressInputKey();
    }
}

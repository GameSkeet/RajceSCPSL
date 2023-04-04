using RajceUI.Elements;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UIElements;

namespace RajceUI.UIToolkit
{
    [RequireComponent(typeof(UIDocument))]
    internal class RajceUIToolkit : RajceUI
    {
        public const string USSRootClassName = "rajceui-root";

        protected UIDocument uiDocument;

        public KeyCode closeWindowKey = KeyCode.Insert;
        public EventModifiers closeWindowKeyModifiers;

        protected override void BuildInternal(Element elem)
        {
            if (uiDocument == null)
                uiDocument = GetComponent<UIDocument>();

            VisualElement root = uiDocument.rootVisualElement;
            root.AddToClassList(USSRootClassName);

            //VisualElement visElem = UIToolkitBuilder.Build(elem);
            //root.Add(visElem);
        }

        public override bool WillUseKeyInput()
        {
            return uiDocument != null; //&& UIToolkitUtility.WillUseKeyInput(uiDocument.rootVisualElement?.panel);
        }

        protected void ApplyCloseWindowKey()
        {
            
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;
            ApplyCloseWindowKey();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (uiDocument != null && uiDocument.rootVisualElement is { } ve)
                ve.visible = true;

            ApplyCloseWindowKey();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (uiDocument != null && uiDocument.rootVisualElement is { } ve)
                ve.visible = false;
        }
    }
}

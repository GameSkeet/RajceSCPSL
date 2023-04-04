using RajceInternal.Features;
using RajceInternal.Tabs;

using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace RajceInternal
{
    internal class Menu : MonoBehaviour
    {
        public static readonly Vector2 MenuSize = new Vector2(650, 400);
        public static readonly int offsetBetweenTabs = 5;

        private bool m_Visible = true;
        private Rect _windowRect = new Rect(100, 100, MenuSize.x, MenuSize.y);
        private bool m_bIsDragging = false;
        private Vector2? _lastCurPos = null;

        private Vector2 m_vCursorPosition = Vector2.zero;

        #region UI

        private Dictionary<string, TabBase> Tabs = new Dictionary<string, TabBase>()
        {
            { "Home", new HomeTab() },
            { "Player", new PlayerTab() },
            { "Visuals", new VisualsTab() },
            { "Movement", new MovementTab() },
            { "Combat", null },
            { "Misc", new MiscTab() },

#if DEBUG
            { "Debug", new DebugTab() }
#endif
        };
        private string m_sCurrentSeletecTab = "Home";
        private float? calculatedStartPos = null;
        private Vector2 m_vScrollPostion = Vector2.zero;

        private void OnMenuDraw(int id)
        {
            Rect tabSpace = new Rect(0, 0, MenuSize.x, 50);

            // Creates the space for the tabs
            GUI.Box(tabSpace, "");

            GUI.skin.button.fontSize = 14;
            if (calculatedStartPos == null)
            {
                calculatedStartPos = MenuSize.x;

                foreach (var tab in Tabs)
                    calculatedStartPos -= (tab.Key.CalcSize(14).x + 14 + offsetBetweenTabs) / 2;

                calculatedStartPos -= MenuSize.x / 2;
            }

            float _startPosX = calculatedStartPos.Value;
            float _startPosY = tabSpace.height / 2;

            Vector2 ContentPosition = new Vector2(0, tabSpace.yMax);
            Vector2 ContentSize = new Vector2(MenuSize.x, MenuSize.y - tabSpace.yMax);

            TabBase drawTab = null;
            foreach (var tab in Tabs)
            {
                Vector2 vec = tab.Key.CalcSize(14);
                if (GUI.Button(new Rect(_startPosX, _startPosY, vec.x + 14, vec.y), tab.Key))
                    m_sCurrentSeletecTab = tab.Key;

                _startPosX += vec.x + 14 + offsetBetweenTabs;

                if (m_sCurrentSeletecTab == tab.Key)
                    drawTab = tab.Value;
            }

            if (drawTab != null)
            {
                GUISkin old = GUI.skin;

                Vector2 targetContentSize = drawTab.PreRenderTab();
                if (drawTab.MinSize != null)
                {
                    Vector2 normed = drawTab.MinSize.Value;
                    Vector2 calced = new Vector2(ContentSize.x * normed.x, ContentSize.y * normed.y);

                    if (targetContentSize.x < calced.x)
                        targetContentSize.x = calced.x;
                    if (targetContentSize.y < calced.y)
                        targetContentSize.y = calced.y;
                }

                Rect viewRect = new Rect(ContentPosition, targetContentSize);
                m_vScrollPostion = GUI.BeginScrollView(new Rect(ContentPosition, ContentSize), m_vScrollPostion, viewRect);

                drawTab.RenderTab(ContentPosition, targetContentSize); // viewRect, targetContentSize

                GUI.EndScrollView();

                GUI.skin = old;
            }  
        }

        #endregion

        #region Unity Callbacks

        private IEnumerator<object> WaitForFullLoad()
        {
            for (int i = 0; i < 80; i++)
                yield return new WaitForEndOfFrame(); // wait 4 frames just to be sure

            FeatureManager.OnConnected();
        }

        void Start()
        {
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name == "Facility")
                    StartCoroutine(WaitForFullLoad());
            };
            SceneManager.activeSceneChanged += (bruh, scene) =>
            {
                if (scene.name != "Facility")
                    FeatureManager.OnDisconnected();
            };

            if (SceneManager.GetActiveScene().name == "Facility")
                FeatureManager.OnConnected();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                m_Visible = !m_Visible;
                Event.current.Use();
            }
            CursorManager.singleton.debuglogopen = m_Visible;

            Vector2 pos = Input.mousePosition.ToV2();
            pos.y = Screen.height - pos.y;

            FeatureManager.RunFeatures(pos);
            m_vCursorPosition = pos;

            #region Window Dragging

            // Window dragging
            if (!m_Visible)
            {
                m_bIsDragging = false;
                _lastCurPos = null;
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_bIsDragging = false;
                _lastCurPos = null;
            }

            //Console.WriteLine("PosX: {0} PosY: {1}", pos.x, pos.y);

            // Check X
            if (pos.x >= _windowRect.x && pos.x <= _windowRect.xMax)
            {
                // Check Y
                if (pos.y >= _windowRect.y && pos.y <= _windowRect.y + 20)
                {
                    if (Input.GetMouseButtonDown(0))
                        m_bIsDragging = true;
                }
            }

            if (Input.GetMouseButton(0) && m_bIsDragging)
            {
                if (_lastCurPos == null)
                    _lastCurPos = pos;

                Vector2 delta = pos - _lastCurPos.Value;
                _windowRect.x += delta.x;
                _windowRect.y += delta.y;

                _lastCurPos = pos;
            }

            #endregion
        }

        void OnGUI()
        {
            FeatureManager.DrawFeatures();

            if (!m_Visible)
                return;

            GUI.Window(0, _windowRect, OnMenuDraw, "Rajce - " + m_sCurrentSeletecTab);
        }

        #endregion

        // Add cleaning code in here
        public void DoCleaning() { }
    }
}

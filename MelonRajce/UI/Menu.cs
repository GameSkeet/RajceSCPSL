using MelonRajce.Features;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace MelonRajce.UI
{
    internal static class Menu
    {
        public enum ElementType
        {
            Undefined, // Unknown object

            #region Wrappers

            Label, // A wrapper for GUI.Label
            Button, // A wrapper for GUI.Button
            Checkbox, // A wrapper for GUI.Toggle

            #endregion

            #region Custom Elements

            Slider, // Creates a slider with text above it
            FeatureToggle, // A toggle that uses a feature for its state

            #endregion

            // These types allow children
            #region Style Elements

            Padding, // Adds padding between elements
            Row, // Changes how the children should be orderer Left -> Right
            Column, // Changes how the children should be orderer Top -> Bottom
            Group // Creates a group where other elements can be added

            #endregion
        }
        public class Element
        {
            #region Main Element Data

            public ElementType Type = ElementType.Undefined; // The type of the element

            public Element Parent = null; // The parent that hold this element (null is root)
            private List<Element> Children = new List<Element>(); // Children that are owned by this element
            public bool RedrawUIOnAction = false; // Should the element trigger the ReDraw flag after its OnAction

            #endregion

            #region Element Style

            // The minimum size required for the style element (null means use the calculated size)
            // Also these are only supposed to be changed before the first Size Calculation
            public float? MinSizeX = null;
            public float? MinSizeY = null;

            // Centers the given element on the axis X, Y or both
            public bool CenterX = false;
            public bool CenterY = false;

            // Padding data (only used if the element is Padding type)
            public float PaddingX = 0f;
            public float PaddingY = 0f;

            // Data for this element to be rendered
            public GUIContent Content = new GUIContent(); // Content to draw
            public GUIStyle UsedStyle = null;
            public int? FontSize = null; // (null means use the fontSize from the copied Style)

            // Offsets
            public float OffsetX = 15f;
            public float OffsetY = 5f;
            public float BorderOffset = 5f;

            #endregion

            #region Type based data

            public Action<object> OnAction = null; // Callback for when the object is some how handled

            // Toggle
            public bool ToggleCurrentValue = false; // Indicates if the toggle is selected

            #region Slider

            public float SliderCurrentValue = 0f; // The current value on the slider
            public float SliderMin = 0f; // Minimum value on the slider
            public float SliderMax = 100f; // Maximum value on the slider
            public Vector2 SliderSize = new Vector2(100, 10); // Slider size 
            public bool WholeNumberSlider = false; // Should the slider slide with whole numbers

            #endregion

            #region Feature Toggle

            public Feature Feature = null; // The feature to use in the drawing
            public bool IsKeybinding = false; // Indicates if the user is picking a keybind
            public double LastKeybindClick = 0d;

            #endregion

            #endregion

            #region Cache

            // The size of this element
            public Vector2 ElementSize { get; private set; } = Vector2.zero;

            #endregion

            // Adds a new child to this element
            public void AddChild(Element child) => Children.Add(child);

            // Returns all of the elements that are owned by this element
            public Element[] GetElements() => Children.ToArray();

            // Calculates the element size
            // Offsets X and Y are used as element spacers
            public Vector2 CalcSize(bool ForceRecalculation = false)
            {
                if (!ForceRecalculation && ElementSize != Vector2.zero)
                    return ElementSize;

                Vector2 result = Vector2.zero;

                switch (Type)
                {
                    #region Custom Elements

                    case ElementType.Slider:
                        {
                            GUIStyle style = GUI.skin.horizontalSlider.Copy();
                            style.fontSize = FontSize ?? 0;

                            // Check if some header text is set
                            if (!string.IsNullOrEmpty(Content.text))
                            {
                                result += style.CalcSize(Content); // Calculate the size of the header

                                result.y += 2; // Add space between the slider and header
                            }

                            result.y += SliderSize.y; // Add the slider height

                            // Check if the slider is wider than the header
                            if (result.x < SliderSize.x)
                                result.x = SliderSize.x; // Set the slider size as the wides

                            result.x += 5; // Add 5 to x cause of the space between slider and value display
                            style.fontSize = (int)SliderSize.y; // Set the fontsize to slider height cause we want the display to be tall as the slider
                            result.x += style.CalcSize(new GUIContent(WholeNumberSlider ? SliderMax.ToString() : SliderMax.ToString().Rep(1))).x; // Add the display text width to the total

                            break;
                        }
                    case ElementType.FeatureToggle:
                        {
                            GUIStyle toggle = GUI.skin.toggle.Copy();
                            toggle.fixedWidth = 0;
                            toggle.fixedHeight = 0;

                            GUIStyle label = GUI.skin.label.Copy();
                            GUIStyle button = GUI.skin.button.Copy();

                            int fontsize = 16; // Fontsize used
                            if (FontSize != null)
                                fontsize = FontSize.Value; // Set the custom fontsize

                            toggle.fontSize = fontsize; // Header size
                            label.fontSize = fontsize - 2; // Description size
                            button.fontSize = fontsize - 2; // Keybind size

                            Vector2 toggleSize = toggle.CalcSize(new GUIContent(Feature.Name)); // Calculates the size of the toggle
                            result.y += toggleSize.y; // Add the height to the total

                            Vector2 buttonSize = button.CalcSize(new GUIContent("DEFAULT")); // Button size
                            Vector2 labelSize = label.CalcSize(new GUIContent(Feature.Description)); // Label size

                            result.y += 2; // Add the spacing between the elements

                            float widest = toggleSize.x + (Feature.IsKeyBindable ? (5 + buttonSize.x) : 0); // Calculate the size of the top

                            // Check if the label is wider than the top
                            if (widest < labelSize.x)
                                widest = labelSize.x; // Set the label to be wider

                            result.y += labelSize.y; // Add the label height
                            result.x += widest; // Add the widest element

                            break;
                        }

                    #endregion

                    #region Style Elements

                    case ElementType.Padding:
                        {
                            result += new Vector2(PaddingX, PaddingY); // Add the padding

                            break;
                        }
                    case ElementType.Row:
                        {
                            float tallestElem = 0f;
                            for (int i = 0; i < Children.Count; i++)
                            {
                                Vector2 eSize = Children[i].CalcSize(ForceRecalculation); // Calculate the child

                                // Check if the child is taller than the currently tallest child
                                if (eSize.y > tallestElem)
                                    tallestElem = eSize.y;

                                result.x += eSize.x; // Add the width of the child to the total

                                // Check if the current element isn't the last one
                                if (!(i == (Children.Count - 1)))
                                    result.x += OffsetX; // Add the spacing between elements
                            }

                            result.y += tallestElem; // Add the height of the row

                            // Check if the row has the minimum required size
                            if (MinSizeX != null && result.x < MinSizeX.Value)
                                result.x = MinSizeX.Value;
                            if (MinSizeY != null && result.y < MinSizeY.Value)
                                result.y = MinSizeY.Value;

                            break;
                        }
                    case ElementType.Column:
                        {
                            float widestElement = 0f;
                            for (int i = 0; i < Children.Count; i++)
                            {
                                Vector2 eSize = Children[i].CalcSize(ForceRecalculation); // Calculate the child

                                // Check if the child is wider than the currently widest child
                                if (eSize.x > widestElement)
                                    widestElement = eSize.x;

                                result.y += eSize.y; // Add the height of the child to the total

                                // Check if the current element isn't the last one
                                if (!(i == (Children.Count - 1)))
                                    result.y += OffsetY; // Add the spacing between elements
                            }

                            result.x += widestElement; // Add the width of the column

                            // Check if the column has the minimum required size
                            if (MinSizeX != null && result.x < MinSizeX.Value)
                                result.x = MinSizeX.Value;
                            if (MinSizeY != null && result.y < MinSizeY.Value)
                                result.y = MinSizeY.Value;

                            break;
                        }
                    case ElementType.Group:
                        {
                            GUIStyle style = GUI.skin.box.Copy();
                            style.fontSize = FontSize ?? 0;

                            Vector2 group = style.CalcSize(Content); // Calculate the box size
                            result = new Vector2(BorderOffset * 2, group.y + BorderOffset * 2); // We want to multiply the border offset by 2 cause we want the space on every side

                            float widestElem = 0f;
                            for (int i = 0; i < Children.Count; i++)
                            {
                                Vector2 eSize = Children[i].CalcSize(ForceRecalculation); // Calculate the child

                                // Check if the child is wider than the currently widest child
                                if (eSize.x > widestElem)
                                    widestElem = eSize.x;

                                result.y += eSize.y; // Add the height to the total size

                                // Check if the this element isn't the last one
                                if (!(i == (Children.Count - 1)))
                                    result.y += OffsetY; // Add the spacing between elements
                            }

                            // Check if the widest element is wider than the box it self
                            if (widestElem > group.x)
                                result.x += widestElem; // Add the widest element width
                            else result.x += group.x; // Add the groupbox's width

                            // Check if the group has the minimum required size
                            if (MinSizeX != null && result.x < MinSizeX.Value)
                                result.x = MinSizeX.Value;
                            if (MinSizeY != null && result.y < MinSizeY.Value)
                                result.y = MinSizeY.Value;

                            break;
                        }

                    #endregion

                    // Calculates the size for the wrapper element
                    default:
                        {
                            GUIStyle style = UsedStyle.Copy();
                            style.fixedWidth = 0;
                            style.fixedHeight = 0;

                            style.fontSize = FontSize ?? 0;

                            result = style.CalcSize(Content); // Calculate the size for the wrapper element

                            break;
                        }
                }

                return ElementSize = result;
            }
        }

        // Global constants
        public static readonly Vector2 MenuSize = new Vector2(650, 450);
        public static readonly float OFFSET_BETWEEN_TABS = 5f;

        // Window data
        private static Rect m_rWindow = new Rect(50, 50, MenuSize.x, MenuSize.y); // Window rect
        private static Vector2? m_vLastCurPos = null; // Last cursor position for the delta of dragging

        // Dragging
        private static bool m_bDragging = false; // Indicates if the user is dragging the window

        // TabSystem
        private static TabSystem m_tabSystem = null; // An instance of the TabSystem
        private static UITab CurrentTab = null; // The current selected tab
        private static bool ReDrawPage = false; // Indicates if the current page should be redrawn
        private static Vector2 m_vScrollPosition = Vector2.zero; // The position where the scroller is at

        // Menu info
        public static GUISkin MenuSkin = null;
        public static GUISkin DefaultSkin = null;
        public static bool IsVisible { get; private set; } = true;

        // Draws the elements/page of the current tab
        private static void DrawContent(Vector2 pos, Vector2 size)
        {
            void DrawElements(Element parent, Element[] elems, bool drawAsColumn, ref Rect rect)
            {
                Vector2 rSize = rect.size; // The original rect size

                for (int i = 0; i < elems.Length; i++)
                {
                    Element elem = elems[i];

                    switch(elem.Type)
                    {
                        #region Wrappers

                        case ElementType.Label:
                            {
                                int oldSize = GUI.skin.label.fontSize;
                                GUI.skin.label.fontSize = elem.FontSize ?? 0;

                                Vector2 eSize = elem.CalcSize();
                                Rect cRect = rect.Copy();

                                // Centers the element on the given axis
                                if (elem.CenterX)
                                    cRect.x += (rSize.x / 2) - (eSize.x / 2);
                                if (elem.CenterY)
                                    cRect.y += (rSize.y / 2) - (eSize.y / 2);

                                // Draw the label
                                GUI.Label(Utils.SetSize(ref cRect, eSize), elem.Content);

                                if (!elem.CenterY)
                                    if (drawAsColumn)
                                        rect.y += cRect.size.y + elem.OffsetY; // Add the height of the label
                                    else rect.x += cRect.size.x + elem.OffsetX; // Add the width of the label

                                GUI.skin.label.fontSize = oldSize;

                                break;
                            }
                        case ElementType.Button:
                            {
                                int oldSize = GUI.skin.button.fontSize;
                                GUI.skin.button.fontSize = elem.FontSize ?? 0;

                                // Draw the button and check if the OnAction isn't null
                                if (GUI.Button(Utils.SetSize(ref rect, elem.CalcSize()), elem.Content) && elem.OnAction != null)
                                    elem.OnAction(null); // Invoke OnAction when the button is cliecked

                                if (drawAsColumn)
                                    rect.y += rect.size.y + elem.OffsetY; // Add the height of the button
                                else rect.x += rect.size.x + elem.OffsetX; // Add the width of the button

                                GUI.skin.button.fontSize = oldSize;

                                break;
                            }
                        case ElementType.Checkbox:
                            {
                                int oldSize = GUI.skin.toggle.fontSize;
                                GUI.skin.toggle.fontSize = elem.FontSize ?? 0;

                                Vector2 eSize = elem.CalcSize();
                                Rect cRect = rect.Copy();

                                // Centers the element on the X
                                if (elem.CenterX)
                                    cRect.x += (rSize.x / 2) - (eSize.x / 2);

                                bool curr = elem.ToggleCurrentValue; // Get the value
                                curr = GUI.Toggle(Utils.SetSize(ref cRect, eSize), curr, elem.Content); // Overwrite the value with the toggle state

                                // Check if the value is different from the original one and check if OnAction isn't null
                                if (curr != elem.ToggleCurrentValue)
                                {
                                    elem.ToggleCurrentValue = curr; // Set the value

                                    // Check if onAction is set
                                    if (elem.OnAction != null)
                                        elem.OnAction(curr); // Invoke the OnAction

                                    // Check if redraw is set
                                    if (elem.RedrawUIOnAction)
                                        ReDrawPage = true; // Redraw the page on the next frame
                                }

                                if (drawAsColumn)
                                    rect.y += cRect.size.y + elem.OffsetY; // Add the height of the checkbox
                                else rect.x += cRect.size.x + elem.OffsetX; // Add the width of the checkbox

                                GUI.skin.toggle.fontSize = oldSize;

                                break;
                            }

                        #endregion

                        #region Custom Elements

                        case ElementType.Slider:
                            {
                                int oldSize = GUI.skin.label.fontSize;
                                GUI.skin.label.fontSize = elem.FontSize ?? 0;

                                Rect cRect = rect.Copy(); // Copy the rect
                                Vector2 lSize = Vector2.zero; // The size of the slider

                                // Check if the some header is set
                                if (!string.IsNullOrEmpty(elem.Content.text))
                                {
                                    lSize = GUI.skin.label.CalcSize(elem.Content); // Calculate the size of the label

                                    GUI.Label(Utils.SetSize(ref cRect, lSize), elem.Content);
                                    cRect.y += lSize.y + 2; // Add the label size + the offset between elements
                                }

                                float curr = elem.SliderCurrentValue; // Get the current value
                                curr = GUI.HorizontalSlider(Utils.SetSize(ref cRect, elem.SliderSize), curr, elem.SliderMin, elem.SliderMax); // Draw the slider

                                // Check if the slider has a different value
                                if (curr != elem.SliderCurrentValue)
                                {
                                    elem.SliderCurrentValue = elem.WholeNumberSlider ? (float)Math.Floor(curr) : curr;

                                    // Check if onAction is set
                                    if (elem.OnAction != null)
                                        elem.OnAction(elem.SliderCurrentValue); // Invoke OnAction with new value
                                }

                                lSize.y += cRect.size.y; // Add the label height to the total

                                // Check if the slider is wider than the label
                                if (lSize.x < cRect.size.x)
                                    lSize.x = cRect.size.x; // Add the label width to the total

                                cRect.position += new Vector2(cRect.size.x + 5, -2); // Add the spacing
                                lSize.x += 5; // Add the spacing to result

                                GUI.skin.label.fontSize = (int)elem.SliderSize.y; // Set the fontsize of the value text
                                GUIContent c = new GUIContent(elem.SliderCurrentValue.ToString()); // Create the content for the value text
                                Vector2 valSize = GUI.skin.label.CalcSize(c); // Calculate the size of the text
                                lSize.x += valSize.x; // Add the size of the text

                                GUI.Label(Utils.SetSize(ref cRect, valSize), c); // Draw the value text

                                if (drawAsColumn)
                                    rect.y += lSize.y + elem.OffsetY; // Add the height to rect
                                else rect.x += lSize.x + elem.OffsetX; // Add the width to rect

                                GUI.skin.label.fontSize = oldSize;

                                break;
                            }
                        case ElementType.FeatureToggle:
                            {
                                GUIStyle toggle = GUI.skin.toggle.Copy();
                                GUIStyle label = GUI.skin.label.Copy();
                                GUIStyle button = GUI.skin.button.Copy();

                                int fontsize = 16; // Fontsize used
                                if (elem.FontSize != null)
                                    fontsize = elem.FontSize.Value; // Set the custom fontsize

                                // Set the styles
                                GUI.skin.toggle.fontSize = fontsize; // This is a header
                                GUI.skin.label.fontSize = fontsize - 2; // This is a description
                                GUI.skin.button.fontSize = fontsize - 2; // For the keybind button


                                Feature feature = elem.Feature; // Get the feature
                                Rect cRect = rect.Copy(); // Copy the rect

                                GUIContent c = new GUIContent(feature.Name); // Creates the text for the toggle
                                Vector2 toggleSize = GUI.skin.toggle.CalcSize(c); // Calculates the size for the toggle

                                bool curr = feature.IsActive; // Current feature value
                                curr = GUI.Toggle(Utils.SetSize(ref cRect, toggleSize), curr, c); // Creates the toggle

                                // Checks if the toggle was changed
                                if (curr != feature.IsActive)
                                {
                                    feature.IsActive = curr; // Set the value

                                    // Check if onAction is set
                                    if (elem.OnAction != null)
                                        elem.OnAction(curr); // invoke onAction

                                    if (elem.RedrawUIOnAction)
                                        ReDrawPage = true;
                                }

                                Vector2 buttonSize = Vector2.zero; // The button size
                                if (feature.IsKeyBindable)
                                {
                                    cRect.x += cRect.width + 5; // Add the size of toggle and spacing

                                    c = new GUIContent(elem.IsKeybinding ? "..." : feature.BindedKey.ToString()); // Create the content for the button
                                    buttonSize = GUI.skin.button.CalcSize(c); // calculates the size

                                    Color old = GUI.backgroundColor; // Get the orig color

                                    // Check if the current keybind button is a toggle
                                    if (feature.IsKeybindToggle)
                                        GUI.backgroundColor = Color.red; // Set the color for the toggle

                                    // Draws the button
                                    if (GUI.Button(Utils.SetSize(ref cRect, buttonSize), c))
                                    {
                                        // Check if the user double clicked
                                        if ((Time.realtimeSinceStartup - elem.LastKeybindClick) < 0.3)
                                        {
                                            feature.IsKeybindToggle = !feature.IsKeybindToggle; // Switches the keybind
                                            elem.IsKeybinding = false; // Stop the keybinding
                                        }
                                        else elem.IsKeybinding = !elem.IsKeybinding; // The user clicked once

                                        elem.LastKeybindClick = Time.realtimeSinceStartup; // Sets the last click so we can detect double clicks
                                    }

                                    // Check if the user is keybinding
                                    if (elem.IsKeybinding && Event.current.isKey)
                                    {
                                        if (Event.current.keyCode == KeyCode.Delete)
                                            elem.IsKeybinding = false;
                                        else
                                        {
                                            feature.BindedKey = Event.current.keyCode; // Set the current pressed key
                                            elem.IsKeybinding = false; // Stop the keybind detection
                                        }
                                    }

                                    GUI.backgroundColor = old; // Restore the color back
                                }

                                cRect.x = rect.x; // Set the position back
                                cRect.y += toggleSize.y + 2;

                                c = new GUIContent(feature.Description); // Create the text for the label
                                Vector2 labelSize = GUI.skin.label.CalcSize(c); // Calculate the size

                                GUI.Label(Utils.SetSize(ref cRect, labelSize), c); // Draw the description

                                float topWidth = toggleSize.x + 5 + buttonSize.x; // Calculates the width of the top
                                float widest = topWidth;

                                // Check if the description is wider than the the top
                                if (topWidth < labelSize.x)
                                    widest = labelSize.x; // Set the description to be wider

                                if (drawAsColumn)
                                    rect.y += toggleSize.y + 2 + labelSize.y + elem.OffsetY; // Add the height of the element to the total
                                else rect.x += widest + elem.OffsetX; // Add the width of the element to the total

                                // Restore the styles
                                GUI.skin.toggle = toggle;
                                GUI.skin.label = label;
                                GUI.skin.button = button;

                                break;
                            }

                        #endregion

                        #region Style Elements

                        case ElementType.Padding:
                            {
                                rect.position += new Vector2(elem.PaddingX, elem.PaddingY); // Add the padding

                                break;
                            }
                        case ElementType.Row:
                            {
                                Utils.SetSize(ref rect, elem.CalcSize()); // Set the row size

                                Rect cRect = rect.Copy(); // Copy the rect so we dont need to worry about position being changed
                                DrawElements(elem, elem.GetElements(), false, ref cRect); // Draw the children of the row

                                if (drawAsColumn)
                                    rect.y += rect.size.y; // Add the height of the row
                                else rect.x += rect.size.x; // Add the width of the row

                                break;
                            }
                        case ElementType.Column:
                            {
                                Utils.SetSize(ref rect, elem.CalcSize()); // Set the column size

                                Rect cRect = rect.Copy(); // Copy the rect so we dont need to worry about position begin changed
                                DrawElements(elem, elem.GetElements(), true, ref cRect); // Draw the children of the column

                                if (drawAsColumn)
                                    rect.y += rect.size.y; // Add the height of the column
                                else rect.x += rect.size.x; // Add the width of the column

                                break;
                            }
                        case ElementType.Group:
                            {
                                int oldSize = GUI.skin.box.fontSize;
                                GUI.skin.box.fontSize = elem.FontSize ?? 0;

                                Vector2 eSize = elem.CalcSize(); // Calculate the size so we can use it for centering if enabled
                                Rect cRect = rect.Copy(); // Copy the rect so we dont need to worry about it begin changed

                                // Check if we need to center on X
                                if (elem.CenterX)
                                    cRect.x += (rSize.x / 2) - (eSize.x / 2); // Calculate center

                                GUI.Box(Utils.SetSize(ref cRect, eSize), elem.Content); // Draws the box

                                cRect.position += new Vector2(elem.BorderOffset, GUI.skin.box.fontSize + (elem.BorderOffset * 2) + 1); // Adds the offsets for the inner elements

                                Vector2 oSize = cRect.size; // Get the current rect size
                                cRect.size -= new Vector2(elem.BorderOffset * 2, 0); // Subtract the border offset from the size due to some miss calculation
                                DrawElements(elem, elem.GetElements(), true, ref cRect); // Draws the children
                                cRect.size = oSize; // Set the old size back

                                if (drawAsColumn)
                                    rect.y += cRect.size.y + elem.OffsetY; // Add the height of the group
                                else rect.x += cRect.size.x + elem.OffsetX; // Add the width of the group

                                GUI.skin.box.fontSize = oldSize;

                                break;
                            }

                        #endregion
                    }
                }
            }

            Rect r = new Rect(pos, size); // Create the base rect
            DrawElements(null, CurrentTab.GetContent(), true, ref r); // Draw the elements of root
        }

        // Called by unity when the menu should be rendered/drawn
        private static void OnMenuDraw(int id)
        {
            // Check if TabSystem was created succefully
            if (m_tabSystem == null)
            {
                RajceMain.logger.Error("TabSystem failed to initialize");
                return;
            }

            Rect rect = new Rect(5, 20, MenuSize.x - 10, 60); // Create the main rect

            m_tabSystem.OnGUI(ref rect); // Draw the tabbar

            if (CurrentTab != null)
            {
                // + 20 (topbar stuff), + 5 (offset from border)
                Vector2 pagePos = new Vector2(5, rect.height + 20 + 5); // The position to start drawing the page at
                // + 20 (topbar stuff), + 10 (offset)
                Vector2 pageSize = new Vector2(MenuSize.x - 10, MenuSize.y - (rect.height + 20 + 10)); // The size of the page

                Vector2 targetSize = CurrentTab.DrawTab(ReDrawPage); // Draws the page

                // Its faster to just write to the var than checking and then writting
                ReDrawPage = false; // Turn off the redraw cause we have redrawn the page

                // Check if the page tab's page has some minsize set
                if (CurrentTab.MinSize != null)
                {
                    Vector2 normed = CurrentTab.MinSize.Value; // Gets the normalized value
                    Vector2 calced = new Vector2(pageSize.x * normed.x, pageSize.y * normed.y); // Calculates the real size

                    // Check if the real values are bigger than the size required by elements
                    if (targetSize.x < calced.x)
                        targetSize.x = calced.x;
                    if (targetSize.y < calced.y)
                        targetSize.y = calced.y;
                }

                Rect viewRect = new Rect(pagePos, targetSize); // The rect for the page
                m_vScrollPosition = GUI.BeginScrollView(new Rect(pagePos, pageSize), m_vScrollPosition, viewRect); // Create a scrollview so the page can be X times bigger

                DrawContent(pagePos, targetSize); // Draw the page elements

                GUI.EndScrollView(); // End the scroller
            }
        }

        #region Callbacks

        public static void Start()
        {
            AssetBundle bundle = Utils.LoadBundle("Data.assets"); // Loads the asset bundle from the resource

            m_tabSystem = new TabSystem(bundle.LoadAsset<Texture>("Icon")); // Initialize the tab system
            m_tabSystem.OnTabSelected += (name, tab) =>
            {
                CurrentTab = tab; // Set the current tab to be the selected one
            }; 
        }

        public static void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                IsVisible = !IsVisible; // Toggle the window
                Event.current.Use(); // Stop the next input for this key
            }
            if (CursorManager.singleton != null)
                CursorManager.singleton.debuglogopen = IsVisible; // Disable/Enable the player input

            Vector2 CurPos = Input.mousePosition.ToV2(); // Gets the current mouse postion
            CurPos.y = Screen.height - CurPos.y; // Fixes the mouse position to start from the top left instead of the bottom left

            // Window dragging
            {
                // Check if the menu is visible
                if (!IsVisible)
                {
                    m_bDragging = false; // The user cannot drag the menu while its hidden yk
                    m_vLastCurPos = null; // Set the last position to be null

                    return; // No updates need to done
                }

                // Check if the user released the button
                if (Input.GetMouseButtonUp(0))
                {
                    m_bDragging = false; // User stopped holding the topbar
                    m_vLastCurPos = null; // Set the last position to be null

                    return; // No updates need to done
                }

                // Check if the mouse is on the topbar
                if (CurPos.x >= m_rWindow.x && CurPos.x <= m_rWindow.xMax)
                    if (CurPos.y >= m_rWindow.y && CurPos.y <= m_rWindow.y + 20)
                        if (Input.GetMouseButtonDown(0))
                            m_bDragging = true; // User is holding the window

                // Check if the user is holding the window and the button
                if (Input.GetMouseButton(0) && m_bDragging)
                {
                    if (m_vLastCurPos == null)
                        m_vLastCurPos = CurPos;

                    m_rWindow.position += CurPos - m_vLastCurPos.Value; // Calculate the delta and add it to the current position

                    m_vLastCurPos = CurPos;
                }
            }
        }

        public static void OnGUI()
        {
            if (DefaultSkin == null)
                DefaultSkin = GUI.skin.Copy();

            // Check if the menu is visible
            if (!IsVisible)
                return;

            GUI.skin = MenuSkin ?? DefaultSkin;
            GUI.Window(0, m_rWindow, OnMenuDraw, "Rajce - " + m_tabSystem.CurrentTab);
        }

        #endregion
    }
}

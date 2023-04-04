using RajceInternal.Features;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace RajceInternal.Tabs
{
    internal abstract class TabBase
    {
        protected enum TabItemType
        {
            Undefined, // Unknown object

            // Item types
            Label, // Basic GUI.Label
            Button, // Basic GUI.Button
            Checkbox, // Basic GUI.Toggle

            Slider, // Creates a horizontal slider with a text
            FeatureToggle, // A toggle to make rendering features more automatic

            // Item styles
            Row, // A row that contains more objects to render
            Column, // A column that contains more objects to render
            GroupBox, // A group box that contains more Items in it
        }
        protected class TabItem
        {
            public class ItemStyle
            {
                public int FeatureToggleSize = 16;
                public int FeatureDescription = 14;
            }

            public static ItemStyle globStyle = new ItemStyle();

            public TabItemType Type = TabItemType.Undefined; // Type of the item
            public TabItem Parent = null; // The TabItem that this item is attached to (if null then it means its parented to root)
            public List<TabItem> Children = new List<TabItem>(); // The children of this item (this is only used to calculate the total size of an item for ex: group box or row)
            public bool UpdateUIAfterActivation = false; // Should the whole UI update when this item is activated (this is disabled on Style item types)

            public float? MinSizeX = null; // The minimum size on the X (this is only applied to objects that fall under Item Style types)
            public float? MinSizeY = null; // The minimum size on the Y (this is only applied to objects that fall under Item Style types)

            // Should the object center to the given axis
            public bool CenterX = false;
            public bool CenterY = false;

            // Data for the elements to callback, min/max Values etc...
            public bool CurrValue = false;
            public Action<object> OnAction = null;

            // Used for sliders
            public float CurrSliderValue = 0f;
            public float ValueMin = 0f;
            public float ValueMax = 100f;
            public Vector2 SliderSize = new Vector2(100, 10);

            // Data
            public Vector2 Size { get; private set; } = Vector2.zero;

            // Used for basic unity GUI class
            public GUIContent content = new GUIContent();
            public GUIStyle style = null;
            public int? fontSize = null; // Used to override the default size

            // Used for items that require a freature
            public FeatureBase Feature = null;

            public void AddChild(TabItem item) => Children.Add(item);

            /// <summary>
            /// Calculates the size needed for this item to fit correctly (Offsets are only used for item styles)
            /// </summary>
            /// <param name="forceRecalc">Should the size be recalculated</param>
            /// <param name="offsetX">Offset between objects on the X</param>
            /// <param name="offsetY">Offset between objects on the Y</param>
            /// <param name="offsetSides">Offset from the Top, Down, Left, Right borders</param>
            /// <returns>Size for this Item to fit</returns>
            public Vector2 CalcSize(bool forceRecalc = false, float offsetX = 0, float offsetY = 0, float offsetSides = 0)
            {
                if (!forceRecalc && Size != Vector2.zero)
                    return Size;

                Func<Vector2> func = () =>
                {
                    switch (Type)
                    {
                        case TabItemType.Slider:
                            {
                                //TODO: make the calc for this

                                return Vector2.zero;
                            }
                        case TabItemType.FeatureToggle:
                            {
                                //TODO: remake the calc for this
                                
                                return Vector2.zero;
                            }

                        // Style item types
                        case TabItemType.Row:
                            {
                                Vector2 res = Vector2.zero;

                                float tallesItem = 0;
                                for (int i = 0; i < Children.Count; i++)
                                {
                                    Vector2 size = Children[i].CalcSize(true, offsetX, offsetY, offsetSides);
                                    if (size.y > tallesItem)
                                        tallesItem = size.y;

                                    res.x += size.x;
                                    if (!(i == (Children.Count - 1)))
                                        res.x += offsetX;
                                }

                                res.y += tallesItem;

                                // Adjust the size if its too small
                                if (MinSizeX != null && res.x < MinSizeX.Value)
                                    res.x = MinSizeX.Value;
                                if (MinSizeY != null && res.y < MinSizeY.Value)
                                    res.y = MinSizeY.Value;

                                return res;
                            }
                        case TabItemType.Column:
                            {
                                Vector2 res = Vector2.zero;

                                float widesItem = 0;
                                for (int i = 0; i < Children.Count; i++)
                                {
                                    Vector2 size = Children[i].CalcSize(true, offsetX, offsetY, offsetSides);
                                    if (size.x > widesItem)
                                        widesItem = size.x;

                                    res.y += size.y;
                                    if (!(i == (Children.Count - 1)))
                                        res.y += offsetY;
                                }

                                res.x += widesItem;

                                // Adjust the size if its too small
                                if (MinSizeX != null && res.x < MinSizeX.Value)
                                    res.x = MinSizeX.Value;
                                if (MinSizeY != null && res.y < MinSizeY.Value)
                                    res.y = MinSizeY.Value;

                                return res;
                            }
                        case TabItemType.GroupBox:
                            {
                                Vector2 gbox;
                                if (fontSize != null)
                                    style.fontSize = fontSize.Value;
                                gbox = style.CalcSize(content);

                                Vector2 res = new Vector2(offsetSides * 2, gbox.y + offsetSides * 2); // *2 cause we add the offset to every border side

                                float widesItem = 0f;
                                for (int i = 0; i < Children.Count; i++)
                                {
                                    Vector2 size = Children[i].CalcSize(true, offsetX, offsetY, offsetSides); // Calc the size of the child object
                                    if (size.x > widesItem)
                                        widesItem = size.x; // Set this X size to be the biggest

                                    res.y += size.y; // Add the size to the total size
                                    if (!(i == (Children.Count - 1)))
                                        res.y += offsetY; // Add the spacer to the total size
                                }

                                if (widesItem >= gbox.x)
                                    res.x += widesItem; // widesItem is wider than the group box it self
                                else res.x += gbox.x; // groub box is wider than the wides item

                                // Adjust the size if its too small
                                if (MinSizeX != null && res.x < MinSizeX.Value)
                                    res.x = MinSizeX.Value;
                                if (MinSizeY != null && res.y < MinSizeY.Value)
                                    res.y = MinSizeY.Value;

                                return res;
                            }

                        // For basic unity GUI class
                        default:
                            {
                                if (fontSize == null)
                                    return style.CalcSize(content);

                                style.fontSize = fontSize.Value;
                                return style.CalcSize(content);
                            }
                    }
                };

                return Size = (func());
            }
        }

        private bool _updateRequired = false;
        private List<TabItem> _itemsToRender = new List<TabItem>();
        private Vector2 _requiredSize = Vector2.zero;
        private TabItem currentParent = null;

        public const float OFFSET_FROM_SIDES = 10;

        public const float OFFSET_BETWEEN_ELEMENTS_X = 15;
        public const float OFFSET_BETWEEN_ELEMENTS_Y = 5;

        // Name of the tab
        public abstract string Name { get; protected set; }
        // Normalized value on X and Y so basically 0 .. 1 value
        public virtual Vector2? MinSize { get; protected set; } = null;

        /* Gives info if the TargetSize is absolute or multiplied
        public abstract bool IsSizeMultiplier { get; protected set; }

        // TargetSize for the viewRect
        public abstract Vector2 TargetSize { get; protected set; }*/

        #region Utils

        private Rect SetSize(ref Rect rect, Vector2 vec)
        {
            rect.size = vec;

            return rect;
        }

        #endregion

        private void AddChildToCurrentTabItem(TabItem item)
        {
            if (currentParent != null)
            {
                item.Parent = currentParent;
                currentParent.AddChild(item); // Add the item to the parent

                return;
            }

            _itemsToRender.Add(item);
        }

        /*protected void DrawToggle(ref Rect rect, FeatureBase feature)
        {
            GUIContent content = new GUIContent()
            {
                text = feature.Name,
            };
            GUIContent content0 = new GUIContent()
            {
                text = feature.Description
            };

            GUI.skin.toggle.fontSize = 16;
            Vector2 vec = GUI.skin.toggle.CalcSize(content);
            rect.width = vec.x;
            rect.height = vec.y;

            feature.IsActive = GUI.Toggle(rect, feature.IsActive, content);
            rect.y += 2;
            rect = rect.GetNew(false);

            GUI.skin.label.fontSize = 14;
            Vector2 vec0 = GUI.skin.label.CalcSize(content0);
            rect.width = vec0.x;
            rect.height = vec0.y;

            GUI.Label(rect, content0);
        }
        protected Color DrawColorPicker(Rect rect, Color col)
        {
            Texture2D tex = new Texture2D(40, 40);

            GUI.skin.label.fontSize = 12;
            GUILayout.BeginArea(rect, "", "Box");

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("R", GUILayout.Width(10));
            col.r = GUILayout.HorizontalSlider(col.r * 255, 0, 255) / 255f;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("G", GUILayout.Width(10));
            col.g = GUILayout.HorizontalSlider(col.g * 255, 0, 255) / 255f;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("B", GUILayout.Width(10));
            col.b = GUILayout.HorizontalSlider(col.b * 255, 0, 255) / 255f;
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box", GUILayout.Width(44), GUILayout.Height(44));

            Color tmp = GUI.color;
            GUI.color = col;
            GUILayout.Label(tex);
            GUI.color = tmp;

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            return col;
        }*/

        #region Tab Items

        protected TabItem DrawLabel(string text, int fontSize = 12)
        {
            TabItem item = new TabItem()
            {
                Type = TabItemType.Label,

                content = new GUIContent(text),
                style = GUI.skin.label.Copy(),

                fontSize = fontSize
            };

            AddChildToCurrentTabItem(item); // Adds the object to a parent or adds it to the root

            return item;
        }
        protected TabItem DrawButton(string text, Action<object> onClick, int fontSize = 12)
        {
            TabItem item = new TabItem()
            {
                Type = TabItemType.Button,

                content = new GUIContent(text),
                style = GUI.skin.button.Copy(),

                fontSize = fontSize,
                OnAction = onClick
            };

            AddChildToCurrentTabItem(item);

            return item;
        }
        protected TabItem DrawToggle(string text, bool v, Action<object> onToggle, int fontSize = 12, bool updateUIOnActivation = false)
        {
            TabItem item = new TabItem()
            {
                Type = TabItemType.Checkbox,

                content = new GUIContent(text),
                style = GUI.skin.toggle.Copy(),

                UpdateUIAfterActivation = updateUIOnActivation,

                fontSize = fontSize,

                CurrValue = v,
                OnAction = onToggle,
            };

            AddChildToCurrentTabItem(item);

            return item;
        }
        protected TabItem DrawToggle(FeatureBase feature, Action<object> onToggle = null, int? fontSize = null, bool updateUIOnActivation = false)
        {
            TabItem item = new TabItem()
            {
                Type = TabItemType.FeatureToggle,

                UpdateUIAfterActivation = updateUIOnActivation,

                fontSize = fontSize,
                Feature = feature,

                OnAction = onToggle
            };

            AddChildToCurrentTabItem(item);

            return item;
        }
        protected TabItem DrawSlider(string text, float val, Action<object> onSlide, float min = 0f, float max = 100f, int? fontSize = 14, Vector2? sliderSize = null)
        {
            TabItem item = new TabItem()
            {
                Type = TabItemType.Slider,

                content = new GUIContent(text),
                style = GUI.skin.label.Copy(),
                fontSize = fontSize,

                OnAction = onSlide,

                CurrSliderValue = val,
                ValueMin = min,
                ValueMax = max,
                SliderSize = sliderSize ?? new Vector2(100, 10),
            };

            AddChildToCurrentTabItem(item);

            return item;
        }
        protected TabItem DrawRadioButton(ref List<TabItem> radios, string text, Action<object> onAction, int fontSize = 12)
        {
            if (currentParent == null || currentParent.Type != TabItemType.GroupBox)
                throw new ApplicationException($"Cannot add a radio button to '{currentParent.Type}'");

            if (radios == null)
                radios = new List<TabItem>();

            foreach (TabItem item in radios)
                if (item.content.text == text)
                {
                    AddChildToCurrentTabItem(item);
                    return item;
                }

            List<TabItem> _radios = radios;
            TabItem radio = DrawToggle(text, false, (t) =>
            {
                if (onAction != null)
                    onAction(t);

                if ((bool)t == false)
                    return; // Skip if the toggle is deactivated

                foreach (TabItem item in _radios)
                    if (item.content.text != text)
                    {
                        item.CurrValue = false;
                        if (item.OnAction != null)
                            item.OnAction(false);
                    }

            }, fontSize);
            radios.Add(radio);

            return radio;

            /*TabItem item = new TabItem()
            {
                Type = TabItemType.RadioButton,

                content = new GUIContent(text),
                style = GUI.skin.toggle.Copy(),

                fontSize = fontSize,

                OnAction = onAction
            };

            AddChildToCurrentTabItem(item);

            return item;*/
        }

        // Item Styles

        protected TabItem BeginRow(float? minX = null, float? minY = null)
        {
            TabItem item = new TabItem()
            {
                Type = TabItemType.Row,

                MinSizeX = minX,
                MinSizeY = minY
            };

            AddChildToCurrentTabItem(item);
            currentParent = item;

            return item;
        }
        protected void EndRow()
        {
            // Check if there is a parent set and check if its a row
            if (currentParent != null && currentParent.Type == TabItemType.Row)
                currentParent = currentParent.Parent; // Ends the current row
        }

        protected TabItem BeginColumn(float? minX = null, float? minY = null)
        {
            TabItem item = new TabItem()
            {
                Type = TabItemType.Column,

                MinSizeX = minX,
                MinSizeY = minY
            };

            AddChildToCurrentTabItem(item);
            currentParent = item;

            return item;
        }
        protected void EndColumn()
        {
            // Check if there is a parent set and check if its a column
            if (currentParent != null && currentParent.Type == TabItemType.Column) 
                currentParent = currentParent.Parent; // Ends the current column
        }

        protected TabItem BeginGroupBox(string headerText = "", int fontSize = 12, float? minX = null, float? minY = null)
        {
            TabItem item = new TabItem()
            {
                Type = TabItemType.GroupBox,
                content = new GUIContent(headerText),
                style = GUI.skin.box.Copy(),

                MinSizeX = minX,
                MinSizeY = minY,

                fontSize = fontSize
            };

            AddChildToCurrentTabItem(item);
            currentParent = item;

            return item;
        }
        protected void EndGroupBox()
        {
            // Check if there is a parent set and check if its a groupbox
            if (currentParent != null && currentParent.Type == TabItemType.GroupBox)
                currentParent = currentParent.Parent; // Ends the current group box
        }

        #endregion

        // Called when the tab should be rendered
        protected abstract void DrawTab();

        // PreRenders the tab aka calculates the size and creates all of the items that will be rendered later with the RenderTab method
        public Vector2 PreRenderTab()
        {
            bool update = false;
            if (_updateRequired || _itemsToRender.Count <= 0 || _requiredSize == Vector2.zero)
            {
                _itemsToRender.Clear(); // Clear all previous items cause we will create new items
                DrawTab(); // Creates/Draws new items

                update = true;
                _updateRequired = false;
            }

            if (!update)
                return _requiredSize;

            Vector2 finalSize = new Vector2(OFFSET_FROM_SIDES + 1, OFFSET_FROM_SIDES);

            float columnsSize = 0;
            foreach (TabItem item in _itemsToRender)
            {
                Vector2 size = item.CalcSize(true, OFFSET_BETWEEN_ELEMENTS_X, OFFSET_BETWEEN_ELEMENTS_Y, offsetSides: OFFSET_FROM_SIDES / 2);
                if (item.Type == TabItemType.Column)
                    columnsSize += size.x;
                else if (finalSize.x < size.x)
                    finalSize.x = size.x;

                finalSize.y += size.y;
            }
            finalSize.x += columnsSize;

            return _requiredSize = finalSize;
        }

        // When called it will render all of the items as unity GUI
        public void RenderTab(Vector2 pos, Vector2 size)
        {
            pos.x += OFFSET_FROM_SIDES + 1;
            pos.y += OFFSET_FROM_SIDES;

            // Making an insider function cause recursion
            void DrawItems(TabItem parent, List<TabItem> items, bool drawAsColumn, ref Rect rect)
            {
                Vector2 rSize = rect.size; // Used to center items

                foreach (TabItem item in items)
                {
                    switch (item.Type)
                    {
                        case TabItemType.Label:
                            {
                                GUI.skin.label = item.style; // Sets the label style
                                if (item.fontSize != null)
                                    GUI.skin.label.fontSize = item.fontSize.Value; // Set the fontSize

                                Vector2 eSize = item.CalcSize();
                                Rect oRect = rect.Copy();

                                if (item.CenterX)
                                    oRect.x += (rSize.x / 2) - (eSize.x / 2);
                                if (item.CenterY)
                                    oRect.y += (rSize.y / 2) - (eSize.y / 2);

                                GUI.Label(SetSize(ref oRect, eSize), item.content); // Renders the label

                                // Add the space between elems
                                if (!item.CenterY)
                                    if (drawAsColumn)
                                        rect.position += new Vector2(0, oRect.size.y + OFFSET_BETWEEN_ELEMENTS_Y);
                                    else rect.position += new Vector2(oRect.size.x + OFFSET_BETWEEN_ELEMENTS_X, 0); 

                                //AddSize(ref rect); // Adds the size to the position

                                break;
                            }
                        case TabItemType.Button:
                            {
                                GUI.skin.button = item.style;
                                if (item.fontSize != null)
                                    GUI.skin.button.fontSize = item.fontSize.Value;

                                if (GUI.Button(SetSize(ref rect, item.CalcSize()), item.content) && item.OnAction != null)
                                    item.OnAction(true);

                                if (drawAsColumn)
                                    rect.position += new Vector2(0, rect.size.y + OFFSET_BETWEEN_ELEMENTS_Y);
                                else rect.position += new Vector2(rect.size.x + OFFSET_BETWEEN_ELEMENTS_X, 0);

                                break;
                            }
                        case TabItemType.Checkbox:
                            {
                                GUI.skin.toggle = item.style;
                                if (item.fontSize != null)
                                    GUI.skin.toggle.fontSize = item.fontSize.Value;

                                bool curr = item.CurrValue;
                                curr = GUI.Toggle(SetSize(ref rect, item.CalcSize()), curr, item.content);
                                if (curr != item.CurrValue && item.OnAction != null)
                                {
                                    item.OnAction(item.CurrValue = curr);
                                    if (item.UpdateUIAfterActivation)
                                        _updateRequired = true;
                                }

                                if (drawAsColumn)
                                    rect.position += new Vector2(0, rect.size.y + OFFSET_BETWEEN_ELEMENTS_Y);
                                else rect.position += new Vector2(rect.size.x + OFFSET_BETWEEN_ELEMENTS_X, 0);

                                break;
                            }
                        case TabItemType.Slider:
                            {
                                GUI.skin.label = item.style;
                                if (item.fontSize != null)
                                    GUI.skin.label.fontSize = item.fontSize.Value;

                                Rect oRect = rect.Copy();
                                Vector2 oSize = GUI.skin.label.CalcSize(item.content);

                                GUI.Label(SetSize(ref oRect, oSize), item.content);
                                oRect.position += new Vector2(0, oSize.y += 2);

                                float curr = item.CurrSliderValue;
                                curr = GUI.HorizontalSlider(SetSize(ref oRect, item.SliderSize), curr, item.ValueMin, item.ValueMax);
                                if (curr != item.CurrSliderValue && item.OnAction != null)
                                    item.OnAction(item.CurrSliderValue = curr);

                                oSize.y += oRect.size.y;
                                if (oSize.x < oRect.size.x)
                                    oSize.x = oRect.size.x;

                                oRect.position += new Vector2(oRect.size.x + 5, -2);
                                oSize.x += 5;

                                GUI.skin.label.fontSize = (int)item.SliderSize.y;
                                GUIContent c = new GUIContent(curr.ToString());
                                Vector2 size2 = GUI.skin.label.CalcSize(c);
                                oSize.x += size2.x;

                                GUI.Label(SetSize(ref oRect, size2), c);

                                if (drawAsColumn)
                                    rect.position += new Vector2(0, oSize.y + OFFSET_BETWEEN_ELEMENTS_Y);
                                else rect.position += new Vector2(oSize.x + OFFSET_BETWEEN_ELEMENTS_X, 0);

                                break;
                            }
                        
                        case TabItemType.FeatureToggle:
                            {
                                // Keybind maybe pointless rn cause nothing will really use it

                                // This will be a toggle with the name of the feature and the description under it with a bind
                                // Bind when double clicked will change how it will work from hold -> toggle
                                // The Feature toggle only for if the feature even should be used
                                // Interval for the double click should be 0.5

                                GUIStyle oldToggle = GUI.skin.toggle.Copy();
                                GUIStyle oldLabel = GUI.skin.label.Copy();

                                GUIStyle toggle = GUI.skin.toggle;
                                GUIStyle label = GUI.skin.label;

                                if (item.fontSize != null)
                                {
                                    toggle.fontSize = item.fontSize.Value;
                                    label.fontSize = item.fontSize.Value - 2;
                                }
                                else
                                {
                                    toggle.fontSize = TabItem.globStyle.FeatureToggleSize;
                                    label.fontSize = TabItem.globStyle.FeatureDescription;
                                }

                                FeatureBase feature = item.Feature;
                                bool curr = feature.IsActive;
                                Rect oRect = rect.Copy();
                                Vector2 oSize = Vector2.zero;

                                GUIContent c = new GUIContent(feature.Name);

                                oSize = toggle.CalcSize(c);
                                feature.IsActive = GUI.Toggle(SetSize(ref oRect, oSize), feature.IsActive, c);
                                if (curr != feature.IsActive)
                                {
                                    if (item.OnAction != null)
                                        item.OnAction(feature.IsActive);

                                    if (item.UpdateUIAfterActivation)
                                        _updateRequired = true;
                                }

                                oRect.position += new Vector2(0, oSize.y += 2);
                                //oSize.y += 2;

                                c = new GUIContent(feature.Description);

                                Vector2 size2 = label.CalcSize(c);
                                oSize.y += size2.y;
                                
                                if (oSize.x < size2.x)
                                    oSize.x = size2.x;

                                GUI.Label(SetSize(ref oRect, size2), c);

                                if (drawAsColumn)
                                    rect.position += new Vector2(0, oSize.y + OFFSET_BETWEEN_ELEMENTS_Y);
                                else rect.position += new Vector2(oSize.x + OFFSET_BETWEEN_ELEMENTS_X, 0);

                                GUI.skin.toggle = oldToggle;
                                GUI.skin.label = oldLabel;

                                break;
                            }

                        // Styles
                        case TabItemType.Row:
                            {
                                SetSize(ref rect, item.CalcSize()); // Set the rect size

                                Rect rect2 = rect.Copy();
                                DrawItems(item, item.Children, false, ref rect2);

                                if (drawAsColumn)
                                    rect.position += new Vector2(0, rect.size.y);
                                else rect.position += new Vector2(rect.size.x, 0);

                                break;
                            }
                        case TabItemType.Column:
                            {
                                SetSize(ref rect, item.CalcSize());

                                Rect rect2 = rect.Copy();
                                DrawItems(item, item.Children, true, ref rect2);

                                if (drawAsColumn)
                                    rect.position += new Vector2(0, rect.size.y);
                                else rect.position += new Vector2(rect.size.x, 0);

                                break;
                            }
                        case TabItemType.GroupBox:
                            {
                                GUI.skin.box = item.style; // Set the style we use for the box
                                if (item.fontSize != null)
                                    GUI.skin.box.fontSize = item.fontSize.Value; // Set the fontSize

                                Vector2 eSize = item.CalcSize();
                                Rect oRect = rect.Copy();
                                if (item.CenterX)
                                    oRect.x += (rSize.x / 2) - (eSize.x / 2);

                                //Console.WriteLine("Center: {0}", item.CenterX);

                                GUI.Box(SetSize(ref oRect, item.CalcSize()), item.content); // Render the box

                                oRect.position += new Vector2(OFFSET_FROM_SIDES / 2, GUI.skin.box.fontSize + OFFSET_FROM_SIDES + 1);
                                DrawItems(item, item.Children, true, ref oRect); // Render the children

                                if (drawAsColumn)
                                    rect.position += new Vector2(0, oRect.size.y + OFFSET_BETWEEN_ELEMENTS_Y); // Add the size of the group box to the position and add the spacing
                                else rect.position += new Vector2(oRect.size.x + OFFSET_BETWEEN_ELEMENTS_X, 0);

                                break;
                            }
                    }
                }
            }

            Rect r = new Rect(pos, size);
            DrawItems(null, _itemsToRender, true, ref r);
        }
    }
}
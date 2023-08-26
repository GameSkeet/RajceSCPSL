using MelonRajce.Features;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace MelonRajce.UI
{
    internal abstract class UITab
    {
        private List<Menu.Element> Content = new List<Menu.Element>(); // Hold every element that should be drawn
        private Menu.Element CurrentParent = null; // The element that should hold new elements

        private Vector2 CachedSize = Vector2.zero; // Size of the tab

        protected float OFFSET_FROM_BORDERS = 5f; // Offset from the borders

        public virtual Vector2? MinSize { get; protected set; } = null; // The minimum size for the page of this tab (This size is nomalized 0 ... 1)

        #region Draw methods

        // Adds a child to a parent if a parent is set otherwise it will be added to the root
        private void AddChildToCurrentElement(Menu.Element child)
        {
            if (CurrentParent != null)
            {
                child.Parent = CurrentParent; // Set the parent
                CurrentParent.AddChild(child); // Add the child to the parent

                return;
            }

            Content.Add(child); // Add the child to the root
        }

        // Draws a label
        protected Menu.Element DrawLabel(string text, int fontSize = 12)
        {
            Menu.Element elem = new Menu.Element()
            {
                Type = Menu.ElementType.Label, // Set the element type to label

                Content = new GUIContent(text), // Create the content for the label
                UsedStyle = GUI.skin.label,
                FontSize = fontSize // Add the label font size
            };

            AddChildToCurrentElement(elem); // Adds the label to the parent or page content

            return elem; // returns the element
        }
        // Draws a button
        protected Menu.Element DrawButton(string text, Action<Menu.Element> onClick, int fontSize = 12)
        {
            Menu.Element elem = null;
            elem = new Menu.Element()
            {
                Type = Menu.ElementType.Button, // Set the element type

                Content = new GUIContent(text), // Set the button content
                UsedStyle = GUI.skin.button,
                FontSize = fontSize, // Set custom fontsize

                OnAction = (o) =>
                {
                    // Check if the onClick is set
                    if (onClick != null)
                        onClick(elem); // Invoke the onClick
                } // Called when the button is clicked
            };

            AddChildToCurrentElement(elem); // Add the element to a parent or page content

            return elem; // Return the element
        }
        // Draws a checkbox/toggle
        protected Menu.Element DrawToggle(string text, bool v, Action<Menu.Element, bool> onToggle, int fontSize = 12, bool redrawOnAction = false)
        {
            Menu.Element elem = null;
            elem = new Menu.Element()
            {
                Type = Menu.ElementType.Checkbox, // Set the element type
                RedrawUIOnAction = redrawOnAction, // Set if the page should redraw after toggle has been un/checked

                Content = new GUIContent(text), // Set the toggle content
                UsedStyle = GUI.skin.toggle,
                FontSize = fontSize, // Set custom fontsize

                ToggleCurrentValue = v, // Set the default toggle value
                OnAction = (o) =>
                {
                    // Check if the onToggle is set
                    if (onToggle != null)
                        onToggle(elem, (bool)o); // Invoke the onToggle
                } // Called when the toggle is un/checked
            };

            AddChildToCurrentElement(elem); // Add the element to a parent or page content

            return elem; // Return the element
        }

        // Draws a slider
        protected Menu.Element DrawSlider(string text, float val, Action<Menu.Element, float> onSlide, float min = 0f, float max = 100f, int? fontSize = 12, Vector2? sliderSize = null, bool wholeNumbers = false)
        {
            Menu.Element elem = null;
            elem = new Menu.Element()
            {
                Type = Menu.ElementType.Slider, // Set the element type

                Content = new GUIContent(text), // Create the content for the slider
                UsedStyle = GUI.skin.horizontalSlider,
                FontSize = fontSize, // Set the custom fontsize

                OnAction = (o) =>
                {
                    // Check if onSlide is set
                    if (onSlide != null)
                        onSlide(elem, (float)o); // Invoke onSlide with the current value
                },

                SliderCurrentValue = val, // Set the current value to val
                SliderMin = min, // Set the minimum value
                SliderMax = max, // Set the maximum value
                SliderSize = sliderSize ?? new Vector2(100, 10), // Set the slider size
                WholeNumberSlider = wholeNumbers, // Should the slider only have whole numbers
            };

            AddChildToCurrentElement(elem); // Add the element to a parent or page content

            return elem; // Return the element
        }
        protected Menu.Element DrawFeature(Feature feature, Action<Menu.Element, bool> onToggle = null, int? headerTextSize = 16, bool redrawOnAction = false)
        {
            Menu.Element elem = null;
            elem = new Menu.Element()
            {
                Type = Menu.ElementType.FeatureToggle, // Set the element type
                RedrawUIOnAction = redrawOnAction, // Should the page redraw after activation

                FontSize = headerTextSize ?? 16, // Set custom fontsize for the toggle

                OnAction = (o) =>
                {
                    // Check if onToggle isn't null
                    if (onToggle != null)
                        onToggle(elem, (bool)o); // Invoke the onToggle
                }, // Handle the OnAction

                Feature = feature, // Set the feature so we can use it later
            };

            AddChildToCurrentElement(elem); // Add the element to a parent or page content

            return elem; // Returns the element
        }
        // Draws a radio button
        protected Menu.Element DrawRadioButton(ref List<Menu.Element> radios, string text, Action<Menu.Element, bool> onToggle, int fontSize = 12, bool toggleFromStart = false)
        {
            if (CurrentParent == null || CurrentParent.Type != Menu.ElementType.Group)
                throw new ApplicationException($"Cannot add a radio to '{CurrentParent.Type}'");

            if (radios == null)
                radios = new List<Menu.Element>(); // Init the list

            foreach (var elem in radios)
                if (elem.Content.text == text)
                {
                    AddChildToCurrentElement(elem); // Add the existing element
                    return elem; // return the element
                }

            if (toggleFromStart)
                foreach (var elem in radios)
                    elem.ToggleCurrentValue = false; // Toggle the toggle off

            List<Menu.Element> _radios = radios;
            Menu.Element radio = null;
            radio = DrawToggle(text, toggleFromStart, (self, t) =>
            {
                // Check if onToggle isn't null
                if (onToggle != null)
                    onToggle(radio, t); // Invoke onToggle

                // Check if the toggle is deactivated
                if (t == false)
                    return; // Skip if the toggle is deactivated

                foreach (var elem in _radios)
                    if (elem.Content.text != text)
                    {
                        elem.ToggleCurrentValue = false;  // Disable other radio button

                        // Check if OnAction isn't null
                        if (elem.OnAction != null)
                            elem.OnAction(false); // Invoke the OnAction of the other element
                    }

            }, fontSize);
            radios.Add(radio); // Add the radio button to the list

            return radio; // Return the radio button
        }

        // Add padding to the current parent or page
        protected Menu.Element AddPadding(float padX, float padY)
        {
            Menu.Element elem = new Menu.Element()
            {
                Type = Menu.ElementType.Padding, // Set the element type

                PaddingX = padX, // Set the padding for X
                PaddingY = padY, // Set the padding for Y
            };

            AddChildToCurrentElement(elem); // Add the element to a parent or page content

            return elem; // Return the element
        }

        // Begins a new row
        protected Menu.Element BeginRow(float? minX = null, float? minY = null)
        {
            Menu.Element elem = new Menu.Element()
            {
                Type = Menu.ElementType.Row, // Set the element type

                MinSizeX = minX, // Set the min size for X
                MinSizeY = minY, // Set the min size for Y
            };

            AddChildToCurrentElement(elem); // Add the element to a parent or page content
            CurrentParent = elem; // Set the current parent to this element

            return elem; // Return the element
        }
        // Ends the current row
        protected void EndRow()
        {
            // Check if there is a parent set and check if its a row
            if (CurrentParent != null && CurrentParent.Type == Menu.ElementType.Row)
                CurrentParent = CurrentParent.Parent; // Ends the current row
        }

        // Begin a new column
        protected Menu.Element BeginColumn(float? minX = null, float? minY = null)
        {
            Menu.Element elem = new Menu.Element()
            {
                Type = Menu.ElementType.Column, // Set the element type

                MinSizeX = minX, // Set the min size for X
                MinSizeY = minY, // Set the min size for Y
            };

            AddChildToCurrentElement(elem); // Add the element to a parent or page content
            CurrentParent = elem; // Set the current parent to this element

            return elem; // Return the element
        }
        // Ends the current column
        protected void EndColumn()
        {
            // Check if there is a parent set and check if its a column
            if (CurrentParent != null && CurrentParent.Type == Menu.ElementType.Column)
                CurrentParent = CurrentParent.Parent; // Ends the current column
        }

        protected Menu.Element BeginGroup(string header = "", int fontSize = 12, float? minX = null, float? minY = null)
        {
            Menu.Element elem = new Menu.Element()
            {
                Type = Menu.ElementType.Group, // Set the element type

                MinSizeX = minX, // Set the min size for the X
                MinSizeY = minY, // Set the min size for the Y

                Content = new GUIContent(header), // Create the content for the group
                FontSize = fontSize, // Set the custom fontsize
            };

            AddChildToCurrentElement(elem); // Add the element to a parent or page content
            CurrentParent = elem; // Set the current parent to this element

            return elem; // Return the element
        }
        protected void EndGroup()
        {
            // Check if there is a parent set and check if its a group
            if (CurrentParent != null && CurrentParent.Type == Menu.ElementType.Group)
                CurrentParent = CurrentParent.Parent; // Ends the current group
        }

        #endregion

        // Draws the tab elements
        protected abstract void OnDraw();

        // "Draws" the tab and receives the required size without MinSize being applied
        public Vector2 DrawTab(bool forceUpdate = false)
        {
            bool update = false; // Should the tab update
            if (forceUpdate || Content.Count <= 0 || CachedSize == Vector2.zero)
            {
                Content.Clear(); // Clear the elements

                OnDraw(); // Draws the elements

                update = true;
            }

            // Check if we need to update
            if (!update)
                return CachedSize; // Return the cached size

            Vector2 finalSize = Vector2.zero;
            float columnSize = 0f;

            foreach (Menu.Element elem in Content)
            {
                Vector2 size = elem.CalcSize(true); // Calculate the size of the element

                // Check if the element is a column
                if (elem.Type == Menu.ElementType.Column)
                    columnSize += size.x; // Add the column width to the total column width
                else if (finalSize.x < size.x)
                    finalSize.x = size.x; // Add the element width to the total

                finalSize.y += size.y; // Add the element height
            }

            finalSize.x += columnSize; // Add the column width to the total

            return CachedSize = finalSize; // Cache the size
        }

        // Get all of the elements that should be drawn
        public Menu.Element[] GetContent() => Content.ToArray();
    }
}

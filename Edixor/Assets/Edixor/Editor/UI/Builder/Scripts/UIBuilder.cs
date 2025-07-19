using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ExTools
{
    public static class UIBuilder
    {
        public static VisualElement CreateSearchBar<T>(
            List<T> source,
            Func<T, string> keySelector,
            Action<List<T>> onFiltered,
            string placeholder = "Enter title to search",
            IEnumerable<SearchExtraAction> extraActions = null
        )
        {
            VisualElement outerContainer = new VisualElement();
            outerContainer.name = "search-bar";
            outerContainer.style.flexDirection = FlexDirection.Column;

            VisualElement innerContainer = new VisualElement();
            innerContainer.AddToClassList("search-bar");

            StyleSheet styleSheet = EdixorObjectLocator.LoadObject<StyleSheet>("Builder/Uss/SearchBar.uss");
            if (styleSheet != null)
                outerContainer.styleSheets.Add(styleSheet);

            VisualElement iconWrapper = new VisualElement();
            iconWrapper.AddToClassList("search-bar-icon");

            Image searchIcon = new Image();
            searchIcon.image = EdixorObjectLocator.LoadObject<Texture2D>("Resources/Images/Icons/search.png");
            iconWrapper.Add(searchIcon);
            innerContainer.Add(iconWrapper);

            VisualElement relativeContainer = new VisualElement();
            relativeContainer.AddToClassList("relative-container");

            TextField searchField = new TextField();
            searchField.label = "";
            searchField.AddToClassList("search-field");
            relativeContainer.Add(searchField);

            Label placeholderLabel = new Label(placeholder);
            placeholderLabel.AddToClassList("search-placeholder");
            placeholderLabel.pickingMode = PickingMode.Ignore;
            relativeContainer.Add(placeholderLabel);

            innerContainer.Add(relativeContainer);

            bool reverse = false;

            Button reverseButton = new Button();
            reverseButton.text = "";
            reverseButton.AddToClassList("reverse-button");

            Image reverseIcon = new Image();
            reverseIcon.image = EdixorObjectLocator.LoadObject<Texture2D>("Resources/Images/Icons/sett.png");
            reverseButton.Add(reverseIcon);
            innerContainer.Add(reverseButton);

            Label errorLabel = new Label();
            errorLabel.AddToClassList("search-error");
            errorLabel.style.display = DisplayStyle.None;

            if (extraActions != null)
            {
                foreach (SearchExtraAction action in extraActions)
                {
                    Button extraButton = new Button();
                    extraButton.AddToClassList("extra-action-button");

                    Image icon = new Image();
                    icon.image = EdixorObjectLocator.LoadObject<Texture2D>(action.IconPath);
                    extraButton.Add(icon);

                    if (!string.IsNullOrEmpty(action.Tooltip))
                        extraButton.tooltip = action.Tooltip;

                    extraButton.clicked += () =>
                    {
                        string query = searchField.value?.Trim() ?? "";
                        bool success = action.Action(query);
                        errorLabel.text = success ? "" : "Invalid path";
                        errorLabel.style.display = success ? DisplayStyle.None : DisplayStyle.Flex;
                    };

                    innerContainer.Add(extraButton);
                }
            }

            Action applyFilter = () =>
            {
                errorLabel.style.display = DisplayStyle.None;

                string query = searchField.value?.Trim().ToLowerInvariant() ?? "";
                List<T> filtered = string.IsNullOrEmpty(query)
                    ? new List<T>(source)
                    : source.Where(item =>
                        !string.IsNullOrEmpty(keySelector(item)) &&
                        keySelector(item).Trim().ToLowerInvariant().Contains(query)
                    ).ToList();

                if (reverse)
                    filtered.Reverse();

                onFiltered?.Invoke(filtered);
            };

            void UpdatePlaceholderVisibility()
            {
                placeholderLabel.visible = string.IsNullOrEmpty(searchField.value);
            }

            searchField.RegisterValueChangedCallback(_ =>
            {
                applyFilter();
                UpdatePlaceholderVisibility();
            });

            reverseButton.clicked += () =>
            {
                reverse = !reverse;
                applyFilter();
            };

            UpdatePlaceholderVisibility();
            applyFilter();

            outerContainer.Add(innerContainer);
            outerContainer.Add(errorLabel);

            return outerContainer;
        }
    }
}

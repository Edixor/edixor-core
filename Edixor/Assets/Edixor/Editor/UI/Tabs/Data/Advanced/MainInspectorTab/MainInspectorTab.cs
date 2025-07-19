using UnityEngine.UIElements;

namespace ExTools.Tabs.Advanced
{
    public class MainInspectorTab : EdixorTab, IInitializableContent
    {
        private VisualElement rootTarget;
        public void InitializeContent(VisualElement parent)
        {
            rootTarget = parent;
        }

        public void Awake()
        {
            tabName = "Main Inspector";
            LoadIcon("Resources/Images/Icons/edix.png");
        }

        public void Start()
        {
            root.Add(rootTarget);
        }
    }
}

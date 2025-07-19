using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using ExTools.Settings;
using ExTools.Controllers;

namespace ExTools
{
    
    [Serializable]
    public abstract class EdixorTab
    {
        [SerializeField] protected string tabName;
        [SerializeField] private int _tabOpens;
        [SerializeField] private string _pathUxml;
        [SerializeField] private string _pathUss;
        [SerializeField] private string _pathIcon;
        [NonSerialized] private Texture2D icon;

        [NonSerialized] protected VisualElement root;
        [NonSerialized] protected VisualElement parentContainer;
        [NonSerialized] protected DIContainer container;
        [NonSerialized] private IControllersFacade _setting;

        private Action _childAwake;
        private Action _childStart;
        private Action _childOnEnable;
        private Action _childOnDisable;
        private Action _childOnDestroy;
        private Action _childUpdate;

        private bool _delegatesSet;
        private string _id;

        public event Action<HotKeyTabInfo> OnHotKeyAdded;

        protected string Id => _id ??= container.ResolveNamed<EdixorRegistrySetting>(ServiceNameKeys.EdixorRegistrySetting).GetCorrectItem().Id;

        public string Title => tabName;
        public Texture2D Icon => icon;
        public int OpenCount => _tabOpens;
        public VisualElement Root => root;
        public string UxmlPath { get; protected set; }
        public string UssPath { get; protected set; }
        public string IconPath { get; protected set; }

        protected IControllersFacade Setting => _setting ??= container.ResolveNamed<IControllersFacade>(
            container.Resolve<ServiceNameResolver>().EdixorControllers
        );

        public static void ShowTab<T>() where T : EdixorTab, new()
        {
            var container = EdixorObjectLocator.LoadObject<DIContainer>("Servers/DIContainer.asset");
            var resolver = container.Resolve<ServiceNameResolver>();
            var controllerKey = resolver.TabController;
            var tabController = container.ResolveNamed<ITabController>(controllerKey);
            tabController.AddTab(typeof(T).AssemblyQualifiedName, true, true);
        }

        public void PrepareMetaDataWithoutUI()
        {
            SetupLifecycleDelegates();
            _childAwake?.Invoke();
        }


        public static TabData CreateTabData(Type logicType)
        {
            var logic = (EdixorTab)Activator.CreateInstance(logicType);
            logic.LoadUxml("auto");
            logic.LoadUss("auto");
            logic.LoadIcon(null);

            var uxml = !string.IsNullOrEmpty(logic.UxmlPath) ? EdixorObjectLocator.LoadObject<VisualTreeAsset>(logic.UxmlPath) : null;
            var uss = !string.IsNullOrEmpty(logic.UssPath) ? EdixorObjectLocator.LoadObject<StyleSheet>(logic.UssPath) : null;
            var icon = !string.IsNullOrEmpty(logic.IconPath) ? EdixorObjectLocator.LoadObject<Texture2D>(logic.IconPath) : null;

            var data = ScriptableObject.CreateInstance<TabData>();
            data.Init(logicType.AssemblyQualifiedName, logic.Title, uxml, uss, icon);
            return data;
        }

        public void Initialize(VisualElement parent, DIContainer cont)
        {
            parentContainer = parent;
            container = cont;
            SetupLifecycleDelegates();
        }

        public void Initialize(TabData data, DIContainer cont, VisualElement parent)
        {
            tabName = data.TabName;
            icon = data.Icon;
            root = data.Uxml?.CloneTree();
            if (root != null)
                root.style.height = Length.Percent(100);

            if (data.Uss != null)
                root?.styleSheets.Add(data.Uss);

            parentContainer = parent;
            parentContainer?.Clear();
            if (root != null)
                parentContainer.Add(root);

            container = cont;
            SetupLifecycleDelegates();
        }

        protected void Option(string title, string uxml = null, string uss = null, string iconPath = null)
        {
            tabName = title;
            LoadUxml(uxml);
            LoadUss(uss);
            LoadIcon(iconPath);
        }

        private void SetupLifecycleDelegates()
        {
            if (_delegatesSet) return;

            var type = GetType();
            _childAwake = CreateDelegate(type, "Awake");
            _childStart = CreateDelegate(type, "Start");
            _childOnEnable = CreateDelegate(type, "OnEnable");
            _childOnDisable = CreateDelegate(type, "OnDisable");
            _childOnDestroy = CreateDelegate(type, "OnDestroy");
            _childUpdate = CreateDelegate(type, "UpdateGUI");

            _delegatesSet = true;
        }

        private Action CreateDelegate(Type type, string methodName)
        {
            var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return method != null && method.GetParameters().Length == 0 && method.ReturnType == typeof(void)
                ? (Action)Delegate.CreateDelegate(typeof(Action), this, method)
                : null;
        }

        public void InvokeAwake()
        {
            parentContainer?.Clear();
            if (root != null)
                parentContainer.Add(root);
            _childAwake?.Invoke();
        }

        public void InvokeStart() => _childStart?.Invoke();
        
        public void InvokeOnEnable()
        {
            parentContainer?.Clear();
            if (root != null)
                parentContainer.Add(root);
            _childOnEnable?.Invoke();
            InitStyleTab(container.ResolveNamed<StyleSetting>(ServiceNameKeys.StyleSetting).GetCorrectItem().GetEdixorParameters());
        }
        public void InvokeOnDisable() => _childOnDisable?.Invoke();

        public void InvokeOnDestroy()
        {
            _childOnDestroy?.Invoke();
            root = null;
        }

        public void InvokeUpdateGUI() => _childUpdate?.Invoke();

        public void LoadUxml(string path = null)
        {
            string resolved = path switch
            {
                "auto" => $"Tabs/{GetType().Name}/{GetType().Name}.uxml",
                "auto-advanced" => $"Tabs Advanced/{GetType().Name}/{GetType().Name}.uxml",
                null or "" => "Tabs Other/Default.uxml",
                _ => path
            };

            var tree = EdixorObjectLocator.LoadObject<VisualTreeAsset>(resolved);
            if (tree == null)
            {
                Debug.LogWarning($"UXML not found: {resolved}, trying fallback.");
                resolved = "Tabs Other/Default.uxml";
                tree = EdixorObjectLocator.LoadObject<VisualTreeAsset>(resolved);
            }

            _pathUxml = resolved;
            UxmlPath = resolved;
            root = tree?.Instantiate();
            if (root != null)
                root.style.height = Length.Percent(100);
        }


        public void LoadUss(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            string resolved = path switch
            {
                "auto" => $"Tabs/{GetType().Name}/{GetType().Name}.uss",
                "auto-advanced" => $"Tabs Advanced/{GetType().Name}/{GetType().Name}.uss",
                _ => path
            };

            var sheet = EdixorObjectLocator.LoadObject<StyleSheet>(resolved);
            if (sheet != null)
            {
                _pathUss = resolved;
                UssPath = resolved;
                root?.styleSheets.Add(sheet);
            }
        }

        public void LoadIcon(string path)
        {
            if (string.IsNullOrEmpty(path))
                path = "Resources/Images/Icons/tabs.png";

            var tex = EdixorObjectLocator.LoadObject<Texture2D>(path);
            icon = tex ?? EdixorObjectLocator.LoadObject<Texture2D>("Resources/Images/Icons/tabs.png");

            _pathIcon = path;
            IconPath = _pathIcon;
        }

        public Texture2D GetMaxIcon()
        {
            if (string.IsNullOrEmpty(_pathIcon)) return null;

            string ext = System.IO.Path.GetExtension(_pathIcon);
            string basePath = _pathIcon[..^ext.Length];
            string maxPath = basePath + "-max" + ext;

            return EdixorObjectLocator.LoadObject<Texture2D>(maxPath) ?? icon;
        }

        public void LoadTabOpens()
        {
            string key = GetType().Name + Id;
            _tabOpens = EditorPrefs.GetInt(key, 0);
        }

        public void AddOpens()
        {
            LoadTabOpens();
            _tabOpens++;
            EditorPrefs.SetInt(GetType().Name + Id, _tabOpens);
        }

        public void DeleteUI()
        {
            InvokeOnDestroy();
            root = null;
        }

        protected void InitStyleTab(EdixorParameters parameters)
        {
            if (parameters == null || root == null) return;

            foreach (ScrollView scroll in root.Query<ScrollView>().ToList())
            {
                scroll.horizontalScroller.AddToClassList("E-Scroll");
                scroll.verticalScroller.AddToClassList("E-Scroll");
            }

            foreach (var entry in parameters.ContentBoxes)
            {
                foreach (var el in root.Query<VisualElement>().Class(entry.Name).ToList())
                    entry.Style.ApplyWithStates(el);
            }

            foreach (var entry in parameters.ContentLabels)
            {
                foreach (var el in root.Query<VisualElement>().Class(entry.Name).ToList())
                    entry.Style.ApplyWithStates(el);
            }

            foreach (var entry in parameters.ContentButtons)
            {
                foreach (var el in root.Query<VisualElement>().Class(entry.Name).ToList())
                    entry.Style.ApplyWithStates(el);
            }

            foreach (var entry in parameters.ScrollStyles)
            {
                foreach (var el in root.Query<VisualElement>().Class(entry.Name).ToList())
                    entry.Style.ApplyScrollStyleWithStates(el);
            }
        }


        public void EnsureTabOpensLoaded()
        {
            if (_tabOpens == 0)
                LoadTabOpens();
        }


        protected void LoadHotKey(string path, Action action = null, string title = null)
        {
            Setting.LoadHotKey(path, title ?? tabName, action);
            OnHotKeyAdded?.Invoke(new HotKeyTabInfo(tabName));
        }
    }
}

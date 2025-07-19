using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ExTools;
using ExTools.Controllers;
using ExTools.Settings;

public class TabController : ITabController
{
    private const int MaxTabs = 50;
    private readonly DIContainer _container;
    private readonly TabUIFactory tabUIFactory;
    private IUIController uiBase;
    private TabSetting tabSetting;
    private List<TabData> tabDatas = new();
    private Dictionary<TabData, EdixorTab> logicInstances = new();
    private Dictionary<TabData, VisualElement> tabContainers = new();
    private int indexTab = -1;
    private bool _isRestoring;
    private TabData basicTab;
    

    public TabController(DIContainer container)
    {
        _container = container;
        tabUIFactory = new TabUIFactory();
    }

    private IUIController UIBase
    {
        get
        {
            if (uiBase == null)
                uiBase = _container.ResolveNamed<IUIController>(_container.Resolve<ServiceNameResolver>().UIController);
            return uiBase;
        }
    }

    public void Initialize(IUIController uiBase = null, TabSetting tabSetting = null)
    {
        this.uiBase = uiBase;
        this.tabSetting = tabSetting;
        var addBtn = (Button)UIBase.GetElement("AddTab");
        if (addBtn != null)
        {
            addBtn.clicked -= OnAddTabClicked;
            addBtn.clicked += OnAddTabClicked;
        }
    }

    public void SetBasicTab(TabData tab)
    {
        basicTab = tab;
    }

    private void OnAddTabClicked()
    {
        if (basicTab != null)
            AddTab(basicTab.LogicTypeName, true, true);
    }

    public void RestoreTabs()
    {
        _isRestoring = true;
        ClearAll();
        string folderPath = EdixorObjectLocator.Resolve($"WindowStates/{tabSetting.EdixorId}/SaveTab");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        string assetFolder = folderPath.Replace(Application.dataPath, "Assets");
        var guids = AssetDatabase.FindAssets("t:TabData", new[] { assetFolder });

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<TabData>(path);
            if (data == null) continue;

            try
            {
                AddTab(data, false, false);
            }
            catch (Exception ex)
            {
                ExDebug.LogError($"Error restoring tab '{data?.TabName ?? "Unknown"}':\n{ex}");
            }
        }

        if (tabDatas.Count == 0 && basicTab != null)
            AddTab(basicTab.LogicTypeName, true, true);

        if (tabDatas.Count > 0)
        {
            int idx = Mathf.Clamp(tabSetting.GetActiveIndex(), 0, tabDatas.Count - 1);
            SwitchTab(idx);
        }

        tabSetting.SetTabData(tabDatas);
        _isRestoring = false;
    }

    public void AddTab(string logicTypeName, bool saveState = true, bool autoSwitch = true)
    {
        var type = Type.GetType(logicTypeName);
        if (type == null) return;
        var data = CreateAndSaveTabDataAsset(type, $"{type.Name}_{Guid.NewGuid():N}");
        AddTab(data, saveState, autoSwitch);
    }

    public void AddTab(TabData data, bool saveState = true, bool autoSwitch = true)
    {
        if (data == null || tabDatas.Count >= MaxTabs) return;
        var logic = CreateLogic(data);
        if (logic == null) return;
        tabDatas.Add(data);
        logicInstances[data] = logic;
        if (autoSwitch) SwitchTab(tabDatas.IndexOf(data));
        RebuildTabsUI(data);
        if (saveState && !_isRestoring)
            tabSetting.SetTabData(tabDatas);
    }

    public void AddTab(EdixorTab logic, bool saveState = true, bool autoSwitch = true)
    {
        if (logic == null) return;
        var data = CreateAndSaveTabDataAsset(logic.GetType(), $"{logic.Title}_{Guid.NewGuid():N}");
        if (tabDatas.Count >= MaxTabs) return;
        tabDatas.Add(data);
        logicInstances[data] = logic;
        logic.Initialize(data, _container, UIBase.GetElement("middle-section-content"));
        if (autoSwitch) SwitchTab(tabDatas.IndexOf(data));
        RebuildTabsUI(data);
        if (saveState && !_isRestoring)
            tabSetting.SetTabData(tabDatas);
    }

    public void SwitchTab(int index)
    {
        ExDebug.Log($"SwitchTab entered: index={index}, tabCount={tabDatas.Count}, currentIndex={indexTab}");
        if (index < 0 || index >= tabDatas.Count)
        {
            ExDebug.Log($"SwitchTab exit: invalid index {index}");
            return;
        }
        if (indexTab >= 0)
            logicInstances[tabDatas[indexTab]].InvokeOnDisable();
        indexTab = index;
        var section = UIBase.GetElement("tab-section");
        var content = UIBase.GetElement("middle-section-content");
        var p = _container.ResolveNamed<StyleSetting>(ServiceNameKeys.StyleSetting)
                        .GetCorrectItem()
                        .GetEdixorParameters();
        var normalStyle = p.GetStyleByName(p.Tabs, "normal");
        var activeStyle = p.GetStyleByName(p.Tabs, "active");
        for (int i = 0; i < tabDatas.Count; i++)
        {
            if (!tabContainers.TryGetValue(tabDatas[i], out var elem))
            {
                ExDebug.LogError($"SwitchTab: no container for {tabDatas[i].TabName}");
                continue;
            }
            (i == indexTab ? activeStyle : normalStyle).ApplyWithStates(elem);
        }
        content.Clear();
        var logic = logicInstances[tabDatas[indexTab]];
        if (logic.Root != null)
            content.Add(logic.Root);
        logic.InvokeOnEnable();
        tabSetting.SetActiveIndex(indexTab);
        ExDebug.Log($"SwitchTab done: now index={indexTab}, tab={tabDatas[indexTab].TabName}");
    }

    public void CloseTab(int index)
    {
        ExDebug.Log($"CloseTab entered: index={index}, tabCount={tabDatas.Count}");
        if (index < 0 || index >= tabDatas.Count)
        {
            ExDebug.Log($"CloseTab exit: invalid index {index}");
            return;
        }
        var data = tabDatas[index];
        ExDebug.Log($"CloseTab: removing {data.TabName}");
        var logic = logicInstances[data];
        logic.InvokeOnDisable();
        logic.InvokeOnDestroy();
        UIBase.GetElement("tab-section").Remove(tabContainers[data]);
        logicInstances.Remove(data);
        tabContainers.Remove(data);
        tabDatas.RemoveAt(index);
        var assetPath = AssetDatabase.GetAssetPath(data);
        if (!string.IsNullOrEmpty(assetPath))
        {
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ExDebug.Log($"CloseTab: deleted asset {assetPath}");
        }
        if (tabDatas.Count == 0)
        {
            indexTab = -1;
            UIBase.Show(new EmptyUI());
            ExDebug.Log("CloseTab: no tabs, showing EmptyUI");
        }
        else
        {
            int newIndex = Mathf.Clamp(indexTab == index ? index - 1 : indexTab, 0, tabDatas.Count - 1);
            ExDebug.Log($"CloseTab: switching to {newIndex}");
            SwitchTab(newIndex);
        }
        tabSetting.SetTabData(tabDatas);
        ExDebug.Log("CloseTab done");
    }

    private void RebuildTabsUI(TabData active)
    {
        var styleSetting = _container.ResolveNamed<StyleSetting>(ServiceNameKeys.StyleSetting);
        var p = styleSetting.GetCorrectItem().GetEdixorParameters();
        
        var activeStyle = p.GetStyleByName(p.Tabs, "active");
        var normalStyle = p.GetStyleByName(p.Tabs, "normal");
        
        var section = UIBase.GetElement("tab-section");
        foreach (var c in section.Children().Where(x => x.ClassListContains("tab-container")).ToList())
            section.Remove(c);
        
        tabContainers.Clear();
        
        for (int i = 0; i < tabDatas.Count; i++)
        {
            int idx = i;
            var data = tabDatas[idx];
            var style = data == active ? activeStyle : normalStyle;
            var logic = logicInstances[data];

            var containerElement = tabUIFactory.CreateTabContainer(
                logic,
                style,
                () => { ExDebug.Log($"RebuildTabsUI.switch {idx}"); SwitchTab(idx); },
                () => { ExDebug.Log($"RebuildTabsUI.close {idx}"); CloseTab(idx); }
            );

            tabContainers[data] = containerElement;
            section.Add(containerElement);
        }
    }

    public void CloseAllTabs()
    {
        foreach (var logic in logicInstances.Values)
        {
            logic.InvokeOnDisable();
            logic.InvokeOnDestroy();
        }
        tabDatas.Clear();
        logicInstances.Clear();
        tabContainers.Clear();
        indexTab = -1;
        UIBase.Show(new EmptyUI());
    }

    public void OnGUI()
    {
        if (indexTab >= 0 && indexTab < tabDatas.Count)
            logicInstances[tabDatas[indexTab]].InvokeUpdateGUI();
    }

    public void OnWindowClose()
    {
        if (!_isRestoring)
            tabSetting.SetTabData(tabDatas);
    }

    public EdixorTab GetLogicFor(TabData data)
    {
        logicInstances.TryGetValue(data, out var logic);
        return logic;
    }

    private EdixorTab CreateLogic(TabData data)
    {
        try
        {
            var type = Type.GetType(data.LogicTypeName);
            if (type == null) throw new Exception($"Type {data.LogicTypeName} not found");

            var instance = (EdixorTab)Activator.CreateInstance(type);
            ExDebug.Log($"[TabController.CreateLogic] Creating {type.Name}");
            instance.Initialize(data, _container, UIBase.GetElement("middle-section-content"));
            instance.InvokeAwake();
            instance.InvokeStart();
            return instance;
        }
        catch (Exception ex)
        {
            ExDebug.LogError($"Error creating tab logic: {data?.TabName ?? "Unknown"}\n{ex}");
            return CreateFallbackTab(data?.TabName ?? "Error", ex);
        }
    }


    private EdixorTab CreateFallbackTab(string title, Exception ex)
    {
        var fallback = new ErrorTab(ex);
        fallback.Initialize(null, _container, UIBase.GetElement("middle-section-content"));
        fallback.InvokeStart();
        return fallback;
    }

    private TabData CreateAndSaveTabDataAsset(Type logicType, string uniqueName = null)
    {
        var logic = (EdixorTab)Activator.CreateInstance(logicType);
        logic.Initialize(null, _container);
        logic.InvokeAwake();

        string tabName = string.IsNullOrEmpty(logic.Title) ? "_" : logic.Title;
        string safeName = MakeSafeFileName(tabName);

        string finalName = $"{safeName}_{Guid.NewGuid():N}";

        var uxml = !string.IsNullOrEmpty(logic.UxmlPath) ? EdixorObjectLocator.LoadObject<VisualTreeAsset>(logic.UxmlPath) : null;
        var uss = !string.IsNullOrEmpty(logic.UssPath) ? EdixorObjectLocator.LoadObject<StyleSheet>(logic.UssPath) : null;
        var icon = !string.IsNullOrEmpty(logic.IconPath) ? EdixorObjectLocator.LoadObject<Texture2D>(logic.IconPath) : null;

        var data = ScriptableObject.CreateInstance<TabData>();
        data.Init(logicType.AssemblyQualifiedName, logic.Title, uxml, uss, icon);

        string folder = EdixorObjectLocator.Resolve($"WindowStates/{tabSetting.EdixorId}/SaveTab");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, $"{finalName}.asset").Replace("\\", "/");

        AssetDatabase.CreateAsset(data, path);
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return AssetDatabase.LoadAssetAtPath<TabData>(path);
    }

    private static string MakeSafeFileName(string name)
    {
        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            name = name.Replace(c.ToString(), "_");
        return name.Trim();
    }

    private void RefreshContent(TabData data)
    {
        var contentContainer = UIBase.GetElement("middle-section-content");
        contentContainer.Clear();
        try
        {
            var logic = logicInstances[data];
            if (logic.Root != null)
                contentContainer.Add(logic.Root);
            logic.InvokeOnEnable();
        }
        catch (Exception ex)
        {
            ExDebug.LogError($"Error on OnEnable for tab: {data.TabName}\n{ex}");
        }
    }

    private void ClearAll()
    {
        foreach (var logic in logicInstances.Values)
        {
            logic.InvokeOnDisable();
            logic.InvokeOnDestroy();
        }
        tabDatas.Clear();
        logicInstances.Clear();
        tabContainers.Clear();
        indexTab = -1;
        var section = UIBase.GetElement("tab-section");
        foreach (var c in section.Children().Where(x => x.ClassListContains("tab-container")).ToList())
            section.Remove(c);
        UIBase.GetElement("middle-section-content").Clear();
    }
}

using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class EdixorUIManager
{
    private List<EdixorTab> tabs;
    private int indexTab;

    private VisualElement root;
    private readonly EdixorWindow window;
    private EdixorDesign design;

    public EdixorUIManager(EdixorWindow window)
    {
        this.window = window;
    }

    public void LoadUI()
    {
        design = window.GetSetting().GetCurrentDesign();

        Debug.Log((design == null) + " - aaa");

        design.LoadUI();
        
        tabs = new List<EdixorTab> { new NewTab(design.GetSection("middle-section-content"))};
        AddTab(new NewTab(design.GetSection("middle-section-content")));

        tabs[indexTab].LoadUI();
    }

    private void AddTabs(VisualElement topSection, VisualElement scrollView, List<EdixorTab> tabs)
    {
        if (topSection == null || tabs == null) return;
        
        int i = 0;
        foreach (EdixorTab tab in tabs)
        { 
            int currentIndex = i;

            Button visualElementTab = new Button(() => SwitchTab(currentIndex))
            {
                text = tab.Title 
            };

            topSection.Add(visualElementTab);

            i++;
        }
    }


    public void AddTab(EdixorTab newTab)
    {
        if (newTab == null) return;

        tabs.Add(newTab);

        int currentIndex = tabs.Count - 1;

        Button visualElementTab = new Button(() => SwitchTab(currentIndex))
        {
            text = newTab.Title 
        };

        design.GetSection("tab-section").Add(visualElementTab);
        
        SwitchTab(indexTab + 1);
    }

    private void SwitchTab(int index) {
        tabs[indexTab].DeleteUI();
        tabs[index].LoadUI();
        tabs[index].OnUI();
        indexTab = index;
    }

    public EdixorDesign GetDesign() {
        return design;
    }
}
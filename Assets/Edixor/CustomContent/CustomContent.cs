using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public abstract class Menu<T, I> : EditorWindow where T : Menu<T, I> where I : ICMItem
{
    protected List<I> menuItems = new List<I>();
    protected BFPStyle style;
    protected bool isOpen = false;
    protected float itemHeight = 20f;
    protected static T activeMenu;
    
    // Dictionary to store groups of CMItemBool
    protected Dictionary<int, List<CMItemBool>> cmItemBoolGroups = new Dictionary<int, List<CMItemBool>>();
    private int nextGroupId = 0;

    public void AddItem(I typeItem) {
        menuItems.Add(typeItem);
    }

    public void AddSomeItem<TItem>(TItem[] typeItems) where TItem : ICMItem
    {
        if (typeItems == null || typeItems.Length == 0)
            return;

        // Track the starting index for the new items
        int minIndex = menuItems.Count;
        int maxIndex = minIndex + typeItems.Length - 1;

        // Determine item type and handle accordingly
        if (typeof(TItem) == typeof(CMItemBool))
        {
            var boolItemsList = new List<CMItemBool>();
            foreach (var item in typeItems)
            {
                if (item is CMItemBool boolItem)
                {
                    menuItems.Add((I)(ICMItem)boolItem); // Cast to I
                    boolItemsList.Add(boolItem);
                }
            }

            // Store the range of indices for this group
            cmItemBoolGroups[nextGroupId] = boolItemsList;
            nextGroupId++;
        }
        else if (typeof(TItem) == typeof(CMItemAction))
        {
            foreach (var item in typeItems)
            {
                if (item is CMItemAction actionItem)
                {
                    menuItems.Add((I)(ICMItem)actionItem); // Cast to I
                }
            }
        }
        else
        {
            throw new ArgumentException("Unsupported item type.");
        }
    }

    public virtual void ShowMenu(Vector2 position)
    {
        if (activeMenu != null && activeMenu != this)
        {
            activeMenu.CloseMenu();
        }

        activeMenu = (T)this;
        isOpen = true;
        float height = Mathf.Max((itemHeight * menuItems.Count) + (2.3f * menuItems.Count), 20f); 
        this.position = new Rect(position.x, position.y, 200, height); 
        this.ShowPopup();
    }

    public void CloseMenu()
    {
        isOpen = false;
        this.Close();

        if (activeMenu == this)
        {
            activeMenu = null;
        }
    }

    protected virtual void BeginGUIMenu()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D || menuItems.Count == 0)
        {
            CloseMenu();
            Event.current.Use();
        }

        if (!isOpen) return;

        //if (/*style.button,*/ == null)
        //{
        //    style.InitializationStyle();
        //}

        //style.Background(new Rect(0, 0, position.width, position.height));
    }

    public virtual void HandleItemSelection(ICMItem selectedItem)
    {
        if (selectedItem is CMItemBool boolItem)
        {
            foreach (var group in cmItemBoolGroups.Values)
            {
                if (group.Contains(boolItem))
                {
                    foreach (var item in group)
                    {
                        item.SetSelected(item.Equals(boolItem));
                    }
                    break;
                }
            }
        }
        else if (selectedItem is CMItemAction actionItem)
        {
            // Обработка выбора CMItemAction
            foreach (var item in menuItems)
            {
                if (item is CMItemAction currentItem)
                {
                    if (currentItem.Equals(actionItem))
                    {
                        Debug.Log("1 " + currentItem.ToString());
                    }
                    else
                    {
                        Debug.Log("0 " + currentItem.ToString());
                    }
                }
            }
        }
    }


    private void OnLostFocus()
    {
        CloseMenu();
    }

    private void OnDestroy()
    {
        if (activeMenu == this)
        {
            activeMenu = null;
        }
    }
}

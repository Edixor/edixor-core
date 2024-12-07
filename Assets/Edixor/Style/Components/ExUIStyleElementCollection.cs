using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BFPStyleElementCollection
{
    public TypeElements type; 
    public List<BFPStyleElements> Elements;

    public BFPStyleElements GetElement(string name) 
    {
        // Поиск элемента по имени
        foreach (var element in Elements)
        {
            if (element.name == name)
            {
                return element;
            }
        }

        // Вернуть null, если элемент не найден
        return null;
    }

    public BFPStyleElements GetElement(int index) 
    {
        // Проверка на допустимость индекса
        if (index >= 0 && index < Elements.Count)
        {
            return Elements[index];
        }

        // Вернуть null, если индекс некорректен
        return null;
    }

    public int GetCountElements() 
    {
        return Elements.Count;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MyEdixorTab : EdixorTab
{
    public MyEdixorTab(VisualElement parentContainer, DIContainer container, string tabName)
    {
        // Вызываем инициализацию с нужными параметрами:
        Initialize(parentContainer, container, tabName, "Path/To/YourUXML.uxml", "Path/To/YourUSS.uss");
    }

    // Определяем методы жизненного цикла без использования new или override.
    // Они будут найдены базовым классом через рефлексию.

    // Можно сделать метод с любой видимостью: public, private, protected...
    // Здесь они объявлены как private, но можно менять по необходимости.

    private void Awake()
    {
        Debug.Log("Awake has been called in MyEdixorTab.");
    }

    private void Start()
    {
        Debug.Log("Start has been called in MyEdixorTab.");
    }

    private void Update()
    {
        Debug.Log("Update is being called in MyEdixorTab.");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable has been called in MyEdixorTab.");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable has been called in MyEdixorTab.");
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy has been called in MyEdixorTab.");
    }

    private void OnUI()
    {
        Debug.Log("OnUI has been called in MyEdixorTab.");
    }
}

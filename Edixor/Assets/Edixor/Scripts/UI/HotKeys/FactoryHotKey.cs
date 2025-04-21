using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FactoryHotKey : IFactoryHotKey
{
    private readonly DIContainer container;
    private readonly IHotKeyController hotKeyController;

    public FactoryHotKey(DIContainer container, IHotKeyController hotKeyController)
    {
        this.container = container;
        this.hotKeyController = hotKeyController;
    }

    public void CreateHotKey(string keyOrPath, Action action = null)
    {
        Debug.Log($"Создание горячей клавиши по ключу: {keyOrPath}");
        if (string.IsNullOrEmpty(keyOrPath))
        {
            Debug.LogError("KeyOrPath пустой или null.");
            return;
        }

        if (IsAssetPath(PathResolver.ResolvePath(keyOrPath)))
        {
            LoadHotKey(keyOrPath, action);
        }
        else
        {
            // ищем по имени
            var resolvedPath = FindAssetPathByName(keyOrPath);
            if (string.IsNullOrEmpty(resolvedPath))
            {
                Debug.LogError($"KeyActionData не найден с именем: {keyOrPath}");
                return;
            }
            LoadHotKey(resolvedPath, action);
        }
    }

    private bool IsAssetPath(string s)
    {
        // можно расширить: проверять наличие ".asset", слэшей и т.п.
        return s.IndexOf('/') >= 0 || s.EndsWith(".asset", StringComparison.OrdinalIgnoreCase);
    }

    private string FindAssetPathByName(string keyName)
    {
        // ищем все ассеты типа KeyActionData с этим именем
        var guids = AssetDatabase.FindAssets($"t:KeyActionData {keyName}");
        if (guids.Length == 0)
            return null;

        // пытаемся найти точно совпадающее по полю Name
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<KeyActionData>(path);
            if (data != null && data.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase))
                return path;
        }

        // иначе возвращаем первый попавшийся
        return AssetDatabase.GUIDToAssetPath(guids[0]);
    }

    protected void LoadHotKey(string path, Action action)
    {
        // ваш оригинальный метод почти без изменений
        KeyActionData data = AssetDatabase.LoadAssetAtPath<KeyActionData>(path);
        if (data == null)
        {
            Debug.LogError($"KeyActionData не найден по пути: {path}");
            return;
        }
        
        KeyActionLogic logic;

        if (action != null)
        {
            logic = new CustomKeyAction(container, action);
        } else
        {
            logic = container.ResolveNamed<KeyActionLogic>(data.LogicKey);
        }

        Debug.Log($"Создание горячей клавиши: {data.Name} с логикой: {logic.GetType().Name}");
        hotKeyController.AddKey(new KeyAction(data, logic));
    }
}

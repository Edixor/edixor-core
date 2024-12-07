using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStyle
{
    private BFPStyle[] styles;
    private int count = 0;

    public DataStyle(int size) {
        styles = new BFPStyle[size];  // Инициализируем массив с фиксированным размером
    }

    // Добавление нового стиля в массив
    public void NewStyle(BFPStyle style) {
        if (count < styles.Length) {
            styles[count] = style;
            count++;
        } else {
            Debug.LogWarning("Array is full, cannot add new style.");
        }
    }

    // Получаем массив стилей, удаляя те, у которых model == null
    public BFPStyle[] GetStyles() {
        List<BFPStyle> validStyles = new List<BFPStyle>();  // Временный список для валидных стилей

        for (int i = 0; i < count; i++) {
            BFPStyleModel model = styles[i].GetModel();  // Проверяем модель у каждого стиля

            if (model != null) {
                validStyles.Add(styles[i]);  // Если модель не пустая, добавляем стиль в список
            }
        }

        return validStyles.ToArray();  // Преобразуем список обратно в массив и возвращаем
    }
}

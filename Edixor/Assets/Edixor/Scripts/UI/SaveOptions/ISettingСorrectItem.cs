public interface ISettingCorrectItem<TItem>
{
    TItem GetCorrectItem();
    void UpdateIndex(int newIndex);
}

public interface ISettingItemFull<TItemFull, TKey>
{
    TItemFull GetItemFull(TKey key);
    TItemFull[] GetAllItemFull();
}

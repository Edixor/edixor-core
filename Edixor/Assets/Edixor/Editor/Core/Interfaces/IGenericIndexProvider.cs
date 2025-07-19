using System.Collections.Generic;
using System.Threading.Tasks;
public interface IGenericIndexProvider<T> where T : class
{
    void Init(string indexUrl);
    Task<List<T>> LoadIndexAsync();
}
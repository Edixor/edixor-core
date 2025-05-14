using System.Collections.Generic;
using System.Threading.Tasks;
public interface IExtensionIndexProvider
{
    Task<List<IndexEntry>> LoadIndexAsync();
}
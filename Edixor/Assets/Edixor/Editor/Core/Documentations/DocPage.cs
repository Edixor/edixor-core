using UnityEngine;

namespace ExTools.Docs
{

    [System.Serializable]
    public class DocPage
    {
        public string Title;
        [TextArea(5, 20)]
        public string MarkdownContent;
    }
}
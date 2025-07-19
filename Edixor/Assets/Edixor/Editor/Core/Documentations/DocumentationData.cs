using System.Collections.Generic;
using UnityEngine;


namespace ExTools.Docs
{
    [CreateAssetMenu(fileName = "Documentation", menuName = "Edixor/Documentation")]
    public class DocumentationData : ScriptableObject
    {
        [SerializeField]
        private List<DocPage> pages = new List<DocPage>();
        public List<DocPage> Pages => pages;
    }
}

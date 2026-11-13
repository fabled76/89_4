using System.Collections.Generic;
using UnityEngine;

namespace DeadAir
{
    [CreateAssetMenu(menuName = "89.4/Evidence")]
    public sealed class EvidenceDefinition : ScriptableObject
    {
        [SerializeField] private string evidenceId;
        [SerializeField, Tooltip("Prototype String Table key for the title.")] private string title;
        [SerializeField, Tooltip("Ordered Prototype String Table page keys.")] private string[] pages;
        public string Id => evidenceId;
        public string Title => PrototypeText.Get(title);
        public int PageCount => pages?.Length ?? 0;
        public IReadOnlyList<string> PageKeys => pages;
        public string GetPage(int index) => PrototypeText.Get(pages[index]);
    }
}

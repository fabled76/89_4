using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeadAir
{
    public sealed class EvidenceJournal : MonoBehaviour
    {
        private readonly List<EvidenceDefinition> entries = new List<EvidenceDefinition>();
        private readonly HashSet<string> evidenceIds = new HashSet<string>(StringComparer.Ordinal);
        public IReadOnlyList<EvidenceDefinition> Entries => entries.AsReadOnly();
        public event Action Changed;

        public bool Contains(EvidenceDefinition evidence) => evidence != null && evidenceIds.Contains(evidence.Id);

        public bool Add(EvidenceDefinition evidence)
        {
            if (evidence == null || string.IsNullOrWhiteSpace(evidence.Id) || !evidenceIds.Add(evidence.Id)) return false;
            entries.Add(evidence);
            Changed?.Invoke();
            return true;
        }
    }
}

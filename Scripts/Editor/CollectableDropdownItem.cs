﻿using UnityEditor.IMGUI.Controls;

namespace BrunoMikoski.ScriptableObjectCollections
{
    public sealed class CollectableDropdownItem : AdvancedDropdownItem
    {
        internal readonly CollectableScriptableObject Collectable;

        public CollectableDropdownItem(CollectableScriptableObject targetCollectable) : base(targetCollectable.name)
        {
            Collectable = targetCollectable;
        }
        
        public CollectableDropdownItem(CollectableScriptableObject targetCollectable, string displayName) : base(displayName)
        {
            Collectable = targetCollectable;
        }
    }
}

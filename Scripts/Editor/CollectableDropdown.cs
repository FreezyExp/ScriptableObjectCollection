using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace BrunoMikoski.ScriptableObjectCollections
{
    public sealed class CollectableDropdown : AdvancedDropdown
    {
        private Action<CollectableScriptableObject> callback;
        private Type collectableType;
        private List<CollectableScriptableObject> allAvailableCollectables;
        private bool multipleCollections;

        public CollectableDropdown(AdvancedDropdownState state,Type targetCollectableType, List<ScriptableObjectCollection> availableCollections) : base(state)
        {
            minimumSize = new Vector2(200, 300);
            collectableType = targetCollectableType;
            allAvailableCollectables = new List<CollectableScriptableObject>();
            HashSet<ScriptableObjectCollection> collectionsWithResults = new HashSet<ScriptableObjectCollection>();
            for (int i = 0; i < availableCollections.Count; i++)
            {
                ScriptableObjectCollection scriptableObjectCollection = availableCollections[i];

                for (int j = 0; j < scriptableObjectCollection.Items.Count; j++)
                {
                    CollectableScriptableObject collectableScriptableObject = scriptableObjectCollection.Items[j];
                    if (collectableScriptableObject.GetType() == collectableType ||
                        collectableType.IsInstanceOfType(collectableScriptableObject))
                    {
                        allAvailableCollectables.Add(collectableScriptableObject);
                        collectionsWithResults.Add(collectableScriptableObject.Collection);
                    }
                }
            }

            if (collectionsWithResults.Count > 1)
                multipleCollections = true;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem(collectableType.Name);
            root.AddChild(new AdvancedDropdownItem("None"));

            for (int i = 0; i < allAvailableCollectables.Count; i++)
            {
                CollectableScriptableObject collectableScriptableObject = allAvailableCollectables[i];
                AdvancedDropdownItem parent = root;

                if (multipleCollections && collectableScriptableObject.Collection.Count > 1)
                {
                    parent = root.children.FirstOrDefault(dropdownItem =>
                        dropdownItem.name.Equals(collectableScriptableObject.Collection.name,
                            StringComparison.Ordinal));
                    if (parent == null)
                    {
                        parent = new AdvancedDropdownItem(collectableScriptableObject.Collection.name);
                        root.AddChild(parent);
                    }
                }

                parent.AddChild(new CollectableDropdownItem(collectableScriptableObject, collectableScriptableObject.name));
            }
            
            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            if (item is CollectableDropdownItem collectableDropdownItem)
                callback?.Invoke(collectableDropdownItem.Collectable);
            else
                callback?.Invoke(null);
        }

        public void Show(Rect rect, Action<CollectableScriptableObject> onSelectedCallback)
        {
            callback = onSelectedCallback;
            base.Show(rect);
        }
    }
}

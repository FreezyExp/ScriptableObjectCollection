using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace BrunoMikoski.ScriptableObjectCollections
{
    public sealed class CollectableDropdown : AdvancedDropdown
    {
        private List<ScriptableObjectCollection> availableCollections;
        private Action<CollectableScriptableObject> callback;
        private Type collectableType;

        public CollectableDropdown(AdvancedDropdownState state,Type collectableType, List<ScriptableObjectCollection> availableCollections) : base(state)
        {
            this.availableCollections = availableCollections;
            this.minimumSize = new Vector2(200, 300);
            this.collectableType = collectableType;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem(collectableType.Name);

            if (availableCollections.Count == 1)
            {
                ScriptableObjectCollection firstCollection = availableCollections.First();
                Type collectableType = firstCollection.GetCollectionType();

                root.AddChild(new AdvancedDropdownItem("None"));
                for (int i = 0; i < availableCollections.Count; i++)
                {
                    CollectableScriptableObject collectionItem = firstCollection[i];

                    Type collectionType = collectionItem.GetType();
                    if (!collectionType.IsAssignableFrom(collectableType))
                        continue;
                    
                    if (collectionType == collectableType)
                        root.AddChild(new CollectableDropdownItem(collectionItem));
                    else
                    {
                        AdvancedDropdownItem parent = GetOrCreateDropdownItemForType(root, collectionItem);
                        parent.AddChild(new CollectableDropdownItem(collectionItem));
                    }
                }
            }
            else
            {
                for (int i = 0; i < availableCollections.Count; i++)
                {
                    ScriptableObjectCollection scriptableObjectCollection = availableCollections[i];

                    AdvancedDropdownItem collectionDropdownItem = new AdvancedDropdownItem(scriptableObjectCollection.name);
                    root.AddChild(collectionDropdownItem);

                    for (int k = 0; k < availableCollections[i].Items.Count; k++)
                    {
                        CollectableScriptableObject collectionItem = availableCollections[i].Items[k];
                        
                        if (!collectionItem.GetType().IsAssignableFrom(collectableType))
                            continue;

                        if (collectionItem.GetType() == collectableType)
                            collectionDropdownItem.AddChild(new CollectableDropdownItem(collectionItem));
                        else
                        {
                            AdvancedDropdownItem parent = GetOrCreateDropdownItemForType(collectionDropdownItem, collectionItem);
                            parent.AddChild(new CollectableDropdownItem(collectionItem));
                        }
                    }
                }
            }
           
            return root;
        }

        private AdvancedDropdownItem GetOrCreateDropdownItemForType(AdvancedDropdownItem root,
            CollectableScriptableObject collectionItem)
        {
            AdvancedDropdownItem item = root.children.FirstOrDefault(dropdownItem =>
                dropdownItem.name.Equals(collectionItem.GetType().Name, StringComparison.Ordinal));
            if (item == null)
            {
                item = new AdvancedDropdownItem(collectionItem.GetType().Name);
                root.AddChild(item);
            }

            return item;
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

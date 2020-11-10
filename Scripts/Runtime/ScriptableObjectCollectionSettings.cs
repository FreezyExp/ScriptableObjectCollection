using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BrunoMikoski.ScriptableObjectCollections.Core;
using UnityEngine;

namespace BrunoMikoski.ScriptableObjectCollections
{
    public class ScriptableObjectCollectionSettings : ResourceScriptableObjectSingleton<ScriptableObjectCollectionSettings>
    {
        [Serializable]
        private class CollectionToSettings
        {
            [SerializeField]
            internal ScriptableObjectCollection collection;
            
            [SerializeField]
            internal GeneratedStaticFileType generatedStaticFileGeneratorType;

            [SerializeField]
            internal string staticGeneratedFileParentFolder;

            [SerializeField]
            internal bool isAutomaticallyLoaded = true;

            [SerializeField] 
            internal bool overrideStaticFileLocation;

            [SerializeField]
            internal bool generateCustomStaticFile;

            [SerializeField]
            internal string customStaticFileName;
        }
        
#if UNITY_EDITOR
        
#pragma warning disable 0649
        [SerializeField]
        private UnityEditor.DefaultAsset defaultScriptableObjectsFolder;
        public string DefaultScriptableObjectsFolder => UnityEditor.AssetDatabase.GetAssetPath(defaultScriptableObjectsFolder);

        [SerializeField]
        private UnityEditor.DefaultAsset defaultScriptsFolder;
        public string DefaultScriptsFolder => UnityEditor.AssetDatabase.GetAssetPath(defaultScriptsFolder);
        
        [SerializeField]
        private UnityEditor.DefaultAsset defaultGeneratedCodeFolder;
#pragma warning restore 0649
#endif
        
        private string DefaultGeneratedCodeFolder
        {
            get
            {
#if UNITY_EDITOR                
                return UnityEditor.AssetDatabase.GetAssetPath(defaultGeneratedCodeFolder);
#endif
#pragma warning disable CS0162
                return string.Empty;
#pragma warning restore CS0162
            }
        }

        [SerializeField]
        private GeneratedStaticFileType defaultGenerator = GeneratedStaticFileType.DirectAccess;
        
        [SerializeField]
        private List<CollectionToSettings> collectionsSettings = new List<CollectionToSettings>();

        [SerializeField] 
        private string defaultNamespace = String.Empty;
        public string DefaultNamespace => defaultNamespace;

        
        public string GetGeneratedStaticFileName(ScriptableObjectCollection collection)
        {
            if (!TryGetSettingsForCollection(collection, out CollectionToSettings settings))
                return collection.GetCollectionType().Name;

            if (string.IsNullOrEmpty(settings.customStaticFileName))
                return collection.GetCollectionType().Name;
            
            return settings.customStaticFileName;
        }
        
        public GeneratedStaticFileType GetStaticFileTypeForCollection(ScriptableObjectCollection collection)
        {
            if (!TryGetSettingsForCollection(collection, out CollectionToSettings settings))
                return defaultGenerator;

            return settings.generatedStaticFileGeneratorType;
        }

        public bool IsCollectionAutomaticallyLoaded(ScriptableObjectCollection collection)
        {
            if (!TryGetSettingsForCollection(collection, out CollectionToSettings settings))
                return true;

            return settings.isAutomaticallyLoaded;
        }
        
        public bool IsOverridingStaticFileLocation(ScriptableObjectCollection collection)
        {
            if (!TryGetSettingsForCollection(collection, out CollectionToSettings settings))
                return false;

            return settings.overrideStaticFileLocation;
        }

        public bool IsGeneratingCustomStaticFile(ScriptableObjectCollection collection)
        {
            if (TryGetSettingsForCollection(collection, out CollectionToSettings settings))
                return settings.generateCustomStaticFile;

            return false;
        }

        
        public void SetOverridingStaticFileLocation(ScriptableObjectCollection collection, bool isOverriding)
        {
            CollectionToSettings settings = GetOrCreateSettingsForCollection(collection);
            settings.overrideStaticFileLocation = isOverriding;
            ObjectUtility.SetDirty(this);
        }
        
        public string GetStaticFileFolderForCollection(ScriptableObjectCollection collection)
        {
            if (!TryGetSettingsForCollection(collection, out CollectionToSettings settings))
                return DefaultGeneratedCodeFolder;

            if (!settings.overrideStaticFileLocation || string.IsNullOrEmpty(settings.staticGeneratedFileParentFolder))
                return DefaultGeneratedCodeFolder;

            return settings.staticGeneratedFileParentFolder;
        }
        
        public void SetCollectionAutomaticallyLoaded(ScriptableObjectCollection targetCollection,  bool isAutomaticallyLoaded)
        {
            CollectionToSettings settings = GetOrCreateSettingsForCollection(targetCollection);
            settings.isAutomaticallyLoaded = isAutomaticallyLoaded;
            ObjectUtility.SetDirty(this);
        }

        public void SetStaticFileGeneratorTypeForCollection(ScriptableObjectCollection targetCollection, GeneratedStaticFileType staticCodeGeneratorType)
        {
            CollectionToSettings settings = GetOrCreateSettingsForCollection(targetCollection);
            settings.generatedStaticFileGeneratorType = staticCodeGeneratorType;
            ObjectUtility.SetDirty(this);
        }

        public void SetStaticFileFolderForCollection(ScriptableObjectCollection targetCollection, string targetFolder)
        {
            CollectionToSettings settings = GetOrCreateSettingsForCollection(targetCollection);
            settings.staticGeneratedFileParentFolder = targetFolder;
            ObjectUtility.SetDirty(this);
        }
        
        public void SetGenerateCustomStaticFile(ScriptableObjectCollection targetCollection, bool isUsingCustom)
        {
            CollectionToSettings settings = GetOrCreateSettingsForCollection(targetCollection);
            settings.generateCustomStaticFile = isUsingCustom;
            ObjectUtility.SetDirty(this);
        }
        
        public void SetGenerateCustomStaticFileName(ScriptableObjectCollection targetCollection, string targetName)
        {
            CollectionToSettings settings = GetOrCreateSettingsForCollection(targetCollection);
            settings.customStaticFileName = targetName;
            ObjectUtility.SetDirty(this);
        }

        private bool TryGetSettingsForCollection(ScriptableObjectCollection targetCollection,
            out CollectionToSettings settings)
        {
            settings = collectionsSettings.FirstOrDefault(toSettings => toSettings.collection == targetCollection);
            return settings != null;
        }
        
        private CollectionToSettings GetOrCreateSettingsForCollection(ScriptableObjectCollection targetCollection)
        {
            if (!TryGetSettingsForCollection(targetCollection, out CollectionToSettings settings))
            {
                settings = new CollectionToSettings {collection = targetCollection};
                collectionsSettings.Add(settings);
            }

            return settings;
        }

    }
}

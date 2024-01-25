namespace ZaiR37.Quest.Editor
{
    using UnityEditor.Experimental.GraphView;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using System;

    public class StringListSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private string[] listItems;
        private Action<string> onSetIndexCallback;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
            searchList.Add(new SearchTreeGroupEntry(new GUIContent("List"), 0));

            List<string> groups = new List<string>();
            foreach (string item in listItems)
            {
                string[] entryTitle = item.Split('/');
                string groupName = "";
                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];
                    if (!groups.Contains(groupName))
                    {
                        searchList.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }

                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()));
                entry.level = entryTitle.Length;
                entry.userData = entryTitle.Last();
                searchList.Add(entry);
            }

            return searchList;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            onSetIndexCallback?.Invoke((string)SearchTreeEntry.userData);
            return true;
        }

        internal void Init(string[] items, Action<object> callback)
        {
            listItems = items;
            onSetIndexCallback = callback;
        }
    }
}
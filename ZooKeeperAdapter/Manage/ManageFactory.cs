using System;
using System.Collections.Generic;
using ZookeeperAdapter.Manage.Dynamic;
using ZookeeperAdapter.Manage.Static;
using ZookeeperAdapter.Manage.Temporary;

namespace ZookeeperAdapter.Manage
{
    internal class ManageFactory
    {
        private IZookeeperAdapter Adapter { get; set; }
        public IZookeeperAdapter InitializeManage(ManageType topic, string path, string topicKey, Dictionary<string, Dictionary<string, string>> dict, string nodeName = null)
        {
            switch (topic)
            {
                case ManageType.Dynamic:
                    Adapter = new DynamicNode(path);
                    Adapter.ItemHandler += (item) => UpdateItem(dict, item, topicKey);
                    Adapter.Start();
                    return Adapter;
                case ManageType.Static:
                    Adapter = new StaticNode(path);
                    Adapter.ItemHandler += (item) => UpdateItem(dict, item, topicKey);
                    Adapter.Start();
                    return Adapter;
                case ManageType.Temporary:
                    if (nodeName == null) throw new ArgumentOutOfRangeException($"NodeName is empty!");
                    TemporyNode TemporyAdapter = new TemporyNode(nodeName, path);
                    TemporyAdapter.ItemHandler += (item) => UpdateItem(dict, item, topicKey);
                    TemporyAdapter.Start();
                    TemporyAdapter.CreateSeq();
                    return TemporyAdapter;
                default:
                    throw new ArgumentOutOfRangeException($"Topic:({topic}) out of factory range!");
            }
        }
        private void UpdateItem(Dictionary<string, Dictionary<string, string>> dict, Dictionary<string, string> item, string topic)
        {
            if (!dict.ContainsKey(topic)) dict.Add(topic, item);
            else dict[topic] = item;
        }
    }
}

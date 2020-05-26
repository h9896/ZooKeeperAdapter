using System;
using System.Collections.Generic;
using ZookeeperAdapter.Manage.Dynamic;
using ZookeeperAdapter.Manage.Static;

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
                    Register(Adapter, dict, topicKey);
                    return Adapter;
                case ManageType.Static:
                    Adapter = new StaticNode(path);
                    Register(Adapter, dict, topicKey);
                    return Adapter;
                default:
                    throw new ArgumentOutOfRangeException($"Topic:({topic}) out of factory range!");
            }
        }
        private void UpdateItem(Dictionary<string, Dictionary<string, string>> dict, Dictionary<string, string> item, string topic)
        {
            if (!dict.ContainsKey(topic)) dict.Add(topic, item);
            else dict[topic] = item;
        }
        private void Register(IZookeeperAdapter adapter, Dictionary<string, Dictionary<string, string>> dict, string topicKey)
        {
            adapter.ItemHandler += (item) => UpdateItem(dict, item, topicKey);
            adapter.Start();
        }
    }
}

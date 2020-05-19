using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZooKeeperNet;

namespace ZookeeperAdapter.Manage
{
    abstract class ManageDynamic: IWatcher
    {
        abstract protected ZooKeeper zk { get; }
        public Dictionary<string, string> item { get { return _item; } set { _item = value; } }
        protected Dictionary<string, string> _item = new Dictionary<string, string>();
        protected void Manage(string path, EventType type, KeeperState state)
        {
            switch (type)
            {
                case EventType.NodeDeleted:
                    string delKey = path.Split('/')[path.Split('/').Count() - 1];
                    if (item.ContainsKey(delKey)) { item.Remove(delKey); }
                    break;
                case EventType.NodeDataChanged:
                    string key = path.Split('/')[path.Split('/').Count() - 1];
                    if (item.ContainsKey(key)) { item[key] = GetValue(path); }
                    break;
                case EventType.NodeChildrenChanged:
                    string[] newList = GetChildren(path);
                    foreach (string i in newList)
                    {
                        if (!item.ContainsKey(i))
                        {
                            item.Add(i, GetValue($"{path}/{i}"));
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        protected string GetValue(string path)
        {
            byte[] byt = zk.GetData(path, this, null);
            string value = byt == null ? "" : Encoding.UTF8.GetString(byt);
            return value;
        }
        protected string[] GetChildren(string path)
        {
            List<string> result = new List<string>();
            result.AddRange(zk.GetChildren(path, this));
            return result.ToArray();
        }

        public void Process(WatchedEvent @event)
        {
            switch (@event.Type)
            {
                case EventType.None:
                    break;
                case EventType.NodeCreated:
                    break;
                case EventType.NodeDeleted:
                case EventType.NodeDataChanged:
                case EventType.NodeChildrenChanged:
                    Manage(@event.Path, @event.Type, @event.State);
                    break;
                default:
                    break;
            }
        }
    }
}

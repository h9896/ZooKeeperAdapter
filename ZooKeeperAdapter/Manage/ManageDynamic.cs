using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZooKeeperNet;

namespace ZookeeperAdapter.Manage
{
    abstract class ManageDynamic: IWatcher, IZookeeperAdapter
    {
        public ZooKeeper Zk { get { return ZooKeeperAdapter._zk; } }
        public Dictionary<string, string> Item { get { return _item; } internal set { _item = value; } }
        public event ItemEvent ItemHandler;
        public bool ActiveNode { get; protected set; }
        protected Dictionary<string, string> _item = new Dictionary<string, string>();
        protected string _path { get; set; }
        public void Start() { GetFirst(_path); }
        protected void Manage(string path, EventType type, KeeperState state)
        {
            switch (type)
            {
                case EventType.NodeDeleted:
                    string delKey = path.Split('/')[path.Split('/').Count() - 1];
                    if (Item.ContainsKey(delKey)) { Item.Remove(delKey); }
                    break;
                case EventType.NodeDataChanged:
                    string key = path.Split('/')[path.Split('/').Count() - 1];
                    if (Item.ContainsKey(key)) { Item[key] = GetValue(path); }
                    break;
                case EventType.NodeChildrenChanged:
                    string[] newList = GetChildren(path);
                    foreach (string i in newList)
                    {
                        if (!Item.ContainsKey(i))
                        {
                            Item.Add(i, GetValue($"{path}/{i}"));
                        }
                    }
                    break;
                default:
                    break;
            }
            ItemHandler?.Invoke(Item);
        }
        protected void GetFirst(string path)
        {
            string[] newList = GetChildren(path);
            foreach (string i in newList)
            {
                if (!Item.ContainsKey(i))
                {
                    Item.Add(i, GetValue($"{path}/{i}"));
                }
            }
            ItemHandler?.Invoke(Item);
        }
        protected string GetValue(string path)
        {
            byte[] byt = Zk.GetData(path, this, null);
            string value = byt == null ? "" : Encoding.UTF8.GetString(byt);
            return value;
        }
        protected string[] GetChildren(string path)
        {
            List<string> result = new List<string>();
            result.AddRange(Zk.GetChildren(path, this));
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

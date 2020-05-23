using System.Collections.Generic;
using System.Text;
using ZooKeeperNet;

namespace ZookeeperAdapter.Manage
{
    abstract class ManageStatic: IZookeeperAdapter
    {
        public ZooKeeper Zk { get { return ZooKeeperAdapter._zk; } }
        public Dictionary<string, string> Item { get { return _item; } internal set { _item = value; } }
        public event ItemEvent ItemHandler;
        public bool ActiveNode { get { return false; } }
        protected Dictionary<string, string> _item = new Dictionary<string, string>();
        protected string _path { get; set; }
        public void Start() { Manage(_path); }
        protected void Manage(string path)
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
            byte[] byt = Zk.GetData(path, false, null);
            string value = byt == null ? "" : Encoding.UTF8.GetString(byt);
            return value;
        }
        protected string[] GetChildren(string path)
        {
            List<string> result = new List<string>();
            result.AddRange(Zk.GetChildren(path, false));
            return result.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ZookeeperAdapter.Manage
{
    abstract class ManageStatic: IZookeeperAdapter
    {
        public ZooKeeper zk { get { return ZooKeeperAdapter._zk; } }
        public Dictionary<string, string> item { get { return _item; } internal set { _item = value; } }
        public event ItemEvent itemHandler;
        protected Dictionary<string, string> _item = new Dictionary<string, string>();
        protected void Manage(string path)
        {
            string[] newList = GetChildren(path);
            foreach (string i in newList)
            {
                if (!item.ContainsKey(i))
                {
                    item.Add(i, GetValue($"{path}/{i}"));
                }
            }
        }
        protected string GetValue(string path)
        {
            byte[] byt = zk.GetData(path, false, null);
            string value = byt == null ? "" : Encoding.UTF8.GetString(byt);
            return value;
        }
        protected string[] GetChildren(string path)
        {
            List<string> result = new List<string>();
            result.AddRange(zk.GetChildren(path, false));
            return result.ToArray();
        }
    }
}

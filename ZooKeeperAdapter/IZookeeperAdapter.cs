using System.Collections.Generic;
using ZooKeeperNet;

namespace ZookeeperAdapter
{
    public interface IZookeeperAdapter
    {
        Dictionary<string, string> Item { get; }
        ZooKeeper Zk { get; }
        bool ActiveNode { get; }
        event ItemEvent ItemHandler;
        void Start();
    }
    public delegate void ItemEvent(Dictionary<string, string> Item);
}

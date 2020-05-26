using System.Collections.Generic;
using ZooKeeperNet;

namespace ZookeeperAdapter
{
    public interface IZookeeperAdapter
    {
        Dictionary<string, string> Item { get; }
        event ItemEvent ItemHandler;
        void Start();
    }
}

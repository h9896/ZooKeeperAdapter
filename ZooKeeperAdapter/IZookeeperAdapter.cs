using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ZookeeperAdapter
{
    public interface IZookeeperAdapter
    {
        Dictionary<string, string> item { get; }
        ZooKeeper zk { get; }
        event ItemEvent itemHandler;
    }
    public delegate void ItemEvent(Dictionary<string, string> item);
}

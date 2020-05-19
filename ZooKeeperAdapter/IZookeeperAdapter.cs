using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ZookeeperAdapter
{
    internal interface IZookeeperAdapter
    {
        Dictionary<string, string> item { get; set; }
    }
}

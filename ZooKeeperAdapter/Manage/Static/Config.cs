using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;
using ZookeeperAdapter;

namespace ZookeeperAdapter.Manage.Static
{
    internal class Config : ManageStatic
    {
        protected override ZooKeeper zk { get { return ZooKeeperAdapter._zk; }}
    }
}

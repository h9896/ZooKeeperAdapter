using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ZookeeperAdapter.Manage.Dynamic
{
    internal class UserOnline: ManageDynamic
    {
        protected override ZooKeeper zk { get { return ZooKeeperAdapter._zk; } }
    }
}

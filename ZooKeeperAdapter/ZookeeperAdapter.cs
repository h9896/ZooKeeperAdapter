using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ZookeeperAdapter
{
    public class ZooKeeperAdapter
    {
        public string RootPath { get; private set; }
        public string EndPoint { get; private set; }
        static internal ZooKeeper _zk;
        private TimeSpan _timeout;
        public ZooKeeperAdapter(string rootPath, string endPoint, TimeSpan sessionTimeout)
        {
            RootPath = rootPath;
            EndPoint = endPoint;
            _timeout = sessionTimeout;
        }
        public void Initialize()
        {
            _zk = new ZooKeeper(EndPoint, _timeout, new WatchProcess());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZookeeperAdapter.Manage;
using ZooKeeperNet;

namespace ZookeeperAdapter
{
    public class ZooKeeperAdapter
    {
        public string RootPath { get; private set; }
        public string EndPoint { get; private set; }
        public Dictionary<string, Dictionary<string, string>> CareNode { get; private set; }
        static internal ZooKeeper _zk;
        private TimeSpan _timeout;
        private ManageFactory _factory = new ManageFactory();

        public ZooKeeperAdapter(string rootPath, string endPoint, TimeSpan sessionTimeout)
        {
            RootPath = rootPath;
            EndPoint = endPoint;
            _timeout = sessionTimeout;
            CareNode = new Dictionary<string, Dictionary<string, string>>();
        }
        public void Initialize(string topicList)
        {
            _zk = new ZooKeeper(EndPoint, _timeout, new WatchProcess());
            foreach(string topic in topicList.Split('|'))
            {
                IZookeeperAdapter adapter = _factory.InitializeManage(topic);
                adapter.itemHandler += (item) => UpdateItem(item, topic);
            }
        }
        private void UpdateItem(Dictionary<string, string> item, string topic)
        {
            CareNode[topic] = item;
        }
    }
}

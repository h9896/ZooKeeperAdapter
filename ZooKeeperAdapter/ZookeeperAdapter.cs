using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZookeeperAdapter.Manage;
using ZooKeeperNet;

namespace ZookeeperAdapter
{
    public class ZooKeeperAdapter : IWatcher
    {
        public string RootPath { get; private set; }
        public string EndPoint { get; private set; }
        public bool ConnectState { get; private set; }
        public Dictionary<string, Dictionary<string, string>> CareNode { get; private set; }
        static internal ZooKeeper _zk;
        private TimeSpan _timeout;
        private ManageFactory _factory = new ManageFactory();
        private CountdownEvent _connectLock { get; set; }
        private int _lockTimeout { get; set; }

        public ZooKeeperAdapter(string rootPath, string endPoint, TimeSpan sessionTimeout)
        {
            RootPath = rootPath;
            EndPoint = endPoint;
            ConnectState = false;
            _timeout = sessionTimeout;
            _lockTimeout = 5000;
            _connectLock = new CountdownEvent(1);
            CareNode = new Dictionary<string, Dictionary<string, string>>();
        }
        // SyncConnected
        public void SyncConnected()
        {
            if (!ConnectState)
            {
                _zk = new ZooKeeper(EndPoint, _timeout, this);
                _connectLock.Wait(_lockTimeout);
            }
        }
        // NonSyncConnected
        public void NonSyncConnected()
        {
            if (!ConnectState) _zk = new ZooKeeper(EndPoint, _timeout, this);
        }
        public void Close()
        {
            try
            {
                _zk.Dispose();
            }
            catch (ThreadInterruptedException errors)
            {
                throw errors;
            }
        }
        public void Initialize(string topicList)
        {
            foreach (string topic in topicList.Split('|'))
            {
                IZookeeperAdapter adapter = _factory.InitializeManage(topic);
                adapter.itemHandler += (item) => UpdateItem(item, topic);
            }
        }
        private void UpdateItem(Dictionary<string, string> item, string topic)
        {
            CareNode[topic] = item;
        }
        public void Process(WatchedEvent @event)
        {
            if (@event.State == KeeperState.SyncConnected)
            {
                ConnectState = true;
                _connectLock.Signal();
            }
        }
    }
}

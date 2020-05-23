using System;
using System.Collections.Generic;
using System.Threading;
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
        public void Initialize(string topic, string path, ManageType type, string nodeName = null)
        {
            IZookeeperAdapter adapter = _factory.InitializeManage(type, path, topic, CareNode, nodeName);
        }
        public string CreateNode(string path, byte[] value)
        {
            if (_zk.Exists(path, false) == null)
            {
                return _zk.Create(path, value, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }
            return null;
        }
        public void DeleteNode(string path)
        {
            if (_zk.Exists(path, false) != null)
            {
                _zk.Delete(path, -1);
            }
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

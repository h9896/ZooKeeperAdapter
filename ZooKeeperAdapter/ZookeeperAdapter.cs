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
        public bool ReConnect { get; private set; }
        public Dictionary<string, Dictionary<string, string>> CareNode { get; private set; }
        public event Connected ConnectedEvent;
        static internal ZooKeeper _zk;
        private TimeSpan _timeout;
        private ManageFactory _factory = new ManageFactory();
        private CountdownEvent _connectLock { get; set; }
        private int _lockTimeout { get; set; }

        public ZooKeeperAdapter(string rootPath, string endPoint, TimeSpan sessionTimeout, int connectedTimeout, bool reconnect = false)
        {
            RootPath = rootPath;
            EndPoint = endPoint;
            ConnectState = false;
            ReConnect = reconnect;
            _timeout = sessionTimeout;
            _lockTimeout = connectedTimeout;
            CareNode = new Dictionary<string, Dictionary<string, string>>();
        }
        /// <summary>
        /// SyncConnected: Connected ZooKeeper server and lock process until SyncConnectedEvent is received, or 5 seconds have passed.
        /// </summary>
        public void SyncConnected()
        {
            if (!CheckConnectState())
            {
                _connectLock = new CountdownEvent(1);
                _zk = new ZooKeeper(EndPoint, _timeout, this);
                _connectLock.Wait(_lockTimeout);
                if (!CheckConnectState()) { ConnectedEvent?.Invoke(ConnectedType.Fail); }
            }
        }
        /// <summary>
        /// AsyncConnected: Connected ZooKeeper server.
        /// </summary>
        public void AsyncConnected()
        {
            _connectLock = new CountdownEvent(1);
            if (!CheckConnectState()) _zk = new ZooKeeper(EndPoint, _timeout, this);
            Task.Run(() => {
                _connectLock.Wait(_lockTimeout);
                if (!CheckConnectState()) { ConnectedEvent?.Invoke(ConnectedType.Fail); }
            });
        }
        public void Close()
        {
            try
            {
                _zk.Dispose();
                ConnectState = false;
                ConnectedEvent?.Invoke(ConnectedType.DisConnected);
            }
            catch (ThreadInterruptedException errors)
            {
                throw errors;
            }
        }
        /// <summary>
        /// Initialize: Register the path, then get the data and put it in CareNode.
        /// </summary>
        /// <param name="topic">Dictionary Key in CareNode</param>
        /// <param name="path">ZooKeeper path</param>
        /// <param name="type">Manage type</param>
        /// <param name="nodeName">Temporary nodeName will create</param>
        public IZookeeperAdapter Initialize(string topic, string path, ManageType type, string nodeName = null)
        {
            return _factory.InitializeManage(type, path, topic, CareNode, nodeName);
        }
        /// <summary>
        /// CreateNode: Create a node.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string CreateNode(string path, byte[] value)
        {
            if (!CheckExists(path))
            {
                return _zk.Create(path, value, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }
            return null;
        }
        /// <summary>
        /// CreateTemporary: Create a temporary sequence node below path, and return sequence.
        /// </summary>
        /// <param name="path">zookeeper path</param>
        /// <param name="value">temporary node value</param>
        /// <returns>if success return sequence, else return null</returns>
        public string CreateTemporary(string path, byte[] value)
        {
            if (CheckExists(path))
                return _zk.Create(path + "/", value, Ids.OPEN_ACL_UNSAFE, CreateMode.EphemeralSequential).Split('/').Last();
            return null;
        }
        public void DeleteNode(string path)
        {
            if (CheckExists(path))
            {
                _zk.Delete(path, -1);
            }
        }
        public string[] GetChildren(string path)
        {
            if (CheckExists(path))
            {
                List<string> result = new List<string>();
                result.AddRange(_zk.GetChildren(path, false));
                return result.ToArray();
            }
            return new List<string>().ToArray();
        }
        public string GetValue(string path)
        {
            if (CheckExists(path))
            {
                byte[] byt = _zk.GetData(path, false, null);
                string value = byt == null ? "" : Encoding.UTF8.GetString(byt);
                return value;
            }
            return null;
        }
        public bool CheckExists(string path)
        {
            if (_zk.Exists(path, false) == null) { return false; }
            else return true;
        }
        public bool CheckConnectState() { return ConnectState; }
        public void Process(WatchedEvent @event)
        {
            switch (@event.State)
            {
                case KeeperState.Disconnected:
                    Close();
                    break;
                case KeeperState.SyncConnected:
                    ConnectState = true;
                    _connectLock.Signal();
                    ConnectedEvent?.Invoke(ConnectedType.Connected);
                    break;
                case KeeperState.Expired:
                    Close();
                    if (ReConnect) { SyncConnected(); }
                    break;
                default:
                    break;
            }
        }
    }
}

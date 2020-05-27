using NUnit.Framework;
using ZooKeeperNet;
using System;
using System.Threading.Tasks;
using System.Threading;
using ZookeeperAdapter;

namespace UnitTestZooKeeperAdapter
{
    [TestFixture]
    public class ConnectZooKeeper
    {
        private AutoResetEvent eventRaised;
        private ConnectedType connectedType;
        [Test, Category("AsyncConnected")]
        public void AsyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            eventRaised = new AutoResetEvent(false);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetConnectEvent()); });
            adapter.AsyncConnected();
            eventStart.Signal();
            Assert.IsTrue(eventRaised.WaitOne(3000), "No Receive any event");
            Assert.AreEqual(true, adapter.ConnectState);
            Assert.AreEqual(ConnectedType.Connected, connectedType);
        }
        [Test, Category("SyncConnected")]
        public void SyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            eventRaised = new AutoResetEvent(false);
            Task.Run(() => { eventStart.Wait(1000); adapter.Process(GetConnectEvent()); });
            adapter.SyncConnected();
            Assert.IsTrue(eventRaised.WaitOne(3000), "No Receive any event");
            Assert.AreEqual(true, adapter.ConnectState);
            Assert.AreEqual(ConnectedType.Connected, connectedType);
        }
        [Test, Category("FailAsyncConnected")]
        public void FailAsyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            eventRaised = new AutoResetEvent(false);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetNonConnectEvent()); });
            adapter.AsyncConnected();
            eventStart.Signal();
            Assert.IsTrue(eventRaised.WaitOne(3000), "No Receive any event");
            Assert.AreEqual(false, adapter.ConnectState);
            Assert.AreEqual(ConnectedType.Fail, connectedType);
        }
        [Test, Category("FailSyncConnected")]
        public void FailSyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            eventRaised = new AutoResetEvent(false);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetNonConnectEvent()); });
            adapter.SyncConnected();
            Assert.IsTrue(eventRaised.WaitOne(3000), "No Receive any event");
            Assert.AreEqual(false, adapter.ConnectState);
            Assert.AreEqual(ConnectedType.Fail, connectedType);
        }
        [Test, Category("ReConnect")]
        public void ReConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter(true);
            CountdownEvent eventStart = new CountdownEvent(1);
            eventRaised = new AutoResetEvent(false);
            Task.Run(() => { eventStart.Wait(1000); adapter.Process(GetConnectEvent()); });
            adapter.SyncConnected();
            adapter.Process(GetExpiredEvent());
            adapter.Process(GetConnectEvent());
            Assert.IsTrue(eventRaised.WaitOne(3000), "No Receive any event");
            Assert.AreEqual(true, adapter.ConnectState);
        }
        [Test, Category("DisConnect")]
        public void DisConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            eventRaised = new AutoResetEvent(false);
            Task.Run(() => { eventStart.Wait(1000); adapter.Process(GetConnectEvent()); });
            adapter.SyncConnected();
            adapter.Process(GetDisConnectedEvent());
            Assert.IsTrue(eventRaised.WaitOne(3000), "No Receive any event");
            Assert.AreEqual(false, adapter.ConnectState);
            Assert.AreEqual(ConnectedType.DisConnected, connectedType);
        }
        public WatchedEvent GetConnectEvent()
        {
            return new WatchedEvent(KeeperState.SyncConnected, EventType.None, "/Test");
        }
        public WatchedEvent GetExpiredEvent()
        {
            return new WatchedEvent(KeeperState.Expired, EventType.None, "/Test");
        }
        public WatchedEvent GetDisConnectedEvent()
        {
            return new WatchedEvent(KeeperState.Disconnected, EventType.None, "/Test");
        }
        public WatchedEvent GetNonConnectEvent()
        {
            return new WatchedEvent(KeeperState.Unknown, EventType.None, "/Test");
        }
        public ZooKeeperAdapter GetAdapter(bool reconnected = false)
        {
            ZooKeeperAdapter adapter = new ZooKeeperAdapter("127.0.0.1", "/Test", new TimeSpan(0, 0, 1), 2000, reconnected);
            adapter.ConnectedEvent += Adapter_ConnectedEvent;
            return adapter;
        }
        private void Adapter_ConnectedEvent(ConnectedType State)
        {
            connectedType = State;
            eventRaised.Set();
        }
    }
}

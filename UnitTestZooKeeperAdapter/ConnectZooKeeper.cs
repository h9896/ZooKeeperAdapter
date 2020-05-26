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
        [Test, Category("AsyncConnected")]
        public void NonSyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            CountdownEvent mainStart = new CountdownEvent(1);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetConnectEvent()); });
            adapter.AsyncConnected();
            eventStart.Signal();
            mainStart.Wait(1000);
            Assert.AreEqual(true, adapter.ConnectState);
        }
        [Test, Category("SyncConnected")]
        public void SyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetConnectEvent()); });
            adapter.SyncConnected();
            Assert.AreEqual(true, adapter.ConnectState);
        }
        [Test, Category("FailAsyncConnected")]
        public void FailNonSyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            CountdownEvent mainStart = new CountdownEvent(1);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetNonConnectEvent()); });
            adapter.AsyncConnected();
            eventStart.Signal();
            mainStart.Wait(1000);
            Assert.AreEqual(false, adapter.ConnectState);
        }
        [Test, Category("FailSyncConnected")]
        public void FailSyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetNonConnectEvent()); });
            adapter.SyncConnected();
            Assert.AreEqual(false, adapter.ConnectState);
        }
        [Test, Category("ReConnect")]
        public void ReConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter(true);
            CountdownEvent eventStart = new CountdownEvent(1);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetConnectEvent()); });
            adapter.SyncConnected();
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetExpiredEvent()); });
            eventStart.Wait(2000);
            Assert.AreEqual(true, adapter.ConnectState);
        }
        [Test, Category("DisConnect")]
        public void DisConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetConnectEvent()); });
            adapter.SyncConnected();
            adapter.Process(GetDisConnectedEvent());
            Assert.AreEqual(false, adapter.ConnectState);
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
        public ZooKeeperAdapter GetAdapter(bool reConnected = false)
        {
            return new ZooKeeperAdapter("/Test", "127.0.0.1", new TimeSpan(0, 0, 1), reConnected);
        }
    }
}

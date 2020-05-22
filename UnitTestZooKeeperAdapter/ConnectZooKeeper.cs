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
        [Test, Category("NonSyncConnected")]
        public void NonSyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            CountdownEvent mainStart = new CountdownEvent(1);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetConnectEvent()); });
            adapter.NonSyncConnected();
            eventStart.Signal();
            mainStart.Wait(1000);
            Assert.AreEqual(adapter.ConnectState, true);
        }
        [Test, Category("SyncConnected")]
        public void SyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetConnectEvent()); });
            adapter.SyncConnected();
            Assert.AreEqual(adapter.ConnectState, true);
        }
        [Test, Category("FailNonSyncConnected")]
        public void FailNonSyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            CountdownEvent mainStart = new CountdownEvent(1);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetNonConnectEvent()); });
            adapter.NonSyncConnected();
            eventStart.Signal();
            mainStart.Wait(1000);
            Assert.AreEqual(adapter.ConnectState, false);
        }
        [Test, Category("FailSyncConnected")]
        public void FailSyncConnected()
        {
            ZooKeeperAdapter adapter = GetAdapter();
            CountdownEvent eventStart = new CountdownEvent(1);
            Task.Run(() => { eventStart.Wait(2000); adapter.Process(GetNonConnectEvent()); });
            adapter.SyncConnected();
            Assert.AreEqual(adapter.ConnectState, false);
        }
        public WatchedEvent GetConnectEvent()
        {
            return new WatchedEvent(KeeperState.SyncConnected, EventType.None, "/Test");
        }
        public WatchedEvent GetNonConnectEvent()
        {
            return new WatchedEvent(KeeperState.Unknown, EventType.None, "/Test");
        }
        public ZooKeeperAdapter GetAdapter()
        {
            return new ZooKeeperAdapter("/Test", "127.0.0.1", new TimeSpan(0, 0, 1));
        }
    }
}

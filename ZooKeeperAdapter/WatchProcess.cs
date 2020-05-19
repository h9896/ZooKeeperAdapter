using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ZookeeperAdapter
{
    internal class WatchProcess : IWatcher
    {

        public void Process(WatchedEvent @event)
        {
            switch (@event.Type)
            {
                case EventType.None:
                    break;
                case EventType.NodeCreated:
                    break;
                case EventType.NodeDeleted:
                    break;
                case EventType.NodeDataChanged:
                    break;
                case EventType.NodeChildrenChanged:
                    break;
                default:
                    break;
            }
            throw new NotImplementedException();
        }
    }
}

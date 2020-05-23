using System.Linq;
using ZooKeeperNet;

namespace ZookeeperAdapter.Manage.Temporary
{
    internal class TemporyNode: ManageDynamic
    {
        private string _seq;
        private string _nodeName { get; set; }
        internal TemporyNode(string nodeName, string path)
        {
            _path = path;
            _nodeName = nodeName;
            ActiveNode = false;
        }
        internal void CreateSeq()
        {
            if (string.IsNullOrEmpty(_seq))
            {
                _seq = Zk.Create(_path + "/", _nodeName.GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.EphemeralSequential).Split('/').Last();
                UpdateState();
            }
            else 
            {
                UpdateState();
            }
        }
        internal void UpdateState()
        {
            if (Item.Keys.Min() != _seq) { ActiveNode = false; }
            else { ActiveNode = true; }
        }
    }
}

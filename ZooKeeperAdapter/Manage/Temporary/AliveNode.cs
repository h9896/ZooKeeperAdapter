using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ZookeeperAdapter.Manage.Temporary
{
    internal class AliveNode: ManageDynamic
    {
        public bool ActiveNode = false;
        private string _seq;
        internal void CreateSeq(string nodeName, string path)
        {
            if (string.IsNullOrEmpty(_seq))
            {
                _seq = zk.Create(path + "/", nodeName.GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.EphemeralSequential).Split('/').Last();
                UpdateState();
            }
            else 
            {
                UpdateState();
            }
        }
        internal void UpdateState()
        {
            if (item.Keys.Min() != _seq) { ActiveNode = false; }
            else { ActiveNode = true; }
        }
    }
}

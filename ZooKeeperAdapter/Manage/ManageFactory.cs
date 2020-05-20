using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZookeeperAdapter.Manage.Dynamic;
using ZookeeperAdapter.Manage.Static;
using ZookeeperAdapter.Manage.Temporary;

namespace ZookeeperAdapter.Manage
{
    internal class ManageFactory
    {
        public IZookeeperAdapter InitializeManage(string topic)
        {
            switch (topic.ToUpper())
            {
                case "USERONLINE":
                    return new UserOnline();
                case "CONFIG":
                    return new Config();
                case "ALIVENODE":
                    return new AliveNode();
                default:
                    throw new ArgumentOutOfRangeException($"Topic:({topic}) out of factory range!");
            }
        }
    }
}

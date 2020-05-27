using System.Collections.Generic;

namespace ZookeeperAdapter
{
    public enum ManageType
    {
        Dynamic,
        Static
    }
    public enum ConnectedType
    {
        Connected,
        DisConnected,
        Fail
    }
    public delegate void ItemEvent(Dictionary<string, string> Item);
    public delegate void Connected(ConnectedType State);
}

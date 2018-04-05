namespace APlusOrFail
{
    public delegate void EventHandler<TSender>(TSender sender);
    public delegate void EventHandler<TSender, TEventArgs>(TSender sender, TEventArgs eventArgs);
}

namespace Weave.Base.Interface
{
    /// <summary>
    /// 含有一个可传递一个泛型类的接口
    /// </summary>
    public interface IWeaveCommand
    {
        void RunCommand<T>(T DataSer, WeaveSockets mysoc);
    }
}

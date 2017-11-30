namespace Weave.Base.Interface
{
    /// <summary>
    /// 含有一个Run(WevaeSocketSession socketSession)方法的接口
    /// </summary>
    public interface IWeaveUniversal
    {
        bool Run(WevaeSocketSession socketSession);
    }
}

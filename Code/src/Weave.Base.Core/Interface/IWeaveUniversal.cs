namespace Weave.Base.Interface
{
    /// <summary>
    /// 含有一个Run方法的接口
    /// </summary>
    public interface IWeaveUniversal
    {
        bool Run(WevaeSocketSession socketSession);
    }
}

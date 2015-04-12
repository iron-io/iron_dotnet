namespace IronIO.Core
{
    public class IronTaskThatReturnsJson<TResult> : IronTask<TResult>
    {
        public IronTaskThatReturnsJson(IronTaskRequestBuilder taskBuilder) : base(taskBuilder)
        {
        }
    }
}
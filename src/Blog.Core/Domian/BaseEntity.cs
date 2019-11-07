namespace Blog.Core.Domian
{
    public abstract class BaseEntity<TKey>
    {
        public TKey Id { get; protected set; }
    }
}
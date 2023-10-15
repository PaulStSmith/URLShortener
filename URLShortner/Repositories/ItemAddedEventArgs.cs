using URLShortener.Common.Model;

namespace URLShortener.Repositories
{

    public partial class Repository<T> where T : IItem
    {
        /// <summary>
        /// Provide information about the item added.
        /// </summary>
        public sealed class ItemAddedEventArgs : ItemEventArgs
        {
            /// <summary>
            /// Creates a new instance of the <see cref="ItemAddedEventArgs"/>.
            /// </summary>
            /// <param name="item">The item added.</param>
            public ItemAddedEventArgs(T item) : base(item) { }
        }
    }
}

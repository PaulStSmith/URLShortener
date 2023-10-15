using URLShortener.Model;

namespace URLShortener.Repositories
{

    public partial class Repository<T> where T : IItem
    {
        /// <summary>
        /// Provide information about the item added.
        /// </summary>
        public sealed class ItemUpdatedEventArgs : ItemEventArgs
        {
            /// <summary>
            /// Gets the item prior the updated.
            /// </summary>
            public readonly T? OldItem;

            /// <summary>
            /// Creates a new instance of the <see cref="ItemAddedEventArgs"/>.
            /// </summary>
            /// <param name="oldItem">The item prior being updated.</param>
            /// <param name="item">The updated item.</param>
            public ItemUpdatedEventArgs(T? oldItem, T item) : base(item) => OldItem = oldItem;
        }
    }
}

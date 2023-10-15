using URLShortener.Common.Model;

namespace URLShortener.Repositories
{

    public partial class Repository<T> where T : IItem
    {
        /// <summary>
        /// Provide information about an item passed as argument of an event.
        /// </summary>
        public abstract class ItemEventArgs : EventArgs 
        {
            /// <summary>
            /// Gets the item that was added to the repository.
            /// </summary>
            public readonly T Item;

            /// <summary>
            /// Creates a new instance of the <see cref="ItemEventArgs"/>.
            /// </summary>
            /// <param name="item">The item added.</param>
            protected internal ItemEventArgs(T item) => Item = item;
        }
    }
}

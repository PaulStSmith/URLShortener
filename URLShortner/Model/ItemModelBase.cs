using System.Runtime.CompilerServices;
using URLShortener.Common.Model;

namespace URLShortener.Model
{
    /// <summary>
    /// Represents a base item object for a repository
    /// </summary>
    public abstract class ItemModelBase : IItem
    {
        /// <summary>
        /// Gets or sets an identifier for this object.
        /// </summary>
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Returns a clone of thins instance.
        /// </summary>
        /// <returns>A clone of thins instance.</returns>
        public abstract object Clone();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public virtual bool Equals(IItem? other)
        {
            return this.Id == other?.Id;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public override bool Equals(object? other)
        {
            return this.Equals((IItem?)other);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => this.Id.GetHashCode();
    }
}

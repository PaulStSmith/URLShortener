namespace URLShortener.Common.Model
{
    /// <summary>
    /// Describes an entity in the model.
    /// </summary>
    public interface IItem : ICloneable, IEquatable<IItem>
    {
        /// <summary>
        /// Gets the id of the entity.
        /// </summary>
        public int Id { get; }
    }
}

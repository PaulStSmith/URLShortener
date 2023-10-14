namespace URLShortener.Model
{
    /// <summary>
    /// Describes an entity in the model.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets the id of the entity.
        /// </summary>
        public int Id { get; }
    }
}

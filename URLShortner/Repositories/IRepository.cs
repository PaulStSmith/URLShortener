﻿using URLShortener.Common.Model;

namespace URLShortener.Repositories
{

    /// <summary>
    /// Describes a repository for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the repository.</typeparam>
    public interface IRepository<T> where T : IItem
    {
        /// <summary>
        /// Adds the specified <typeparamref name="T"/> to the repository.
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> to be added in the repository.</param>
        public T Add(T value);

        /// <summary>
        /// Updates the specified <typeparamref name="T"/> in the repository.
        /// </summary>
        /// <param name="value"></param>
        public T Update(T value);

        /// <summary>
        /// Returns all elements in the repository.
        /// </summary>
        /// <returns>All elements in the repository.</returns>
        public IEnumerable<T> GetAll();

        /// <summary>
        /// Returns an element from the repository with the specified id.
        /// </summary>
        /// <param name="id">The id of the element to be obtained from the repository.</param>
        /// <returns>An element from the repository with the specified id.</returns>
        public T? GetById(int id);

        /// <summary>
        /// Delete an element in the repository by its Id.
        /// </summary>
        /// <param name="id">The id of the element to delete.</param>
        public void DeleteById(int id);

        /// <summary>
        /// Deletes the specified <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> to be deleted</param>
        public void Delete(T value);

        /// <summary>
        /// Deletes all items in the repository.
        /// </summary>
        public void DeleteAll();
    }
}

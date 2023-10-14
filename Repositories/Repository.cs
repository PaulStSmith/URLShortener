using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc.Routing;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Mapping;
using System.Collections.Immutable;
using URLShortener.Model;

namespace URLShortener.Repositories
{

    /// <summary>
    /// Represents a repository for <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the repository.</typeparam>
    public class Repository<T> : IRepository<T>  where T : IEntity
    {
        /// <summary>
        /// Adds the specified <see cref="T"/> to the repository.
        /// </summary>
        /// <param name="value">The <see cref="T"/> to be added in the repository.</param>
        public void Add(T value)
        {
            ExecDB((s) => { s.Save(value); });
        }

        /// <summary>
        /// Deletes the specified <see cref="T"/>.
        /// </summary>
        /// <param name="value">The <see cref="T"/> to be deleted</param>
        public void Delete(T value)
        {
            ExecDB((s) => { s.Delete(value); });
        }

        /// <summary>
        /// Deletes all items in the repository.
        /// </summary>
        public void DeleteAll()
        {
            ExecDB((s) => { s.Query<T>().Delete(); });
        }

        /// <summary>
        /// Delete an element in the repository by its Id.
        /// </summary>
        /// <param name="id">The id of the element to delete.</param>
        public void DeleteById(int id)
        {
            ExecDB((s) => { s.Delete(GetById(id)); });
        }

        /// <summary>
        /// Returns all elements in the repository.
        /// </summary>
        /// <returns>All elements in the repository.</returns>
        public IEnumerable<T> GetAll()
        {
            return QueryDB().ToImmutableArray();
        }

        /// <summary>
        /// Returns an element from the repository with the specified id.
        /// </summary>
        /// <param name="id">The id of the element to be obtained from the repository.</param>
        /// <returns>An element from the repository with the specified id.</returns>
        public T? GetById(int id)
        {
            return QueryDB().Where(x=> x.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Updates the specified <see cref="T"/> in the repository.
        /// </summary>
        /// <param name="value"></param>
        public void Update(T value)
        {
            ExecDB((s) => { s.Update(value); });
        }

        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/> to be used by the repository.
        /// </summary>
        protected IQueryable<T> QueryDB()
        {
            var s = Program.SessionFactory.OpenSession();
            return s.Query<T>();
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> within a transaction.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to be executed.</param>
        protected void ExecDB(Action<NHibernate.ISession> action)
        {
            using (var s = Program.SessionFactory.OpenSession())
            using (var t = s.BeginTransaction())
                ExecDB(() => action(s), t);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> within the specified <see cref="ITransaction"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to be executed.</param>
        protected void ExecDB(Action action, ITransaction transaction)
        {
            try
            {
                action();
                transaction.Commit();
            }
            catch
            {
                transaction?.Rollback();
                throw;
            }
        }
    }
}

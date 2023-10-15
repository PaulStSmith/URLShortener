using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc.Routing;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Mapping;
using System.Collections.Immutable;
using URLShortener.Common.Model;

namespace URLShortener.Repositories
{

    /// <summary>
    /// Represents a repository for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the repository.</typeparam>
    public partial class Repository<T> : IRepository<T>  where T : IItem
    {
        /// <summary>
        /// An instance of the <see cref="EmptyInterceptor"/> for protected internal use.
        /// </summary>
        protected internal readonly IInterceptor ei = EmptyInterceptor.Instance;

        /// <summary>
        /// Raised every time an item is added to the repository.
        /// </summary>
        public event EventHandler<ItemAddedEventArgs> ItemAdded;

        /// <summary>
        /// Raised every time an item is updated to the repository.
        /// </summary>
        public event EventHandler<ItemUpdatedEventArgs> ItemUpdated;

        /// <summary>
        /// Raised every time an item is deleted to the repository.
        /// </summary>
        public event EventHandler<ItemDeletedEventArgs> ItemDeleted;

        /// <summary>
        /// Adds the specified item to the repository.
        /// </summary>
        /// <param name="value">The item to be added in the repository.</param>
        /// <returns>The item added.</returns>
        public T Add(T value) => Add(value, ei);

        /// <summary>
        /// Adds the specified item to the repository.
        /// </summary>
        /// <param name="value">The item to be added in the repository.</param>
        /// <param name="interceptor">An <see cref="IInterceptor"/> instance that can be used to modify the query.</param>
        /// <returns>The item added.</returns>
        protected internal T Add(T value, IInterceptor interceptor)
        {
            ExecDB((s) =>
            {
                if (value.Id != 0)
                    s.Save(value, value.Id);
                else
                    s.Save(value);

            }, interceptor);
            ItemAdded?.Invoke(this, new ItemAddedEventArgs(value));
            return value;
        }

        /// <summary>
        /// Deletes the specified <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> to be deleted</param>
        public void Delete(T? value) => Delete(value, ei);

        /// <summary>
        /// Deletes the specified <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> to be deleted</param>
        /// <param name="interceptor">An <see cref="IInterceptor"/> instance that can be used to modify the query.</param>
        protected internal void Delete(T? value, IInterceptor interceptor)
        {
            if (value == null) return;
            ItemDeleted?.Invoke(this, new ItemDeletedEventArgs(value));
            ExecDB((s) => { s.Delete(value); }, interceptor);
        }

        /// <summary>
        /// Deletes all items in the repository.
        /// </summary>
        public void DeleteAll() => DeleteAll(ei);

        /// <summary>
        /// Deletes all items in the repository.
        /// </summary>
        /// <param name="interceptor">An <see cref="IInterceptor"/> instance that can be used to modify the query.</param>
        protected internal void DeleteAll(IInterceptor interceptor) => ExecDB((s) => { s.Query<T>().Delete(); }, interceptor);

        /// <summary>
        /// Delete an element in the repository by its Id.
        /// </summary>
        /// <param name="id">The id of the element to delete.</param>
        public void DeleteById(int id) => DeleteById(id, ei);

        /// <summary>
        /// Delete an element in the repository by its Id.
        /// </summary>
        /// <param name="id">The id of the element to delete.</param>
        /// <param name="interceptor">An <see cref="IInterceptor"/> instance that can be used to modify the query.</param>
        protected internal void DeleteById(int id, IInterceptor interceptor) => Delete(GetById(id), interceptor);

        /// <summary>
        /// Returns all elements in the repository.
        /// </summary>
        /// <returns>All elements in the repository.</returns>
        public IEnumerable<T> GetAll() => QueryDB().ToImmutableArray();

        /// <summary>
        /// Returns an element from the repository with the specified id.
        /// </summary>
        /// <param name="id">The id of the element to be obtained from the repository.</param>
        /// <returns>An element from the repository with the specified id.</returns>
        public T? GetById(int id) => QueryDB().Where(x => x.Id == id).FirstOrDefault();

        /// <summary>
        /// Updates the specified <typeparamref name="T"/> in the repository.
        /// </summary>
        /// <param name="value"></param>
        public T Update(T value) => Update(value, ei);

        /// <summary>
        /// Updates the specified <typeparamref name="T"/> in the repository.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="interceptor">An <see cref="IInterceptor"/> instance that can be used to modify the query.</param>
        public T Update(T value, IInterceptor interceptor)
        {
            var oldValue = (T?)value.Clone();
            ExecDB((s) => { s.Update(value); }, interceptor);
            ItemUpdated?.Invoke(this, new ItemUpdatedEventArgs(oldValue, value));
            return value;
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
        protected void ExecDB(Action<NHibernate.ISession> action) => ExecDB((s) => action(s), ei);

        /// <summary>
        /// Executes the specified <see cref="Action"/> within a transaction.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to be executed.</param>
        /// <param name="interceptor">An instance of <see cref="IInterceptor"/> that can be used to modify the query before being sent to the database.</param>
        protected void ExecDB(Action<NHibernate.ISession> action, IInterceptor interceptor)
        {
            using var s = Program.SessionFactory
                                 .WithOptions()
                                 .Interceptor(interceptor)
                                 .OpenSession();
            using var t = s.BeginTransaction();
            ExecDB(() => action(s), t);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> within the specified <see cref="ITransaction"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to be executed.</param>
        /// <param name="transaction">An <see cref="ITransaction"/> object within which the action will be executed.</param>
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

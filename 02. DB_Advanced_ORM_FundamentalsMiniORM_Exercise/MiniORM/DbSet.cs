using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace MiniORM
{
    public class DbSet<TEntity> : ICollection<TEntity>
            where TEntity : class, new()
    {
        public DbSet(IEnumerable<TEntity> entities)
        {
            this.Entities = entities.ToList();
            this.ChangeTraker = new ChangeTraker<TEntity>(entities);
        }
        internal ChangeTraker<TEntity> ChangeTraker { get; set; }
        internal IList<TEntity> Entities { get; set; }

        public void Add(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item),
                    "Item cannot be null");
            }

            this.Entities.Add(item);
            this.ChangeTraker.Add(item);
        }

        public void Clear()
        {
            while (this.Entities.Any())
            {
                TEntity entity = this.Entities.First();
                this.Remove(entity);
            }
        }

        public bool Contains(TEntity item) => this.Entities.Contains(item);

        public void CopyTo(TEntity[] array, int startIndex)
        {
            this.Entities.CopyTo(array, startIndex);
        }

        public int Count => this.Entities.Count;

        public bool IsReadOnly => this.Entities.IsReadOnly;

        public bool Remove(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }

            bool removedSuccessfully = this.Entities.Remove(item);

            if (removedSuccessfully)
            {
                this.ChangeTraker.Remove(item);
            }

            return removedSuccessfully;
        }

        public void RemovedRange(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities.ToArray())
            {
                this.Remove(entity);
            }
        }


        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
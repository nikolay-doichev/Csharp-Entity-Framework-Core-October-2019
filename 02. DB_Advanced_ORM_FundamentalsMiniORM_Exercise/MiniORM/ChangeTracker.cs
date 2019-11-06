﻿using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniORM
{
	internal class ChangeTraker<T>
        where T : class, new()
    {
        private readonly List<T> allEntities;

        private readonly List<T> added;

        private readonly List<T> removed;

        public ChangeTraker(IEnumerable<T> entities)
        {
            this.added = new List<T>();
            this.removed = new List<T>();

            this.allEntities = CloneEntities(entities);
        }

        public IReadOnlyCollection<T> AllEntities => 
            this.allEntities.AsReadOnly();
        public IReadOnlyCollection<T> Added =>
            this.added.AsReadOnly();
        public IReadOnlyCollection<T> Removed =>
            this.removed.AsReadOnly();

        public void Add(T item) => this.added.Add(item);

        public void Remove(T item) => this.removed.Add(item);

        public IEnumerable<T> GetModifiedEntities(DbSet<T> dbSet)
        {
            List<T> modifiedEntities = new List<T>();

            PropertyInfo[] primaryKeys = typeof(T)
                .GetProperties()
                .Where(pi => pi.HasAttribute<KeyAttribute>())
                .ToArray();

            foreach (var proxyEntity in this.AllEntities)
            {
                object[] primaryKeyValues =
                    GetPrimaryKeyValues(primaryKeys, proxyEntity)
                    .ToArray();


                T entity = dbSet
                    .Entities
                    .Single(e => GetPrimaryKeyValues(primaryKeys, e).SequenceEqual(primaryKeyValues));

                bool isModified = IsModified(proxyEntity, entity);

                if (isModified)
                {
                    modifiedEntities.Add(entity);
                }
            }
            return modifiedEntities;
        }

        private static IEnumerable<object> GetPrimaryKeyValues(IEnumerable<PropertyInfo> primaryKeys, T entity)
        {
            return primaryKeys.Select(pi => pi.GetValue(entity));
        }

        private static bool IsModified(T proxyEntity, T realEntity)
        {
            PropertyInfo[] monitoredProperties = typeof(T)
                .GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
                .ToArray();

            PropertyInfo[] modifiedProperties = monitoredProperties
                .Where(pi => !Equals(pi.GetValue(realEntity), pi.GetValue(proxyEntity)))
                .ToArray();

            bool isModified = modifiedProperties.Any();
            return isModified;
        }



        private static List<T> CloneEntities(IEnumerable<T> entites)
        {
            List<T> cloneEntities = new List<T>();

            PropertyInfo[] propertiesToClone = typeof(T)
                .GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
                .ToArray();

            foreach (T entity in entites)
            {
                T clonedEntity = Activator.CreateInstance<T>();

                foreach (PropertyInfo property in propertiesToClone)
                {
                    object value = property.GetValue(entity);
                    property.SetValue(clonedEntity, value);

                }
                    cloneEntities.Add(clonedEntity);
            }

            return cloneEntities;
        }
    }
}
﻿using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniORM
{
    public abstract class DbContext
    {
        private readonly DatabaseConnection connection;

        private readonly Dictionary<Type, PropertyInfo> dbSetProperties;

        protected DbContext(string connectionString)
        {
            this.connection = new DatabaseConnection(connectionString);

            this.dbSetProperties = this.DiscoverDbSet();

            using (new ConnectionManager(this.connection))
            {
                this.InitializeDbSets();
            }

            this.MapAllRelations();
        }

        internal static Type[] AllowedSqlTypes =
        {
            typeof(string),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(decimal),
            typeof(bool),
            typeof(DateTime)
        };

        public void SaveChages()
        {
            object[] dbSets = this.dbSetProperties
                .Select(pi => pi.Value.GetValue(this))
                .ToArray();

            foreach (IEnumerable<object> dbSet in dbSets)
            {
                object[] invalidEntities = dbSet.Where(enitity => !IsObjectValid(enitity))
                    .ToArray();

                if (invalidEntities.Any())
                {
                    throw new InvalidOperationException($"{invalidEntities.Length} Invalid Entities found in {dbSet.GetType().Name}!");
                }
            }

            using (new ConnectionManager(this.connection))
            {
                using (SqlTransaction transaction = this.connection.StartTransaction())
                {
                    foreach (IEnumerable dbSet in dbSets)
                    {
                        Type dbSetType = dbSet.GetType()
                                             .GetGenericArguments()
                                             .First();

                        MethodInfo persistMethod = typeof(DbContext)
                            .GetMethod("Persist", BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(dbSetType);


                        try
                        {
                            persistMethod.Invoke(this, new object[] { dbSet });
                        }
                        catch (TargetInvocationException tie)
                        {
                            throw tie.InnerException;
                        }
                        catch(InvalidOperationException)
                        {
                            transaction.Rollback();
                            throw;
                        }
                        catch (SqlException)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                    transaction.Commit();
                }
            }
        }
        private void Persist<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            string tableName = this.GetTableName(typeof(TEntity));

            string[] columns = this.connection.FetchColumnNames(tableName).ToArray();

            if (dbSet.ChangeTraker.Added.Any())
            {
                this.connection.InsertEntities(dbSet.ChangeTraker.Added, tableName, columns);
            }

            TEntity[] modifiedEntities = dbSet
                .ChangeTraker
                .GetModifiedEntities(dbSet)
                .ToArray();

            if (modifiedEntities.Any())
            {
                this.connection.UpdateEntities(modifiedEntities, tableName, columns);
            }

            if (dbSet.ChangeTraker.Removed.Any())
            {
                this.connection.DeleteEntities(dbSet.ChangeTraker.Removed, tableName, columns);
            }
        }
        private void InitializeDbSets()
        {
            foreach (var dbSet in this.dbSetProperties)
            {
                Type dbSetType = dbSet.Key;
                PropertyInfo dbSetProperty = dbSet.Value;

                MethodInfo populateDbSetGeneric = typeof(DbContext)
                    .GetMethod("PopulateDbSet", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(dbSetType);

                populateDbSetGeneric.Invoke(this, new object[] { dbSetProperty });
            }
        }
        private void PopulateDbSet<TEntity>(PropertyInfo dbSet)
            where TEntity : class, new()
        {
            IEnumerable<TEntity> entities = LoadTableEntities<TEntity>();

            DbSet<TEntity> dbSetInstance = new DbSet<TEntity>(entities);
            ReflectionHelper.ReplaceBackingField(this, dbSet.Name, dbSetInstance);
        }
        private void MapAllRelations()
        {
            foreach (var dbSetProperty in this.dbSetProperties)
            {
                Type dbSetType = dbSetProperty.Key;
                object dbSet = dbSetProperty.Value.GetValue(this);

                MethodInfo mapRelationsGeneric = typeof(DbContext)
                    .GetMethod("MapRelations", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(dbSetType);

                mapRelationsGeneric.Invoke(this, new object[] { dbSet });
            }
        }
        private void MapRelations<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            MapNavigationProperties(dbSet);

            PropertyInfo[] collections = entityType
                .GetProperties()
                .Where(pi =>
                            pi.PropertyType.IsGenericType && 
                            pi.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                .ToArray();

            foreach (PropertyInfo collection in collections)
            {
                Type collectionType = collection
                    .PropertyType
                    .GenericTypeArguments
                    .First();

                MethodInfo mapColletionMethod = typeof(DbContext)
                    .GetMethod("MapCollection", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entityType ,collectionType);

                mapColletionMethod.Invoke(this, new object[] { dbSet, collection });
            }
        }
        private void MapCollection<TDbSet, TCollection>(DbSet<TDbSet> dbSet, PropertyInfo collectionProperty)
            where TDbSet : class, new() 
            where TCollection : class, new()
        {
            Type entityType = typeof(TDbSet);
            Type colletionType = typeof(TCollection);

            PropertyInfo[] primaryKeys = colletionType
                .GetProperties()
                .Where(pi => pi.HasAttribute<KeyAttribute>())
                .ToArray();

            PropertyInfo primaryKey = primaryKeys.First();
            PropertyInfo foreighKey = entityType
                .GetProperties()
                .First(pi => pi.HasAttribute<KeyAttribute>());

            bool isManyToMany = primaryKeys.Length >= 2;

            if (isManyToMany)
            {
                primaryKey = colletionType
                    .GetProperties()
                    .First(pi => colletionType.GetProperty(pi.GetCustomAttribute<ForeignKeyAttribute>().Name)
                   .PropertyType == entityType);
            }

            DbSet<TCollection> navigationDbSet = (DbSet<TCollection>)this
                .dbSetProperties[colletionType]
                .GetValue(this);

            foreach (TDbSet entity in dbSet)
            {
                object primaryKeyValue = foreighKey.GetValue(entity);

                var navigationEntities = navigationDbSet
                    .Where(ne => primaryKey.GetValue(ne).Equals(primaryKeyValue))
                    .ToArray();

                ReflectionHelper.ReplaceBackingField(entity, collectionProperty.Name, navigationEntities);
            }
        }
        private void MapNavigationProperties<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);

            PropertyInfo[] foreignKeys = entityType
                .GetProperties()
                .Where(pi => pi.HasAttribute<ForeignKeyAttribute>())
                .ToArray();

            foreach (PropertyInfo foreignKey in foreignKeys)
            {
                string navigationPropertyName = foreignKey
                    .GetCustomAttribute<ForeignKeyAttribute>().Name;

                PropertyInfo navigationPropertie = entityType
                    .GetProperty(navigationPropertyName);

                object navigationDbSet = this.dbSetProperties[navigationPropertie.PropertyType]
                    .GetValue(this);

                PropertyInfo navigationPrimaryKey = navigationPropertie.PropertyType
                    .GetProperties()
                    .First(pi => pi.HasAttribute<KeyAttribute>());

                foreach (TEntity entity in dbSet)
                {
                    object foreignKeyValue = foreignKey.GetValue(entity);

                    object navigationPropertyValue = ((IEnumerable<object>)navigationDbSet)
                        .First(cnp => navigationPrimaryKey.GetValue(cnp).Equals(foreignKeyValue));

                    navigationPropertie.SetValue(entity, navigationPropertyValue);
                }
            }
        }
        private static bool IsObjectValid(object e)
        {
            ValidationContext validationContext = new ValidationContext(e);
            List<ValidationResult> validationErrors = new List<ValidationResult>();

            bool validationResult = Validator
                .TryValidateObject(e, validationContext, validationErrors, true);

            return validationResult;
        }
        private IEnumerable<TEntity> LoadTableEntities<TEntity>()
            where TEntity : class
        {
            Type table = typeof(TEntity);

            string[] columns = GetEntityColumnNames(table);

            string tableName = GetTableName(table);

            TEntity[] fetchedRows = this.connection.FetchResultSet<TEntity>(tableName, columns)
                .ToArray();

            return fetchedRows;
        }
        private string GetTableName(Type tableType)
        {
            string tableName = ((TableAttribute)tableType.GetCustomAttribute<TableAttribute>())?.Name;
                       
            if (tableName == null)
            {
                tableName = this.dbSetProperties[tableType].Name;
            }

            return tableName;
        }
        private Dictionary<Type, PropertyInfo> DiscoverDbSet()
        {
            Dictionary<Type, PropertyInfo> dbSets = this.GetType()
                .GetProperties()
                .Where(pi => pi.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToDictionary(pi => pi.PropertyType.GetGenericArguments().First(), pi => pi);

            return dbSets;
        }
        private string[] GetEntityColumnNames(Type table)
        {
            string tableName = GetTableName(table);
            IEnumerable<string> dbColumns = this.connection.FetchColumnNames(tableName);

            string[] columns = table
                .GetProperties()
                .Where(pi => dbColumns.Contains(pi.Name) &&
                        !pi.HasAttribute<NotMappedAttribute>() &&
                        AllowedSqlTypes.Contains(pi.PropertyType))
                .Select(pi => pi.Name)
                .ToArray();

            return columns;

        }
    }
}
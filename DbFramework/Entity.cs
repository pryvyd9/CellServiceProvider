using Npgsql;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DbFramework
{
  

  

   

    //public abstract class Transact
    //{
    //    #region TObject

    //    protected internal abstract class TObject
    //    {

    //    }

    //    protected internal sealed class TEntity : TObject
    //    {
    //        internal Entity Entity;
    //    }

    //    protected internal sealed class TSavepoint : TObject
    //    {
    //        internal string Name;
    //    }

    //    #endregion

    //    #region Transact Action

    //    protected internal abstract class TransactAction
    //    {

    //    }

    //    protected internal sealed class GetCommandAction : TransactAction
    //    {
    //        internal Func<NpgsqlCommand> Action;
    //    }

    //    protected internal abstract class SavePoint : TransactAction
    //    {
    //        internal string Name;

    //    }

    //    protected internal sealed class CreateSavePoint : SavePoint
    //    {
    //    }

    //    protected internal sealed class RemoveSavePoint : SavePoint
    //    {
    //    }

    //    protected internal sealed class RollbackOnException<T> : SavePoint where T : Exception
    //    {
    //        internal Predicate<T> Predicate;
    //    }

    //    #endregion

    //    #region Thread Synchronization

    //    internal sealed class TransactParams
    //    {
    //        internal bool[] Statuses;
    //        internal int CompleteCount;
    //    }

    //    #endregion


    //    protected readonly IEnumerable<(TObject obj, TransactAction action)> Actions;

    //    protected TObject CurrentObject { get; }

    //    protected Transact(TObject obj, TransactAction action)
    //    {
    //        Actions = new[] { (obj, action) };
    //    }

    //    protected Transact(TObject obj, IEnumerable<(TObject obj, TransactAction action)> actions)
    //    {
    //        CurrentObject = obj;
    //        Actions = actions;
    //    }

    //    public void Commit()
    //    {
    //        // Sort actions by contexts.
    //        var uniqueContexts = Actions
    //            .Select(n => n.obj)
    //            .OfType<TEntity>()
    //            .GroupBy(n => n.Entity.Context)
    //            .ToDictionary(n => n.Key, n => new List<TransactAction>());


    //        foreach (var (obj, action) in Actions)
    //        {
    //            if (obj is TSavepoint sp)
    //            {
    //                foreach (var context in uniqueContexts.Keys)
    //                {
    //                    uniqueContexts[context].Add(action);
    //                }
    //            }
    //            else if (obj is TEntity e)
    //            {
    //                uniqueContexts[e.Entity.Context].Add(action);
    //            }
    //            else
    //            {
    //                throw new Exception("Such a type is not supported.");
    //            }
    //        }


    //        // Fill savepoints.
    //        var savepoints = new List<List<TransactAction>>();

    //        foreach (var action in Actions.Select(n => n.action).OfType<SavePoint>())
    //        {
    //            switch (action)
    //            {
    //                case CreateSavePoint createSavePoint:
    //                    savepoints.Add(new List<TransactAction> { createSavePoint });
    //                    break;
    //                case RemoveSavePoint removeSavePoint:
    //                    savepoints.Add(new List<TransactAction> { removeSavePoint });
    //                    break;
    //                default:
    //                    var saveList = savepoints.Last(n => n.OfType<CreateSavePoint>().First().Name == action.Name);
    //                    saveList.Add(action);
    //                    break;
    //            }
    //        }

    //        // Fill sessions.

    //        var contextedSessions = new Dictionary<DbContext, List<(List<TransactAction> savepoint, List<TransactAction> entity)>>();

    //        foreach (var context in uniqueContexts)
    //        {
    //            foreach (var action in context.Value)
    //            {
    //                if (action is CreateSavePoint || action is RemoveSavePoint)
    //                {
    //                    contextedSessions[context.Key] = new List<(List<TransactAction>, List<TransactAction>)>
    //                    {
    //                        (savepoints.Find(n => n.First() == action), new List<TransactAction>())
    //                    };
    //                }
    //                else if (!(action is SavePoint))
    //                {
    //                    if (!contextedSessions.ContainsKey(context.Key))
    //                    {
    //                        contextedSessions[context.Key] = new List<(List<TransactAction>, List<TransactAction>)>
    //                        {
    //                            (savepoints.Find(n => n.First() == action), new List<TransactAction>())
    //                        };
    //                    }
    //                    else
    //                    {
    //                        contextedSessions[context.Key].Last().Item2.Add(action);
    //                    }
    //                }
    //            }
    //        }

    //        // Execute sessions.

    //        var param = new TransactParams
    //        {
    //            Statuses = new bool[contextedSessions.Count],
    //        };

    //        foreach (var contextedSession in contextedSessions)
    //        {
    //            var context = contextedSession.Key;
    //            var session = contextedSession.Value;

    //            context.ExecuteTransaction(session, param);
    //        }


    //    }
    //}

    //public sealed class TransactEntity : Transact
    //{

    //    private new TEntity CurrentObject => (TEntity)base.CurrentObject;

    //    internal TransactEntity(Entity entity, Func<NpgsqlCommand> action) 
    //        : base(new TEntity { Entity = entity }, new GetCommandAction { Action = action })
    //    {

    //    }

    //    internal TransactEntity(TEntity entity, IEnumerable<(TObject obj, TransactAction action)> actions)
    //       : base(entity, actions)
    //    {

    //    }

    //    public TransactEntity CommitWith(Entity entity)
    //    {
    //        if (entity is null)
    //        {
    //            throw new NullReferenceException("Entity was null.");
    //        }

    //        if (CurrentObject is null)
    //        {
    //            throw new NullReferenceException("CurrentObject was null.");
    //        }

    //        var action = (CurrentObject, new GetCommandAction { Action = CurrentObject.Entity.CreateCommitCommand });

    //        return new TransactEntity(new TEntity { Entity = entity }, Actions.Append(action));
    //    }

    //    public TransactEntity DeleteWith(Entity entity)
    //    {
    //        if (entity is null)
    //        {
    //            throw new NullReferenceException("Entity was null.");
    //        }

    //        if (CurrentObject is null)
    //        {
    //            throw new NullReferenceException("CurrentObject was null.");
    //        }

    //        var action = (CurrentObject, new GetCommandAction { Action = CurrentObject.Entity.CreateDeleteCommand });

    //        return new TransactEntity(new TEntity { Entity = entity }, Actions.Append(action));
    //    }

    //    public TransactSavepoint CommitWith(string savepoint)
    //    {
    //        if (string.IsNullOrWhiteSpace(savepoint))
    //        {
    //            throw new NullReferenceException("savepoint Name was empty.");
    //        }

    //        if (CurrentObject is null)
    //        {
    //            throw new NullReferenceException("CurrentObject was null.");
    //        }

    //        var action = (CurrentObject, new GetCommandAction { Action = CurrentObject.Entity.CreateCommitCommand });

    //        return new TransactSavepoint(new TSavepoint { Name = savepoint }, Actions.Append(action));
    //    }


    //}

    //public sealed class TransactSavepoint : Transact
    //{
    //    private new TSavepoint CurrentObject => (TSavepoint)base.CurrentObject;

    //    internal TransactSavepoint(string savepoint, SavePoint action)
    //        : base(new TSavepoint { Name = savepoint }, action)
    //    {

    //    }

    //    internal TransactSavepoint(TSavepoint entity, IEnumerable<(TObject obj, TransactAction action)> actions)
    //        : base(entity, actions)
    //    {

    //    }

    //    public TransactEntity SaveWith(Entity entity)
    //    {
    //        if (entity is null)
    //        {
    //            throw new NullReferenceException("Entity was null.");
    //        }

    //        if (CurrentObject is null)
    //        {
    //            throw new NullReferenceException("CurrentObject was null.");
    //        }

    //        var action = (CurrentObject, new CreateSavePoint { Name = CurrentObject.Name });

    //        return new TransactEntity(new TEntity { Entity = entity }, Actions.Append(action));
    //    }

    //    public TransactEntity RemoveWith(Entity entity)
    //    {
    //        if (entity is null)
    //        {
    //            throw new NullReferenceException("Entity was null.");
    //        }

    //        if (CurrentObject is null)
    //        {
    //            throw new NullReferenceException("CurrentObject was null.");
    //        }

    //        var action = (CurrentObject, new RemoveSavePoint { Name = CurrentObject.Name });

    //        return new TransactEntity(new TEntity { Entity = entity }, Actions.Append(action));
    //    }

    //    public TransactSavepoint RollBackWith<T>(Predicate<T> predicate = null) where T : Exception
    //    {
    //        if (CurrentObject is null)
    //        {
    //            throw new NullReferenceException("savepoint Name was empty or null.");
    //        }

    //        var action = (CurrentObject, new RollbackOnException<T> { Name = CurrentObject.Name, Predicate = predicate });

    //        return new TransactSavepoint(CurrentObject, Actions.Append(action));
    //    }

    //}





    public abstract class Entity
    {
        internal DbContext Context { get; }

        protected Entity(DbContext context)
        {
            Context = context;
        }


        internal Dictionary<string, object> GetValues(IEnumerable<PropertyInfo> properties)
        {
            var values = new Dictionary<string, object>();

            foreach (var (property, value) in properties.Select(n => (n, (IDbField)n.GetValue(this))))
            {
                var attributes = property.GetCustomAttributes(false);

                if (!value.IsAssigned)
                {
                    if (Has<DefaultOverrideAttribute>(out var defaultOverrideAttribute))
                    {
                        var defaultValue = defaultOverrideAttribute.Value;

                        if (defaultValue == null)
                        {
                            if (Has<NullableAttribute>(out _))
                            {
                                Add(DBNull.Value);
                            }
                            else
                            {
                                throw new Exception($"Non nullable field {TableName()}.{FieldName()} ({TableClassName()}.{PropertyName()}) cannot have null as default value.");
                            }
                        }
                        else
                        {
                            Add(defaultValue);
                        }
                    }
                    else
                    {
                        if (!Has<DefaultAttribute>(out _))
                        {
                            if (value.IsNull && Has<NullableAttribute>(out _))
                            {
                                Add(DBNull.Value);
                            }
                            else
                            {
                                throw new Exception($"Field {TableName()}.{FieldName()} ({TableClassName()}.{PropertyName()}) with no default value was not assigned.");
                            }
                        }
                    }
                }
                else if (value.IsNull)
                {
                    if (Has<NullableAttribute>(out _))
                    {
                        Add(DBNull.Value);
                    }
                    else
                    {
                        throw new Exception($"Not nullable field {TableName()}.{FieldName()} ({TableClassName()}.{PropertyName()}) was assigned with null.");
                    }
                }
                else
                {
                    Add(value.Value);
                }

                // Functions

                bool Has<T>(out T attribute) => (attribute = attributes.OfType<T>().FirstOrDefault()) != null;

                void Add(object val)
                {
                    var name = attributes.OfType<FieldAttribute>().Single().Name;

                    values.Add(name, val);
                }

                string PropertyName() => property.Name;

                string FieldName() => attributes.OfType<FieldAttribute>().Single().Name;

                string TableClassName() => property.DeclaringType?.FullName;

                string TableName() => property.DeclaringType?.GetCustomAttributes(false).OfType<TableAttribute>().Single().Name;
            }

            return values;

        }

        public virtual void Commit()
        {
            Context.Commit(Context.CommandFactory.Commit(this));
        }

        public virtual void Delete()
        {
            Context.Commit(Context.CommandFactory.Delete(this));
        }
    }
}

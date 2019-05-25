using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using CellServiceProvider.Models;
using DbFramework;
using Microsoft.Extensions.Configuration;

namespace WebClient22.Controllers
{
    public class DbController : Controller
    {
        IConfiguration configuration;

        public DbController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // GET: Db
        public ActionResult Index()
        {
            var availableTables = Assembly.GetExecutingAssembly().DefinedTypes
               .Where(n => typeof(Entity).IsAssignableFrom(n));

            if (!availableTables.Any())
            {
                throw new Exception("No table was found");
            }

            Type type = availableTables.First();

            return PrintTable(type);
        }


        public ActionResult ShowTable(string __entityType)
        {
            var type = Type.GetType(__entityType);

            return PrintTable(type);
        }

        public ActionResult Delete(int __index, string __entityType)
        {
            try
            {
                if (HttpContext.Session.GetInt32("dbContext") is null)
                {
                    throw new Exception("dbContext was not set");
                }

                var type = Type.GetType(__entityType);

                SelectAll(type).ElementAt(__index).Delete();

                return PrintTable(type);
            }
            catch (Exception e)
            {
                ViewData["Message"] = e.Message;

                return ShowTable(__entityType);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int __index, string __entityType, IFormCollection collection)
        {
            try
            {
                var entities = SelectAll(Type.GetType(__entityType));

                var entity = entities.ElementAt(__index);

                var excludedNames = new[] { "__RequestVerificationToken", "__entityType", "__index" };

                var values = collection
                    .Where(n => !string.IsNullOrWhiteSpace(n.Value.FirstOrDefault()))
                    .Where(n => !excludedNames.Contains(n.Key))
                    .ToDictionary(n => n.Key, n => (object)n.Value.FirstOrDefault());


                entity.InitializeWithConvert(values);

                entity.Commit();

                return ShowTable(__entityType);
            }
            catch(Exception e)
            {
                ViewData["Message"] = e.Message;

                return ShowTable(__entityType);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(string __entityType, IFormCollection collection)
        {
            try
            {
                var type = Type.GetType(__entityType);

                var entity = Instantiate(type);

                var excludedNames = new[] { "__RequestVerificationToken", "__entityType" };

                var values = collection
                    .Where(n => !string.IsNullOrWhiteSpace(n.Value.FirstOrDefault()))
                    .Where(n => !excludedNames.Contains(n.Key))
                    .ToDictionary(n => n.Key, n => (object)n.Value.FirstOrDefault());

                entity.InitializeWithConvert(values);

                entity.Commit();

                return PrintTable(type);
            }
            catch (Exception e)
            {
                ViewData["Message"] = e.Message;

                return ShowTable(__entityType);
            }
        }




        public ActionResult PrintTable(Type type)
        {
            var availableTables = Assembly.GetExecutingAssembly().DefinedTypes
             .Where(n => typeof(Entity).IsAssignableFrom(n));

            if (!availableTables.Any())
            {
                throw new Exception("No table was found");
            }

            var availableTablesNames = availableTables
               .ToDictionary(n =>Entity.GetTableName(n), n => n.FullName);

            if (HttpContext.Session.GetInt32("dbContext") is null)
            {
                string connString = Get("DBConnection:ConnectionString");

                var dbContext = new ProviderContext(connString)
                {
                    CommandFactory = new DbFramework.NpgsqlCommandFactory(),
                    ConnectionFactory = new DbFramework.NpgsqlConnectionFactory(),
                };

                HttpContext.Session.SetInt32("dbContext", ContextHolder.Instance.Contexts.Count);

                ContextHolder.Instance.Contexts.Add(dbContext);
            }

            var entities = SelectAll(type);

            var model = new WebClient22.Models.TableProvider()
            {
                Entities = entities.ToArray(),
                EntityType = type,
                FieldInfos = Entity.GetFieldInfos(type),
                AvailableTables = availableTablesNames,
            };

            return View("Index", model);

            string Get(string path) => configuration.GetValue<string>(path);
        }



        private DbContext GetDbContext()
        {
            return ContextHolder.Instance.Contexts[HttpContext.Session.GetInt32("dbContext").Value];
        }

        private IEnumerable<Entity> SelectAll(Type type)
        {
            var selectAllFunc = typeof(DbContext).GetMethod("SelectAll");

            var dbContext = GetDbContext();

            var entities = (IEnumerable<Entity>)selectAllFunc.MakeGenericMethod(type).Invoke(dbContext, null);

            return entities;
        }

        private Entity Instantiate(Type type)
        {
            var entity = (Entity)type.GetConstructor(new[] { typeof(DbContext) }).Invoke(new[] { GetDbContext() });

            return entity;
        }

    }
}
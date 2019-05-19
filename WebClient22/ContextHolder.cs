using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DbFramework;

namespace WebClient22
{
    public sealed class ContextHolder
    {
        private static ContextHolder instance;

        public static ContextHolder Instance => instance ?? (instance = new ContextHolder());

        public List<DbContext> Contexts { get; } = new List<DbContext>();

        private ContextHolder()
        {

        }
    }
}

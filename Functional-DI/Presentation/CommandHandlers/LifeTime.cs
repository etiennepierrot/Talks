﻿using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Web;

namespace Presentation.CommandHandlers
{
    public class LifeTime : IDisposable
    {
        private readonly ConcurrentDictionary<Type, ThreadLocal<object>> _dependencies = new ConcurrentDictionary<Type, ThreadLocal<object>>(); 

        public TResult PerThread<TResult>(Func<TResult> dependencyFactory) where TResult : class
        {
            ThreadLocal<object> threadLocal = new ThreadLocal<object>(dependencyFactory);
            threadLocal = _dependencies.GetOrAdd(typeof(TResult), threadLocal);

            return (TResult)threadLocal.Value;
        }

        // Only for web Api v2
        public TResult PerWebRequest<TResult>(Func<TResult> dependencyFactory) where TResult : class
        {
            HttpRequestMessage httpRequestMessage = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;

            if (httpRequestMessage == null)
                throw new NotSupportedException("HttpRequestMessage is not supported in this context.");

            object dependency;
            if (!httpRequestMessage.Properties.TryGetValue(typeof(TResult).ToString(), out dependency))
            {
                httpRequestMessage.Properties.Add(typeof(TResult).ToString(), dependencyFactory);
                dependency = dependencyFactory;
            }

            var convertBack = (Func<TResult>)dependency;

            return convertBack();
        }

        public void Dispose()
        {
            foreach (var dep in _dependencies)
            {
                dep.Value.Dispose();
            }
        }
    }
}
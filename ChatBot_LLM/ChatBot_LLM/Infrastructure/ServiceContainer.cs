using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBot_LLM.Infrastructure
{
    /// <summary>
    /// Basit Dependency Injection Container
    /// Servislerin kaydedilmesi ve çözümlenmesi için
    /// </summary>
    public static class ServiceContainer
    {
        private static readonly Dictionary<Type, object> _singletons = new();
        private static readonly Dictionary<Type, System.Func<object>> _factories = new();
        private static readonly object _lockObject = new();

        /// <summary>
        /// Singleton instance kaydeder
        /// </summary>
        public static void RegisterSingleton<TInterface, TImplementation>(TImplementation instance)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            lock (_lockObject)
            {
                _singletons[typeof(TInterface)] = instance;
            }
        }

        /// <summary>
        /// Singleton instance kaydeder (interface olmadan)
        /// </summary>
        public static void RegisterSingleton<T>(T instance) where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            lock (_lockObject)
            {
                _singletons[typeof(T)] = instance;
            }
        }

        /// <summary>
        /// Factory method kaydeder (her çağrıda yeni instance)
        /// </summary>
        public static void RegisterFactory<TInterface>(System.Func<TInterface> factory) where TInterface : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            lock (_lockObject)
            {
                _factories[typeof(TInterface)] = () => factory();
            }
        }

        /// <summary>
        /// Kayıtlı servisi çözümler
        /// </summary>
        public static T Resolve<T>() where T : class
        {
            lock (_lockObject)
            {
                var type = typeof(T);

                // Önce singleton ara
                if (_singletons.TryGetValue(type, out var singleton))
                {
                    return (T)singleton;
                }

                // Sonra factory ara
                if (_factories.TryGetValue(type, out var factory))
                {
                    return (T)factory();
                }

                throw new InvalidOperationException($"'{type.Name}' türü için kayıtlı servis bulunamadı.");
            }
        }

        /// <summary>
        /// Servisin kayıtlı olup olmadığını kontrol eder
        /// </summary>
        public static bool IsRegistered<T>() where T : class
        {
            lock (_lockObject)
            {
                var type = typeof(T);
                return _singletons.ContainsKey(type) || _factories.ContainsKey(type);
            }
        }

        /// <summary>
        /// Tüm kayıtları temizler (test amaçlı)
        /// </summary>
        public static void Clear()
        {
            lock (_lockObject)
            {
                // Disposable singleton'ları temizle
                foreach (var singleton in _singletons.Values)
                {
                    if (singleton is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                _singletons.Clear();
                _factories.Clear();
            }
        }
    }
}

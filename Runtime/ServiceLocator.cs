using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Fraktal.DesignPatterns
{
    /// <summary>
    /// A service locator implementation that provides a centralized registry for application services.
    /// Supports registration, retrieval, and management of services by their type.
    /// </summary>
    /// <remarks>
    /// This implementation uses a dictionary to store service registrations and provides thread-unsafe operations.
    /// All modifications automatically update the read-only view of registrations.
    /// </remarks>
    /// <example>
    /// <code>
    /// var serviceLocator = new ServiceLocator();
    /// serviceLocator.Register&lt;ILogger&gt;(new ConsoleLogger());
    /// 
    /// if (serviceLocator.Get&lt;ILogger&gt;(out var logger))
    /// {
    ///     logger.Log("Service retrieved successfully");
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public class ServiceLocator
    {
        private IDictionary<Type, object> registrations;

        /// <summary>
        /// Initializes a new instance of the ServiceLocator with the specified registrations.
        /// </summary>
        /// <param name="registrations">Initial service registrations dictionary</param>
        /// <exception cref="ArgumentNullException">Thrown when registrations is null</exception>
        public ServiceLocator(IDictionary<Type, object> registrations)
        {
            this.registrations = registrations ?? throw new ArgumentNullException(nameof(registrations));
            Update();
        }

        /// <summary>
        /// Initializes a new empty instance of the ServiceLocator.
        /// </summary>
        public ServiceLocator() : this(new Dictionary<Type, object>())
        {

        }
        
        /// <summary>
        /// Gets a read-only view of all registered services.
        /// </summary>
        /// <remarks>
        /// This view is automatically updated whenever the underlying registrations change.
        /// </remarks>
        /// <value>A read-only dictionary containing all current service registrations</value>
        public IReadOnlyDictionary<Type, object> ReadOnlyRegistrations { get; private set; }
        
        /// <summary>
        /// Gets the number of registered services.
        /// </summary>
        /// <value>The total count of currently registered services</value>
        public int Count => registrations.Count;
        
        /// <summary>
        /// Removes a service registration of the specified type.
        /// </summary>
        /// <param name="t">The type of service to remove</param>
        /// <returns><c>true</c> if the service was successfully removed; otherwise, <c>false</c></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="t"/> is null</exception>
        /// <example>
        /// <code>
        /// bool removed = serviceLocator.Remove(typeof(ILogger));
        /// if (removed)
        /// {
        ///     Debug.Log("Logger service removed successfully");
        /// }
        /// </code>
        /// </example>
        public bool Remove(Type t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            
            bool result = registrations.Remove(t);
            if (result)
                Update();
            return result;
        }
        
        /// <summary>
        /// Registers a service with the specified type.
        /// </summary>
        /// <param name="t">The type to register the service as</param>
        /// <param name="val">The service instance to register</param>
        /// <param name="overwrite">Whether to overwrite existing registration</param>
        /// <returns><c>true</c> if registration succeeded; otherwise, <c>false</c></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="t"/> is null</exception>
        /// <remarks>
        /// This method validates that the provided value is assignable to the specified type.
        /// If validation fails, an error is logged and the method returns <c>false</c>.
        /// </remarks>
        /// <example>
        /// <code>
        /// var logger = new ConsoleLogger();
        /// bool success = serviceLocator.Register(typeof(ILogger), logger, overwrite: true);
        /// </code>
        /// </example>
        public bool Register(Type t, object val, bool overwrite = true)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            
            if (val == null)
            {
                Debug.LogError("Cannot register a null object!");
                return false;
            }
            
            if (!t.IsAssignableFrom(val.GetType()))
            {
                Debug.LogError($"Value of type {val.GetType().Name} is not assignable to {t.Name}");
                return false;
            }
            
            return RegisterForced(t, val, overwrite);
        }
        
        /// <summary>
        /// Registers a service without type compatibility checks.
        /// </summary>
        /// <param name="t">The type to register the service as</param>
        /// <param name="val">The service instance to register</param>
        /// <param name="overwrite">Whether to overwrite existing registration</param>
        /// <returns><c>true</c> if registration succeeded; otherwise, <c>false</c></returns>
        /// <remarks>
        /// <para>This method bypasses type compatibility checks. Use with caution.</para>
        /// <para>When <paramref name="overwrite"/> is <c>true</c>, any existing registration will be replaced.</para>
        /// <para>When <paramref name="overwrite"/> is <c>false</c>, registration fails if the type is already registered.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Use only when you're certain about type compatibility
        /// bool success = serviceLocator.RegisterForced(typeof(IService), serviceInstance, true);
        /// </code>
        /// </example>
        public bool RegisterForced(Type t, object val, bool overwrite = true)
        {
            if (overwrite)
            {
                registrations[t] = val;
                Update();
                return true;
            }
            
            bool added = registrations.TryAdd(t, val);
            if (added)
                Update();
            return added;
        }

        /// <summary>
        /// Registers a service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of service to register</typeparam>
        /// <param name="val">The service instance to register</param>
        /// <param name="overwrite">Whether to overwrite existing registration</param>
        /// <returns><c>true</c> if registration succeeded; otherwise, <c>false</c></returns>
        /// <example>
        /// <code>
        /// var audioService = new AudioManager();
        /// bool success = serviceLocator.Register&lt;IAudioService&gt;(audioService);
        /// </code>
        /// </example>
        public bool Register<T>(T val, bool overwrite = true)
        {
            return Register(typeof(T), val, overwrite);
        }

        /// <summary>
        /// Retrieves a service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve</typeparam>
        /// <param name="val">When this method returns, contains the retrieved service if found; otherwise, <c>default(T)</c></param>
        /// <returns><c>true</c> if the service was found; otherwise, <c>false</c></returns>
        /// <example>
        /// <code>
        /// if (serviceLocator.Get&lt;IAudioService&gt;(out var audioService))
        /// {
        ///     audioService.PlaySound("click");
        /// }
        /// else
        /// {
        ///     Debug.LogWarning("Audio service not found!");
        /// }
        /// </code>
        /// </example>
        public bool Get<T>(out T val)
        {
            bool result = Get(typeof(T), out object value);
            if (!result)
            {
                val = default;
                return false;
            }
            
            val = (T)value;
            return true;
        }

        /// <summary>
        /// Retrieves an existing service of type <typeparamref name="T"/> or creates and registers a new one using the provided factory.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve or create</typeparam>
        /// <param name="result">When this method returns, contains the retrieved or created service if successful; otherwise, <c>default(T)</c></param>
        /// <param name="factory">Function to create a new service instance if needed. Can be <c>null</c>.</param>
        /// <returns><c>true</c> if a service was retrieved or created successfully; otherwise, <c>false</c></returns>
        /// <remarks>
        /// <para>This method first attempts to retrieve an existing service of type <typeparamref name="T"/>.</para>
        /// <para>If no service is found and <paramref name="factory"/> is provided, it creates a new instance and registers it.</para>
        /// <para>Returns <c>false</c> if no service exists, no factory is provided, or registration fails.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Lazy initialization - creates service only if needed
        /// if (serviceLocator.GetOrRegister&lt;IDataService&gt;(out var dataService, () => new DatabaseService()))
        /// {
        ///     dataService.LoadData();
        /// }
        /// 
        /// // Safe retrieval without factory
        /// if (serviceLocator.GetOrRegister&lt;IOptionalService&gt;(out var optionalService, null))
        /// {
        ///     optionalService.DoSomething();
        /// }
        /// </code>
        /// </example>
        public bool GetOrRegister<T>(out T result, Func<T> factory)
        {
            if (Get<T>(out result))
            {
                return true;
            }

            if (factory == null) 
            {
                result = default;
                return false;
            }
            
            result = factory();
            if (!Register(typeof(T), result))
            {
                result = default;
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Retrieves an existing service of the specified type or creates and registers a new one using the provided factory.
        /// </summary>
        /// <param name="type">The type of service to retrieve or create</param>
        /// <param name="result">When this method returns, contains the retrieved or created service if successful; otherwise, <c>null</c></param>
        /// <param name="factory">Function to create a new service instance if needed</param>
        /// <returns><c>true</c> if a service was retrieved or created successfully; otherwise, <c>false</c></returns>
        /// <remarks>
        /// <para>This is the non-generic version of <see cref="GetOrRegister{T}(out T, Func{T})"/>.</para>
        /// <para>Returns <c>false</c> if <paramref name="type"/> or <paramref name="factory"/> is null, or if registration fails.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var serviceType = typeof(IDataService);
        /// if (serviceLocator.GetOrRegister(serviceType, out object service, () => new DatabaseService()))
        /// {
        ///     var dataService = (IDataService)service;
        ///     dataService.LoadData();
        /// }
        /// </code>
        /// </example>
        public bool GetOrRegister(Type type, out object result, Func<object> factory)
        {
            result = null;
            
            if (type == null)
            {
                Debug.LogError("Cannot retrieve service with null type");
                return false;
            }
            
            if (factory == null)
            {
                Debug.LogError("Cannot create service with null factory");
                return false;
            }
            
            if (Get(type, out result))
            {
                return true;
            }
            
            result = factory();
            if (!Register(type, result))
            {
                result = null;
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Retrieves a service by type.
        /// </summary>
        /// <param name="t">The type of service to retrieve</param>
        /// <param name="val">When this method returns, contains the retrieved service if found; otherwise, <c>null</c></param>
        /// <returns><c>true</c> if the service was found; otherwise, <c>false</c></returns>
        /// <example>
        /// <code>
        /// if (serviceLocator.Get(typeof(ILogger), out object logger))
        /// {
        ///     ((ILogger)logger).Log("Service found");
        /// }
        /// </code>
        /// </example>
        public bool Get(Type t, out object val)
        {
            return registrations.TryGetValue(t, out val);
        }

        /// <summary>
        /// Removes all service registrations from the locator.
        /// </summary>
        /// <remarks>
        /// This method clears all registered services and updates the read-only view.
        /// Use this when reinitializing the service locator or cleaning up resources.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Cleanup all services
        /// serviceLocator.Clear();
        /// Debug.Log($"Services remaining: {serviceLocator.Count}"); // Output: 0
        /// </code>
        /// </example>
        public void Clear()
        {
            registrations.Clear();
            Update();
        }

        /// <summary>
        /// Updates the read-only view of registrations.
        /// </summary>
        /// <remarks>
        /// This method is called automatically after any modification to ensure 
        /// the <see cref="ReadOnlyRegistrations"/> property reflects current state.
        /// </remarks>
        private void Update()
        {
            ReadOnlyRegistrations = new ReadOnlyDictionary<Type, object>(registrations);
        }
        
    }
}
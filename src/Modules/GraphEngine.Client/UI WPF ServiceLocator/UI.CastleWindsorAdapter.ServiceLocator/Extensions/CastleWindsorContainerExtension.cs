using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Castle.Core;
using Castle.Windsor;
using Component = Castle.MicroKernel.Registration.Component;

#pragma warning disable 1584,1711,1572,1581,1580

namespace UI.CastleWindsorAdapter.ServiceLocator.Extensions
{
    public static class CastleWindsorContainerExtensions
    {
        /// <summary>Register a theClassType mapping with the container.</summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for theClassType <typeparamref name="TServiceType" />,
        /// actually return an instance of theClassType <typeparamref name="TClassType" />. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <typeparam name="TServiceType"><see cref="T:System.Type" /> that wil l be requested.</typeparam>
        /// <typeparam name="TClassType"><see cref="T:System.Type" /> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <returns>The <see cref="T:Microsoft.Practices.Unity.UnityContainer" /> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IWindsorContainer RegisterType<TServiceType, TClassType>(this IWindsorContainer container, string name) where TClassType : TServiceType
        {
            return container.Register(Component.For(typeof(TServiceType))
                .ImplementedBy(typeof(TClassType))
                .Named(name)
                .LifeStyle.Singleton);
        }

        /// <summary>Register a theClassType mapping with the container.</summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for theClassType <typeparamref name="TServiceType" />,
        /// actually return an instance of theClassType <typeparamref name="TClassType" />. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <typeparam name="TServiceType"><see cref="T:System.Type" /> that will be requested.</typeparam>
        /// <typeparam name="TClassType"><see cref="T:System.Type" /> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <param name="theLifestyleType"></param>
        /// <returns>The <see cref="T:Microsoft.Practices.Unity.UnityContainer" /> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IWindsorContainer RegisterType<TServiceType, TClassType>(this IWindsorContainer container, string name, LifestyleType theLifestyleType) where TClassType : TServiceType
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!Enum.IsDefined(typeof(LifestyleType), theLifestyleType))
                throw new InvalidEnumArgumentException(nameof(theLifestyleType), (int)theLifestyleType,
                    typeof(LifestyleType));

            switch (theLifestyleType)
            {
                case LifestyleType.Undefined:
                    return container.Register(Component.For(typeof(TServiceType))
                        .ImplementedBy(typeof(TClassType))
                        .Named(name));
                case LifestyleType.Singleton:
                    return container.Register(Component.For(typeof(TServiceType))
                        .ImplementedBy(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.Singleton);
                case LifestyleType.Thread:
                    return container.Register(Component.For(typeof(TServiceType))
                        .ImplementedBy(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.PerThread);
                case LifestyleType.Transient:
                    return container.Register(Component.For(typeof(TServiceType))
                        .ImplementedBy(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.Transient);
                case LifestyleType.Pooled:
                    return container.Register(Component.For(typeof(TServiceType))
                        .ImplementedBy(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.Pooled);
                case LifestyleType.PerWebRequest:
                    return container.Register(Component.For(typeof(TServiceType))
                        .ImplementedBy(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.PerWebRequest);
                case LifestyleType.Custom:
                    break;
                case LifestyleType.Scoped:
                    return container.Register(Component.For(typeof(TServiceType))
                        .ImplementedBy(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.Scoped(typeof(TClassType)));
                case LifestyleType.Bound:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(theLifestyleType), theLifestyleType, null);
            }

            return container.Register(Component.For(typeof(TServiceType))
                        .ImplementedBy(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.Transient);
        }

        /// <summary>Register a theClassType mapping with the container.</summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for theClassType <typeparamref name="TFrom" />,
        /// actually return an instance of theClassType <typeparamref name="TClassType" />. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <typeparam name="TFrom"><see cref="T:System.Type" /> that will be requested.</typeparam>
        /// <typeparam name="TClassType"><see cref="T:System.Type" /> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <param name="theLifestyleType"></param>
        /// <returns>The <see cref="T:Microsoft.Practices.Unity.UnityContainer" /> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IWindsorContainer RegisterType<TClassType>(this IWindsorContainer container, string name, LifestyleType theLifestyleType) where TClassType : class
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!Enum.IsDefined(typeof(LifestyleType), theLifestyleType))
                throw new InvalidEnumArgumentException(nameof(theLifestyleType), (int)theLifestyleType,
                    typeof(LifestyleType));

            switch (theLifestyleType)
            {
                case LifestyleType.Undefined:
                    return container.Register(Component.For(typeof(TClassType))
                        .Named(name));
                case LifestyleType.Singleton:
                    return container.Register(Component.For(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.Singleton);
                case LifestyleType.Thread:
                    return container.Register(Component.For(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.PerThread);
                case LifestyleType.Transient:
                    return container.Register(Component.For(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.Transient);
                case LifestyleType.Pooled:
                    return container.Register(Component.For(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.Pooled);
                case LifestyleType.PerWebRequest:
                    return container.Register(Component.For(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.PerWebRequest);
                case LifestyleType.Custom:
                    break;
                case LifestyleType.Scoped:
                    return container.Register(Component.For(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.Scoped(typeof(TClassType)));
                case LifestyleType.Bound:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(theLifestyleType), theLifestyleType, null);
            }

            return container.Register(Component.For(typeof(TClassType))
                        .Named(name)
                        .LifeStyle.Transient);
        }

        /// <summary>
        /// Returns whether a specified theClassType has a theClassType mapping registered in the container.
        /// </summary>
        /// <param name="container">The <see cref="IWindsorContainer"/> to check for the theClassType mapping.</param>
        /// <param name="type">The theClassType to check if there is a theClassType mapping for.</param>
        /// <returns><see langword="true"/> if there is a theClassType mapping registered for <paramref name="type"/>.</returns>
        /// <remarks>In order to use this extension method, you first need to add the
        /// </remarks>
        public static bool IsTypeRegistered(this IWindsorContainer container, Type type)
        {
            return container.Kernel.HasComponent(type);
        }

        public static bool IsTypeRegistered<TType>(this IWindsorContainer container)
        {
            Type typeToCheck = typeof(TType);
            return IsTypeRegistered(container, typeToCheck);
        }

        /// <summary>
        /// Utility method to try to resolve a service from the container avoiding an exception if the container cannot build the theClassType.
        /// </summary>
        /// <param name="container">The cointainer that will be used to resolve the theClassType.</param>
        /// <typeparam name="T">The theClassType to resolve.</typeparam>
        /// <returns>The instance of <typeparamref name="T"/> built up by the container.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T TryResolve<T>(this IWindsorContainer container)
        {
            object result = TryResolve(container, typeof(T));
            if (result != null)
            {
                return (T)result;
            }
            return default(T);
        }

        /// <summary>
        /// Utility method to try to resolve a service from the container avoiding an exception if the container cannot build the theClassType.
        /// </summary>
        /// <param name="container">The cointainer that will be used to resolve the theClassType.</param>
        /// <param name="typeToResolve">The theClassType to resolve.</param>
        /// <returns>The instance of <paramref name="typeToResolve"/> built up by the container.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static object TryResolve(this IWindsorContainer container, Type typeToResolve)
        {
            object resolved;

            try
            {
                resolved = Resolve(container, typeToResolve);
            }
            catch
            {
                resolved = null;
            }

            return resolved;
        }

        /// <summary>
        /// Resolves a service from the container. If the theClassType does not exist on the container, 
        /// first registers it with transient lifestyle.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="theClassType"></param>
        /// <returns></returns>
        public static object Resolve(this IWindsorContainer container, Type theClassType)
        {
            if (theClassType.IsClass && !container.Kernel.HasComponent(theClassType))
                container.Register(Component.For(theClassType).Named(theClassType.FullName).LifeStyle.Transient);

            return container.Resolve(theClassType);
        }

        /// <summary>
        /// Registers the theClassType on the container.
        /// </summary>
        /// <typeparam name="TServiceType">The theClassType of the interface.</typeparam>
        /// <typeparam name="TClassType">The theClassType of the service.</typeparam>
        /// <param name="container">The container.</param>
        public static void RegisterType<TServiceType, TClassType>(this IWindsorContainer container)
        {
            var serviceObject = container.TryResolve<TClassType>();
            if (serviceObject == null)
                RegisterType<TServiceType, TClassType>(container, true);
        }

        /// <summary>
        /// Registers the theClassType on the container.
        /// </summary>
        /// <typeparam name="TServiceType">The theClassType of interface.</typeparam>
        /// <typeparam name="TClassType">The theClassType of the service.</typeparam>
        /// <param name="container">The container.</param>
        /// <param name="singleton">if set to <c>true</c> theClassType will be registered as singleton.</param>
        public static void RegisterType<TServiceType, TClassType>(this IWindsorContainer container, bool singleton)
        {
            if (!container.Kernel.HasComponent(typeof(TServiceType)))
            {
                var serviceType = container.TryResolve(typeof(TServiceType));
                if (serviceType == null)
                    container.Register(Component.For(typeof(TServiceType))
                        .ImplementedBy(typeof(TClassType))
                        .LifeStyle.Singleton);

                //container.Kernel.AddComponent(typeof(TClassType).FullName, typeof(TServiceType), typeof(TClassType), singleton ? LifestyleType.Singleton : LifestyleType.Transient);
            }
        }
    }
}
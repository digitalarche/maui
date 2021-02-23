﻿using System;
using System.Collections.Generic;

namespace Xamarin.Platform.Hosting
{

	class MauiServiceProvider : IMauiServiceProvider
	{
		IDictionary<Type, Func<IServiceProvider, object?>?> _collection;

		public MauiServiceProvider(IMauiServiceCollection collection)
		{
			_collection = collection ?? throw new ArgumentNullException(nameof(collection));
		}

		public object? GetService(Type serviceType)
		{
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));

			List<Type> types = new List<Type> { serviceType };
			foreach (var interfac in serviceType.GetInterfaces())
			{
				if (typeof(IView).IsAssignableFrom(interfac))
					types.Add(interfac);
			}

			Type? baseType = serviceType.BaseType;

			while (baseType != null)
			{
				types.Add(baseType);
				baseType = baseType.BaseType;
			}

			foreach (var type in types)
			{
				if (_collection != null && _collection.ContainsKey(type))
				{
					var typeInstance = _collection[type]?.Invoke(this);
					return typeInstance;
				}
			}

			return default!;
		}
	}
}

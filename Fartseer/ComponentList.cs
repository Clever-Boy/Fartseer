using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartseer.Components;

namespace Fartseer
{
	public class ComponentList
	{
		List<Type> contained;

		public ComponentList()
		{
			contained = new List<Type>();
		}

		public System.Collections.ObjectModel.ReadOnlyCollection<Type> GetReadOnlyList()
		{
			return contained.AsReadOnly();
		}

		public ComponentList Add<T>()
		{
			contained.Add(typeof(T));
			return this;
		}

		public bool Contains<T>()
		{
			return contained.Contains(typeof(T));
		}
		public bool Contains(Type t)
		{
			return contained.Contains(t);
		}
	}
}

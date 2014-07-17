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

		public System.Collections.ObjectModel.ReadOnlyCollection<Type> AsReadOnly()
		{
			return contained.AsReadOnly();
		}

		public ComponentList Add<T>() where T : GameComponent
		{
			contained.Add(typeof(T));
			return this;
		}

		public bool Contains<T>() where T : GameComponent
		{
			return contained.Contains(typeof(T));
		}
		public bool Contains(Type t)
		{
			return contained.Contains(t);
		}
	}
}

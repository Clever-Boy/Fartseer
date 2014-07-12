using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Fartseer.Components
{
	public enum ComponentEvent
	{
		ChildComponentAdded,
		ChildComponentRemoving,
		Removing,
		Initialized
	}

	public enum ComponentRestriction
	{
		Either,
		Normal, // TODO: maybe make something else than "Normal"
		Drawable
	}

	public class ComponentEventArgs : EventArgs
	{
		public ComponentEvent Event { get; private set; }
		public object[] Data { get; private set; }

		public ComponentEventArgs(ComponentEvent componentEvent, params object[] data)
		{
			Event = componentEvent;
			Data = data;
		}
	}

	public abstract class GameComponent
	{
		public bool Enabled { get; set; }
		public List<GameComponent> Components { get; private set; }
		public GameComponent Parent { get; private set; }
		public Game Game { get; protected set; }

		public event EventHandler<ComponentEventArgs> ChildComponentAdded;
		public event EventHandler<ComponentEventArgs> ChildComponentRemoving;
		public event EventHandler<ComponentEventArgs> Removing;
		public event EventHandler<ComponentEventArgs> Initialized;

		protected int initIndex;
		public int initPriority; // components are initialized in descending order of init priorities
		protected ComponentRestriction parentRestriction;

		public GameComponent(int initPriority)
		{
			this.initPriority = initPriority;

			Components = new List<GameComponent>();
			Enabled = true;
			parentRestriction = ComponentRestriction.Either;
			//Console.WriteLine("{0} {1}", this, this is DrawableGameComponent);
		}

		public string GetComponentTypeString()
		{
			if (this is DrawableGameComponent)
				return "DrawableGameComponent";
			else
				return "GameComponent";
		}

		// supposed to be overridden by component
		protected virtual List<GameComponent> GetInitComponents()
		{
			return new List<GameComponent>();
		}

		// used by AddComponent, makes sure the component is initialized correctly
		public bool DoInit(int initIndex)
		{
			Console.WriteLine("{0} initializing\n\tInit priority: {1}, init index: {2}\n\tParent restriction: {3}",
				this.GetType().Name, initPriority, initIndex, parentRestriction);
			this.initIndex = initIndex;

			if (!InitComponents())
				return false;

			if (!Init())
				return false;

			return true;
		}

		bool InitComponents()
		{
			// get init components (method above), sort them by init priority and add them
			List<GameComponent> initComponents = GetInitComponents();
			initComponents = initComponents.OrderByDescending(c => c.initPriority).ToList();
			foreach (GameComponent component in initComponents)
				if (!AddComponent(component))
					return false;

			return true;
		}

		// init should never be called by something else than DoInit (above)
		protected virtual bool Init()
		{
			if (!parentRestriction.Matches(Parent))
			{
				Console.WriteLine("{0} component's parent's type does not match parent type restriction (Parent: {1}, Restriction: {2}",
					this.GetType().Name, Parent.GetComponentTypeString(), parentRestriction);
				return false;
			}

			if (Initialized != null)
				Initialized(this, new ComponentEventArgs(ComponentEvent.Initialized));

			return true;
		}

		public virtual void Update(double frametime)
		{
			if (Enabled)
				foreach (GameComponent component in Components)
					component.Update(frametime);
		}

		public bool AddComponent(GameComponent component)
		{
			component.Parent = this;
			component.Game = Game;
			if (!component.DoInit(initIndex + 1))
			{
				Console.WriteLine("{0} component failed to initialize and will not be added to {1} component", component.GetType().Name, this.GetType().Name);
				return false;
			}
			Components.Add(component);

			if (ChildComponentAdded != null)
				ChildComponentAdded(this, new ComponentEventArgs(ComponentEvent.ChildComponentAdded, component));

			return true;
		}

		public void RemoveComponent(GameComponent component)
		{
			if (Components.Contains(component))
			{
				if (ChildComponentRemoving != null)
					ChildComponentRemoving(this, new ComponentEventArgs(ComponentEvent.ChildComponentRemoving, component));

				Components.Remove(component);
			}
		}

		public void Remove()
		{
			if (Removing != null)
				Removing(this, new ComponentEventArgs(ComponentEvent.Removing));

			Parent.RemoveComponent(this);
		}

		public T GetComponent<T>()
		{
			bool failed;
			return GetComponent<T>(out failed, (c) => { return true; });
		}
		public T GetComponent<T>(out bool failed)
		{
			// calls GetComponent with condition that will always be true
			return GetComponent<T>(out failed, (c) => { return true; });
		}
		public T GetComponent<T>(out bool failed, Func<GameComponent, bool> condition)
		{
			failed = false;
			List<T> components = GetComponents<T>(out failed, condition);
			if (components.Count < 1)
			{
				failed = true;
				return default(T);
			}
			return components[0];
		}

		public List<T> GetComponents<T>()
		{
			bool failed;
			return GetComponents<T>(out failed, (c) => { return true; });
		}
		public List<T> GetComponents<T>(out bool failed)
		{
			return GetComponents<T>(out failed, (c) => { return true; });
		}
		public List<T> GetComponents<T>(out bool failed, Func<GameComponent, bool> condition)
		{
			// this works since all items in the result list are of type T
			// else the conversion would fail
			List<T> components = Components.FindAll((c) => { return c is T && condition(c) == true; }).ConvertAll(new Converter<GameComponent, T>((t) => { return (T)Convert.ChangeType(t, typeof(T)); }));
			failed = components.Count < 1;
			return components;
		}

		public bool ContainsComponent<T>()
		{
			// http://stackoverflow.com/questions/8216881/how-do-i-check-if-a-list-contains-an-object-of-a-certain-type-c-sharp
			return Components.OfType<T>().Any();
		}
	}

	public abstract class DrawableGameComponent : GameComponent, Drawable
	{
		public bool Visible { get; set; }
		public virtual Vector2f Position { get; set; }

		public DrawableGameComponent(int initPriority)
			: base(initPriority)
		{
			Visible = true;
		}

		public virtual void Draw(RenderTarget target, RenderStates states)
		{
			if (Visible)
			{
				foreach (GameComponent component in Components)
				{
					// maybe not best implementation
					if (component is DrawableGameComponent)
						((DrawableGameComponent)component).Draw(target, states);
				}
			}
		}
	}
}

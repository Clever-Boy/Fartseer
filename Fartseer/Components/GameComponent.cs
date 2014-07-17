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

	public class ComponentFindResult
	{
		public bool Failed { get; set; }
		public List<string> FailedComponents { get; set; }

		public ComponentFindResult()
		{
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
			System.Diagnostics.Stopwatch initTimer = System.Diagnostics.Stopwatch.StartNew();
			Console.WriteLine("{0} initializing\n\tInit priority: {1}, init index: {2}\n\tParent restriction: {3}",
				this.GetType().Name, initPriority, initIndex, parentRestriction);
			this.initIndex = initIndex;

			if (!InitComponents())
				return false;

			if (!Init())
				return false;

			initTimer.Stop();
			Console.WriteLine("{0} initializing took {1} ms", this.GetType().Name, initTimer.Elapsed.TotalMilliseconds);
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

		public T GetComponent<T>() where T : GameComponent
		{
			ComponentFindResult result;
			return GetComponent<T>(out result, (c) => { return true; });
		}
		public T GetComponent<T>(out ComponentFindResult result) where T : GameComponent
		{
			// calls GetComponent with condition that will always be true
			return GetComponent<T>(out result, (c) => { return true; });
		}
		public T GetComponent<T>(out ComponentFindResult result, Func<T, bool> condition) where T : GameComponent
		{
			result = new ComponentFindResult();
			T component = Components.Find(c => c is T && condition(c as T)) as T;
			result.Failed = component == null;
			result.FailedComponents = new List<string>() { typeof(T).Name };
			return component;
		}

		public List<GameComponent> GetComponents(ComponentList components)
		{
			ComponentFindResult result;
			return GetComponents(components, out result, c => true);
		}
		public List<GameComponent> GetComponents(ComponentList components, out ComponentFindResult result)
		{
			return GetComponents(components, out result, c => true);
		}
		public List<GameComponent> GetComponents(ComponentList components, out ComponentFindResult result, Func<GameComponent, bool> condition)
		{
			result = new ComponentFindResult();
			result.Failed = false;
			result.FailedComponents = new List<string>();
			List<GameComponent> list = Components.FindAll((c) =>
			{
				if (!components.Contains(c.GetType()))
					return false;

				if (!condition(c))
					return false;

				return true;
			});

			foreach (Type t in components.AsReadOnly())
				if (!list.Any(c => c.GetType() == t))
				{
					result.FailedComponents.Add(t.Name);
					result.Failed = true;
				}

			return list;
		}

		public bool ContainsComponent<T>() where T : GameComponent
		{
			// source: http://stackoverflow.com/questions/8216881/how-do-i-check-if-a-list-contains-an-object-of-a-certain-type-c-sharp
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

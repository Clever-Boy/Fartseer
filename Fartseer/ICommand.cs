using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartseer.Components;

namespace Fartseer
{
	public interface ICommand
	{
		Game Game { get; set; }
		bool TryExecute(Actor target);
	}
}

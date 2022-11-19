using System;
using System.Collections.Generic;

namespace Core.FsmUtil
{
	/// <summary>
	/// Handle for FSM Hooks, created via <see cref="Hooks"/>.
	/// </summary>
	/// <typeparam name="T">Action type</typeparam>
	public sealed class FSMHookHandle<T> where T : Delegate
	{
		private readonly Dictionary<FSMData, T> dict;
		private readonly FSMData data;
		private readonly T action;

		internal FSMHookHandle(Dictionary<FSMData, T> dict, FSMData data, T action)
		{
			this.dict = dict;
			this.data = data;
			this.action = action;
		}

		/// <summary>
		/// Enables the hook.
		/// </summary>
		public void Enable()
		{
			if (!dict.ContainsKey(data))
			{
				dict.Add(data, action);
			}
			else
			{
				dict[data] = (T) Delegate.Combine(dict[data], action);
			}
		}

		/// <summary>
		/// Disables the hook.
		/// </summary>
		public void Disable()
		{
			if (dict.ContainsKey(data))
			{
				dict[data] = (T) Delegate.Remove(dict[data], action);
			}
		}
	}
}

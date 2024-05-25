using System;
using System.Runtime.CompilerServices;

namespace IngameScript {
	public interface IState<TState> {
		TState Name { get; }
		Action OnEnter { get; }
		Action<StateMachine<TState>> OnUpdate { get; }
		Action OnLeave { get; }
	}

	public class State<TState> : IState<TState> {
		public TState Name { get; }
		public Action OnEnter { get; }

		public Action<StateMachine<TState>> OnUpdate { get; }

		public Action OnLeave { get; }

		public State( TState name, Action onEnter, Action<StateMachine<TState>> onUpdate, Action onLeave ) {
			Name = name;
			OnEnter = onEnter;
			OnUpdate = onUpdate;
			OnLeave = onLeave;
		}
	}
}

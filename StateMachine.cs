using System;
using System.Collections.Generic;

namespace IngameScript {
	public class StateMachine<TState> {
		private Dictionary<TState, IState<TState>> m_states = new Dictionary<TState, IState<TState>>();
		private IState<TState> m_nextState { get; set; } = null;
		private IState<TState> m_currentState { get; set; } = null;
		private bool m_isStarted = false;

		private IState<TState> FindState(TState stateName) {
			IState<TState> state;

			if ( m_states.TryGetValue( stateName, out state ) ) {
				return state;
			}

			return null;
		}

		public void AddStates(params IState<TState>[] states) {
			foreach ( var state in states ) {
				m_states.Add( state.Name, state );
			}
		}

		public void StartWithState( TState stateName ) {
			SwitchState( stateName );
			m_isStarted = true;
		}

		public void Stop() {
			m_isStarted = false;
		}

		public void SwitchState( TState stateName ) {
			m_nextState = FindState( stateName );
		}

		public void Update() {
			if (!m_isStarted ) {
				return;
			}

			if (m_nextState != null) {
				if (m_currentState != null) {
					m_currentState.OnLeave?.Invoke();
					m_currentState = null;
				} else {
					m_currentState = m_nextState;
					m_currentState.OnEnter?.Invoke();
					m_nextState = null;
				}
			} else {
				if ( m_currentState != null ) {
					m_currentState.OnUpdate?.Invoke( this );
				}
			}
		}
	}
}

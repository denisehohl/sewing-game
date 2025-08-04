using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Ateo.StateManagement
{
    public static class StateHelper
    {
#if STATEMACHINE
        private static Dictionary<string, IState> _stringToState;
        private static Dictionary<string, StatesEnum> _stringToEnum;
        
        private static Dictionary<StatesEnum, IState> _enumToState;
        private static Dictionary<IState, StatesEnum> _stateToEnum;

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			_stringToState = null;
			_stringToEnum = null;
			_enumToState = null;
			_stateToEnum = null;
		}
#endif

        public static List<IState> GetStates()
        {
            UpdateDictionaries();
            return _stringToState.Values.ToList();
        }
        
        public static List<string> GetStateNames()
        {
            UpdateDictionaries();
            return _stringToState.Keys.ToList();
        }

        public static IState GetState(string name)
        {
            if (_stringToState == null)
                UpdateDictionaries();
            
            return _stringToState != null && _stringToState.TryGetValue(name, out IState state) ? state : default;
        }

        public static IState GetState(StatesEnum stateEnum)
        {
            if (_enumToState == null)
                UpdateDictionaries();
            
            return _enumToState != null && _enumToState.TryGetValue(stateEnum, out IState state) ? state : default;
        }

        public static StatesEnum GetStatesEnum(string name)
        {
            if (_stringToEnum == null)
                UpdateDictionaries();
            
            return _stringToEnum != null && !string.IsNullOrEmpty(name) && _stringToEnum.TryGetValue(name, out StatesEnum statesEnum) ? statesEnum : default;
        }

        public static StatesEnum GetStatesEnum(IState state)
        {
            if (_stringToEnum == null)
                UpdateDictionaries();
            
            return _stateToEnum != null && state != null && _stateToEnum.TryGetValue(state, out StatesEnum statesEnum) ? statesEnum : default;
        }
        
        public static void UpdateDictionaries()
        {
            _stringToEnum = new Dictionary<string, StatesEnum>();
            _stringToState = new Dictionary<string, IState>();
            
            _enumToState = new Dictionary<StatesEnum, IState>();
            _stateToEnum = new Dictionary<IState, StatesEnum>();
            
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                IEnumerable<Type> types = assembly.GetTypes()
                    .Where(type => type.GetInterfaces().Contains((typeof(IState))))
                    .Select(x => new {x, y = x.BaseType})
                    .Where(@t => !@t.x.IsAbstract && !@t.x.IsInterface && @t.y != null && @t.y.IsGenericType)
                    .Select(@t => @t.x);

                foreach (Type type in types)
                {
                    object instance = type.GetProperty("Instance", (BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static))
                        ?.GetValue(null, null);

                    if (instance is IState state)
                    {
                        _stringToState.Add(state.Name, state);
                        
                        if (Enum.TryParse<StatesEnum>(state.Name, true, out StatesEnum statesEnum))
                        {
                            _enumToState.Add(statesEnum, state);
                            _stateToEnum.Add(state, statesEnum);
                            _stringToEnum.Add(state.Name, statesEnum);
                        }
                    }
                }
            }
        }
#endif
    }
}
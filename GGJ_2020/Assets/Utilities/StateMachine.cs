using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Attributes.AlwaysRepaint]
public class StateMachine : MonoBehaviour
{
    [SerializeField] State defaultState = null;
    public IState currentState { get; private set; }

    private void Start()
    {
        if (!defaultState)
        {
            Debug.LogError($"{gameObject.transform.root.name}'s stateMachine has no initial state. Disabling State Machine", this);
            enabled = false;
            return;
        }
        currentState = defaultState;
        currentState.OnEnter();
    }

    public void ChangeState(IState next)
    {
        if (currentState != next)
        {
            currentState.OnExit();
            if (next == null)
                currentState = defaultState;
            else currentState = next;
            currentState.OnEnter();
        }
    }

    private void OnEnable()
    {
        StateMachineManager.Add(this);
    }

    private void OnDisable()
    {
        StateMachineManager.Remove(this);
    }

    int position;

    class StateMachineManager : GameSystem, Events.IOnUpdate, Events.IOnInspect
    {
        static List<StateMachine> stateMachines = new List<StateMachine>();

        public void OnUpdate(float deltaTime)
        {
            foreach (var statemachine in stateMachines)
                statemachine.ChangeState(statemachine.currentState.OnUpdate(deltaTime));
        }

        public static void Add(StateMachine stateMachine)
        {
            stateMachine.position = stateMachines.Count;
            stateMachines.Add(stateMachine);
        }

        public static void Remove(StateMachine stateMachine)
        {
            var last = stateMachines[stateMachines.Count - 1];
            last.position = stateMachine.position;
            stateMachines[last.position] = last;
            stateMachines.RemoveAt(stateMachines.Count - 1);
        }

        public void OnInspect()
        {
            GUILayout.Label($"Statemachine Count : {stateMachines.Count}");
        }
    }
}

#if UNITY_EDITOR
namespace Inspectors
{
    class DrawStateMachine : DrawClass<StateMachine>
    {
        public override void Draw()
        {
            var State = (target as StateMachine).currentState as State;

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Current State", GUILayout.Width(UnityEditor.EditorGUIUtility.labelWidth));

                using (new GUILayout.VerticalScope())
                {
                    while (State != null)
                    {
                        GUILayout.Label(State.GetType().Name);
                        State = State.currentState as State;
                    }
                }
            }
            base.Draw();
        }
    }
}
#endif


public abstract class State : MonoBehaviour, IState
{
    protected virtual void OnEnter() { }
    protected virtual IState OnUpdate(float deltaTime, float stateTime) => null;
    protected virtual void OnExit() { }

    [SerializeField] protected State DefaultSubState = null;

    public IState currentState { get; private set; }
    
    void IState.OnEnter()
    {
        OnEnter();
        currentState = DefaultSubState;
        currentState?.OnEnter();
    }

    void IState.OnExit()
    {
        currentState?.OnExit();
        OnExit();
        stateTime = 0;
    }

    float stateTime;
    IState IState.OnUpdate(float deltaTime)
    {
        stateTime += deltaTime;
        IState next = currentState?.OnUpdate(Time.deltaTime);
        if (next != currentState)
        {
            currentState?.OnExit();

            if (next == null)
                currentState = DefaultSubState;
            else currentState = next;

            currentState?.OnEnter();
        }        
        return OnUpdate(deltaTime, stateTime);
    }
}

public interface IState
{
    void OnEnter();
    IState OnUpdate(float deltaTime);
    void OnExit();
}
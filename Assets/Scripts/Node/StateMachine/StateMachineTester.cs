using UnityEngine;

public class StateMachineTester : MonoBehaviour
{
    public TestStateMachineGraph _graph;
    void Update()
    {
        _graph.Update();
    }
}

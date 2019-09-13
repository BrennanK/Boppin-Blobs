namespace BehaviorTree {
    /// <summary>
    /// Conditions are leaf nodes and it is the primary way the tree can check information in the world.
    /// They rely on the tree's return statuses (SUCCESS and FAILURE) to signal true and false
    /// </summary>
    public abstract class Condition : Behavior {

    }
}
